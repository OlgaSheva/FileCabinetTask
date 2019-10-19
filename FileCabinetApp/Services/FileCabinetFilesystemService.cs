using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using FileCabinetApp.Validators;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// The file cabinet FileStream service.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Services.IFileCabinetService" />
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private readonly FileStream fileStream;
        private readonly IRecordValidator validator;
        private UnicodeEncoding uniEncoding = new UnicodeEncoding();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="validator">The validator.</param>
        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator)
        {
            this.fileStream = fileStream;
            this.validator = validator;
        }

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="rec">The record.</param>
        /// <returns>
        /// New file cabinet record.
        /// </returns>
        public int CreateRecord(Record rec)
        {
            this.validator.ValidateParameters(rec.FirstName, rec.LastName, rec.DateOfBirth, rec.Gender, rec.MaterialStatus, rec.CatsCount, rec.CatsBudget);

            var record = new FileCabinetRecord
            {
                Id = this.fileStream.Length != 0
                ? (int)(this.fileStream.Length / 280) + 1
                : 1,
                FirstName = rec.FirstName,
                LastName = rec.LastName,
                DateOfBirth = rec.DateOfBirth,
                Gender = rec.Gender,
                MateriallStatus = rec.MaterialStatus,
                CatsCount = rec.CatsCount,
                CatsBudget = rec.CatsBudget,
            };

            var byteId = BitConverter.GetBytes(record.Id);
            var byteFirstName = System.Text.UnicodeEncoding.Unicode.GetBytes(record.FirstName.PadRight(60));
            var byteLastName = System.Text.UnicodeEncoding.Unicode.GetBytes(record.LastName.PadRight(60));
            var byteYear = BitConverter.GetBytes(record.DateOfBirth.Year);
            var byteMonth = BitConverter.GetBytes(record.DateOfBirth.Month);
            var byteDay = BitConverter.GetBytes(record.DateOfBirth.Day);
            var byteGender = BitConverter.GetBytes((char)record.Gender);
            var byteStatus = BitConverter.GetBytes(record.MateriallStatus);
            var byteCatsCount = BitConverter.GetBytes(record.CatsCount);
            var byteCatsBudget = GetBytes(record.CatsBudget);

            BinaryWriter writeBinay = new BinaryWriter(this.fileStream);
            writeBinay.Write(new byte[2], 0, 2); // reserved
            writeBinay.Write(byteId, 0, byteId.Length);
            writeBinay.Write(byteFirstName, 0, byteFirstName.Length);
            writeBinay.Write(byteLastName, 0, byteLastName.Length);
            writeBinay.Write(byteYear, 0, byteYear.Length);
            writeBinay.Write(byteMonth, 0, byteMonth.Length);
            writeBinay.Write(byteDay, 0, byteDay.Length);
            writeBinay.Write(byteGender, 0, byteGender.Length);
            writeBinay.Write(byteStatus, 0, byteStatus.Length);
            writeBinay.Write(byteCatsCount, 0, byteCatsCount.Length);
            writeBinay.Write(byteCatsBudget, 0, byteCatsBudget.Length);

            return record.Id;
        }

        /// <summary>
        /// Edits the record.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="record">The record.</param>
        public void EditRecord(int id, Record record)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// All records with specified parameters.
        /// </returns>
        public ReadOnlyCollection<FileCabinetRecord> Find(string parameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the records.
        /// </summary>
        /// <returns>
        /// All existing records.
        /// </returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the stat.
        /// </summary>
        /// <returns>
        /// The quantity of records.
        /// </returns>
        public int GetStat()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether [is there a record with this identifier] [the specified identifier].
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="index">The index.</param>
        /// <returns>
        /// <c>true</c> if [is there a record with this identifier] [the specified identifier]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsThereARecordWithThisId(int id, out int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Makes the snapshot.
        /// </summary>
        /// <returns>
        /// The file cabinet service snapshot.
        /// </returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }

        private static byte[] GetBytes(decimal dec)
        {
            int[] bits = decimal.GetBits(dec);
            List<byte> bytes = new List<byte>();
            foreach (int i in bits)
            {
                bytes.AddRange(BitConverter.GetBytes(i));
            }

            return bytes.ToArray();
        }
    }
}
