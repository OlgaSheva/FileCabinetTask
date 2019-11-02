using System;
using System.IO;

namespace FileCabinetApp.CommandHandlers
{
    internal class ExitCommandHandler : CommandHandlerBase
    {
        private static FileStream stream;

        public ExitCommandHandler(FileStream fileStream)
        {
            stream = fileStream;
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request.Command == "exit")
            {
                Exit(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            Program.isRunning = false;
            stream?.Close();
        }
    }
}
