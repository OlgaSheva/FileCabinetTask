using System;
using CommandLine;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.CommandLineOptions;
using FileCabinetApp.Converters;
using FileCabinetApp.Enums;
using FileCabinetApp.Services;
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

        public static bool isRunning = true;
        public static IFileCabinetService fileCabinetService;
        public static string validationRules;
        public static ServiceType serviceType;

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

        private static CommandHandler CreateCommandHandlers()
        {
            var commandHendler = new CommandHandler();
            return commandHendler;
        }
    }
}