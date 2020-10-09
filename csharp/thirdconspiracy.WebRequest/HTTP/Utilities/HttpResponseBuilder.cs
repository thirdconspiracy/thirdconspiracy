using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using thirdconspiracy.WebRequest.HTTP.Models;

namespace thirdconspiracy.WebRequest.HTTP.Utilities
{
    public static class HttpResponseBuilder
    {
        public static async Task<HttpResponseModel> BuildHttpResponseModel(int transactionId, DateTimeOffset sentAtUtc, DateTimeOffset completedAtUtc, bool saveInMemory, HttpResponseMessage respMsg)
        {
            var response = new HttpResponseModel
            {
                TransactionId = transactionId,
                SentAtUtc = sentAtUtc,
                CompletedAtUtc = completedAtUtc,
                StatusCode = respMsg.StatusCode,
            };

            AddHeadersToResponseModel(response, respMsg.Headers);


            if (saveInMemory)
            {
                response.IsBodyInMemory = true;
                response.ResponseBytes = await respMsg.Content.ReadAsByteArrayAsync();
                respMsg.Dispose();
            }
            else
            {
                //If we are not loading the response into memory, then set a reference to the underlying
                //http message -- note this does not dispose the connection :( it should be used carefully.
                response.IsBodyInMemory = false;
                response.ResponseMessage = respMsg;
            }

            return response;
        }

        private static void AddHeadersToResponseModel(HttpResponseModel model, HttpResponseHeaders headers)
        {
            foreach (var header in headers)
            {
                if (!model.Headers.ContainsKey(header.Key))
                {
                    model.Headers[header.Key] = new List<string>();
                }

                model.Headers[header.Key].AddRange(header.Value);
            }
        }
    }
}
