using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using thirdconspiracy.Utilities.Utilities;
using thirdconspiracy.WebRequest.HTTP.Utilities;

namespace thirdconspiracy.WebRequest.HTTP.Models
{
    public class HttpRequestModel : IHttpRequestModel
    {
        #region CTOR

        private HttpRequestModel(HttpAction method, Uri rootUri)
        {
            Method = method;
            RootUri = rootUri;
        }

        public static IHttpRequestModel CreateSimpleRequest(HttpAction method, Uri rootUri)
        {
            return new HttpRequestModel(method, rootUri)
            {
                UseMultipart = false
            };
        }

        public static IHttpRequestModel CreateMultipartRequest(HttpAction method, Uri rootUri, string boundary = null)
        {
            return new HttpRequestModel(method, rootUri)
            {
                UseMultipart = true,
                Body = string.IsNullOrWhiteSpace(boundary)
                    ? new MultipartFormDataContent()
                    : new MultipartFormDataContent(boundary)
            };
        }

        #endregion CTOR

        public TimeSpan? ResponseTimeoutOverride { get; set; }
        public bool ShouldBufferResponseBody { get; set; }
        private bool UseMultipart { get; set; }

        #region API

        public HttpAction Method { get; set; }
        public Uri FullUri
            => UriUtilities.GetFullUri(RootUri, RelativePath, QueryStringParameters);

        public Uri RootUri { get; set; }
        public Uri RelativePath { get; set; }
        public Dictionary<string, string> QueryStringParameters { get; set; }

        #endregion API

        #region Headers

        public Dictionary<string, List<string>> Headers { get; }

        public string ContentType
        {
            get => GetSingleHeaderVal("Content-Type");
            set => SetSingleHeaderVal("Content-Type", value);
        }

        public string Authorization
        {
            get => GetSingleHeaderVal("Authorization");
            set => SetSingleHeaderVal("Authorization", value);
        }

        public string Accept
        {
            get => GetSingleHeaderVal("Accept");
            set => SetSingleHeaderVal("Accept", value);
        }

        private string GetSingleHeaderVal(string headerKey)
        {
            if (Headers.TryGetValue(headerKey, out List<string> val))
            {
                if (val.Count > 0)
                {
                    return val[0];
                }
            }

            return null;
        }

        private void SetSingleHeaderVal(string headerKey, string val)
        {
            Headers[headerKey] = new List<string> { val };
        }

        #endregion Headers

        #region Body Reads

        public HttpContent Body { get; set; }

        public string GetBodyAsString() => Body?.ReadAsStringAsync().Result;
        public byte[] GetBodyAsByteArray() => Body?.ReadAsByteArrayAsync().Result;
        public Stream GetBodyAsStream() => Body?.ReadAsStreamAsync().Result;

        #endregion Body Reads

        #region Body Sets

        public void SetBody(string body)
        {
            Body = new StringContent(body);
        }
        public void SetBody(Stream body)
        {
            Body = new StreamContent(body);
        }
        public void SetBody(byte[] body)
        {
            Body = new ByteArrayContent(body);
        }

        public void AddMultipartContent(string name, Stream body, string filename = null, bool isAttachment = false)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            var httpContent = new StreamContent(body);
            AddMultipartContent(name, filename, isAttachment, httpContent);
        }

        public void AddMultipartContent(string name, byte[] body, string filename = null, bool isAttachment = false)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            var httpContent = new ByteArrayContent(body);
            AddMultipartContent(name, filename, isAttachment, httpContent);
        }

        private void AddMultipartContent(string name, string filename, bool isAttachment, HttpContent httpContent)
        {
            if (!UseMultipart)
            {
                throw new ArgumentException("Please use multipart constructor");
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"{nameof(name)} cannot be blank");
            }

            if (isAttachment)
            {
                httpContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            }

            var formContent = Body as MultipartFormDataContent
                              ?? throw new ArgumentException("Please use multipart constructor");

            if (string.IsNullOrWhiteSpace(filename))
            {
                formContent.Add(httpContent, name);
            }
            else
            {
                httpContent.Headers.ContentDisposition.FileName = filename;
                formContent.Add(httpContent, name, filename);
            }

            Body = formContent;
            this.ValidateBody();
        }

        #endregion Body Sets

    }
}
