﻿using System;
using System.Globalization;
using System.Text.RegularExpressions;

using FileCabinetApp.Enums;
using FileCabinetApp.Services;

namespace FileCabinetApp
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Olga Kripulevich";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private static readonly DateTime MinDate = new DateTime(1950, 1, 1);
        private static readonly string NamePattern = @"^[a-zA-Z '.-]*$";

        private static bool isRunning = true;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("find", FindByParameter),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "shows statistics by records", "The 'stat' command shows statistics by records" },
            new string[] { "create", "creates a new record", "The 'create' command creates a new record." },
            new string[] { "edit <ID>", "edits an existing entry", "The 'edit' command edits an existing entry." },
            new string[] { "list", "returns a list of records added to the service", "The 'list' command returns a list of records added to the service." },
            new string[] { "find <parameter name> <parameter value>", "returns a list of records with the given parameter", "The 'find firstname' command returns a list of records with the given parameter." },
        };

        /// <summary>
        /// The file cabinet service.
        /// </summary>
        private static FileCabinetService fileCabinetService;

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        public static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();
            string validationRules = "default";

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Contains("--"))
                {
                    var parameters = args[i].Split('=');
                    if (parameters[0].Equals("--validation-rules"))
                    {
                        validationRules = parameters[1].ToLower();
                    }
                }
                else if (args[i].StartsWith('-'))
                {
                    validationRules = args[2].ToLower();
                }
            }

            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine($"Using {validationRules} validation rules.");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            if (validationRules.Equals("custom"))
            {
                fileCabinetService = new FileCabinetCustomService();
            }
            else
            {
                fileCabinetService = new FileCabinetDefaultService();
            }

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            var (firstName, lastName, dateOfBirth, gender, status, catsCount, catsBudget) = ParameterEntry();

            int recordId = default(int);
            try
            {
                Record record = new Record(firstName, lastName, dateOfBirth, gender, status, catsCount, catsBudget);
                recordId = fileCabinetService.CreateRecord(record);
                Console.WriteLine($"Record #{recordId} is created.");
            }
            catch (ArgumentNullException anex)
            {
                Console.WriteLine($"Record wasn't created. {anex.Message}");
                Console.WriteLine(Program.HintMessage);
            }
            catch (ArgumentException aex)
            {
                Console.WriteLine($"Record wasn't created. {aex.Message}");
                Console.WriteLine(Program.HintMessage);
            }
        }

        private static void Edit(string parameters)
        {
            int.TryParse(parameters, out int id);
            if (id == 0)
            {
                Console.WriteLine($"The '{parameters}' isn't an ID.");
                return;
            }

            if (fileCabinetService.IsThereARecordWithThisId(id, out int index))
            {
                var (firstName, lastName, dateOfBirth, gender, status, catsCount, catsBudget) = ParameterEntry();
                try
                {
                    Record record = new Record(firstName, lastName, dateOfBirth, gender, status, catsCount, catsBudget);
                    fileCabinetService.EditRecord(id, record);
                }
                catch (ArgumentNullException anex)
                {
                    Console.WriteLine("Record wasn't edited.", anex.Message);
                    Console.WriteLine(Program.HintMessage);
                }
                catch (ArgumentException aex)
                {
                    Console.WriteLine("Record wasn't edited.", aex.Message);
                    Console.WriteLine(Program.HintMessage);
                }
            }
            else
            {
                Console.WriteLine($"#{id} record is not found.");
            }
        }

        private static void FindByParameter(string parameters)
        {
            try
            {
                var findList = fileCabinetService.Find(parameters);
                Print(findList);
            }
            catch (InvalidOperationException ioex)
            {
                Console.WriteLine("The record didn't find.", ioex.Message);
            }
            catch (ArgumentException aex)
            {
                Console.WriteLine("The record didn't find.", aex.Message);
            }
        }

        private static (string firstName, string lastName, DateTime dateOfBirth, Gender gender, char status, short catsCount, decimal catsBudget) ParameterEntry()
        {
            var firstAndLastName = new CultureInfo("ru-RU").TextInfo;

            bool flag = true;
            string firstName = default(string);
            while (flag)
            {
                Console.WriteLine("First name: ");
                firstName = Console.ReadLine();
                if (firstName != null && firstName.Length > 2 && firstName.Length < 60 && Regex.IsMatch(firstName, NamePattern))
                {
                    firstName = firstAndLastName.ToTitleCase(firstAndLastName.ToLower(firstName));
                    flag = false;
                }
                else
                {
                    if (firstName.Length < 2 || firstName.Length > 60)
                    {
                        Console.WriteLine("Please try again. The name length can't be less than 2 symbols and larger than 60 symbols.");
                    }
                    else
                    {
                        Console.WriteLine($"Please try again. The {firstName} isn't a valid name.");
                    }
                }
            }

            flag = true;
            string lastName = default(string);
            while (flag)
            {
                Console.WriteLine("Last name: ");
                lastName = Console.ReadLine();
                if (lastName != null && lastName.Length > 2 && lastName.Length < 60 && Regex.IsMatch(lastName, NamePattern))
                {
                    lastName = firstAndLastName.ToTitleCase(firstAndLastName.ToLower(lastName));
                    flag = false;
                }
                else
                {
                    if (firstName.Length < 2 || firstName.Length > 60)
                    {
                        Console.WriteLine("Please try again. The name length can't be less than 2 symbols and larger than 60 symbols.");
                    }
                    else
                    {
                        Console.WriteLine($"Please try again. The {lastName} isn't a valid name.");
                    }
                }
            }

            flag = true;
            string data = default(string);
            DateTime dateOfBirth = default(DateTime);
            while (flag)
            {
                Console.WriteLine("Date of birth month/day/year: ");
                data = Console.ReadLine();
                if (DateTime.TryParse(data, out dateOfBirth) && (dateOfBirth <= DateTime.Today || dateOfBirth >= MinDate))
                {
                    flag = false;
                }
                else
                {
                    Console.WriteLine($"Please try again. The {data} isn't a valid date.");
                }
            }

            flag = true;
            Gender gender = default(Gender);
            while (flag)
            {
                Console.WriteLine("Gender M (male) / F (female) / O (other) / U (unknown):");
                data = Console.ReadLine();
                if (Enum.TryParse<Gender>(data, out gender) && (gender == Gender.F || gender == Gender.M || gender == Gender.O || gender == Gender.U))
                {
                    flag = false;
                }
                else
                {
                    Console.WriteLine($"Please try again. The symbol '{data}' wasn't recognized as a valid gender.");
                }
            }

            flag = true;
            char status = default(char);
            while (flag)
            {
                Console.WriteLine("Material status M (married) / U (unmarried)");
                data = Console.ReadLine();
                if (char.TryParse(data, out status) && (status == 'M' || status == 'U'))
                {
                    flag = false;
                }
                else
                {
                    Console.WriteLine($"Please try again. The symbol '{data}' wasn't recognized as a valid material status.");
                }
            }

            flag = true;
            short catsCount = 0;
            var age = DateTime.Today.Year - dateOfBirth.Year;
            while (flag)
            {
                Console.WriteLine($"How many cats does {firstName} {lastName} have?");
                if (age > 30 && gender == Gender.F && status == 'U')
                {
                    catsCount = 30;
                    Console.WriteLine(30);
                    Console.WriteLine($"{firstName} {lastName} is a strong independent woman. (^-.-^)");
                    flag = false;
                }
                else
                {
                    data = Console.ReadLine();
                    if (short.TryParse(data, out catsCount) && catsCount >= 0 && catsCount <= 100)
                    {
                        flag = false;
                        if (catsCount > 10)
                        {
                            Console.WriteLine("Are you seriously??? 0_o");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Please try again. The number '{catsCount}' is not like the truth.");
                    }
                }
            }

            flag = true;
            decimal catsBudget = 0;
            if (catsCount != 0)
            {
                while (flag)
                {
                    Console.WriteLine($"How much does {firstName} {lastName} spend per month on cats?");
                    data = Console.ReadLine();
                    if (decimal.TryParse(data, out catsBudget) && catsBudget > 0)
                    {
                        flag = false;
                    }
                    else
                    {
                        Console.WriteLine($"Please try again. The number '{catsBudget}' is not like the truth.");
                    }
                }
            }

            return (firstName, lastName, dateOfBirth, gender, status, catsCount, catsBudget);
        }

        private static void List(string parameters)
        {
            var fileCabinetRecords = fileCabinetService.GetRecords();
            Print(fileCabinetRecords);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static void Print(FileCabinetRecord[] fileCabinetRecords)
        {
            foreach (var item in fileCabinetRecords)
            {
                string date = item.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
                Console.WriteLine($"#{item.Id}, {item.FirstName}, {item.LastName}, {date}, {item.Gender}, " +
                    $"{item.MaritalStatus}, {item.CatsCount}, {item.CatsBudget}");
            }
        }
    }
}