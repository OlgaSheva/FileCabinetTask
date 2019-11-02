using System;

namespace FileCabinetApp.CommandHandlers
{
    internal class ExitCommandHandler : CommandHandlerBase
    {
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request.Command == "exit")
            {
                Exit(request.Parameters);
            }

            return base.Handle(request);
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            Program.isRunning = false;
            Program.fileStream?.Close();
        }
    }
}
