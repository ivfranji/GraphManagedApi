namespace Microsoft.Graph.ManagedAPI.Tests.CoreHttp
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Graph.CoreAuth;
    using Microsoft.Graph.CoreHttp;
    using NSubstitute;
    using VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test http handler behavior.
    /// </summary>
    [TestClass]
    public class HttpHandlerTests
    {
        /// <summary>
        /// Test if correct default user agent is present on requests.
        /// </summary>
        [TestMethod]
        public async Task Test_UserAgentHttpHandler_DefaultHeader()
        {
            IHttpExtensionHandler extensionHandler = Substitute.For<IHttpExtensionHandler>();
            extensionHandler.SendAsync(
                null,
                default(CancellationToken),
                null).ReturnsForAnyArgs(
                (callInfo) =>
                {
                    HttpRequestMessage requestMessage = callInfo[0] as HttpRequestMessage;
                    foreach (ProductInfoHeaderValue value in requestMessage.Headers.UserAgent)
                    {
                        Assert.AreEqual(
                            "Graph-ManagedAPI",
                            value.Product.Name);
                    }

                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
                });
            HttpRequestContext context = new HttpRequestContext();
            context.HttpExtensionHandler = extensionHandler;
            context.AuthorizationProvider = Substitute.For<IAuthorizationProvider>();
            using (HttpRequest request = HttpRequest.Get(new Uri("http://localhost"), context))
            {
                await request.GetHttpResponseAsync();
            }
        }

        /// <summary>
        /// Test if correct user agent is present on requests.
        /// </summary>
        [TestMethod]
        public async Task Test_UserAgentHttpHandler_CustomHeader()
        {
            IHttpExtensionHandler extensionHandler = Substitute.For<IHttpExtensionHandler>();
            extensionHandler.SendAsync(
                null,
                default(CancellationToken),
                null).ReturnsForAnyArgs(
                (callInfo) =>
                {
                    HttpRequestMessage requestMessage = callInfo[0] as HttpRequestMessage;
                    foreach (ProductInfoHeaderValue value in requestMessage.Headers.UserAgent)
                    {
                        Assert.AreEqual(
                            "Graph-ManagedAPI-abc",
                            value.Product.Name);
                    }

                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
                });
            HttpRequestContext context = new HttpRequestContext();
            context.HttpExtensionHandler = extensionHandler;
            context.AuthorizationProvider = Substitute.For<IAuthorizationProvider>();
            using (HttpRequest request = HttpRequest.Get(new Uri("http://localhost"), context))
            {
                request.UserAgent = "abc";
                await request.GetHttpResponseAsync();
            }
        }

        /// <summary>
        /// Test <see cref="AuthZHttpHandler"/> throws on retry count exceeded.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_AuthZHttpHandler_ThrowsOnRetryCountExceeded()
        {
            IHttpExtensionHandler extensionHandler = Substitute.For<IHttpExtensionHandler>();
            extensionHandler.SendAsync(
                null,
                default(CancellationToken),
                null).ReturnsForAnyArgs(
                Task.FromResult(
                    new HttpResponseMessage(HttpStatusCode.Unauthorized)));

            HttpRequestContext context = new HttpRequestContext();
            context.HttpExtensionHandler = extensionHandler;
            context.AuthorizationProvider = Substitute.For<IAuthorizationProvider>();

            using (HttpRequest get = HttpRequest.Get(new Uri("http://localhost"), context))
            {
                try
                {
                    HttpResponse response = await get.GetHttpResponseAsync();
                }
                catch (HttpRetryCountException e)
                {
                    Assert.AreEqual(
                        3,
                        e.RetryCount);

                    Assert.AreEqual(
                        0,
                        e.TotalDelayApplied);

                    Assert.AreEqual(
                        HttpStatusCode.Unauthorized,
                        e.StatusCode);
                }
            }
        }

        /// <summary>
        /// Test if <see cref="AuthZHttpHandler"/> perform authentication correctly.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_AuthZHttpHandler_SuccessfullyAuthenticates()
        {
            IHttpExtensionHandler extensionHandler = Substitute.For<IHttpExtensionHandler>();
            extensionHandler.SendAsync(
                new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost")),
                default(CancellationToken),
                null).ReturnsForAnyArgs(
                (callInfo) =>
                {
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized));
                },
                (callInfo) =>
                {
                    HttpRequestMessage requestMessage = callInfo[0] as HttpRequestMessage;
                    Assert.AreEqual(
                        "Bearer",
                        requestMessage.Headers.Authorization.Scheme);

                    Assert.AreEqual(
                        "123",
                        requestMessage.Headers.Authorization.Parameter);
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
                });

            IAuthorizationProvider authorizationProvider = Substitute.For<IAuthorizationProvider>();
            authorizationProvider.GetAuthenticationHeader().Returns(new AuthenticationHeaderValue("Bearer", "123"));

            HttpRequestContext context = new HttpRequestContext();
            context.HttpExtensionHandler = extensionHandler;
            context.AuthorizationProvider = authorizationProvider;

            using (HttpRequest get = HttpRequest.Get(new Uri("http://localhost"), context))
            {
                HttpResponse response = await get.GetHttpResponseAsync();

                IEnumerable<string> header = response.ResponseHeaders.GetValues("X-RetryAttempt-HttpAuthZHandler");
                Assert.IsNotNull(
                    header);

                foreach (string s in header)
                {
                    int i = 0;
                    Assert.IsTrue(
                        int.TryParse(s, out i));

                    Assert.AreEqual(
                        1,
                        i);
                }
            }
        }

        /// <summary>
        /// Test if throttling is applied correctly.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_ThrottlingHttpHandler_ThrowsOnRetryCountExceeded()
        {
            IHttpExtensionHandler extensionHandler = Substitute.For<IHttpExtensionHandler>();
            extensionHandler.SendAsync(
                null,
                default(CancellationToken),
                null).ReturnsForAnyArgs(
                Task.FromResult(
                    new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)));

            HttpRequestContext context = new HttpRequestContext();
            context.HttpExtensionHandler = extensionHandler;
            context.AuthorizationProvider = Substitute.For<IAuthorizationProvider>();

            using (HttpRequest get = HttpRequest.Get(new Uri("http://localhost"), context))
            {
                try
                {
                    HttpResponse response = await get.GetHttpResponseAsync();
                }
                catch (HttpRetryCountException e)
                {
                    Assert.AreEqual(
                        3,
                        e.RetryCount);

                    Assert.AreEqual(
                        15,
                        e.TotalDelayApplied);

                    Assert.AreEqual(
                        HttpStatusCode.ServiceUnavailable,
                        e.StatusCode);
                }
            }
        }

        /// <summary>
        /// Test if throttling handler apply delay and then success.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_ThrottlingHttpHandler_SuccessfulRetry()
        {
            IHttpExtensionHandler extensionHandler = Substitute.For<IHttpExtensionHandler>();
            extensionHandler.SendAsync(
                new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost")),
                default(CancellationToken),
                null).ReturnsForAnyArgs(
                (callInfo) =>
                {
                    return Task.FromResult(new HttpResponseMessage((HttpStatusCode)429));
                },
                (callInfo) =>
                {
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
                },
                (callInfo) =>
                {
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
                });

            IAuthorizationProvider authorizationProvider = Substitute.For<IAuthorizationProvider>();
            authorizationProvider.GetAuthenticationHeader().Returns(new AuthenticationHeaderValue("Bearer", "123"));

            HttpRequestContext context = new HttpRequestContext();
            context.HttpExtensionHandler = extensionHandler;
            context.AuthorizationProvider = authorizationProvider;

            using (HttpRequest get = HttpRequest.Get(new Uri("http://localhost"), context))
            {
                HttpResponse response = await get.GetHttpResponseAsync();

                IEnumerable<string> header = response.ResponseHeaders.GetValues("X-RetryAttempt-HttpThrottlingHandler");
                Assert.IsNotNull(
                    header);

                foreach (string s in header)
                {
                    int i = 0;
                    Assert.IsTrue(
                        int.TryParse(s, out i));

                    Assert.AreEqual(
                        2,
                        i);
                }

                header = response.ResponseHeaders.GetValues("X-TotalDelayApplied-HttpThrottlingHandler");
                Assert.IsNotNull(
                    header);

                foreach (string s in header)
                {
                    int i = 0;
                    Assert.IsTrue(
                        int.TryParse(s, out i));

                    Assert.AreEqual(
                        10,
                        i);
                }
            }
        }
    }
}
