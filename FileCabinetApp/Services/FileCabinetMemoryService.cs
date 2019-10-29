using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public int CreateRecord(Record rec)
        {
            if (rec == null)
            {
                throw new ArgumentNullException(nameof(rec));
            }

            this.validator.ValidateParameters(rec.FirstName, rec.LastName, rec.DateOfBirth, rec.Gender, rec.Office, rec.Salary);

            int newId = (this.list.Count > 0) ? this.list[this.list.Count - 1].Id + 1 : 1;

            var record = new FileCabinetRecord
            {
                Id = newId,
                FirstName = rec.FirstName,
                LastName = rec.LastName,
                DateOfBirth = rec.DateOfBirth,
                Gender = rec.Gender,
                Office = rec.Office,
                Salary = rec.Salary,
            };

            this.list.Add(record);
            this.AddToDictionaries(record);

            return record.Id;
        }

        /// <summary>
        /// Gets the records.
        /// </summary>
        /// <returns>All existing records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            var copy = new ReadOnlyCollection<FileCabinetRecord>(this.list);
            return copy;
        }

        /// <summary>
        /// Gets the stat.
        /// </summary>
        /// <returns>The quantity of records.</returns>
        public int GetStat()
        {
            return this.list.Count;
        }

        /// <summary>
        /// Edits the record.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="rec">The record.</param>
        /// <exception cref="ArgumentException">
        /// The {nameof(id)} can't be less than zero.
        /// or
        /// The {nameof(id)} doesn't exist.
        /// </exception>
        public void EditRecord(int id, Record rec)
        {
            if (rec == null)
            {
                throw new ArgumentNullException(nameof(rec));
            }

            if (id < 0)
            {
                throw new ArgumentException($"The {nameof(id)} can't be less than zero.", nameof(id));
            }

            if (!this.IsThereARecordWithThisId(id, out int indexInList))
            {
                throw new ArgumentException($"The {nameof(id)} doesn't exist.", nameof(id));
            }

            this.validator.ValidateParameters(rec.FirstName, rec.LastName, rec.DateOfBirth, rec.Gender, rec.Office, rec.Salary);

            this.RemoveFromDictionaries(indexInList);

            this.list[indexInList].FirstName = rec.FirstName;
            this.list[indexInList].LastName = rec.LastName;
            this.list[indexInList].DateOfBirth = rec.DateOfBirth;
            this.list[indexInList].Gender = rec.Gender;
            this.list[indexInList].Office = rec.Office;
            this.list[indexInList].Salary = rec.Salary;

            this.EditDictionaries(rec, indexInList);
        }

        /// <summary>
        /// Finds the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>All records with specified parameters.</returns>
        /// <exception cref="InvalidOperationException">The {parameterName} isn't a search parameter name. Only 'FirstName', 'LastName' or 'DateOfBirth'.</exception>
        /// <exception cref="ArgumentException">The record with {parameterName} '{parameterValue}' doesn't exist.</exception>
        public ReadOnlyCollection<FileCabinetRecord> Find(string parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var param = parameters.Split(' ');
            string parameterName = param[0];
            string parameterValue = param[1].Trim('"');

            var textInfo = new CultureInfo("ru-RU").TextInfo;
            parameterValue = textInfo.ToTitleCase(textInfo.ToLower(parameterValue));
            parameterName = textInfo.ToTitleCase(textInfo.ToLower(parameterName));

            ReadOnlyCollection<FileCabinetRecord> findCollection = null;
            try
            {
                switch (parameterName)
                {
                    case "Firstname":
                        findCollection = this.firstNameDictionary[parameterValue].AsReadOnly();
                        break;
                    case "Lastname":
                        findCollection = this.lastNameDictionary[parameterValue].AsReadOnly();
                        break;
                    case "Dateofbirth":
                        findCollection = this.FindByDateOfBirth(parameterValue);
                        break;
                    default:
                        throw new InvalidOperationException($"The {parameterName} isn't a search parameter name. Only 'FirstName', 'LastName' or 'DateOfBirth'.");
                }
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException($"The record with {parameterName} '{parameterValue}' doesn't exist.");
            }

            return findCollection;
        }

        /// <summary>
        /// Determines whether [is there a record with this identifier] [the specified identifier].
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="index">The index.</param>
        /// <returns>
        ///   <c>true</c> if [is there a record with this identifier] [the specified identifier]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsThereARecordWithThisId(int id, out int index)
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
                    this.validator.ValidateParameters(record.FirstName, record.LastName, record.DateOfBirth, record.Gender, record.Office, record.Salary);
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

        private ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            var dateList = new List<FileCabinetRecord>();
            foreach (var item in this.list)
            {
                if (DateTime.TryParse(dateOfBirth, out DateTime date))
                {
                    if (DateTime.Compare(item.DateOfBirth, date) == 0)
                    {
                        dateList.Add(item);
                    }
                }
            }

            var dateCollection = new ReadOnlyCollection<FileCabinetRecord>(dateList);
            return dateCollection;
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

        private void EditDictionaries(Record rec, int indexInList)
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
    }
}
