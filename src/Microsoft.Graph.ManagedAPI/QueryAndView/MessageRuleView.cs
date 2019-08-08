namespace Microsoft.Graph.Exchange
{
    using System;
    using Microsoft.Graph.ChangeTracking;

    /// <summary>
    /// Message rule view.
    /// </summary>
    public class MessageRuleView : ViewBase
    {
        /// <summary>
        /// View type.
        /// </summary>
        private static readonly Type viewType = typeof(MessageRule);

        /// <summary>
        /// Object schema.
        /// </summary>
        private static readonly ObjectSchema objectSchema = new MessageRuleObjectSchema();

        /// <summary>
        /// Create new instance of <see cref="MessageRuleView"/>
        /// </summary>
        public MessageRuleView()
            : this(10)
        {
        }

        /// <summary>
        /// Create new instance of <see cref="MessageRuleView"/>
        /// </summary>
        /// <param name="pageSize">Page size.</param>
        public MessageRuleView(int pageSize)
            : this(pageSize, 0)
        {
        }

        /// <summary>
        /// Create new instance of <see cref="MessageRuleView"/>
        /// </summary>
        /// <param name="pageSize">Page size.</param>
        /// <param name="offset">Offset.</param>
        public MessageRuleView(int pageSize, int offset) 
            : base(pageSize, offset)
        {
            this.PropertySet = new ItemPropertySet(
                MessageRuleView.objectSchema,
                MessageRuleObjectSchema.DisplayName, 
                MessageRuleObjectSchema.IsEnabled, 
                MessageRuleObjectSchema.IsReadOnly);
        }

        /// <summary>
        /// View type.
        /// </summary>
        internal override Type ViewType
        {
            get { return MessageRuleView.viewType; }
        }
    }
}