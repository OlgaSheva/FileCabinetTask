using System;
using FileCabinetApp.Enums;

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
        /// <param name="materialStatus">The material status.</param>
        /// <param name="catsCount">The cats count.</param>
        /// <param name="catsBudget">The cats budget.</param>
        /// <exception cref="ArgumentNullException">
        /// The {nameof(firstName)} can't be null.
        /// or
        /// The {nameof(lastName)} can't be null.
        /// or
        /// The {nameof(dateOfBirth)} can't be null.
        /// or
        /// The {nameof(materialStatus)} isn't a valid material status. You can use only 'M' or 'U' symbols.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The {nameof(firstName)} length can't be less than 2 symbols and larger than 60 symbols.
        /// or
        /// The {nameof(firstName)} can't consists only from spases.
        /// or
        /// The {nameof(lastName)} length can't be less than 2 symbols and larger than 60 symbols.
        /// or
        /// The {nameof(lastName)} can't consists only from spases.
        /// or
        /// The {nameof(dateOfBirth)} can't be less than 01-Jan-1950 and larger than the current date.
        /// or
        /// The {nameof(gender)} can be only 'M', 'F', 'O' or 'U'.
        /// or
        /// The {nameof(catsCount)} can't be less than 0 or larger than 50.
        /// or
        /// The {nameof(catsBudget)} can't be less than zero.
        /// </exception>
        public void ValidateParameters(string firstName, string lastName, DateTime dateOfBirth, Gender gender, char materialStatus, short catsCount, decimal catsBudget)
        {
            if (firstName == null)
            {
                throw new ArgumentNullException($"The {nameof(firstName)} can't be null.");
            }

            if (lastName == null)
            {
                throw new ArgumentNullException($"The {nameof(lastName)} can't be null.");
            }

            if (firstName.Length <= 2 || firstName.Length > 60)
            {
                throw new ArgumentException($"The {nameof(firstName)} length can't be less than 2 symbols and larger than 60 symbols.");
            }

            if (ConsistsOfSpaces(firstName))
            {
                throw new ArgumentException($"The {nameof(firstName)} can't consists only from spases.");
            }

            if (lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException($"The {nameof(lastName)} length can't be less than 2 symbols and larger than 60 symbols.");
            }

            if (ConsistsOfSpaces(lastName))
            {
                throw new ArgumentException($"The {nameof(lastName)} can't consists only from spases.");
            }

            if (dateOfBirth == null)
            {
                throw new ArgumentNullException($"The {nameof(dateOfBirth)} can't be null.");
            }

            if (dateOfBirth > DateTime.Today || dateOfBirth < MinDate)
            {
                throw new ArgumentException($"The {nameof(dateOfBirth)} can't be less than 01-Jan-1950 and larger than the current date.");
            }

            if (gender != Gender.M && gender != Gender.F && gender != Gender.O && gender != Gender.U)
            {
                throw new ArgumentException($"The {nameof(gender)} can be only 'M', 'F', 'O' or 'U'.");
            }

            if (materialStatus != 'M' && materialStatus != 'U')
            {
                throw new ArgumentNullException($"The {nameof(materialStatus)} isn't a valid material status. You can use only 'M' or 'U' symbols.");
            }

            if (catsCount < 0 || catsCount > 100)
            {
                throw new ArgumentException($"The {nameof(catsCount)} can't be less than 0 or larger than 100.");
            }

            if (catsBudget < 0)
            {
                throw new ArgumentException($"The {nameof(catsBudget)} can't be less than zero.");
            }
        }

        private static bool ConsistsOfSpaces(string @string)
        {
            foreach (var item in @string)
            {
                if (item != ' ')
                {
                    return false;
                }
            }

            return true;
        }
    }
}
