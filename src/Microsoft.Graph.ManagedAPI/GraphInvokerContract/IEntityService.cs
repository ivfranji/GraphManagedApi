namespace Microsoft.Graph
{
    using System.Threading.Tasks;
    using Microsoft.Graph.Exchange;

    /// <summary>
    /// Entity service. contract.
    /// </summary>
    public interface IEntityService : IActionInvoker
    {
        /// <summary>
        /// Create entity on the server and refresh local property bag.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="destination">Destination.</param>
        /// <returns></returns>
        Task<Entity> CreateAsync(Entity entity, Entity destination);

        /// <summary>
        /// Create entity on the server and refresh local property bag.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="destination">Destination.</param>
        /// <returns></returns>
        Task<Entity> CreateAsync(Entity entity, EntityPath destination);

        /// <summary>
        /// Delete entity from the server.
        /// </summary>
        /// <param name="entity"></param>
        Task DeleteAsync(Entity entity);

        /// <summary>
        /// Update entity and refresh property bag.
        /// </summary>
        /// <param name="entity"></param>
        Task<Entity> UpdateAsync(Entity entity);

        /// <summary>
        /// Invoke
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityPath"></param>
        /// <returns></returns>
        Task<T> GetAsync<T>(EntityPath entityPath) where T : Entity;

        /// <summary>
        /// Navigate to particular entity with specified page size.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityPath">Entity path.</param>
        /// <param name="pageQuery">Page query.</param>
        /// <returns></returns>
        Task<FindEntityResults<T>> Navigate<T>(EntityPath entityPath, PageQuery pageQuery) where T : Entity;
    }
}
