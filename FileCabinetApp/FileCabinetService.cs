using System;
using System.Collections.Generic;
using FileCabinetApp.Enums;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private static readonly DateTime MinDate = new DateTime(1950, 1, 1);
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, Gender gender, char materialStatus, short catsCount, decimal catsBudget)
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

            this.firstNameDictionary[firstName].Add(record);

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

            bool flag = false;
            int index = 0;
            for (int i = 0; i < this.list.Count; i++)
            {
                if (this.list[i].Id == id)
                {
                    flag = true;
                    index = i;
                }
            }

            if (!flag)
            {
                throw new ArgumentException($"The {nameof(id)} doesn't exist.");
            }

            this.Validation(firstName, lastName, dateOfBirth, gender, materialStatus, catsCount, catsBudget);

            this.firstNameDictionary[this.list[index].FirstName].Remove(this.list[index]);

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

            this.firstNameDictionary[firstName].Add(this.list[index]);
        }

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            return this.firstNameDictionary[firstName].ToArray();
        }

        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            var nameList = new List<FileCabinetRecord>();
            foreach (var item in this.list)
            {
                if (item.LastName.ToLower() == lastName.ToLower())
                {
                    nameList.Add(item);
                }
            }

            return nameList.ToArray();
        }

        public FileCabinetRecord[] FindByDateOfBirth(string dateOfBirth)
        {
            var nameList = new List<FileCabinetRecord>();
            foreach (var item in this.list)
            {
                if (DateTime.TryParse(dateOfBirth, out DateTime date))
                {
                    if (item.DateOfBirth == date)
                    {
                        nameList.Add(item);
                    }
                }
            }

            return nameList.ToArray();
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
