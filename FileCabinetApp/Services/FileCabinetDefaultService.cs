using FileCabinetApp.Validators;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// The file cabinet custom service with default validation.
    /// </summary>
    /// <seealso cref="FileCabinetApp.FileCabinetService" />
    public class FileCabinetDefaultService : FileCabinetService
    {
        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <returns>
        /// The validator.
        /// </returns>
        public override IRecordValidator CreateValidator()
        {
            return new DefaultValidator();
        }
    }
}
