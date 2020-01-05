using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace GR.Core.Extensions
{
    public static class MemoryCacheExtensions
    {
        /// <summary>
        /// Get entries
        /// </summary>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static IEnumerable<ICacheEntry> GetEntries(this IMemoryCache cache)
        {
            // Get the empty definition for the EntriesCollection
            var cacheEntriesCollectionDefinition = typeof(MemoryCache).GetProperty("EntriesCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (cacheEntriesCollectionDefinition == null) return default;

            // Populate the definition with your IMemoryCache instance.  
            // It needs to be cast as a dynamic, otherwise you can't
            // loop through it due to it being a collection of objects.
            var cacheEntriesCollection = cacheEntriesCollectionDefinition.GetValue(cache) as dynamic;

            // Define a new list we'll be adding the cache entries too
            var cacheCollectionValues = new List<ICacheEntry>();

            foreach (var cacheItem in cacheEntriesCollection)
            {
                // Get the "Value" from the key/value pair which contains the cache entry   
                ICacheEntry cacheItemValue = cacheItem.GetType().GetProperty("Value").GetValue(cacheItem, null);

                // Add the cache entry to the list
                cacheCollectionValues.Add(cacheItemValue);
            }

            return cacheCollectionValues;
        }
    }
}
