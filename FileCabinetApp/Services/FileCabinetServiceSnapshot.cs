using System.Collections.Generic;
using System.IO;
using FileCabinetApp.Writers;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// File cabinet service snapshot.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private FileCabinetRecord[] fileCabinetRecords;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        public FileCabinetServiceSnapshot(List<FileCabinetRecord> list)
        {
            this.fileCabinetRecords = list.ToArray();
        }

        /// <summary>
        /// Makes the snapshot.
        /// </summary>
        /// <returns>The file cabinet service snapshot.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return this;
        }

        /// <summary>
        /// Saves to CSV.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void SaveToCSV(StreamWriter writer)
        {
            var csvWriter = new FileCabinetRecordCsvWriter(writer);
            writer.WriteLine("Id,First Name,Last Name,Date of Birth,Gender,Material Status,Cats Count,Cats Budget");
            foreach (var item in this.fileCabinetRecords)
            {
                csvWriter.Write(item);
            }
        }
    }
}
