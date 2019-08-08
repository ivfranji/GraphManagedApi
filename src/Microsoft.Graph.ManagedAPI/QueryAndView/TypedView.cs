namespace Microsoft.Graph.Exchange
{
    using System;
    using Microsoft.Graph.ChangeTracking;

    /// <summary>
    /// Typed view.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="S"></typeparam>
    public abstract class TypedView<T,S> : ViewBase where T : Entity where S : ObjectSchema, new()
    {   
        /// <summary>
        /// View type.
        /// </summary>
        private static readonly Type viewType = typeof(T);

        /// <summary>
        /// Object schema.
        /// </summary>
        private static readonly ObjectSchema objectSchema = new S();
        
        /// <summary>
        /// Create new instance of <see cref="TypedView{T,S}"/>
        /// </summary>
        /// <param name="pageSize">Page size.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="properties">Properties.</param>
        protected TypedView(int pageSize, int offset, params PropertyDefinition[] properties) 
            : base(pageSize, offset)
        {
            this.PropertySet = new ItemPropertySet(
                TypedView<T, S>.objectSchema, 
                properties);
        }

        /// <summary>
        /// View type.
        /// </summary>
        internal sealed override Type ViewType
        {
            get { return TypedView<T, S>.viewType; }
        }
    }
}