using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace thirdconspiracy.WebRequest.HTTP.Models
{
    public interface IHttpResponseModel
    {
        int TransactionId { get; }
        HttpStatusCode StatusCode { get; }
        Dictionary<string, List<string>> Headers { get; }

        //Body Reading
        bool IsBodyInMemory { get; }
        string GetBodyAsString();
        byte[] GetBodyAsByteArray();
        Stream GetBodyAsStream();

        //Diagnostic Info
        DateTimeOffset SentAtUtc { get; }
        DateTimeOffset CompletedAtUtc { get; }
        TimeSpan ResponseTime { get; }
    }
}
