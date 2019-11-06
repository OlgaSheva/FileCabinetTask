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
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public ImportCommandHandler(IFileCabinetService service)
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
            string fileFormat = comands[0];
            string filePath = comands[1];
            const string csvFormat = "csv";
            const string xmlFormat = "xml";

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Import error: file {filePath} is not exist.");
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
                        Console.WriteLine($"Unknown file format '{fileFormat}'.");
                        break;
                }
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Records were not imported.");
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
                Console.WriteLine($"Record #{ex.Key} was not imported.");
            }

            Console.WriteLine($"{recordsCount - exceptions.Count} records were imported from {filePath}.");
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
                Console.WriteLine($"Record #{ex.Key} was not imported.");
            }

            Console.WriteLine($"{recordsCount - exceptions.Count} records were imported from {filePath}.");
        }
    }
}
