using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using thirdconspiracy.Utilities.RateLimiter.Contracts;
using thirdconspiracy.Utilities.RateLimiter.Exceptions;
using thirdconspiracy.Utilities.RateLimiter.Models;
using thirdconspiracy.Utilities.RateLimiter.Repo;

namespace thirdconspiracy.Utilities.RateLimiter
{
    public class PollingRateLimiter : IRateLimiter
    {
        #region Member Variables

        private readonly IRateLimitRepository _repo;

        #endregion Member Variables

        #region CTOR

        internal PollingRateLimiter(RateLimitRepositoryFactory factory)
        {
            _repo = factory.Create();
        }

        #endregion CTOR

        #region Wait

        public async Task TryWait(params RateLimiterConfig[] rateLimitConfigs)
        {
            var rateLimiters = rateLimitConfigs
                .Select(GetRepoConfigs)
                .ToList();

            try
            {
                //Allocated tokens are either consumed or manually released
                foreach (var rateLimiter in rateLimiters)
                {
                    rateLimiter.AllocationTime = await TryAllocateToken(rateLimiter);
                }

                //Consumed tokens only expire after TTL
                foreach (var rateLimiter in rateLimiters)
                {
                    await _repo.ConsumeAllocatedToken(rateLimiter);
                }
            }
            catch (Exception)
            {
                await ReleaseAllocations(rateLimiters);
                throw;
            }
        }

        #endregion Wait

        #region Configure

        private RepositoryConfig GetRepoConfigs(RateLimiterConfig config)
        {
            var dbConfig = new RepositoryConfig
            {
                RequestTimeout = config.RequestTimeout
            };
            switch (config.RateLimiterType)
            {
                case RateLimiterType.Burst:
                    dbConfig.MaxTokens = config.IntervalCount;
                    dbConfig.OperationsInterval = config.IntervalRange;
                    break;
                case RateLimiterType.Interval:
                    dbConfig.MaxTokens = 1;
                    dbConfig.OperationsInterval = TimeSpanDivision(config.IntervalRange, config.IntervalCount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown RateLimiter type of {config.RateLimiterType:G}");
            }

            dbConfig.StorageKey = GetStorageKey(
                config.Key,
                dbConfig.MaxTokens,
                dbConfig.OperationsInterval.Milliseconds);

            return dbConfig;
        }

        private string GetStorageKey(string key, int intervalCount, int intervalMSeconds)
        {
            return $"RateLimit_{key}_{intervalCount}_{intervalMSeconds}";
        }

        private TimeSpan TimeSpanDivision(TimeSpan dividend, int divisor)
        {
            var mSeconds = Math.DivRem(dividend.Milliseconds, divisor, out _);
            return TimeSpan.FromMilliseconds(mSeconds);
        }

        #endregion Configure

        #region Allocate

        private async Task<DateTimeOffset> TryAllocateToken(RepositoryConfig config)
        {
            var watch = new Stopwatch();

            watch.Start();
            do
            {
                var expirationDate = DateTimeOffset.Now.Subtract(config.OperationsInterval);
                var tokenCount = await _repo.GetTokenCount(config, expirationDate);
                if (tokenCount >= config.MaxTokens)
                {
                    //TODO: dynamic backoff sleep or retry class (move to top)
                    await Task.Delay(TimeSpan.FromMilliseconds(500));
                    continue;
                }
                await _repo.AllocateToken(config);
                return DateTimeOffset.Now;
            }
            while (watch.Elapsed < config.RequestTimeout);

            throw new RateLimitTimeoutException($"Could not get a token in '{config.RequestTimeout.Seconds}' seconds");
        }

        #endregion Allocate
        #region Release

        private async Task ReleaseAllocations(List<RepositoryConfig> rateLimiters)
        {
            foreach (var rateLimiter in rateLimiters)
            {
                await _repo.ReleaseAllocation(rateLimiter);
            }
        }

        #endregion Release

    }
}
