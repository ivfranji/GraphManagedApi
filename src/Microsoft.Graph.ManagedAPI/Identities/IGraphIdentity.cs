namespace Microsoft.Graph.Identities
{
    /// <summary>
    /// Graph identity.
    /// </summary>
    public interface IGraphIdentity
    {
        /// <summary>
        /// Id.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Construct sub entity path.
        /// </summary>
        /// <param name="subEntityPath"></param>
        /// <returns></returns>
        string GetSubEntityFullPath(EntityPath subEntityPath);
    }
}
