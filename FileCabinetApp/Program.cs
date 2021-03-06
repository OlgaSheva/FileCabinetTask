﻿using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;

using FileCabinetApp.CommandHandlers;
using FileCabinetApp.CommandHandlers.SpecificCommandHandlers;
using FileCabinetApp.CommandLineOptions;
using FileCabinetApp.Converters;
using FileCabinetApp.Enums;
using FileCabinetApp.Printer;
using FileCabinetApp.Services;
using FileCabinetApp.Validators.InputValidator;
using FileCabinetApp.Validators.RecordValidator;

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

        private static readonly Action<string> Write = Console.WriteLine;

        private static IInputConverter converter;
        private static IInputValidator validator;
        private static IFileCabinetService fileCabinetService;
        private static FileStream fileStream;

        private static bool isRunning = true;

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">Input parameters.</param>
        public static void Main(string[] args)
        {
            string validationRules = DefaultValidationRules;
            ServiceType serviceType = ServiceType.Memory;
            MeterStatus meter = MeterStatus.Off;
            LoggerStatus logger = LoggerStatus.Off;

            var parser = new Parser(with => with.CaseInsensitiveEnumValues = true);
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       validationRules = o.Validate.Equals(CustomValidationType, StringComparison.InvariantCultureIgnoreCase)
                            ? CustomValidationType : DefaultValidationRules;
                       serviceType = (o.Storage == ServiceType.File)
                            ? ServiceType.File : ServiceType.Memory;
                       meter = (o.Meter == MeterStatus.On)
                            ? MeterStatus.On : MeterStatus.Off;
                       logger = (o.Logger == LoggerStatus.On)
                            ? LoggerStatus.On : LoggerStatus.Off;
                   });
            parser?.Dispose();

            fileCabinetService = CreateServise(validationRules, serviceType, out converter, out validator);

            if (meter == MeterStatus.On)
            {
                fileCabinetService = new ServiceMeter(fileCabinetService, Write);
            }

            if (logger == LoggerStatus.On)
            {
                fileCabinetService = new ServiceLogger(fileCabinetService);
            }

            var commandHandler = CreateCommandHandlers();

            Write($"File Cabinet Application, developed by {Program.DeveloperName}" + Environment.NewLine +
                              $"Using {validationRules} validation rules." + Environment.NewLine +
                              $"The {serviceType.ToString()} service.");
            Write(HintMessage + Environment.NewLine);

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];
                const int parametersIndex = 1;
                var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                try
                {
                    commandHandler.Handle(new AppCommandRequest(command, parameters));
                }
                catch (ArgumentNullException anex)
                {
                    Write($"Error. {anex.Message}");
                }
                catch (ArgumentException aex)
                {
                    Write($"Error. {aex.Message}");
                }
                catch (InvalidOperationException ioex)
                {
                    Write($"Error. {ioex.Message}");
                }
            }
            while (isRunning);
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            var recordPrinter = new Action<List<string>, IEnumerable<FileCabinetRecord>>((x, y) => DefaultRecordPrint(x, y));

            var exitHandler = new ExitCommandHandler(fileStream, Write, (x) => isRunning = x);
            var helpHandler = new HelpCommandHandler(Write);
            var createHandle = new CreateCommandHandler(fileCabinetService, converter, validator, Write);
            var insertHandle = new InsertCommandHandler(fileCabinetService, Write);
            var updateHandler = new UpdateCommandHandler(fileCabinetService, Write);
            var exportHandler = new ExportCommandHandler(fileCabinetService, Write);
            var importHandler = new ImportCommandHandler(fileCabinetService, Write);
            var deleteHandler = new DeleteCommandHandler(fileCabinetService, Write);
            var statHandler = new StatCommandHandler(fileCabinetService, Write);
            var purgeHadler = new PurgeCommandHandler(fileCabinetService, Write);
            var selectHandler = new SelectCommandHandler(fileCabinetService, recordPrinter);
            var missedCommandHandler = new SimilarCommandHandler(Write);
            helpHandler
                .SetNext(exitHandler)
                .SetNext(statHandler)
                .SetNext(createHandle)
                .SetNext(insertHandle)
                .SetNext(updateHandler)
                .SetNext(deleteHandler)
                .SetNext(exportHandler)
                .SetNext(importHandler)
                .SetNext(purgeHadler)
                .SetNext(selectHandler)
                .SetNext(missedCommandHandler);
            return helpHandler;
        }

        private static IFileCabinetService CreateServise(
            string validationRules, ServiceType serviceType, out IInputConverter converter, out IInputValidator validator)
        {
            const string dataFilePath = "cabinet-records.db";

            switch (validationRules)
            {
                case CustomValidationType:
                    validator = InputValidatorBuilder.CreateCustom();
                    converter = new CustomInputConverter();
                    break;
                case DefaultValidationRules:
                    validator = InputValidatorBuilder.CreateDefault();
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
                         ? new FileCabinetFilesystemService(fileStream, new ValidatorBuilder().CreateCustom())
                         : new FileCabinetFilesystemService(fileStream, new ValidatorBuilder().CreateDefault());
                    break;
                case ServiceType.Memory:
                    fileCabinetService = (validationRules == CustomValidationType)
                                ? new FileCabinetMemoryService(new ValidatorBuilder().CreateCustom())
                                : new FileCabinetMemoryService(new ValidatorBuilder().CreateDefault());
                    break;
                default:
                    throw new Exception();
            }

            return fileCabinetService;
        }

        private static void DefaultRecordPrint(List<string> columns, IEnumerable<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            if (columns is null)
            {
                throw new ArgumentNullException(nameof(columns));
            }

            if (columns.Count == 0)
            {
                throw new ArgumentException(
                    $"You must specify at least one parameter to display information about records.",
                    nameof(columns));
            }

            Write(ConsoleTable.From<FileCabinetRecord>(records, columns).ToString());
        }
    }
}