using System.Xml.Serialization;

namespace FileCabinetGenerator.Models
{
    /// <summary>
    /// XML export model.
    /// </summary>
    [XmlRoot("records")]
    public class RecordsOrder
    {
        /// <summary>
        /// The file cabinet records.
        /// </summary>
        [XmlElement("record")]
        public FileCabinetRecord[] FileCabinetRecords;
    }
}
