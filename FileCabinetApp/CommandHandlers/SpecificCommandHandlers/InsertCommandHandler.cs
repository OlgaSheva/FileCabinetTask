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
        private static Action<string> write;
        private readonly Dictionary<string, string> values;

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="writeDelegate">The write delegate.</param>
        public InsertCommandHandler(IFileCabinetService service, Action<string> writeDelegate)
            : base(service)
        {
            this.values = new Dictionary<string, string>();
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

            if (request.Command == "insert")
            {
                this.Insert(request.Parameters);
                if (this.Service is FileCabinetMemoryService)
                {
                    Memoizer.GetMemoizer(this.Service).MemoizerDictionary.Clear();
                }

                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private void Insert(string parameters)
        {
            if (!parameters.Contains("values", StringComparison.InvariantCulture))
            {
                throw new ArgumentException(
                    $"Invalid command format. Example:" +
                    $" insert (id, firstname, lastname, dateofbirth, gender, office, salary) values ('1', 'John', 'Doe', '5/18/1986', 'M', '133', '1234.5')",
                    nameof(parameters));
            }

            var words = parameters
                .ToUpperInvariant()
                .Split(this.Separator.ToArray())
                .Where(s => s.Length > 0)
                .ToList();

            if (words.Count != 15)
            {
                throw new ArgumentException(
                    $"Invalid command format. Example:" +
                    $" insert (id, firstname, lastname, dateofbirth, gender, office, salary) values ('1', 'John', 'Doe', '5/18/1986', 'M', '133', '1234.5')",
                    nameof(parameters));
            }

            try
            {
                this.FillInValues(words);

                RecordParameters recordParameters = new RecordParameters(
                    CultureInfo.InvariantCulture.TextInfo.ToTitleCase(this.values["FIRSTNAME"].ToLowerInvariant()),
                    CultureInfo.InvariantCulture.TextInfo.ToTitleCase(this.values["LASTNAME"].ToLowerInvariant()),
                    DateTime.Parse(this.values["DATEOFBIRTH"], CultureInfo.InvariantCulture),
                    char.Parse(this.values["GENDER"]),
                    short.Parse(this.values["OFFICE"], CultureInfo.InvariantCulture),
                    decimal.Parse(this.values["SALARY"], CultureInfo.InvariantCulture));
                this.Service.InsertRecord(recordParameters, int.Parse(this.values["ID"], CultureInfo.InvariantCulture));
                write($"Record #{this.values["ID"]} is created.");
            }
            catch (ArgumentException aex)
            {
                write($"Record wasn't created. {aex.Message}");
                write(HintMessage);
            }
            catch (FormatException fex)
            {
                write($"Record wasn't created. {fex.Message}");
                write(HintMessage);
            }
            catch (OverflowException ofex)
            {
                write($"Record wasn't created. {ofex.Message}");
                write(HintMessage);
            }
        }

        private void FillInValues(List<string> words)
        {
            this.values.Clear();
            for (int i = 0; i <= RecordsParametersCount; i++)
            {
                bool flag = true;
                foreach (var name in this.NamesOfRecordElements)
                {
                    if (words[i].Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        flag = false;
                    }
                }

                if (flag)
                {
                    throw new ArgumentException($"The '{words[i]}' is not valid record property name.", nameof(words));
                }

                this.values.Add(words[i], words[i + RecordsParametersCount + 2]);
            }
        }
    }
}