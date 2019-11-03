using System.Collections.Generic;

namespace FileCabinetApp.CommandHandlers.Printer
{
    /// <summary>
    /// File cabinet records printer interface.
    /// </summary>
    public interface IRecordPrinter
    {
        /// <summary>
        /// Prints the specified records.
        /// </summary>
        /// <param name="records">The records.</param>
        void Print(IEnumerable<FileCabinetRecord> records);
    }
}
