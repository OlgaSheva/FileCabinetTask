using System;
using System.Globalization;

namespace FileCabinetApp.CommandHandlers
{
    internal class RemoveCommandHandler : CommandHandlerBase
    {
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request.Command == "remove")
            {
                Remove(request.Parameters);
            }

            return base.Handle(request);
        }

        private static void Remove(string parameters)
        {
            int id = -1;
            if (!int.TryParse(parameters, NumberStyles.Integer, CultureInfo.InvariantCulture, out id)
                || id == 0
                || Program.fileCabinetService.GetStat(out int deletedRecordsCount) == 0)
            {
                Console.WriteLine($"Record '{parameters}' doesn't exists.");
                return;
            }

            if (Program.fileCabinetService.IsThereARecordWithThisId(id, out long position))
            {
                Program.fileCabinetService.Remove(id, position);
                Console.WriteLine($"Record #{id} is removed.");
            }
            else
            {
                Console.WriteLine($"Record #{id} didn't found.");
            }
        }
    }
}
