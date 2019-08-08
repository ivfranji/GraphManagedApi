namespace Microsoft.Graph.ManagedAPI.Tests.QueryAndView
{
    using Microsoft.Graph.Exchange;
    using Microsoft.Graph.Search;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Url query tests.
    /// </summary>
    [TestClass]
    public class IUrlQueryTests
    {
        /// <summary>
        /// Test composite query.
        /// </summary>
        [TestMethod]
        public void Test_CompositeQuery()
        {
            IUrlQuery pageQuery = new PageQuery(17, 11);
            IUrlQuery isEqualToQuery = new SearchFilter.IsEqualTo(MessageObjectSchema.Id, "abcd");
            IUrlQuery selectQuery = new SelectQuery(new[] { MessageObjectSchema.Id, MessageObjectSchema.Categories });

            IUrlQuery compositeQuery = new CompositeQuery(new[]
            {
                pageQuery,
                isEqualToQuery,
                selectQuery
            });

            Assert.AreEqual(
                "$top=11&$skip=17&$filter=Id eq 'abcd'&$select=Id,Categories",
                compositeQuery.GetUrlQuery());

            isEqualToQuery = new SearchFilter.IsEqualTo(MessageObjectSchema.From, "test@domain.com");
            compositeQuery = new CompositeQuery(new IUrlQuery[]{isEqualToQuery});

            Assert.AreEqual(
                "$filter=From/EmailAddress/Address eq 'test@domain.com'",
                compositeQuery.GetUrlQuery());
        }
    }
}
