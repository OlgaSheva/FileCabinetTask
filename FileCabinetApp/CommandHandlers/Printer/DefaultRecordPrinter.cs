using System;
using System.Collections.Generic;

namespace FileCabinetApp.CommandHandlers.Printer
{
    public class DefaultRecordPrinter : IRecordPrinter
    {
        public void Print(IEnumerable<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            foreach (var item in records)
            {
                Console.WriteLine(item.ToString());
            }
        }
    }
}
