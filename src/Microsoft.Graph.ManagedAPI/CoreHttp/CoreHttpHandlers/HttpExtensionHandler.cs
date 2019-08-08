namespace Microsoft.Graph.CoreHttp
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension http handler. Last handler in the pipeline and can
    /// short circuit request before sending it to HttpClientHandler.
    /// This is currently designed for unit testing.
    /// </summary>
    internal class HttpExtensionHandler : DelegatingHandler
    {
        /// <summary>
        /// Http header name for this handler.
        /// </summary>
        private const string HttpHeaderName = "X-HttpExtensionHandler";

        /// <summary>
        /// Send request async.
        /// </summary>
        /// <param name="httpRequest">Http request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        protected sealed override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequest, CancellationToken cancellationToken)
        {
            HttpRequestContext requestContext = httpRequest.GetHttpRequestContext();
            if (null == requestContext.HttpExtensionHandler)
            {
                return await base.SendAsync(httpRequest, cancellationToken);
            }

            HttpResponseMessage httpResponseMessage = await requestContext.HttpExtensionHandler.SendAsync(
                httpRequest,
                cancellationToken,
                base.SendAsync);

            httpResponseMessage.Headers.Add(
                HttpExtensionHandler.HttpHeaderName,
                requestContext.HttpExtensionHandler.ToString());

            return httpResponseMessage;
        }
    }
}
