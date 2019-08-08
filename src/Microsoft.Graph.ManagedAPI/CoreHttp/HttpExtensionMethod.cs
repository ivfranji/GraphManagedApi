namespace Microsoft.Graph.CoreHttp
{
    using System.Net.Http;

    /// <summary>
    /// Extension helper methods.
    /// </summary>
    internal static class HttpExtensionMethod
    {
        /// <summary>
        /// Get <see cref="HttpRequestContext"/> from <see cref="HttpRequestMessage"/>
        /// </summary>
        /// <param name="request">Request to get context from.</param>
        /// <returns></returns>
        public static HttpRequestContext GetHttpRequestContext(this HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey(nameof(HttpRequestContext)))
            {
                object requestContext = request.Properties[nameof(HttpRequestContext)];
                if (requestContext is HttpRequestContext context)
                {
                    return context;
                }
            }

            // just return empty context in case no other set.
            return new HttpRequestContext();
        }
    }
}
