namespace Microsoft.Graph.Exchange
{
    using System;
    using Microsoft.Graph.ChangeTracking;

    /// <summary>
    /// User view.
    /// </summary>
    public class UserView : ViewBase
    {
        /// <summary>
        /// View type.
        /// </summary>
        private static readonly Type viewType = typeof(User);

        /// <summary>
        /// Object schema.
        /// </summary>
        private static readonly ObjectSchema objectSchema = new UserObjectSchema();

        /// <summary>
        /// Create new instance of <see cref="UserView"/>
        /// </summary>
        public UserView()
            : this(1)
        {
        }

        /// <summary>
        /// Create new instance of <see cref="UserView"/>
        /// </summary>
        /// <param name="pageSize">Page size.</param>
        public UserView(int pageSize)
            : this(pageSize, 0)
        {
        }

        /// <summary>
        /// Create new instance of <see cref="UserView"/>
        /// </summary>
        /// <param name="pageSize">Page size.</param>
        /// <param name="offset">Offset.</param>
        public UserView(int pageSize, int offset) 
            : base(pageSize, offset)
        {
            this.PropertySet = new ItemPropertySet(
                UserView.objectSchema,
                UserObjectSchema.DisplayName,
                UserObjectSchema.GivenName,
                UserObjectSchema.EmployeeId);
        }

        /// <summary>
        /// View type.
        /// </summary>
        internal override Type ViewType
        {
            get { return UserView.viewType; }
        }
    }
}