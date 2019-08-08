namespace Microsoft.Graph.CoreHttp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Graph.Logging;

    /// <summary>
    /// Log writer http handler. Writes data to log.
    /// </summary>
    internal class HttpLogWriterHandler : DelegatingHandler
    {
        /// <summary>
        /// Send request and retrieve response.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpRequestContext ctx = request.GetHttpRequestContext();
            if (ctx.LogWriter.LoggingEnabled &&
                ctx.LogWriter.LogFlag != LogFlag.None)
            {
                await this.LogRequest(
                    ctx,
                    request);

                var httpResponseMessage = await base.SendAsync(
                    request,
                    cancellationToken);

                await this.LogResponse(
                    ctx,
                    httpResponseMessage);

                return httpResponseMessage;
            }
            else
            {
                return await base.SendAsync(request, cancellationToken);
            }
        }

        /// <summary>
        /// Log request.
        /// </summary>
        /// <param name="requestContext">Request context.</param>
        /// <param name="requestMessage">Request message.</param>
        /// <returns></returns>
        private async Task LogRequest(HttpRequestContext requestContext, HttpRequestMessage requestMessage)
        {
            if ((requestContext.LogWriter.LogFlag &
                 LogFlag.Request) == LogFlag.Request)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"{requestMessage.Method.Method} {requestMessage.RequestUri}");

                if (null != requestMessage.Content)
                {
                    string requestContent = await requestMessage.Content.ReadAsStringAsync();
                    sb.AppendLine(requestContent);
                }

                await this.Log(
                    requestContext,
                    LogFlag.Request,
                    sb);
            }

            await this.LogRequestHeaders(requestContext, requestMessage);
        }

        /// <summary>
        /// Log request headers.
        /// </summary>
        /// <param name="requestContext">Request context.</param>
        /// <param name="requestMessage">Request message.</param>
        /// <returns></returns>
        private async Task LogRequestHeaders(HttpRequestContext requestContext, HttpRequestMessage requestMessage)
        {
            if ((requestContext.LogWriter.LogFlag &
                 LogFlag.RequestHeaders) == LogFlag.RequestHeaders)
            {
                IDictionary<string, string> requestHeaders = new Dictionary<string, string>();
                foreach (KeyValuePair<string, IEnumerable<string>> header in requestMessage.Headers)
                {
                    requestHeaders.Add(
                        header.Key,
                        this.FormatHttpHeaderValue(header.Value));
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Join(
                    Environment.NewLine,
                    requestHeaders.Select(x => x.Key + ":" + x.Value)));

                await this.Log(
                    requestContext,
                    LogFlag.RequestHeaders,
                    sb);
            }
        }

        /// <summary>
        /// Log response.
        /// </summary>
        /// <param name="requestContext">Request context.</param>
        /// <param name="responseMessage">Response message.</param>
        /// <returns></returns>
        private async Task LogResponse(HttpRequestContext requestContext, HttpResponseMessage responseMessage)
        {
            if ((requestContext.LogWriter.LogFlag &
                 LogFlag.Response) == LogFlag.Response)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Response content empty");
                if (responseMessage.Content != null)
                {
                    sb.Clear();
                    sb.AppendLine(
                        await responseMessage.Content.ReadAsStringAsync());
                }

                await this.Log(
                    requestContext,
                    LogFlag.Response,
                    sb);
            }

            await this.LogResponseHeaders(requestContext, responseMessage);
        }

        /// <summary>
        /// Log response headers.
        /// </summary>
        /// <param name="requestContext">Request context.</param>
        /// <param name="responseMessage">Response message.</param>
        /// <returns></returns>
        private async Task LogResponseHeaders(HttpRequestContext requestContext, HttpResponseMessage responseMessage)
        {
            StringBuilder sb = new StringBuilder();
            if ((requestContext.LogWriter.LogFlag &
                 LogFlag.ResponseHeaders) == LogFlag.ResponseHeaders)
            {
                sb.AppendLine(string.Join(
                    Environment.NewLine,
                    responseMessage.Headers.Select(
                        x => x.Key + ":" + this.FormatHttpHeaderValue(x.Value))));

                await this.Log(
                    requestContext,
                    LogFlag.ResponseHeaders,
                    sb);
            }
        }

        /// <summary>
        /// Log message to <see cref="ILogWriter"/>
        /// </summary>
        /// <param name="requestContext">Request context.</param>
        /// <param name="logFlag">Log flag.</param>
        /// <param name="message">Log message.</param>
        /// <returns></returns>
        private async Task Log(HttpRequestContext requestContext, LogFlag logFlag, StringBuilder message)
        {
            message.AppendLine("");
            await requestContext.LogWriter.Log(
                logFlag.ToString(),
                message.ToString());
        }

        /// <summary>
        /// Joins header values and separate them by comma ','
        /// </summary>
        /// <param name="value">Header value.</param>
        /// <returns></returns>
        private string FormatHttpHeaderValue(IEnumerable<string> value)
        {
            if (null == value)
            {
                return string.Empty;
            }

            return string.Join(", ", value);
        }
    }
}
