namespace Microsoft.Graph.CoreHttp
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Http request.
    /// </summary>
    internal class HttpRequest : IDisposable
    {
        /// <summary>
        /// PATCH method.
        /// </summary>
        private static readonly HttpMethod PATCH = new HttpMethod("PATCH");

        /// <summary>
        /// Request message.
        /// </summary>
        private HttpRequestMessage requestMessage;

        /// <summary>
        /// Create http request.
        /// </summary>
        /// <param name="context">Request context.</param>
        /// <param name="requestUri">Request uri.</param>
        /// <param name="httpMethod">Http method.</param>
        /// <param name="content">Content.</param>
        private HttpRequest(HttpRequestContext context, Uri requestUri, HttpMethod httpMethod, string content)
        {
            this.requestMessage = new HttpRequestMessage(httpMethod, requestUri);
            this.requestMessage.Properties.Add(nameof(HttpRequestContext), context);

            if (httpMethod == HttpRequest.PATCH ||
                httpMethod == HttpMethod.Post)
            {
                // PATCH shouldn't have empty content.
                if (httpMethod == HttpRequest.PATCH &&
                    string.IsNullOrEmpty(content))
                {
                    throw new ArgumentNullException(nameof(content),
                        "Content cannot be empty for PATCH request.");
                }

                this.requestMessage.Content = this.CreateContent(content);
            }
        }

        /// <summary>
        /// Get response in async fashion.
        /// </summary>
        /// <returns></returns>
        internal async Task<HttpResponse> GetHttpResponseAsync()
        {
            HttpClient httpClient = HttpClientFactory.Get();
            HttpResponseMessage responseMessage = null;

            try
            {
                if (this.AdditionalHttpHeaders != null &&
                    this.AdditionalHttpHeaders.Count > 0)
                {
                    foreach (KeyValuePair<string, string> header in this.AdditionalHttpHeaders)
                    {
                        this.requestMessage.Headers.Add(
                            header.Key,
                            header.Value);
                    }
                }

                // TODO: Implement overload which will return stream to be used by Json.
                responseMessage = await httpClient.SendAsync(this.requestMessage);
                string content = string.Empty;
                string error = string.Empty;
                bool success = false;

                if (responseMessage.Content != null)
                {
                    content = await responseMessage.Content.ReadAsStringAsync();
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        success = true;
                    }
                    else
                    {
                        error = content;
                    }
                }
                else
                {
                    error = "Http response empty.";
                }

                return new HttpResponse(
                    content,
                    error,
                    success,
                    responseMessage.Headers);
            }
            finally
            {
                responseMessage?.Dispose();
            }
        }

        /// <summary>
        /// User agent.
        /// </summary>
        internal string UserAgent
        {
            set
            {
                if (null != this.requestMessage &&
                    !string.IsNullOrEmpty(value))
                {
                    this.requestMessage.Headers.UserAgent.Clear();
                    this.requestMessage.Headers.UserAgent.Add(
                        new ProductInfoHeaderValue(
                            value,
                            "0.0"));
                }
            }
        }

        /// <summary>
        /// Additional http headers.
        /// </summary>
        internal Dictionary<string, string> AdditionalHttpHeaders { get; set; }

        /// <summary>
        /// Dispose implementation.
        /// </summary>
        public void Dispose()
        {
            this.requestMessage?.Dispose();
        }

        /// <summary>
        /// Create HTTP GET request.
        /// </summary>
        /// <param name="requestUri">Request uri.</param>
        /// <param name="context">Request context.</param>
        /// <returns></returns>
        public static HttpRequest Get(Uri requestUri, HttpRequestContext context)
        {
            return new HttpRequest(
                context,
                requestUri,
                HttpMethod.Get,
                null);
        }

        /// <summary>
        /// Create HTTP DELETE request.
        /// </summary>
        /// <param name="requestUri">Request uri.</param>
        /// <param name="context">Request context.</param>
        /// <returns></returns>
        public static HttpRequest Delete(Uri requestUri, HttpRequestContext context)
        {
            return new HttpRequest(
                context,
                requestUri,
                HttpMethod.Delete,
                null);
        }

        /// <summary>
        /// Create HTTP POST request.
        /// </summary>
        /// <param name="requestUri">Request uri.</param>
        /// <param name="context">Request context.</param>
        /// <returns></returns>
        public static HttpRequest Post(Uri requestUri, string content, HttpRequestContext context)
        {
            return new HttpRequest(
                context,
                requestUri,
                HttpMethod.Post,
                content);
        }

        /// <summary>
        /// Create HTTP PATCH request.
        /// </summary>
        /// <param name="requestUri">Request uri.</param>
        /// <param name="context">Request context.</param>
        /// <returns></returns>
        public static HttpRequest Patch(Uri requestUri, string content, HttpRequestContext context)
        {
            return new HttpRequest(
                context,
                requestUri,
                HttpRequest.PATCH,
                content);
        }

        /// <summary>
        /// Create HttpContent.
        /// </summary>
        /// <param name="value">Content value.</param>
        /// <returns></returns>
        private HttpContent CreateContent(string value)
        {
            value = value ?? string.Empty;
            return new StringContent(
                value,
                Encoding.UTF8,
                "application/json");
        }
    }
}
