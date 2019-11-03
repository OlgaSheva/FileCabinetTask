using System;
using System.Text.RegularExpressions;

namespace FileCabinetApp.Validators.RecordValidator
{
    internal class LastNameValidator : IRecordValidator
    {
        private const string NamePattern = @"^[a-zA-Z '.-]*$";
        private readonly int minLength;
        private readonly int maxLength;

        internal LastNameValidator(int min, int max)
        {
            this.minLength = min;
            this.maxLength = max;
        }

        public void ValidateParameters(RecordParameters parameters)
        {
            var lastname = parameters.LastName;
            if (lastname is null)
            {
                throw new ArgumentNullException($"{parameters.LastName} cannot be null.", nameof(parameters.LastName));
            }

            if ((lastname.Length < this.minLength)
                || (lastname.Length >= this.maxLength)
                || !Regex.IsMatch(lastname, NamePattern)
                || string.IsNullOrWhiteSpace(lastname))
            {
                throw new ArgumentException(nameof(parameters.LastName));
            }
        }
    }
}
