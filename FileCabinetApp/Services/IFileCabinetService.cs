﻿using System.Collections.ObjectModel;

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
        /// <returns>New file cabinet record.</returns>
        int CreateRecord(Record record);

        /// <summary>
        /// Gets the records.
        /// </summary>
        /// <returns>All existing records.</returns>
        ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Gets the stat.
        /// </summary>
        /// <returns>The quantity of records.</returns>
        int GetStat();

        /// <summary>
        /// Edits the record.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="record">The record.</param>
        void EditRecord(int id, Record record);

        /// <summary>
        /// Finds the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>All records with specified parameters.</returns>
        ReadOnlyCollection<FileCabinetRecord> Find(string parameters);
    }
}