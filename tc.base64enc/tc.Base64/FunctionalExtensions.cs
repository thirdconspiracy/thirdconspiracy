using System;
using System.Collections.Generic;

namespace tc.Base64
{
    public static class FunctionalExtensions
    {
        public static TResult Process<TSource, TResult>(this TSource @this, Func<TSource, TResult> func)
        {
            return func(@this);
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
                    currentBatch.Clear();
                }
            }
            if (currentBatch.Count > 0)
            {
                yield return currentBatch;
            }
        }

    }
}
