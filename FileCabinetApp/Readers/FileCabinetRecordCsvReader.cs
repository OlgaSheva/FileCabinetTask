using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FileCabinetApp.Readers
{
    /// <summary>
    /// File cabinet record csv reader.
    /// </summary>
    internal class FileCabinetRecordCsvReader
    {
        private readonly StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public FileCabinetRecordCsvReader(StreamReader reader)
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
            this.reader.ReadLine();
            while (!this.reader.EndOfStream)
            {
                string[] elements = this.reader.ReadLine().Split(',');
                var record = new FileCabinetRecord()
                {
                    Id = int.Parse(elements[0], CultureInfo.InvariantCulture),
                    FirstName = elements[1],
                    LastName = elements[2],
                    DateOfBirth = DateTime.Parse(elements[3], CultureInfo.InvariantCulture),
                    Gender = char.Parse(elements[4]),
                    Office = short.Parse(elements[5], CultureInfo.InvariantCulture),
                    Salary = decimal.Parse(elements[6], CultureInfo.InvariantCulture),
                };
                list.Add(record);
            }

            return list;
        }
    }
}
