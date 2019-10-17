using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using FileCabinetApp.Converters;
using FileCabinetApp.Enums;
using FileCabinetApp.Services;
using FileCabinetApp.Validators;
using FileCabinetApp.Validators.InputValidator;

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
            new Tuple<string, Action<string>>("export", Export),
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
            new string[] { "export csv <file adress>", "export service data to a CSV file", "The 'export csv' command export service data to a CSV file." },
        };

        private static IFileCabinetService fileCabinetService;
        private static IInputConverter converter;
        private static IInputValidator validator;
        private static StreamWriter streamWriter;

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
                fileCabinetService = new FileCabinetService(new CustomValidator());
                validator = new CustomInputValidator();
                converter = new CustomInputConverter();
            }
            else
            {
                fileCabinetService = new FileCabinetService(new DefaultValidator());
                validator = new DefaultInputValidator();
                converter = new DefaultInputConverter();
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

        private static void Export(string parameters)
        {
            string[] comands = parameters.Split(' ');
            string format = comands[0];
            string path = comands[1];

            if (File.Exists(path))
            {
                Console.Write($"File is exist - rewrite {path}? [Y/n] ");
                if (char.TryParse(Console.ReadLine(), out char answer))
                {
                    if (answer == 'Y' || answer == 'y')
                    {
                        File.Delete(path);
                        Write(path);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                try
                {
                    Write(path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Export failed: can't open file {path}.", ex.Message);
                }
            }

            void Write(string p)
            {
                using (streamWriter = new StreamWriter(p))
                {
                    FileCabinetServiceSnapshot snapshot = fileCabinetService.MakeSnapshot();

                    if (format.Equals("csv"))
                    {
                        snapshot.SaveToCSV(streamWriter);
                    }
                    else if (comands[0].Equals("xml"))
                    {
                        snapshot.SaveToXML(streamWriter);
                    }
                    else
                    {
                        Console.WriteLine($"There is no {commands[0]} command.");
                        return;
                    }

                    Console.WriteLine($"All records are exported to file {p}.");
                }
            }
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
                ReadOnlyCollection<FileCabinetRecord> findList = fileCabinetService.Find(parameters);
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

        private static (string firstName, string lastName, DateTime dateOfBirth, Gender gender, char status, short catsCount, decimal catsBudget)
            ParameterEntry()
        {
            var firstAndLastName = new CultureInfo("ru-RU").TextInfo;

            Func<string, Tuple<bool, string>> firstNameValidator = validator.FirstNameValidator;
            Func<string, Tuple<bool, string>> lastNameValidator = validator.LastNameValidator;
            Func<DateTime, Tuple<bool, string>> dateOfBirthValidator = validator.DateOfBirthValidator;
            Func<Gender, Tuple<bool, string>> genderValidator = validator.GenderValidator;
            Func<char, Tuple<bool, string>> materialStatusValidator = validator.MaterialStatusValidator;
            Func<short, Tuple<bool, string>> catsCountValidator = validator.CatsCountValidator;
            Func<decimal, Tuple<bool, string>> catsBudgetValidator = validator.CatsBudgetValidator;

            Func<string, Tuple<bool, string, string>> stringConverter = converter.StringConverter;
            Func<string, Tuple<bool, string, DateTime>> dateConverter = converter.DateConverter;
            Func<string, Tuple<bool, string, Gender>> genderConverter = converter.GenderConverter;
            Func<string, Tuple<bool, string, char>> charConverter = converter.CharConverter;
            Func<string, Tuple<bool, string, short>> shortConverter = converter.ShortConverter;
            Func<string, Tuple<bool, string, decimal>> decimalConverter = converter.DecimalConverter;

            Console.Write("First name: ");
            var firstName = ReadInput(stringConverter, firstNameValidator);

            Console.Write("Last name: ");
            var lastName = ReadInput(stringConverter, lastNameValidator);

            Console.Write("Date of birth: ");
            var dateOfBirth = ReadInput(dateConverter, dateOfBirthValidator);

            Console.Write("Gender M (male) / F (female) / O (other) / U (unknown): ");
            var gender = ReadInput(genderConverter, genderValidator);

            Console.Write("Material status M (married) / U (unmarried): ");
            var materialStatus = ReadInput(charConverter, materialStatusValidator);

            short catsCount = 0;
            decimal catsBudget = 0;
            var age = DateTime.Today.Year - dateOfBirth.Year;
            if (age > 30 && gender == Gender.F && materialStatus == 'U')
            {
                catsCount = 30;
                catsBudget = 100;
                Console.WriteLine($"{firstName} {lastName} is a strong independent woman. (^-.-^)");
            }
            else
            {
                Console.Write($"How many cats does {firstName} {lastName} have? ");
                catsCount = ReadInput(shortConverter, catsCountValidator);
                if (catsCount != 0)
                {
                    Console.Write("What is the budget for cats? ");
                    catsBudget = ReadInput(decimalConverter, catsBudgetValidator);
                }
            }

            return (firstName, lastName, dateOfBirth, gender, materialStatus, catsCount, catsBudget);
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        private static void List(string parameters)
        {
            ReadOnlyCollection<FileCabinetRecord> fileCabinetRecords = fileCabinetService.GetRecords();
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

        private static void Print(ReadOnlyCollection<FileCabinetRecord> fileCabinetRecords)
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