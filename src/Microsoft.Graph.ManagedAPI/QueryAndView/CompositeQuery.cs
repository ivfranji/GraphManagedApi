namespace Microsoft.Graph.Exchange
{
    using System.Text;
    using Microsoft.Graph.Utilities;

    /// <summary>
    /// Composite query, consist of one of more <see cref="IUrlQuery"/>
    /// </summary>
    internal class CompositeQuery : IUrlQuery
    {
        /// <summary>
        /// Queries.
        /// </summary>
        private IUrlQuery[] urlQueries;

        /// <summary>
        /// Create new instance of <see cref="CompositeQuery"/>
        /// </summary>
        /// <param name="urlQueries">Url queries.</param>
        internal CompositeQuery(IUrlQuery[] urlQueries)
        {
            urlQueries.ThrowIfNullOrEmptyArray(nameof(urlQueries));
            this.urlQueries = urlQueries;
        }

        /// <summary>
        /// Get url query.
        /// </summary>
        /// <returns></returns>
        public string GetUrlQuery()
        {
            if (this.urlQueries.Length == 1)
            {
                return this.urlQueries[0].GetUrlQuery();
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this.urlQueries.Length; i++)
            {
                if (i + 1 == this.urlQueries.Length)
                {
                    sb.Append(this.urlQueries[i].GetUrlQuery());
                }
                else
                {
                    sb.AppendFormat("{0}&", this.urlQueries[i].GetUrlQuery());
                }
            }

            return sb.ToString();
        }
    }
}
