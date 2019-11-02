using System;
using System.IO;

namespace FileCabinetApp.CommandHandlers
{
    internal class ExitCommandHandler : CommandHandlerBase
    {
        private static FileStream stream;
        private readonly Action<bool> isRunningAction;

        public ExitCommandHandler(FileStream fileStream, Action<bool> isRunningAction)
        {
            stream = fileStream;
            this.isRunningAction = isRunningAction;
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request.Command == "exit")
            {
                Console.WriteLine("Exiting an application...");
                this.isRunningAction(false);
                stream?.Close();
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }
    }
}
