using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Http.Internal;

namespace thirdconspiracy.Utilities.Extensions
{
	public static class FunctionalExtensions
	{
		public static Task AsyncParallelForEach<TInput>(this IEnumerable<TInput> source, Func<TInput, Task> body, int maxDegreeOfParallelism = DataflowBlockOptions.Unbounded, TaskScheduler scheduler = null)
		{
			var options = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism };

			if (scheduler != null)
				options.TaskScheduler = scheduler;

			var block = new ActionBlock<TInput>(body, options);
			foreach (var item in source)
				block.Post(item);
			block.Complete();
			return block.Completion;
		}

		public static async Task<ICollection<TResult>> SelectAsync<TInput, TResult>(this IEnumerable<TInput> source,
			Func<TInput, Task<TResult>> func)
		{
			var slim = new SemaphoreSlim(5);
			var tasks = source
				.Select(async item =>
				{
					await slim.WaitAsync();
					try
					{
						return await func(item);
					}
					finally
					{
						slim.Release();
					}
				});
			var result = await Task.WhenAll(tasks);
			return result;
		}

		public static Task ForEachAsync<TSource, TResult>(
			this IEnumerable<TSource> source,
			Func<TSource, Task<TResult>> taskSelector,
			Action<TSource, TResult> resultProcessor)
		{
			var oneAtATime = new SemaphoreSlim(initialCount: 1, maxCount: 1);
			var tasks = source.Select(item => ProcessAsync(item, taskSelector, resultProcessor, oneAtATime));
			return Task.WhenAll(tasks);
		}

		private static async Task ProcessAsync<TSource, TResult>(
			TSource item,
			Func<TSource, Task<TResult>> taskSelector,
			Action<TSource, TResult> resultProcessor,
			SemaphoreSlim oneAtATime)
		{
			var result = await taskSelector(item);
			await oneAtATime.WaitAsync();
			try
			{
				resultProcessor(item, result);
			}
			finally
			{
				oneAtATime.Release();
			}
		}

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
