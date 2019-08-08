namespace Microsoft.Graph.Exchange
{
    /// <summary>
    /// Mail folder property set.
    /// </summary>
    public class MailFolderPropertySet : PropertySet
    {
        /// <summary>
        /// Create new instance of <see cref="MailFolderPropertySet"/>
        /// </summary>
        internal MailFolderPropertySet()
            : base(new MailFolderObjectSchema())
        {
            this.firstClassProperties.Add(MailFolderObjectSchema.ChildFolderCount);
            this.firstClassProperties.Add(MailFolderObjectSchema.DisplayName);
            this.firstClassProperties.Add(MailFolderObjectSchema.ParentFolderId);
        }
    }
}