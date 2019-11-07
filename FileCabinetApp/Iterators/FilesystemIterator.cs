using System.Collections.Generic;
using System.IO;
using System.Text;
using FileCabinetApp.Services;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// File system iterator.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Iterators.IRecordIterator" />
    public class FilesystemIterator : IRecordIterator
    {
        private readonly FileStream fileStream;
        private readonly List<long> positions;
        private int current;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemIterator"/> class.
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="positions">The positions.</param>
        public FilesystemIterator(FileStream fileStream, List<long> positions)
        {
            this.positions = positions;
            this.fileStream = fileStream;
            this.current = 0;
        }

        /// <summary>
        /// Gets the next record.
        /// </summary>
        /// <returns>
        /// The next record.
        /// </returns>
        public FileCabinetRecord GetNext()
        {
            using (BinaryReader binaryReader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                this.fileStream.Seek(this.positions[this.current++], SeekOrigin.Begin);
                return FileCabinetFilesystemService.CreateNewFileCabinetRecord(binaryReader);
            }
        }

        /// <summary>
        /// Determines whether this instance has more.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance has more; otherwise, <c>false</c>.
        /// </returns>
        public bool HasMore() => this.current < this.positions.Count;
    }
}
