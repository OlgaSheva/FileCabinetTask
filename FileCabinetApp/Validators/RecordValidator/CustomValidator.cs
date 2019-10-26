using System;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Custom validator class.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordValidator" />
    public class CustomValidator : IRecordValidator
    {
        private static readonly DateTime MinDate = new DateTime(1930, 1, 1);

        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <param name="gender">The gender.</param>
        /// <param name="office">The office.</param>
        /// <param name="salary">The salary.</param>
        /// <exception cref="ArgumentNullException">
        /// firstName
        /// or
        /// lastName
        /// or
        /// dateOfBirth.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// firstName - The {nameof(firstName)} length can't be less than 2 symbols and larger than 20 symbols.
        /// or
        /// firstName - The {nameof(firstName)} can't consists only from spases.
        /// or
        /// lastName - The {nameof(lastName)} length can't be less than 2 symbols and larger than 20 symbols.
        /// or
        /// lastName - The {nameof(lastName)} can't consists only from spases.
        /// or
        /// dateOfBirth - The {nameof(dateOfBirth)} can't be less than 01-Jan-1950 and larger than the current date.
        /// or
        /// gender - The {nameof(gender)} can be only 'M', 'F', 'O' or 'U'.
        /// or
        /// office - The {nameof(office)} can't be less than 0 or larger than 500.
        /// or
        /// salary - The {nameof(salary)} can't be less than zero.
        /// </exception>
        public void ValidateParameters(string firstName, string lastName, DateTime dateOfBirth, char gender, short office, decimal salary)
        {
            if (firstName == null)
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            if (lastName == null)
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            if (firstName.Length < 2 || firstName.Length > 30)
            {
                throw new ArgumentException(nameof(firstName), $"The {nameof(firstName)} length can't be less than 2 symbols and larger than 20 symbols.");
            }

            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException(nameof(firstName), $"The {nameof(firstName)} can't consists only from spases.");
            }

            if (lastName.Length <= 2 || lastName.Length > 30)
            {
                throw new ArgumentException(nameof(lastName), $"The {nameof(lastName)} length can't be less than 2 symbols and larger than 20 symbols.");
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException(nameof(lastName), $"The {nameof(lastName)} can't consists only from spases.");
            }

            if (dateOfBirth == null)
            {
                throw new ArgumentNullException(nameof(dateOfBirth));
            }

            if (dateOfBirth > DateTime.Today || dateOfBirth < MinDate)
            {
                throw new ArgumentException(nameof(dateOfBirth), $"The {nameof(dateOfBirth)} can't be less than 01-Jan-1950 and larger than the current date.");
            }

            if (gender != 'M' && gender != 'F' && gender != 'O' && gender != 'U')
            {
                throw new ArgumentException(nameof(gender), $"The {nameof(gender)} can be only 'M', 'F', 'O' or 'U'.");
            }

            if (office < 0 || office > 300)
            {
                throw new ArgumentException(nameof(office), $"The {nameof(office)} can't be less than 0 or larger than 500.");
            }

            if (salary < 0)
            {
                throw new ArgumentException(nameof(salary), $"The {nameof(salary)} can't be less than zero.");
            }
        }
    }
}
