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
                this.Remove(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private void Remove(string parameters)
        {
            int id = -1;
            if (!int.TryParse(parameters, NumberStyles.Integer, CultureInfo.InvariantCulture, out id) || id == 0)
            {
                Console.WriteLine($"Record '{parameters}' doesn't exists.");
                return;
            }

            if (this.Service.IsThereARecordWithThisId(id, out long position))
            {
                this.Service.Remove(id, position);
                Console.WriteLine($"Record #{id} is removed.");
            }
            else
            {
                Console.WriteLine($"Record #{id} didn't found.");
            }
        }
    }
}
