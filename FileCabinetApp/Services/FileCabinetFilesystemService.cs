using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
        private const int RecordInBytesLength = 278;
        private const int IdPosition = 2;
        private const int FirstNamePosition = 6;
        private const int LastNamePosition = 126;
        private const int YearPosition = 246;
        private const int MonthPosition = 250;
        private const int DayPosition = 254;
        private const int GenderPosition = 258;
        private const int OfficePosition = 260;
        private const int SalaryPosition = 262;
        private const int StringInBitesLength = 120;
        private const int DecimalInBitesLength = 16;
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

            var byteId = BitConverter.GetBytes(record.Id);
            var byteFirstName = System.Text.UnicodeEncoding.Unicode.GetBytes(record.FirstName.PadRight(60));
            var byteLastName = System.Text.UnicodeEncoding.Unicode.GetBytes(record.LastName.PadRight(60));
            var byteYear = BitConverter.GetBytes(record.DateOfBirth.Year);
            var byteMonth = BitConverter.GetBytes(record.DateOfBirth.Month);
            var byteDay = BitConverter.GetBytes(record.DateOfBirth.Day);
            var byteGender = BitConverter.GetBytes(record.Gender);
            var byteOffice = BitConverter.GetBytes(record.Office);
            var byteSalary = GetBytes(record.Salary);

            using (BinaryWriter writeBinay = new BinaryWriter(this.fileStream, Encoding.Unicode, true))
            {
                writeBinay.Write(new byte[2], 0, 2); // reserved
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

            return record.Id;
        }

        /// <summary>
        /// Edits the record.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="record">The record.</param>
        public void EditRecord(int id, Record record)
        {
            if (id < 0)
            {
                throw new ArgumentException(nameof(id), $"The {nameof(id)} can't be less than zero.");
            }

            if (!this.IsThereARecordWithThisId(id, out int index))
            {
                throw new ArgumentException(nameof(id), $"The {nameof(id)} doesn't exist.");
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

            this.fileStream.Position = (RecordInBytesLength * (index - 1)) + 2 + BitConverter.GetBytes(default(int)).Length;

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
            var param = parameters.Split(' ');
            string parameterName = param[0];
            string parameterValue = param[1].Trim('"');

            var textInfo = new CultureInfo("ru-RU").TextInfo;
            parameterValue = textInfo.ToTitleCase(textInfo.ToLower(parameterValue));

            ReadOnlyCollection<FileCabinetRecord> findCollection = null;
            try
            {
                switch (parameterName.ToLower())
                {
                    case "firstname":
                        findCollection = this.FindByFirstName(parameterValue);
                        break;
                    case "lastname":
                        findCollection = this.FindByLastName(parameterValue);
                        break;
                    case "dateofbirth":
                        findCollection = this.FindByDateOfBirth(parameterValue);
                        break;
                    default:
                        throw new InvalidOperationException($"The {parameterName} isn't a search parameter name. Only 'FirstName', 'LastName' or 'DateOfBirth'.");
                }
            }
            catch (KeyNotFoundException knfex)
            {
                throw new ArgumentException($"The record with {parameterName} '{parameterValue}' doesn't exist.", knfex.Message);
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
                    binaryReader.ReadBytes(2);
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
                this.fileStream.Position = 2;
                while (count-- > 0)
                {
                    if (binaryReader.ReadInt32() == id)
                    {
                        index = id;
                        return true;
                    }

                    this.fileStream.Position += RecordInBytesLength - BitConverter.GetBytes(default(int)).Length;
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
                throw new ArgumentException(nameof(bytes), "A decimal must be created from exactly 16 bytes.");
            }

            int[] bits = new int[4];
            for (int i = 0; i <= 15; i += 4)
            {
                bits[i / 4] = BitConverter.ToInt32(bytes, i);
            }

            return new decimal(bits);
        }

        private ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            var dateList = new List<FileCabinetRecord>();
            using (BinaryReader binaryReader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                int count = (int)(this.fileStream.Length / RecordInBytesLength);
                this.fileStream.Position = FirstNamePosition;
                string firstNameFromFile;
                while (count-- > 0)
                {
                    firstNameFromFile = System.Text.UnicodeEncoding.Unicode.GetString(
                        binaryReader.ReadBytes(StringInBitesLength), 0, StringInBitesLength).Trim();
                    if (firstNameFromFile.Equals(firstName))
                    {
                        this.fileStream.Position -= LastNamePosition;
                        binaryReader.ReadBytes(2);
                        dateList.Add(this.CreateNewFileCabinetRecord(binaryReader));
                    }

                    this.fileStream.Position += RecordInBytesLength - LastNamePosition + FirstNamePosition;
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
                this.fileStream.Position = LastNamePosition;
                string firstNameFromFile;
                while (count-- > 0)
                {
                    firstNameFromFile = System.Text.UnicodeEncoding.Unicode.GetString(
                        binaryReader.ReadBytes(StringInBitesLength), 0, StringInBitesLength).Trim();
                    if (firstNameFromFile.Equals(lastName))
                    {
                        this.fileStream.Position -= YearPosition;
                        binaryReader.ReadBytes(2);
                        dateList.Add(this.CreateNewFileCabinetRecord(binaryReader));
                    }

                    this.fileStream.Position += RecordInBytesLength - YearPosition + LastNamePosition;
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
                this.fileStream.Position = YearPosition;
                DateTime dateFromFile;
                while (count-- > 0)
                {
                    dateFromFile = new DateTime(binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32());
                    if (DateTime.TryParse(dateOfBirth, out DateTime date))
                    {
                        if (DateTime.Compare(dateFromFile, date) == 0)
                        {
                            this.fileStream.Position -= GenderPosition;
                            binaryReader.ReadBytes(2);
                            dateList.Add(this.CreateNewFileCabinetRecord(binaryReader));
                        }
                    }

                    this.fileStream.Position += RecordInBytesLength - GenderPosition + YearPosition;
                }
            }

            var dateCollection = new ReadOnlyCollection<FileCabinetRecord>(dateList);
            return dateCollection;
        }

        private FileCabinetRecord CreateNewFileCabinetRecord(BinaryReader binaryReader)
        {
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
    }
}
