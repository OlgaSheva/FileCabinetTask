using System;

namespace FileCabinetApp.Validators.RecordValidator
{
    /// <summary>
    /// Custom validator class.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordValidator" />
    public class CustomValidator : CompositeValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomValidator"/> class.
        /// </summary>
        public CustomValidator()
            : base(new IRecordValidator[]
            {
                new FirstNameValidator(2, 30),
                new LastNameValidator(2, 30),
                new DateOfBirthValidator(new DateTime(1930, 1, 1), new DateTime(2000, 1, 1)),
                new GenderValidator(new char[] { 'M', 'F' }),
                new OfficeValidator(100, 300),
                new SalaryValidator(100, 7000),
            })
        {
        }
    }
}
