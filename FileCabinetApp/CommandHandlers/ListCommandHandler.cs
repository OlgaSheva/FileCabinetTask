using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FileCabinetApp.Iterators;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// List command handler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    internal class ListCommandHandler : ServiceCommandHandlerBase
    {
        private readonly Action<IRecordIterator> printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="printer">The printer.</param>
        public ListCommandHandler(IFileCabinetService service, Action<IRecordIterator> printer)
            : base(service)
        {
            this.printer = printer;
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
            if (request.Command == "list")
            {
                var fileCabinetRecords = this.Service.GetRecords();
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
