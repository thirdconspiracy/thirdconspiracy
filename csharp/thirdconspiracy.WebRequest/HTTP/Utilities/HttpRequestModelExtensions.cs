using System;
using System.Text;
using thirdconspiracy.WebRequest.HTTP.Models;

namespace thirdconspiracy.WebRequest.HTTP.Utilities
{
    public static class HttpRequestModelExtensions
    {
        public static void AddBasicAuth(this IHttpRequestModel request, string username, string password)
        {
            var asciiBytes = Encoding.ASCII.GetBytes(username + ":" + password);
            request.Authorization = "Basic " + Convert.ToBase64String(asciiBytes);
        }

        #region Validation

        public static void ValidateBody(this HttpRequestModel request)
        {
            if (request.Body == null)
            {
                return;
            }

            if (request.Method != HttpAction.GET && request.Method != HttpAction.DELETE)
            {
                return;
            }

            throw new ArgumentException($"{request.Method} requests do not support payloads");
        }

        #endregion Validation

    }
}
