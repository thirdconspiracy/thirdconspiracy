using System;
using System.Threading.Tasks;
using thirdconspiracy.Utilities.RateLimiter.Models;

namespace thirdconspiracy.Utilities.RateLimiter.Contracts
{
	internal interface IRateLimitRepository
	{
		/// <summary>
		/// Gets the token count of both allocated and consumed tokens
		/// </summary>
		/// <param name="storageKey"></param>
		/// <param name="expirationDate"></param>
		/// <returns></returns>
		Task<int> GetTokenCount(RepositoryConfig repoConfig, DateTimeOffset expirationDate);

		/// <summary>
		/// Creates a temporary token to prevent consumption of a token that will never be used
		/// </summary>
		/// <param name="rateLimiter"></param>
		Task AllocateToken(RepositoryConfig repoConfig);

		/// <summary>
		/// Releases the temporary token
		/// </summary>
		/// <param name="rateLimiter"></param>
		Task ReleaseAllocation(RepositoryConfig repoConfig);

		/// <summary>
		/// Moves temporary token from allocated to consumed
		/// </summary>
		/// <param name="rateLimiter"></param>
		Task ConsumeAllocatedToken(RepositoryConfig repoConfig);
	}
}
