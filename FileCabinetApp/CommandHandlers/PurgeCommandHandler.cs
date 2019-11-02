using System;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    internal class PurgeCommandHandler : CommandHandlerBase
    {
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request.Command == "purge")
            {
                Purge(request.Parameters);
            }

            return base.Handle(request);
        }

        private static void Purge(string parameters)
        {
            Program.fileCabinetService.Purge(out int deletedRecordsCount, out int recordsCount);
            if (Program.fileCabinetService is FileCabinetFilesystemService)
            {
                Console.WriteLine($"Data file processing is completed: {deletedRecordsCount} of {recordsCount} records were purged.");
            }
        }
    }
}
