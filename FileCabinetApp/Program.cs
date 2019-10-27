using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

using CommandLine;

using FileCabinetApp.CommandLineOptions;
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
        private const string CustomValidationType = "custom";
        private const string DefaultValidationRules = "default";
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
            new Tuple<string, Action<string>>("import", Import),
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
            new string[] { "export <csv/xml> <file adress>", "exports service data to a CSV or XML file", "The 'export' command exports service data to a CSV or XML file." },
            new string[] { "import <csv/xml> <file adress>", "imports service data from a CSV or XML file", "The 'export' command imports service data from a CSV or XML file." },
        };

        private static IFileCabinetService fileCabinetService;
        private static IInputConverter converter;
        private static IInputValidator validator;
        private static StreamWriter streamWriter;
        private static FileStream fileStream;

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">Input parameters.</param>
        public static void Main(string[] args)
        {
            string validationRules = DefaultValidationRules;
            ServiceType serviceType = ServiceType.Memory;

            var parser = new Parser(with => with.CaseInsensitiveEnumValues = true);
            var result = parser.ParseArguments<Options>(args);
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       validationRules = o.Validate.Equals(CustomValidationType, StringComparison.InvariantCultureIgnoreCase)
                            ? CustomValidationType : DefaultValidationRules;
                       serviceType = (o.Storage == ServiceType.File)
                            ? ServiceType.File : ServiceType.Memory;
                   });
            parser.Dispose();
            fileCabinetService = CreateServise(validationRules, serviceType, out converter, out validator);

            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine($"Using {validationRules} validation rules.");
            Console.WriteLine($"The {serviceType.ToString()} service.");
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

        private static IFileCabinetService CreateServise(string validationRules, ServiceType serviceType, out IInputConverter converter, out IInputValidator validator)
        {
            const string dataFilePath = "cabinet-records.db";

            switch (validationRules)
            {
                case CustomValidationType:
                    validator = new CustomInputValidator();
                    converter = new CustomInputConverter();
                    break;
                case DefaultValidationRules:
                    validator = new DefaultInputValidator();
                    converter = new DefaultInputConverter();
                    break;
                default:
                    throw new Exception();
            }

            switch (serviceType)
            {
                case ServiceType.File:
                    fileStream = new FileStream(dataFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fileCabinetService = (validationRules == CustomValidationType)
                         ? new FileCabinetFilesystemService(fileStream, new CustomValidator())
                         : new FileCabinetFilesystemService(fileStream, new DefaultValidator());
                    break;
                case ServiceType.Memory:
                    fileCabinetService = (validationRules == CustomValidationType)
                                ? new FileCabinetMemoryService(new CustomValidator())
                                : new FileCabinetMemoryService(new DefaultValidator());
                    break;
                default:
                    throw new Exception();
            }

            return fileCabinetService;
        }

        private static void Export(string parameters)
        {
            string[] comands = parameters.Split(' ');
            string format = comands[0];
            string path = comands[1];
            const string yesAnswer = "y";
            const string csvFileType = "csv";
            const string xmlFileType = "xml";

            if (File.Exists(path))
            {
                Console.Write($"File is exist - rewrite {path}? [Y/n] ");
                if (char.TryParse(Console.ReadLine(), out char answer))
                {
                    if (answer.ToString(CultureInfo.InvariantCulture).Equals(yesAnswer, StringComparison.InvariantCultureIgnoreCase))
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
                catch (FileLoadException ex)
                {
                    Console.WriteLine($"Export failed: can't open file {path}.", ex.Message);
                }
            }

            void Write(string p)
            {
                using (streamWriter = new StreamWriter(p))
                {
                    FileCabinetServiceSnapshot snapshot = fileCabinetService.MakeSnapshot();

                    if (format == csvFileType)
                    {
                        snapshot.SaveToCSV(streamWriter);
                    }
                    else if (comands[0] == xmlFileType)
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

        private static void Import(string parameters)
        {
            string[] comands = parameters.Split(' ');
            string fileFormat = comands[0];
            string filePath = comands[1];
            const string csvFormat = "csv";
            const string xmlFormat = "xml";

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Import error: file {filePath} is not exist.");
                return;
            }

            try
            {
                switch (fileFormat)
                {
                    case csvFormat:
                        ImportFromCSVFile(filePath);
                        break;
                    case xmlFormat:
                        ImportFromXMLFile(filePath);
                        break;
                    default:
                        throw new Exception(nameof(fileFormat));
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Records were not imported.");
            }
        }

        private static void ImportFromCSVFile(string filePath)
        {
            FileCabinetServiceSnapshot snapshot = new FileCabinetServiceSnapshot();
            int recordsCount = 0;
            using (StreamReader reader = new StreamReader(filePath))
            {
                snapshot.LoadFromCSV(reader, out recordsCount);
                fileCabinetService.Restore(snapshot);
            }

            Console.WriteLine($"{recordsCount} records were imported from {filePath}.");
        }

        private static void ImportFromXMLFile(string filePath)
        {
            throw new NotImplementedException();
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            var (firstName, lastName, dateOfBirth, gender, office, salary) = ParameterEntry();

            int recordId = 0;
            try
            {
                Record record = new Record(firstName, lastName, dateOfBirth, gender, office, salary);
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
            int.TryParse(parameters, NumberStyles.Integer, CultureInfo.InvariantCulture, out int id);
            if (id == 0 || Program.fileCabinetService.GetStat() == 0)
            {
                Console.WriteLine($"The '{parameters}' isn't an ID.");
                return;
            }

            if (fileCabinetService.IsThereARecordWithThisId(id, out int index))
            {
                var (firstName, lastName, dateOfBirth, gender, office, salary) = ParameterEntry();
                try
                {
                    Record record = new Record(firstName, lastName, dateOfBirth, gender, office, salary);
                    fileCabinetService.EditRecord(id, record);
                }
                catch (ArgumentNullException anex)
                {
                    Console.WriteLine($"Record wasn't edited.", anex.Message);
                    Console.WriteLine(Program.HintMessage);
                }
                catch (ArgumentException aex)
                {
                    Console.WriteLine($"Record wasn't edited.", aex.Message);
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
                Console.WriteLine($"The record didn't find.", ioex.Message);
            }
            catch (ArgumentException aex)
            {
                Console.WriteLine($"The record didn't find.", aex.Message);
            }
        }

        private static (string firstName, string lastName, DateTime dateOfBirth, char gender, short office, decimal salary)
            ParameterEntry()
        {
            var firstAndLastName = new CultureInfo("ru-RU").TextInfo;

            Func<string, Tuple<bool, string>> firstNameValidator = validator.FirstNameValidator;
            Func<string, Tuple<bool, string>> lastNameValidator = validator.LastNameValidator;
            Func<DateTime, Tuple<bool, string>> dateOfBirthValidator = validator.DateOfBirthValidator;
            Func<char, Tuple<bool, string>> genderValidator = validator.GenderValidator;
            Func<short, Tuple<bool, string>> officeValidator = validator.OfficeValidator;
            Func<decimal, Tuple<bool, string>> salaryValidator = validator.SalaryValidator;

            Func<string, Tuple<bool, string, string>> stringConverter = converter.StringConverter;
            Func<string, Tuple<bool, string, DateTime>> dateConverter = converter.DateConverter;
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
            var gender = ReadInput(charConverter, genderValidator);

            Console.Write("Office: ");
            var office = ReadInput(shortConverter, officeValidator);

            Console.Write("Salary: ");
            var salary = ReadInput(decimalConverter, salaryValidator);

            return (firstName, lastName, dateOfBirth, gender, office, salary);
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
            if (fileStream != null)
            {
                fileStream.Close();
            }
        }

        private static void Print(ReadOnlyCollection<FileCabinetRecord> fileCabinetRecords)
        {
            foreach (var item in fileCabinetRecords)
            {
                Console.WriteLine(item.ToString());
            }
        }
    }
}