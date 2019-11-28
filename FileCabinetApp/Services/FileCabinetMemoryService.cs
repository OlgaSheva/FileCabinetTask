using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using FileCabinetApp.Services;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// The file cabinet memory service.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly IRecordValidator validator;
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();
        private List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="validator">The validator.</param>
        public FileCabinetMemoryService(IRecordValidator validator)
        {
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <summary>
        /// Makes the snapshot.
        /// </summary>
        /// <returns>
        /// The file cabinet service snapshot.
        /// </returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            var snapshot = new FileCabinetServiceSnapshot(this.list);
            return snapshot;
        }

        /// <summary>
        /// Restores the specified snapshot.
        /// </summary>
        /// <param name="snapshot">The snapshot.</param>
        /// <param name="exceptions">The exceptions.</param>
        /// <exception cref="ArgumentNullException">Snapshot is null.</exception>
        public void Restore(FileCabinetServiceSnapshot snapshot, out Dictionary<int, string> exceptions)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            var recordsFromFile = snapshot.FileCabinetRecords.ToList();
            List<FileCabinetRecord> newArray = this.GenerateNewListWithExistAndRestoreRecords(recordsFromFile, out exceptions);
            this.list = newArray;
        }

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="rec">The record.</param>
        /// <returns>New file cabinet record.</returns>
        public int CreateRecord(RecordParameters rec)
        {
            if (rec == null)
            {
                throw new ArgumentNullException(nameof(rec));
            }

            int id = (this.list.Count > 0) ? this.list[this.list.Count - 1].Id + 1 : 1;
            this.InsertRecord(rec, id);

            return id;
        }

        /// <summary>
        /// Inserts the record.
        /// </summary>
        /// <param name="rec">The record.</param>
        /// <param name="id">The identifier.</param>
        /// <exception cref="ArgumentNullException">rec is null.</exception>
        /// <exception cref="ArgumentException">The id can not be less than one.</exception>
        public void InsertRecord(RecordParameters rec, int id)
        {
            if (rec == null)
            {
                throw new ArgumentNullException(nameof(rec));
            }

            if (id < 1)
            {
                throw new ArgumentException($"The '{nameof(id)}' can not be less than one.", nameof(id));
            }

            this.validator.ValidateParameters(rec);
            if (!this.IsThereARecordWithThisId(id))
            {
                this.CreateFileCabinetRecord(rec, id);
            }
            else
            {
                throw new ArgumentException($"The record #{id} is already exist.", nameof(id));
            }
        }

        /// <summary>
        /// Gets the stat.
        /// </summary>
        /// <param name="deletedRecordsCount">The deleted records count.</param>
        /// <returns>The quantity of records.</returns>
        public int GetStat(out int deletedRecordsCount)
        {
            deletedRecordsCount = 0;
            return this.list.Count;
        }

        /// <summary>
        /// Updates the specified records to update.
        /// </summary>
        /// <param name="recordsToUpdate">The records to update.</param>
        /// <param name="recordParameters">The record parameters.</param>
        /// <param name="keyValuePairs">The key value pairs.</param>
        /// <returns>
        /// IDs of updated records.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// recordsToUpdate
        /// or
        /// recordParameters
        /// or
        /// keyValuePairs.
        /// </exception>
        public List<int> Update(IEnumerable<FileCabinetRecord> recordsToUpdate, RecordParameters recordParameters, List<KeyValuePair<string, string>> keyValuePairs)
        {
            if (recordsToUpdate == null)
            {
                throw new ArgumentNullException(nameof(recordsToUpdate));
            }

            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            if (keyValuePairs == null)
            {
                throw new ArgumentNullException(nameof(keyValuePairs));
            }

            List<int> ids = new List<int>();
            int id;
            foreach (var record in recordsToUpdate)
            {
                id = record.Id;
                this.UpdateRecord(recordParameters, id);
                ids.Add(id);
            }

            return ids;
        }

        /// <summary>
        /// Gets the records.
        /// </summary>
        /// <returns>
        /// All records.
        /// </returns>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            foreach (var record in this.list)
            {
                yield return record;
            }
        }

        /// <summary>
        /// Deletes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// Deleted records id.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// key
        /// or
        /// value.
        /// </exception>
        /// <exception cref="ArgumentException">Search by key '{key}' does not supported.</exception>
        public List<int> Delete(string key, string value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var ids = new List<int>();
            var indexes = new List<int>();
            bool flag = false;
            for (var i = 0; i < this.list.Count; i++)
            {
                flag = false;
                switch (key.ToUpperInvariant())
                {
                    case "ID":
                        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int id))
                        {
                            flag = this.list[i].Id == int.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            throw new ArgumentException("Wrong id format.", nameof(value));
                        }

                        break;
                    case "FIRSTNAME":
                        flag = this.list[i].FirstName.Equals(value, StringComparison.InvariantCultureIgnoreCase);
                        break;
                    case "LASTNAME":
                        flag = this.list[i].LastName.Equals(value, StringComparison.InvariantCultureIgnoreCase);
                        break;
                    case "DATEOFBIRTH":
                        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                        {
                            flag = this.list[i].DateOfBirth == date;
                        }
                        else
                        {
                            throw new ArgumentException("You can use only the MM.DD.YYYY, MM/DD/YYYY, YYYY-MM-DD, format", nameof(value));
                        }

                        break;
                    default:
                        throw new ArgumentException($"Search by key '{key}' does not supported.", nameof(key));
                }

                if (flag)
                {
                    ids.Add(this.list[i].Id);
                    indexes.Add(i);
                }
            }

            for (int i = indexes.Count - 1; i >= 0; i--)
            {
                this.RemoveFromDictionaries(indexes[i]);
                this.list.Remove(this.list[indexes[i]]);
            }

            return ids;
        }

        /// <summary>
        /// Determines whether [is there a record with this identifier] [the specified identifier].
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="index">The index.</param>
        /// <returns>
        ///   <c>true</c> if [is there a record with this identifier] [the specified identifier]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsThereARecordWithThisId(int id, out long index)
        {
            index = -1;
            for (int i = 0; i < this.list.Count; i++)
            {
                if (this.list[i].Id == id)
                {
                    index = i;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether [is there a record with this identifier] [the specified identifier].
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        ///   <c>true</c> if [is there a record with this identifier] [the specified identifier]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsThereARecordWithThisId(int id)
        {
            for (int i = 0; i < this.list.Count; i++)
            {
                if (this.list[i].Id == id)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Purges the specified records count.
        /// </summary>
        /// <param name="recordsCount">The records count.</param>
        /// <returns>
        /// deleted records count.
        /// </returns>
        public int Purge(out int recordsCount)
        {
            int deletedRecordsCount = 0;
            recordsCount = this.list.Count;
            return deletedRecordsCount;
        }

        private void CreateFileCabinetRecord(RecordParameters rec, int id)
        {
            this.validator.ValidateParameters(rec);
            var record = new FileCabinetRecord
            {
                Id = id,
                FirstName = rec.FirstName,
                LastName = rec.LastName,
                DateOfBirth = rec.DateOfBirth,
                Gender = rec.Gender,
                Office = rec.Office,
                Salary = rec.Salary,
            };

            this.list.Add(record);
            this.AddToDictionaries(record);
        }

        private List<FileCabinetRecord> GenerateNewListWithExistAndRestoreRecords(
            List<FileCabinetRecord> recordsFromFile, out Dictionary<int, string> exceptions)
        {
            exceptions = new Dictionary<int, string>();
            List<FileCabinetRecord> newList = new List<FileCabinetRecord>(this.list);

            int indexFileRecord = 0;
            int indexListRecord = 0;
            bool flag = false;
            int matchingIdPosition = -1;
            foreach (var fileRecord in recordsFromFile)
            {
                indexListRecord = 0;
                flag = false;
                matchingIdPosition = -1;
                foreach (var listRecord in this.list)
                {
                    if (fileRecord.Id == listRecord.Id)
                    {
                        flag = true;
                        matchingIdPosition = indexListRecord;
                    }

                    indexListRecord++;
                }

                try
                {
                    var record = fileRecord;
                    if (record.Id <= 0)
                    {
                        throw new ArgumentException(nameof(record.Id));
                    }

                    this.validator.ValidateParameters(
                        new RecordParameters(record.FirstName, record.LastName, record.DateOfBirth, record.Gender, record.Office, record.Salary));
                    if (flag)
                    {
                        this.RemoveFromDictionaries(matchingIdPosition);
                        newList[matchingIdPosition] = fileRecord;
                        this.AddToDictionaries(fileRecord);
                    }
                    else
                    {
                        newList.Add(record);
                        this.AddToDictionaries(record);
                    }
                }
                catch (ArgumentException ex)
                {
                    exceptions.Add(fileRecord.Id, ex.Message);
                }

                indexFileRecord++;
            }

            return newList;
        }

        private void AddToDictionaries(FileCabinetRecord record)
        {
            if (!this.firstNameDictionary.ContainsKey(record.FirstName))
            {
                this.firstNameDictionary.Add(record.FirstName, new List<FileCabinetRecord>());
            }

            if (!this.lastNameDictionary.ContainsKey(record.LastName))
            {
                this.lastNameDictionary.Add(record.LastName, new List<FileCabinetRecord>());
            }

            if (!this.dateOfBirthDictionary.ContainsKey(record.DateOfBirth))
            {
                this.dateOfBirthDictionary.Add(record.DateOfBirth, new List<FileCabinetRecord>());
            }

            this.firstNameDictionary[record.FirstName].Add(record);
            this.lastNameDictionary[record.LastName].Add(record);
            this.dateOfBirthDictionary[record.DateOfBirth].Add(record);
        }

        private void RemoveFromDictionaries(int index)
        {
            this.firstNameDictionary[this.list[index].FirstName].Remove(this.list[index]);
            this.lastNameDictionary[this.list[index].LastName].Remove(this.list[index]);
            this.dateOfBirthDictionary[this.list[index].DateOfBirth].Remove(this.list[index]);
        }

        private void EditDictionaries(RecordParameters rec, int indexInList)
        {
            if (!this.firstNameDictionary.ContainsKey(rec.FirstName))
            {
                this.firstNameDictionary.Add(rec.FirstName, new List<FileCabinetRecord>());
            }

            if (!this.lastNameDictionary.ContainsKey(rec.LastName))
            {
                this.lastNameDictionary.Add(rec.LastName, new List<FileCabinetRecord>());
            }

            if (!this.dateOfBirthDictionary.ContainsKey(rec.DateOfBirth))
            {
                this.dateOfBirthDictionary.Add(rec.DateOfBirth, new List<FileCabinetRecord>());
            }

            this.firstNameDictionary[rec.FirstName].Add(this.list[indexInList]);
            this.lastNameDictionary[rec.LastName].Add(this.list[indexInList]);
            this.dateOfBirthDictionary[rec.DateOfBirth].Add(this.list[indexInList]);
        }

        private void UpdateRecord(RecordParameters recordParameters, int id)
        {
            if (this.IsThereARecordWithThisId(id, out long indexInList))
            {
                int index = (int)indexInList;
                var newRecordParameters = new RecordParameters(
                        recordParameters.FirstName ?? this.list[index].FirstName,
                        recordParameters.LastName ?? this.list[index].LastName,
                        (!recordParameters.DateOfBirth.Equals(default(DateTime))) ? recordParameters.DateOfBirth : this.list[index].DateOfBirth,
                        (!recordParameters.Gender.Equals(default(char))) ? recordParameters.Gender : this.list[index].Gender,
                        (recordParameters.Office != -1) ? recordParameters.Office : this.list[index].Office,
                        (recordParameters.Salary != -1) ? recordParameters.Salary : this.list[index].Salary);
                this.validator.ValidateParameters(newRecordParameters);

                this.RemoveFromDictionaries(index);

                this.list[index].FirstName = newRecordParameters.FirstName;
                this.list[index].LastName = newRecordParameters.LastName;
                this.list[index].DateOfBirth = newRecordParameters.DateOfBirth;
                this.list[index].Gender = newRecordParameters.Gender;
                this.list[index].Office = newRecordParameters.Office;
                this.list[index].Salary = newRecordParameters.Salary;

                this.EditDictionaries(newRecordParameters, index);
            }
            else
            {
                throw new ArgumentException($"Record #{id} does not exist.");
            }
        }
    }
}