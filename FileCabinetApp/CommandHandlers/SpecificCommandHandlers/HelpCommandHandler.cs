using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Help command handler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.CommandHandlerBase" />
    internal class HelpCommandHandler : CommandHandlerBase
    {
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private static Action<string> write;
        private static string[][] helpMessages = new string[][]
        {
            new string[]
            {
                "help",
                "prints the help screen",
                "The 'help' command prints the help screen.",
            },
            new string[]
            {
                "exit",
                "exits the application",
                "The 'exit' command exits the application.",
            },
            new string[]
            {
                "stat",
                "shows statistics by records",
                "The 'stat' command shows statistics by records",
            },
            new string[]
            {
                "create",
                "creates a new record",
                "The 'create' command creates a new record.",
            },
            new string[]
            {
                "insert (parameters) values (values)",
                "inserts a new record",
                "The 'insert' command inserts a new record.",
            },
            new string[]
            {
                "update set <parameter> = '<value>', <parameter> = '<value>', ... where  <parameter> = '<value>'",
                "updates the record",
                "The 'update' command updates the record.",
            },
            new string[]
            {
                "select <parameter>, <parameter>, ... where <parameter> = '<value>' or/and <parameter> = '<value>'",
                "returns a list of records whith this parameters / all records",
                "The 'select' command returns a list of records matching query parameters.",
            },
            new string[]
            {
                "export <csv/xml> <file adress>",
                "exports service data to a CSV or XML file",
                "The 'export' command exports service data to a CSV or XML file.",
            },
            new string[]
            {
                "import <csv/xml> <file adress>",
                "imports service data from a CSV or XML file",
                "The 'export' command imports service data from a CSV or XML file.",
            },
            new string[]
            {
                "delete where <parameter> = '<value>'",
                "deletes a record by parameter",
                "The 'delete' command deletes a record by parameter.",
            },
            new string[]
            {
                "purge",
                "defragment a data file",
                "The 'purge' command defragment a data file.",
            },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpCommandHandler"/> class.
        /// </summary>
        /// <param name="writeDelegate">The write delegate.</param>
        public HelpCommandHandler(Action<string> writeDelegate)
        {
            write = writeDelegate;
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

            if (request.Command == "help")
            {
                PrintHelp(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    write(helpMessages[index][ExplanationHelpIndex]);
                }
                else
                {
                    write($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                write("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    write($"\t{helpMessage[CommandHelpIndex]}\t- {helpMessage[DescriptionHelpIndex]}");
                }
            }

            write(string.Empty);
        }
    }
}
