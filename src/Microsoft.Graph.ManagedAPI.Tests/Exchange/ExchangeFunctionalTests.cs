namespace Microsoft.Graph.ManagedAPI.Tests.Exchange
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Auth;
    using CoreAuth;
    using CoreJson;
    using Graph.CoreJson;
    using Graph.Exchange;
    using Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Search;

    /// <summary>
    /// Class contains all functional test for exchange.
    /// </summary>
    [TestClass]
    public class ExchangeFunctionalTests
    {
        [TestMethod]
        public async Task Test_FindFolders()
        {
            IAuthorizationProvider authorizationProvider = new TestAuthenticationProvider();
            ExchangeServiceContext exchangeServiceContext = new ExchangeServiceContext(authorizationProvider);
            ExchangeService exchangeService = exchangeServiceContext[AppConfig.MailboxA];

            FindMailFolderResults findMailFolderResults =
                await exchangeService.FindFolders(WellKnownFolderName.MsgFolderRoot, new MailFolderView());

            MailFolderView folderView = new MailFolderView();
            folderView.PropertySet.Add(MailFolderObjectSchema.TotalItemCount);
            ExtendedPropertyDefinition extendedPropertyDefinition1 = new ExtendedPropertyDefinition(MapiPropertyType.Long, 0x340F);
            folderView.PropertySet.Add(extendedPropertyDefinition1);
            SearchFilter searchfilter = new SearchFilter.IsEqualTo(MailFolderObjectSchema.DisplayName, "Inbox");
            findMailFolderResults = await exchangeService.FindFolders("MsgFolderRoot", searchfilter, folderView);
        }

        [TestMethod]
        public async Task Test_FindFolders_WithFilterAndExtendedProperties()
        {
            IAuthorizationProvider authorizationProvider = new TestAuthenticationProvider();
            ExchangeServiceContext exchangeServiceContext = new ExchangeServiceContext(authorizationProvider);
            ExchangeService exchangeService = exchangeServiceContext[AppConfig.MailboxA];

            MailFolderView folderView = new MailFolderView();
            folderView.PropertySet.Add(MailFolderObjectSchema.TotalItemCount);
            ExtendedPropertyDefinition extendedPropertyDefinition1 = new ExtendedPropertyDefinition(MapiPropertyType.Integer, 0x0FFE);
            folderView.PropertySet.Add(extendedPropertyDefinition1);
            SearchFilter searchfilter = new SearchFilter.IsEqualTo(MailFolderObjectSchema.DisplayName, "Inbox");
            FindMailFolderResults findMailFolderResults = await exchangeService.FindFolders("MsgFolderRoot", searchfilter, folderView);
        }

        [TestMethod]
        public async Task Test_FindMessageItems()
        {
            IAuthorizationProvider authorizationProvider = new TestAuthenticationProvider();
            ExchangeServiceContext exchangeServiceContext = new ExchangeServiceContext(authorizationProvider);
            ExchangeService exchangeService = exchangeServiceContext[AppConfig.MailboxA];

            MessageView view = new MessageView();

            FindItemResults<Message> result = await exchangeService.FindItems(WellKnownFolderName.Inbox, view);

            view.PropertySet.Add(new ExtendedPropertyDefinition(MapiPropertyType.String, 0x0037));
            view.PropertySet.Add(new ExtendedPropertyDefinition(MapiPropertyType.String, 0x001A));
            view.Offset += view.PageSize;

            exchangeService.LogFlag = LogFlag.All;
            exchangeService.LoggingEnabled = true;
            result = await exchangeService.FindItems(WellKnownFolderName.Inbox, view);
        }

        [TestMethod]
        public async Task Test_FindMessageItems_WithFilter()
        {
            IAuthorizationProvider authorizationProvider = new TestAuthenticationProvider();
            ExchangeServiceContext exchangeServiceContext = new ExchangeServiceContext(authorizationProvider);
            ExchangeService exchangeService = exchangeServiceContext[AppConfig.MailboxA];
            exchangeService.Preferences.Add("IdType=ImmutableId");
            MessageView view = new MessageView();
            SearchFilter searchFilter = new SearchFilter.IsEqualTo(MessageObjectSchema.Subject, "test1");
            FindItemResults<Message> result = await exchangeService.FindItems(WellKnownFolderName.Inbox.ToString(), view, searchFilter);
        }

        [TestMethod]
        public async Task Test_SyncFolderHierarchy()
        {
            IAuthorizationProvider authorizationProvider = new TestAuthenticationProvider();
            ExchangeServiceContext exchangeServiceContext = new ExchangeServiceContext(authorizationProvider, "MyApp");
            ExchangeService exchangeService = exchangeServiceContext[AppConfig.MailboxA];
            ChangeCollection<MailFolderChange> folderChange = null;
            string syncState = null;
            do
            {
                folderChange = await exchangeService.SyncFolderHierarchy(null, syncState);
                syncState = folderChange.SyncState;

                foreach (MailFolderChange mailFolderChange in folderChange.Items)
                {
                    Assert.AreEqual(
                        ChangeType.Created,
                        mailFolderChange.ChangeType);
                }

            } while (folderChange.MoreAvailable);

            SearchFilter inbox = new SearchFilter.IsEqualTo(MailFolderObjectSchema.DisplayName, "Inbox");
            FindMailFolderResults result =
                await exchangeService.FindFolders(WellKnownFolderName.MsgFolderRoot, inbox, new MailFolderView());

            MailFolder mf = new MailFolder(exchangeService);
            mf.DisplayName = "kakakoko";
            await mf.SaveAsync(result.MailFolders[0]);

            do
            {
                folderChange = await exchangeService.SyncFolderHierarchy(null, syncState);
                syncState = folderChange.SyncState;

                Assert.AreEqual(
                    1, 
                    folderChange.TotalCount);

                Assert.AreEqual(
                    ChangeType.Created,
                    folderChange.Items[0].ChangeType);
            } while (folderChange.MoreAvailable);

            await mf.DeleteAsync();

            do
            {
                folderChange = await exchangeService.SyncFolderHierarchy(null, syncState);
                syncState = folderChange.SyncState;

                Assert.AreEqual(
                    1,
                    folderChange.TotalCount);

                Assert.AreEqual(
                    ChangeType.Deleted,
                    folderChange.Items[0].ChangeType);
            } while (folderChange.MoreAvailable);
        }

        [TestMethod]
        public void TestSyncStateDeserialization()
        {
            string rawSync = "AUx6dFp3V2pvNUlpdldCaHl4dzVyQUhRTTZBcmtWWmloekxNRldrckd2T2dXc0FkQWZ4Rm9SRHJWOG53RHdoT3NJSk9oQmY5Zjd0Mmd1dFpOUGNJR2pYU0NRQnNkVm5wczRBX3Fzc1JkbG9FLjc3QlI5TE9yMUx2aWpBMHZibmRSVTZBRC12TEsyU1RRRmx6Skp0TG9scWs=";
            SyncStateQuery<MailFolder> s = SyncStateQuery<MailFolder>.Deserialize(rawSync);
        }
    }
}
