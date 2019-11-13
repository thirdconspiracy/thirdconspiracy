using System;
using System.Collections.Generic;
using System.IO;

namespace thirdconspiracy.WebRequest.HTTP.Models
{
    public interface IHttpRequestModel
    {
        HttpAction Method { get; }
        Uri FullUri { get; }

        Dictionary<string, List<string>> Headers { get; }
        string ContentType { get; set; }
        string Authorization { get; set; }
        string Accept { get; set; }

        TimeSpan? ResponseTimeoutOverride { get; set; }

        //Body Reading
        string GetBodyAsString();
        byte[] GetBodyAsByteArray();
        Stream GetBodyAsStream();

        //Body Sets
        void SetBody(string body);
        void SetBody(Stream body);
        void SetBody(byte[] body);
        void AddMultipartContent(string name, Stream body, string filename = null, bool isAttachment = false);
        void AddMultipartContent(string name, byte[] body, string filename = null, bool isAttachment = false);
    }
}
