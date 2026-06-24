using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.core.extension
{
    internal static class ListExtensions
    {
        public static List<T> Empty<T>(this List<T> collection) // define alias for Clear() that also returns the removed items.
        {
            List<T> collectionBeforeClear = [];
            foreach (T item in collection)
            {
                collectionBeforeClear.Add(item);
            }
            collection.Clear();
            return collectionBeforeClear;
        }
    }
}
