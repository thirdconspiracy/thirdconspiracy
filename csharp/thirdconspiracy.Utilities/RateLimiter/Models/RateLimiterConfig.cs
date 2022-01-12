using System;

namespace thirdconspiracy.Utilities.RateLimiter.Models
{
    public class RateLimiterConfig
    {
        public string Key { get; set; }
        public RateLimiterType RateLimiterType { get; set; }
        public int IntervalCount { get; set; }
        public TimeSpan IntervalRange { get; set; }
        public TimeSpan RequestTimeout { get; set; }
    }
}
