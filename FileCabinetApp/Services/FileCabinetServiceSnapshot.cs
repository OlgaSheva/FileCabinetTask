using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using FileCabinetApp.Readers;
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
        public FileCabinetServiceSnapshot(List<FileCabinetRecord> list = null)
        {
            this.fileCabinetRecords = list?.ToArray() ?? Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Gets the file cabinet records.
        /// </summary>
        /// <value>
        /// The file cabinet records.
        /// </value>
        public ReadOnlyCollection<FileCabinetRecord> FileCabinetRecords
        {
            get => new ReadOnlyCollection<FileCabinetRecord>(this.fileCabinetRecords);
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
        /// <exception cref="ArgumentNullException">Writer is null.</exception>
        internal void SaveToCSV(StreamWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            var csvWriter = new FileCabinetRecordCsvWriter(writer);
            writer.WriteLine($"{nameof(FileCabinetRecord.Id)}," +
                $"{nameof(FileCabinetRecord.FirstName)}," +
                $"{nameof(FileCabinetRecord.LastName)}," +
                $"{nameof(FileCabinetRecord.DateOfBirth)}," +
                $"{nameof(FileCabinetRecord.Gender)}," +
                $"{nameof(FileCabinetRecord.Office)}," +
                $"{nameof(FileCabinetRecord.Salary)}");
            foreach (var item in this.fileCabinetRecords)
            {
                csvWriter.Write(item);
            }
        }

        /// <summary>
        /// Saves to XML.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <exception cref="ArgumentNullException">Writer is null.</exception>
        internal void SaveToXML(StreamWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            var xmlWriter = new FileCabinetRecordXmlWriter(writer);
            XElement records = new XElement("records");
            var doc = new XDocument(
               new XDeclaration("1.0", "utf-16", "yes"),
               records);

            foreach (var item in this.fileCabinetRecords)
            {
                FileCabinetRecordXmlWriter.Write(item, doc);
            }

            doc.Save(writer);
        }

        /// <summary>
        /// Loads from CSV.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="recordsCount">The records count.</param>
        /// <exception cref="ArgumentNullException">Reader is null.</exception>
        internal void LoadFromCSV(StreamReader reader, out int recordsCount)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var csvReader = new FileCabinetRecordCsvReader(reader);
            var recordsFromFile = csvReader.ReadAll();

            recordsCount = recordsFromFile.Count;
            if (recordsCount == 0)
            {
                return;
            }

            this.fileCabinetRecords = recordsFromFile.ToArray();
        }

        /// <summary>
        /// Loads from XML.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="recordsCount">The records count.</param>
        /// <exception cref="ArgumentNullException">Reader is null.</exception>
        internal void LoadFromXML(StreamReader reader, out int recordsCount)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var xmlReader = new FileCabinetRecordXmlReader(reader);
            var recordsFromFile = xmlReader.ReadAll();

            recordsCount = recordsFromFile.Count;
            if (recordsCount == 0)
            {
                return;
            }

            this.fileCabinetRecords = recordsFromFile.ToArray();
        }
    }
}
