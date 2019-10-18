using CommandLine;

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
        [Option('s', "storage", Default = "memory", HelpText = "Defines storage location: memory or file.")]
        public string Storage { get; set; }

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
