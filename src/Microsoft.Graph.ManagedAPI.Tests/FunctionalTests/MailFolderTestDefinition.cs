namespace Microsoft.Graph.ManagedAPI.Tests.FunctionalTests
{
    using System.Threading.Tasks;
    using Microsoft.Graph.Exchange;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test definition for mail folders.
    /// </summary>
    internal static class MailFolderTestDefinition
    {
        /// <summary>
        /// Test mail folder sync.
        /// </summary>
        /// <param name="exchangeService"></param>
        public static async Task SyncMailFolders(ExchangeService exchangeService)
        {
            string folder1Name = "TempSyncFolder1";
            string folder2Name = "TempSyncFolder2";

            MailFolder msgFolderRoot = await exchangeService.GetAsync<MailFolder>(
                new EntityPath(
                    WellKnownFolderName.MsgFolderRoot.ToString(),
                    typeof(MailFolder)));

            FindMailFolderResults findFolders = await exchangeService.FindFolders(
                WellKnownFolderName.MsgFolderRoot,
                new MailFolderView(30, 0));

            foreach (MailFolder mailFolder in findFolders)
            {
                if (mailFolder.DisplayName == folder1Name ||
                    mailFolder.DisplayName == folder2Name)
                {
                    await mailFolder.DeleteAsync();
                }
            }

            string syncState = null;
            int counter = 0;
            ChangeCollection<MailFolderChange> sync;

            do
            {
                sync = await exchangeService.SyncFolderHierarchy(null, syncState);
                syncState = sync.SyncState;

                counter++;

            } while (sync.MoreAvailable || counter == 4);

            Assert.IsFalse(sync.MoreAvailable);

            MailFolder folder1 = new MailFolder(exchangeService);
            folder1.DisplayName = folder1Name;
            await folder1.SaveAsync(msgFolderRoot);

            MailFolder folder2 = new MailFolder(exchangeService);
            folder2.DisplayName = folder2Name;
            await folder2.SaveAsync(msgFolderRoot);

            sync = await exchangeService.SyncFolderHierarchy(null, syncState);
            syncState = sync.SyncState;

            Assert.AreEqual(
                2,
                sync.TotalCount);

            foreach (MailFolderChange change in sync)
            {
                Assert.IsTrue(change.ChangeType == ChangeType.Created);
            }

            await folder1.DeleteAsync();
            await folder2.DeleteAsync();

            sync = await exchangeService.SyncFolderHierarchy(null, syncState);

            Assert.IsTrue(sync.TotalCount == 2);
            foreach (MailFolderChange change in sync)
            {
                Assert.IsTrue(change.ChangeType == ChangeType.Deleted);
            }
        }

        /// <summary>
        /// Get mail folders request.
        /// </summary>
        public static async Task GetMailFolders(ExchangeService exchangeService)
        {
            FindMailFolderResults findFoldersResults = null;
            MailFolderView folderView = new MailFolderView(10, 0);

            do
            {
                findFoldersResults = await exchangeService.FindFolders(WellKnownFolderName.MsgFolderRoot, folderView);
                folderView.Offset += folderView.PageSize;

                foreach (MailFolder folder in findFoldersResults)
                {
                    Assert.IsNotNull(folder.EntityService);
                }

            } while (findFoldersResults.MoreAvailable);
        }

        /// <summary>
        /// Basic CRUD operations test.
        /// </summary>
        /// <param name="exchangeService"></param>
        public static async Task CreateReadUpdateDeleteMailFolder(ExchangeService exchangeService)
        {
            MailFolder inbox = await exchangeService.GetAsync<MailFolder>(
                new EntityPath(WellKnownFolderName.Inbox.ToString(),
                    typeof(MailFolder)));

            foreach (MailFolder folder in await exchangeService.FindFolders(inbox.Id, new MailFolderView(10, 0)))
            {
                await folder.DeleteAsync();
            }

            MailFolder folder1 = new MailFolder(exchangeService)
            {
                DisplayName = "MyTestFolder1"
            };

            Assert.IsNull(folder1.Id);
            await folder1.SaveAsync(inbox);
            Assert.IsNotNull(folder1.Id);

            MailFolder folder2 = new MailFolder(exchangeService);
            folder2.DisplayName = "MyTestFolder2";

            Assert.IsNull(folder2.Id);
            await folder2.SaveAsync(inbox);
            Assert.IsNotNull(folder2.Id);
            
            folder2 = await folder2.Move(folder1.Id);

            folder1.DisplayName = "NewDisplayName";
            await folder1.UpdateAsync();

            Assert.AreEqual(
                "NewDisplayName",
                folder1.DisplayName);

            Assert.AreEqual(
                folder1.Id,
                folder2.ParentFolderId);

            await folder2.DeleteAsync();
            Assert.IsNull(folder2.DisplayName);
            Assert.IsNull(folder2.Id);

            await folder1.DeleteAsync();
            Assert.IsNull(folder1.DisplayName);
            Assert.IsNull(folder1.Id);
        }

        /// <summary>
        /// Validate if extended properties are pulled from folder.
        /// </summary>
        /// <param name="exchangeService"></param>
        public static async Task GetExtendedPropertyFromFolder(ExchangeService exchangeService)
        {
            MailFolderView folderView = new MailFolderView(20);
            folderView.PropertySet.Add(new ExtendedPropertyDefinition(MapiPropertyType.Binary, 0x0E3F));

            foreach (MailFolder folder in await exchangeService.FindFolders(WellKnownFolderName.MsgFolderRoot, folderView))
            {
                Assert.AreEqual(
                    1,
                    folder.SingleValueExtendedProperties.Count);
            }
        }
    }
}
