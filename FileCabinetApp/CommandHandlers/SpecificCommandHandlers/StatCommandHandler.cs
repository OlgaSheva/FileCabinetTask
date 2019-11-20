using System;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Stat command handler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    internal class StatCommandHandler : ServiceCommandHandlerBase
    {
        private static Action<string> write;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="writeDelegate">The write delegate.</param>
        public StatCommandHandler(IFileCabinetService service, Action<string> writeDelegate)
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
            if (request.Command == "stat")
            {
                this.Stat(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private void Stat(string parameters)
        {
            var recordsCount = this.Service.GetStat(out int deletedRecordsCount);
            write($"{recordsCount} record(s). Number of deleted records: {deletedRecordsCount}.");
        }
    }
}
