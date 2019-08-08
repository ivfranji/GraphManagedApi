namespace Microsoft.Graph.ManagedAPI.Tests.GraphModel
{
    using System;
    using Microsoft.Graph.Exchange;
    using Microsoft.Graph.Identities;
    using Microsoft.Graph.GraphModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Graph uri tests.
    /// </summary>
    [TestClass]
    public class GraphUriTests
    {
        /// <summary>
        /// Test Graph Uri operations.
        /// </summary>
        [TestMethod]
        public void Test_GraphUriOperations()
        {
            IGraphIdentity graphIdentity = new UserIdentity("user@tenant.com");
            GraphUri graphUri = new GraphUri(
                graphIdentity,
                new EntityPath(typeof(Message)), 
                false);

            Assert.IsFalse(graphUri.IsBeta);
            this.Validate(
                "https://graph.microsoft.com/v1.0/Users/user@tenant.com/Messages",
                graphUri,
                false);

            graphUri = new GraphUri(
                graphIdentity,
                new EntityPath(typeof(Message)),
                true);

            this.Validate(
                "https://graph.microsoft.com/beta/Users/user@tenant.com/Messages",
                graphUri,
                true);

            graphUri.AddSegment("SubSegment");
            this.Validate(
                "https://graph.microsoft.com/beta/Users/user@tenant.com/Messages/SubSegment",
                graphUri,
                true);

            IUrlQuery urlQuery = new PageQuery(
                10, 
                12);

            graphUri.AddQuery(urlQuery);
            this.Validate(
                "https://graph.microsoft.com/beta/Users/user@tenant.com/Messages/SubSegment?$top=12&$skip=10",
                graphUri,
                true);

            graphUri = new GraphUri(
                new UserIdentity("mock@user.com"),
                new EntityPath("412b1e7c-f7b2-4c3e-9feb-a2b882ffc7cf",typeof(User)).SubEntity = new EntityPath(typeof(MailFolder)),
                false);

            this.Validate(
                "https://graph.microsoft.com/v1.0/Users/mock@user.com/MailFolders",
                graphUri,
                false);

            graphUri.AddSegment(nameof(MailFolder.ChildFolders));
            this.Validate(
                "https://graph.microsoft.com/v1.0/Users/mock@user.com/MailFolders/ChildFolders",
                graphUri,
                false);
        }

        /// <summary>
        /// Validate uri.
        /// </summary>
        /// <param name="expected">Expected string.</param>
        /// <param name="graphUri">Graph Uri.</param>
        /// <param name="isBeta">Is beta.</param>
        private void Validate(string expected, GraphUri graphUri, bool isBeta)
        {
            Uri uri = graphUri;
            Assert.AreEqual(
                expected,
                uri.ToString());

            Assert.AreEqual(
                isBeta,
                graphUri.IsBeta);
        }
    }
}
