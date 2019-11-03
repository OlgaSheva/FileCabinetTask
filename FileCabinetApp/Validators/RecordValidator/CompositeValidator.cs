using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp.Validators.RecordValidator
{
    /// <summary>
    /// Composited validator.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordValidator" />
    public class CompositeValidator : IRecordValidator
    {
        private readonly List<IRecordValidator> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">The validators.</param>
        internal CompositeValidator(IEnumerable<IRecordValidator> validators)
        {
            this.validators = validators.ToList();
        }

        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public void ValidateParameters(RecordParameters parameters)
        {
            foreach (var validator in this.validators)
            {
                validator.ValidateParameters(parameters);
            }
        }
    }
}
