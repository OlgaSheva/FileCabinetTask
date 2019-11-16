using System;
using System.Collections.Generic;
using System.Linq;
using FileCabinetApp.Enums;
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
            this.MemoizerDictionary = new Dictionary<Tuple<List<KeyValuePair<string, string>>, SearchCondition>, List<FileCabinetRecord>>();
        }

        /// <summary>
        /// Gets the memoizer dictionary.
        /// </summary>
        /// <value>
        /// The memoizer dictionary.
        /// </value>
        public Dictionary<Tuple<List<KeyValuePair<string, string>>, SearchCondition>, List<FileCabinetRecord>> MemoizerDictionary
        { get; internal set; }

        /// <summary>
        /// Gets the memoizer.
        /// </summary>
        /// <param name="fileCabinetService">The file cabinet service.</param>
        /// <returns>Memoizer object.</returns>
        public static Memoizer GetMemoizer(IFileCabinetService fileCabinetService)
        {
            service = fileCabinetService;
            return Lazy.Value;
        }

        /// <summary>
        /// Selects the specified key value pairs.
        /// </summary>
        /// <param name="keyValuePairs">The key value pairs.</param>
        /// <param name="condition">The condition.</param>
        /// <returns>Select records.</returns>
        public IEnumerable<FileCabinetRecord> Select(List<KeyValuePair<string, string>> keyValuePairs, SearchCondition condition)
        {
            List<FileCabinetRecord> results;
            var tuple = Tuple.Create(keyValuePairs, condition);
            if (!this.MemoizerDictionary.TryGetValue(tuple, out results))
            {
                results = service.SelectRecords(keyValuePairs, condition).ToList();
                this.MemoizerDictionary.Add(tuple, results);
            }

            return results;
        }
    }
}