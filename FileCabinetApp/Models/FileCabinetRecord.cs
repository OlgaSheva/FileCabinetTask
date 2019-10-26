using System;
using System.Globalization;
using System.Text;

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
        /// Gets or sets the marital status.
        /// </summary>
        /// <value>
        /// The marital status.
        /// </value>
        public char Gender { get; set; } // 'M' - male, 'F' - female, 'O' - other, 'U' - unknown

        /// <summary>
        /// Gets or sets the cats count.
        /// </summary>
        /// <value>
        /// The cats count.
        /// </value>
        public short Office { get; set; }

        /// <summary>
        /// Gets or sets the cats budget.
        /// </summary>
        /// <value>
        /// The cats budget.
        /// </value>
        public decimal Salary { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"{this.Id}, ");
            builder.Append($"{this.FirstName}, ");
            builder.Append($"{this.LastName}, ");
            builder.Append($"{this.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}, ");
            builder.Append($"{this.Gender}, ");
            builder.Append($"{this.Office}, ");
            builder.Append($"{this.Salary}");
            return builder.ToString();
        }
    }
}
