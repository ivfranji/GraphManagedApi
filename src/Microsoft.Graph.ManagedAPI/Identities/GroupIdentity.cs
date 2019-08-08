namespace Microsoft.Graph.Identities
{
    /// <summary>
    /// Represents graph user identity.
    /// </summary>
    public class GroupIdentity : GraphIdentity
    {
        /// <summary>
        /// Create new instance of <see cref="GroupIdentity"/>
        /// </summary>
        /// <param name="id"></param>
        public GroupIdentity(string id)
            : base(typeof(Group), id)
        {
        }
    }
}