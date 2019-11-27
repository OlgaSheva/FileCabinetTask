using System;
using System.IO;

namespace FileCabinetApp.Services.Extensions
{
    /// <summary>
    /// Extensions to binary reader.
    /// </summary>
    public static class BinaryReaderExtensions
    {
        private const int RecordInBytesLength // 278
            = (sizeof(byte) * 2) // status
            + sizeof(int) // id
            + (NameInBytesLength * 2) // first and last name
            + (sizeof(int) * 3) // dateofbirth
            + sizeof(char) // gender
            + sizeof(short) // office
            + sizeof(decimal); // salary

        private const int StatusInBytesLength = sizeof(byte) * 2;
        private const int FirstNamePosition = StatusInBytesLength + sizeof(int);
        private const int NameInBytesLength = 120;

        /// <summary>
        /// Gets the record status.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="fileStream">The file stream.</param>
        /// <returns>Record status: false - is deleted, true - is alive.</returns>
        /// <exception cref="ArgumentNullException">
        /// reader
        /// or
        /// fileStream.
        /// </exception>
        public static bool IsTheRecordDelete(this BinaryReader reader, FileStream fileStream)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (fileStream == null)
            {
                throw new ArgumentNullException(nameof(fileStream));
            }

            byte[] status = reader.ReadBytes(sizeof(byte));
            fileStream.Seek(-sizeof(byte), SeekOrigin.Current);

            return status[0] == 1;
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="fileStream">The file stream.</param>
        /// <returns>Record ID.</returns>
        /// <exception cref="ArgumentNullException">
        /// reader
        /// or
        /// fileStream.
        /// </exception>
        public static int GetId(this BinaryReader reader, FileStream fileStream)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (fileStream == null)
            {
                throw new ArgumentNullException(nameof(fileStream));
            }

            fileStream.Seek(sizeof(byte) * 2, SeekOrigin.Current);
            int id = reader.ReadInt32();
            fileStream.Seek(-FirstNamePosition, SeekOrigin.Current);

            return id;
        }

        /// <summary>
        /// Gets the record parameters.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="fileStream">The file stream.</param>
        /// <returns>Records parameters.</returns>
        /// <exception cref="ArgumentNullException">
        /// reader
        /// or
        /// fileStream.
        /// </exception>
        public static RecordParameters GetRecordParameters(this BinaryReader reader, FileStream fileStream)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (fileStream == null)
            {
                throw new ArgumentNullException(nameof(fileStream));
            }

            RecordParameters recordParameters = GetRecordParametersFromFile(reader, fileStream);
            fileStream.Seek(-RecordInBytesLength, SeekOrigin.Current);

            return recordParameters;
        }

        /// <summary>
        /// Gets the file cabinet record from file.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="position">The position.</param>
        /// <returns>File cabinet record.</returns>
        /// <exception cref="ArgumentNullException">
        /// reader
        /// or
        /// fileStream.
        /// </exception>
        public static FileCabinetRecord GetFileCabinetRecordFromFile(this BinaryReader reader, FileStream fileStream, long position)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (fileStream == null)
            {
                throw new ArgumentNullException(nameof(fileStream));
            }

            if (position < 0)
            {
                throw new ArgumentException($"The {nameof(position)} cannot be less than zero.", nameof(position));
            }

            fileStream.Seek(position, SeekOrigin.Begin);
            reader.ReadBytes(StatusInBytesLength);
            return new FileCabinetRecord
            {
                Id = reader.ReadInt32(),
                FirstName = System.Text.UnicodeEncoding.Unicode.GetString(
                                reader.ReadBytes(NameInBytesLength), 0, NameInBytesLength).Trim(),
                LastName = System.Text.UnicodeEncoding.Unicode.GetString(
                                reader.ReadBytes(NameInBytesLength), 0, NameInBytesLength).Trim(),
                DateOfBirth = new DateTime(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()),
                Gender = reader.ReadChar(),
                Office = reader.ReadInt16(),
                Salary = reader.ReadDecimal(),
            };
        }

        private static RecordParameters GetRecordParametersFromFile(BinaryReader reader, FileStream fileStream)
        {
            fileStream.Seek((sizeof(byte) * 2) + sizeof(int), SeekOrigin.Current);
            return new RecordParameters(
                System.Text.UnicodeEncoding.Unicode.GetString(
                                reader.ReadBytes(NameInBytesLength), 0, NameInBytesLength).Trim(),
                System.Text.UnicodeEncoding.Unicode.GetString(
                                reader.ReadBytes(NameInBytesLength), 0, NameInBytesLength).Trim(),
                new DateTime(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()),
                reader.ReadChar(),
                reader.ReadInt16(),
                reader.ReadDecimal());
        }
    }
}
