namespace Microsoft.Graph.Exchange
{
    using System.Text;
    using Microsoft.Graph.ChangeTracking;
    using Microsoft.Graph.Utilities;

    /// <summary>
    /// Select query.
    /// </summary>
    public class SelectQuery : IUrlQuery
    {
        /// <summary>
        /// Prefix.
        /// </summary>
        private const string SelectPrefix = "$select=";

        /// <summary>
        /// Create new instance of <see cref="SelectQuery"/>
        /// </summary>
        /// <param name="property">Property to retrieve.</param>
        public SelectQuery(PropertyDefinition property)
            : this(new[] { property })
        {
        }

        /// <summary>
        /// Create new instance of <see cref="ISelectQuery"/>
        /// </summary>
        /// <param name="properties">Properties to fetch.</param>
        public SelectQuery(PropertyDefinition[] properties)
        {
            properties.ThrowIfNullOrEmptyArray(nameof(properties));
            this.Properties = properties;
        }

        /// <inheritdoc cref="ISelectQuery.Properties"/>
        public PropertyDefinition[] Properties { get; }

        /// <summary>
        /// Get Url query.
        /// </summary>
        /// <returns></returns>
        public string GetUrlQuery()
        {
            StringBuilder sb = new StringBuilder(SelectQuery.SelectPrefix);
            for (int i = 0; i < this.Properties.Length; i++)
            {
                if (i + 1 == this.Properties.Length)
                {
                    sb.Append(this.Properties[i].Name);
                }
                else
                {
                    sb.Append(this.Properties[i].Name);
                    sb.Append(",");
                }
            }

            return sb.ToString();
        }
    }
}
