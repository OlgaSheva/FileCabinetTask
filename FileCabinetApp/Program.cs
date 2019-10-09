using System;
using System.Globalization;
using FileCabinetApp.Enums;

namespace FileCabinetApp
{
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
            new Tuple<string, Action<string>>("list", List),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "shows statistics by records", "The 'stat' command shows statistics by records" },
            new string[] { "create", "creates a new record", "The 'create' command creates a new record." },
            new string[] { "list", "returns a list of records added to the service", "The 'list' command return a list of records added to the service." },
        };

        private static FileCabinetService fileCabinetService = new FileCabinetService();

        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

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
            Console.WriteLine("First name: ");
            string firstName = Console.ReadLine();

            Console.WriteLine("Last name: ");
            string lastName = Console.ReadLine();

            Console.WriteLine("Date of birth month/day/year: ");
            string data = Console.ReadLine();
            if (!DateTime.TryParse(data, out DateTime dateOfBirth))
            {
                Console.WriteLine($"The string '{data}' wasn't recognized as a valid date.");
                Console.WriteLine("Record wasn't created.");
                Console.WriteLine(Program.HintMessage);
                return;
            }

            Console.WriteLine("Gender Male / Female / Other / Unknown:");
            data = Console.ReadLine();
            if (!Enum.TryParse<Gender>(data, out Gender gender))
            {
                Console.WriteLine($"The symbol '{data}' wasn't recognized as a valid gender.");
                Console.WriteLine("Record wasn't created.");
                Console.WriteLine(Program.HintMessage);
                return;
            }

            Console.WriteLine("Material status M (married) / U (unmarried)");
            data = Console.ReadLine();
            if (!char.TryParse(data, out char status) || (status != 'M' && status != 'U'))
            {
                Console.WriteLine($"The symbol '{data}' wasn't recognized as a valid material status.");
                Console.WriteLine("Record wasn't created.");
                Console.WriteLine(Program.HintMessage);
                return;
            }

            Console.WriteLine("How many cats do you have?");
            short catsCount = 0;
            var age = DateTime.Today.Year - dateOfBirth.Year;
            if (age > 30 && gender == Gender.Female && status == 'U')
            {
                catsCount = 30;
                Console.WriteLine(30);
                Console.WriteLine($"{firstName} {lastName} is a strong independent woman. (^-.-^)");
            }
            else
            {
                data = Console.ReadLine();
                if (!short.TryParse(data, out catsCount))
                {
                    Console.WriteLine($"The number '{catsCount}' is not like the truth.");
                    Console.WriteLine("Record wasn't created.");
                    Console.WriteLine(Program.HintMessage);
                    return;
                }

                if (catsCount > 50)
                {
                    Console.WriteLine("Are you seriously??? 0_o");
                }
            }

            decimal catsBudget = 0;
            if (catsCount != 0)
            {
                Console.WriteLine("How much do you spend per month on cats?");
                data = Console.ReadLine();
                if (!decimal.TryParse(data, out catsBudget))
                {
                    Console.WriteLine($"The number '{catsBudget}' is not like the truth.");
                    Console.WriteLine("Record wasn't created.");
                    Console.WriteLine(Program.HintMessage);
                    return;
                }

                if (catsBudget < 10)
                {
                    Console.WriteLine("You need to pamper your cats more!");
                }
            }

            int recordId = default(int);
            try
            {
                recordId = fileCabinetService.CreateRecord(firstName, lastName, dateOfBirth, gender, status, catsCount, catsBudget);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("All fields are required.");
                Console.WriteLine("Record wasn't created.");
                Console.WriteLine(Program.HintMessage);
                return;
            }

            Console.WriteLine($"Record #{recordId} is created.");
        }

        private static void List(string parameters)
        {
            var list = fileCabinetService.GetRecords();

            foreach (var item in list)
            {
                string date = item.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
                Console.WriteLine($"#{item.Id}, {item.FirstName}, {item.LastName}, {date}, {item.Gender}, " +
                    $"{item.MaritalStatus}, {item.CatsCount}, {item.CatsBudget}");
            }
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
    }
}