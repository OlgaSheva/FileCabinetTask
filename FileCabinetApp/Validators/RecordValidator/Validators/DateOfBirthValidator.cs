using System;

namespace FileCabinetApp.Validators.RecordValidator
{
    /// <summary>
    /// The date of birthday validotor.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordValidator" />
    internal class DateOfBirthValidator : IRecordValidator
    {
        private readonly DateTime from;
        private readonly DateTime to;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        internal DateOfBirthValidator(DateTime from, DateTime to)
        {
            this.from = from;
            this.to = to;
        }

        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <exception cref="ArgumentNullException">DateOfBirth is null.</exception>
        /// <exception cref="ArgumentException">DateOfBirth - The {nameof(parameters.DateOfBirth)} can't be less than {this.from.ToShortDateString()} and larger than {this.to.ToShortDateString()}.</exception>
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
