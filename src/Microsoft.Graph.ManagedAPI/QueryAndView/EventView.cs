namespace Microsoft.Graph.Exchange
{
    /// <summary>
    /// Event view.
    /// </summary>
    public class EventView : TypedView<Event, EventObjectSchema>
    {
        /// <summary>
        /// Create new instance of <see cref="EventView"/> with page size of 10.
        /// </summary>
        public EventView()
            : this(10)
        {
        }

        /// <summary>
        /// Create new instance of <see cref="EventView"/>
        /// </summary>
        /// <param name="pageSize">Page size.</param>
        public EventView(int pageSize)
            : this(pageSize, 0)
        {
        }

        /// <summary>
        /// Create new instance of <see cref="EventView"/>.
        /// </summary>
        /// <param name="pageSize">Page size.</param>
        /// <param name="offset">Offset.</param>
        public EventView(int pageSize, int offset) 
            : base(pageSize, offset, EventObjectSchema.Start, EventObjectSchema.End, EventObjectSchema.Location)
        {
        }
    }
}