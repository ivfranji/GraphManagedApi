namespace Microsoft.Graph.Exchange
{
    using System;

    /// <summary>
    /// Message change.
    /// </summary>
    public class MessageChange : SyncChange<Message>
    {
        /// <summary>
        /// Create new instance of <see cref="MessageChange"/>
        /// </summary>
        /// <param name="item">Message item.</param>
        public MessageChange(Message item) 
            : base(item)
        {
            // created datetime cannot be update and if not
            // returned this is considered as updated item 
            // for example read flag change.
            if (this.ChangeType != ChangeType.Deleted &&
                item.CreatedDateTime == default(DateTimeOffset))
            {
                this.ChangeType = ChangeType.Updated;
            }
        }
    }
}