using System;
using System.Text.RegularExpressions;

namespace FileCabinetApp.Validators.RecordValidator
{
    /// <summary>
    /// The last name validotor.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordValidator" />
    internal class LastNameValidator : IRecordValidator
    {
        private readonly int minLength;
        private readonly int maxLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastNameValidator"/> class.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        internal LastNameValidator(int min, int max)
        {
            this.minLength = min;
            this.maxLength = max;
        }

        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <exception cref="ArgumentNullException">LastName is null.</exception>
        /// <exception cref="ArgumentException">LastName.</exception>
        public void ValidateParameters(RecordParameters parameters)
        {
            var lastname = parameters.LastName;
            if (lastname is null)
            {
                throw new ArgumentNullException($"{parameters.LastName} cannot be null.", nameof(parameters.LastName));
            }

            if ((lastname.Length < this.minLength)
                || (lastname.Length >= this.maxLength)
                || string.IsNullOrWhiteSpace(lastname))
            {
                throw new ArgumentException(nameof(parameters.LastName));
            }
        }
    }
}
