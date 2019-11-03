using System;
using System.Text.RegularExpressions;

namespace FileCabinetApp.Validators.RecordValidator
{
    /// <summary>
    /// The first name validotor.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordValidator" />
    internal class FirstNameValidator : IRecordValidator
    {
        private const string NamePattern = @"^[a-zA-Z '.-]*$";
        private readonly int minLength;
        private readonly int maxLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstNameValidator"/> class.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        internal FirstNameValidator(int min, int max)
        {
            this.minLength = min;
            this.maxLength = max;
        }

        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <exception cref="ArgumentNullException">FirstName is null.</exception>
        /// <exception cref="ArgumentException">FirstName.</exception>
        public void ValidateParameters(RecordParameters parameters)
        {
            var firstname = parameters.FirstName;
            if (firstname is null)
            {
                throw new ArgumentNullException($"{parameters.FirstName} cannot be null.", nameof(parameters.FirstName));
            }

            if ((firstname.Length < this.minLength)
                && (firstname.Length >= this.maxLength)
                && !Regex.IsMatch(firstname, NamePattern)
                && string.IsNullOrWhiteSpace(firstname))
            {
                throw new ArgumentException(nameof(parameters.FirstName));
            }
        }
    }
}
