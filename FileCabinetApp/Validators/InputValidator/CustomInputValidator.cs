using System;
using System.Globalization;
using System.Text.RegularExpressions;
using FileCabinetApp.Enums;

namespace FileCabinetApp.Validators.InputValidator
{
    /// <summary>
    /// Custom input parameters validator.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.InputValidator.IInputValidator" />
    public class CustomInputValidator : IInputValidator
    {
        private static readonly string NamePattern = @"^[a-zA-Z '.-]*$";
        private static readonly DateTime MinDate = new DateTime(1930, 1, 1);

        /// <summary>
        /// Firsts the name validator.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <returns>
        /// Is parameter valid.
        /// </returns>
        public Tuple<bool, string> FirstNameValidator(string firstName)
        {
            bool flag = IsTheStringValid(firstName);
            return new Tuple<bool, string>(flag, firstName);
        }

        /// <summary>
        /// Lasts the name validator.
        /// </summary>
        /// <param name="lastName">The last name.</param>
        /// <returns>
        /// Is parameter valid.
        /// </returns>
        public Tuple<bool, string> LastNameValidator(string lastName)
        {
            bool flag = IsTheStringValid(lastName);
            return new Tuple<bool, string>(flag, lastName);
        }

        /// <summary>
        /// Dates the of birth validator.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <returns>
        /// Is parameter valid.
        /// </returns>
        public Tuple<bool, string> DateOfBirthValidator(DateTime dateOfBirth)
        {
            bool flag = (dateOfBirth <= DateTime.Today) && (dateOfBirth >= MinDate);
            return new Tuple<bool, string>(flag, dateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Genders the validator.
        /// </summary>
        /// <param name="gender">The gender.</param>
        /// <returns>
        /// Is parameter valid.
        /// </returns>
        public Tuple<bool, string> GenderValidator(Gender gender)
        {
            bool flag = gender == Gender.M || gender == Gender.F || gender == Gender.O || gender == Gender.U;
            return new Tuple<bool, string>(flag, gender.ToString());
        }

        /// <summary>
        /// Materials the status validator.
        /// </summary>
        /// <param name="materialStatus">The material status.</param>
        /// <returns>
        /// Is parameter valid.
        /// </returns>
        public Tuple<bool, string> MaterialStatusValidator(char materialStatus)
        {
            bool flag = materialStatus == 'M' || materialStatus == 'U';
            return new Tuple<bool, string>(flag, materialStatus.ToString());
        }

        /// <summary>
        /// Catses the count validator.
        /// </summary>
        /// <param name="catsCount">The cats count.</param>
        /// <returns>
        /// Is parameter valid.
        /// </returns>
        public Tuple<bool, string> CatsCountValidator(short catsCount)
        {
            bool flag = catsCount >= 0 || catsCount < 50;
            return new Tuple<bool, string>(flag, catsCount.ToString());
        }

        /// <summary>
        /// Catses the budget validator.
        /// </summary>
        /// <param name="catsBudget">The cats budget.</param>
        /// <returns>
        /// Is parameter valid.
        /// </returns>
        public Tuple<bool, string> CatsBudgetValidator(decimal catsBudget)
        {
            bool flag = catsBudget >= 0;
            return new Tuple<bool, string>(flag, catsBudget.ToString());
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

        private static bool IsTheStringValid(string @string)
        {
            return (@string != null) && (@string.Length > 2) && (@string.Length <= 30)
                && Regex.IsMatch(@string, NamePattern) && !ConsistsOfSpaces(@string);
        }
    }
}
