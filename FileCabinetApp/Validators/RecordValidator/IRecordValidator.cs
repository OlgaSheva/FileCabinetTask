namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Validator.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        void ValidateParameters(RecordParameters parameters);
    }
}
