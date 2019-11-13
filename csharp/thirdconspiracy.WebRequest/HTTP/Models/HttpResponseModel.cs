using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace thirdconspiracy.WebRequest.HTTP.Models
{
    public class HttpResponseModel : IHttpResponseModel
    {
        public int TransactionId { get; set; }
        public DateTimeOffset SentAtUtc { get; set; }
        public DateTimeOffset CompletedAtUtc { get; set; }
        public TimeSpan ResponseTime => CompletedAtUtc - SentAtUtc;

        public HttpStatusCode StatusCode { get; set; }
        public Dictionary<string, List<string>> Headers { get; } = new Dictionary<string, List<string>>();

        public byte[] ResponseBytes { get; set; }
        public HttpResponseMessage ResponseMessage { get; set; }

        private int _httpResponseMessageReadCount = 0;
        public bool IsBodyInMemory { get; set; }
        public bool IsBodyAvailableAsStream => IsBodyInMemory || _httpResponseMessageReadCount == 0;

        public string GetBodyAsString()
        {
            return GetBodyAsString(Encoding.UTF8);
        }

        public string GetBodyAsString(Encoding enc)
        {
            if (!IsBodyInMemory)
            {
                throw new NotSupportedException("GetBodyAsString is not supported in a non-buffered request");
            }

            return enc.GetString(ResponseBytes);
        }

        public byte[] GetBodyAsByteArray()
        {
            if (!IsBodyInMemory)
            {
                throw new NotSupportedException("GetBodyAsByteArray is not supported in a non-buffered request");
            }

            return ResponseBytes;
        }

        public Stream GetBodyAsStream()
        {
            if (IsBodyInMemory)
            {
                return new MemoryStream(ResponseBytes);
            }

            // When we store the HttpResponseMessage and leave the HTTP connection open,
            // the request can only be read one time.  Fail on second attempt.
            var currentReadCount = Interlocked.Increment(ref _httpResponseMessageReadCount);
            if (currentReadCount <= 1)
            {
                return ResponseMessage.Content.ReadAsStreamAsync().Result;
            }

            throw new IOException("Body is unavailable, because it has already been read from");

        }

        public void Dispose()
        {
            // Dispose of body
            ResponseMessage?.Dispose();
            ResponseBytes = null;
        }
    }
}
