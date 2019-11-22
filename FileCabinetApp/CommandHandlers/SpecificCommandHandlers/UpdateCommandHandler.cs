﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FileCabinetApp.Memoizers;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers.SpecificCommandHandlers
{
    /// <summary>
    /// Update command handler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    internal class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        private static Action<string> write;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="writeDelegate">The write delegate.</param>
        public UpdateCommandHandler(IFileCabinetService service, Action<string> writeDelegate)
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
            if (request.Command == "update")
            {
                this.Update(request.Parameters);
                Memoizer.GetMemoizer(this.Service).MemoizerDictionary.Clear();
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private void Update(string parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (!parameters.Contains("set", StringComparison.InvariantCulture)
                || !parameters.Contains("where", StringComparison.InvariantCulture))
            {
                throw new ArgumentException(
                    "Invalid command. Example: update set firstname = 'John', lastname = 'Doe' , dateofbirth = '5/18/1986' where id = '1'",
                    nameof(parameters));
            }

            var words = parameters
                .Split(this.Separator.ToArray())
                .Where(s => s.Length > 0)
                .ToList();
            string firstname = null;
            string lastname = null;
            DateTime dateofbirth = default(DateTime);
            char gender = default(char);
            short office = -1;
            decimal salary = -1;
            string key;
            string value;
            FileCabinetRecord desiredRecord = new FileCabinetRecord();
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>()
            {
                { "ID", null },
                { "FIRSTNAME", null },
                { "LASTNAME", null },
                { "DATEOFBIRTH", null },
            };
            if (words[0].Equals("set", StringComparison.InvariantCultureIgnoreCase))
            {
                int i = 1;
                do
                {
                    key = words[i].ToUpperInvariant();
                    value = words[i + 1];
                    switch (key)
                    {
                        case "FIRSTNAME":
                            firstname = value;
                            break;
                        case "LASTNAME":
                            lastname = value;
                            break;
                        case "DATEOFBIRTH":
                            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                            {
                                dateofbirth = date;
                            }

                            break;
                        case "GENDER":
                            gender = char.Parse(value);
                            break;
                        case "OFFICE":
                            office = short.Parse(value, CultureInfo.InvariantCulture);
                            break;
                        case "SALARY":
                            salary = decimal.Parse(value, CultureInfo.InvariantCulture);
                            break;
                        default:
                            throw new ArgumentException(nameof(key), $"The parameter '{words[i]}' does not supported.");
                    }

                    i += 2;
                }
                while (!words[i].Equals("where", StringComparison.InvariantCultureIgnoreCase));
                RecordParameters setRecord = new RecordParameters(firstname, lastname, dateofbirth, gender, office, salary);

                while (++i < words.Count)
                {
                    keyValuePairs[words[i].ToUpperInvariant()] = words[i + 1];
                    i += 2;
                }

                int id = this.Service.Update(setRecord, keyValuePairs);
                write($"Record #{id} has been updated.");
            }
            else
            {
                throw new ArgumentException("Request is not valid. " +
                    "Example: update set firstname = 'John', lastname = 'Doe' , dateofbirth = '5/18/1986' where id = '1'");
            }
        }
    }
}
