using System;

namespace FileCabinetApp.Validators.RecordValidator
{
    internal class GenderValidator : IRecordValidator
    {
        private readonly char[] gender;

        internal GenderValidator(char[] gender)
        {
            if (gender == null)
            {
                throw new ArgumentNullException(nameof(gender));
            }

            if (gender.Length == 0)
            {
                throw new ArgumentException($"{nameof(gender)} cannot be empty.", nameof(gender));
            }

            this.gender = gender;
        }

        public void ValidateParameters(RecordParameters parameters)
        {
            var gender = parameters.Gender;
            bool flag = true;
            foreach (var g in this.gender)
            {
                if (g == parameters.Gender)
                {
                    flag = false;
                }
            }

            if (flag)
            {
                string exc = null;
                foreach (var g in this.gender)
                {
                    exc += g + " ";
                }

                throw new ArgumentException(nameof(gender), $"The {nameof(gender)} can be only {exc}.");
            }
        }
    }
}
