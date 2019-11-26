using System;
using System.Globalization;
using System.IO;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Export command handler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    internal class ExportCommandHandler : ServiceCommandHandlerBase
    {
        private const int FormatPosition = 0;
        private const int PathPosition = 1;
        private static Action<string> write;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="writeDelegate">The write delegate.</param>
        public ExportCommandHandler(IFileCabinetService service, Action<string> writeDelegate)
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
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Command == "export")
            {
                this.Export(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private void Export(string parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            string[] commands = parameters.Split(' ');
            if (commands.Length != 2)
            {
                throw new ArgumentException("Wrong command format. Example: export csv d:/records.csv", nameof(parameters));
            }

            string format = commands[FormatPosition];
            string path = commands[PathPosition];
            const string yesAnswer = "y";
            const string csvFileType = "csv";
            const string xmlFileType = "xml";

            if (File.Exists(path))
            {
                write($"File is exist - rewrite {path}? [Y/n] ");
                if (char.TryParse(Console.ReadLine(), out char answer))
                {
                    if (answer.ToString(CultureInfo.InvariantCulture).Equals(yesAnswer, StringComparison.InvariantCultureIgnoreCase))
                    {
                        File.Delete(path);
                        Write(path);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                try
                {
                    Write(path);
                }
                catch (FileLoadException ex)
                {
                    throw new ArgumentException($"Export failed: can't open file {path}.", ex.Message);
                }
            }

            void Write(string p)
            {
                using (StreamWriter streamWriter = new StreamWriter(p))
                {
                    FileCabinetServiceSnapshot snapshot = this.Service.MakeSnapshot();

                    if (format == csvFileType)
                    {
                        snapshot.SaveToCSV(streamWriter);
                    }
                    else if (commands[FormatPosition] == xmlFileType)
                    {
                        snapshot.SaveToXML(streamWriter);
                    }
                    else
                    {
                        write($"There is no {commands[FormatPosition]} command.");
                        return;
                    }

                    write($"All records are exported to file {p}.");
                }
            }
        }
    }
}