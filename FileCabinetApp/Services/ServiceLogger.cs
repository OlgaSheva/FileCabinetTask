using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using FileCabinetApp.Iterators;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// The service logger.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Services.IFileCabinetService" />
    public class ServiceLogger : IFileCabinetService
    {
        private readonly IFileCabinetService service;
        private readonly NLog.Logger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public ServiceLogger(IFileCabinetService service)
        {
            this.service = service;
            this.logger = NLog.LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns>
        /// Id of new file cabinet record.
        /// </returns>
        /// <exception cref="ArgumentNullException">record is null.</exception>
        public int CreateRecord(RecordParameters record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.logger.Info($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - " +
                $"Calling Create() with FirstName = '{record.FirstName}', " +
                $"LastName = '{record.LastName}', DateOfBirth = '{record.DateOfBirth.ToShortDateString()}', " +
                $"Gender = '{record.Gender}', Office = '{record.Office}', Salary = '{record.Salary}'");
            var result = this.service.CreateRecord(record);
            this.logger.Info($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - " +
                $"Create() returned '{result}'");
            return result;
        }

        /// <summary>
        /// Edits the record.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="record">The record.</param>
        /// <exception cref="ArgumentNullException">record is null.</exception>
        public void EditRecord(int id, RecordParameters record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.logger.Info($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - " +
                $"Calling Edit() with id = '{id}'");
            this.service.EditRecord(id, record);
            this.logger.Info($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - " +
                $"Edit() edit the record {id} with FirstName = '{record.FirstName}', " +
                $"LastName = '{record.LastName}', DateOfBirth = '{record.DateOfBirth.ToShortDateString()}', " +
                $"Gender = '{record.Gender}', Office = '{record.Office}', Salary = '{record.Salary}'");
        }

        /// <summary>
        /// Finds the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// All records with specified parameters.
        /// </returns>
        /// <exception cref="ArgumentNullException">parameters is null.</exception>
        public IRecordIterator Find(string parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var p = parameters.Split(' ', 2);
            this.logger.Info($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - " +
                $"Calling Find() with parameters '{p[0]}' - '{p[1]}'");
            var result = this.service.Find(parameters);
            this.logger.Info($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - " +
                $"Find() returned records");
            return result;
        }

        /// <summary>
        /// Gets the records.
        /// </summary>
        /// <returns>
        /// All existing records.
        /// </returns>
        public IRecordIterator GetRecords()
        {
            this.logger.Info($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - " +
                $"Calling GetRecords()");
            var result = this.service.GetRecords();
            this.logger.Info($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - " +
                $"GetRecords() returned records");
            return result;
        }

        /// <summary>
        /// Gets the stat.
        /// </summary>
        /// <param name="deletedRecordsCount">The deleted records count.</param>
        /// <returns>
        /// The quantity of records.
        /// </returns>
        public int GetStat(out int deletedRecordsCount)
        {
            this.logger.Info($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - " +
                $"Calling GetStat()");
            var result = this.service.GetStat(out deletedRecordsCount);
            this.logger.Info($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - " +
                $"GetStat() returned '{result}'");
            return result;
        }

        /// <summary>
        /// Determines whether [is there a record with this identifier] [the specified identifier].
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="position">The record position.</param>
        /// <returns>
        /// <c>true</c> if [is there a record with this identifier] [the specified identifier]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsThereARecordWithThisId(int id, out long position)
            => this.service.IsThereARecordWithThisId(id, out position);

        /// <summary>
        /// Makes the snapshot.
        /// </summary>
        /// <returns>
        /// The file cabinet service snapshot.
        /// </returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
            => this.service.MakeSnapshot();

        /// <summary>
        /// Purges the specified deleted records count.
        /// </summary>
        /// <param name="deletedRecordsCount">The deleted records count.</param>
        /// <param name="recordsCount">The records count.</param>
        public void Purge(out int deletedRecordsCount, out int recordsCount)
        {
            this.logger.Info($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - " +
                $"Calling Purge()");
            this.service.Purge(out deletedRecordsCount, out recordsCount);
            this.logger.Info($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - " +
                $"Purge() done defragmentation");
        }

        /// <summary>
        /// Removes a record by the identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="position">Record position.</param>
        public void Remove(int id, long position)
        {
            this.logger.Info($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - " +
                $"Calling Remove() with id - '{id}'");
            this.service.Remove(id, position);
            this.logger.Info($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - " +
                $"Remove() remove record with id - '{id}'");
        }

        /// <summary>
        /// Restores the specified snapshot.
        /// </summary>
        /// <param name="snapshot">The snapshot.</param>
        /// <param name="exceptions">The exceptions.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot, out Dictionary<int, string> exceptions)
        {
            this.service.Restore(snapshot, out exceptions);
        }
    }
}
