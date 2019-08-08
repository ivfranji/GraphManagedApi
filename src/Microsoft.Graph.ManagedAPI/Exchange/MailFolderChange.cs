namespace Microsoft.Graph.Exchange
{
    /// <summary>
    /// Mail folder change.
    /// </summary>
    public class MailFolderChange : SyncChange<MailFolder>
    {
        /// <summary>
        /// Create new instance of <see cref="MailFolderChange"/>
        /// </summary>
        /// <param name="item">Item.</param>
        public MailFolderChange(MailFolder item) 
            : base(item)
        {
        }
    }
}
