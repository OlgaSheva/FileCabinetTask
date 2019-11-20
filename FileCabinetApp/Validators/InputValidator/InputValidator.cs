using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp.Validators.InputValidator
{
    /// <summary>
    /// Abstract input validator.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.InputValidator.IInputValidator" />
    public class InputValidator : IInputValidator
    {
        /// <summary>
        /// Gets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        public List<char> Gender { get; internal set; }

        /// <summary>
        /// Gets the minimum date.
        /// </summary>
        /// <value>
        /// The minimum date.
        /// </value>
        public DateTime MinDate { get; internal set; }

        /// <summary>
        /// Gets the maximum date.
        /// </summary>
        /// <value>
        /// The maximum date.
        /// </value>
        public DateTime MaxDate { get; internal set; }

        /// <summary>
        /// Gets the minimum office.
        /// </summary>
        /// <value>
        /// The minimum office.
        /// </value>
        public short MinOffice { get; internal set; }

        /// <summary>
        /// Gets the maximum office.
        /// </summary>
        /// <value>
        /// The maximum office.
        /// </value>
        public short MaxOffice { get; internal set; }

        /// <summary>
        /// Gets the minimum salary.
        /// </summary>
        /// <value>
        /// The minimum salary.
        /// </value>
        public decimal MinSalary { get; internal set; }

        /// <summary>
        /// Gets the maximum salary.
        /// </summary>
        /// <value>
        /// The maximum salary.
        /// </value>
        public decimal MaxSalary { get; internal set; }

        /// <summary>
        /// Gets the first length of the name minimum.
        /// </summary>
        /// <value>
        /// The first length of the name minimum.
        /// </value>
        public int FirstNameMinLength { get; internal set; }

        /// <summary>
        /// Gets the first length of the name maximum.
        /// </summary>
        /// <value>
        /// The first length of the name maximum.
        /// </value>
        public int FirstNameMaxLength { get; internal set; }

        /// <summary>
        /// Gets the last length of the name minimum.
        /// </summary>
        /// <value>
        /// The last length of the name minimum.
        /// </value>
        public int LastNameMinLength { get; internal set; }

        /// <summary>
        /// Gets the last length of the name maximum.
        /// </summary>
        /// <value>
        /// The last length of the name maximum.
        /// </value>
        public int LastNameMaxLength { get; internal set; }

        /// <summary>
        /// Dates the of birth validator.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <returns>
        /// Is parameter valid.
        /// </returns>
        public Tuple<bool, string> DateOfBirthValidator(DateTime dateOfBirth)
        {
            bool flag = (dateOfBirth <= this.MaxDate) && (dateOfBirth >= this.MinDate);
            return new Tuple<bool, string>(flag, dateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Firsts the name validator.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <returns>
        /// Is parameter valid.
        /// </returns>
        public Tuple<bool, string> FirstNameValidator(string firstName)
        {
            bool flag = (firstName != null)
                && (firstName.Length > this.FirstNameMinLength)
                && (firstName.Length <= this.FirstNameMaxLength)
                && !string.IsNullOrWhiteSpace(firstName);
            return new Tuple<bool, string>(flag, firstName);
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
            bool flag = false;
            foreach (var g in this.Gender)
            {
                if (g == gender)
                {
                    flag = true;
                }
            }

            return new Tuple<bool, string>(flag, gender.ToString(CultureInfo.InvariantCulture));
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
            bool flag = (lastName != null)
                && (lastName.Length > this.LastNameMinLength)
                && (lastName.Length <= this.LastNameMaxLength)
                && !string.IsNullOrWhiteSpace(lastName);
            return new Tuple<bool, string>(flag, lastName);
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
            bool flag = office >= this.MinOffice
                && office < this.MaxOffice;
            return new Tuple<bool, string>(flag, office.ToString(CultureInfo.InvariantCulture));
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
            bool flag = salary >= this.MinSalary
                && salary <= this.MaxSalary;
            return new Tuple<bool, string>(flag, salary.ToString(CultureInfo.InvariantCulture));
        }
    }
}
