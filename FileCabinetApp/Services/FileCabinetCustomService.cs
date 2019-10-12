using FileCabinetApp.Validators;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// The file cabinet service with custom validation.
    /// </summary>
    /// <seealso cref="FileCabinetApp.FileCabinetService" />
    public class FileCabinetCustomService : FileCabinetService
    {
        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <returns>
        /// The validator.
        /// </returns>
        public override IRecordValidator CreateValidator()
        {
            return new CustomValidator();
        }
    }
}
