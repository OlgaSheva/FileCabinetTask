using System;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Purge command handler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    internal class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        private static Action<string> write;

        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="writeDelegate">The write delegate.</param>
        public PurgeCommandHandler(IFileCabinetService service, Action<string> writeDelegate)
            : base(service)
        {
            write = writeDelegate;
        }

        /// <summary>
        /// Handles the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// Class AppCommandRequest Instance.
        /// </returns>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Command == "purge")
            {
                this.Purge(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private void Purge(string parameters)
        {
            this.Service.Purge(out int deletedRecordsCount, out int recordsCount);
            write($"Data file processing is completed: {deletedRecordsCount} of {recordsCount} records were purged.");
        }
    }
}