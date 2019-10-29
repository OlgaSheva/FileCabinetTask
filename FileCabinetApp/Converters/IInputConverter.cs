using System;

namespace FileCabinetApp.Converters
{
    /// <summary>
    /// Input parameters convertor.
    /// </summary>
    public interface IInputConverter
    {
        /// <summary>
        /// Strings the converter.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>String representation.</returns>
        Tuple<bool, string, string> StringConverter(string input);

        /// <summary>
        /// Dates the converter.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>DateTime representation.</returns>
        Tuple<bool, string, DateTime> DateConverter(string input);

        /// <summary>
        /// Characters the converter.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Char representation.</returns>
        Tuple<bool, string, char> CharConverter(string input);

        /// <summary>
        /// Shorts the converter.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Short representation.</returns>
        Tuple<bool, string, short> ShortConverter(string input);

        /// <summary>
        /// Decimals the converter.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Decimal representation.</returns>
        Tuple<bool, string, decimal> DecimalConverter(string input);
    }
}
