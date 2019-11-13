using System;

namespace thirdconspiracy.Utilities.RateLimiter
{
    internal interface IRateLimitRepository
    {
        /// <summary>
        /// Gets the token count of both allocated and consumed tokens
        /// </summary>
        /// <param name="storageKey"></param>
        /// <param name="expirationDate"></param>
        /// <returns></returns>
        int GetTokenCount(string storageKey, DateTimeOffset expirationDate);

        /// <summary>
        /// Creates a temporary token to prevent consumption of a token that will never be used
        /// </summary>
        /// <param name="rateLimiter"></param>
        void AllocateToken(DbRateLimiterConfig rateLimiter);

        /// <summary>
        /// Releases the temporary token
        /// </summary>
        /// <param name="rateLimiter"></param>
        void ReleaseAllocation(DbRateLimiterConfig rateLimiter);

        /// <summary>
        /// Moves temporary token from allocated to consumed
        /// </summary>
        /// <param name="rateLimiter"></param>
        void ConsumeAllocatedToken(DbRateLimiterConfig rateLimiter);
    }
}
