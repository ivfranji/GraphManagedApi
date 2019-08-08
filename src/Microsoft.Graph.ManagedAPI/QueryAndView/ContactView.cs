namespace Microsoft.Graph.Exchange
{
    /// <summary>
    /// Event view.
    /// </summary>
    public class ContactView : TypedView<Contact, ContactObjectSchema>
    {
        /// <summary>
        /// Create new instance of <see cref="ContactView"/> with page size of 10.
        /// </summary>
        public ContactView()
            : this(10)
        {
        }

        /// <summary>
        /// Create new instance of <see cref="ContactView"/>
        /// </summary>
        /// <param name="pageSize">Page size.</param>
        public ContactView(int pageSize)
            : this(pageSize, 0)
        {
        }

        /// <summary>
        /// Create new instance of <see cref="ContactView"/>.
        /// </summary>
        /// <param name="pageSize">Page size.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="forceObjectClassFilter">Force object class filter.</param>
        public ContactView(int pageSize, int offset)
            : base(pageSize, offset, ContactObjectSchema.DisplayName, ContactObjectSchema.EmailAddresses)
        {
        }
    }
}