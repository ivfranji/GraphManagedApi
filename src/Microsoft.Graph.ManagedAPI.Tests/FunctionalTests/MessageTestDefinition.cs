namespace Microsoft.Graph.ManagedAPI.Tests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Microsoft.Graph.Exchange;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Graph.Search;

    /// <summary>
    /// Mail message test definition.
    /// </summary>
    public class MessageTestDefinition
    {
        /// <summary>
        /// Get mail with single extended property.
        /// </summary>
        public static async Task GetMessageWithSingleExtendedProperties(ExchangeService exchangeService)
        {
            MessageView messageView = new MessageView(1);
            messageView.PropertySet.Add(new ExtendedPropertyDefinition(
                MapiPropertyType.String,
                0x0C1F));

            messageView.PropertySet.Add(MessageObjectSchema.HasAttachments);
            FindItemResults<Message> findItemResults = await exchangeService.FindItems(
                WellKnownFolderName.Inbox,
                messageView);

            foreach (Message message in findItemResults)
            {
                Assert.AreEqual(
                    1,
                    message.SingleValueExtendedProperties.Count);
            }

            messageView.Offset += messageView.PageSize;
            messageView.PropertySet.Add(
                new ExtendedPropertyDefinition(MapiPropertyType.String,
                    0x0037));

            findItemResults = await exchangeService.FindItems(
                WellKnownFolderName.Inbox,
                messageView);
            foreach (Message item in findItemResults)
            {
                Assert.AreEqual(
                    2,
                    item.SingleValueExtendedProperties.Count);
            }
        }

        /// <summary>
        /// Send message from mailbox a to mailbox b
        /// </summary>
        public static async Task SendMessageFromMailboxAToMailboxB(ExchangeService exchangeServiceA, ExchangeService exchangeServiceB)
        {
            string messageSubject = Guid.NewGuid().ToString();
            Message message = new Message(exchangeServiceA)
            {
                Subject = messageSubject,
                Body = new ItemBody()
                {
                    Content = "Test message",
                    ContentType = BodyType.Html
                }
            };

            message.ToRecipients = new List<Recipient>
            {
                new Recipient()
                {
                    EmailAddress = new EmailAddress()
                    {
                        Address = AppConfig.MailboxB
                    }
                }
            };

            MailFolder draftFolder = await exchangeServiceA.GetAsync<MailFolder>(
                    new EntityPath(WellKnownFolderName.Drafts.ToString(),
                    typeof(MailFolder)));

            await message.SaveAsync(draftFolder);
            await message.Send();

            Thread.Sleep(6000); // allow some time for email to be delivered
            MessageView messageView = new MessageView(10);
            SearchFilter subjectFilter = new SearchFilter.IsEqualTo(
                MessageObjectSchema.Subject,
                messageSubject);

            FindItemResults<Message> mailboxBMessages = await exchangeServiceB.FindItems(
                "Inbox", 
                messageView,
                subjectFilter);
            
            Assert.AreEqual(1, mailboxBMessages.TotalCount);
            Message msg = mailboxBMessages.Items[0];
            await msg.Reply("this is my reply");

            Thread.Sleep(8000); // allow some time for email to be delivered

            subjectFilter = new SearchFilter.IsEqualTo(
                MessageObjectSchema.Subject,
                $"Re: {messageSubject}");

            FindItemResults<Message> mailboxAMessages = await exchangeServiceA.FindItems(
                "Inbox",
                messageView,
                subjectFilter);

            Assert.IsTrue(mailboxAMessages.TotalCount == 1);

            await mailboxAMessages.Items[0].DeleteAsync();
        }

        /// <summary>
        /// CRUD operation against extended properties.
        /// </summary>
        /// <param name="exchangeService">Exchange service.</param>
        public static async Task CreateReadUpdateDeleteExtendedProperties(ExchangeService exchangeService)
        {
            const string extendedPropertyGuid = "4d557659-9e3f-405e-8f6d-86d2d9d5c630";
            string subject = Guid.NewGuid().ToString();
            MailFolder inbox = await exchangeService.GetAsync<MailFolder>(new EntityPath("Inbox", typeof(MailFolder)));
            Message msg = new Message(exchangeService);
            msg.Subject = subject;
            msg.SingleValueExtendedProperties.Add(new SingleValueLegacyExtendedProperty()
            {
                Id = $"String {extendedPropertyGuid} Name Blah",
                Value = "BlahValue"
            });

            msg.MultiValueExtendedProperties.Add(new MultiValueLegacyExtendedProperty()
            {
                Id = $"StringArray {extendedPropertyGuid} Name BlahArray",
                Value = new List<string>()
                    {
                        "A",
                        "B",
                        "C"
                    }
            });

            await msg.SaveAsync(inbox);

            MessageView msgView = new MessageView(1);
            msgView.PropertySet.Add(new ExtendedPropertyDefinition(
                MapiPropertyType.String,
                "Blah",
                new Guid(extendedPropertyGuid)));

            msgView.PropertySet.Add(new ExtendedPropertyDefinition(
                MapiPropertyType.StringArray,
                "BlahArray",
                new Guid(extendedPropertyGuid)));

            SearchFilter filter = new SearchFilter.IsEqualTo(
                MessageObjectSchema.Subject,
                subject);
            
            FindItemResults<Message> findItemsResults = await exchangeService.FindItems(
                WellKnownFolderName.Inbox.ToString(),
                msgView,
                filter);

            foreach (Message item in findItemsResults)
            {
                msg = (Message)item;
                Assert.AreEqual(
                    1,
                    msg.SingleValueExtendedProperties.Count);

                Assert.AreEqual(
                    1,
                    msg.MultiValueExtendedProperties.Count);

                await msg.DeleteAsync();
            }
        }

        /// <summary>
        /// Find message call.
        /// </summary>
        public static async Task FindMessage(ExchangeService exchangeService)
        {
            string folderName = "TestFindItemFolder";
            exchangeService.LogFlag = LogFlag.All;
            exchangeService.LoggingEnabled = true;
            await FunctionalTestHelpers.DeleteFolderIfExist(
                folderName,
                exchangeService,
                WellKnownFolderName.MsgFolderRoot);

            MailFolder folder = await FunctionalTestHelpers.CreateFolder(
                folderName,
                exchangeService,
                WellKnownFolderName.MsgFolderRoot);

            for (int i = 0; i < 9; i++)
            {
                await FunctionalTestHelpers.CreateMessage(
                    1,
                    folder,
                    exchangeService);
            }

            for (int i = 0; i < 10; i++)
            {
                await FunctionalTestHelpers.CreateMessage(
                    i,
                    folder,
                    exchangeService);
            }

            // there are 10 "Test msg 1" and 9 others. Expecting to see
            // sync 5 times.
            SearchFilter subjectFilter = new SearchFilter.IsEqualTo(
                MessageObjectSchema.Subject,
                "Test msg 1");

            MessageView mv = new MessageView(2);
            FindItemResults<Message> items = null;
            int counter = 0;
            do
            {
                items = await exchangeService.FindItems(
                    folder.Id,
                    mv,
                    subjectFilter);

                mv.Offset += mv.PageSize;
                counter++;

            } while (items.MoreAvailable);

            Assert.AreEqual(
                6,
                counter);
        }

        /// <summary>
        /// Sync messages
        /// </summary>
        public static async Task SyncMessages(ExchangeService exchangeService)
        {
            string folderName = "TempSyncFolder";

            await FunctionalTestHelpers.DeleteFolderIfExist(
                folderName,
                exchangeService,
                WellKnownFolderName.MsgFolderRoot);

            MailFolder folder = await FunctionalTestHelpers.CreateFolder(
                folderName,
                exchangeService,
                WellKnownFolderName.MsgFolderRoot);

            for (int i = 0; i < 10; i++)
            {
                await FunctionalTestHelpers.CreateMessage(
                    i,
                    folder,
                    exchangeService);
            }

            string syncState = null;
            MessagePropertySet propertySet = new MessagePropertySet();
            propertySet.Add(MessageObjectSchema.ToRecipients);
            ChangeCollection<MessageChange> syncCollection;
            int counter = 0;
            int numberOfMessages = 0;
            do
            {
                syncCollection = await exchangeService.SyncFolderItems(
                    folder.Id,
                    propertySet,
                    syncState);

                syncState = syncCollection.SyncState;
                numberOfMessages += syncCollection.TotalCount;
                counter++;

                foreach (MessageChange itemChange in syncCollection.Items)
                {
                    Assert.IsTrue(
                        itemChange.ChangeType == ChangeType.Created);
                }

            } while (syncCollection.MoreAvailable || counter == 4);

            Assert.IsFalse(syncCollection.MoreAvailable);
            Assert.AreEqual(10, numberOfMessages);

            FindItemResults<Message> items = await exchangeService.FindItems(folder.Id, new MessageView(4));

            for (int i = 0; i < items.TotalCount; i++)
            {
                Message msg = items.Items[i];
                if (i < 2)
                {
                    msg.IsRead = false;
                    await msg.UpdateAsync();
                }
                else
                {
                    await msg.DeleteAsync();
                }
            }

            syncCollection = await exchangeService.SyncFolderItems(
                folder.Id,
                propertySet,
                syncState);

            Assert.IsFalse(syncCollection.MoreAvailable);
            Assert.IsTrue(
                syncCollection.TotalCount == 4);

            int changes = syncCollection.Items.Count(i => i.ChangeType == ChangeType.Deleted);
            Assert.AreEqual(
                2,
                changes);

            changes = syncCollection.Items.Count(i => i.ChangeType == ChangeType.Updated);
            Assert.IsTrue(changes == 2);

            await folder.DeleteAsync();
        }

        /// <summary>
        /// Create, read, update, delete message.
        /// </summary>
        /// <param name="exchangeService">Exchange service.</param>
        /// <returns></returns>
        public static async Task CreateReadUpdateDeleteMessage(ExchangeService exchangeService)
        {
            string folderName = "TestCrudItems";
            await FunctionalTestHelpers.DeleteFolderIfExist(
                folderName,
                exchangeService,
                WellKnownFolderName.MsgFolderRoot);

            MailFolder mailFolder = await FunctionalTestHelpers.CreateFolder(
                folderName,
                exchangeService,
                WellKnownFolderName.MsgFolderRoot);

            for (int i = 0; i < 10; i++)
            {
                Message msg = new Message(exchangeService);
                msg.Subject = Guid.NewGuid().ToString();
                msg.Body = new ItemBody()
                {
                    ContentType = BodyType.Html,
                    Content = $"body {Guid.NewGuid().ToString()}"
                };

                await msg.SaveAsync(mailFolder);
            }

            FindItemResults<Message> items = await exchangeService.FindItems(mailFolder.Id, new MessageView(12));
            Assert.AreEqual(
                10,
                items.TotalCount);

            foreach (Message item in items)
            {
                item.Subject = $"Changed subject - {item.Subject}";
                await item.UpdateAsync();
            }

            items = await exchangeService.FindItems(mailFolder.Id, new MessageView(12));
            foreach (Message item in items)
            {
                Assert.IsTrue(
                    item.Subject.StartsWith("Changed subject -"));

                await item.DeleteAsync();
            }
        }
    }
}