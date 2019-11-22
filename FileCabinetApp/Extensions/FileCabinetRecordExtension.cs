using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FileCabinetApp.Enums;

namespace FileCabinetApp.Extensions
{
    /// <summary>
    /// Extension for FileCabinetRecords to find records whith some properties.
    /// </summary>
    public static class FileCabinetRecordExtension
    {
        private static readonly Type RecordType = typeof(FileCabinetRecord);
        private static readonly ParameterExpression Record = Expression.Parameter(typeof(FileCabinetRecord), "record");
        private static readonly Dictionary<string, MemberInfo> Properties = new Dictionary<string, MemberInfo>
        {
            { "ID", RecordType.GetProperty("Id") },
            { "FIRSTNAME", RecordType.GetProperty("FirstName") },
            { "LASTNAME", RecordType.GetProperty("LastName") },
            { "DATEOFBIRTH", RecordType.GetProperty("DateOfBirth") },
            { "GENDER", RecordType.GetProperty("Gender") },
            { "OFFICE", RecordType.GetProperty("Office") },
            { "SALARY", RecordType.GetProperty("Salary") },
        };

        /// <summary>
        /// Wheres the specified key value pairs.
        /// </summary>
        /// <param name="records">The records.</param>
        /// <param name="keyValuePairs">The key value pairs.</param>
        /// <param name="condition">The condition.</param>
        /// <returns>Records that contains some parameters.</returns>
        public static IEnumerable<FileCabinetRecord> Where(
            this IEnumerable<FileCabinetRecord> records,
            List<KeyValuePair<string, string>> keyValuePairs,
            SearchCondition condition)
        {
            var selectExpression = Expression
                .Lambda<Func<FileCabinetRecord, FileCabinetRecord>>(Record, Record);
            Func<FileCabinetRecord, FileCabinetRecord> delegateForSelect = selectExpression.Compile();

            BinaryExpression whereBinaryExpression = IsMatch(keyValuePairs, condition);
            if (whereBinaryExpression != null)
            {
                var whereExpression = Expression.Lambda<Func<FileCabinetRecord, bool>>(whereBinaryExpression, Record);
                Func<FileCabinetRecord, bool> delegateForWhere = whereExpression.Compile();

                return records.Select(delegateForSelect).Where(delegateForWhere);
            }
            else
            {
                return records.Select(delegateForSelect);
            }
        }

        private static BinaryExpression IsMatch(List<KeyValuePair<string, string>> keyValuePairs, SearchCondition condition)
        {
            if (keyValuePairs == null)
            {
                return default(BinaryExpression);
            }

            var filterExpressions = new List<BinaryExpression>();
            foreach (KeyValuePair<string, string> pair in keyValuePairs)
            {
                var kv = pair;
                switch (kv.Key.ToUpperInvariant())
                {
                    case "ID":
                        if (int.TryParse(kv.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int id))
                        {
                            filterExpressions.Add(MakeBinary(kv.Key, id, Record));
                        }
                        else
                        {
                            throw new ArgumentException($"Id '{kv.Value}' is not valid.");
                        }

                        break;
                    case "FIRSTNAME":
                    case "LASTNAME":
                        filterExpressions.Add(MakeBinary(kv.Key, kv.Value, Record));
                        break;
                    case "DATEOFBIRTH":
                        if (DateTime.TryParse(kv.Value, out DateTime dateofbirth))
                        {
                            filterExpressions.Add(MakeBinary(kv.Key, dateofbirth, Record));
                        }
                        else
                        {
                            throw new ArgumentException($"Date of birth '{kv.Value}' is not valid.");
                        }

                        break;
                    default:
                        throw new InvalidOperationException(
                            $"The {kv.Key} isn't a search parameter name. Only 'Id', 'FirstName', 'LastName' or 'DateOfBirth'.");
                }
            }

            BinaryExpression filter = default(BinaryExpression);
            if (condition.Equals(SearchCondition.Or))
            {
                filter = CombinePredicates(filterExpressions, Expression.Or);
            }
            else
            {
                filter = CombinePredicates(filterExpressions, Expression.And);
            }

            return filter;
        }

        private static BinaryExpression CombinePredicates(
            IList<BinaryExpression> predicateExpressions, Func<Expression, Expression, BinaryExpression> logicalFunction)
        {
            if (predicateExpressions == null)
            {
                throw new ArgumentNullException(nameof(predicateExpressions));
            }

            BinaryExpression filter = default(BinaryExpression);

            if (predicateExpressions.Count > 0)
            {
                filter = predicateExpressions[0];
                for (int i = 1; i < predicateExpressions.Count; i++)
                {
                    filter = logicalFunction(filter, predicateExpressions[i]);
                }
            }

            return filter;
        }

        private static BinaryExpression MakeBinary<T>(string propName, T value, ParameterExpression arg)
        {
            MemberInfo property = Properties[propName];
            MemberExpression propertyExpr = Expression.MakeMemberAccess(arg, property);
            BinaryExpression equals = Expression.Equal(propertyExpr, Expression.Constant(value, value.GetType()));

            return equals;
        }
    }
}