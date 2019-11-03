using System;

namespace FileCabinetApp.Validators.RecordValidator
{
    internal class OfficeValidator : IRecordValidator
    {
        private readonly int min;
        private readonly int max;

        internal OfficeValidator(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

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
