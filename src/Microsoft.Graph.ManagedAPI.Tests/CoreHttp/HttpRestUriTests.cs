namespace Microsoft.Graph.ManagedAPI.Tests.CoreHttp
{
    using System;
    using Microsoft.Graph.CoreHttp;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HttpRestUriTests
    {
        /// <summary>
        /// Test http query parameter.
        /// </summary>
        [TestMethod]
        public void Test_HttpQueryParameter()
        {
            string query = "?$top=10&$skip=12&$customParam=abcd122";
            HttpQueryParameter queryParameter = new HttpQueryParameter(query);

            Assert.IsTrue(queryParameter.Count == 3);
            Assert.IsTrue(queryParameter.ContainsKey("$top"));
            Assert.IsTrue(queryParameter.ContainsKey("$skip"));
            Assert.IsTrue(queryParameter.ContainsKey("$customParam"));

            Assert.AreEqual(
                queryParameter["$top"],
                "10");

            Assert.AreEqual(
                queryParameter["$skip"],
                "12");

            Assert.AreEqual(
                queryParameter["$customParam"],
                "abcd122");

            queryParameter.Add(
                "$prop",
                17);

            Assert.IsTrue(queryParameter.ContainsKey("$prop"));
            Assert.AreEqual(
                queryParameter["$prop"],
                17);

            Assert.IsTrue(queryParameter.Count == 4);

            Assert.AreEqual(
                "$top=10&$skip=12&$customParam=abcd122&$prop=17",
                queryParameter.ToQueryString());

            Assert.IsTrue(queryParameter.Remove("$skip"));
            Assert.AreEqual(
                queryParameter.Count,
                3);

            Assert.AreEqual(
                "$top=10&$customParam=abcd122&$prop=17",
                queryParameter.ToQueryString());
        }

        /// <summary>
        /// Test http rest uri.
        /// </summary>
        [TestMethod]
        public void Test_HttpRestUri()
        {
            HttpRestUri restUri = new HttpRestUri("https://localhost");

            Uri cast = restUri;
            Assert.AreEqual(
                "https://localhost/",
                cast.ToString());

            restUri.AddQueryParameter(
                "param1",
                10);

            cast = restUri;
            Assert.AreEqual(
                "https://localhost/?param1=10",
                cast.ToString());

            restUri.AddSegment("seg1");

            restUri.RemoveQueryParameter("param1");
            cast = restUri;
            Assert.AreEqual(
                "https://localhost/seg1",
                cast.ToString());

            restUri.AddSegment("seg2");
            restUri.AddSegment("seg3");
            restUri.RemoveSegment("seg1");

            restUri.AddQueryParameter("p1", "abc");
            restUri.AddQueryParameter("p2", 11);

            cast = restUri;
            Assert.AreEqual(
                "https://localhost/seg2/seg3?p1=abc&p2=11",
                cast.ToString());

            restUri.AddSegment("seg4");
            cast = restUri;
            Assert.AreEqual(
                "https://localhost/seg2/seg3/seg4?p1=abc&p2=11",
                cast.ToString());

            // Test 'immutability' of submitted url
            restUri = new HttpRestUri("https://localhost/seg1?p17=10");

            restUri.AddQueryParameter("test", "fff");
            restUri.AddQueryParameter("p17", 20);
            restUri.RemoveSegment("seg1");

            cast = restUri;
            Assert.AreEqual(
                "https://localhost/seg1?p17=10&test=fff",
                cast.ToString());

            restUri = new HttpRestUri("https://localhost/beta/me/u");

            string segment;
            Assert.IsTrue(
                restUri.TryGetSegment(
                    1,
                    out segment));

            Assert.AreEqual("beta", segment);

            Assert.IsFalse(
                restUri.TryGetSegment(
                    4,
                    out segment));

            restUri.AddSegment("tttt");

            Assert.IsTrue(
                restUri.TryGetSegment(
                    4,
                    out segment));

            Assert.AreEqual("tttt", segment);
        }
    }
}
