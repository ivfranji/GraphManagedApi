namespace Microsoft.Graph.Search
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Graph.ChangeTracking;
    using Microsoft.Graph.Exchange;
    using Microsoft.Graph.Utilities;

    /// <summary>
    /// Represents search filter.
    /// </summary>
    public abstract class SearchFilter : IUrlQuery
    {
        /// <summary>
        /// Search format provider.
        /// </summary>
        private static SearchFormatProvider searchFormatProvider = new SearchFormatProvider();

        /// <summary>
        /// Filter prefix.
        /// </summary>
        private const string FilterPrefix = "$filter=";

        /// <summary>
        /// Create new instance of <see cref="SearchFilter"/>
        /// </summary>
        /// <param name="filterOperator">Filter operator.</param>
        protected SearchFilter(FilterOperator filterOperator)
        {
            this.FilterOperator = filterOperator;
        }

        /// <summary>
        /// Search format provider.
        /// </summary>
        protected SearchFormatProvider SearchFormatProvider
        {
            get { return SearchFilter.searchFormatProvider; }
        }

        /// <summary>
        /// Format filter.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="filterOperator"></param>
        /// <param name="propertyDefinition"></param>
        /// <returns></returns>
        protected string FormatFilter(object value, FilterOperator filterOperator, PropertyDefinition propertyDefinition)
        {
            ISearchFilterFormatter formatter = this.SearchFormatProvider[propertyDefinition.Type.FullName];
            return formatter.Format(
                value,
                filterOperator,
                propertyDefinition);
        }

        /// <inheritdoc cref="IQuery.Query"/>
        public string Query
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                this.ToString(stringBuilder);

                string filter = stringBuilder.ToString();
                if (filter.StartsWith(SearchFilter.FilterPrefix))
                {
                    return filter;
                }

                return $"{SearchFilter.FilterPrefix}{filter}";
            }
        }

        /// <inheritdoc cref="Search.FilterOperator"/>
        public FilterOperator FilterOperator { get; }

        /// <summary>
        /// Validate formatting supported for particular object.
        /// </summary>
        /// <param name="value"></param>
        protected void ValidateFormattingSupportedOrThrow(object value, PropertyDefinition propertyDefinition)
        {
            value.ThrowIfNull(nameof(value));
            if (value is string || propertyDefinition.Type.IsInstanceOfType(value))
            {
                return;
            }

            throw new ArgumentException($"Cannot format type '{value.GetType().FullName}' with definition '{propertyDefinition.Type.FullName}'.");
        }

        #region Filter implementations

        /// <summary>
        /// Simple propertyname / propertyvalue matching.
        /// </summary>
        public abstract class SimplePropertyMatchingFilter : SearchFilter
        {
            /// <summary>
            /// Create new instance of <see cref="SearchFilter.SimplePropertyMatchingFilter"/>
            /// </summary>
            /// <param name="filterOperator">Filter operator.</param>
            /// <param name="propertyValue">Property value.</param>
            /// <param name="propertyDefinition">Property name.</param>
            protected SimplePropertyMatchingFilter(PropertyDefinition propertyDefinition, object propertyValue, FilterOperator filterOperator)
                : base(filterOperator)
            {
                ArgumentValidator.ThrowIfNull(
                    propertyDefinition,
                    nameof(propertyDefinition));

                ArgumentValidator.ThrowIfNull(
                    propertyValue,
                    nameof(propertyValue));

                this.ValidateFormattingSupportedOrThrow(propertyValue, propertyDefinition);
                this.PropertyDefinition = propertyDefinition;
                this.PropertyValue = propertyValue;
            }

            /// <summary>
            /// Property name.
            /// </summary>
            public PropertyDefinition PropertyDefinition { get; }

            /// <summary>
            /// Property value.
            public object PropertyValue { get; }

            /// <inheritdoc cref="SearchFilter.ToString(StringBuilder)"/>
            protected internal sealed override void ToString(StringBuilder sb)
            {
                sb.Append(this.FormatFilter(
                    this.PropertyValue,
                    this.FilterOperator,
                    this.PropertyDefinition));
            }
        }

        /// <summary>
        /// Filter collection.
        /// </summary>
        public abstract class FilterCollection : SearchFilter
        {
            /// <summary>
            /// List of search filters.
            /// </summary>
            private readonly List<SearchFilter> filters;

            /// <summary>
            /// Create new instance of <see cref="SearchFilter.FilterCollection"/>
            /// </summary>
            /// <param name="filterOperator"></param>
            protected FilterCollection(FilterOperator filterOperator) 
                : base(filterOperator)
            {
                this.filters = new List<SearchFilter>();
            }

            /// <summary>
            /// Filter collection empty.
            /// </summary>
            public bool CollectionEmpty
            {
                get { return this.filters.Count == 0; }
            }

            /// <summary>
            /// Add filter to collection.
            /// </summary>
            /// <param name="searchFilter"></param>
            public void AddFilter(SearchFilter searchFilter)
            {
                searchFilter.ThrowIfNull(nameof(searchFilter));
                if (!this.filters.Contains(searchFilter))
                {
                    this.filters.Add(searchFilter);
                }
            }

            /// <inheritdoc cref="SearchFilter.ToString(StringBuilder)"/>
            protected internal sealed override void ToString(StringBuilder sb)
            {
                for (int i = 0; i < this.filters.Count; i++)
                {
                    if (i + 1 == this.filters.Count)
                    {
                        this.filters[i].ToString(sb);
                    }
                    else
                    {
                        this.filters[i].ToString(sb);
                        sb.Append($" {this.FilterOperator} ");
                    }
                }
            }
        }

        /// <summary>
        /// Is equal to filter.
        /// </summary>
        public sealed class IsEqualTo : SimplePropertyMatchingFilter
        {
            /// <summary>
            /// Create new instance of <see cref="SearchFilter.IsEqualTo"/>.
            /// </summary>
            /// <param name="propertyDefinition">Property name.</param>
            /// <param name="propertyValue">Property value.</param>
            public IsEqualTo(PropertyDefinition propertyDefinition, object propertyValue)
                : base(propertyDefinition, propertyValue, FilterOperator.eq)
            {
            }
        }

        /// <summary>
        /// Is greater than filter.
        /// </summary>
        public sealed class IsGreaterThan : SimplePropertyMatchingFilter
        {
            /// <summary>
            /// Create new instance of <see cref="SearchFilter.IsGreaterThan"/>
            /// </summary>
            /// <param name="propertyDefinition">Property name.</param>
            /// <param name="propertyValue">Property value.</param>
            public IsGreaterThan(PropertyDefinition propertyDefinition, object propertyValue)
                : base(propertyDefinition, propertyValue, FilterOperator.gt)
            {
            }
        }

        /// <summary>
        /// Is greater than or equal to filter.
        /// </summary>
        public sealed class IsGreaterThanOrEqualTo : SimplePropertyMatchingFilter
        {
            /// <summary>
            /// Create new instance of <see cref="SearchFilter.IsGreaterThanOrEqualTo"/>
            /// </summary>
            /// <param name="propertyDefinition">Property name.</param>
            /// <param name="propertyValue">Property value.</param>
            public IsGreaterThanOrEqualTo(PropertyDefinition propertyDefinition, object propertyValue)
                : base(propertyDefinition, propertyValue, FilterOperator.ge)
            {
            }
        }

        /// <summary>
        /// Is less than filter.
        /// </summary>
        public sealed class IsLessThan : SimplePropertyMatchingFilter
        {
            /// <summary>
            /// Create new instance of <see cref="SearchFilter.IsLessThan"/>
            /// </summary>
            /// <param name="propertyDefinition">Property name.</param>
            /// <param name="propertyValue">Property value.</param>
            public IsLessThan(PropertyDefinition propertyDefinition, object propertyValue)
                : base(propertyDefinition, propertyValue, FilterOperator.lt)
            {
            }
        }

        /// <summary>
        /// Is less than or equal to filter.
        /// </summary>
        public sealed class IsLessThanOrEqualTo : SimplePropertyMatchingFilter
        {
            /// <summary>
            /// Create new instance of <see cref="SearchFilter.IsLessThanOrEqualTo"/>
            /// </summary>
            /// <param name="propertyDefinition">Property name.</param>
            /// <param name="propertyValue">Property value.</param>
            public IsLessThanOrEqualTo(PropertyDefinition propertyDefinition, object propertyValue)
                : base(propertyDefinition, propertyValue, FilterOperator.le)
            {
            }
        }

        /// <summary>
        /// Not equal to filter.
        /// </summary>
        public sealed class NotEqualTo : SimplePropertyMatchingFilter
        {
            /// <summary>
            /// Create new instance of <see cref="SearchFilter.NotEqualTo"/>
            /// </summary>
            /// <param name="propertyDefinition">Property name.</param>
            /// <param name="propertyValue">Property value.</param>
            public NotEqualTo(PropertyDefinition propertyDefinition, object propertyValue)
                : base(propertyDefinition, propertyValue, FilterOperator.ne)
            {
            }
        }

        /// <summary>
        /// Create 'and' filter collection.
        /// </summary>
        public sealed class AndFilterCollection : FilterCollection
        {
            /// <summary>
            /// Create new instance of <see cref="SearchFilter.AndFilterCollection"/>
            /// </summary>
            public AndFilterCollection() 
                : base(FilterOperator.and)
            {
            }
        }

        /// <summary>
        /// Create 'or' filter collection.
        /// </summary>
        public sealed class OrFilterCollection : FilterCollection
        {
            /// <summary>
            /// Create new instance of <see cref="SearchFilter.OrFilterCollection"/>
            /// </summary>
            public OrFilterCollection()
                : base(FilterOperator.or)
            {
            }
        }

        /// <summary>
        /// Extended property filter.
        /// </summary>
        public class ExtendedPropertyFilter : SearchFilter
        {
            public ExtendedPropertyFilter(FilterOperator filterOperator, ExtendedPropertyDefinition extendedProperty, string value)
                : base(filterOperator)
            {
                ArgumentValidator.ThrowIfNull(extendedProperty, nameof(extendedProperty));
                this.ExtendedProperty = extendedProperty;
                this.Value = value;
            }

            /// <summary>
            /// Value.
            /// </summary>
            public string Value { get; set; }

            /// <summary>
            /// Extended property.
            /// </summary>
            public ExtendedPropertyDefinition ExtendedProperty { get; }

            /// <inheritdoc cref="SearchFilter.ToString(StringBuilder)"/>
            protected internal sealed override void ToString(StringBuilder sb)
            {
                sb.AppendFormat("{0}/Any(ep: ep/id {1} '{2}' and ep/value eq '{3}')",
                    this.ExtendedProperty.MapiPropertyValueType,
                    this.FilterOperator,
                    this.ExtendedProperty.Definition,
                    this.Value);
            }
        }

        #endregion

        /// <summary>
        /// Create filter string.
        /// </summary>
        /// <param name="sb"></param>
        protected internal abstract void ToString(StringBuilder sb);

        /// <summary>
        /// Url query.
        /// </summary>
        /// <returns></returns>
        public string GetUrlQuery()
        {
            return this.Query;
        }
    }
}
