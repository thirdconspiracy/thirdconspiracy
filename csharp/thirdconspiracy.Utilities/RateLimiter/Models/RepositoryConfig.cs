using System;

namespace thirdconspiracy.Utilities.RateLimiter.Models
{
	internal class RepositoryConfig
	{
		internal string StorageKey { get; set; }
		internal int MaxTokens { get; set; }
		internal TimeSpan OperationsInterval { get; set; }
		internal TimeSpan RequestTimeout { get; set; }
		internal DateTimeOffset AllocationTime { get; set; }
	}
}
