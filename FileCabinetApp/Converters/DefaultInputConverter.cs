using System;
using System.Globalization;

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
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

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
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            bool flag = DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);

            var tuple = new Tuple<bool, string, DateTime>(flag, "You can use only the MM.DD.YYYY, MM/DD/YYYY, YYYY-MM-DD, format", date);
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
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            bool flag = char.TryParse(input, out char status);

            var tuple = new Tuple<bool, string, char>(flag, "Should be a char", status);
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
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            bool flag = short.TryParse(input, out short output);

            var tuple = new Tuple<bool, string, short>(flag, "Should be a number", output);
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
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            bool flag = decimal.TryParse(input, out decimal output);

            var tuple = new Tuple<bool, string, decimal>(flag, "Should be a decimal number", output);
            return tuple;
        }
    }
}