using System;
using System.Collections.Generic;
using FileCabinetApp.Enums;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, Gender gender, char materialStatus, short catsCount, decimal catsBudget)
        {
            if (firstName == null || lastName == null || dateOfBirth == null)
            {
                throw new ArgumentNullException($"The {nameof(firstName)}, the {nameof(lastName)}, " +
                    $"the {nameof(dateOfBirth)} or the {nameof(materialStatus)} can't be null.");
            }

            if (firstName.Length == 0 || lastName.Length == 0)
            {
                throw new ArgumentException($"The {nameof(firstName)} and the {nameof(lastName)} can't be empty.");
            }

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
    }
}
