using System;
using FileCabinetApp.Enums;

namespace FileCabinetApp
{
    /// <summary>
    /// Class record for transmission to FileCabinetService.CreateRecord and FileCabinetService.EditRecord.
    /// </summary>
    public class Record
    {
        private readonly string firstName;
        private readonly string lastName;
        private readonly DateTime dateOfBirth;
        private readonly Gender gender;
        private readonly char materialStatus;
        private readonly short catsCount;
        private readonly decimal catsBudget;

        /// <summary>
        /// Initializes a new instance of the <see cref="Record"/> class.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <param name="gender">The gender.</param>
        /// <param name="materialStatus">The material status.</param>
        /// <param name="catsCount">The cats count.</param>
        /// <param name="catsBudget">The cats budget.</param>
        public Record(string firstName, string lastName, DateTime dateOfBirth, Gender gender, char materialStatus, short catsCount = 0, decimal catsBudget = 0)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.dateOfBirth = dateOfBirth;
            this.gender = gender;
            this.materialStatus = materialStatus;
            this.catsCount = catsCount;
            this.catsBudget = catsBudget;
        }

        /// <summary>
        /// Gets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName { get => this.firstName; }

        /// <summary>
        /// Gets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName { get => this.lastName; }

        /// <summary>
        /// Gets the date of birth.
        /// </summary>
        /// <value>
        /// The date of birth.
        /// </value>
        public DateTime DateOfBirth { get => this.dateOfBirth; }

        /// <summary>
        /// Gets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        public Gender Gender { get => this.gender; }

        /// <summary>
        /// Gets the material status.
        /// </summary>
        /// <value>
        /// The material status.
        /// </value>
        public char MaterialStatus { get => this.materialStatus; }

        /// <summary>
        /// Gets the cats count.
        /// </summary>
        /// <value>
        /// The cats count.
        /// </value>
        public short CatsCount { get => this.catsCount; }

        /// <summary>
        /// Gets the cats budget.
        /// </summary>
        /// <value>
        /// The cats budget.
        /// </value>
        public decimal CatsBudget { get => this.catsBudget; }
    }
}
