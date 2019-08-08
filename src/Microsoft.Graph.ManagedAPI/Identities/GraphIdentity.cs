namespace Microsoft.Graph.Identities
{
    using System;
    using Microsoft.Graph.Utilities;

    /// <summary>
    /// Graph identity.
    /// </summary>
    public abstract class GraphIdentity : IGraphIdentity
    {
        /// <summary>
        /// Create new instance of <see cref="GraphIdentity"/>
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="id"></param>
        protected GraphIdentity(Type entityType, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            this.Id = id;
            this.EntityPath = new EntityPath(id, entityType);
        }

        /// <summary>
        /// User entity path.
        /// </summary>
        protected EntityPath EntityPath { get; }

        /// <summary>
        /// User Id.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Create sub entity path.
        /// </summary>
        /// <param name="subEntity">Sub entity.</param>
        /// <returns></returns>
        public virtual string GetSubEntityFullPath(EntityPath subEntity)
        {
            subEntity.ThrowIfNull(nameof(subEntity));
            return $"{this.EntityPath.Path}/{subEntity.Path}";
        }
    }
}