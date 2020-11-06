using System;
using thirdconspiracy.WebRequest.HTTP.Models;

namespace thirdconspiracy.WebRequest.HttpLogger.EventManager
{

    public class HttpCommunicationEventArgs : EventArgs
    {
        public IHttpRequestModel Request { get; }
        public IHttpResponseModel Response { get; }
        public Exception CaughtException { get; }

        public HttpCommunicationEventArgs(IHttpRequestModel request, IHttpResponseModel response, Exception caughtEx)
        {
            Request = request;
            Response = response;
            CaughtException = caughtEx;
        }

    }
}
