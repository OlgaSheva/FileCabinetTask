using System;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Default validator class.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordValidator" />
    public class DefaultValidator : IRecordValidator
    {
        private static readonly DateTime MinDate = new DateTime(1950, 1, 1);

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
        /// The {nameof(firstName)} length can't be less than 2 symbols and larger than 60 symbols. - firstName
        /// or
        /// The {nameof(firstName)} can't consists only from spases. - firstName
        /// or
        /// The {nameof(lastName)} length can't be less than 2 symbols and larger than 60 symbols. - lastName
        /// or
        /// The {nameof(lastName)} can't consists only from spases. - lastName
        /// or
        /// The {nameof(dateOfBirth)} can't be less than 01-Jan-1950 and larger than the current date. - dateOfBirth
        /// or
        /// The {nameof(gender)} can be only 'M', 'F', 'O' or 'U'. - gender
        /// or
        /// The {nameof(office)} can't be less than 0 or larger than 500. - office
        /// or
        /// The {nameof(salary)} can't be less than zero. - salary.
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

            if (firstName.Length <= 2 || firstName.Length > 60)
            {
                throw new ArgumentException($"The {nameof(firstName)} length can't be less than 2 symbols and larger than 60 symbols.", nameof(firstName));
            }

            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException($"The {nameof(firstName)} can't consists only from spases.", nameof(firstName));
            }

            if (lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException($"The {nameof(lastName)} length can't be less than 2 symbols and larger than 60 symbols.", nameof(lastName));
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException($"The {nameof(lastName)} can't consists only from spases.", nameof(lastName));
            }

            if (dateOfBirth == null)
            {
                throw new ArgumentNullException(nameof(dateOfBirth));
            }

            if (dateOfBirth > DateTime.Today || dateOfBirth < MinDate)
            {
                throw new ArgumentException($"The {nameof(dateOfBirth)} can't be less than 01-Jan-1950 and larger than the current date.", nameof(dateOfBirth));
            }

            if (gender != 'M' && gender != 'F' && gender != 'O' && gender != 'U')
            {
                throw new ArgumentException($"The {nameof(gender)} can be only 'M', 'F', 'O' or 'U'.", nameof(gender));
            }

            if (office < 0 || office > 500)
            {
                throw new ArgumentException($"The {nameof(office)} can't be less than 0 or larger than 500.", nameof(office));
            }

            if (salary < 0)
            {
                throw new ArgumentException($"The {nameof(salary)} can't be less than zero.", nameof(salary));
            }
        }
    }
}
