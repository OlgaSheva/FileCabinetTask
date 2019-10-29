using System;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Validator.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <param name="gender">The gender.</param>
        /// <param name="office">The office.</param>
        /// <param name="salary">The salary.</param>
        void ValidateParameters(string firstName, string lastName, DateTime dateOfBirth, char gender, short office, decimal salary);
    }
}
