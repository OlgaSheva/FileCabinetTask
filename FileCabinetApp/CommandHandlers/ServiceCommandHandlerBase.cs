using System;
using System.Collections.Generic;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The service command handler base.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.CommandHandlerBase" />
    public abstract class ServiceCommandHandlerBase : CommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCommandHandlerBase"/> class.
        /// </summary>
        /// <param name="fileCabinetService">The file cabinet service.</param>
        /// <exception cref="ArgumentNullException">The fileCabinetService is null.</exception>
        protected ServiceCommandHandlerBase(IFileCabinetService fileCabinetService)
        {
            this.Service = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
        }

        /// <summary>
        /// Gets the separator.
        /// </summary>
        /// <value>
        /// The separator.
        /// </value>
        protected List<char> Separator { get; } = new List<char>() { ' ', ',', ':', ';', '-', '=', '(', ')', '\'', '!', '?', '\t' };

        /// <summary>
        /// Gets the names of record elements.
        /// </summary>
        /// <value>
        /// The names of record elements.
        /// </value>
        protected List<string> NamesOfRecordElements { get; } = new List<string>
                {
                    "ID",
                    "FIRSTNAME",
                    "LASTNAME",
                    "DATEOFBIRTH",
                    "GENDER",
                    "OFFICE",
                    "SALARY",
                };

        /// <summary>
        /// Gets or sets the service.
        /// </summary>
        /// <value>
        /// The service.
        /// </value>
        protected IFileCabinetService Service { get; set; }
    }
}
