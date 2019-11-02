using System;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    internal class StatCommandHandler : ServiceCommandHandlerBase
    {
        public StatCommandHandler(IFileCabinetService service)
            : base(service)
        {
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
            var recordsCount = service.GetStat(out int deletedRecordsCount);
            Console.WriteLine($"{recordsCount} record(s). Number of deleted records: {deletedRecordsCount}.");
        }
    }
}
