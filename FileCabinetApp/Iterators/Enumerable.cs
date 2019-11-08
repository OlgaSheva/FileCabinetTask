using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Records enumerable.
    /// </summary>
    /// <typeparam name="T">The FileCabinetRecord.</typeparam>
    /// <seealso cref="System.Collections.Generic.IEnumerable{T}" />
    public class Enumerable<T> : IEnumerable<T>
        where T : FileCabinetRecord
    {
        private readonly FileStream fileStream;
        private readonly List<long> positions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Enumerable{T}"/> class.
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="positions">The positions.</param>
        public Enumerable(FileStream fileStream, List<long> positions)
        {
            this.positions = positions;
            this.fileStream = fileStream;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator<T>(this.fileStream, this.positions);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
