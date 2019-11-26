using System;
using System.Collections.Generic;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Similar command handler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.CommandHandlerBase" />
    internal class SimilarCommandHandler : CommandHandlerBase
    {
        private static Action<string> write;
        private readonly List<string> commandList;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimilarCommandHandler"/> class.
        /// </summary>
        /// <param name="writeDelegate">The write delegate.</param>
        public SimilarCommandHandler(Action<string> writeDelegate)
        {
            write = writeDelegate;
            this.commandList = new List<string>
            {
                "create",
                "delete",
                "exit",
                "export",
                "help",
                "import",
                "insert",
                "select",
                "purge",
                "stat",
                "update",
            };
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

            write($"'{request.Command}' is not a command. See 'help'.");
            List<string> similarCommands = new List<string>();
            foreach (var command in this.commandList)
            {
                int result = GetDamerauLevenshteinDistance(request.Command, command);
                if (result < 3)
                {
                    similarCommands.Add(command);
                }
            }

            if (similarCommands.Count == 1)
            {
                write("The most similar commands is");
                write($"\t\t{similarCommands[0]}");
            }
            else if (similarCommands.Count > 1)
            {
                write("The most similar commands are");
                foreach (var command in similarCommands)
                {
                    write($"\t\t{command}");
                }
            }

            return null;
        }

        private static int GetDamerauLevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                throw new ArgumentNullException(s, "String Cannot Be Null Or Empty");
            }

            if (string.IsNullOrEmpty(t))
            {
                throw new ArgumentNullException(t, "String Cannot Be Null Or Empty");
            }

            int n = s.Length;
            int m = t.Length;

            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            int[] p = new int[n + 1];
            int[] d = new int[n + 1];

            int i;
            int j;

            for (i = 0; i <= n; i++)
            {
                p[i] = i;
            }

            for (j = 1; j <= m; j++)
            {
                char tj = t[j - 1];
                d[0] = j;

                for (i = 1; i <= n; i++)
                {
                    int cost = s[i - 1] == tj ? 0 : 1;
                    d[i] = Math.Min(Math.Min(d[i - 1] + 1, p[i] + 1), p[i - 1] + cost);
                }

                int[] dplaceholder = p;
                p = d;
                d = dplaceholder;
            }

            return p[n];
        }
    }
}
