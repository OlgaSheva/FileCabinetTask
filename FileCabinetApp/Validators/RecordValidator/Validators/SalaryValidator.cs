using System;

namespace FileCabinetApp.Validators.RecordValidator
{
    /// <summary>
    /// The salary validotor.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordValidator" />
    internal class SalaryValidator : IRecordValidator
    {
        private readonly int min;
        private readonly int max;

        /// <summary>
        /// Initializes a new instance of the <see cref="SalaryValidator"/> class.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        internal SalaryValidator(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <exception cref="ArgumentException">Salary - The {nameof(parameters.Salary)} can't be less than {this.min} and larger than {this.max}.</exception>
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
