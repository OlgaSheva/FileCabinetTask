using System.Globalization;
using System.IO;
using System.Xml.Linq;

namespace FileCabinetApp.Writers
{
    /// <summary>
    /// File cabinet record xml writer.
    /// </summary>
    internal class FileCabinetRecordXmlWriter
    {
        private readonly StreamWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public FileCabinetRecordXmlWriter(StreamWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Writes the specified file cabinet record.
        /// </summary>
        /// <param name="fileCabinetRecord">The file cabinet record.</param>
        /// <param name="doc">The document.</param>
        public static void Write(FileCabinetRecord fileCabinetRecord, XDocument doc)
        {
            XElement record = new XElement("record");
            doc.Root.Add(record);
            record.Add(
                new XAttribute("id", fileCabinetRecord.Id.ToString(CultureInfo.InvariantCulture)));
            XElement name = new XElement("name");
            record.Add(name);
            name.Add(
                new XAttribute("first", fileCabinetRecord.FirstName),
                new XAttribute("last", fileCabinetRecord.LastName));
            record.Add(new XElement("dateOfBirth", fileCabinetRecord.DateOfBirth.ToShortDateString()));
            record.Add(new XElement("gender", fileCabinetRecord.Gender.ToString(CultureInfo.InvariantCulture)));
            record.Add(new XElement("office", fileCabinetRecord.Office));
            record.Add(new XElement("salary", fileCabinetRecord.Salary.ToString(CultureInfo.InvariantCulture)));
        }
    }
}
