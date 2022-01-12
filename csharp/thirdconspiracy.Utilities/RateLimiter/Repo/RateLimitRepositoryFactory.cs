using thirdconspiracy.Utilities.RateLimiter.Contracts;

namespace thirdconspiracy.Utilities.RateLimiter.Repo
{
	internal class RateLimitRepositoryFactory
	{
		public IRateLimitRepository Create()
		{
			return new SemaphoreRateLimiterRepository();
		}
	}
}
