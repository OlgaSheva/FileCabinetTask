using System;
using System.IO;
using System.Linq;
using FileCabinetApp.Validators.Configuration;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp.Validators.InputValidator
{
    /// <summary>
    /// Input validator builder.
    /// </summary>
    public static class InputValidatorBuilder
    {
        private static readonly IConfigurationRoot Config =
            new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("validation-rules.json").Build();

        private static ValidatorSettings validatorSettings;
        private static AbstractInputValidator validator;

        /// <summary>
        /// Creates the default.
        /// </summary>
        /// <returns>Default input validator.</returns>
        public static IInputValidator CreateDefault() => CreateValidator("default");

        /// <summary>
        /// Creates the custom.
        /// </summary>
        /// <returns>Custom input validator.</returns>
        public static IInputValidator CreateCustom() => CreateValidator("custom");

        private static IInputValidator CreateValidator(string validatorType)
        {
            if (validatorType.Equals("custom", StringComparison.InvariantCultureIgnoreCase))
            {
                validatorSettings = Config.GetSection("custom").Get<ValidatorSettings>();
                validator = new CustomInputValidator();
            }
            else
            {
                validatorSettings = Config.GetSection("default").Get<ValidatorSettings>();
                validator = new DefaultInputValidator();
            }

            validator.FirstNameMinLength = validatorSettings.FirstName.Min;
            validator.FirstNameMaxLength = validatorSettings.FirstName.Max;
            validator.LastNameMinLength = validatorSettings.LastName.Min;
            validator.LastNameMaxLength = validatorSettings.LastName.Max;
            validator.MinDate = validatorSettings.DateOfBirth.From;
            validator.MaxDate = validatorSettings.DateOfBirth.To;
            validator.Gender = validatorSettings.Gender.ToList();
            validator.MinOffice = validatorSettings.Office.Min;
            validator.MaxOffice = validatorSettings.Office.Max;
            validator.MinSalary = validatorSettings.Salary.Min;
            validator.MaxSalary = validatorSettings.Salary.Max;

            return validator;
        }
    }
}
