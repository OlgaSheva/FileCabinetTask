using CommandLine;
using FileCabinetApp.Enums;

namespace FileCabinetApp.CommandLineOptions
{
    /// <summary>
    /// Command line options.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Gets or sets the storage.
        /// </summary>
        /// <value>
        /// The storage.
        /// </value>
        [Option('s', "storage", Default = ServiceType.File, HelpText = "Defines storage location: Memory or File.")]
        public ServiceType Storage { get; set; }

        /// <summary>
        /// Gets or sets the validate.
        /// </summary>
        /// <value>
        /// The validate.
        /// </value>
        [Option('v', "validation-rules", Default = "default", HelpText = "Defines validation rules: default or custom.")]
        public string Validate { get; set; }

        /// <summary>
        /// Gets or sets the meter.
        /// </summary>
        /// <value>
        /// The meter.
        /// </value>
        [Option("use-stopwatch", Default = MeterStatus.Off, HelpText = "Defines meter rules: On or Off.")]
        public MeterStatus Meter { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        [Option("use-logger", Default = LoggerStatus.Off, HelpText = "Defines logger rules: On or Off.")]
        public LoggerStatus Logger { get; set; }
    }
}
