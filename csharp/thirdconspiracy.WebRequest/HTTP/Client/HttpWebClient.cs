using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using thirdconspiracy.WebRequest.HTTP.Models;
using thirdconspiracy.WebRequest.HTTP.Utilities;
using thirdconspiracy.WebRequest.HttpLogger.EventManager;

namespace thirdconspiracy.WebRequest.HTTP.Client
{
    public class HttpWebClient : IHttpWebClient
    {
        #region Member Variables

        private static readonly TimeSpan HTTP_CLIENT_MAX_SYSTEM_TIMEOUT = TimeSpan.FromMinutes(10);

        private static Lazy<HttpClient> _httpClient;
        private static HttpClient HttpClient => _httpClient.Value;

        private readonly TimeSpan _clientDefaultRequestTimeout;
        private int _currentTransactionId = 0;

        #endregion Member Variables

        #region CTOR

        public HttpWebClient(TimeSpan defaultTimeout)
        {
            if (defaultTimeout > HTTP_CLIENT_MAX_SYSTEM_TIMEOUT)
            {
                throw new ArgumentException(
                    "Core client created with request timeout greater than system max");
            }

            _clientDefaultRequestTimeout = defaultTimeout;
            _httpClient = new Lazy<HttpClient>(() =>
                new HttpClient
                {
                    Timeout = HTTP_CLIENT_MAX_SYSTEM_TIMEOUT
                });
        }

        #endregion CTOR

        public async Task<IHttpResponseModel> Execute(IHttpRequestModel httpRequest)
        {
            if (!(httpRequest is HttpRequestModel request))
            {
                throw new ArgumentException("Client does not support request object type");
            }

            IHttpResponseModel response = null;
            Exception caughtEx = null;

            try
            {
                ValidateRequest(request);

                response = await Send(request);

                return response;
            }
            catch (Exception ex)
            {
                caughtEx = ex;

                throw;
            }
            finally
            {
                CommunicationLoggerEventManager.NotifyHttpRequestCompleted(
                    this, new HttpCommunicationEvent(request, response, caughtEx));
            }
        }

        private void ValidateRequest(HttpRequestModel model)
        {
            //Validate the content type settings
            if (model.ContentType != null && model.ContentType == string.Empty)
            {
                throw new ArgumentException($"{nameof(model.ContentType)} cannot be empty");
            }

            //If a timeout override has been specified and its greater than the max system specified
            //timeout, throw here. That isn't a valid input and would not be respected.
            if (model.ResponseTimeoutOverride.HasValue
                && model.ResponseTimeoutOverride > HTTP_CLIENT_MAX_SYSTEM_TIMEOUT)
            {
                throw new ArgumentException(
                    $"{nameof(model.ResponseTimeoutOverride)} cannot be greater than the max system timeout of: {HTTP_CLIENT_MAX_SYSTEM_TIMEOUT.TotalMilliseconds}ms");
            }

            //Validate the body -- there shouldn't be a body for get or delete requests
            if (model.Body != null &&
                (model.Method == HttpAction.GET || model.Method == HttpAction.DELETE))
            {
                throw new ArgumentException($"HttpRequests with method:{model.Method} do not support body payloads");
            }
        }

        private async Task<HttpResponseModel> Send(HttpRequestModel request)
        {
            using (var reqMsg = HttpRequestMessageBuilder.BuildHttpRequestMessage(request))
            {
                var cancellationToken = GetCancellationToken(request.ResponseTimeoutOverride);
                var transactionId = GetNextTransactionId();

                var sentAtUtc = DateTimeOffset.UtcNow;
                var respMsg = await SendNow(reqMsg, cancellationToken);

                var resp = HttpResponseBuilder
                    .BuildHttpResponseModel(
                        transactionId,
                        sentAtUtc,
                        DateTimeOffset.UtcNow, 
                        request.ShouldBufferResponseBody,
                        respMsg);

                return await resp;
            }
        }

        private CancellationToken GetCancellationToken(TimeSpan? timeout)
        {
            if (timeout == null || timeout == TimeSpan.Zero)
            {
                timeout = _clientDefaultRequestTimeout;
            }

            return new CancellationTokenSource(timeout.Value).Token;
        }

        private int GetNextTransactionId()
        {
	        return Interlocked.Increment(ref _currentTransactionId);
        }

        private async Task<HttpResponseMessage> SendNow(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            try
            {
                var task = HttpClient
                    .SendAsync(
                        requestMessage,
                        HttpCompletionOption.ResponseHeadersRead,
                        cancellationToken);

                return await task;
            }
            catch (OperationCanceledException e)
            {
                throw new TimeoutException("Timeout expired via cancellation token", e);
            }
        }

    }
}
