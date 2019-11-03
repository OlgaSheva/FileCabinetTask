using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

using CommandLine;

using FileCabinetGenerator.CommandLineOptions;
using FileCabinetGenerator.Models;

namespace FileCabinetGenerator
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            string outputType = "csv";
            string output = "records.csv";
            int recordAmount = 0;
            int startId = 10000;

            var result = Parser.Default.ParseArguments<Options>(args);
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       outputType = o.OutputType;
                       output = o.Output;
                       recordAmount = o.RecordAmount;
                       startId = o.StartId;
                   });

            var records = RecordsOrderGeneration(startId, recordAmount);

            switch (outputType)
            {
                case "csv":
                    ExportCSV(output, records.FileCabinetRecords);
                    Console.WriteLine($"{recordAmount} records were written to {output}.");
                    break;
                case "xml":
                    ExportXML(output, records);
                    Console.WriteLine($"{recordAmount} records were written to {output}.");
                    break;
                default:
                    Console.WriteLine($"Records were not written.");
                    throw new Exception(nameof(outputType));
            }
        }

        private static FileCabinetRecord FileCabinetRecordGenerator(int startId, int index)
        {
            Random random = new Random();
            string genderChars = "MFOU";
            DateTime minDate = new DateTime(1950, 1, 1);
            DateTime maxDate = new DateTime(2005, 1, 1);
            int daysDiff = Convert.ToInt32(maxDate.Subtract(minDate).TotalDays + 1);

            FileCabinetRecord record = new FileCabinetRecord()
            {
                Id = startId + index,
                FullName = new Name()
                {
                    FirstName = "Fn" + random.Next(0, 10000),
                    LastName = "Ln" + random.Next(0, 10000),
                },
                DateOfBirth = minDate.AddDays(random.Next(0, daysDiff)),
                Gender = genderChars[random.Next(0, genderChars.Length - 1)],
                Office = (short)random.Next(0, 500),
                Salary = (decimal)NextDouble(random, 0, 4000),
            };
            return record;
        }

        private static RecordsOrder RecordsOrderGeneration(int startId, int recordAmount)
        {
            RecordsOrder recordsOrder = new RecordsOrder();
            for (int i = 0; i < recordAmount; i++)
            {
                var record = FileCabinetRecordGenerator(startId, i);
                recordsOrder.FileCabinetRecords.Add(record);
            }

            return recordsOrder;
        }

        private static void ExportCSV(string filePath, List<FileCabinetRecord> recordsList)
        {
            if (recordsList != null && recordsList.Count > 0)
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine($"{nameof(FileCabinetRecord.Id)}," +
                    $"{nameof(FileCabinetRecord.FullName.FirstName)}," +
                    $"{nameof(FileCabinetRecord.FullName.LastName)}," +
                    $"{nameof(FileCabinetRecord.DateOfBirth)}," +
                    $"{nameof(FileCabinetRecord.Gender)}," +
                    $"{nameof(FileCabinetRecord.Office)}," +
                    $"{nameof(FileCabinetRecord.Salary)}");
                    foreach (var record in recordsList)
                    {
                        writer.WriteLine(record.ToString());
                    }
                }
            }
        }

        private static void ExportXML(string filePath, RecordsOrder recordsOrder)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(RecordsOrder));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, recordsOrder);
            }
        }

        private static double NextDouble(Random rnd, double min, double max)
        {
            return Math.Round((rnd.NextDouble() * (max - min)) + min, 2);
        }
    }
}
