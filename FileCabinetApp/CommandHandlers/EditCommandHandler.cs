﻿using System;
using System.Globalization;

namespace FileCabinetApp.CommandHandlers
{
    internal class EditCommandHandler : CommandHandlerBase
    {
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request.Command == "edit")
            {
                Edit(request.Parameters);
            }

            return base.Handle(request);
        }

        private static void Edit(string parameters)
        {
            int id = -1;
            if (!int.TryParse(parameters, NumberStyles.Integer, CultureInfo.InvariantCulture, out id)
                || id == 0
                || Program.fileCabinetService.GetStat(out int deletedRecordsCount) == 0)
            {
                Console.WriteLine($"The '{parameters}' isn't an ID.");
                return;
            }

            if (Program.fileCabinetService.IsThereARecordWithThisId(id, out long index))
            {
                var (firstName, lastName, dateOfBirth, gender, office, salary) = ParameterEntry();
                try
                {
                    Record record = new Record(firstName, lastName, dateOfBirth, gender, office, salary);
                    Program.fileCabinetService.EditRecord(id, record);
                }
                catch (ArgumentNullException anex)
                {
                    Console.WriteLine($"Record wasn't edited.", anex.Message);
                    Console.WriteLine(HintMessage);
                }
                catch (ArgumentException aex)
                {
                    Console.WriteLine($"Record wasn't edited.", aex.Message);
                    Console.WriteLine(HintMessage);
                }
            }
            else
            {
                Console.WriteLine($"#{id} record is not found.");
            }
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

        private static (string firstName, string lastName, DateTime dateOfBirth, char gender, short office, decimal salary)
            ParameterEntry()
        {
            var firstAndLastName = new CultureInfo("ru-RU").TextInfo;

            Func<string, Tuple<bool, string>> firstNameValidator = Program.validator.FirstNameValidator;
            Func<string, Tuple<bool, string>> lastNameValidator = Program.validator.LastNameValidator;
            Func<DateTime, Tuple<bool, string>> dateOfBirthValidator = Program.validator.DateOfBirthValidator;
            Func<char, Tuple<bool, string>> genderValidator = Program.validator.GenderValidator;
            Func<short, Tuple<bool, string>> officeValidator = Program.validator.OfficeValidator;
            Func<decimal, Tuple<bool, string>> salaryValidator = Program.validator.SalaryValidator;

            Func<string, Tuple<bool, string, string>> stringConverter = Program.converter.StringConverter;
            Func<string, Tuple<bool, string, DateTime>> dateConverter = Program.converter.DateConverter;
            Func<string, Tuple<bool, string, char>> charConverter = Program.converter.CharConverter;
            Func<string, Tuple<bool, string, short>> shortConverter = Program.converter.ShortConverter;
            Func<string, Tuple<bool, string, decimal>> decimalConverter = Program.converter.DecimalConverter;

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
    }
}