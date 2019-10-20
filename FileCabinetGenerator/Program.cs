using System;
using CommandLine;
using FileCabinetGenerator.CommandLineOptions;

namespace FileCabinetGenerator
{
    class Program
    {
        static void Main(string[] args)
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

            Console.WriteLine($"{recordAmount} records were written to {output}.");
        }
    }
}
