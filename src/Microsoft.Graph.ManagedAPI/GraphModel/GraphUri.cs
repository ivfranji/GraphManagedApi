namespace Microsoft.Graph.GraphModel
{
    using System;
    using Microsoft.Graph.Exchange;
    using Microsoft.Graph.CoreHttp;
    using Microsoft.Graph.Identities;
    using Utilities;

    /// <summary>
    /// Represents Graph Uri.
    /// </summary>
    internal class GraphUri
    {
        /// <summary>
        /// Rest uri.
        /// </summary>
        private HttpRestUri restUri;

        /// <summary>
        /// Beta relative path.
        /// </summary>
        private const string betaPath = "beta";

        /// <summary>
        /// Prod relative path.
        /// </summary>
        private const string prodPath = "v1.0";

        /// <summary>
        /// Base graph uri.
        /// </summary>
        private const string baseGraphUri = "https://graph.microsoft.com";

        /// <summary>
        /// Create new instance of <see cref="GraphUri"/>
        /// </summary>
        /// <param name="identity">Identity.</param>
        /// <param name="subEntity">Sub entity.</param>
        /// <param name="beta">Connect to beta endpoint.</param>
        internal GraphUri(IGraphIdentity identity, EntityPath subEntity, bool beta)
        {
            identity.ThrowIfNull(nameof(identity));
            subEntity.ThrowIfNull(nameof(subEntity));

            this.IsBeta = beta;
            this.Identity = identity;
            this.restUri = new HttpRestUri(
                $"{GraphUri.baseGraphUri}/{this.GetServiceInstance()}/{this.Identity.GetSubEntityFullPath(subEntity)}");
        }

        /// <summary>
        /// Create graph uri with method on the entity.
        /// </summary>
        /// <param name="identity">Identity.</param>
        /// <param name="subEntity">Sub entity.</param>
        /// <param name="method">Method.</param>
        /// <param name="beta">Is beta.</param>
        internal GraphUri(IGraphIdentity identity, EntityPath subEntity, string method, bool beta)
        {
            identity.ThrowIfNull(nameof(identity));
            subEntity.ThrowIfNull(nameof(subEntity));
            method.ThrowIfNullOrEmpty(nameof(method));

            this.IsBeta = beta;
            this.Identity = identity;
            this.restUri = new HttpRestUri(
                $"{GraphUri.baseGraphUri}/{this.GetServiceInstance()}/{this.Identity.GetSubEntityFullPath(subEntity)}/{method}");
        }

        /// <summary>
        /// Indicate if this is beta endpoint.
        /// </summary>
        internal bool IsBeta { get; }

        /// <summary>
        /// Graph identity.
        /// </summary>
        internal IGraphIdentity Identity { get; }

        /// <summary>
        /// Indicate if this is 'me' entity.
        /// </summary>
        internal bool IsMeEntity
        {
            get
            {
                return this.Identity.Id.Equals(
                    "me",
                    StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Cast conversion from <see cref="GraphUri"/> to <see cref="Uri"/>
        /// </summary>
        /// <param name="graphUri">Rest uri.</param>
        public static implicit operator Uri(GraphUri graphUri)
        {
            return graphUri.restUri;
        }

        /// <summary>
        /// Adds segment.
        /// </summary>
        /// <param name="segment"></param>
        public void AddSegment(string segment)
        {
            this.restUri.AddSegment(segment);
        }

        /// <summary>
        /// Add query to request.
        /// </summary>
        /// <param name="urlQuery"></param>
        public void AddQuery(IUrlQuery urlQuery)
        {
            urlQuery.ThrowIfNull(nameof(urlQuery));
            this.restUri.AddQuery(urlQuery.GetUrlQuery());
        }

        /// <summary>
        /// Get service instance - beta or prod.
        /// </summary>
        /// <returns></returns>
        private string GetServiceInstance()
        {
            return this.IsBeta
                ? GraphUri.betaPath
                : GraphUri.prodPath;
        }

        /// <summary>
        /// Validate identity.
        /// </summary>
        private void ValidateIdentity()
        {
            if (this.IsMeEntity &&
                this.restUri.TryGetSegment(2, out string meSegment))
            {
                if (this.Identity.Id.Equals(meSegment, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            if (this.restUri.TryGetSegment(3, out string userSegment) &&
                this.Identity.Id.Equals(userSegment, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            throw new ArgumentException($"Couldn't validate identity. Expected: '{this.Identity.Id}'. Actual: '{userSegment}'.");
        }
    }
}
