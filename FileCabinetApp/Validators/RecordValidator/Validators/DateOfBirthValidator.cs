using System;

namespace FileCabinetApp.Validators.RecordValidator
{
    internal class DateOfBirthValidator : IRecordValidator
    {
        private readonly DateTime from;
        private readonly DateTime to;

        internal DateOfBirthValidator(DateTime from, DateTime to)
        {
            this.from = from;
            this.to = to;
        }

        public void ValidateParameters(RecordParameters parameters)
        {
            if (parameters.DateOfBirth == null)
            {
                throw new ArgumentNullException($"{parameters.DateOfBirth} cannot be null.", nameof(parameters.DateOfBirth));
            }

            if (parameters.DateOfBirth > this.to || parameters.DateOfBirth < this.from)
            {
                throw new ArgumentException(
                    nameof(parameters.DateOfBirth),
                    $"The {nameof(parameters.DateOfBirth)} can't be less than {this.from.ToShortDateString()} and larger than {this.to.ToShortDateString()}.");
            }
        }
    }
}
