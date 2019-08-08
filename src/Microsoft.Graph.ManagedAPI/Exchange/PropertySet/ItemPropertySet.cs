namespace Microsoft.Graph.Exchange
{
    using Microsoft.Graph.ChangeTracking;

    /// <summary>
    /// Outlook item property set.
    /// </summary>
    public class ItemPropertySet : PropertySet
    {
        /// <summary>
        /// Create new instance of <see cref="ItemPropertySet"/>
        /// </summary>
        /// <param name="objectSchema">Object schema.</param>
        /// <param name="properties"></param>
        internal ItemPropertySet(ObjectSchema objectSchema, params PropertyDefinition[] properties)
            : base(objectSchema)
        {
            if (null != properties &&
                properties.Length > 0)
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    this.firstClassProperties.Add(properties[i]);
                }
            }
        }
    }
}