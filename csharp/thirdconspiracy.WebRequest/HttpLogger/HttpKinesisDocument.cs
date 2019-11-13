using System;
using System.Collections.Generic;
using System.Text;

namespace thirdconspiracy.WebRequest.HttpLogger
{
    public class HttpKinesisDocument
    {
        public DateTimeOffset? RequestSentAtUtc { get; set; }
        public DateTimeOffset? ResponseReceivedAtUtc { get; set; }
        public long? TotalElapsedTimeMs { get; set; }

        public Uri Url { get; set; }
        public string Method { get; set; }
        public int StatusCode { get; set; }
        public string TransactionId { get; set; }

        public List<string> RequestHeaders { get; set; }
        public string RequestBody { get; set; }

        public List<string> ResponseHeaders { get; set; }
        public string ResponseBody { get; set; }

        public Exception Exception { get; set; }
    }
}
