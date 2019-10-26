using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FileCabinetApp.Validators.InputValidator
{
    /// <summary>
    /// Custom input parameters validator.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.InputValidator.IInputValidator" />
    public class CustomInputValidator : IInputValidator
    {
        private const string NamePattern = @"^[a-zA-Z '.-]*$";
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
        public Tuple<bool, string> GenderValidator(char gender)
        {
            bool flag = gender == 'M' || gender == 'F' || gender == 'O' || gender == 'U';
            return new Tuple<bool, string>(flag, gender.ToString());
        }

        /// <summary>
        /// Offices the validator.
        /// </summary>
        /// <param name="office">The office.</param>
        /// <returns>
        /// Is parameter valid.
        /// </returns>
        public Tuple<bool, string> OfficeValidator(short office)
        {
            bool flag = office >= 0 && office < 300;
            return new Tuple<bool, string>(flag, office.ToString());
        }

        /// <summary>
        /// Salaries the validator.
        /// </summary>
        /// <param name="salary">The salary.</param>
        /// <returns>
        /// Is parameter valid.
        /// </returns>
        public Tuple<bool, string> SalaryValidator(decimal salary)
        {
            bool flag = salary >= 0;
            return new Tuple<bool, string>(flag, salary.ToString());
        }

        private static bool IsTheStringValid(string @string)
        {
            return (@string != null) && (@string.Length > 2) && (@string.Length <= 30)
                && Regex.IsMatch(@string, NamePattern) && !string.IsNullOrWhiteSpace(@string);
        }
    }
}
