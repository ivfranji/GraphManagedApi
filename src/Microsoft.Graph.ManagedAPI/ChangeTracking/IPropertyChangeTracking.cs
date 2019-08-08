namespace Microsoft.Graph.ChangeTracking
{
    using System.Collections.Generic;

    /// <summary>
    /// Property change tracking.
    /// </summary>
    public interface IPropertyChangeTracking
    {
        /// <summary>
        /// Get changed properties.
        /// </summary>
        /// <returns></returns>
        IEnumerable<PropertyDefinition> GetChangedProperties();

        /// <summary>
        /// Index getter.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns></returns>
        object this[PropertyDefinition key] { get; }

        /// <summary>
        /// Index getter.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns></returns>
        object this[string key] { get; }
    }
}
