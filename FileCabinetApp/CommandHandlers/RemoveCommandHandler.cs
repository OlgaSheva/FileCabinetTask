using System;
using System.Globalization;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Remove command handler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    internal class RemoveCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public RemoveCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Handles the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// Class AppCommandRequest Instance.
        /// </returns>
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
                || service.GetStat(out int deletedRecordsCount) == 0)
            {
                Console.WriteLine($"Record '{parameters}' doesn't exists.");
                return;
            }

            if (service.IsThereARecordWithThisId(id, out long position))
            {
                service.Remove(id, position);
                Console.WriteLine($"Record #{id} is removed.");
            }
            else
            {
                Console.WriteLine($"Record #{id} didn't found.");
            }
        }
    }
}
