using System;
using System.Collections.ObjectModel;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    internal class FindCommandHandler : CommandHandlerBase
    {
        private static IFileCabinetService fileCabinetService;

        public FindCommandHandler(IFileCabinetService service)
        {
            fileCabinetService = service;
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request.Command == "find")
            {
                FindByParameter(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private static void FindByParameter(string parameters)
        {
            try
            {
                ReadOnlyCollection<FileCabinetRecord> findList = fileCabinetService.Find(parameters);
                Print(findList);
            }
            catch (InvalidOperationException ioex)
            {
                Console.WriteLine($"The record didn't find.", ioex.Message);
            }
            catch (ArgumentException aex)
            {
                Console.WriteLine($"The record didn't find.", aex.Message);
            }
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
