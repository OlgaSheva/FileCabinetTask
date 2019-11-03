using System;
using System.Text.RegularExpressions;

namespace FileCabinetApp.Validators.RecordValidator
{
    internal class FirstNameValidator : IRecordValidator
    {
        private const string NamePattern = @"^[a-zA-Z '.-]*$";
        private readonly int minLength;
        private readonly int maxLength;

        internal FirstNameValidator(int min, int max)
        {
            this.minLength = min;
            this.maxLength = max;
        }

        public void ValidateParameters(RecordParameters parameters)
        {
            var firstname = parameters.FirstName;
            if (firstname is null)
            {
                throw new ArgumentNullException($"{parameters.FirstName} cannot be null.", nameof(parameters.FirstName));
            }

            if ((firstname.Length < this.minLength)
                || (firstname.Length >= this.maxLength)
                || !Regex.IsMatch(firstname, NamePattern)
                || string.IsNullOrWhiteSpace(firstname))
            {
                throw new ArgumentException(nameof(parameters.FirstName));
            }
        }
    }
}
