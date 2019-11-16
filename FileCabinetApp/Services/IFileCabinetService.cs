﻿using System.Collections.Generic;
using FileCabinetApp.Enums;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// File cabinet service.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns>Id of new file cabinet record.</returns>
        int CreateRecord(RecordParameters record);

        /// <summary>
        /// Inserts the record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="id">The identifier.</param>
        void InsertRecord(RecordParameters record, int id);

        /// <summary>
        /// Gets the stat.
        /// </summary>
        /// <param name="deletedRecordsCount">The deleted records count.</param>
        /// <returns>The quantity of records.</returns>
        int GetStat(out int deletedRecordsCount);

        /// <summary>
        /// Updates the specified record parameters.
        /// </summary>
        /// <param name="recordParameters">The record parameters.</param>
        /// <param name="keyValuePairs">The key value pairs.</param>
        /// <returns>Updated record id.</returns>
        int Update(RecordParameters recordParameters, Dictionary<string, string> keyValuePairs);

        /// <summary>
        /// Selects the specified key value pairs.
        /// </summary>
        /// <param name="keyValuePairs">The key value pairs.</param>
        /// <param name="condition">The condition.</param>
        /// <returns>All records with specified parameters.</returns>
        IEnumerable<FileCabinetRecord> SelectRecords(List<KeyValuePair<string, string>> keyValuePairs, SearchCondition condition);

        /// <summary>
        /// Determines whether [is there a record with this identifier] [the specified identifier].
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="position">The record position.</param>
        /// <returns>
        ///   <c>true</c> if [is there a record with this identifier] [the specified identifier]; otherwise, <c>false</c>.
        /// </returns>
        bool IsThereARecordWithThisId(int id, out long position);

        /// <summary>
        /// Makes the snapshot.
        /// </summary>
        /// <returns>The file cabinet service snapshot.</returns>
        FileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>
        /// Restores the specified snapshot.
        /// </summary>
        /// <param name="snapshot">The snapshot.</param>
        /// <param name="exceptions">The exceptions.</param>
        void Restore(FileCabinetServiceSnapshot snapshot, out Dictionary<int, string> exceptions);

        /// <summary>
        /// Deletes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>Deleted records id.</returns>
        List<int> Delete(string key, string value);

        /// <summary>
        /// Purges the specified deleted records count.
        /// </summary>
        /// <param name="deletedRecordsCount">The deleted records count.</param>
        /// <param name="recordsCount">The records count.</param>
        void Purge(out int deletedRecordsCount, out int recordsCount);
    }
}
