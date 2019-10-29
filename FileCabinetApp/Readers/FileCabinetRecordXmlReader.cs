using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using FileCabinetGenerator.Models;

namespace FileCabinetApp.Readers
{
    internal class FileCabinetRecordXmlReader
    {
        private readonly StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <exception cref="ArgumentNullException">Reader is null.</exception>
        public FileCabinetRecordXmlReader(StreamReader reader)
        {
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        /// <summary>
        /// Reads all.
        /// </summary>
        /// <returns>List of records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            var list = new List<FileCabinetRecord>();

            XmlSerializer serializer = new XmlSerializer(typeof(RecordsOrder));
            RecordsOrder recordsOrder = null;
            using (XmlReader xmlReader = XmlReader.Create(this.reader))
            {
                recordsOrder = (RecordsOrder)serializer.Deserialize(xmlReader);
            }

            foreach (var record in recordsOrder.FileCabinetRecords)
            {
                list.Add(new FileCabinetRecord
                {
                Id = record.Id,
                FirstName = record.FullName.FirstName,
                LastName = record.FullName.LastName,
                DateOfBirth = record.DateOfBirth,
                Gender = record.Gender,
                Office = record.Office,
                Salary = record.Salary,
                });
            }

            return list;
        }
    }
}
