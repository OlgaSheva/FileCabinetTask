using System;
using FileCabinetApp.Enums;

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
        Tuple<bool, string> GenderValidator(Gender gender);

        /// <summary>
        /// Materials the status validator.
        /// </summary>
        /// <param name="materialStatus">The material status.</param>
        /// <returns>Is parameter valid.</returns>
        Tuple<bool, string> MaterialStatusValidator(char materialStatus);

        /// <summary>
        /// Catses the count validator.
        /// </summary>
        /// <param name="catsCount">The cats count.</param>
        /// <returns>Is parameter valid.</returns>
        Tuple<bool, string> CatsCountValidator(short catsCount);

        /// <summary>
        /// Catses the budget validator.
        /// </summary>
        /// <param name="catsBudget">The cats budget.</param>
        /// <returns>Is parameter valid.</returns>
        Tuple<bool, string> CatsBudgetValidator(decimal catsBudget);
    }
}
