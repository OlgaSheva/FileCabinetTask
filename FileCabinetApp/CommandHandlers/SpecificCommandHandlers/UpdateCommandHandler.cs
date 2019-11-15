using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers.SpecificCommandHandlers
{
    /// <summary>
    /// Update command handler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    internal class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public UpdateCommandHandler(IFileCabinetService service)
            : base(service)
        {
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
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private void Update(string parameters)
        {
            var words = parameters.Split(
                new char[] { ' ', ',', ':', ';', '-', '=', '(', ')', '\'', '!', '?', '\t' }).Where(s => s.Length > 0).ToList();
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
                { "id", null },
                { "firstname", null },
                { "lastname", null },
                { "dateofbirth", null },
            };
            if (words[0].Equals("set", StringComparison.InvariantCultureIgnoreCase))
            {
                int i = 1;
                do
                {
                    key = words[i].ToLower(CultureInfo.CurrentCulture);
                    value = words[i + 1];
                    switch (key)
                    {
                        case "firstname":
                            firstname = value;
                            break;
                        case "lastname":
                            lastname = value;
                            break;
                        case "dateofbirth":
                            dateofbirth = DateTime.Parse(value, CultureInfo.InvariantCulture);
                            break;
                        case "gender":
                            gender = char.Parse(value);
                            break;
                        case "office":
                            office = short.Parse(value, CultureInfo.InvariantCulture);
                            break;
                        case "salary":
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
                    keyValuePairs[words[i].ToLower(CultureInfo.CurrentCulture)] = words[i + 1];
                    i += 2;
                }

                int id = this.Service.Update(setRecord, keyValuePairs);
                Console.WriteLine($"Record #{id} has been updated.");
            }
            else
            {
                throw new ArgumentException("Request is not valid. " +
                    "Example: update set firstname = 'John', lastname = 'Doe' , dateofbirth = '5/18/1986' where id = '1'");
            }
        }
    }
}
