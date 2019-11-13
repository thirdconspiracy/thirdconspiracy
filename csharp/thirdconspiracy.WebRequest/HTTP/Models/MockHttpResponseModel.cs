using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace thirdconspiracy.WebRequest.HTTP.Models
{
    public class MockHttpResponseModel : IHttpResponseModel
    {
        public int TransactionId { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public Dictionary<string, List<string>> Headers { get; set; }

        public bool IsBodyInMemory { get; private set; }
        public byte[] BodyBytes { get; private set; }
        public Stream BodyStream { get; private set; }

        public void SetBody(string body)
        {
            BodyBytes = Encoding.UTF8.GetBytes(body);
            IsBodyInMemory = true;
        }
        public void SetBody(byte[] body)
        {
            BodyBytes = body;
            IsBodyInMemory = true;
        }

        public void SetBody(Stream body)
        {
            BodyStream = body;
            IsBodyInMemory = false;
        }

        public string GetBodyAsString()
        {
            return IsBodyInMemory
                ? GetBodyAsString(Encoding.UTF8)
                : new StreamReader(BodyStream).ReadToEnd();
        }

        public string GetBodyAsString(Encoding enc)
        {
            return enc.GetString(BodyBytes);
        }

        public byte[] GetBodyAsByteArray()
        {
            return IsBodyInMemory
                ? BodyBytes
                : Encoding.UTF8.GetBytes(new StreamReader(BodyStream).ReadToEnd());
        }

        public Stream GetBodyAsStream()
        {
            return !IsBodyInMemory
                ? BodyStream
                : new MemoryStream(BodyBytes);
        }


        public DateTimeOffset SentAtUtc { get; set; }
        public DateTimeOffset CompletedAtUtc { get; set; }
        public TimeSpan ResponseTime { get; set; }
    }
}
