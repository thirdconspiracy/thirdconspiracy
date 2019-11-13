using System;

namespace thirdconspiracy.Utilities.RateLimiter
{
    public class RateLimitTimeoutException : Exception
    {
        public RateLimitTimeoutException(string message) : base(message)
        {}
    }
}
