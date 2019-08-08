namespace Microsoft.Graph.Exchange
{
    using ChangeTracking;

    /// <summary>
    /// Outlook task view.
    /// </summary>
    public class OutlookTaskView : TypedView<OutlookTask, OutlookTaskObjectSchema>
    {
        /// <summary>
        /// Create new instance of <see cref="OutlookTaskView"/> with page size 10
        /// </summary>
        public OutlookTaskView()
            : this(10)
        {
        }

        /// <summary>
        /// Create new instance of <see cref="OutlookTaskView"/>
        /// </summary>
        /// <param name="pageSize">Page size.</param>
        public OutlookTaskView(int pageSize)
            : this(pageSize, 0)
        {
        }

        /// <summary>
        /// Create new instance of <see cref="OutlookTaskView"/>
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="offset"></param>
        public OutlookTaskView(int pageSize, int offset) 
            : base(
                pageSize, 
                offset, 
                OutlookTaskObjectSchema.Owner,
                OutlookTaskObjectSchema.Subject,
                OutlookTaskObjectSchema.Status)
        {
        }
    }
}