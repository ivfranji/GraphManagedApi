namespace Microsoft.Graph.CoreHttp
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Retry http handler.
    /// </summary>
    internal abstract class HttpRetryHandler : DelegatingHandler
    {
        /// <summary>
        /// Retry attempt.
        /// </summary>
        private const string RetryAttempt = "X-RetryAttempt";

        /// <summary>
        /// Total delay applied.
        /// </summary>
        private const string TotalDelayApplied = "X-TotalDelayApplied";

        /// <summary>
        /// Create new instance of <see cref="RetryHttpHandler"/>
        /// </summary>
        /// <param name="httpRetryOptions">Retry options.</param>
        protected HttpRetryHandler(HttpRetryOptions httpRetryOptions)
        {
            this.HttpRetryOptions = httpRetryOptions ?? HttpRetryOptions.DefaultHttpRetryOptions;

            this.HandlerName = this.GetType().Name;
            this.TotalDelayAppliedHttpHeaderName = $"{HttpRetryHandler.TotalDelayApplied}-{this.HandlerName}";
            this.RetryAttemptHttpHeaderName = $"{HttpRetryHandler.RetryAttempt}-{this.HandlerName}";
        }

        /// <summary>
        /// Handler name.
        /// </summary>
        internal string HandlerName { get; }

        /// <summary>
        /// Retry options.
        /// </summary>
        internal HttpRetryOptions HttpRetryOptions
        {
            get;
        }

        /// <summary>
        /// Total delay http header name.
        /// </summary>
        internal string TotalDelayAppliedHttpHeaderName
        {
            get;
        }

        /// <summary>
        /// Total delay http header name.
        /// </summary>
        internal string RetryAttemptHttpHeaderName
        {
            get;
        }

        /// <summary>
        /// Send request and retrieve response.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        protected sealed override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            int retryCount = 0;
            int totalDelayApplied = 0;
            do
            {
                await this.PreProcessHttpRequest(request);
                HttpResponseMessage responseMessage = await base.SendAsync(request, cancellationToken);
                if (!this.ShouldRetry(responseMessage))
                {
                    this.SetHttpHeader(
                        responseMessage,
                        this.TotalDelayAppliedHttpHeaderName,
                        totalDelayApplied.ToString());

                    this.SetHttpHeader(
                        responseMessage,
                        this.RetryAttemptHttpHeaderName,
                        retryCount.ToString());

                    return responseMessage;
                }
                else
                {
                    if (retryCount >= this.HttpRetryOptions.RetryCount)
                    {
                        throw new HttpRetryCountException(
                            this.HandlerName,
                            retryCount,
                            totalDelayApplied,
                            request.RequestUri,
                            request.Method.Method,
                            responseMessage.StatusCode);
                    }

                    retryCount++;
                    totalDelayApplied += await this.ApplyDelay(
                        responseMessage,
                        cancellationToken);
                }

            } while (true);
        }

        /// <summary>
        /// Give chance to child classes to prepare request
        /// before sending.
        /// </summary>
        protected virtual async Task PreProcessHttpRequest(HttpRequestMessage httpRequestMessage)
        {
            await Task.Run((() => { }));
        }

        /// <summary>
        /// Indicate if it should retry based on entityResponse message.
        /// </summary>
        /// <param name="responseMessage">Response message.</param>
        /// <returns></returns>
        protected abstract bool ShouldRetry(HttpResponseMessage responseMessage);

        /// <summary>
        /// Apply delay to throttled call.
        /// </summary>
        protected abstract Task<int> ApplyDelay(HttpResponseMessage responseMessage, CancellationToken cancellationToken);

        /// <summary>
        /// Sets http header to particular value.
        /// </summary>
        /// <param name="responseMessage">Response message.</param>
        /// <param name="httpHeader">Http header name.</param>
        /// <param name="value">Http header value.</param>
        private void SetHttpHeader(HttpResponseMessage responseMessage, string httpHeader, string value)
        {
            if (responseMessage.Headers.Contains(httpHeader))
            {
                responseMessage.Headers.Remove(httpHeader);
            }

            responseMessage.Headers.Add(
                httpHeader,
                value);
        }
    }
}
