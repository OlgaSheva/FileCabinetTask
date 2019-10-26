using System;
using System.Collections.Generic;
using CommandLine;

using FileCabinetGenerator.CommandLineOptions;

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
            int recordAmount = 5000;
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
            List<FileCabinetRecord> fileCabinetRecords = FileCabinetRecordGenerator(startId, 10);
            foreach (var item in fileCabinetRecords)
            {
                Console.WriteLine(item.ToString());
            }

            Console.ReadKey();
        }

        private static List<FileCabinetRecord> FileCabinetRecordGenerator(int startId, int recordAmoutn)
        {
            var recordsList = new List<FileCabinetRecord>(recordAmoutn);
            Random random = new Random();
            string genderChars = "MFOU";
            DateTime minDate = new DateTime(1950, 1, 1);
            DateTime maxDate = new DateTime(2005, 1, 1);
            int daysDiff = Convert.ToInt32(maxDate.Subtract(minDate).TotalDays + 1);

            for (int i = 0; i < recordAmoutn; i++)
            {
                FileCabinetRecord record = new FileCabinetRecord(
                    startId + i,
                    "fn" + random.Next(0, 10000),
                    "ln" + random.Next(0, 10000),
                    minDate.AddDays(random.Next(0, daysDiff)),
                    genderChars[random.Next(0, genderChars.Length - 1)],
                    (short)random.Next(0, 500),
                    (decimal)NextDouble(random, 0, 4000));
                recordsList.Add(record);
            }

            return recordsList;
        }

        private static double NextDouble(Random rnd, double min, double max)
        {
            return Math.Round((rnd.NextDouble() * (max - min)) + min, 2);
        }
    }
}
