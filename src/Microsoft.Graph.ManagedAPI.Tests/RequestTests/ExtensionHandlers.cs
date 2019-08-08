namespace Microsoft.Graph.ManagedAPI.Tests.RequestTests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Graph.CoreHttp;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Url validator extension handler.
    /// </summary>
    internal class UrlValidatorExtensionHandler : IHttpExtensionHandler
    {
        /// <summary>
        /// Expected uri.
        /// </summary>
        private Uri expectedUri;

        /// <summary>
        /// Create new instance of <see cref="UrlValidatorExtensionHandler"/>
        /// </summary>
        /// <param name="expectedUri">Expected uri.</param>
        internal UrlValidatorExtensionHandler(Uri expectedUri)
        {
            this.expectedUri = expectedUri;
        }

        /// <inheritdoc cref="IHttpExtensionHandler.SendAsync"/>
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequest, CancellationToken cancellationToken, Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> nextHandler)
        {
            Assert.AreEqual(
                httpRequest.RequestUri,
                this.expectedUri);

            return HttpResponseExtensionHandler.Default.SendAsync(httpRequest, cancellationToken, nextHandler);
        }
    }

    /// <summary>
    /// Http response validator handler.
    /// </summary>
    internal class HttpResponseExtensionHandler : IHttpExtensionHandler
    {
        /// <summary>
        /// Response message to return.
        /// </summary>
        private HttpResponseMessage responseMessage;

        /// <summary>
        /// Create new instance of <see cref="HttpResponseExtensionHandler"/>
        /// </summary>
        /// <param name="responseMessage"></param>
        internal HttpResponseExtensionHandler(HttpResponseMessage responseMessage)
        {
            this.responseMessage = responseMessage;
        }

        /// <summary>
        /// Default handler.
        /// </summary>
        internal static HttpResponseExtensionHandler Default
        {
            get
            {
                return new HttpResponseExtensionHandler(
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("")
                });
            }
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequest, CancellationToken cancellationToken, Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> nextHandler)
        {
            return Task.FromResult(this.responseMessage);
        }
    }

    /// <summary>
    /// Url validator and http response extension handler.
    /// </summary>
    internal class UrlValidatorAndResponseExtensionHandler : IHttpExtensionHandler
    {
        /// <summary>
        /// Url validator handler.
        /// </summary>
        private UrlValidatorExtensionHandler urlValidatorExtensionHandler;

        /// <summary>
        /// Response handler.
        /// </summary>
        private HttpResponseExtensionHandler responseExtensionHandler;

        /// <summary>
        /// Create new instance of <see cref="UrlValidatorAndResponseExtensionHandler"/>
        /// </summary>
        /// <param name="expectedUri">Request Uri.</param>
        /// <param name="content">Content.</param>
        /// <param name="httpStatusCode">Response code.</param>
        internal UrlValidatorAndResponseExtensionHandler(Uri expectedUri, string content,
            HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            this.urlValidatorExtensionHandler = new UrlValidatorExtensionHandler(expectedUri);

            HttpResponseMessage responseMessage = new HttpResponseMessage(httpStatusCode)
            {
                Content = new StringContent(content)
            };

            this.responseExtensionHandler = new HttpResponseExtensionHandler(responseMessage);
        }

        /// <summary>
        /// Send async.
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="nextHandler"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequest, CancellationToken cancellationToken, Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> nextHandler)
        {
            // validate Url.
            await this.urlValidatorExtensionHandler.SendAsync(
                httpRequest, 
                cancellationToken, 
                nextHandler);

            // Return configured response.
            return await this.responseExtensionHandler.SendAsync(
                httpRequest, 
                cancellationToken, 
                nextHandler);
        }
    }
}
