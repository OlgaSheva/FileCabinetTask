using System;
using System.Collections.ObjectModel;

namespace FileCabinetApp.CommandHandlers
{
    internal class ListCommandHandler : CommandHandlerBase
    {
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request.Command == "list")
            {
                List(request.Parameters);
            }

            return base.Handle(request);
        }

        private static void List(string parameters)
        {
            ReadOnlyCollection<FileCabinetRecord> fileCabinetRecords = Program.fileCabinetService.GetRecords();
            Print(fileCabinetRecords);
        }

        private static void Print(ReadOnlyCollection<FileCabinetRecord> fileCabinetRecords)
        {
            foreach (var item in fileCabinetRecords)
            {
                Console.WriteLine(item.ToString());
            }
        }
    }
}
