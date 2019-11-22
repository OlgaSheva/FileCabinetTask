using System;
using System.Linq;
using System.Text;
using FileCabinetApp.Memoizers;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers.SpecificCommandHandlers
{
    /// <summary>
    /// Delete command handler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    internal class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        private static Action<string> write;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="writeDelegate">The write delegate.</param>
        public DeleteCommandHandler(IFileCabinetService service, Action<string> writeDelegate)
            : base(service)
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
            if (request.Command == "delete")
            {
                this.Delete(request.Parameters);
                Memoizer.GetMemoizer(this.Service).MemoizerDictionary.Clear();
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private void Delete(string parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters), "You need to specify the parameters.");
            }

            if (!parameters.Contains("where", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException("The request is not valid. For example: delete where id = '1'", nameof(parameters));
            }

            var words = parameters
                .Split(this.Separator.ToArray())
                .Where(s => s.Length > 0)
                .ToList();

            if (words.Count > 3)
            {
                throw new ArgumentException("The command accepts only one parameter. For example: delete where id = '1'");
            }

            string key = null;
            string value = null;
            if (words[0].Equals("where", StringComparison.InvariantCultureIgnoreCase))
            {
                key = words[1];
                value = words[2];
            }

            var ids = this.Service.Delete(key, value);

            if (ids.Count == 1)
            {
                write($"Record #{ids[0]} is deleted.");
            }
            else if (ids.Count > 1)
            {
                var sb = new StringBuilder();
                for (int i = 0; i < ids.Count; i++)
                {
                    if (i < ids.Count - 1)
                    {
                        sb.Append($"#{ids[i]}, ");
                    }
                    else
                    {
                        sb.Append($"#{ids[i]}");
                    }
                }

                write($"Records {sb} are deleted.");
            }
            else
            {
                write("There are no entries with this parameter.");
            }
        }
    }
}
