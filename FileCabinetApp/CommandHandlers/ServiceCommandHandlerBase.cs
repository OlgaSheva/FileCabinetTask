using System;
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
        /// The file cabinet service.
        /// </summary>
        protected static IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCommandHandlerBase"/> class.
        /// </summary>
        /// <param name="fileCabinetService">The file cabinet service.</param>
        protected ServiceCommandHandlerBase(IFileCabinetService fileCabinetService)
        {
            service = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
        }
    }
}
