using System;
using System.Collections.Generic;

namespace thirdconspiracy.Utilities.Extensions
{
    public static class FunctionalExtensions
    {
        public static T Process<T>(this T @this, Action<T> action)
        {
            action(@this);
            return @this;
        }

        public static TResult Process<TSource, TResult>(this TSource @this, Func<TSource, TResult> func)
        {
            return func(@this);
        }

        public static T? FirstOrNull<T>(this IEnumerable<T> collection, Func<T, bool> suchThat) where T : struct
        {
            foreach (var item in collection)
            {
                if (suchThat(item))
                {
                    return item;
                }
            }
            return null;
        }

        public static IEnumerable<IReadOnlyList<T>> Batch<T>(this IEnumerable<T> collection, int batchSize)
        {
            var currentBatch = new List<T>(batchSize);
            foreach (T item in collection)
            {
                currentBatch.Add(item);
                if (currentBatch.Count == batchSize)
                {
                    yield return currentBatch;
                    currentBatch = new List<T>(batchSize);
                }
            }
            if (currentBatch.Count > 0)
            {
                yield return currentBatch;
            }
        }
    }
}
