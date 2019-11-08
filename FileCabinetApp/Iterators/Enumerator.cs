using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FileCabinetApp.Services;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Records enumerator.
    /// </summary>
    /// <typeparam name="T">The FileCabinetRecord.</typeparam>
    /// <seealso cref="System.Collections.Generic.IEnumerator{T}" />
    public sealed class Enumerator<T> : IEnumerator<T>
        where T : FileCabinetRecord
    {
        private readonly FileStream fileStream;
        private readonly List<long> positions;
        private int position = 0;
        private FileCabinetRecord current;

        /// <summary>
        /// Initializes a new instance of the <see cref="Enumerator{T}"/> class.
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="positions">The positions.</param>
        public Enumerator(FileStream fileStream, List<long> positions)
        {
            this.positions = positions;
            this.fileStream = fileStream;
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <exception cref="InvalidOperationException">current is null.</exception>
        /// <value>
        /// The element in the collection at the current position of the enumerator.
        /// </value>
        public FileCabinetRecord Current
        {
            get
            {
                if (this.current == null)
                {
                    throw new InvalidOperationException(nameof(this.current));
                }

                return this.current;
            }
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value>
        /// The element in the collection at the current position of the enumerator.
        /// </value>
        T IEnumerator<T>.Current
        {
            get
            {
                return (T)this.Current;
            }
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value>
        /// The element in the collection at the current position of the enumerator.
        /// </value>
        object IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            using (BinaryReader binaryReader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                if (this.position < this.positions.Count)
                {
                    this.fileStream.Seek(this.positions[this.position++], SeekOrigin.Begin);
                    this.current = FileCabinetFilesystemService.CreateNewFileCabinetRecord(binaryReader);
                }
                else
                {
                    this.current = null;
                }
            }

            if (this.current == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public void Reset()
        {
            this.position = 0;
            this.current = null;
        }
    }
}
