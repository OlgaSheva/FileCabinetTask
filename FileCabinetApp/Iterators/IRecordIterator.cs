namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Record iterator.
    /// </summary>
    public interface IRecordIterator
    {
        /// <summary>
        /// Gets the next record.
        /// </summary>
        /// <returns>The next record.</returns>
        FileCabinetRecord GetNext();

        /// <summary>
        /// Determines whether this instance has more.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance has more; otherwise, <c>false</c>.
        /// </returns>
        bool HasMore();
    }
}
