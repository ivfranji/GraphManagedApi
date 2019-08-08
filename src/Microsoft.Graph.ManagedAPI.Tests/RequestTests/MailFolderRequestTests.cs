namespace Microsoft.Graph.ManagedAPI.Tests.RequestTests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Microsoft.Graph.CoreAuth;
    using Microsoft.Graph.Exchange;
    using Microsoft.Graph.Search;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;
    using Utilities;

    [TestClass]
    public class MailFolderRequestTests
    {
        /// <summary>
        /// Test Find folders with default mail folder view.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_FindFoldersWithDefaultMailFolderView()
        {
            ExchangeService service = this.GetExchangeServiceWithUrlValidator(
                new Uri("https://graph.microsoft.com/v1.0/Users/test@user.com/MailFolders/Inbox/ChildFolders?$top=10&$skip=0"));

            await service.FindFolders(
                WellKnownFolderName.Inbox,
                new MailFolderView());
        }

        /// <summary>
        /// Test find folders with custom page size and offset.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_FindFoldersWithCustomPageSizeMailFolderView()
        {
            ExchangeService service = this.GetExchangeServiceWithUrlValidator(
                new Uri("https://graph.microsoft.com/v1.0/Users/test@user.com/MailFolders/Inbox/ChildFolders?$top=7&$skip=12"));

            MailFolderView mailFolderView = new MailFolderView(7, 12);

            await service.FindFolders(
                WellKnownFolderName.Inbox,
                mailFolderView);
        }

        /// <summary>
        /// Test find folders with custom page size and property.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_FindFoldersWithCustomPageSizeAndProperty()
        {
            ExchangeService service = this.GetExchangeServiceWithUrlValidator(
                new Uri("https://graph.microsoft.com/v1.0/Users/test@user.com/MailFolders/Inbox/ChildFolders?$top=7&$skip=12&$select=Id,ChildFolderCount,DisplayName,ParentFolderId,TotalItemCount"));

            MailFolderView mailFolderView = new MailFolderView(7, 12);
            mailFolderView.PropertySet.Add(MailFolderObjectSchema.TotalItemCount);

            await service.FindFolders(
                WellKnownFolderName.Inbox,
                mailFolderView);
        }

        /// <summary>
        /// Test find folders with custom page size, property and extended property.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_FindFoldersWithCustomPageSizeAndPropertyExtendedProperty()
        {
            ExchangeService service = this.GetExchangeServiceWithUrlValidator(
                new Uri("https://graph.microsoft.com/v1.0/Users/test@user.com/MailFolders/Inbox/ChildFolders?$top=7&$skip=12&$select=Id,ChildFolderCount,DisplayName,ParentFolderId,TotalItemCount&$expand=SingleValueExtendedProperties($filter=Id eq 'Double 0x1234')"));

            MailFolderView mailFolderView = new MailFolderView(7, 12);
            mailFolderView.PropertySet.Add(MailFolderObjectSchema.TotalItemCount);
            mailFolderView.PropertySet.Add(new ExtendedPropertyDefinition(MapiPropertyType.Double, 0x1234));

            await service.FindFolders(
                WellKnownFolderName.Inbox,
                mailFolderView);
        }
        
        /// <summary>
        /// Test FindFolders request.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_FindFoldersWithCustomPropertiesAndFilter()
        {
            ExchangeService service = this.GetExchangeServiceWithUrlValidator(
                new Uri("https://graph.microsoft.com/v1.0/Users/test@user.com/MailFolders/Inbox/ChildFolders?$top=7&$skip=12&$select=Id,ChildFolderCount,DisplayName,ParentFolderId,TotalItemCount&$expand=SingleValueExtendedProperties($filter=Id eq 'Double 0x1234')&$filter=DisplayName eq 'Abcd'"));

            MailFolderView mailFolderView = new MailFolderView(7, 12);
            mailFolderView.PropertySet.Add(MailFolderObjectSchema.TotalItemCount);
            mailFolderView.PropertySet.Add(new ExtendedPropertyDefinition(MapiPropertyType.Double, 0x1234));
            
            SearchFilter searchFilter = new SearchFilter.IsEqualTo(
                MailFolderObjectSchema.DisplayName,
                "Abcd");

            await service.FindFolders(
                WellKnownFolderName.Inbox,
                searchFilter,
                mailFolderView);
        }

        /// <summary>
        /// Test GetAsync mail folder.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_GetAsyncMailFolder()
        {
            ExchangeService exchangeService = this.GetExchangeServiceWithUrlValidatorAndConfiguredResponse(
                new Uri("https://graph.microsoft.com/v1.0/Users/test@user.com/MailFolders/Inbox"),
                "{ \"DisplayName\": \"Inbox\", \"Id\": \"abcd\" }");

            MailFolder mailFolder = await exchangeService.GetAsync<MailFolder>(new EntityPath("Inbox", typeof(MailFolder)));
            Assert.AreEqual(
                "abcd",
                mailFolder.Id);

            Assert.IsFalse(mailFolder.IsNew);
        }

        /// <summary>
        /// Get exchange service with url validator.
        /// </summary>
        /// <returns></returns>
        private ExchangeService GetExchangeServiceWithUrlValidator(Uri expectedUriForRequest)
        {
            ExchangeService service = this.GetExchangeService();
            service.HttpRequestContext.HttpExtensionHandler = new UrlValidatorExtensionHandler(expectedUriForRequest);

            return service;
        }

        /// <summary>
        /// Get exchange service with configured response.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        private ExchangeService GetExchangeServiceWithConfiguredResponse(string response, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            response.ThrowIfNullOrEmpty(nameof(response));
            ExchangeService service = this.GetExchangeService();

            HttpResponseMessage responseMessage = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(response)
            };

            service.HttpRequestContext.HttpExtensionHandler = new HttpResponseExtensionHandler(responseMessage);
            return service;
        }

        /// <summary>
        /// Get exchange service with url validator and configured response.
        /// </summary>
        /// <param name="expectedUri"></param>
        /// <param name="response"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        private ExchangeService GetExchangeServiceWithUrlValidatorAndConfiguredResponse(Uri expectedUri, string response, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            ExchangeService service = this.GetExchangeService();
            service.HttpRequestContext.HttpExtensionHandler = new UrlValidatorAndResponseExtensionHandler(
                expectedUri, 
                response, 
                statusCode);

            return service;
        }

        /// <summary>
        /// Get exchange service.
        /// </summary>
        /// <returns></returns>
        private ExchangeService GetExchangeService()
        {
            IAuthorizationProvider authProvider = Substitute.For<IAuthorizationProvider>();
            authProvider.GetAuthenticationHeader().ReturnsForAnyArgs(Task.FromResult(new AuthenticationHeaderValue("Bearer", "abcd")));

            ExchangeServiceContext ctx = new ExchangeServiceContext(authProvider);
            return ctx["test@user.com"];
        }
    }
}
