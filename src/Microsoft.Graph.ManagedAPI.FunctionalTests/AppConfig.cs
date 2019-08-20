namespace Microsoft.Graph.ManagedAPI.FunctionalTests
{
    using System;

    /// <summary>
    /// App config.
    /// </summary>
    internal static class AppConfig
    {
        /// <summary>
        /// Test mailbox A.
        /// </summary>
        internal static string MailboxA
        {
            get
            {
                return "testmbx1@itor.onmicrosoft.com";
            }
        }

        /// <summary>
        /// Test mailbox B.
        /// </summary>
        internal static string MailboxB
        {
            get
            {
                return "testmbx2@itor.onmicrosoft.com";
            }
        }

        /// <summary>
        /// Application id.
        /// </summary>
        internal static Guid ApplicationId
        {
            get
            {
                return new Guid("55b25885-c08b-459d-90ba-5560436678b1");
            }
        }

        /// <summary>
        /// Tenant id.
        /// </summary>
        internal static Guid TenantId
        {
            get
            {
                return new Guid("68f340ce-815f-4411-a204-601e573b80f1");
            }
        }

        /// <summary>
        /// Cert thumbprint.
        /// </summary>
        internal static string CertThumbprint
        {
            get
            {
                return "FBC672206178629F154AA73284CE071CF1999C3C";
            }
        }
    }
}
