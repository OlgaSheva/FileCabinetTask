using System;
using FileCabinetApp.Enums;

namespace FileCabinetApp
{
    /// <summary>
    /// The class to subscribe file cabinet record.
    /// </summary>
    public class FileCabinetRecord
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>
        /// The date of birth.
        /// </value>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        public Gender Gender { get; set; }

        /// <summary>
        /// Gets or sets the marital status.
        /// </summary>
        /// <value>
        /// The marital status.
        /// </value>
        public char MaritalStatus { get; set; } // 'M' - married, 'U' - unmarried

        /// <summary>
        /// Gets or sets the cats count.
        /// </summary>
        /// <value>
        /// The cats count.
        /// </value>
        public short CatsCount { get; set; }

        /// <summary>
        /// Gets or sets the cats budget.
        /// </summary>
        /// <value>
        /// The cats budget.
        /// </value>
        public decimal CatsBudget { get; set; }
    }
}
