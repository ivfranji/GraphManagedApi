namespace Microsoft.Graph.ManagedAPI.Tests.Auth
{
    using System;
    using System.Net.Http.Headers;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Microsoft.Graph.CoreAuth;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    internal class TestAuthenticationProvider : IAuthorizationProvider
    {
        private string ResourceUri
        {
            get { return "https://graph.microsoft.com"; }
        }

        public async Task<AuthenticationHeaderValue> GetAuthenticationHeader()
        {
            string token = await this.GetToken();
            return new AuthenticationHeaderValue(
                "Bearer",
                token);
        }

        public string Name
        {
            get { return nameof(TestAuthenticationProvider); }
        }

        private async Task<string> GetToken()
        {
            string authority = $"https://login.microsoftonline.com/{AppConfig.TenantId}";
            AuthenticationContext context = new AuthenticationContext(authority);

            X509Certificate2 certFromStore = null;
            using (X509Store store = new X509Store(StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection collection = store.Certificates.Find(
                    X509FindType.FindByThumbprint,
                    AppConfig.CertThumbprint,
                    false);

                if (collection.Count == 1)
                {
                    certFromStore = collection[0];
                }
            }

            if (certFromStore == null)
            {
                throw new ArgumentNullException("Certificate");
            }

            ClientAssertionCertificate cert = new ClientAssertionCertificate(
                AppConfig.ApplicationId.ToString(),
                certFromStore);

            AuthenticationResult token = await context.AcquireTokenAsync(
                this.ResourceUri,
                cert);

            return token.AccessToken;
        }
    }
}
