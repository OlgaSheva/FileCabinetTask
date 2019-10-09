using System;
using FileCabinetApp.Enums;

namespace FileCabinetApp
{
    public class FileCabinetRecord
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        public char MaritalStatus { get; set; } // 'M' - married, 'U' - unmarried

        public short CatsCount { get; set; }

        public decimal CatsBudget { get; set; }
    }
}
