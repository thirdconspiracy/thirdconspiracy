using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using thirdconspiracy.Utilities.RateLimiter.Contracts;
using thirdconspiracy.Utilities.RateLimiter.Models;

namespace thirdconspiracy.Utilities.RateLimiter.Repo
{
	internal class SemaphoreRateLimiterRepository : IRateLimitRepository
	{
		private readonly Dictionary<string, SemaphoreSlim> _semaphores = new Dictionary<string, SemaphoreSlim>();

		#region CTOR

		public SemaphoreRateLimiterRepository()
		{
		}

		#endregion CTOR

		#region Publics

		public async Task<int> GetTokenCount(RepositoryConfig repoConfig, DateTimeOffset expirationDate)
		{
			var slim = GetSemaphoreInstance(repoConfig);
			return await Task.FromResult(repoConfig.MaxTokens - slim.CurrentCount);
		}

		public async Task AllocateToken(RepositoryConfig repoConfig)
		{
			//Hack: we're consuming a token even if we might need to roll back later
			var slim = GetSemaphoreInstance(repoConfig);
			await Consume(slim, repoConfig);
		}

		public async Task ReleaseAllocation(RepositoryConfig repoConfig)
		{
			//Do Nothing for now, test code only
			await Task.CompletedTask;
		}

		public async Task ConsumeAllocatedToken(RepositoryConfig repoConfig)
		{
			//Do Nothing for now, test code only
			await Task.CompletedTask;
		}

		#endregion Publics

		#region Privates

		private SemaphoreSlim GetSemaphoreInstance(RepositoryConfig repoConfig)
		{
			//TODO: Thread safe only add if not exists
			_semaphores.TryAdd(repoConfig.StorageKey, new SemaphoreSlim(0, repoConfig.MaxTokens));
			return _semaphores[repoConfig.StorageKey];
		}

		private async Task Consume(SemaphoreSlim slim, RepositoryConfig config)
		{
			await slim.WaitAsync(config.RequestTimeout);
			//Note: Do not await here
			ReleaseAfterTTL(slim, config.OperationsInterval);
		}

		private static async Task ReleaseAfterTTL(SemaphoreSlim slim, TimeSpan ttl)
		{
			//expireDateTime = config.AllocationTime.Add(config.OperationsInterval);
			//ttl = expireDateTime - DateTimeOffset.Now;
			await Task.Delay(ttl);
			slim.Release();
		}

		#endregion Privates

	}
}
