namespace Microsoft.Graph.CoreHttp
{
    using System;
    using Microsoft.Graph.Logging;
    using Microsoft.Graph.CoreAuth;

    /// <summary>
    /// Http request context.
    /// </summary>
    internal class HttpRequestContext
    {
        /// <summary>
        /// Authorization provider.
        /// </summary>
        private IAuthorizationProvider authorizationProvider;

        /// <summary>
        /// Log writer.
        /// </summary>
        private ILogWriter logWriter;

        /// <summary>
        /// Create new instance of <see cref="HttpRequestContext"/>.
        /// </summary>
        internal HttpRequestContext()
        {
        }

        /// <summary>
        /// Authorization provider.
        /// </summary>
        internal IAuthorizationProvider AuthorizationProvider
        {
            get
            {
                if (null == this.authorizationProvider)
                {
                    throw new ArgumentNullException(nameof(this.authorizationProvider));
                }

                return this.authorizationProvider;
            }

            set { this.authorizationProvider = value; }
        }

        /// <summary>
        /// Log writer associated with context.
        /// </summary>
        internal ILogWriter LogWriter
        {
            get
            {
                if (null == this.logWriter)
                {
                    return ConsoleLogWriter.Instance.Value;
                }

                return this.logWriter;
            }

            set { this.logWriter = value; }
        }

        /// <summary>
        /// Http extension handler.
        /// </summary>
        internal IHttpExtensionHandler HttpExtensionHandler { get; set; }
    }
}
