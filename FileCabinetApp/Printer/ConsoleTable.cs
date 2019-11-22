using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace FileCabinetApp.Printer
{
    /// <summary>
    /// Console table.
    /// </summary>
    public class ConsoleTable
    {
        private static HashSet<Type> numericTypes = new HashSet<Type>
        {
            typeof(int),  typeof(double),  typeof(decimal),
            typeof(long), typeof(short),   typeof(sbyte),
            typeof(byte), typeof(ulong),   typeof(ushort),
            typeof(uint), typeof(float),
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleTable"/> class.
        /// </summary>
        /// <param name="columns">The columns.</param>
        public ConsoleTable(params string[] columns)
            : this(new ConsoleTableOptions { Columns = new List<string>(columns) })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleTable"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <exception cref="ArgumentNullException">options.</exception>
        public ConsoleTable(ConsoleTableOptions options)
        {
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
            this.Rows = new List<object[]>();
            this.Columns = new List<object>(options.Columns);
        }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public IList<object> Columns { get; }

        /// <summary>
        /// Gets the rows.
        /// </summary>
        /// <value>
        /// The rows.
        /// </value>
        public IList<object[]> Rows { get; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public ConsoleTableOptions Options { get; protected set; }

        /// <summary>
        /// Gets the column types.
        /// </summary>
        /// <value>
        /// The column types.
        /// </value>
        public List<Type> ColumnTypes { get; private set; }

        /// <summary>
        /// Froms the specified values.
        /// </summary>
        /// <typeparam name="T">The t.</typeparam>
        /// <param name="values">The values.</param>
        /// <param name="columnsList">The columns list.</param>
        /// <returns>Console table.</returns>
        public static ConsoleTable From<T>(IEnumerable<T> values, List<string> columnsList)
        {
            var table = new ConsoleTable
            {
                ColumnTypes = GetColumnsType<T>().ToList(),
            };

            var columns = GetColumns<T>(columnsList);

            table.AddColumn(columns);

            foreach (
                var propertyValues
                in values.Select(value => columns.Select(column => GetColumnValue<T>(value, column))))
            {
                table.AddRow(propertyValues.ToArray());
            }

            return table;
        }

        /// <summary>
        /// Adds the column.
        /// </summary>
        /// <param name="names">The names.</param>
        /// <returns>Console table.</returns>
        public ConsoleTable AddColumn(IEnumerable<string> names)
        {
            if (names == null)
            {
                throw new ArgumentNullException(nameof(names));
            }

            foreach (var name in names)
            {
                this.Columns.Add(name);
            }

            return this;
        }

        /// <summary>
        /// Adds the row.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>Console table.</returns>
        /// <exception cref="ArgumentNullException">values.</exception>
        /// <exception cref="Exception">
        /// Please set the columns first
        /// or
        /// The number columns in the row ({Columns.Count}) does not match the values ({values.Length}.
        /// </exception>
        public ConsoleTable AddRow(params object[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (!this.Columns.Any())
            {
                throw new Exception("Please set the columns first");
            }

            if (this.Columns.Count != values.Length)
            {
                throw new Exception(
                    $"The number columns in the row ({this.Columns.Count}) does not match the values ({values.Length}");
            }

            this.Rows.Add(values);
            return this;
        }

        /// <summary>
        /// Configures the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>Console table.</returns>
        /// <exception cref="ArgumentNullException">action.</exception>
        public ConsoleTable Configure(Action<ConsoleTableOptions> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            action(this.Options);
            return this;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this.Columns.Contains("DateOfBirth"))
            {
                int j = 0;
                for (; j < this.Columns.Count; j++)
                {
                    if (this.Columns[j].Equals("DateOfBirth"))
                    {
                        for (int i = 0; i < this.Rows.Count; i++)
                        {
                            this.Rows[i][j] = ((DateTime)this.Rows[i][j]).ToString("MM'/'dd'/'yyyy", CultureInfo.InvariantCulture);
                        }
                    }
                }
            }

            var builder = new StringBuilder();
            var columnLengths = this.ColumnLengths();
            var columnAlignment = Enumerable.Range(0, this.Columns.Count)
                .Select(this.GetNumberAlignment)
                .ToList();
            var format = Enumerable.Range(0, this.Columns.Count)
                .Select(i => " | {" + i + "," + columnAlignment[i] + columnLengths[i] + "}")
                .Aggregate((s, a) => s + a) + " |";
            var maxRowLength = Math.Max(0, this.Rows.Any() ? this.Rows.Max(row => string.Format(CultureInfo.InvariantCulture, format, row).Length) : 0);
            var columnHeaders = string.Format(CultureInfo.InvariantCulture, format, this.Columns.ToArray());
            var longestLine = Math.Max(maxRowLength, columnHeaders.Length);
            var results = this.Rows.Select(row => string.Format(CultureInfo.InvariantCulture, format, row)).ToList();
            var divider = " " + string.Join(string.Empty, Enumerable.Repeat("-", longestLine - 1)) + " ";

            builder.AppendLine(divider);
            builder.AppendLine(columnHeaders);
            foreach (var row in results)
            {
                builder.AppendLine(divider);
                builder.AppendLine(row);
            }

            builder.AppendLine(divider);
            if (this.Options.EnableCount)
            {
                builder.AppendLine(string.Empty);
                builder.AppendFormat(CultureInfo.InvariantCulture, " Count: {0}", this.Rows.Count);
            }

            return builder.ToString();
        }

        private static IEnumerable<string> GetColumns<T>(List<string> listcolumns)
        {
            return listcolumns
                .Select(column => typeof(T).GetProperty(column, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).Name);
        }

        private static object GetColumnValue<T>(object target, string column)
        {
            return typeof(T).GetProperty(column).GetValue(target);
        }

        private static IEnumerable<Type> GetColumnsType<T>()
        {
            return typeof(T).GetProperties().Select(x => x.PropertyType).ToArray();
        }

        private string ToMarkDownString(char delimiter)
        {
            var builder = new StringBuilder();
            var columnLengths = this.ColumnLengths();
            var format = this.Format(columnLengths, delimiter);
            var columnHeaders = string.Format(CultureInfo.InvariantCulture, format, this.Columns.ToArray());
            var results = this.Rows.Select(row => string.Format(CultureInfo.InvariantCulture, format, row)).ToList();
            var divider = Regex.Replace(columnHeaders, @"[^|]", "-");

            builder.AppendLine(columnHeaders);
            builder.AppendLine(divider);
            results.ForEach(row => builder.AppendLine(row));

            return builder.ToString();
        }

        private string Format(List<int> columnLengths, char delimiter = '|')
        {
            var columnAlignment = Enumerable.Range(0, this.Columns.Count)
                .Select(this.GetNumberAlignment)
                .ToList();

            var delimiterStr = delimiter == char.MinValue ? string.Empty : delimiter.ToString(CultureInfo.InvariantCulture);
            var format = (Enumerable.Range(0, this.Columns.Count)
                .Select(i => " " + delimiterStr + " {" + i + "," + columnAlignment[i] + columnLengths[i] + "}")
                .Aggregate((s, a) => s + a) + " " + delimiterStr).Trim();
            return format;
        }

        private string GetNumberAlignment(int i)
        {
            return this.ColumnTypes != null
                  && numericTypes.Contains(this.ColumnTypes[i])
                  && (this.ColumnTypes[i].Equals(typeof(int))
                    || this.ColumnTypes[i].Equals(typeof(short))
                    || this.ColumnTypes[i].Equals(typeof(decimal)))
               ? string.Empty
               : "-";
        }

        private List<int> ColumnLengths()
        {
            var columnLengths = this.Columns
                .Select((t, i) => this.Rows.Select(x => x[i])
                    .Union(new[] { this.Columns[i] })
                    .Where(x => x != null)
                    .Select(x => x.ToString().Length).Max())
                .ToList();
            return columnLengths;
        }
    }
}
