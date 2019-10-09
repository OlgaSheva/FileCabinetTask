using System;
using System.Collections.Generic;
using FileCabinetApp.Enums;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private static readonly DateTime MinDate = new DateTime(1950, 1, 1);
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, Gender gender, char materialStatus, short catsCount, decimal catsBudget)
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

            if (this.ConsistsOfSpaces(firstName))
            {
                throw new ArgumentException($"The {nameof(firstName)} can't consists only from spases.");
            }

            if (lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException($"The {nameof(lastName)} length can't be less than 2 symbols and larger than 60 symbols.");
            }

            if (this.ConsistsOfSpaces(lastName))
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

            if (gender != Gender.Female || gender != Gender.Male || gender != Gender.Other || gender != Gender.Unknown)
            {
                throw new ArgumentException($"The {nameof(gender)} can be only 'Male', 'Female', 'Other' or 'Unknown'.");
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

        private bool ConsistsOfSpaces(string @string)
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
    }
}
