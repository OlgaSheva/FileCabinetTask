using System;

namespace FileCabinetApp.Validators.RecordValidator
{
    /// <summary>
    /// Default validator class.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordValidator" />
    public class DefaultValidator : CompositeValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultValidator"/> class.
        /// </summary>
        public DefaultValidator()
            : base(new IRecordValidator[]
            {
                new FirstNameValidator(2, 60),
                new LastNameValidator(2, 60),
                new DateOfBirthValidator(new DateTime(1950, 1, 1), new DateTime(2010, 1, 1)),
                new GenderValidator(new char[] { 'M', 'F', 'O', 'U' }),
                new OfficeValidator(0, 500),
                new SalaryValidator(0, 10000),
            })
        {
        }
    }
}
