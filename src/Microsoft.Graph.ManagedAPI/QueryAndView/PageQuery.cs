namespace Microsoft.Graph.Exchange
{
    using System;

    /// <summary>
    /// Paging query.
    /// </summary>
    public class PageQuery : IUrlQuery
    {
        /// <summary>
        /// Top prefix.
        /// </summary>
        private const string TopPrefix = "$top=";

        /// <summary>
        /// Skip prefix.
        /// </summary>
        private const string SkipPrefix = "$skip=";

        /// <summary>
        /// Page offset.
        /// </summary>
        private int offset;

        /// <summary>
        /// Page size.
        /// </summary>
        private int pageSize;

        /// <summary>
        /// Create new instance of <see cref="PageQuery"/> with offset = 0 and pageSize = 5.
        /// </summary>
        internal PageQuery()
            : this(5)
        {
        }

        /// <summary>
        /// Create new instance of <see cref="PageQuery"/> with offset 0 and specified page size.
        /// </summary>
        /// <param name="pageSize">Page size.</param>
        public PageQuery(int pageSize)
            : this(0, pageSize)
        {
        }

        /// <summary>
        /// Create new instance of <see cref="PageQuery"/> with specified offset and page size.
        /// </summary>
        /// <param name="offset">Page offset.</param>
        /// <param name="pageSize">Page size.</param>
        public PageQuery(int offset, int pageSize)
        {
            this.Offset = offset;
            this.PageSize = pageSize;
        }

        /// <summary>
        /// Query string.
        /// </summary>
        public string GetUrlQuery()
        {
            return $"{PageQuery.TopPrefix}{this.PageSize}&{PageQuery.SkipPrefix}{this.Offset}";
        }

        /// <summary>
        /// Page offset.
        /// </summary>
        public int Offset
        {
            get { return this.offset; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(this.Offset),
                        "Offset must be zero or greater.");
                }

                this.offset = value;
            }
        }

        /// <summary>
        /// Page size.
        /// </summary>
        public int PageSize
        {
            get { return this.pageSize; }
            set
            {
                if (value <= 0 || value > 50)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(this.PageSize),
                        "PageSize must be greater than zero or less / equal to 50");
                }

                this.pageSize = value;
            }
        }
    }
}
