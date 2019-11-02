using System;
using System.Globalization;
using System.IO;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    internal class ExportCommandHandler : ServiceCommandHandlerBase
    {
        private static StreamWriter streamWriter;

        public ExportCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            if (request.Command == "export")
            {
                Export(request.Parameters);
                return null;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private static void Export(string parameters)
        {
            string[] commands = parameters.Split(' ');
            string format = commands[0];
            string path = commands[1];
            const string yesAnswer = "y";
            const string csvFileType = "csv";
            const string xmlFileType = "xml";

            if (File.Exists(path))
            {
                Console.Write($"File is exist - rewrite {path}? [Y/n] ");
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
                    Console.WriteLine($"Export failed: can't open file {path}.", ex.Message);
                }
            }

            void Write(string p)
            {
                using (streamWriter = new StreamWriter(p))
                {
                    FileCabinetServiceSnapshot snapshot = service.MakeSnapshot();

                    if (format == csvFileType)
                    {
                        snapshot.SaveToCSV(streamWriter);
                    }
                    else if (commands[0] == xmlFileType)
                    {
                        snapshot.SaveToXML(streamWriter);
                    }
                    else
                    {
                        Console.WriteLine($"There is no {commands[0]} command.");
                        return;
                    }

                    Console.WriteLine($"All records are exported to file {p}.");
                }
            }
        }
    }
}