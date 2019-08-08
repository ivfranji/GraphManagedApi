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
    internal class HttpUserAgentHandler : DelegatingHandler
    {
        /// <summary>
        /// Create new instance of <see cref="UserAgentHttpHandler"/>.
        /// </summary>
        public HttpUserAgentHandler()
        {
            this.DefaultUserAgent = "Graph-ManagedAPI";
        }

        /// <summary>
        /// Default user agent.
        /// </summary>
        public string DefaultUserAgent { get; }

        /// <summary>
        /// Version.
        /// </summary>
        public Lazy<string> Version
        {
            get { return HttpUserAgentHandler.buildNumber; }
        }

        /// <summary>
        /// Send request and retrieve response.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
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
