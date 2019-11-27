using System;
using System.Collections.Generic;
using System.Linq;
using FileCabinetApp.Enums;
using FileCabinetApp.Extensions;
using FileCabinetApp.Services;

namespace FileCabinetApp.Memoizers
{
    /// <summary>
    /// Memoizer class for FileCabinetMemoryService.SelectRecords.
    /// </summary>
    public class Memoizer
    {
        private static readonly Lazy<Memoizer> Lazy = new Lazy<Memoizer>(() => new Memoizer());
        private static IFileCabinetService service;

        private Memoizer()
        {
            this.MemoizerDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        }

        /// <summary>
        /// Gets the memoizer dictionary.
        /// </summary>
        /// <value>
        /// The memoizer dictionary.
        /// </value>
        public Dictionary<string, List<FileCabinetRecord>> MemoizerDictionary
        { get; internal set; }

        /// <summary>
        /// Gets the memoizer.
        /// </summary>
        /// <param name="fileCabinetService">The file cabinet service.</param>
        /// <returns>Memoizer object.</returns>
        public static Memoizer GetMemoizer(IFileCabinetService fileCabinetService)
        {
            service = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
            return Lazy.Value;
        }

        /// <summary>
        /// Selects the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="keyValuePairs">The key value pairs.</param>
        /// <param name="condition">The condition.</param>
        /// <returns>Select records.</returns>
        /// <exception cref="ArgumentNullException">keyValuePairs.</exception>
        public IEnumerable<FileCabinetRecord> Select(string parameters, List<KeyValuePair<string, string>> keyValuePairs, SearchCondition condition)
        {
            if (keyValuePairs == null)
            {
                throw new ArgumentNullException(nameof(keyValuePairs));
            }

            List<FileCabinetRecord> results;
            var tuple = Tuple.Create(keyValuePairs, condition);
            if (!this.MemoizerDictionary.Keys.Contains(parameters))
            {
                results = service.GetRecords().Where(keyValuePairs, condition).ToList();
                this.MemoizerDictionary.Add(parameters, results);
            }
            else
            {
                results = this.MemoizerDictionary.GetValueOrDefault(parameters);
            }

            return results;
        }
    }
}