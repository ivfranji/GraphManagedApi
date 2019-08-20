namespace Microsoft.Graph.Exchange
{
    using System;
    using System.Net;
    using Microsoft.Graph.CoreHttp;
    using Microsoft.Graph.CoreAuth;
    using Microsoft.Graph.CoreJson;
    using Microsoft.Graph.Identities;

    /// <summary>
    /// Exchange service context.
    /// </summary>
    public class ExchangeServiceContext
    {
        /// <summary>
        /// Json converter.
        /// </summary>
        private Converter jsonConverter;

        /// <summary>
        /// Build exchange service context.
        /// </summary>
        /// <param name="authorizationProvider">Authorization provider.</param>
        /// <param name="userAgent">User agent.</param>
        /// <param name="beta">Use beta endpoint.</param>
        public ExchangeServiceContext(IAuthorizationProvider authorizationProvider, string userAgent = "", bool beta = false)
        {
            this.AuthorizationProvider =
                authorizationProvider ?? throw new ArgumentNullException(nameof(authorizationProvider));

            this.UserAgent = userAgent;
            this.jsonConverter = new Converter();
            this.Beta = beta;
        }

        /// <summary>
        /// Authorization provider.
        /// </summary>
        public IAuthorizationProvider AuthorizationProvider { get; }

        /// <summary>
        /// User agent.
        /// </summary>
        public string UserAgent { get; }

        /// <summary>
        /// Use beta endpoint.
        /// </summary>
        public bool Beta { get; }

        /// <summary>
        /// Web proxy to be used.
        /// </summary>
        public IWebProxy WebProxy
        {
            set
            {
                HttpClientFactory.SetProxyServer(value);
            }
        }

        /// <summary>
        /// Create <see cref="ExchangeService"/> for specified user.
        /// </summary>
        /// <param name="user">User identifier.</param>
        /// <returns></returns>
        public ExchangeService this[string user]
        {
            get { return this[new UserIdentity(user)]; }
        }

        /// <summary>
        /// Create <see cref="ExchangeService"/> for specific identity.
        /// </summary>
        /// <param name="identity">Identity - user or group.</param>
        /// <returns></returns>
        public ExchangeService this[IGraphIdentity identity]
        {
            get
            {
                return new ExchangeService(
                    identity, 
                    this.AuthorizationProvider, 
                    this.jsonConverter,
                    this.UserAgent,
                    this.Beta);
            }
        }
    }
}