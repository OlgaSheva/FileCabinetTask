using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FileCabinetApp.Memoizers;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers.SpecificCommandHandlers
{
    /// <summary>
    /// Insert command handler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    internal class InsertCommandHandler : ServiceCommandHandlerBase
    {
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int RecordsParametersCount = 6;
        private readonly Dictionary<string, string> values;

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public InsertCommandHandler(IFileCabinetService service)
            : base(service)
        {
            this.values = new Dictionary<string, string>();
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
            if (request.Command == "insert")
            {
                this.Insert(request.Parameters);
                Memoizer.GetMemoizer(this.Service).MemoizerDictionary.Clear();
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private void Insert(string parameters)
        {
            var words = parameters.ToLower(CultureInfo.DefaultThreadCurrentCulture).Split(
                new char[] { ' ', ',', '.', ':', ';', '(', ')', '\'', '!', '?', '\t' }).Where(s => s.Length > 0).ToList();
            try
            {
                for (int i = 0; i <= RecordsParametersCount; i++)
                {
                    this.values.Add(words[i], words[i + RecordsParametersCount + 2]);
                }

                RecordParameters recordParameters = new RecordParameters(
                    this.values["firstname"],
                    this.values["lastname"],
                    DateTime.Parse(this.values["dateofbirth"], CultureInfo.InvariantCulture),
                    char.Parse(this.values["gender"]),
                    short.Parse(this.values["office"], CultureInfo.InvariantCulture),
                    decimal.Parse(this.values["salary"], CultureInfo.InvariantCulture));
                this.Service.InsertRecord(recordParameters, int.Parse(this.values["id"], CultureInfo.InvariantCulture));
            }
            catch (ArgumentException aex)
            {
                Console.WriteLine($"Record wasn't created. {aex.Message}");
                Console.WriteLine(HintMessage);
            }
        }
    }
}