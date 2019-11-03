using System;

namespace FileCabinetApp.Validators.RecordValidator
{
    /// <summary>
    /// The office validotor.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordValidator" />
    internal class OfficeValidator : IRecordValidator
    {
        private readonly int min;
        private readonly int max;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfficeValidator"/> class.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        internal OfficeValidator(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <exception cref="ArgumentException">office - The {nameof(office)} can't be less than {this.min} or larger than {this.max}.</exception>
        public void ValidateParameters(RecordParameters parameters)
        {
            var office = parameters.Office;
            if (office < this.min || office > this.max)
            {
                throw new ArgumentException(nameof(office), $"The {nameof(office)} can't be less than {this.min} or larger than {this.max}.");
            }
        }
    }
}
