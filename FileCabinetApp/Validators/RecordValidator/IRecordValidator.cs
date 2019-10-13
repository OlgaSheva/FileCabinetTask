using System;
using FileCabinetApp.Enums;

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
        /// <param name="materialStatus">The material status.</param>
        /// <param name="catsCount">The cats count.</param>
        /// <param name="catsBudget">The cats budget.</param>
        void ValidateParameters(string firstName, string lastName, DateTime dateOfBirth, Gender gender, char materialStatus, short catsCount, decimal catsBudget);
    }
}
