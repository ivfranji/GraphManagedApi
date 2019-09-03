namespace Microsoft.Graph.CoreHttp
{
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// User agent http handler. Configure user agent on outgoing requests.
    /// </summary>
    internal class HttpRequestHeaderHandler : DelegatingHandler
    {
        /// <summary>
        /// The client request identifier header
        /// </summary>
        private const string ClientRequestIdHeader = "client-request-id";

        /// <summary>
        /// The SDK version request header
        /// </summary>
        private const string SdkVersionRequestHeader = "SdkVersion";

        /// <summary>
        /// Create new instance of <see cref="HttpRequestHeaderHandler"/>.
        /// </summary>
        public HttpRequestHeaderHandler()
        {
        }

        /// <summary>
        /// Default user agent.
        /// </summary>
        public string DefaultUserAgent
        {
            get { return "Graph-ManagedAPI"; }
        }

        /// <summary>
        /// Version.
        /// </summary>
        public Lazy<string> Version
        {
            get { return HttpRequestHeaderHandler.buildNumber; }
        }

        /// <summary>
        /// Send request and retrieve response.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // this will be propagated back to the client.
            request.Headers.Add(
                HttpRequestHeaderHandler.ClientRequestIdHeader,
                Guid.NewGuid().ToString());

            request.Headers.Add(
                SdkVersionRequestHeader,
                $"{this.DefaultUserAgent}/{this.Version.Value}");

            // if no agent added, append default, otherwise append default name to 
            // all agents in request.
            if (request.Headers.UserAgent.Count == 0)
            {
                request.Headers.UserAgent.Add(
                    new ProductInfoHeaderValue(
                        this.DefaultUserAgent,
                        this.Version.Value));
            }
            else
            {
                ProductInfoHeaderValue[] currentValues = new ProductInfoHeaderValue[request.Headers.UserAgent.Count];
                request.Headers.UserAgent.CopyTo(
                    currentValues,
                    0);

                request.Headers.UserAgent.Clear();
                foreach (ProductInfoHeaderValue requestUserAgent in currentValues)
                {
                    request.Headers.UserAgent.Add(new ProductInfoHeaderValue(
                        $"{this.DefaultUserAgent}-{requestUserAgent.Product.Name}",
                        this.Version.Value));
                }
            }

            return base.SendAsync(request, cancellationToken);
        }

        /// <summary>
        /// Lazy buildNumber member.
        /// </summary>
        private static Lazy<string> buildNumber = new Lazy<string>(() =>
        {
            try
            {
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                return fileVersionInfo.FileVersion;
            }
            catch
            {
                return "0.0";
            }
        });
    }
}
