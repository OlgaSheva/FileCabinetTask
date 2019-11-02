using System;
using System.IO;
using CommandLine;
using FileCabinetApp.CommandHandlers;
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
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const string CustomValidationType = "custom";
        private const string DefaultValidationRules = "default";
        private const string DeveloperName = "Olga Kripulevich";

        public static IInputConverter converter;
        public static IInputValidator validator;

        public static bool isRunning = true;
        public static IFileCabinetService fileCabinetService;
        public static string validationRules;
        public static ServiceType serviceType;
        public static FileStream fileStream;

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">Input parameters.</param>
        public static void Main(string[] args)
        {
            validationRules = DefaultValidationRules;
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
            parser?.Dispose();

            fileCabinetService = CreateServise(validationRules, serviceType, out converter, out validator);

            var commandHandler = CreateCommandHandlers();

            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}" + Environment.NewLine +
                              $"Using {validationRules} validation rules." + Environment.NewLine +
                              $"The {serviceType.ToString()} service.");
            Console.WriteLine(HintMessage + Environment.NewLine);

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];
                const int parametersIndex = 1;
                var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                commandHandler.Handle(new AppCommandRequest(command, parameters));
            }
            while (isRunning);
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            var exitHandler = new ExitCommandHandler();
            var helpHandler = new HelpCommandHandler();
            var createHandle = new CreateCommandHandler();
            var editHandler = new EditCommandHandler();
            var findHandler = new FindCommandHandler();
            var exportHandler = new ExportCommandHandler();
            var importHandler = new ImportCommandHandler();
            var removeHandler = new RemoveCommandHandler();
            var listHandle = new ListCommandHandler();
            var statHandler = new StatCommandHandler();
            var purgeHadler = new PurgeCommandHandler();
            var missedCommandHandler = new MissedCommandHandler();
            helpHandler
                .SetNext(exitHandler)
                .SetNext(listHandle)
                .SetNext(statHandler)
                .SetNext(createHandle)
                .SetNext(editHandler)
                .SetNext(findHandler)
                .SetNext(removeHandler)
                .SetNext(exportHandler)
                .SetNext(importHandler)
                .SetNext(purgeHadler)
                .SetNext(missedCommandHandler);
            return helpHandler;
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
    }
}