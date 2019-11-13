using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using thirdconspiracy.Utilities.Utilities;
using thirdconspiracy.WebRequest.HTTP.Models;

namespace thirdconspiracy.WebRequest.HTTP.Utilities
{
    public static class HttpRequestMessageBuilder
    {
        public static HttpRequestMessage BuildHttpRequestMessage(HttpRequestModel reqModel)
        {
            var msg = new HttpRequestMessage
            {
                Method = GetHttpMethodFromAction(reqModel.Method),
                RequestUri = UriUtilities.GetFullUri(reqModel.RootUri, reqModel.RelativePath, reqModel.QueryStringParameters)
            };

            if (reqModel.Body != null)
            {
                //Copy the stream contents.  This is done to keep the request object
                //Re-usable in a retry scenario.  (The send async used in the client will
                //not clean this up correctly if the request is cancelled via cancellation token)

                //The tradeoff here is bloated memory, we are taking the risk, with the assumption that
                //Request bodies will not be very large, generally (hopefully)
                var ms = new MemoryStream();
                reqModel.Body.CopyToAsync(ms).Wait();

                //Memory stream position needs to be reset for the send to function
                ms.Position = 0;

                msg.Content = new StreamContent(ms);
            }

            if (!string.IsNullOrWhiteSpace(reqModel.Authorization))
            {
                msg.Headers.Add("Authorization", reqModel.Authorization);
            }

            if (!string.IsNullOrWhiteSpace(reqModel.Accept))
            {
                msg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(reqModel.Accept));
            }

            if (msg.Content != null && !string.IsNullOrWhiteSpace(reqModel.ContentType))
            {
                msg.Content.Headers.ContentType = new MediaTypeHeaderValue(reqModel.ContentType);
            }

            foreach (var requestHeader in reqModel.Headers)
            {
                //These headers are set above.
                if (requestHeader.Key.Equals("Authorization", StringComparison.InvariantCultureIgnoreCase)
                    || requestHeader.Key.Equals("Accept", StringComparison.InvariantCultureIgnoreCase)
                    || requestHeader.Key.Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                msg.Headers.Add(requestHeader.Key, requestHeader.Value);
            }

            return msg;
        }

        internal static HttpMethod GetHttpMethodFromAction(HttpAction action)
        {
            switch (action)
            {
                case HttpAction.GET:
                    return HttpMethod.Get;
                case HttpAction.PUT:
                    return HttpMethod.Put;
                case HttpAction.POST:
                    return HttpMethod.Post;
                case HttpAction.DELETE:
                    return HttpMethod.Delete;
                default:
                    throw new NotImplementedException("This HTTP action has not been implemented");
            }
        }
    }
}
