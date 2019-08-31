namespace Microsoft.Graph.ManagedAPI.UnitTests.Search
{
    using Microsoft.Graph.Search;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Search filter tests.
    /// </summary>
    [TestClass]
    public class SearchFilterTests
    {
        /// <summary>
        /// Tests the is equal to.
        /// </summary>
        [TestMethod]
        public void Test_IsEqualTo()
        {
            SearchFilter isEqualTo = new SearchFilter.IsEqualTo(
                MessageObjectSchema.Id,
                "abcd==");

            Assert.AreEqual(
                "$filter=Id eq 'abcd=='",
                isEqualTo.GetUrlQuery());

            isEqualTo = new SearchFilter.IsEqualTo(
                MessageObjectSchema.From,
                "test@domain.com");

            Assert.AreEqual(
                $"$filter=From/EmailAddress/Address eq 'test@domain.com'",
                isEqualTo.GetUrlQuery());
        }

        /// <summary>
        /// Tests the is equal to.
        /// </summary>
        [TestMethod]
        public void Test_StartsWith()
        {
            SearchFilter startsWith = new SearchFilter.StartsWith(
                MessageObjectSchema.Subject,
                "Hey!");

            Assert.AreEqual(
                "$filter=startswith(Subject,'Hey!')",
                startsWith.GetUrlQuery());

            startsWith = new SearchFilter.StartsWith(
                MessageObjectSchema.From,
                "test@domain.com");

            Assert.AreEqual(
                $"$filter=startswith(From/EmailAddress/Address,'test@domain.com')",
                startsWith.GetUrlQuery());
        }
    }
}
