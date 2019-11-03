using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    internal class ListCommandHandler : ServiceCommandHandlerBase
    {
        private Action<IEnumerable<FileCabinetRecord>> printer;

        public ListCommandHandler(IFileCabinetService service, Action<IEnumerable<FileCabinetRecord>> printer)
            : base(service)
        {
            this.printer = printer;
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request.Command == "list")
            {
                ReadOnlyCollection<FileCabinetRecord> fileCabinetRecords = service.GetRecords();
                this.printer(fileCabinetRecords);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }
    }
}
