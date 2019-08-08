namespace Microsoft.Graph.Exchange
{
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Graph.ChangeTracking;
    using Microsoft.Graph.Search;
    using Microsoft.Graph.Utilities;

    /// <summary>
    /// Expandable properties.
    /// </summary>
    internal class ExpandablePropertySet : IExpandablePropertySet
    {
        /// <summary>
        /// Expand prefix.
        /// </summary>
        private const string ExpandPrefix = "$expand=";

        /// <summary>
        /// Single value extended property filter collection.
        /// </summary>
        private SearchFilter.OrFilterCollection singleValueExtendedPropertyFilterCollection;

        /// <summary>
        /// Multi value extended property filter collection.
        /// </summary>
        private SearchFilter.OrFilterCollection multiValueExtendedPropertyFilterCollection;

        /// <summary>
        /// Property definition set.
        /// </summary>
        private IList<PropertyDefinition> propertyDefinitionSet;

        /// <summary>
        /// Create new instance of <see cref="ExpandablePropertySet"/>
        /// </summary>
        internal ExpandablePropertySet()
        {
            this.singleValueExtendedPropertyFilterCollection = new SearchFilter.OrFilterCollection();
            this.multiValueExtendedPropertyFilterCollection = new SearchFilter.OrFilterCollection();
            this.propertyDefinitionSet = new List<PropertyDefinition>(3);

        }

        /// <summary>
        /// IExpandable value.
        /// </summary>
        public string Value { get { return this.GetUrlQuery(); } }

        /// <summary>
        /// Add property definition to set.
        /// </summary>
        /// <param name="propertyDefinition"></param>
        public void Add(PropertyDefinition propertyDefinition)
        {
            if (!this.propertyDefinitionSet.Contains(propertyDefinition))
            {
                this.propertyDefinitionSet.Add(propertyDefinition);
            }
        }

        /// <summary>
        /// Add extended property to set.
        /// </summary>
        /// <param name="extendedPropertyDefinition"></param>
        public void Add(ExtendedPropertyDefinition extendedPropertyDefinition)
        {
            extendedPropertyDefinition.ThrowIfNull(nameof(extendedPropertyDefinition));
            switch (extendedPropertyDefinition.MapiPropertyValueType)
            {
                case MapiPropertyValueType.MultiValueExtendedProperties:
                    SearchFilter multiValueExtFilter = new SearchFilter.IsEqualTo(
                        MultiValueLegacyExtendedPropertyObjectSchema.Id,
                        extendedPropertyDefinition.Definition);

                    this.multiValueExtendedPropertyFilterCollection.AddFilter(multiValueExtFilter);
                    break;

                case MapiPropertyValueType.SingleValueExtendedProperties:
                    SearchFilter singleValueExtFilter = new SearchFilter.IsEqualTo(
                        SingleValueLegacyExtendedPropertyObjectSchema.Id,
                        extendedPropertyDefinition.Definition);

                    this.singleValueExtendedPropertyFilterCollection.AddFilter(singleValueExtFilter);
                    break;
            }
        }

        /// <inheritdoc cref="IUrlQuery.GetUrlQuery"/>
        public string GetUrlQuery()
        {
            StringBuilder filterBuilder = new StringBuilder();
            filterBuilder.Append(ExpandablePropertySet.ExpandPrefix);
            if (!this.singleValueExtendedPropertyFilterCollection.CollectionEmpty)
            {
                filterBuilder.Append(this.GetFilter(
                    this.singleValueExtendedPropertyFilterCollection,
                    MapiPropertyValueType.SingleValueExtendedProperties));
            }

            if (!this.multiValueExtendedPropertyFilterCollection.CollectionEmpty)
            {
                this.AppendCommaIfNeeded(filterBuilder);
                filterBuilder.Append(this.GetFilter(
                    this.multiValueExtendedPropertyFilterCollection,
                    MapiPropertyValueType.MultiValueExtendedProperties));

            }

            if (this.propertyDefinitionSet.Count > 0)
            {
                this.AppendCommaIfNeeded(filterBuilder);
                for (int i = 0; i < this.propertyDefinitionSet.Count; i++)
                {
                    if (i + 1 == this.propertyDefinitionSet.Count)
                    {
                        filterBuilder.Append(this.propertyDefinitionSet[i].Name);
                    }
                    else
                    {
                        filterBuilder.Append(this.propertyDefinitionSet[i].Name);
                        this.AppendComma(filterBuilder);
                    }
                }
            }

            return filterBuilder.ToString();
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

        /// <summary>
        /// Append comma to builder if needed.
        /// </summary>
        /// <param name="sb"></param>
        private void AppendCommaIfNeeded(StringBuilder sb)
        {
            if (sb.Length > ExpandablePropertySet.ExpandPrefix.Length)
            {
                this.AppendComma(sb);
            }
        }

        /// <summary>
        /// Append comma to string builder.
        /// </summary>
        /// <param name="sb"></param>
        private void AppendComma(StringBuilder sb)
        {
            sb.Append(",");
        }
    }
}