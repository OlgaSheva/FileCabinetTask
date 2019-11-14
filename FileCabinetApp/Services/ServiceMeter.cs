using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// The servise meter.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Services.IFileCabinetService" />
    public class ServiceMeter : IFileCabinetService
    {
        private readonly IFileCabinetService service;
        private readonly Stopwatch stopWatch;
        private long ticks;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMeter"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public ServiceMeter(IFileCabinetService service)
        {
            this.service = service;
            this.stopWatch = new Stopwatch();
        }

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns>Id of new file cabinet record.</returns>
        public int CreateRecord(RecordParameters record)
        {
            this.stopWatch.Reset();
            this.stopWatch.Start();
            var result = this.service.CreateRecord(record);

            this.stopWatch.Stop();
            this.ticks = this.stopWatch.ElapsedTicks;

            Console.WriteLine($"Create method execution duration is {this.ticks} ticks.");
            return result;
        }

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="recordParameters">The record parameters.</param>
        /// <param name="id">The identifier.</param>
        public void InsertRecord(RecordParameters recordParameters, int id)
        {
            this.stopWatch.Reset();
            this.stopWatch.Start();
            this.service.InsertRecord(recordParameters, id);

            this.stopWatch.Stop();
            this.ticks = this.stopWatch.ElapsedTicks;

            Console.WriteLine($"Insert method execution duration is {this.ticks} ticks.");
        }

        /// <summary>
        /// Edits the record.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="record">The record.</param>
        public void EditRecord(int id, RecordParameters record)
        {
            this.stopWatch.Reset();
            this.stopWatch.Start();
            this.service.EditRecord(id, record);

            this.stopWatch.Stop();
            this.ticks = this.stopWatch.ElapsedTicks;

            Console.WriteLine($"Edit method execution duration is {this.ticks} ticks.");
        }

        /// <summary>
        /// Finds the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Records by parameter.</returns>
        public IEnumerable<FileCabinetRecord> Find(string parameters)
        {
            this.stopWatch.Reset();
            this.stopWatch.Start();
            var result = this.service.Find(parameters);

            this.stopWatch.Stop();
            this.ticks = this.stopWatch.ElapsedTicks;

            Console.WriteLine($"Find method execution duration is {this.ticks} ticks.");
            return result;
        }

        /// <summary>
        /// Gets the records.
        /// </summary>
        /// <returns>All records.</returns>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            this.stopWatch.Reset();
            this.stopWatch.Start();
            var result = this.service.GetRecords();

            this.stopWatch.Stop();
            this.ticks = this.stopWatch.ElapsedTicks;

            Console.WriteLine($"Create method execution duration is {this.ticks} ticks.");
            return result;
        }

        /// <summary>
        /// Gets the stat.
        /// </summary>
        /// <param name="deletedRecordsCount">The deleted records count.</param>
        /// <returns>Count of records.</returns>
        public int GetStat(out int deletedRecordsCount)
        {
            this.stopWatch.Reset();
            this.stopWatch.Start();
            var result = this.service.GetStat(out deletedRecordsCount);

            this.stopWatch.Stop();
            this.ticks = this.stopWatch.ElapsedTicks;

            Console.WriteLine($"Stat method execution duration is {this.ticks} ticks.");
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
            this.stopWatch.Reset();
            this.stopWatch.Start();
            this.service.Purge(out deletedRecordsCount, out recordsCount);

            this.stopWatch.Stop();
            this.ticks = this.stopWatch.ElapsedTicks;

            Console.WriteLine($"Purge method execution duration is {this.ticks} ticks.");
        }

        /// <summary>
        /// Removes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="position">The position.</param>
        public void Remove(int id, long position)
        {
            this.stopWatch.Reset();
            this.stopWatch.Start();
            this.service.Remove(id, position);

            this.stopWatch.Stop();
            this.ticks = this.stopWatch.ElapsedTicks;

            Console.WriteLine($"Remove method execution duration is {this.ticks} ticks.");
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
            this.stopWatch.Reset();
            this.stopWatch.Start();
            var result = this.service.Delete(key, value);

            this.stopWatch.Stop();
            this.ticks = this.stopWatch.ElapsedTicks;

            Console.WriteLine($"Delete method execution duration is {this.ticks} ticks.");
            return result;
        }

        /// <summary>
        /// Restores the specified snapshot.
        /// </summary>
        /// <param name="snapshot">The snapshot.</param>
        /// <param name="exceptions">The exceptions.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot, out Dictionary<int, string> exceptions)
        {
            this.stopWatch.Reset();
            this.stopWatch.Start();
            this.service.Restore(snapshot, out exceptions);

            this.stopWatch.Stop();
            this.ticks = this.stopWatch.ElapsedTicks;

            Console.WriteLine($"Restore method execution duration is {this.ticks} ticks.");
        }
    }
}
