﻿using System;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Purge command handler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    internal class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public PurgeCommandHandler(IFileCabinetService service)
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
            if (request.Command == "purge")
            {
                this.Purge(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private void Purge(string parameters)
        {
            this.Service.Purge(out int deletedRecordsCount, out int recordsCount);
            Console.WriteLine($"Data file processing is completed: {deletedRecordsCount} of {recordsCount} records were purged.");
        }
    }
}