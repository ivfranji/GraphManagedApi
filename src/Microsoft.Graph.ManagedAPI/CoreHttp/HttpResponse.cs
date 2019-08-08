namespace Microsoft.Graph.CoreHttp
{
    using System.Net.Http.Headers;

    /// <summary>
    /// Http response.
    /// </summary>
    public class HttpResponse
    {
        /// <summary>
        /// Create new instance of <see cref="HttpResponse"/>
        /// </summary>
        /// <param name="content">Response content.</param>
        /// <param name="error">Response error.</param>
        /// <param name="success">Response successful.</param>
        internal HttpResponse(string content, string error, bool success, HttpResponseHeaders responseHeaders)
        {
            this.Content = content;
            this.Error = error;
            this.Success = success;
            this.ResponseHeaders = responseHeaders;
        }

        /// <inheritdoc cref="IHttpResponse.Content"/>
        public string Content { get; }

        /// <inheritdoc cref="IHttpResponse.Error"/>
        public string Error { get; }

        /// <inheritdoc cref="IHttpResponse.Success"/>
        public bool Success { get; }

        /// <inheritdoc cref="IHttpResponse.ResponseHeaders"/>
        public HttpResponseHeaders ResponseHeaders { get; }
    }
}
