using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using thirdconspiracy.Logger;
using thirdconspiracy.Logger.Console;
using thirdconspiracy.WebRequest.HTTP.Models;
using thirdconspiracy.WebRequest.HttpLogger.EventManager;

namespace thirdconspiracy.WebRequest.HttpLogger
{
    public class HttpConsoleLogger : IDisposable
    {

        #region Member Variables

        private bool _isDisposed;
        private static ConsoleLogger _consoleLogger;

        #endregion Member Variables

        #region CTOR

        public HttpConsoleLogger()
        {
            _consoleLogger = new ConsoleLogger();
            SubscribeToHttpEvents();
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
            var sb = new StringBuilder();
            sb.AppendLine("\n");
            sb.AppendLine("===============================================");

            //Request
            sb.AppendLine(evtArgs.Request == null ? "No Request" : FormatRequestForLog(evtArgs.Request));

            //Exception
            if (evtArgs.CaughtException != null)
            {
                sb.AppendLine("Exception:");
                sb.AppendLine($"{evtArgs.CaughtException}");
                sb.AppendLine();
            }

            //Response
            if (evtArgs.Response != null)
            {
                sb.AppendLine(evtArgs.Response == null ? "No Response" : FormatResponseForLog(evtArgs.Response));
            }

            sb.AppendLine("===============================================");

            _consoleLogger.Log(LogLevel.Info, sb.ToString(), null);
        }

        private string FormatRequestForLog(IHttpRequestModel request)
        {
            var sb = new StringBuilder();

            sb.AppendLine("API Request:");
            sb.AppendFormat("{0} {1} HTTP/1.1\n", request.Method, request.FullUri);

            FormatAndPrintHeaders(sb, request.Headers);

            sb.AppendFormat("Host: {0}\n", request.FullUri);
            sb.AppendLine("Body:");

            string body = request.GetBodyAsString();
            if (string.IsNullOrEmpty(body))
            {
                sb.AppendLine(body);
            }

            return sb.ToString();
        }

        private string FormatResponseForLog(IHttpResponseModel response)
        {
            var sb = new StringBuilder();

            sb.AppendLine("API Response:");
            sb.AppendLine();

            FormatAndPrintHeaders(sb, response.Headers);

            if (response.IsBodyInMemory)
            {
                string responseBody = response.GetBodyAsString();
                if (!string.IsNullOrWhiteSpace(responseBody))
                {
                    sb.AppendLine();
                    sb.AppendLine(responseBody);
                }
            }

            return sb.ToString();
        }

        private void FormatAndPrintHeaders(StringBuilder builder, Dictionary<string, List<string>> rawHeaders)
        {
            if (rawHeaders == null || rawHeaders.Count == 0)
            {
                return;
            }

            var headerLength = rawHeaders
                .OrderByDescending(kvp => kvp.Key.Length)
                .First()
                .Key.Length;

            var headers = rawHeaders
                .Select(kvp => $" [{kvp.Key.PadRight(headerLength, ' ')}] = {string.Join(",", kvp.Value)}");

            foreach (var header in headers)
            {
                builder.AppendLine(header);
            }
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
