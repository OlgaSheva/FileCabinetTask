using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
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
        private const int RecordInBytesLength = 278;
        private const int ReservedFieldLength = 2;
        private const int FirstNamePosition = 6;
        private const int LastNamePosition = 126;
        private const int DatePosition = 246;
        private const int GenderPosition = 258;
        private const int StringInBitesLength = 120;
        private const int DecimalInBitesLength = 16;
        private readonly FileStream fileStream;
        private readonly IRecordValidator validator;
        private readonly SortedList<int, long> idpositionPairs = new SortedList<int, long>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="validator">The validator.</param>
        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator)
        {
            this.fileStream = fileStream ?? throw new ArgumentNullException(nameof(fileStream));
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
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
            if (rec == null)
            {
                throw new ArgumentNullException(nameof(rec));
            }

            this.validator.ValidateParameters(rec.FirstName, rec.LastName, rec.DateOfBirth, rec.Gender, rec.Office, rec.Salary);

            var record = new FileCabinetRecord
            {
                Id = this.fileStream.Position != 0
                ? (int)(this.fileStream.Position / RecordInBytesLength) + 1
                : 1,
                FirstName = rec.FirstName,
                LastName = rec.LastName,
                DateOfBirth = rec.DateOfBirth,
                Gender = rec.Gender,
                Office = rec.Office,
                Salary = rec.Salary,
            };

            this.WriteToTheBinaryFile(record);
            this.idpositionPairs.Add(record.Id, this.fileStream.Position - RecordInBytesLength);

            return record.Id;
        }

        /// <summary>
        /// Edits the record.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="record">The record.</param>
        public void EditRecord(int id, Record record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (id <= 0)
            {
                throw new ArgumentException($"The {nameof(id)} have to be larger than zero.", nameof(id));
            }

            if (!this.IsThereARecordWithThisId(id, out int index))
            {
                throw new ArgumentException($"Record #{nameof(id)} doesn't exist.", nameof(id));
            }

            this.validator.ValidateParameters(record.FirstName, record.LastName, record.DateOfBirth, record.Gender, record.Office, record.Salary);

            var byteFirstName = System.Text.UnicodeEncoding.Unicode.GetBytes(record.FirstName.PadRight(60));
            var byteLastName = System.Text.UnicodeEncoding.Unicode.GetBytes(record.LastName.PadRight(60));
            var byteYear = BitConverter.GetBytes(record.DateOfBirth.Year);
            var byteMonth = BitConverter.GetBytes(record.DateOfBirth.Month);
            var byteDay = BitConverter.GetBytes(record.DateOfBirth.Day);
            var byteGender = BitConverter.GetBytes(record.Gender);
            var byteOffice = BitConverter.GetBytes(record.Office);
            var byteSalary = GetBytes(record.Salary);

            this.fileStream.Position = (RecordInBytesLength * (index - 1)) + FirstNamePosition;

            using (BinaryWriter writeBinay = new BinaryWriter(this.fileStream, Encoding.Unicode, true))
            {
                writeBinay.Write(byteFirstName, 0, byteFirstName.Length);
                writeBinay.Write(byteLastName, 0, byteLastName.Length);
                writeBinay.Write(byteYear, 0, byteYear.Length);
                writeBinay.Write(byteMonth, 0, byteMonth.Length);
                writeBinay.Write(byteDay, 0, byteDay.Length);
                writeBinay.Write(byteGender, 0, byteGender.Length);
                writeBinay.Write(byteOffice, 0, byteOffice.Length);
                writeBinay.Write(byteSalary, 0, byteSalary.Length);
            }
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
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            string key = parameters.Split(' ')[0].ToLower(CultureInfo.CurrentCulture);
            string value = parameters.Replace(key, string.Empty, StringComparison.InvariantCultureIgnoreCase).Trim().Trim('"');

            var textInfo = new CultureInfo("ru-RU").TextInfo;
            value = textInfo.ToTitleCase(textInfo.ToLower(value));

            ReadOnlyCollection<FileCabinetRecord> findCollection = null;
            try
            {
                switch (key)
                {
                    case "firstname":
                        findCollection = this.FindByFirstName(value);
                        break;
                    case "lastname":
                        findCollection = this.FindByLastName(value);
                        break;
                    case "dateofbirth":
                        findCollection = this.FindByDateOfBirth(value);
                        break;
                    default:
                        throw new InvalidOperationException($"The {key} isn't a search parameter name. Only 'FirstName', 'LastName' or 'DateOfBirth'.");
                }
            }
            catch (KeyNotFoundException knfex)
            {
                throw new ArgumentException($"The record with {key} '{value}' doesn't exist.", knfex.Message);
            }

            return findCollection;
        }

        /// <summary>
        /// Gets the records.
        /// </summary>
        /// <returns>
        /// All existing records.
        /// </returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            this.fileStream.Position = 0;
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            using (BinaryReader binaryReader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                long count = this.fileStream.Length / RecordInBytesLength;
                while (count-- > 0)
                {
                    if (binaryReader.ReadBytes(2)[0] == 0)
                    {
                        records.Add(new FileCabinetRecord
                        {
                            Id = binaryReader.ReadInt32(),
                            FirstName = System.Text.UnicodeEncoding.Unicode.GetString(
                                binaryReader.ReadBytes(StringInBitesLength), 0, StringInBitesLength).Trim(),
                            LastName = System.Text.UnicodeEncoding.Unicode.GetString(
                                binaryReader.ReadBytes(StringInBitesLength), 0, StringInBitesLength).Trim(),
                            DateOfBirth = new DateTime(binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32()),
                            Gender = binaryReader.ReadChar(),
                            Office = binaryReader.ReadInt16(),
                            Salary = ToDecimal(binaryReader.ReadBytes(DecimalInBitesLength)),
                        });
                    }
                    else
                    {
                        this.fileStream.Seek(RecordInBytesLength - ReservedFieldLength, SeekOrigin.Current);
                    }
                }
            }

            ReadOnlyCollection<FileCabinetRecord> result = new ReadOnlyCollection<FileCabinetRecord>(records);
            return result;
        }

        /// <summary>
        /// Gets the stat.
        /// </summary>
        /// <returns>
        /// The quantity of records.
        /// </returns>
        public int GetStat()
        {
            return (int)(this.fileStream.Position / RecordInBytesLength);
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
            index = -1;
            using (BinaryReader binaryReader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                int count = (int)(this.fileStream.Length / RecordInBytesLength);
                this.fileStream.Seek(0, SeekOrigin.Begin);
                int position = 0;
                while (count-- > 0)
                {
                    if (binaryReader.ReadBytes(2)[0] == 0)
                    {
                        if (binaryReader.ReadInt32() == id)
                        {
                            index = position;
                            return true;
                        }

                        this.fileStream.Seek(-FirstNamePosition, SeekOrigin.Current);
                    }

                    position++;
                    this.fileStream.Seek(RecordInBytesLength, SeekOrigin.Current);
                }
            }

            return false;
        }

        /// <summary>
        /// Makes the snapshot.
        /// </summary>
        /// <returns>The file cabinet service snapshot.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Restores the specified snapshot.
        /// </summary>
        /// <param name="snapshot">The snapshot.</param>
        /// <param name="exceptions">The exceptions.</param>
        /// <exception cref="ArgumentNullException">Snapshot is null.</exception>
        public void Restore(FileCabinetServiceSnapshot snapshot, out Dictionary<int, string> exceptions)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            exceptions = new Dictionary<int, string>();
            var recordsFromFile = snapshot.FileCabinetRecords.ToList();
            using (BinaryReader binaryReader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                this.IdPositionSortedListPlaceholder(binaryReader);

                bool flag = false;
                int existId = -1;
                foreach (var record in recordsFromFile)
                {
                    flag = false;
                    foreach (var idkey in this.idpositionPairs.Keys)
                    {
                        if (record.Id == idkey)
                        {
                            flag = true;
                            existId = idkey;
                        }
                    }

                    if (flag)
                    {
                        long existRecordPosition = this.idpositionPairs[existId];
                        this.fileStream.Seek(existRecordPosition, SeekOrigin.Begin);
                    }
                    else
                    {
                        this.fileStream.Seek(0, SeekOrigin.End);
                        this.idpositionPairs.Add(record.Id, this.fileStream.Position - RecordInBytesLength);
                    }

                    try
                    {
                        if (record.Id <= 0)
                        {
                            throw new ArgumentException(nameof(record.Id));
                        }

                        this.validator.ValidateParameters(record.FirstName, record.LastName, record.DateOfBirth, record.Gender, record.Office, record.Salary);
                        this.WriteToTheBinaryFile(record);
                    }
                    catch (ArgumentException ex)
                    {
                        exceptions.Add(record.Id, ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Removes a record by the identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="position">Record position.</param>
        public void Remove(int id, int position)
        {
            if (id <= 0)
            {
                throw new ArgumentException($"{nameof(id)} have to be larger than zero.", nameof(id));
            }

            if (id <= 0)
            {
                throw new ArgumentException($"{nameof(position)} have to be larger than zero.", nameof(position));
            }

            if (!this.IsThereARecordWithThisId(id, out int index))
            {
                throw new ArgumentException($"Record #{nameof(id)} doesn't exist.", nameof(id));
            }

            this.fileStream.Seek(RecordInBytesLength * index, SeekOrigin.Begin);
            this.fileStream.WriteByte(1);
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

        private static decimal ToDecimal(byte[] bytes)
        {
            if (bytes.Length != 16)
            {
                throw new ArgumentException("A decimal must be created from exactly 16 bytes.", nameof(bytes));
            }

            int[] bits = new int[4];
            for (int i = 0; i <= 15; i += 4)
            {
                bits[i / 4] = BitConverter.ToInt32(bytes, i);
            }

            return new decimal(bits);
        }

        private static FileCabinetRecord CreateNewFileCabinetRecord(BinaryReader binaryReader)
        {
            binaryReader.ReadBytes(ReservedFieldLength);
            return new FileCabinetRecord
            {
                Id = binaryReader.ReadInt32(),
                FirstName = System.Text.UnicodeEncoding.Unicode.GetString(
                                binaryReader.ReadBytes(StringInBitesLength), 0, StringInBitesLength).Trim(),
                LastName = System.Text.UnicodeEncoding.Unicode.GetString(
                                binaryReader.ReadBytes(StringInBitesLength), 0, StringInBitesLength).Trim(),
                DateOfBirth = new DateTime(binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32()),
                Gender = binaryReader.ReadChar(),
                Office = binaryReader.ReadInt16(),
                Salary = ToDecimal(binaryReader.ReadBytes(DecimalInBitesLength)),
            };
        }

        private void IdPositionSortedListPlaceholder(BinaryReader binaryReader)
        {
            this.fileStream.Seek(0, SeekOrigin.Begin);
            int id;
            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
            {
                if (binaryReader.ReadBytes(ReservedFieldLength)[0] == 0)
                {
                    id = binaryReader.ReadInt32();
                    this.fileStream.Seek(-FirstNamePosition, SeekOrigin.Current);
                    if (!this.idpositionPairs.Keys.Contains(id))
                    {
                        this.idpositionPairs.Add(id, this.fileStream.Position);
                    }

                    this.fileStream.Seek(RecordInBytesLength, SeekOrigin.Current);
                }
                else
                {
                    this.fileStream.Seek(-ReservedFieldLength + RecordInBytesLength, SeekOrigin.Current);
                }
            }
        }

        private void WriteToTheBinaryFile(FileCabinetRecord record)
        {
            using (BinaryWriter writeBinay = new BinaryWriter(this.fileStream, Encoding.Unicode, true))
            {
                var byteId = BitConverter.GetBytes(record.Id);
                var byteFirstName = System.Text.UnicodeEncoding.Unicode.GetBytes(record.FirstName.PadRight(60));
                var byteLastName = System.Text.UnicodeEncoding.Unicode.GetBytes(record.LastName.PadRight(60));
                var byteYear = BitConverter.GetBytes(record.DateOfBirth.Year);
                var byteMonth = BitConverter.GetBytes(record.DateOfBirth.Month);
                var byteDay = BitConverter.GetBytes(record.DateOfBirth.Day);
                var byteGender = BitConverter.GetBytes(record.Gender);
                var byteOffice = BitConverter.GetBytes(record.Office);
                var byteSalary = GetBytes(record.Salary);

                writeBinay.Write(new byte[2] { 0, 0 }, 0, 2); // 0 - not deleted, 1 - deleted
                writeBinay.Write(byteId, 0, byteId.Length);
                writeBinay.Write(byteFirstName, 0, byteFirstName.Length);
                writeBinay.Write(byteLastName, 0, byteLastName.Length);
                writeBinay.Write(byteYear, 0, byteYear.Length);
                writeBinay.Write(byteMonth, 0, byteMonth.Length);
                writeBinay.Write(byteDay, 0, byteDay.Length);
                writeBinay.Write(byteGender, 0, byteGender.Length);
                writeBinay.Write(byteOffice, 0, byteOffice.Length);
                writeBinay.Write(byteSalary, 0, byteSalary.Length);
            }
        }

        private ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            var dateList = new List<FileCabinetRecord>();
            using (BinaryReader binaryReader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                int count = (int)(this.fileStream.Length / RecordInBytesLength);
                this.fileStream.Seek(0, SeekOrigin.Begin);
                string firstNameFromFile;
                while (count-- > 0)
                {
                    if (binaryReader.ReadBytes(2)[0] == 0)
                    {
                        this.fileStream.Seek(-ReservedFieldLength + FirstNamePosition, SeekOrigin.Current);
                        firstNameFromFile = System.Text.UnicodeEncoding.Unicode.GetString(
                            binaryReader.ReadBytes(StringInBitesLength), 0, StringInBitesLength).Trim();
                        if (firstNameFromFile == firstName)
                        {
                            this.fileStream.Seek(-LastNamePosition, SeekOrigin.Current);
                            dateList.Add(CreateNewFileCabinetRecord(binaryReader));
                        }
                        else
                        {
                            this.fileStream.Seek(-LastNamePosition + RecordInBytesLength, SeekOrigin.Current);
                        }
                    }
                    else
                    {
                        this.fileStream.Seek(-ReservedFieldLength + RecordInBytesLength, SeekOrigin.Current);
                    }
                }
            }

            var dateCollection = new ReadOnlyCollection<FileCabinetRecord>(dateList);
            return dateCollection;
        }

        private ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            var dateList = new List<FileCabinetRecord>();
            using (BinaryReader binaryReader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                int count = (int)(this.fileStream.Length / RecordInBytesLength);
                this.fileStream.Seek(0, SeekOrigin.Begin);
                string lastNameFromFile;
                while (count-- > 0)
                {
                    if (binaryReader.ReadBytes(2)[0] == 0)
                    {
                        this.fileStream.Seek(-ReservedFieldLength + LastNamePosition, SeekOrigin.Current);
                        lastNameFromFile = System.Text.UnicodeEncoding.Unicode.GetString(
                            binaryReader.ReadBytes(StringInBitesLength), 0, StringInBitesLength).Trim();
                        if (lastNameFromFile == lastName)
                        {
                            this.fileStream.Seek(-DatePosition, SeekOrigin.Current);
                            dateList.Add(CreateNewFileCabinetRecord(binaryReader));
                        }
                        else
                        {
                            this.fileStream.Seek(-DatePosition + RecordInBytesLength, SeekOrigin.Current);
                        }
                    }
                    else
                    {
                        this.fileStream.Seek(-ReservedFieldLength + RecordInBytesLength, SeekOrigin.Current);
                    }
                }
            }

            var dateCollection = new ReadOnlyCollection<FileCabinetRecord>(dateList);
            return dateCollection;
        }

        private ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            var dateList = new List<FileCabinetRecord>();
            using (BinaryReader binaryReader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                int count = (int)(this.fileStream.Length / RecordInBytesLength);
                this.fileStream.Seek(0, SeekOrigin.Begin);
                DateTime dateFromFile;
                while (count-- > 0)
                {
                    if (binaryReader.ReadBytes(2)[0] == 0)
                    {
                        this.fileStream.Seek(-ReservedFieldLength + DatePosition, SeekOrigin.Current);
                        dateFromFile = new DateTime(binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32());
                        if (DateTime.TryParse(dateOfBirth, out DateTime date))
                        {
                            if (DateTime.Compare(dateFromFile, date) == 0)
                            {
                                this.fileStream.Seek(-GenderPosition, SeekOrigin.Current);
                                dateList.Add(CreateNewFileCabinetRecord(binaryReader));
                            }
                        }
                        else
                        {
                            this.fileStream.Seek(-GenderPosition + RecordInBytesLength, SeekOrigin.Current);
                        }
                    }
                    else
                    {
                        this.fileStream.Seek(-ReservedFieldLength + RecordInBytesLength, SeekOrigin.Current);
                    }
                }
            }

            var dateCollection = new ReadOnlyCollection<FileCabinetRecord>(dateList);
            return dateCollection;
        }
    }
}
