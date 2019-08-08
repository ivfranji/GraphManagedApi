namespace Microsoft.Graph.Exchange
{
    using System;
    using Microsoft.Graph.Utilities;

    /// <summary>
    /// Base view.
    /// </summary>
    public abstract class ViewBase : IUrlQuery
    {
        /// <summary>
        /// Create new instance of <see cref="ViewBase"/>
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="offset"></param>
        protected ViewBase(int pageSize, int offset)
        {
            this.PageQuery = new PageQuery(offset, pageSize);
        }

        /// <summary>
        /// Page query.
        /// </summary>
        protected PageQuery PageQuery { get; }

        /// <summary>
        /// View type.
        /// </summary>
        internal abstract Type ViewType { get; }
        
        /// <summary>
        /// Property set.
        /// </summary>
        public PropertySet PropertySet { get; protected set; }

        /// <summary>
        /// Page size.
        /// </summary>
        public int PageSize
        {
            get { return this.PageQuery.PageSize; }
            set { this.PageQuery.PageSize = value; }
        }

        /// <summary>
        /// View offset.
        /// </summary>
        public int Offset
        {
            get { return this.PageQuery.Offset; }
            set { this.PageQuery.Offset = value; }
        }

        /// <summary>
        /// Get url query.
        /// </summary>
        /// <returns></returns>
        public string GetUrlQuery()
        {
            if (this.PropertySet.UrlQueryEmpty)
            {
                return this.PageQuery.GetUrlQuery();
            }

            CompositeQuery compositeQuery = new CompositeQuery(new IUrlQuery[] { this.PageQuery, this.PropertySet });
            return compositeQuery.GetUrlQuery();
        }

        /// <summary>
        /// Validate view type supported.
        /// </summary>
        /// <param name="type"></param>
        internal void ValidateViewTypeSupported(Type type)
        {
            type.ThrowIfNull(nameof(type));
            if (this.ViewType != type)
            {
                throw new ArgumentException($"View support '{this.ViewType.Name}' type while provided type is '{type.Name}'.");
            }
        }
    }
}