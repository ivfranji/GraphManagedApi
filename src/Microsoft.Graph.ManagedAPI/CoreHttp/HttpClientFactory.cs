namespace Microsoft.Graph.CoreHttp
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;

    /// <summary>
    /// Http client factory.
    /// </summary>
    internal static class HttpClientFactory
    {
        /// <summary>
        /// Http client.
        /// </summary>
        private static HttpClient httpClient = HttpClientFactory.CreateHttpClient(null);

        /// <summary>
        /// Set proxy server. This will recreate http handler pipeline.
        /// </summary>
        /// <param name="proxyServer">Proxy server.</param>
        public static void SetProxyServer(IWebProxy proxyServer)
        {
            if (HttpClientFactory.httpClient != null)
            {
                HttpClientFactory.httpClient.Dispose();
            }

            HttpClientFactory.httpClient = HttpClientFactory.CreateHttpClient(proxyServer);
        }

        /// <summary>
        /// Create http client per request, however, it re-uses handler
        /// </summary>
        /// <returns></returns>
        public static HttpClient Get()
        {
            return HttpClientFactory.httpClient;
        }

        /// <summary>
        /// Create http client.
        /// </summary>
        /// <param name="webProxy"></param>
        /// <returns></returns>
        private static HttpClient CreateHttpClient(IWebProxy webProxy)
        {
            HttpClient httpClient = new HttpClient(
                HttpClientFactory.CreateHandlerPipeline(webProxy),
                true);

            httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue()
            {
                NoCache = true,
                NoStore = true
            };

            httpClient.Timeout = TimeSpan.FromSeconds(30);
            return httpClient;
        }

        /// <summary>
        /// Reused pipeline.
        /// </summary>
        /// <returns></returns>
        private static HttpMessageHandler CreateHandlerPipeline(IWebProxy webProxy)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            if (null != webProxy)
            {
                httpClientHandler.Proxy = webProxy;
            }

            // Order should be:
            // UserAgent -> Throttling -> Authorization -> LogWriter -> Extension
            DelegatingHandler[] delegatingHandlers = new DelegatingHandler[]
            {
                new HttpRequestHeaderHandler(), 
                new HttpThrottlingHandler(), 
                new HttpAuthZHandler(), 
                new HttpLogWriterHandler(),
                new HttpExtensionHandler(),
            };

            HttpMessageHandler httpHandlerPipeline = httpClientHandler;
            for (int i = delegatingHandlers.Length - 1; i >= 0; i--)
            {
                if (delegatingHandlers[i] == null)
                {
                    throw new ArgumentNullException(nameof(delegatingHandlers));
                }
                if (delegatingHandlers[i].InnerHandler != null)
                {
                    throw new InvalidOperationException("Delegating handler already has inner handler.");
                }

                delegatingHandlers[i].InnerHandler = httpHandlerPipeline;
                httpHandlerPipeline = delegatingHandlers[i];
            }

            return httpHandlerPipeline;
        }
    }
}
