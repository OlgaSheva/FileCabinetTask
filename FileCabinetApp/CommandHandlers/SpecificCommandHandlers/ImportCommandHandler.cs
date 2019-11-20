using System;
using System.Collections.Generic;
using System.IO;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Import command handler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    internal class ImportCommandHandler : ServiceCommandHandlerBase
    {
        private const int FormatPosition = 0;
        private const int PathPosition = 1;
        private static Action<string> write;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="writeDelegate">The write delegate.</param>
        public ImportCommandHandler(IFileCabinetService service, Action<string> writeDelegate)
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
            if (request.Command == "import")
            {
                this.Import(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private void Import(string parameters)
        {
            string[] comands = parameters.Split(' ');
            string fileFormat = comands[FormatPosition];
            string filePath = comands[PathPosition];
            const string csvFormat = "csv";
            const string xmlFormat = "xml";

            if (!File.Exists(filePath))
            {
                write($"Import error: file {filePath} is not exist.");
                return;
            }

            try
            {
                switch (fileFormat)
                {
                    case csvFormat:
                        this.ImportFromCSVFile(filePath);
                        break;
                    case xmlFormat:
                        this.ImportFromXMLFile(filePath);
                        break;
                    default:
                        write($"Unknown file format '{fileFormat}'.");
                        break;
                }
            }
            catch (ArgumentException)
            {
                write("Records were not imported.");
            }
        }

        private void ImportFromCSVFile(string filePath)
        {
            FileCabinetServiceSnapshot snapshot = new FileCabinetServiceSnapshot();
            Dictionary<int, string> exceptions = new Dictionary<int, string>();
            int recordsCount = 0;
            using (StreamReader reader = new StreamReader(filePath))
            {
                snapshot.LoadFromCSV(reader, out recordsCount);
                this.Service.Restore(snapshot, out exceptions);
            }

            foreach (var ex in exceptions)
            {
                write($"Record #{ex.Key} was not imported.");
            }

            write($"{recordsCount - exceptions.Count} records were imported from {filePath}.");
        }

        private void ImportFromXMLFile(string filePath)
        {
            FileCabinetServiceSnapshot snapshot = new FileCabinetServiceSnapshot();
            Dictionary<int, string> exceptions = new Dictionary<int, string>();
            int recordsCount = 0;
            using (StreamReader reader = new StreamReader(filePath))
            {
                snapshot.LoadFromXML(reader, out recordsCount);
                this.Service.Restore(snapshot, out exceptions);
            }

            foreach (var ex in exceptions)
            {
                write($"Record #{ex.Key} was not imported.");
            }

            write($"{recordsCount - exceptions.Count} records were imported from {filePath}.");
        }
    }
}
