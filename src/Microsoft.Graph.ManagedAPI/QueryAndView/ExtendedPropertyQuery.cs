namespace Microsoft.Graph.Exchange
{
    using System.Text;
    using Microsoft.Graph.Search;

    /// <summary>
    /// Extended properties query.
    /// </summary>
    internal class ExtendedPropertyQuery : IUrlQuery
    {
        /// <summary>
        /// Expand prefix.
        /// </summary>
        private const string ExpandPrefix = "$expand=";

        /// <summary>
        /// Extended properties query.
        /// </summary>
        /// <param name="singleValueExtendedPropertyFilter">Single value extended property filter.</param>
        /// <param name="multiValueExtendedPropertyFilter">Multi value extended property filter.</param>
        internal ExtendedPropertyQuery(SearchFilter singleValueExtendedPropertyFilter, SearchFilter multiValueExtendedPropertyFilter)
        {
            this.SingleValueExtendedPropertyFilter = singleValueExtendedPropertyFilter;
            this.MultiValueExtendedPropertyFilter = multiValueExtendedPropertyFilter;
        }

        /// <summary>
        /// Single value extended filter properties.
        /// </summary>
        internal SearchFilter SingleValueExtendedPropertyFilter { get; }
        
        /// <summary>
        /// Multi value extended filter properties.
        /// </summary>
        internal SearchFilter MultiValueExtendedPropertyFilter { get; }
        
        /// <summary>
        /// Get url query.
        /// </summary>
        /// <returns></returns>
        public string GetUrlQuery()
        {
            StringBuilder filterBuilder = new StringBuilder();
            filterBuilder.Append(ExtendedPropertyQuery.ExpandPrefix);
            if (null != this.SingleValueExtendedPropertyFilter)
            {
                filterBuilder.Append(this.GetFilter(
                    this.SingleValueExtendedPropertyFilter,
                    MapiPropertyValueType.SingleValueExtendedProperties));

                if (null != this.MultiValueExtendedPropertyFilter)
                {
                    filterBuilder.Append(",");
                }
            }

            if (null != this.MultiValueExtendedPropertyFilter)
            {
                filterBuilder.Append(this.GetFilter(
                    this.MultiValueExtendedPropertyFilter,
                    MapiPropertyValueType.MultiValueExtendedProperties));
            }

            return filterBuilder.ToString();
        }

        /// <summary>
        /// ToString impl.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.GetUrlQuery();
        }

        /// <summary>
        /// Get filter.
        /// </summary>
        /// <param name="searchFilter">Search filter.</param>
        /// <param name="mapiPropertyValueType">Mapi property type.</param>
        /// <returns></returns>
        private string GetFilter(SearchFilter searchFilter, MapiPropertyValueType mapiPropertyValueType)
        {
            return $"{mapiPropertyValueType}({searchFilter.Query})";
        }
    }
}
