using System.Collections.Generic;
using System.Linq;

namespace thirdconspiracy.Utilities.Extensions
{
    public static class EnumerableExtension
    {
        public static void Iterate<T>(this IEnumerable<T> @this)
        {
            foreach (var s in @this)
            {
                // force iteration of enumerable
            }
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }

        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return list == null || list.Count == 0;
        }

        public static bool IsNullOrEmpty<T>(this T[] a)
        {
            return a == null || a.Length == 0;
        }
    }
}
