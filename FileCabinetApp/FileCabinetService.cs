using System;
using System.Collections.Generic;
using System.Globalization;
using FileCabinetApp.Enums;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private static readonly DateTime MinDate = new DateTime(1950, 1, 1);
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, Gender gender, char materialStatus, short catsCount = 0, decimal catsBudget = 0)
        {
            this.Validation(firstName, lastName, dateOfBirth, gender, materialStatus, catsCount, catsBudget);

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Gender = gender,
                MaritalStatus = materialStatus,
                CatsCount = catsCount,
                CatsBudget = catsBudget,
            };

            this.list.Add(record);

            if (!this.firstNameDictionary.ContainsKey(firstName))
            {
                this.firstNameDictionary.Add(firstName, new List<FileCabinetRecord>());
            }

            if (!this.lastNameDictionary.ContainsKey(lastName))
            {
                this.lastNameDictionary.Add(lastName, new List<FileCabinetRecord>());
            }

            if (!this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                this.dateOfBirthDictionary.Add(dateOfBirth, new List<FileCabinetRecord>());
            }

            this.firstNameDictionary[firstName].Add(record);
            this.lastNameDictionary[lastName].Add(record);
            this.dateOfBirthDictionary[dateOfBirth].Add(record);

            return record.Id;
        }

        public FileCabinetRecord[] GetRecords()
        {
            var copy = new List<FileCabinetRecord>(this.list);
            return copy.ToArray();
        }

        public int GetStat()
        {
            return this.list.Count;
        }

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, Gender gender, char materialStatus, short catsCount, decimal catsBudget)
        {
            if (id < 0)
            {
                throw new ArgumentException($"The {nameof(id)} can't be less than zero.");
            }

            if (!this.IsThereARecordWithThisId(id, out int index))
            {
                throw new ArgumentException($"The {nameof(id)} doesn't exist.");
            }

            this.Validation(firstName, lastName, dateOfBirth, gender, materialStatus, catsCount, catsBudget);

            this.firstNameDictionary[this.list[index].FirstName].Remove(this.list[index]);
            this.lastNameDictionary[this.list[index].LastName].Remove(this.list[index]);
            this.dateOfBirthDictionary[this.list[index].DateOfBirth].Remove(this.list[index]);

            this.list[index].FirstName = firstName;
            this.list[index].LastName = lastName;
            this.list[index].DateOfBirth = dateOfBirth;
            this.list[index].Gender = gender;
            this.list[index].MaritalStatus = materialStatus;
            this.list[index].CatsCount = catsCount;
            this.list[index].CatsBudget = catsBudget;

            if (!this.firstNameDictionary.ContainsKey(firstName))
            {
                this.firstNameDictionary.Add(firstName, new List<FileCabinetRecord>());
            }

            if (!this.lastNameDictionary.ContainsKey(lastName))
            {
                this.lastNameDictionary.Add(lastName, new List<FileCabinetRecord>());
            }

            if (!this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                this.dateOfBirthDictionary.Add(dateOfBirth, new List<FileCabinetRecord>());
            }

            this.firstNameDictionary[firstName].Add(this.list[index]);
            this.lastNameDictionary[lastName].Add(this.list[index]);
            this.dateOfBirthDictionary[dateOfBirth].Add(this.list[index]);
        }

        public FileCabinetRecord[] Find(string parameters)
        {
            var param = parameters.Split(' ');
            string parameterName = param[0];
            string parameterValue = param[1].Trim('"');

            var textInfo = new CultureInfo("ru-RU").TextInfo;
            parameterValue = textInfo.ToTitleCase(textInfo.ToLower(parameterValue));

            FileCabinetRecord[] findList = null;
            try
            {
                switch (parameterName.ToLower())
                {
                    case "firstname":
                        findList = this.firstNameDictionary[parameterValue].ToArray();
                        break;
                    case "lastname":
                        findList = this.lastNameDictionary[parameterValue].ToArray();
                        break;
                    case "dateofbirth":
                        findList = this.FindByDateOfBirth(parameterValue);
                        break;
                    default:
                        throw new InvalidOperationException($"The {parameterName} isn't a search parameter name. Only 'FirstName', 'LastName' or 'DateOfBirth'.");
                }
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException($"The record with {parameterName} '{parameterValue}' doesn't exist.");
            }

            return findList;
        }

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

        private static bool ConsistsOfSpaces(string @string)
        {
            foreach (var item in @string)
            {
                if (item != ' ')
                {
                    return false;
                }
            }

            return true;
        }

        private FileCabinetRecord[] FindByDateOfBirth(string dateOfBirth)
        {
            var nameList = new List<FileCabinetRecord>();
            foreach (var item in this.list)
            {
                if (DateTime.TryParse(dateOfBirth, out DateTime date))
                {
                    if (DateTime.Compare(item.DateOfBirth, date) == 0)
                    {
                        nameList.Add(item);
                    }
                }
            }

            return nameList.ToArray();
        }

        private void Validation(string firstName, string lastName, DateTime dateOfBirth, Gender gender, char materialStatus, short catsCount, decimal catsBudget)
        {
            if (firstName == null)
            {
                throw new ArgumentNullException($"The {nameof(firstName)} can't be null.");
            }

            if (lastName == null)
            {
                throw new ArgumentNullException($"The {nameof(lastName)} can't be null.");
            }

            if (firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException($"The {nameof(firstName)} length can't be less than 2 symbols and larger than 60 symbols.");
            }

            if (ConsistsOfSpaces(firstName))
            {
                throw new ArgumentException($"The {nameof(firstName)} can't consists only from spases.");
            }

            if (lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException($"The {nameof(lastName)} length can't be less than 2 symbols and larger than 60 symbols.");
            }

            if (ConsistsOfSpaces(lastName))
            {
                throw new ArgumentException($"The {nameof(lastName)} can't consists only from spases.");
            }

            if (dateOfBirth == null)
            {
                throw new ArgumentNullException($"The {nameof(dateOfBirth)} can't be null.");
            }

            if (dateOfBirth > DateTime.Today || dateOfBirth < MinDate)
            {
                throw new ArgumentException($"The {nameof(dateOfBirth)} can't be less than 01-Jan-1950 and larger than the current date.");
            }

            if (gender != Gender.M && gender != Gender.F && gender != Gender.O && gender != Gender.U)
            {
                throw new ArgumentException($"The {nameof(gender)} can be only 'M', 'F', 'O' or 'U'.");
            }

            if (materialStatus != 'M' && materialStatus != 'U')
            {
                throw new ArgumentNullException($"The {nameof(materialStatus)} isn't a valid material status. You can use only 'M' or 'U' symbols.");
            }

            if (catsCount < 0 || catsCount > 100)
            {
                throw new ArgumentException($"The {nameof(catsCount)} can't be less than 0 or larger than 50.");
            }

            if (catsBudget < 0)
            {
                throw new ArgumentException($"The {nameof(catsBudget)} can't be less than zero.");
            }
        }
    }
}
