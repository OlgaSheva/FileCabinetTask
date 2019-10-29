using System;

namespace FileCabinetApp.Validators.InputValidator
{
    /// <summary>
    /// Input parameters validator.
    /// </summary>
    public interface IInputValidator
    {
        /// <summary>
        /// Firsts the name validator.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <returns>Is parameter valid.</returns>
        Tuple<bool, string> FirstNameValidator(string firstName);

        /// <summary>
        /// Lasts the name validator.
        /// </summary>
        /// <param name="lastName">The last name.</param>
        /// <returns>Is parameter valid.</returns>
        Tuple<bool, string> LastNameValidator(string lastName);

        /// <summary>
        /// Dates the of birth validator.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <returns>Is parameter valid.</returns>
        Tuple<bool, string> DateOfBirthValidator(DateTime dateOfBirth);

        /// <summary>
        /// Genders the validator.
        /// </summary>
        /// <param name="gender">The gender.</param>
        /// <returns>Is parameter valid.</returns>
        Tuple<bool, string> GenderValidator(char gender);

        /// <summary>
        /// Offices the validator.
        /// </summary>
        /// <param name="office">The office.</param>
        /// <returns>Is parameter valid.</returns>
        Tuple<bool, string> OfficeValidator(short office);

        /// <summary>
        /// Salaries the validator.
        /// </summary>
        /// <param name="salary">The salary.</param>
        /// <returns>Is parameter valid.</returns>
        Tuple<bool, string> SalaryValidator(decimal salary);
    }
}
