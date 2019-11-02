using System;
using System.Globalization;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    internal class RemoveCommandHandler : CommandHandlerBase
    {
        private static IFileCabinetService fileCabinetService;

        public RemoveCommandHandler(IFileCabinetService service)
        {
            fileCabinetService = service;
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request.Command == "remove")
            {
                Remove(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private static void Remove(string parameters)
        {
            int id = -1;
            if (!int.TryParse(parameters, NumberStyles.Integer, CultureInfo.InvariantCulture, out id)
                || id == 0
                || fileCabinetService.GetStat(out int deletedRecordsCount) == 0)
            {
                Console.WriteLine($"Record '{parameters}' doesn't exists.");
                return;
            }

            if (fileCabinetService.IsThereARecordWithThisId(id, out long position))
            {
                fileCabinetService.Remove(id, position);
                Console.WriteLine($"Record #{id} is removed.");
            }
            else
            {
                Console.WriteLine($"Record #{id} didn't found.");
            }
        }
    }
}
