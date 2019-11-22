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
        /// <param name="exceptions">The exceptions.</param>
        /// <returns>List of records.</returns>
        public IList<FileCabinetRecord> ReadAll(out List<string> exceptions)
        {
            exceptions = new List<string>();
            var list = new List<FileCabinetRecord>();
            var textInfo = new CultureInfo("ru-RU").TextInfo;
            this.reader.ReadLine();
            int line = 0;
            while (!this.reader.EndOfStream)
            {
                string[] elements = this.reader.ReadLine().Split(',');
                FileCabinetRecord record = null;
                try
                {
                    record = new FileCabinetRecord()
                    {
                        Id = int.Parse(elements[0], CultureInfo.InvariantCulture),
                        FirstName = textInfo.ToTitleCase(textInfo.ToLower(elements[1])),
                        LastName = textInfo.ToTitleCase(textInfo.ToLower(elements[2])),
                        DateOfBirth = DateTime.Parse(elements[3], CultureInfo.InvariantCulture),
                        Gender = char.Parse(elements[4]),
                        Office = short.Parse(elements[5], CultureInfo.InvariantCulture),
                        Salary = decimal.Parse(elements[6], CultureInfo.InvariantCulture),
                    };
                    list.Add(record);
                }
                catch (FormatException)
                {
                    exceptions.Add($"Line '{line}' is not valid.");
                }

                line++;
            }

            return list;
        }
    }
}
