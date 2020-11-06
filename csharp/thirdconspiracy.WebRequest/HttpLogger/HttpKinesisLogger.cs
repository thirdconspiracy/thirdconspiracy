using System;
using System.Collections.Generic;
using thirdconspiracy.Logger.Kinesis;
using thirdconspiracy.WebRequest.HttpLogger.EventManager;

namespace thirdconspiracy.WebRequest.HttpLogger
{

    public class HttpKinesisLogger
    {

        #region Member Variables

        private readonly KinesisConfig _cfg;
        private static KinesisLogger _kinesisLogger;

        private bool _isDisposed;

        #endregion Member Variables

        #region CTOR

        public HttpKinesisLogger(KinesisConfig cfg)
        {
            _cfg = cfg;
            _kinesisLogger = new KinesisLogger(cfg);
        }

        #endregion CTOR

        #region Subscribe

        private void SubscribeToHttpEvents()
        {
            CommunicationLoggerEventManager.CommunicationListener += HandleRequestCompletedEvent;
        }

        private void UnsubscribeFromHttpEvents()
        {
            CommunicationLoggerEventManager.CommunicationListener -= HandleRequestCompletedEvent;
        }

        #endregion Subscribe

        #region Write

        private void HandleRequestCompletedEvent(object sender, HttpCommunicationEventArgs evtArgs)
        {
            var document = BuildKinesisDocument(evtArgs);
            if (_cfg.IsEnabled)
            {
                _kinesisLogger.LogDocument(
                    KinesisIndexeAndTypeConstants.HTTP.Index,
                    KinesisIndexeAndTypeConstants.HTTP.DocumentType,
                    document);
            }
        }

        private HttpKinesisDocument BuildKinesisDocument(HttpCommunicationEventArgs evtArgs)
        {
            var doc = new HttpKinesisDocument
            {
                RequestSentAtUtc = evtArgs?.Response?.SentAtUtc,
                ResponseReceivedAtUtc = evtArgs?.Response?.CompletedAtUtc,
                TotalElapsedTimeMs = (long?)evtArgs?.Response?.ResponseTime.TotalMilliseconds,

                Url = evtArgs?.Request.FullUri,
                Method = evtArgs?.Request.Method.ToString(),
                StatusCode = (int)evtArgs?.Response?.StatusCode,
                TransactionId = evtArgs?.Response?.TransactionId.ToString(),

                RequestHeaders = FlattenHeaders(evtArgs?.Request?.Headers),
                RequestBody = evtArgs?.Request?.GetBodyAsString(),

                ResponseHeaders = FlattenHeaders(evtArgs?.Response?.Headers),
                ResponseBody = string.Empty,

                Exception = evtArgs.CaughtException
            };

            if (evtArgs?.Response?.IsBodyInMemory ?? false)
            {
                doc.ResponseBody = evtArgs.Response.GetBodyAsString();
            }

            return doc;
        }

        private List<string> FlattenHeaders(Dictionary<string, List<string>> headerValues)
        {
            var flattened = new List<string>();

            foreach (var kvp in headerValues)
            {
                if (kvp.Key.Equals("Authorization", StringComparison.InvariantCultureIgnoreCase)) continue;
                foreach (var val in kvp.Value)
                {
                    flattened.Add($"{kvp.Key} {val}");
                }
            }

            return flattened;
        }

        #endregion Write

        #region DTOR

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            UnsubscribeFromHttpEvents();

            _isDisposed = true;
        }

        #endregion DTOR

    }
}
