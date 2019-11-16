using System;
using System.Collections.Generic;

namespace FileCabinetApp.Printer
{
    /// <summary>
    /// Console table options.
    /// </summary>
    public class ConsoleTableOptions
    {
        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public IEnumerable<string> Columns { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a value indicating whether [enable count].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable count]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableCount { get; set; } = true;
    }
}