﻿using System;
using System.Collections.Generic;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// List command handler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    internal class ListCommandHandler : ServiceCommandHandlerBase
    {
        private readonly Action<List<string>, IEnumerable<FileCabinetRecord>> printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="printer">The printer.</param>
        public ListCommandHandler(IFileCabinetService service, Action<List<string>, IEnumerable<FileCabinetRecord>> printer)
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
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }
    }
}
