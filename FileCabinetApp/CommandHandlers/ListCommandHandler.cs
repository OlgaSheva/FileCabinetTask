using System.Collections.ObjectModel;
using FileCabinetApp.CommandHandlers.Printer;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    internal class ListCommandHandler : ServiceCommandHandlerBase
    {
        private IRecordPrinter printer;

        public ListCommandHandler(IFileCabinetService service, IRecordPrinter printer)
            : base(service)
        {
            this.printer = printer;
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request.Command == "list")
            {
                ReadOnlyCollection<FileCabinetRecord> fileCabinetRecords = service.GetRecords();
                this.printer.Print(fileCabinetRecords);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }
    }
}
