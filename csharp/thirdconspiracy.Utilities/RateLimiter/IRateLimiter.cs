namespace thirdconspiracy.Utilities.RateLimiter
{
    public interface IRateLimiter
    {
        void TryWait(params RateLimiterConfig[] config);
    }

    /*
    public class RateLimiterRedis : IRateLimiter
{
   private readonly TimeSpan _ttl;
   private readonly int _maxRequests;
   private readonly string _muxKey;
   private readonly Redis _redis;
   
   public RateLimiter(RateLimiterOptions options, Redis redis)
   {
      _redis = redis;
      _ttl = options.AllowBurst
         ? option.Window
         : options.Window.Divide(option.Requests);
      _maxRequests = options.AllowBurst
         ? option.Requests
         : 1;
      
      
      var suffix = options.AllEnvironments ? string.Empty : Congit.Get("env");
      var name = options.Name;
      var ttlStr = _ttl.TotalSeconds();
      _muxKey = $"MUX{name}{suffix}_{ttlStr}_{_maxRequests}"
   }
   
   public async Task<bool> TryWait(TimeSpan timeout)
   {
      var cts = new CancellationTokenSource(timeout);
      return await Wait(Inserted());
   }
   
   private static async Task<bool> Wait(TimeSpan timeout, Func<Task<bool>> function, CancellationToken token)
   {
      While (true)
      {
         if(token.IsCancellationRequested)
            return false;
         if (await Inserted())
            return true;
         await Task.Delay(200);
      }
   }

   private async Task<bool> Inserted()
   {
      for (var index = 1; index <= _maxRequests; index ++)
      {
         var currentKey = $"{_muxKey}_{index}";
         var inserted = _redis.StringSet(currentKey, "true", _ttl.FromSeconds, When.NotExists, CommandFlags.None);
         if (inserted)
            return true;
      }
      return false;
   }
}
    //1. var requestId = Guid.NewGuid();
    //2. var tokens = new List<KVP<limiter, string>>();
    //3. foreach (limiter in limiters)
    //      var token = limiter.TryWait(limiter, requestId);
    //      if (!string.IsNullOrWhitespace(token)) tokens.Add(KVP<limiter, token>);
    //      else { DeleteTokens(tokens, requestId) }
    */

}
