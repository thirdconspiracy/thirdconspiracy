using System;

namespace thirdconspiracy.WebRequest.HttpLogger.EventManager
{
    public static class CommunicationLoggerEventManager
    {
        public static event EventHandler<HttpCommunicationEventArgs> CommunicationListener;

        public static void NotifyHttpRequestCompleted(object sender, HttpCommunicationEventArgs httpEventArgs)
        {
	        var eventHandler = CommunicationListener;
	        var receivers = eventHandler?.GetInvocationList();
	        if (receivers == null)
		        return;
            foreach (EventHandler<HttpCommunicationEventArgs> receiver in receivers)
	            receiver.BeginInvoke(sender, httpEventArgs, null, null);
        }
    }
}
