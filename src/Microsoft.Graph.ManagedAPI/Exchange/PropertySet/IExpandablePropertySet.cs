namespace Microsoft.Graph.Exchange
{
    using Microsoft.Graph.ChangeTracking;

    /// <summary>
    /// Defines expandable property.
    /// </summary>
    internal interface IExpandablePropertySet : IUrlQuery
    {
        /// <summary>
        /// Value of expandable property.
        /// </summary>
        string Value { get; }

        /// <summary>
        /// Add property definition.
        /// </summary>
        /// <param name="propertyDefinition">Property definition.</param>
        void Add(PropertyDefinition propertyDefinition);

        /// <summary>
        /// Add extended property definition.
        /// </summary>
        /// <param name="extendedPropertyDefinition">Extended property definition.</param>
        void Add(ExtendedPropertyDefinition extendedPropertyDefinition);
    }
}