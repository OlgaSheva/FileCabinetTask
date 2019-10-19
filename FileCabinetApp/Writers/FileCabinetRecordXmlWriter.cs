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
        public void Write(FileCabinetRecord fileCabinetRecord, XDocument doc)
        {
            XElement record = new XElement("record");
            doc.Root.Add(record);
            record.Add(
                new XAttribute("id", fileCabinetRecord.Id.ToString()));
            XElement name = new XElement("name");
            record.Add(name);
            name.Add(
                new XAttribute("first", fileCabinetRecord.FirstName),
                new XAttribute("last", fileCabinetRecord.LastName));
            record.Add(new XElement("dateOfBirth", fileCabinetRecord.DateOfBirth.ToShortDateString()));
            record.Add(new XElement("gender", fileCabinetRecord.Gender.ToString()));
            record.Add(new XElement("materialStatus", fileCabinetRecord.MateriallStatus.ToString()));
            record.Add(new XElement("catsCount", fileCabinetRecord.CatsCount));
            record.Add(new XElement("catsBudget", fileCabinetRecord.CatsBudget.ToString()));
        }
    }
}
