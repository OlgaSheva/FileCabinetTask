﻿using System;
using System.Text;

namespace FileCabinetApp.Validators.RecordValidator
{
    /// <summary>
    /// The gender validotor.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordValidator" />
    internal class GenderValidator : IRecordValidator
    {
        private readonly char[] gender;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenderValidator"/> class.
        /// </summary>
        /// <param name="gender">The gender.</param>
        /// <exception cref="ArgumentNullException">Gender is null.</exception>
        /// <exception cref="ArgumentException">Gender is empty.</exception>
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

        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <exception cref="ArgumentException">gender - The {nameof(gender)} can be only {exc}.</exception>
        public void ValidateParameters(RecordParameters parameters)
        {
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
                StringBuilder exc = new StringBuilder();
                foreach (var g in this.gender)
                {
                    exc.Append(g + " ");
                }

                throw new ArgumentException(nameof(parameters.Gender), $"The {nameof(parameters.Gender)} can be only {exc.ToString()}.");
            }
        }
    }
}
