using CommandLine;

namespace FileCabinetGenerator.CommandLineOptions
{
    /// <summary>
    /// Command line options.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Gets or sets the type of the output.
        /// </summary>
        /// <value>
        /// The type of the output.
        /// </value>
        [Option('t', "output-type", Default = "csv", HelpText = "Output format type (csv, xml).")]
        public string OutputType { get; set; }

        /// <summary>
        /// Gets or sets the output.
        /// </summary>
        /// <value>
        /// The output.
        /// </value>
        [Option('o', "output", Default = "records.csv", HelpText = "Output file name.")]
        public string Output { get; set; }

        /// <summary>
        /// Gets or sets the record amount.
        /// </summary>
        /// <value>
        /// The record amount.
        /// </value>
        [Option('a', "records-amount", Default = 5000, HelpText = "Amount of generated records.")]
        public int RecordAmount { get; set; }

        /// <summary>
        /// Gets or sets the start identifier.
        /// </summary>
        /// <value>
        /// The start identifier.
        /// </value>
        [Option('i', "start-id", Default = 10000, HelpText = "ID value to start.")]
        public int StartId { get; set; }
    }
}
