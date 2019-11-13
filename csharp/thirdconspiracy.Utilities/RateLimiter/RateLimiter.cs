using System;
using System.Collections.Generic;
using System.Linq;

namespace thirdconspiracy.Utilities.RateLimiter
{
    public class RateLimiter : IRateLimiter
    {
        #region Member Variables

        private readonly IRateLimitRepository _repo;

        #endregion Member Variables

        #region CTOR

        internal RateLimiter(IRateLimitRepository repo)
        {
            _repo = repo;
        }

        #endregion CTOR

        #region Wait

        public void TryWait(params RateLimiterConfig[] rateLimitConfigs)
        {
            var rateLimiters = rateLimitConfigs
                .Select(GetDbConfig)
                .ToList();

            try
            {
                AllocateTokens(rateLimiters);
                ConsumeTokens(rateLimiters);
            }
            catch (Exception)
            {
                ReleaseAllocations(rateLimiters);
                throw;
            }
        }

        #endregion Wait

        #region Configure

        private DbRateLimiterConfig GetDbConfig(RateLimiterConfig config)
        {
            var dbConfig = new DbRateLimiterConfig
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

        private void AllocateTokens(List<DbRateLimiterConfig> rateLimiters)
        {
            foreach (var rateLimiter in rateLimiters)
            {
                rateLimiter.AllocationTime = TryAllocateToken(rateLimiter);
            }
        }

        private DateTimeOffset TryAllocateToken(DbRateLimiterConfig config)
        {
            var startDate = DateTimeOffset.UtcNow;
            for (var current = startDate;
                current - startDate < config.RequestTimeout;
                current = DateTimeOffset.UtcNow)
            {
                var expirationDate = current.Subtract(config.OperationsInterval);
                var tokenCount = _repo.GetTokenCount(config.StorageKey, expirationDate);
                if (tokenCount >= config.MaxTokens)
                {
                    //TODO: dynamic backoff sleep or retry class (move to top)
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
                    continue;
                }

                _repo.AllocateToken(config);
                return current;
            }
            throw new RateLimitTimeoutException($"Could not get a token in '{config.RequestTimeout.Seconds}' seconds");
        }

        #endregion Allocate

        #region Consume

        private void ConsumeTokens(List<DbRateLimiterConfig> rateLimiters)
        {
            foreach (var rateLimiter in rateLimiters)
            {
                _repo.ConsumeAllocatedToken(rateLimiter);
            }
        }

        #endregion Consume

        #region Release

        private void ReleaseAllocations(List<DbRateLimiterConfig> rateLimiters)
        {
            foreach (var rateLimiter in rateLimiters)
            {
                _repo.ReleaseAllocation(rateLimiter);
            }
        }

        #endregion Release

    }
}
