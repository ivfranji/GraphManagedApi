namespace Microsoft.Graph.Exchange
{
    /// <summary>
    /// Message view.
    /// </summary>
    public class MessageView : TypedView<Message, MessageObjectSchema>
    {
        /// <summary>
        /// Create new instance of <see cref="MessageView"/> with page size 10.
        /// </summary>
        public MessageView()
            : this(10, 0)
        {
        }

        /// <summary>
        /// Create new instance of <see cref="MessageView"/> with page size 1 and offset 0.
        /// </summary>
        /// <param name="pageSize">Page size.</param>
        public MessageView(int pageSize)
            : this(pageSize, 0)
        {
        }

        /// <summary>
        /// Create new instance of <see cref="MessageView"/> with specified page size and offset.
        /// </summary>
        /// <param name="pageSize">Page size.</param>
        /// <param name="offset">Offset.</param>
        public MessageView(int pageSize, int offset)
            : base(
                pageSize, 
                offset, 
                MessageObjectSchema.IsRead, 
                MessageObjectSchema.Subject, 
                MessageObjectSchema.ParentFolderId, 
                MessageObjectSchema.CreatedDateTime)
        {
        }
    }
}