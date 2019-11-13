namespace thirdconspiracy.Utilities.RateLimiter
{
    public interface IRateLimiter
    {
        void TryWait(params RateLimiterConfig[] config);
    }
}
