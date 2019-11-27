using System;
using System.Collections.Generic;
using System.Linq;
using FileCabinetApp.Enums;
using FileCabinetApp.Extensions;
using FileCabinetApp.Memoizers;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers.SpecificCommandHandlers
{
    /// <summary>
    /// Select command handler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    public class SelectCommandHandler : ServiceCommandHandlerBase
    {
        private readonly Action<List<string>, IEnumerable<FileCabinetRecord>> printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="printer">The printer.</param>
        public SelectCommandHandler(
            IFileCabinetService service, Action<List<string>, IEnumerable<FileCabinetRecord>> printer)
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
        /// <exception cref="ArgumentNullException">Request is null.</exception>
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Command == "select")
            {
                this.Select(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private void Select(string parameters)
        {
            if (parameters.Contains("or", StringComparison.InvariantCultureIgnoreCase)
                && parameters.Contains("and", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException(
                    "This method supports only AND or only OR condition. Example: select id, firstname, lastname where firstname = 'John' and lastname = 'Doe' and dateofbirth = '02/02/2000'",
                    nameof(parameters));
            }

            SearchCondition condition = parameters.Contains("and", StringComparison.InvariantCultureIgnoreCase)
                ? SearchCondition.And
                : SearchCondition.Or;
            var words = parameters
                .Split(this.Separator.ToArray())
                .Where(s => s.Length > 0)
                .ToList();
            var keyValuePairs = new List<KeyValuePair<string, string>>();
            int i = 0;
            List<string> columns = new List<string>();
            while (i < words.Count && !words[i].Equals("where", StringComparison.InvariantCultureIgnoreCase))
            {
                columns.Add(words[i++]);
            }

            while (++i < words.Count)
            {
                keyValuePairs.Add(new KeyValuePair<string, string>(words[i].ToUpperInvariant(), words[i + 1]));
                i += 2;
            }

            if (columns.Count == 0)
            {
                columns = this.NamesOfRecordElements;
            }
            else
            {
                this.IsValid(columns);
            }

            IEnumerable<FileCabinetRecord> records = null;
            if (this.Service is FileCabinetMemoryService)
            {
                records = Memoizer.GetMemoizer(this.Service).Select(parameters, keyValuePairs, condition);
            }
            else
            {
                records = this.Service.GetRecords().Where(keyValuePairs, condition);
            }

            this.printer(columns, records);
        }

        private void IsValid(List<string> columns)
        {
            bool flag = true;
            foreach (var column in columns)
            {
                flag = true;
                foreach (var dc in this.NamesOfRecordElements)
                {
                    if (column.Equals(dc, StringComparison.InvariantCultureIgnoreCase))
                    {
                        flag = false;
                    }
                }

                if (flag)
                {
                    throw new ArgumentException(nameof(column), $"Parameter '{column}' is not valid.");
                }
            }
        }
    }
}