namespace Microsoft.Graph.Exchange
{
    /// <summary>
    /// Item property set.
    /// </summary>
    public class MessagePropertySet : PropertySet
    {
        /// <summary>
        /// Create new instance of <see cref="MessagePropertySet"/>
        /// </summary>
        internal MessagePropertySet()
            : base(new MessageObjectSchema())
        {
            this.firstClassProperties.Add(MessageObjectSchema.IsRead);
            this.firstClassProperties.Add(MessageObjectSchema.Subject);
            this.firstClassProperties.Add(MessageObjectSchema.ParentFolderId);
            this.firstClassProperties.Add(OutlookItemObjectSchema.CreatedDateTime);
        }
    }
}