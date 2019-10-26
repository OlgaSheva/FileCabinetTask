using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
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
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

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
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            var csvWriter = new FileCabinetRecordCsvWriter(writer);
            writer.WriteLine("Id,First Name,Last Name,Date of Birth,Gender,Office,Salary");
            foreach (var item in this.fileCabinetRecords)
            {
                csvWriter.Write(item);
            }
        }

        /// <summary>
        /// Saves to XML.
        /// </summary>
        /// <param name="writer">The writer.</param>
        internal void SaveToXML(StreamWriter writer)
        {
            var xmlWriter = new FileCabinetRecordXmlWriter(writer);
            XElement records = new XElement("records");
            var doc = new XDocument(
               new XDeclaration("1.0", "utf-16", "yes"),
               records);

            foreach (var item in this.fileCabinetRecords)
            {
                xmlWriter.Write(item, doc);
            }

            doc.Save(writer);
        }
    }
}
