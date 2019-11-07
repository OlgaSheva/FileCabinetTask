using System.Collections.Generic;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Memory iterator.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Iterators.IRecordIterator" />
    public class MemoryIterator : IRecordIterator
    {
        private readonly List<FileCabinetRecord> records;
        private int current;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryIterator"/> class.
        /// </summary>
        /// <param name="records">The records.</param>
        public MemoryIterator(List<FileCabinetRecord> records)
        {
            this.records = records;
            this.current = 0;
        }

        /// <summary>
        /// Gets the next record.
        /// </summary>
        /// <returns>
        /// The next record.
        /// </returns>
        public FileCabinetRecord GetNext() => this.records[this.current++];

        /// <summary>
        /// Determines whether this instance has more.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance has more; otherwise, <c>false</c>.
        /// </returns>
        public bool HasMore() => this.current < this.records.Count;
    }
}
