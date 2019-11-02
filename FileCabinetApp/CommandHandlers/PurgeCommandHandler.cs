using System;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    internal class PurgeCommandHandler : CommandHandlerBase
    {
        private static IFileCabinetService fileCabinetService;

        public PurgeCommandHandler(IFileCabinetService service)
        {
            fileCabinetService = service;
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
            fileCabinetService.Purge(out int deletedRecordsCount, out int recordsCount);
            if (fileCabinetService is FileCabinetFilesystemService)
            {
                Console.WriteLine($"Data file processing is completed: {deletedRecordsCount} of {recordsCount} records were purged.");
            }
        }
    }
}
