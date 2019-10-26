using System.IO;

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
            this.writer.WriteLine(record.ToString());
        }
    }
}
