using System;

namespace FileCabinetApp.Validators.RecordValidator
{
    internal class SalaryValidator : IRecordValidator
    {
        private readonly int min;
        private readonly int max;

        internal SalaryValidator(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public void ValidateParameters(RecordParameters parameters)
        {
            if (parameters.Salary < this.min || parameters.Salary > this.max)
            {
                throw new ArgumentException(
                    nameof(parameters.Salary),
                    $"The {nameof(parameters.Salary)} can't be less than {this.min} and larger than {this.max}.");
            }
        }
    }
}
