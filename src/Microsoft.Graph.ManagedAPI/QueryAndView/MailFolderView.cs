namespace Microsoft.Graph.Exchange
{
    using System;

    /// <summary>
    /// Mail folder view.
    /// </summary>
    public class MailFolderView : ViewBase
    {
        /// <summary>
        /// View type.
        /// </summary>
        private static readonly Type viewType = typeof(MailFolder);

        /// <summary>
        /// Create new instance of <see cref="MailFolderView"/> with page of 10.
        /// </summary>
        public MailFolderView()
            : this(10, 0)
        {
        }

        /// <summary>
        /// Create new instance of <see cref="MailFolderView"/> with provided page size.
        /// </summary>
        /// <param name="pageSize">Page size.</param>
        public MailFolderView(int pageSize)
            : this(pageSize, 0)
        {
        }

        /// <summary>
        /// Create new instance of <see cref="MailFolderView"/> with specified page and offset.
        /// </summary>
        /// <param name="pageSize">Page size.</param>
        /// <param name="offset">Page offset.</param>
        public MailFolderView(int pageSize, int offset)
            : base(pageSize, offset)
        {
            this.PropertySet = new MailFolderPropertySet();
        }

        /// <summary>
        /// View type.
        /// </summary>
        internal override Type ViewType
        {
            get { return MailFolderView.viewType; }
        }
    }
}
