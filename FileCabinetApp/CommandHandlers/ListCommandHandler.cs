using System;
using System.Collections.ObjectModel;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    internal class ListCommandHandler : ServiceCommandHandlerBase
    {
        public ListCommandHandler(IFileCabinetService service) 
            : base(service)
        {
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request.Command == "list")
            {
                List(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private static void List(string parameters)
        {
            ReadOnlyCollection<FileCabinetRecord> fileCabinetRecords = service.GetRecords();
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
