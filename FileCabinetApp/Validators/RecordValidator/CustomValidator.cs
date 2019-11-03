using System;

namespace FileCabinetApp.Validators.RecordValidator
{
    /// <summary>
    /// Custom validator class.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordValidator" />
    public class CustomValidator : IRecordValidator
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

            new FirstNameValidator(2, 30).ValidateParameters(parameters);
            new LastNameValidator(2, 30).ValidateParameters(parameters);
            new DateOfBirthValidator(new DateTime(1930, 1, 1), new DateTime(2000, 1, 1)).ValidateParameters(parameters);
            new GenderValidator(new char[] { 'M', 'F' }).ValidateParameters(parameters);
            new OfficeValidator(100, 300).ValidateParameters(parameters);
            new SalaryValidator(100, 7000).ValidateParameters(parameters);
        }
    }
}
