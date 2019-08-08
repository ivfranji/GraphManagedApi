namespace Microsoft.Graph.ChangeTracking
{
    using System.Threading.Tasks;
    using Microsoft.Graph.Exchange;
    using Microsoft.Graph.Utilities;

    /// <summary>
    /// Navigation property.
    /// </summary>
    /// <typeparam name="T">Type of property.</typeparam>
    public class NavigationProperty<T> where T : Entity
    {
        /// <summary>
        /// Property bag.
        /// </summary>
        private PropertyBag propertyBag;

        /// <summary>
        /// Page query.
        /// </summary>
        private PageQuery pageQuery;

        /// <summary>
        /// Create new instance of <see cref="NavigationProperty{T}"/>
        /// </summary>
        /// <param name="propertyBag">Property bag.</param>
        /// <param name="relativePath">Relative path.</param>
        public NavigationProperty(PropertyBag propertyBag, string relativePath)
        {
            this.propertyBag = propertyBag;
            this.RelativePath = relativePath;
            this.pageQuery = new PageQuery(10);
        }

        /// <summary>
        /// Relative path.
        /// </summary>
        public string RelativePath { get; }

        /// <summary>
        /// Get next page.
        /// </summary>
        /// <returns></returns>
        public async Task<FindEntityResults<T>> GetNextPage()
        {
            this.propertyBag.TryGetKey(
                nameof(Entity.EntityService), 
                out PropertyDefinition entityServicePropDef);
            entityServicePropDef.ThrowIfNull(nameof(entityServicePropDef));

            this.propertyBag.TryGetKey(
                nameof(Entity.EntityPath), 
                out PropertyDefinition entityPathPropDef);
            entityPathPropDef.ThrowIfNull(nameof(entityPathPropDef));

            IEntityService entityService = (IEntityService)this.propertyBag[entityServicePropDef];
            EntityPath entityPath = (EntityPath) this.propertyBag[entityPathPropDef];
            entityPath.SubEntity = new EntityPath(typeof(T));
            FindEntityResults<T> result = await entityService.Navigate<T>(entityPath, this.pageQuery);
            this.pageQuery.Offset += this.pageQuery.PageSize;

            return result;
        }

        /// <summary>
        /// Reset tracker.
        /// </summary>
        public void Reset()
        {
            this.pageQuery = new PageQuery(10);
        }
    }
}
