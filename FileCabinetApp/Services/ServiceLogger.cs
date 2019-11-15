using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using FileCabinetApp.Enums;

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
        /// Inserts the record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="id">The identifier.</param>
        /// <exception cref="ArgumentNullException">record is null.</exception>
        public void InsertRecord(RecordParameters record, int id)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.logger.Info($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - " +
                $"Calling Insert() with Id = '{id}', FirstName = '{record.FirstName}', " +
                $"LastName = '{record.LastName}', DateOfBirth = '{record.DateOfBirth.ToShortDateString()}', " +
                $"Gender = '{record.Gender}', Office = '{record.Office}', Salary = '{record.Salary}'");
            this.service.InsertRecord(record, id);
            this.logger.Info($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - " +
                $"Insert() inserted record");
        }

        /// <summary>
        /// Updates the specified record parameters.
        /// </summary>
        /// <param name="recordParameters">The record parameters.</param>
        /// <param name="keyValuePairs">The key value pairs.</param>
        /// <returns>
        /// Updated record id.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// recordParameters
        /// or
        /// keyValuePairs.
        /// </exception>
        public int Update(RecordParameters recordParameters, Dictionary<string, string> keyValuePairs)
        {
            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            if (keyValuePairs == null)
            {
                throw new ArgumentNullException(nameof(keyValuePairs));
            }

            this.logger.Info($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - " +
                $"Calling Update() with parameters FirstNme = '{recordParameters?.FirstName}', LastName = '{recordParameters.LastName}', " +
                $"DateOfBirth = '{recordParameters.DateOfBirth}', Gender = '{recordParameters.Gender}', " +
                $"Office = '{recordParameters.Office}', Salary = '{recordParameters.Salary}'");
            int id = this.service.Update(recordParameters, keyValuePairs);
            this.logger.Info($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - " +
                $"Update() update the record {id}.'");
            return id;
        }

        /// <summary>
        /// Selects the specified key value pairs.
        /// </summary>
        /// <param name="keyValuePairs">The key value pairs.</param>
        /// <param name="condition">The condition.</param>
        /// <returns>
        /// All records with specified parameters.
        /// </returns>
        public IEnumerable<FileCabinetRecord> SelectRecords(List<KeyValuePair<string, string>> keyValuePairs, SearchCondition condition)
        {
            if (keyValuePairs == null)
            {
                throw new ArgumentNullException(nameof(keyValuePairs));
            }

            StringBuilder sb = new StringBuilder();
            foreach (var kv in keyValuePairs)
            {
                sb.Append($"{kv.Key} - '{kv.Value}' ");
            }

            this.logger.Info($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - " +
                $"Calling Select() with parameters {sb}");
            var result = this.service.SelectRecords(keyValuePairs, condition);
            this.logger.Info($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - " +
                $"Select() returned records");
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
        /// Restores the specified snapshot.
        /// </summary>
        /// <param name="snapshot">The snapshot.</param>
        /// <param name="exceptions">The exceptions.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot, out Dictionary<int, string> exceptions)
        {
            this.service.Restore(snapshot, out exceptions);
        }

        /// <summary>
        /// Deletes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// Deleted records id.
        /// </returns>
        public List<int> Delete(string key, string value)
        {
            this.logger.Info($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - " +
                $"Calling Delete() where {key} = '{value}'");
            var ids = this.service.Delete(key, value);

            var sb = new StringBuilder();
            for (int i = 0; i < ids.Count; i++)
            {
                if (i < ids.Count - 1)
                {
                    sb.Append($"#{ids[i]}, ");
                }
                else
                {
                    sb.Append($"#{ids[i]} ");
                }
            }

            this.logger.Info($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)} - " +
                $"Delete() remove records {sb}");
            return ids;
        }
    }
}
