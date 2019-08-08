namespace Microsoft.Graph.Exchange
{
    using System;

    /// <summary>
    /// Type of the mapi property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    internal class MapiPropertyTypeValueAttribute : Attribute
    {
        /// <summary>
        /// Create new instance of <see cref="MapiPropertyTypeValueAttribute"/>
        /// </summary>
        /// <param name="mapiPropertyValueType">Property value type.</param>
        public MapiPropertyTypeValueAttribute(MapiPropertyValueType mapiPropertyValueType)
        {
            this.MapiPropertyValueType = mapiPropertyValueType;
        }

        /// <summary>
        /// Property value type.
        /// </summary>
        internal MapiPropertyValueType MapiPropertyValueType { get; }
    }
}
