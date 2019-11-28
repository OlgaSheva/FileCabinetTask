using System;
using FileCabinetApp.Converters;
using FileCabinetApp.Memoizers;
using FileCabinetApp.Services;
using FileCabinetApp.Validators.InputValidator;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The command handler creater.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    internal class CreateCommandHandler : ServiceCommandHandlerBase
    {
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private static Action<string> write;
        private readonly IInputConverter converter;
        private readonly IInputValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">The file cabinet service.</param>
        /// <param name="inputConverter">The input converter.</param>
        /// <param name="inputValidator">The input validator.</param>
        /// <param name="writeDelegate">The write delegate.</param>
        public CreateCommandHandler(
            IFileCabinetService fileCabinetService, IInputConverter inputConverter, IInputValidator inputValidator, Action<string> writeDelegate)
            : base(fileCabinetService)
        {
            this.converter = inputConverter;
            this.validator = inputValidator;
            write = writeDelegate;
        }

        /// <summary>
        /// Handles the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// Class AppCommandRequest Instance.
        /// </returns>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Command == "create")
            {
                this.Create();
                if (this.Service is FileCabinetMemoryService)
                {
                    Memoizer.GetMemoizer(this.Service).MemoizerDictionary.Clear();
                }

                return null;
            }
            else
            {
                return base.Handle(request);
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
                    write($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    write($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        private (string firstName, string lastName, DateTime dateOfBirth, char gender, short office, decimal salary)
            ParameterEntry()
        {
            Func<string, Tuple<bool, string>> firstNameValidator = this.validator.FirstNameValidator;
            Func<string, Tuple<bool, string>> lastNameValidator = this.validator.LastNameValidator;
            Func<DateTime, Tuple<bool, string>> dateOfBirthValidator = this.validator.DateOfBirthValidator;
            Func<char, Tuple<bool, string>> genderValidator = this.validator.GenderValidator;
            Func<short, Tuple<bool, string>> officeValidator = this.validator.OfficeValidator;
            Func<decimal, Tuple<bool, string>> salaryValidator = this.validator.SalaryValidator;

            Func<string, Tuple<bool, string, string>> stringConverter = this.converter.StringConverter;
            Func<string, Tuple<bool, string, DateTime>> dateConverter = this.converter.DateConverter;
            Func<string, Tuple<bool, string, char>> charConverter = this.converter.CharConverter;
            Func<string, Tuple<bool, string, short>> shortConverter = this.converter.ShortConverter;
            Func<string, Tuple<bool, string, decimal>> decimalConverter = this.converter.DecimalConverter;

            write("First name: ");
            var firstName = ReadInput(stringConverter, firstNameValidator);

            write("Last name: ");
            var lastName = ReadInput(stringConverter, lastNameValidator);

            write("Date of birth: ");
            var dateOfBirth = ReadInput(dateConverter, dateOfBirthValidator);

            write("Gender M (male) / F (female) / O (other) / U (unknown): ");
            var gender = ReadInput(charConverter, genderValidator);

            write("Office: ");
            var office = ReadInput(shortConverter, officeValidator);

            write("Salary: ");
            var salary = ReadInput(decimalConverter, salaryValidator);

            return (firstName, lastName, dateOfBirth, gender, office, salary);
        }

        private void Create()
        {
            var (firstName, lastName, dateOfBirth, gender, office, salary) = this.ParameterEntry();

            int recordId = 0;
            try
            {
                RecordParameters record = new RecordParameters(firstName, lastName, dateOfBirth, gender, office, salary);
                recordId = this.Service.CreateRecord(record);
                write($"Record #{recordId} is created.");
            }
            catch (ArgumentNullException anex)
            {
                write($"Record wasn't created. {anex.Message}");
                write(HintMessage);
            }
            catch (ArgumentException aex)
            {
                write($"Record wasn't created. {aex.Message}");
                write(HintMessage);
            }
        }
    }
}