﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FileCabinetApp.Enums;
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
            this.CreateFileCabinetRecord(rec, id);
        }

        /// <summary>
        /// Gets the records.
        /// </summary>
        /// <returns>All existing records.</returns>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            foreach (var record in this.list)
            {
                yield return record;
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
        /// <exception cref="ArgumentException">
        /// There are several entries with such parameters.
        /// or
        /// There are no entries with such parameters.
        /// or
        /// There are no entries with such parameters.
        /// or
        /// Record whith firstname = '{firstname}' does not exist.
        /// or
        /// There are no entries with such parameters.
        /// or
        /// Record #{id} does not exist.
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

            int id = 0;
            string firstname;
            string lastname;
            List<FileCabinetRecord> firstnameList = new List<FileCabinetRecord>(),
                                    lastnameList = new List<FileCabinetRecord>();
            if (keyValuePairs["id"] != null)
            {
                id = int.Parse(keyValuePairs["id"], NumberStyles.Integer, CultureInfo.CurrentCulture);
            }
            else if ((firstname = keyValuePairs["firstname"]) != null)
            {
                if (this.firstNameDictionary.TryGetValue(firstname, out firstnameList))
                {
                    lastname = keyValuePairs["lastname"];
                    if (firstnameList.Count == 1 && lastname == null)
                    {
                        id = firstnameList[0].Id;
                    }
                    else if (lastname != null)
                    {
                        foreach (var record in firstnameList)
                        {
                            if (record.LastName.Equals(lastname, StringComparison.InvariantCultureIgnoreCase))
                            {
                                lastnameList.Add(record);
                            }
                        }

                        if (lastnameList.Count == 1)
                        {
                            id = lastnameList[0].Id;
                        }
                        else if (lastnameList.Count > 1)
                        {
                            throw new ArgumentException("There are several entries with such parameters.");
                        }
                        else
                        {
                            throw new ArgumentException("There are no entries with such parameters.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("There are no entries with such parameters.");
                    }
                }
                else
                {
                    throw new ArgumentException($"Record whith firstname = '{firstname}' does not exist.");
                }
            }
            else
            {
                throw new ArgumentException("There are no entries with such parameters.");
            }

            if (this.IsThereARecordWithThisId(id, out long indexInList))
            {
                int index = (int)indexInList;
                this.RemoveFromDictionaries(index);

                this.list[index].FirstName = recordParameters.FirstName ?? this.list[index].FirstName;
                this.list[index].LastName = recordParameters.LastName ?? this.list[index].LastName;
                this.list[index].DateOfBirth = (!recordParameters.DateOfBirth.Equals(default(DateTime)))
                    ? recordParameters.DateOfBirth : this.list[index].DateOfBirth;
                this.list[index].Gender = (!recordParameters.Gender.Equals(default(char)))
                    ? recordParameters.Gender : this.list[index].Gender;
                this.list[index].Office = (recordParameters.Office != -1)
                    ? recordParameters.Office : this.list[index].Office;
                this.list[index].Salary = (recordParameters.Salary != -1)
                    ? recordParameters.Salary : this.list[index].Salary;

                var newRecordParameters = new RecordParameters(
                        this.list[index].FirstName,
                        this.list[index].LastName,
                        this.list[index].DateOfBirth,
                        this.list[index].Gender,
                        this.list[index].Office,
                        this.list[index].Salary);
                this.validator.ValidateParameters(newRecordParameters);
                this.EditDictionaries(newRecordParameters, index);
            }
            else
            {
                throw new ArgumentException($"Record #{id} does not exist.");
            }

            return id;
        }

        /// <summary>
        /// Finds the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>All records with specified parameters.</returns>
        /// <exception cref="InvalidOperationException">The {parameterName} isn't a search parameter name. Only 'FirstName', 'LastName' or 'DateOfBirth'.</exception>
        /// <exception cref="ArgumentException">The record with {parameterName} '{parameterValue}' doesn't exist.</exception>
        public IEnumerable<FileCabinetRecord> Find(string parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var param = parameters.Split(' ', 2);
            string parameterName = param[0];
            string parameterValue = param[1].Trim('"');

            var textInfo = new CultureInfo("ru-RU").TextInfo;
            parameterValue = textInfo.ToTitleCase(textInfo.ToLower(parameterValue));
            parameterName = textInfo.ToTitleCase(textInfo.ToLower(parameterName));

            List<FileCabinetRecord> findedRecords = null;
            try
            {
                switch (parameterName)
                {
                    case "Firstname":
                        findedRecords = this.firstNameDictionary[parameterValue];
                        break;
                    case "Lastname":
                        findedRecords = this.lastNameDictionary[parameterValue];
                        break;
                    case "Dateofbirth":
                        findedRecords = this.FindByDateOfBirth(parameterValue);
                        break;
                    default:
                        throw new InvalidOperationException($"The {parameterName} isn't a search parameter name. Only 'FirstName', 'LastName' or 'DateOfBirth'.");
                }
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException($"The record with {parameterName} '{parameterValue}' doesn't exist.");
            }

            foreach (var record in findedRecords)
            {
                yield return record;
            }
        }

        /// <summary>
        /// Selects the specified key value pairs.
        /// </summary>
        /// <param name="keyValuePairs">The key value pairs.</param>
        /// <param name="condition">The condition.</param>
        /// <returns>
        /// All records with specified parameters.
        /// </returns>
        /// <exception cref="ArgumentNullException">keyValuePairs is null.</exception>
        /// <exception cref="InvalidOperationException">The {key} isn't a search parameter name. Only 'Id', 'FirstName', 'LastName' or 'DateOfBirth'.</exception>
        /// <exception cref="ArgumentException">The record with {key} = '{value}' doesn't exist.</exception>
        public IEnumerable<FileCabinetRecord> SelectRecords(List<KeyValuePair<string, string>> keyValuePairs, SearchCondition condition)
        {
            if (keyValuePairs == null)
            {
                throw new ArgumentNullException(nameof(keyValuePairs));
            }

            List<FileCabinetRecord> findedRecords = new List<FileCabinetRecord>();
            if (keyValuePairs.Count == 0)
            {
                findedRecords.AddRange(this.list);
            }
            else
            {
                string key;
                string value;
                foreach (var criterion in keyValuePairs)
                {
                    key = criterion.Key;
                    value = criterion.Value;
                    try
                    {
                        switch (key)
                        {
                            case "id":
                                int id = int.Parse(value, CultureInfo.CurrentCulture);
                                if (this.IsThereARecordWithThisId(id, out long index))
                                {
                                    findedRecords.Add(this.list[(int)index]);
                                }

                                break;
                            case "firstname":
                                if (condition.Equals(SearchCondition.Or))
                                {
                                    findedRecords.AddRange(this.firstNameDictionary[value]);
                                }
                                else
                                {
                                    if (findedRecords.Count == 0)
                                    {
                                        findedRecords.AddRange(this.firstNameDictionary[value]);
                                    }
                                    else
                                    {
                                        findedRecords = findedRecords.Intersect(this.firstNameDictionary[value]).ToList();
                                    }
                                }

                                break;
                            case "lastname":
                                if (condition.Equals(SearchCondition.Or))
                                {
                                    findedRecords.AddRange(this.lastNameDictionary[value]);
                                }
                                else
                                {
                                    if (findedRecords.Count == 0)
                                    {
                                        findedRecords.AddRange(this.lastNameDictionary[value]);
                                    }
                                    else
                                    {
                                        findedRecords = findedRecords.Intersect(this.lastNameDictionary[value]).ToList();
                                    }
                                }

                                break;
                            case "dateofbirth":
                                if (DateTime.TryParse(value, out DateTime date))
                                {
                                    if (condition.Equals(SearchCondition.Or))
                                    {
                                        findedRecords.AddRange(this.dateOfBirthDictionary[date]);
                                    }
                                    else
                                    {
                                        if (findedRecords.Count == 0)
                                        {
                                            findedRecords.AddRange(this.dateOfBirthDictionary[date]);
                                        }
                                        else
                                        {
                                            findedRecords = findedRecords.Intersect(this.dateOfBirthDictionary[date]).ToList();
                                        }
                                    }
                                }

                                break;
                            default:
                                throw new InvalidOperationException(
                                    $"The {key} isn't a search parameter name. Only 'Id', 'FirstName', 'LastName' or 'DateOfBirth'.");
                        }
                    }
                    catch (KeyNotFoundException knfex)
                    {
                        throw new ArgumentException($"The record with {key} = '{value}' doesn't exist.", knfex.Message);
                    }
                }
            }

            foreach (var record in findedRecords)
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
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var ids = new List<int>();
            bool flag = false;
            for (var i = 0; i < this.list.Count; i++)
            {
                flag = false;
                switch (key.ToLower(CultureInfo.CurrentCulture))
                {
                    case "id":
                        flag = this.list[i].Id == int.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
                        break;
                    case "firstname":
                        flag = this.list[i].FirstName.Equals(value, StringComparison.InvariantCultureIgnoreCase);
                        break;
                    case "lastname":
                        flag = this.list[i].LastName.Equals(value, StringComparison.InvariantCultureIgnoreCase);
                        break;
                    case "dateofbirth":
                        DateTime dateofbirth = DateTime.Parse(value, CultureInfo.InvariantCulture);
                        flag = this.list[i].DateOfBirth == dateofbirth;
                        break;
                    default:
                        throw new ArgumentException($"Search by key '{key}' does not supported.", nameof(key));
                }

                if (flag)
                {
                    ids.Add(this.list[i].Id);
                    this.RemoveFromDictionaries(i);
                    this.list.Remove(this.list[i]);
                }
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
        /// Do nothing.
        /// </summary>
        /// <param name="deletedRecordsCount">The deleted records count.</param>
        /// <param name="recordsCount">The records count.</param>
        public void Purge(out int deletedRecordsCount, out int recordsCount)
        {
            deletedRecordsCount = 0;
            recordsCount = this.list.Count;
        }

        private void EditRecord(int id, RecordParameters rec)
        {
            if (id < 0)
            {
                throw new ArgumentException($"The {nameof(id)} can't be less than zero.", nameof(id));
            }

            if (!this.IsThereARecordWithThisId(id, out long indexInList))
            {
                throw new ArgumentException($"The {nameof(id)} doesn't exist.", nameof(id));
            }

            int index = (int)indexInList;
            this.validator.ValidateParameters(
                new RecordParameters(rec.FirstName, rec.LastName, rec.DateOfBirth, rec.Gender, rec.Office, rec.Salary));

            this.RemoveFromDictionaries(index);

            this.list[index].FirstName = rec.FirstName;
            this.list[index].LastName = rec.LastName;
            this.list[index].DateOfBirth = rec.DateOfBirth;
            this.list[index].Gender = rec.Gender;
            this.list[index].Office = rec.Office;
            this.list[index].Salary = rec.Salary;

            this.EditDictionaries(rec, index);
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

        private List<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
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

            return dateList;
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
    }
}