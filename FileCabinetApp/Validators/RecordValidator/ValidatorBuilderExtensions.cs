using System;
using System.IO;
using FileCabinetApp.Validators.Configuration;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp.Validators.RecordValidator
{
    /// <summary>
    /// The validotor builder extensions.
    /// </summary>
    public static class ValidatorBuilderExtensions
    {
        private static readonly IConfigurationRoot Config =
            new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("validation-rules.json").Build();

        private static ValidatorSettings validatorSettings;

        /// <summary>
        /// Creates the default.
        /// </summary>
        /// <param name="validatorBuilder">The validator builder.</param>
        /// <returns>The default validator.</returns>
        public static IRecordValidator CreateDefault(this ValidatorBuilder validatorBuilder)
            => CreateValidator("default");

        /// <summary>
        /// Creates the custom.
        /// </summary>
        /// <param name="validatorBuilder">The validator builder.</param>
        /// <returns>The custom validator.</returns>
        public static IRecordValidator CreateCustom(this ValidatorBuilder validatorBuilder)
            => CreateValidator("custom");

        private static IRecordValidator CreateValidator(string validatorType)
        {
            if (validatorType.Equals("custom", StringComparison.InvariantCultureIgnoreCase))
            {
                validatorSettings = Config.GetSection("custom").Get<ValidatorSettings>();
            }
            else
            {
                validatorSettings = Config.GetSection("default").Get<ValidatorSettings>();
            }

            var validator = new ValidatorBuilder()
                .ValidateFirstName(validatorSettings.FirstName.Min, validatorSettings.FirstName.Max)
                .ValidateLastName(validatorSettings.LastName.Min, validatorSettings.LastName.Max)
                .ValidateDateOfBirth(validatorSettings.DateOfBirth.From, validatorSettings.DateOfBirth.To)
                .ValidateGender(validatorSettings.Gender.ToCharArray())
                .ValidateOffice(validatorSettings.Office.Min, validatorSettings.Office.Max)
                .ValidateSalary(validatorSettings.Salary.Min, validatorSettings.Salary.Max)
                .Create();

            return validator;
        }
    }
}
