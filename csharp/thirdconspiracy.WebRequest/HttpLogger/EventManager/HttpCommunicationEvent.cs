using System;
using thirdconspiracy.WebRequest.HTTP.Models;

namespace thirdconspiracy.WebRequest.HttpLogger.EventManager
{
    public class HttpCommunicationEvent : EventArgs
    {
        public IHttpRequestModel Request { get; }
        public IHttpResponseModel Response { get; }
        public Exception CaughtException { get; }

        public HttpCommunicationEvent(IHttpRequestModel request, IHttpResponseModel response, Exception caughtEx)
        {
            Request = request;
            Response = response;
            CaughtException = caughtEx;
        }
    }
}
