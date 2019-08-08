namespace Microsoft.Graph.Identities
{
    using System;
    using Microsoft.Graph.Utilities;

    /// <summary>
    /// Represents graph user identity.
    /// </summary>
    public class UserIdentity : GraphIdentity
    {
        /// <summary>
        /// Create new instance of <see cref="UserIdentity"/>
        /// </summary>
        /// <param name="id"></param>
        public UserIdentity(string id)
            : base(typeof(User), id)
        {
        }

        /// <summary>
        /// Return sub entity path.
        /// </summary>
        /// <param name="subEntity">Sub entity.</param>
        /// <returns></returns>
        public override string GetSubEntityFullPath(EntityPath subEntity)
        {
            // if it works with User graph object then just forward 
            // actual entity path.
            subEntity.ThrowIfNull(nameof(subEntity));
            if (subEntity.Path.StartsWith("users", StringComparison.OrdinalIgnoreCase))
            {
                if (subEntity.Id.Equals("") ||
                    this.Id.Equals(subEntity.Id, StringComparison.OrdinalIgnoreCase))
                {
                    return $"{this.EntityPath.Path}";
                }

                if (Guid.TryParse(subEntity.Id, out Guid id))
                {
                    // navigation property case, return subEntity path as
                    // correct path will be there.
                    return $"{subEntity.Path}";
                }

                return $"{this.EntityPath.Path}/{subEntity.Id}";
            }

            return base.GetSubEntityFullPath(subEntity);
        }
    }
}