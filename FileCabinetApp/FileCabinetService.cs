using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth)
        {
            if (firstName == null || lastName == null || dateOfBirth == null)
            {
                throw new ArgumentNullException($"The {nameof(firstName)}, the {nameof(lastName)} or the {nameof(dateOfBirth)} can't be null.");
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
