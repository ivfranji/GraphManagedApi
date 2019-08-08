namespace Microsoft.Graph.Search
{
    using Microsoft.Graph.ChangeTracking;
    using Microsoft.Graph.Exchange;

    /// <summary>
    /// Filter formatter.
    /// </summary>
    interface ISearchFilterFormatter
    {
        /// <summary>
        /// Format object to filterable string.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="filterOperator">Filter operator.</param>
        /// <param name="propertyDefinition">Property definition.</param>
        /// <returns></returns>
        string Format(object obj, FilterOperator filterOperator, PropertyDefinition propertyDefinition);

        /// <summary>
        /// Type of filter it supports.
        /// </summary>
        string Type { get; }
    }
}
