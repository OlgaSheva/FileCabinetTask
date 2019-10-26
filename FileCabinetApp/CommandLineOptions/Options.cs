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
        [Option('s', "storage", Default = ServiceType.Memory, HelpText = "Defines storage location: Memory or File.")]
        public ServiceType Storage { get; set; }

        /// <summary>
        /// Gets or sets the validate.
        /// </summary>
        /// <value>
        /// The validate.
        /// </value>
        [Option('v', "validation-rules", Default = "default", HelpText = "Defines validation rules: default or custom.")]
        public string Validate { get; set; }
    }
}
