using System;
using System.Collections.Generic;
using System.IO;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    internal class ImportCommandHandler : CommandHandlerBase
    {
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request.Command == "import")
            {
                Import(request.Parameters);
            }

            return base.Handle(request);
        }

        private static void Import(string parameters)
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
                        ImportFromCSVFile(filePath);
                        break;
                    case xmlFormat:
                        ImportFromXMLFile(filePath);
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

        private static void ImportFromCSVFile(string filePath)
        {
            FileCabinetServiceSnapshot snapshot = new FileCabinetServiceSnapshot();
            Dictionary<int, string> exceptions = new Dictionary<int, string>();
            int recordsCount = 0;
            using (StreamReader reader = new StreamReader(filePath))
            {
                snapshot.LoadFromCSV(reader, out recordsCount);
                Program.fileCabinetService.Restore(snapshot, out exceptions);
            }

            foreach (var ex in exceptions)
            {
                Console.WriteLine($"Record #{ex.Key} was not imported.");
            }

            Console.WriteLine($"{recordsCount - exceptions.Count} records were imported from {filePath}.");
        }

        private static void ImportFromXMLFile(string filePath)
        {
            FileCabinetServiceSnapshot snapshot = new FileCabinetServiceSnapshot();
            Dictionary<int, string> exceptions = new Dictionary<int, string>();
            int recordsCount = 0;
            using (StreamReader reader = new StreamReader(filePath))
            {
                snapshot.LoadFromXML(reader, out recordsCount);
                Program.fileCabinetService.Restore(snapshot, out exceptions);
            }

            foreach (var ex in exceptions)
            {
                Console.WriteLine($"Record #{ex.Key} was not imported.");
            }

            Console.WriteLine($"{recordsCount - exceptions.Count} records were imported from {filePath}.");
        }
    }
}
