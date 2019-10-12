using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// The file cabinet service.
    /// </summary>
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();
        private readonly IRecordValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetService"/> class.
        /// </summary>
        /// <param name="validator">The validator.</param>
        public FileCabinetService(IRecordValidator validator)
        {
            this.validator = validator;
        }

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="rec">The record.</param>
        /// <returns>New file cabinet record.</returns>
        public int CreateRecord(Record rec)
        {
            this.validator.ValidateParameters(rec.FirstName, rec.LastName, rec.DateOfBirth, rec.Gender, rec.MaterialStatus, rec.CatsCount, rec.CatsBudget);

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = rec.FirstName,
                LastName = rec.LastName,
                DateOfBirth = rec.DateOfBirth,
                Gender = rec.Gender,
                MaritalStatus = rec.MaterialStatus,
                CatsCount = rec.CatsCount,
                CatsBudget = rec.CatsBudget,
            };

            this.list.Add(record);

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

            this.firstNameDictionary[rec.FirstName].Add(record);
            this.lastNameDictionary[rec.LastName].Add(record);
            this.dateOfBirthDictionary[rec.DateOfBirth].Add(record);

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
            if (id < 0)
            {
                throw new ArgumentException($"The {nameof(id)} can't be less than zero.");
            }

            if (!this.IsThereARecordWithThisId(id, out int index))
            {
                throw new ArgumentException($"The {nameof(id)} doesn't exist.");
            }

            this.validator.ValidateParameters(rec.FirstName, rec.LastName, rec.DateOfBirth, rec.Gender, rec.MaterialStatus, rec.CatsCount, rec.CatsBudget);

            this.firstNameDictionary[this.list[index].FirstName].Remove(this.list[index]);
            this.lastNameDictionary[this.list[index].LastName].Remove(this.list[index]);
            this.dateOfBirthDictionary[this.list[index].DateOfBirth].Remove(this.list[index]);

            this.list[index].FirstName = rec.FirstName;
            this.list[index].LastName = rec.LastName;
            this.list[index].DateOfBirth = rec.DateOfBirth;
            this.list[index].Gender = rec.Gender;
            this.list[index].MaritalStatus = rec.MaterialStatus;
            this.list[index].CatsCount = rec.CatsCount;
            this.list[index].CatsBudget = rec.CatsBudget;

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

            this.firstNameDictionary[rec.FirstName].Add(this.list[index]);
            this.lastNameDictionary[rec.LastName].Add(this.list[index]);
            this.dateOfBirthDictionary[rec.DateOfBirth].Add(this.list[index]);
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
            var param = parameters.Split(' ');
            string parameterName = param[0];
            string parameterValue = param[1].Trim('"');

            var textInfo = new CultureInfo("ru-RU").TextInfo;
            parameterValue = textInfo.ToTitleCase(textInfo.ToLower(parameterValue));

            ReadOnlyCollection<FileCabinetRecord> findCollection = null;
            try
            {
                switch (parameterName.ToLower())
                {
                    case "firstname":
                        findCollection = this.firstNameDictionary[parameterValue].AsReadOnly();
                        break;
                    case "lastname":
                        findCollection = this.lastNameDictionary[parameterValue].AsReadOnly();
                        break;
                    case "dateofbirth":
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
        internal bool IsThereARecordWithThisId(int id, out int index)
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
    }
}
