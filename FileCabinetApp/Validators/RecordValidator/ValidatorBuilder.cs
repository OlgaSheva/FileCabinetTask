using System;
using System.Collections.Generic;

namespace FileCabinetApp.Validators.RecordValidator
{
    public class ValidatorBuilder
    {
        private readonly List<IRecordValidator> validators;

        public ValidatorBuilder(List<IRecordValidator> validators)
        {
            this.validators = validators;
        }

        public IRecordValidator Create()
            => new CompositeValidator(this.validators);

        public void ValidateFirstName(int minLength, int maxLength)
        {
            this.validators.Add(new FirstNameValidator(minLength, maxLength));
        }

        public void ValidateLastName(int minLength, int maxLength)
        {
            this.validators.Add(new LastNameValidator(minLength, maxLength));
        }

        public void ValidateDateOfBirth(DateTime from, DateTime to)
        {
            this.validators.Add(new DateOfBirthValidator(from, to));
        }

        public void ValidateGender(char[] gender)
        {
            this.validators.Add(new GenderValidator(gender));
        }

        public void ValidateOffice(int min, int max)
        {
            this.validators.Add(new OfficeValidator(min, max));
        }

        public void ValidateSalary(int min, int max)
        {
            this.validators.Add(new SalaryValidator(min, max));
        }
    }
}
