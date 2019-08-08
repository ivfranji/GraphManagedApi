namespace Microsoft.Graph.CoreAuth
{
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    /// <summary>
    /// Authorization provider interface.
    /// </summary>
    public interface IAuthorizationProvider
    {
        /// <summary>
        /// Create authentication header which will be used for authenticating requests.
        /// </summary>
        /// <returns></returns>
        Task<AuthenticationHeaderValue> GetAuthenticationHeader();

        /// <summary>
        /// Name of the provider.
        /// </summary>
        string Name { get; }
    }
}
