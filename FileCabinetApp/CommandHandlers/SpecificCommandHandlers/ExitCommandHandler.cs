using System;
using System.IO;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Exit command handler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.CommandHandlerBase" />
    internal class ExitCommandHandler : CommandHandlerBase
    {
        private readonly Action<bool> isRunningAction;
        private readonly FileStream stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="isRunningAction">The is running action.</param>
        public ExitCommandHandler(FileStream fileStream, Action<bool> isRunningAction)
        {
            this.stream = fileStream;
            this.isRunningAction = isRunningAction;
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
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Command == "exit")
            {
                Console.WriteLine("Exiting an application...");
                this.isRunningAction(false);
                this.stream?.Close();
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }
    }
}
