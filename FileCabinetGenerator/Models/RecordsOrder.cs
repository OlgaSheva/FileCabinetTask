using System.Collections.Generic;
using System.Xml.Serialization;

namespace FileCabinetGenerator.Models
{
    /// <summary>
    /// XML export model.
    /// </summary>
    [XmlRoot("records")]
    public class RecordsOrder
    {
        private readonly List<FileCabinetRecord> fileCabinetRecords = new List<FileCabinetRecord>();

        /// <summary>
        /// Gets the file cabinet records.
        /// </summary>
        /// <value>
        /// The file cabinet records.
        /// </value>
        [XmlElement("record")]
        public List<FileCabinetRecord> FileCabinetRecords
        {
            get => this.fileCabinetRecords;
        }
    }
}
