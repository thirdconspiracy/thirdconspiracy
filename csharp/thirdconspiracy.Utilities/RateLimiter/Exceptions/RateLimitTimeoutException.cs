using System;

namespace thirdconspiracy.Utilities.RateLimiter.Exceptions
{
    public class RateLimitTimeoutException : Exception
    {
        public RateLimitTimeoutException(string message) : base(message)
        {}
    }
}
