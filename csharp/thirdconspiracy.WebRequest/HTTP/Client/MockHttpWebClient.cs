using System;
using System.Collections.Generic;
using thirdconspiracy.WebRequest.HTTP.Models;
using thirdconspiracy.WebRequest.HttpLogger.EventManager;

namespace thirdconspiracy.WebRequest.HTTP.Client
{
    public class MockHttpWebClient : IHttpWebClient
    {
        #region Member Variables

        public Queue<MockHttpResponseModel> MockResponseQueue { get; }
            = new Queue<MockHttpResponseModel>();

        public List<MockHttpResponseMatcher> MockResponseLookup { get; }
            = new List<MockHttpResponseMatcher>();

        #endregion Member Variables

        public IHttpResponseModel Execute(IHttpRequestModel httpRequest)
        {
            if (!(httpRequest is HttpRequestModel request))
            {
                throw new ArgumentException("Client does not support request object type");
            }

            MockHttpResponseModel response = null;
            Exception caughtException = null;
            try
            {
                response = MockSend(request);
                return response;
            }
            catch (Exception e)
            {
                caughtException = e;
                throw;
            }
            finally
            {
                CommunicationLoggerEventManager.NotifyHttpRequestCompleted(
                    this, new HttpCommunicationEvent(request, response, caughtException));
            }
        }

        private MockHttpResponseModel MockSend(HttpRequestModel request)
        {
            MockHttpResponseModel mockResponse;
            if (MockResponseQueue.Count > 0)
            {
                var sentAtUtc = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(-1);

                mockResponse = MockResponseQueue.Dequeue();
                mockResponse.SentAtUtc = sentAtUtc;
                mockResponse.CompletedAtUtc = DateTimeOffset.UtcNow;

                return mockResponse;
            }

            foreach (var lookupResponse in MockResponseLookup)
            {
                if (lookupResponse.MatchesRequest(request))
                {
                    return lookupResponse.Process(request);
                }
            }

            throw new NotImplementedException("There are no mocked responses queued or added to pattern match");
        }

    }
}
