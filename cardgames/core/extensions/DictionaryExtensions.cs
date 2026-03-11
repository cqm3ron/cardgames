using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.core.extensions
{
    internal static class DictionaryExtensions
    {
        public static void AddRange<TKey,TValue>(this IDictionary<TKey, TValue> target, IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            foreach (var kvp in source)
            {
                target.Add(kvp.Key, kvp.Value);
            }
        }

        public static void RemoveRange<TKey, TValue>(this IDictionary<TKey, TValue> target, IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            foreach (var kvp in source)
            {
                target.Remove(kvp.Key);
            }
        }
    }
}
