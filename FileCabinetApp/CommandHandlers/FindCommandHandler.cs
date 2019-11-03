using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    internal class FindCommandHandler : ServiceCommandHandlerBase
    {
        private Action<IEnumerable<FileCabinetRecord>> printer;

        public FindCommandHandler(IFileCabinetService service, Action<IEnumerable<FileCabinetRecord>> printer)
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
                    this.printer(findList);
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
