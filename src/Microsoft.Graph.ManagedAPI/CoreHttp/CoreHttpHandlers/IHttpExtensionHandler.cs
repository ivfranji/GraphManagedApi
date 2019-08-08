namespace Microsoft.Graph.CoreHttp
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Http extension handler interface.
    /// </summary>
    internal interface IHttpExtensionHandler
    {
        /// <summary>
        /// Perform Send async.
        /// </summary>
        /// <param name="httpRequest">Http request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <param name="nextHandler">Next handler (base).</param>
        /// <returns></returns>
        Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage httpRequest,
            CancellationToken cancellationToken,
            Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> nextHandler);
    }
}
