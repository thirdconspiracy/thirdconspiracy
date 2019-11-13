using System;

namespace thirdconspiracy.WebRequest.HttpLogger.EventManager
{
    public static class CommunicationLoggerEventManager
    {
        public static event EventHandler<HttpCommunicationEvent> CommunicationListener;

        public static void NotifyHttpRequestCompleted(object sender, HttpCommunicationEvent httpEvent)
        {
            CommunicationListener?.Invoke(sender, httpEvent);
        }
    }
}
