using System;

namespace FileCabinetApp.Validators.RecordValidator
{
    /// <summary>
    /// Default validator class.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordValidator" />
    public class DefaultValidator : IRecordValidator
    {
        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public void ValidateParameters(RecordParameters parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            new FirstNameValidator(2, 60).ValidateParameters(parameters);
            new LastNameValidator(2, 60).ValidateParameters(parameters);
            new DateOfBirthValidator(new DateTime(1950, 1, 1), new DateTime(2010, 1, 1)).ValidateParameters(parameters);
            new GenderValidator(new char[] { 'M', 'F', 'O', 'U' }).ValidateParameters(parameters);
            new OfficeValidator(0, 500).ValidateParameters(parameters);
            new SalaryValidator(0, 10000).ValidateParameters(parameters);
        }
    }
}
