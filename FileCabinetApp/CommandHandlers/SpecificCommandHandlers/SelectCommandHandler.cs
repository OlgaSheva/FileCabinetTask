﻿using System;
using System.Collections.Generic;
using System.Globalization;
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
            var words = parameters
                .Split(this.Separator.ToArray())
                .Where(s => s.Length > 0)
                .ToList();
            var keyValuePairs = new List<KeyValuePair<string, string>>(2);
            int i = 0;
            List<string> columns = new List<string>();
            SearchCondition condition = SearchCondition.Or; // 'And' or 'Or'
            while (i < words.Count && !words[i].Equals("where", StringComparison.InvariantCultureIgnoreCase))
            {
                columns.Add(words[i++]);
            }

            while (++i < words.Count)
            {
                keyValuePairs.Add(new KeyValuePair<string, string>(words[i].ToLower(CultureInfo.CurrentCulture), words[i + 1]));
                i += 2;
                if (i < words.Count)
                {
                    condition = words[i].Equals("and", StringComparison.InvariantCulture) ? SearchCondition.And : SearchCondition.Or;
                }
            }

            if (columns.Count == 0)
            {
                columns = new List<string>
                {
                    "id",
                    "firstname",
                    "lastname",
                    "dateofbirth",
                    "gender",
                    "office",
                    "salary",
                };
            }

            IEnumerable<FileCabinetRecord> records = null;
            if (this.Service is FileCabinetMemoryService)
            {
                records = Memoizer.GetMemoizer(this.Service).Select(keyValuePairs, condition);
            }
            else
            {
                records = this.Service.GetRecords().Where(keyValuePairs, condition);
            }

            this.printer(columns, records);
        }
    }
}