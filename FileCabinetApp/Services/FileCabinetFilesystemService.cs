using System;
using System.Collections.Generic;
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
        private const int IdPosition = 2;
        private const int FirstNamePosition = 6;
        private const int GenderPosition = 258;
        private const int StringInBitesLength = 120;
        private const int DecimalInBitesLength = 16;
        private readonly FileStream fileStream;
        private readonly IRecordValidator validator;
        private SortedDictionary<int, long> idpositionPairs;
        private Dictionary<string, List<long>> firstNameDictionary;
        private Dictionary<string, List<long>> lastNameDictionary;
        private Dictionary<DateTime, List<long>> dateOfBirthDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="validator">The validator.</param>
        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator)
        {
            this.fileStream = fileStream ?? throw new ArgumentNullException(nameof(fileStream));
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));

            this.fileStream.SetLength(0);
            this.FillAllDictionaries();
        }

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="rec">The record.</param>
        /// <returns>
        /// New file cabinet record.
        /// </returns>
        public int CreateRecord(RecordParameters rec)
        {
            if (rec == null)
            {
                throw new ArgumentNullException(nameof(rec));
            }

            int id = this.idpositionPairs.Count > 0 ? this.idpositionPairs.Keys.Last() + 1 : 1;

            this.InsertRecord(rec, id);

            return id;
        }

        /// <summary>
        /// Inserts the record.
        /// </summary>
        /// <param name="rec">The record.</param>
        /// <param name="id">The identifier.</param>
        /// <exception cref="ArgumentNullException">rec is null.</exception>
        /// <exception cref="ArgumentException">The '{nameof(id)}' can not be less than one.</exception>
        public void InsertRecord(RecordParameters rec, int id)
        {
            if (rec == null)
            {
                throw new ArgumentNullException(nameof(rec));
            }

            if (id < 1)
            {
                throw new ArgumentException($"The '{nameof(id)}' can not be less than one.", nameof(id));
            }

            this.validator.ValidateParameters(rec);
            this.fileStream.Seek(0, SeekOrigin.End);
            this.CreateFileCabinetRecord(rec, id);
        }

        /// <summary>
        /// Updates the specified records to update.
        /// </summary>
        /// <param name="recordsToUpdate">The records to update.</param>
        /// <param name="recordParameters">The record parameters.</param>
        /// <param name="keyValuePairs">The key value pairs.</param>
        /// <returns>
        /// IDs of updated records.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// recordsToUpdate
        /// or
        /// recordParameters
        /// or
        /// keyValuePairs.
        /// </exception>
        public List<int> Update(IEnumerable<FileCabinetRecord> recordsToUpdate, RecordParameters recordParameters, List<KeyValuePair<string, string>> keyValuePairs)
        {
            if (recordsToUpdate == null)
            {
                throw new ArgumentNullException(nameof(recordsToUpdate));
            }

            if (recordParameters == null)
            {
                throw new ArgumentNullException(nameof(recordParameters));
            }

            if (keyValuePairs == null)
            {
                throw new ArgumentNullException(nameof(keyValuePairs));
            }

            List<int> ids = new List<int>();
            int id;
            long position;
            foreach (var record in recordsToUpdate)
            {
                id = record.Id;
                position = this.idpositionPairs[id];
                this.GetRecordWithNewParameters(recordParameters, out id, position, out RecordParameters newRecordParameters);
                this.EditRecord(id, newRecordParameters);
                ids.Add(id);
            }

            return ids;
        }

        /// <summary>
        /// Gets the records.
        /// </summary>
        /// <returns>
        /// All records.
        /// </returns>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            using (BinaryReader binaryReader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                foreach (var item in this.idpositionPairs)
                {
                    yield return this.GetFileCabinetRecordFromFile(binaryReader, item.Value);
                }
            }
        }

        /// <summary>
        /// Gets the stat.
        /// </summary>
        /// <param name="deletedRecordsCount">The deleted records count.</param>
        /// <returns>
        /// The quantity of records.
        /// </returns>
        public int GetStat(out int deletedRecordsCount)
        {
            this.fileStream.Seek(0, SeekOrigin.End);
            int recordsCount = (int)(this.fileStream.Position / RecordInBytesLength);
            deletedRecordsCount = recordsCount - this.idpositionPairs.Count;
            return recordsCount;
        }

        /// <summary>
        /// Determines whether [is there a record with this identifier] [the specified identifier].
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="position">The index.</param>
        /// <returns>
        /// <c>true</c> if [is there a record with this identifier] [the specified identifier]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsThereARecordWithThisId(int id, out long position)
        {
            position = -1;
            if (this.idpositionPairs.ContainsKey(id))
            {
                position = this.idpositionPairs[id];
                return true;
            }

            return false;
        }

        /// <summary>
        /// Makes the snapshot.
        /// </summary>
        /// <returns>The file cabinet service snapshot.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            var snapshot = new FileCabinetServiceSnapshot(this.GetRecords().ToList());
            return snapshot;
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

            long position;
            exceptions = new Dictionary<int, string>();
            var recordsFromFile = snapshot.FileCabinetRecords.ToList();
            using (BinaryReader binaryReader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                this.FillAllDictionaries();

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
                            break;
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
                    }

                    try
                    {
                        if (record.Id <= 0)
                        {
                            throw new ArgumentException(nameof(record.Id));
                        }

                        var recordParameters = new RecordParameters(
                            record.FirstName, record.LastName, record.DateOfBirth, record.Gender, record.Office, record.Salary);
                        this.validator.ValidateParameters(recordParameters);
                        if (this.idpositionPairs.ContainsKey(record.Id))
                        {
                            this.RemoveFromDictionaries(record.Id);
                            this.fileStream.Seek(-GenderPosition, SeekOrigin.Current);
                            this.idpositionPairs.Remove(record.Id);
                        }

                        if (!this.idpositionPairs.ContainsKey(record.Id))
                        {
                            position = this.fileStream.Position;
                            this.idpositionPairs.Add(record.Id, position);
                            this.AddToDictionaries(recordParameters, position);
                        }

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
        /// Deletes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// Deleted records id.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// key
        /// or
        /// value.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Search by key '{key}' does not supported. - key
        /// or
        /// There are no records with {key} = '{value}'.
        /// </exception>
        public List<int> Delete(string key, string value)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            List<int> ids;
            dynamic dictionary;
            dynamic searchKey;
            try
            {
                ids = this.GetIdsOfRecordsThatMathForKeyValue(key, value, out dictionary, out searchKey);
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException($"There are no records with {key} = '{value}'.");
            }

            this.RemoveRecords(ids, dictionary, searchKey);

            return ids;
        }

        /// <summary>
        /// Purges the specified deleted records count.
        /// </summary>
        /// <param name="deletedRecordsCount">The deleted records count.</param>
        /// <param name="recordsCount">The records count.</param>
        public void Purge(out int deletedRecordsCount, out int recordsCount)
        {
            this.fileStream.Seek(0, SeekOrigin.Begin);
            int count = (int)(this.fileStream.Length / RecordInBytesLength);
            recordsCount = count;
            deletedRecordsCount = 0;
            byte[] buffer = new byte[RecordInBytesLength];
            Queue<long> deletedRecordPositions = new Queue<long>();
            long lastDeletedRecordStart = -1;
            int deadRecordsCount = 0;
            long lastAliveRecordEnd = 0;
            int id;
            RecordParameters recordParameters;

            using (BinaryReader reader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            using (BinaryWriter writer = new BinaryWriter(this.fileStream, Encoding.Unicode, true))
            {
                while (count-- > 0)
                {
                    if (reader.ReadBytes(ReservedFieldLength)[0] == 1)
                    {
                        this.fileStream.Seek(-ReservedFieldLength, SeekOrigin.Current);
                        deletedRecordPositions.Enqueue(this.fileStream.Position);
                        deletedRecordsCount++;
                        this.fileStream.Seek(RecordInBytesLength, SeekOrigin.Current);
                    }
                    else
                    {
                        id = reader.ReadInt32();
                        this.fileStream.Seek(-FirstNamePosition, SeekOrigin.Current);
                        recordParameters = GetRecordParametersFromFile(reader);
                        this.fileStream.Seek(-RecordInBytesLength, SeekOrigin.Current);
                        if (deletedRecordPositions.TryDequeue(out lastDeletedRecordStart))
                        {
                            buffer = reader.ReadBytes(RecordInBytesLength);
                            this.fileStream.Seek(-RecordInBytesLength, SeekOrigin.Current);
                            deletedRecordPositions.Enqueue(this.fileStream.Position);
                            this.fileStream.WriteByte(1); // deleted

                            deadRecordsCount = (int)((this.fileStream.Position - lastAliveRecordEnd) / RecordInBytesLength);
                            this.RemoveFromDictionaries(id);
                            this.idpositionPairs[id] = lastDeletedRecordStart;
                            this.AddToDictionaries(recordParameters, lastDeletedRecordStart);
                            this.fileStream.Seek(lastDeletedRecordStart, SeekOrigin.Begin);
                            writer.Write(buffer, 0, RecordInBytesLength);

                            lastAliveRecordEnd = this.fileStream.Position;
                            this.fileStream.Seek(RecordInBytesLength * deadRecordsCount, SeekOrigin.Current);
                        }
                        else
                        {
                            this.fileStream.Seek(RecordInBytesLength, SeekOrigin.Current);
                            lastAliveRecordEnd = this.fileStream.Position;
                        }
                    }
                }
            }

            this.fileStream.SetLength(lastAliveRecordEnd);
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

        private static RecordParameters GetRecordParametersFromFile(BinaryReader binaryReader)
        {
            binaryReader.ReadBytes(ReservedFieldLength);
            binaryReader.ReadInt32();
            return new RecordParameters(
                System.Text.UnicodeEncoding.Unicode.GetString(
                                binaryReader.ReadBytes(StringInBitesLength), 0, StringInBitesLength).Trim(),
                System.Text.UnicodeEncoding.Unicode.GetString(
                                binaryReader.ReadBytes(StringInBitesLength), 0, StringInBitesLength).Trim(),
                new DateTime(binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32()),
                binaryReader.ReadChar(),
                binaryReader.ReadInt16(),
                ToDecimal(binaryReader.ReadBytes(DecimalInBitesLength)));
        }

        private FileCabinetRecord GetFileCabinetRecordFromFile(BinaryReader binaryReader, long position)
        {
            if (binaryReader == null)
            {
                throw new ArgumentNullException(nameof(binaryReader));
            }

            this.fileStream.Seek(position, SeekOrigin.Begin);
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

        private void EditRecord(int id, RecordParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (id <= 0)
            {
                throw new ArgumentException($"The {nameof(id)} have to be larger than zero.", nameof(id));
            }

            if (!this.IsThereARecordWithThisId(id, out long position))
            {
                throw new ArgumentException($"Record #{nameof(id)} doesn't exist.", nameof(id));
            }

            this.validator.ValidateParameters(parameters);

            this.RemoveFromDictionaries(id);

            var byteFirstName = System.Text.UnicodeEncoding.Unicode.GetBytes(parameters.FirstName.PadRight(60));
            var byteLastName = System.Text.UnicodeEncoding.Unicode.GetBytes(parameters.LastName.PadRight(60));
            var byteYear = BitConverter.GetBytes(parameters.DateOfBirth.Year);
            var byteMonth = BitConverter.GetBytes(parameters.DateOfBirth.Month);
            var byteDay = BitConverter.GetBytes(parameters.DateOfBirth.Day);
            var byteGender = BitConverter.GetBytes(parameters.Gender);
            var byteOffice = BitConverter.GetBytes(parameters.Office);
            var byteSalary = GetBytes(parameters.Salary);

            this.fileStream.Seek(position + FirstNamePosition, SeekOrigin.Begin);

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

            this.AddToDictionaries(parameters, position);
        }

        private void CreateFileCabinetRecord(RecordParameters rec, int id)
        {
            this.validator.ValidateParameters(rec);
            var record = new FileCabinetRecord
            {
                Id = id,
                FirstName = rec.FirstName,
                LastName = rec.LastName,
                DateOfBirth = rec.DateOfBirth,
                Gender = rec.Gender,
                Office = rec.Office,
                Salary = rec.Salary,
            };

            this.WriteToTheBinaryFile(record);
            long recordPosition = this.fileStream.Position - RecordInBytesLength;
            this.idpositionPairs.Add(record.Id, recordPosition);
            this.AddToDictionaries(rec, recordPosition);
        }

        private void FillAllDictionaries()
        {
            this.idpositionPairs = new SortedDictionary<int, long>();
            this.firstNameDictionary = new Dictionary<string, List<long>>();
            this.lastNameDictionary = new Dictionary<string, List<long>>();
            this.dateOfBirthDictionary = new Dictionary<DateTime, List<long>>();

            this.fileStream.Seek(0, SeekOrigin.Begin);
            int id;
            long position;

            using (BinaryReader reader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    if (reader.ReadBytes(ReservedFieldLength)[0] == 0)
                    {
                        id = reader.ReadInt32();
                        this.fileStream.Seek(-FirstNamePosition, SeekOrigin.Current);
                        position = this.fileStream.Position;
                        if (!this.idpositionPairs.Keys.Contains(id))
                        {
                            this.idpositionPairs.Add(id, position);
                        }

                        var parameters = GetRecordParametersFromFile(reader);
                        this.AddToDictionaries(parameters, position);
                    }
                    else
                    {
                        this.fileStream.Seek(-ReservedFieldLength + RecordInBytesLength, SeekOrigin.Current);
                    }
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

        private void AddToDictionaries(RecordParameters parameters, long recordPosition)
        {
            if (!this.firstNameDictionary.ContainsKey(parameters.FirstName))
            {
                this.firstNameDictionary.Add(parameters.FirstName, new List<long>());
            }

            if (!this.lastNameDictionary.ContainsKey(parameters.LastName))
            {
                this.lastNameDictionary.Add(parameters.LastName, new List<long>());
            }

            if (!this.dateOfBirthDictionary.ContainsKey(parameters.DateOfBirth))
            {
                this.dateOfBirthDictionary.Add(parameters.DateOfBirth, new List<long>());
            }

            this.firstNameDictionary[parameters.FirstName].Add(recordPosition);
            this.lastNameDictionary[parameters.LastName].Add(recordPosition);
            this.dateOfBirthDictionary[parameters.DateOfBirth].Add(recordPosition);
        }

        private void RemoveFromDictionaries(int id)
        {
            long recordPositon = this.idpositionPairs[id];
            this.fileStream.Seek(recordPositon + FirstNamePosition, SeekOrigin.Begin);
            string firstName;
            string lastName;
            DateTime dateOfBirth;
            using (var reader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            {
                firstName = System.Text.UnicodeEncoding.Unicode.GetString(
                            reader.ReadBytes(StringInBitesLength), 0, StringInBitesLength).Trim();
                lastName = System.Text.UnicodeEncoding.Unicode.GetString(
                            reader.ReadBytes(StringInBitesLength), 0, StringInBitesLength).Trim();
                dateOfBirth = new DateTime(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
            }

            this.firstNameDictionary[firstName].Remove(recordPositon);
            this.lastNameDictionary[lastName].Remove(recordPositon);
            this.dateOfBirthDictionary[dateOfBirth].Remove(recordPositon);

            if (this.firstNameDictionary[firstName].Count == 0)
            {
                this.firstNameDictionary.Remove(firstName);
            }

            if (this.lastNameDictionary[lastName].Count == 0)
            {
                this.lastNameDictionary.Remove(lastName);
            }

            if (this.dateOfBirthDictionary[dateOfBirth].Count == 0)
            {
                this.dateOfBirthDictionary.Remove(dateOfBirth);
            }
        }

        private void GetRecordWithNewParameters(RecordParameters recordParameters, out int id, long position, out RecordParameters newRecordParameters)
        {
            string pfirstname = null;
            string plastname = null;
            DateTime pdateofbirth = default(DateTime);
            char pgender = default(char);
            short poffice = -1;
            decimal psalary = -1;

            using (BinaryReader reader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
            using (BinaryWriter writeBinay = new BinaryWriter(this.fileStream, Encoding.Unicode, true))
            {
                this.fileStream.Seek(position, SeekOrigin.Begin);
                var existRecordsParameters = GetRecordParametersFromFile(reader);
                this.fileStream.Seek(position + (long)IdPosition, SeekOrigin.Begin);
                id = reader.ReadInt32();
                if (recordParameters.FirstName != null)
                {
                    pfirstname = recordParameters.FirstName;
                }
                else
                {
                    pfirstname = existRecordsParameters.FirstName;
                }

                if (recordParameters.LastName != null)
                {
                    plastname = recordParameters.LastName;
                }
                else
                {
                    plastname = existRecordsParameters.LastName;
                }

                if (recordParameters.DateOfBirth != default(DateTime))
                {
                    pdateofbirth = recordParameters.DateOfBirth;
                }
                else
                {
                    pdateofbirth = existRecordsParameters.DateOfBirth;
                }

                if (recordParameters.Gender != default(char))
                {
                    pgender = recordParameters.Gender;
                }
                else
                {
                    pgender = existRecordsParameters.Gender;
                }

                if (recordParameters.Office != -1)
                {
                    poffice = recordParameters.Office;
                }
                else
                {
                    poffice = existRecordsParameters.Office;
                }

                if (recordParameters.Salary != -1)
                {
                    psalary = recordParameters.Salary;
                }
                else
                {
                    psalary = existRecordsParameters.Salary;
                }
            }

            newRecordParameters = new RecordParameters(pfirstname, plastname, pdateofbirth, pgender, poffice, psalary);
        }

        private List<int> GetIdsOfRecordsThatMathForKeyValue(string key, string value, out dynamic dictionary, out dynamic searchKey)
        {
            List<int> ids = new List<int>();
            switch (key.ToUpperInvariant())
            {
                case "ID":
                    if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int id))
                    {
                        searchKey = id;
                        dictionary = new Dictionary<int, List<long>>()
                        { { (int)searchKey, new List<long>() { this.idpositionPairs[searchKey] } }, };
                    }
                    else
                    {
                        throw new ArgumentException("Wrong id format.", nameof(value));
                    }

                    break;
                case "FIRSTNAME":
                    dictionary = this.firstNameDictionary;
                    searchKey = value;
                    break;
                case "LASTNAME":
                    dictionary = this.lastNameDictionary;
                    searchKey = value;
                    break;
                case "DATEOFBIRTH":
                    if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                    {
                        searchKey = date;
                        dictionary = this.dateOfBirthDictionary;
                    }
                    else
                    {
                        throw new ArgumentException("You can use only the MM.DD.YYYY, MM/DD/YYYY, YYYY-MM-DD, format", nameof(value));
                    }

                    break;
                default:
                    throw new ArgumentException($"Search by key '{key}' does not supported.", nameof(key));
            }

            return ids;
        }

        private void RemoveRecords(List<int> ids, dynamic dictionary, dynamic searchKey)
        {
            if (dictionary.TryGetValue(searchKey, out List<long> positionList))
            {
                foreach (var position in positionList)
                {
                    this.fileStream.Seek(position, SeekOrigin.Begin);
                    this.fileStream.WriteByte(1);
                    this.fileStream.Seek(position + ReservedFieldLength, SeekOrigin.Begin);
                    int id = -1;
                    using (var reader = new BinaryReader(this.fileStream, Encoding.Unicode, true))
                    {
                        id = reader.ReadInt32();
                        ids.Add(id);
                    }
                }

                foreach (var id in ids)
                {
                    this.RemoveFromDictionaries(id);
                    this.idpositionPairs.Remove(id);
                }
            }
        }
    }
}