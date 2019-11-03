using System;

namespace FileCabinetApp.Validators.RecordValidator
{
    /// <summary>
    /// The validotor builder extensions.
    /// </summary>
    public static class ValidatorBuilderExtensions
    {
        /// <summary>
        /// Creates the default validator.
        /// </summary>
        /// <param name="validatorBuilder">The validator builder.</param>
        /// <returns>The default vlidator.</returns>
        public static IRecordValidator CreateDefault(this ValidatorBuilder validatorBuilder)
        {
            var validator = new ValidatorBuilder()
                .ValidateFirstName(2, 60)
                .ValidateLastName(2, 60)
                .ValidateDateOfBirth(new DateTime(1950, 1, 1), new DateTime(2010, 1, 1))
                .ValidateGender(new char[] { 'M', 'F', 'O', 'U' })
                .ValidateOffice(0, 500)
                .ValidateSalary(0, 10000)
                .Create();

            return validator;
        }

        /// <summary>
        /// Creates the custom validator.
        /// </summary>
        /// <param name="validatorBuilder">The validator builder.</param>
        /// <returns>The custom validator.</returns>
        public static IRecordValidator CreateCustom(this ValidatorBuilder validatorBuilder)
        {
            var validator = new ValidatorBuilder()
                .ValidateFirstName(2, 30)
                .ValidateLastName(2, 30)
                .ValidateDateOfBirth(new DateTime(1930, 1, 1), new DateTime(2000, 1, 1))
                .ValidateGender(new char[] { 'M', 'F' })
                .ValidateOffice(100, 300)
                .ValidateSalary(100, 7000)
                .Create();

            return validator;
        }
    }
}
