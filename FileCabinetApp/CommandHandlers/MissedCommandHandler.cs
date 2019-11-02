using System;

namespace FileCabinetApp.CommandHandlers
{
    internal class MissedCommandHandler : CommandHandlerBase
    {
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            PrintMissedCommandInfo(request.Command);
            return null;
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }
    }
}
