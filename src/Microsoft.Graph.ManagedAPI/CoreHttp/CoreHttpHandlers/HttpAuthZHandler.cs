namespace Microsoft.Graph.CoreHttp
{
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Authorization http handler.
    /// </summary>
    internal class HttpAuthZHandler : HttpRetryHandler
    {
        /// <summary>
        /// AuthZ handler name.
        /// </summary>
        private const string AuthZHandlerNameHeader = "X-HttpAuthZHandler";

        /// <summary>
        /// Create new instance of <see cref="AuthZHttpHandler"/>
        /// </summary>
        /// <param name="retryOptions"></param>
        public HttpAuthZHandler(HttpRetryOptions retryOptions = null)
            : base(retryOptions)
        {
        }

        /// <inheritdoc cref="HttpRetryHandler.PreProcessHttpRequest"/>
        protected override async Task PreProcessHttpRequest(HttpRequestMessage request)
        {
            // Authenticate request before it is sent.
            HttpRequestContext requestContext = request.GetHttpRequestContext();
            if (request.Headers.Contains(HttpAuthZHandler.AuthZHandlerNameHeader))
            {
                request.Headers.Remove(HttpAuthZHandler.AuthZHandlerNameHeader);
            }

            // logging will capture this.
            request.Headers.Add(
                HttpAuthZHandler.AuthZHandlerNameHeader,
                requestContext.AuthorizationProvider.Name);

            request.Headers.Authorization = await requestContext.AuthorizationProvider.GetAuthenticationHeader();
        }

        /// <summary>
        /// Retry on unauthorized.
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        protected override bool ShouldRetry(HttpResponseMessage responseMessage)
        {
            return responseMessage.StatusCode == HttpStatusCode.Unauthorized;
        }

        /// <inheritdoc cref="HttpRetryHandler.ApplyDelay"/>
        protected override Task<int> ApplyDelay(HttpResponseMessage responseMessage, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}
