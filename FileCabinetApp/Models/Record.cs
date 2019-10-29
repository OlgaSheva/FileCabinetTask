using System;

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
        private readonly char gender;
        private readonly short office;
        private readonly decimal salary;

        /// <summary>
        /// Initializes a new instance of the <see cref="Record"/> class.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <param name="gender">The gender.</param>
        /// <param name="office">The office.</param>
        /// <param name="salary">The salary.</param>
        public Record(string firstName, string lastName, DateTime dateOfBirth, char gender, short office, decimal salary)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.dateOfBirth = dateOfBirth;
            this.gender = gender;
            this.office = office;
            this.salary = salary;
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
        public char Gender { get => this.gender; }

        /// <summary>
        /// Gets the cats count.
        /// </summary>
        /// <value>
        /// The cats count.
        /// </value>
        public short Office { get => this.office; }

        /// <summary>
        /// Gets the cats budget.
        /// </summary>
        /// <value>
        /// The cats budget.
        /// </value>
        public decimal Salary { get => this.salary; }
    }
}
