using System;
using System.Collections.Generic;

namespace FileCabinetApp.Validators.RecordValidator
{
    /// <summary>
    /// Records validators builder.
    /// </summary>
    public class ValidatorBuilder
    {
        private readonly List<IRecordValidator> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorBuilder"/> class.
        /// </summary>
        public ValidatorBuilder()
        {
            this.validators = new List<IRecordValidator>();
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>Record validator.</returns>
        public IRecordValidator Create()
            => new CompositeValidator(this.validators);

        /// <summary>
        /// Validates the first name.
        /// </summary>
        /// <param name="minLength">The minimum length.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns>This validator builder.</returns>
        public ValidatorBuilder ValidateFirstName(int minLength, int maxLength)
        {
            this.validators.Add(new FirstNameValidator(minLength, maxLength));
            return this;
        }

        /// <summary>
        /// Validates the last name.
        /// </summary>
        /// <param name="minLength">The minimum length.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns>This validator builder.</returns>
        public ValidatorBuilder ValidateLastName(int minLength, int maxLength)
        {
            this.validators.Add(new LastNameValidator(minLength, maxLength));
            return this;
        }

        /// <summary>
        /// Validates the date of birth.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns>This validator builder.</returns>
        public ValidatorBuilder ValidateDateOfBirth(DateTime from, DateTime to)
        {
            this.validators.Add(new DateOfBirthValidator(from, to));
            return this;
        }

        /// <summary>
        /// Validates the gender.
        /// </summary>
        /// <param name="gender">The gender.</param>
        /// <returns>This validator builder.</returns>
        public ValidatorBuilder ValidateGender(char[] gender)
        {
            this.validators.Add(new GenderValidator(gender));
            return this;
        }

        /// <summary>
        /// Validates the office.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>This validator builder.</returns>
        public ValidatorBuilder ValidateOffice(int min, int max)
        {
            this.validators.Add(new OfficeValidator(min, max));
            return this;
        }

        /// <summary>
        /// Validates the salary.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>This validator builder.</returns>
        public ValidatorBuilder ValidateSalary(decimal min, decimal max)
        {
            this.validators.Add(new SalaryValidator(min, max));
            return this;
        }
    }
}
