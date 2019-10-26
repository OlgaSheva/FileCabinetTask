using System.Globalization;
using System.IO;
using System.Text;

namespace FileCabinetApp.Writers
{
    /// <summary>
    /// File cabinet record csv writer.
    /// </summary>
    internal class FileCabinetRecordCsvWriter
    {
        private readonly StreamWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public FileCabinetRecordCsvWriter(StreamWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Writes the record to csv file.
        /// </summary>
        /// <param name="record">The record.</param>
        public void Write(FileCabinetRecord record)
        {
            var builder = new StringBuilder();
            builder.Append($"{record.Id},");
            builder.Append($"{record.FirstName},");
            builder.Append($"{record.LastName},");
            builder.Append($"{record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)},");
            builder.Append($"{record.Gender},");
            builder.Append($"{record.Office},");
            builder.Append($"{record.Salary}");
            this.writer.WriteLine(builder.ToString());
        }
    }
}
