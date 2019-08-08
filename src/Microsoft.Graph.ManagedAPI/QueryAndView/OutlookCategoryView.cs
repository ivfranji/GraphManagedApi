namespace Microsoft.Graph.Exchange
{
    /// <summary>
    /// Outlook category view.
    /// </summary>
    public class OutlookCategoryView : TypedView<OutlookCategory, OutlookCategoryObjectSchema>
    {
        /// <summary>
        /// Create new instance of <see cref="OutlookCategoryView"/>
        /// </summary>
        public OutlookCategoryView()
            : this(10)
        {
        }

        /// <summary>
        /// Create new instance of <see cref="OutlookCategoryView"/>
        /// </summary>
        /// <param name="pageSize">Page size.</param>
        public OutlookCategoryView(int pageSize)
            : this(pageSize, 0)
        {
        }

        /// <summary>
        /// Create new instance of <see cref="OutlookCategoryView"/>
        /// </summary>
        /// <param name="pageSize">Page size.</param>
        /// <param name="offset">Offset.</param>
        public OutlookCategoryView(int pageSize, int offset) 
            : base(
                pageSize, 
                offset,
                OutlookCategoryObjectSchema.Color,
                OutlookCategoryObjectSchema.DisplayName)
        {
        }
    }
}