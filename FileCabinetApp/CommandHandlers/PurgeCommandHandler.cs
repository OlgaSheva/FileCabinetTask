using System;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    internal class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        public PurgeCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request.Command == "purge")
            {
                Purge(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private static void Purge(string parameters)
        {
            service.Purge(out int deletedRecordsCount, out int recordsCount);
            if (service is FileCabinetFilesystemService)
            {
                Console.WriteLine($"Data file processing is completed: {deletedRecordsCount} of {recordsCount} records were purged.");
            }
        }
    }
}
