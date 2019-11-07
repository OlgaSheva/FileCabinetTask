using System;

namespace FileCabinetApp.Validators.Configuration
{
    /// <summary>
    /// The validation settings class.
    /// </summary>
    internal class ValidatorSettings
    {
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public FirstNameSettings FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public LastNameSettings LastName { get; set; }

        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>
        /// The date of birth.
        /// </value>
        public DateOfBirthSettings DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the office.
        /// </summary>
        /// <value>
        /// The office.
        /// </value>
        public OfficeSettings Office { get; set; }

        /// <summary>
        /// Gets or sets the salary.
        /// </summary>
        /// <value>
        /// The salary.
        /// </value>
        public SalarySettings Salary { get; set; }

        /// <summary>
        /// First name settings.
        /// </summary>
        public class FirstNameSettings
        {
            /// <summary>
            /// Gets or sets determines the minimum of the parameters.
            /// </summary>
            /// <value>
            /// The minimum.
            /// </value>
            public int Min { get; set; }

            /// <summary>
            /// Gets or sets determines the maximum of the parameters.
            /// </summary>
            /// <value>
            /// The maximum.
            /// </value>
            public int Max { get; set; }
        }

        /// <summary>
        /// Last name settings.
        /// </summary>
        public class LastNameSettings
        {
            /// <summary>
            /// Gets or sets determines the minimum of the parameters.
            /// </summary>
            /// <value>
            /// The minimum.
            /// </value>
            public int Min { get; set; }

            /// <summary>
            /// Gets or sets determines the maximum of the parameters.
            /// </summary>
            /// <value>
            /// The maximum.
            /// </value>
            public int Max { get; set; }
        }

        /// <summary>
        /// Date of birth settings.
        /// </summary>
        public class DateOfBirthSettings
        {
            /// <summary>
            /// Gets or sets from.
            /// </summary>
            /// <value>
            /// From.
            /// </value>
            public DateTime From { get; set; }

            /// <summary>
            /// Gets or sets to.
            /// </summary>
            /// <value>
            /// To.
            /// </value>
            public DateTime To { get; set; }
        }

        /// <summary>
        /// Office settings.
        /// </summary>
        public class OfficeSettings
        {
            /// <summary>
            /// Gets or sets determines the minimum of the parameters.
            /// </summary>
            /// <value>
            /// The minimum.
            /// </value>
            public int Min { get; set; }

            /// <summary>
            /// Gets or sets determines the maximum of the parameters.
            /// </summary>
            /// <value>
            /// The maximum.
            /// </value>
            public int Max { get; set; }
        }

        /// <summary>
        /// Salary settings.
        /// </summary>
        public class SalarySettings
        {
            /// <summary>
            /// Gets or sets determines the minimum of the parameters.
            /// </summary>
            /// <value>
            /// The minimum.
            /// </value>
            public int Min { get; set; }

            /// <summary>
            /// Gets or sets determines the maximum of the parameters.
            /// </summary>
            /// <value>
            /// The maximum.
            /// </value>
            public int Max { get; set; }
        }
    }
}
