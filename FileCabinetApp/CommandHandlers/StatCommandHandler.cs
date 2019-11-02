using System;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    internal class StatCommandHandler : CommandHandlerBase
    {
        private static IFileCabinetService fileCabinetService;

        public StatCommandHandler(IFileCabinetService service)
        {
            fileCabinetService = service;
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request.Command == "stat")
            {
                Stat(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private static void Stat(string parameters)
        {
            var recordsCount = fileCabinetService.GetStat(out int deletedRecordsCount);
            Console.WriteLine($"{recordsCount} record(s). Number of deleted records: {deletedRecordsCount}.");
        }
    }
}
