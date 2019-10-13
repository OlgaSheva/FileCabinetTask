using System;
using System.Globalization;
using FileCabinetApp.Enums;

namespace FileCabinetApp.Converters
{
    /// <summary>
    /// Default input parameters convertor.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Converters.IInputConverter" />
    internal class DefaultInputConverter : IInputConverter
    {
        /// <summary>
        /// Strings the converter.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        /// String representation.
        /// </returns>
        public Tuple<bool, string, string> StringConverter(string input)
        {
            string output = input.Trim('"');

            var textInfo = new CultureInfo("ru-RU").TextInfo;
            output = textInfo.ToTitleCase(textInfo.ToLower(input));

            var tuple = new Tuple<bool, string, string>(true, input, output);
            return tuple;
        }

        /// <summary>
        /// Dates the converter.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        /// DateTime representation.
        /// </returns>
        public Tuple<bool, string, DateTime> DateConverter(string input)
        {
            bool flag = DateTime.TryParse(input, out DateTime date);

            var tuple = new Tuple<bool, string, DateTime>(flag, input, date);
            return tuple;
        }

        /// <summary>
        /// Genders the converter.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        /// Gender enum representation.
        /// </returns>
        public Tuple<bool, string, Gender> GenderConverter(string input)
        {
            bool flag = Enum.TryParse(input, out Gender gender);

            var tuple = new Tuple<bool, string, Gender>(flag, input, gender);
            return tuple;
        }

        /// <summary>
        /// Characters the converter.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        /// Char representation.
        /// </returns>
        public Tuple<bool, string, char> CharConverter(string input)
        {
            bool flag = char.TryParse(input, out char status);

            var tuple = new Tuple<bool, string, char>(flag, input, status);
            return tuple;
        }

        /// <summary>
        /// Shorts the converter.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        /// Short representation.
        /// </returns>
        public Tuple<bool, string, short> ShortConverter(string input)
        {
            bool flag = short.TryParse(input, out short output);

            var tuple = new Tuple<bool, string, short>(flag, input, output);
            return tuple;
        }

        /// <summary>
        /// Decimals the converter.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        /// Decimal representation.
        /// </returns>
        public Tuple<bool, string, decimal> DecimalConverter(string input)
        {
            bool flag = decimal.TryParse(input, out decimal output);

            var tuple = new Tuple<bool, string, decimal>(flag, input, output);
            return tuple;
        }
    }
}
