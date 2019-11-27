using System;
using System.Globalization;
using System.Xml.Linq;

namespace FileCabinetApp.Writers
{
    /// <summary>
    /// File cabinet record xml writer.
    /// </summary>
    internal class FileCabinetRecordXmlWriter
    {
        /// <summary>
        /// Writes the specified file cabinet record.
        /// </summary>
        /// <param name="fileCabinetRecord">The file cabinet record.</param>
        /// <param name="doc">The document.</param>
        public static void Write(FileCabinetRecord fileCabinetRecord, XDocument doc)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecord));
            }

            if (doc == null)
            {
                throw new ArgumentNullException(nameof(doc));
            }

            XElement record = new XElement("record");
            doc.Root.Add(record);
            record.Add(
                new XAttribute("id", fileCabinetRecord.Id));
            XElement name = new XElement("name");
            record.Add(name);
            name.Add(
                new XAttribute("first", fileCabinetRecord.FirstName),
                new XAttribute("last", fileCabinetRecord.LastName));
            record.Add(
                new XElement("dateOfBirth", fileCabinetRecord.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                new XElement("gender", (int)fileCabinetRecord.Gender),
                new XElement("office", fileCabinetRecord.Office),
                new XElement("salary", fileCabinetRecord.Salary));
        }
    }
}
