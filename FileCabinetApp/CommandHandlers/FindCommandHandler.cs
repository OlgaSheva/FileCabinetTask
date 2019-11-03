using System;
using System.Collections.ObjectModel;
using FileCabinetApp.CommandHandlers.Printer;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    internal class FindCommandHandler : ServiceCommandHandlerBase
    {
        private IRecordPrinter printer;

        public FindCommandHandler(IFileCabinetService service, IRecordPrinter printer)
            : base(service)
        {
            this.printer = printer;
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request.Command == "find")
            {
                try
                {
                    ReadOnlyCollection<FileCabinetRecord> findList = service.Find(request.Parameters);
                    this.printer.Print(findList);
                }
                catch (InvalidOperationException ioex)
                {
                    Console.WriteLine($"The record didn't find.", ioex.Message);
                }
                catch (ArgumentException aex)
                {
                    Console.WriteLine($"The record didn't find.", aex.Message);
                }

                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }
    }
}
