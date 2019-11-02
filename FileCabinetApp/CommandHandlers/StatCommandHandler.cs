using System;

namespace FileCabinetApp.CommandHandlers
{
    internal class StatCommandHandler : CommandHandlerBase
    {
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request.Command == "stat")
            {
                Stat(request.Parameters);
            }

            return base.Handle(request);
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat(out int deletedRecordsCount);
            Console.WriteLine($"{recordsCount} record(s). Number of deleted records: {deletedRecordsCount}.");
        }
    }
}
