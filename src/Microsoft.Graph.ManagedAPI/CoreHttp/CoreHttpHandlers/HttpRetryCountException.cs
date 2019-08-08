namespace Microsoft.Graph.CoreHttp
{
    using System;
    using System.Net;

    /// <summary>
    /// Retry count exception.
    /// </summary>
    public class HttpRetryCountException : Exception
    {
        /// <summary>
        /// Create new instance of <see cref="RetryCountException"/>.
        /// </summary>
        /// <param name="retryHandler">Retry handler name.</param>
        /// <param name="retryCount">Retry count.</param>
        /// <param name="totalDelayApplied">Total delay applied.</param>
        /// <param name="requestUri">Request uri.</param>
        /// <param name="method">Request method.</param>
        /// <param name="statusCode">Status code.</param>
        public HttpRetryCountException(string retryHandler, int retryCount, int totalDelayApplied, Uri requestUri, string method, HttpStatusCode statusCode)
            : base($"{retryHandler}: Retry count exceeded to uri '{requestUri}'")
        {
            this.RetryCount = retryCount;
            this.TotalDelayApplied = totalDelayApplied;
            this.RequestUri = requestUri;
            this.RequestMethod = method;
            this.StatusCode = statusCode;
        }

        /// <summary>
        /// Retry count.
        /// </summary>
        public int RetryCount { get; }

        /// <summary>
        /// Total delay applied.
        /// </summary>
        public int TotalDelayApplied { get; }

        /// <summary>
        /// Requst uri.
        /// </summary>
        public Uri RequestUri { get; }

        /// <summary>
        /// Request method.
        /// </summary>
        public string RequestMethod { get; }

        /// <summary>
        /// Http status code.
        /// </summary>
        public HttpStatusCode StatusCode { get; }
    }
}
