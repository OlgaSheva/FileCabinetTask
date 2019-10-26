using System;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace FileCabinetGenerator.Models
{
    /// <summary>
    /// The class to subscribe file cabinet record.
    /// </summary>
    [XmlRoot("record")]
    public class FileCabinetRecord
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [XmlAttribute("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [XmlElement("name")]
        public Name FullName { get; set; }

        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>
        /// The date of birth.
        /// </value>
        [XmlElement("dateOfBirth", DataType = "date")]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the marital status.
        /// </summary>
        /// <value>
        /// The marital status.
        /// </value>
        [XmlElement("gender")]
        public char Gender { get; set; } // 'M' - male, 'F' - female, 'O' - other, 'U' - unknown

        /// <summary>
        /// Gets or sets the cats count.
        /// </summary>
        /// <value>
        /// The cats count.
        /// </value>
        [XmlElement("office")]
        public short Office { get; set; }

        /// <summary>
        /// Gets or sets the cats budget.
        /// </summary>
        /// <value>
        /// The cats budget.
        /// </value>
        [XmlElement("salary")]
        public decimal Salary { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"{this.Id},");
            builder.Append($"{this.FullName.FirstName},");
            builder.Append($"{this.FullName.LastName},");
            builder.Append($"{this.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)},");
            builder.Append($"{this.Gender},");
            builder.Append($"{this.Office},");
            builder.Append($"{this.Salary.ToString("F", CultureInfo.InvariantCulture)}");

            return builder.ToString();
        }
    }
}
