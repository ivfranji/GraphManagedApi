namespace Microsoft.Graph.ManagedAPI.Tests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Graph.Exchange;
    using VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// User test definition.
    /// </summary>
    internal static class UserTestDefinition
    {
        /// <summary>
        /// Get user.
        /// </summary>
        /// <param name="exchangeService"></param>
        /// <returns></returns>
        public static async Task GetUser(ExchangeService exchangeService)
        {
            User user = await exchangeService.GetCurrentUser();
            int counter = 0;
            FindEntityResults<Message> messages;
            do
            {
                messages = await user.Messages.GetNextPage();
                counter++;
            } while (messages.MoreAvailable && counter < 10);

            FindEntityResults<MailFolder> mailFolders;
            do
            {
                mailFolders = await user.MailFolders.GetNextPage();
                foreach (MailFolder mailFolder in mailFolders.Items)
                {
                    if (mailFolder.DisplayName == "Inbox")
                    {
                        messages = await mailFolder.Messages.GetNextPage();
                    }
                }
            } while (mailFolders.MoreAvailable);
        }

        /// <summary>
        /// Get user availability.
        /// </summary>
        /// <param name="exchangeServiceA">Exchange service of mailbox A.</param>
        /// <param name="exchangeServiceB">Exchange service of mailbox B.</param>
        /// <returns></returns>
        public static async Task GetUserAvailability(ExchangeService exchangeServiceA, ExchangeService exchangeServiceB)
        {
            string subject = Guid.NewGuid().ToString();
            Event mailboxBEvent = new Event(exchangeServiceB);
            mailboxBEvent.Subject = subject;
            mailboxBEvent.Start = new DateTimeTimeZone()
            {
                DateTime = DateTimeHelper.GetFormattedDateTime(4).ToString("yyyy-MM-ddThh:mm:ss"),
                TimeZone = "UTC"
            };

            mailboxBEvent.End = new DateTimeTimeZone()
            {
                DateTime = DateTimeHelper.GetFormattedDateTime(5).ToString("yyyy-MM-ddThh:mm:ss"),
                TimeZone = "UTC"
            };

            await mailboxBEvent.SaveAsync();

            // sleep a bit to ensure event is
            // saved.
            Thread.Sleep(1500);
            List<string> users = new List<string>();
            users.Add(AppConfig.MailboxB);
            DateTimeTimeZone start = new DateTimeTimeZone()
            {
                DateTime = DateTimeHelper.GetFormattedDateTime().ToString("yyyy-MM-ddThh:mm:ss"),
                TimeZone = "UTC"
            };

            DateTimeTimeZone end = new DateTimeTimeZone()
            {
                DateTime = DateTimeHelper.GetFormattedDateTime(72).ToString("yyyy-MM-ddThh:mm:ss"),
                TimeZone = "UTC"
            };
            
            IList<ScheduleInformation> availability = await exchangeServiceA.GetUserAvailability(
                users, 
                start, 
                end, 
                60);

            Assert.AreEqual(
                1,
                availability.Count);

            bool hasItem = false;
            foreach (ScheduleItem item in availability[0].ScheduleItems)
            {
                if (item.Subject == subject)
                {
                    Assert.AreEqual(
                        FreeBusyStatus.Busy,
                        item.Status);
                    hasItem = true;
                }
            }

            Assert.IsTrue(hasItem);
            await mailboxBEvent.DeleteAsync();
        }

        /// <summary>
        /// Get mailbox settings.
        /// </summary>
        /// <param name="exchangeService">Exchange service.</param>
        /// <returns></returns>
        public static async Task GetMailboxSettings(ExchangeService exchangeService)
        {
            // set
            MailboxSettings mailboxSettings = new MailboxSettings()
            {
                AutomaticRepliesSetting = new AutomaticRepliesSetting()
                {
                    ExternalAudience = ExternalAudienceScope.All,
                    ExternalReplyMessage = "This is external OOF",
                    InternalReplyMessage = "This is internal OOF",
                    Status = AutomaticRepliesStatus.AlwaysEnabled
                }
            };
            
            // update will return new settings, however using get to test that part of the code
            await exchangeService.UpdateMailboxSettings(mailboxSettings);
            mailboxSettings = await exchangeService.GetMailboxSettings();

            Assert.AreEqual(
                "This is external OOF",
                mailboxSettings.AutomaticRepliesSetting.ExternalReplyMessage);

            Assert.AreEqual(
                "This is internal OOF",
                mailboxSettings.AutomaticRepliesSetting.InternalReplyMessage);

            Assert.AreEqual(
                AutomaticRepliesStatus.AlwaysEnabled,
                mailboxSettings.AutomaticRepliesSetting.Status);

            Assert.AreEqual(
                ExternalAudienceScope.All,
                mailboxSettings.AutomaticRepliesSetting.ExternalAudience);

            // revert - non Entity objects doesn't currently support property change tracking
            // so setting non required properties to null. Future versions will support
            // tracking and this no longer will be issue and necessary. 
            mailboxSettings.AutomaticRepliesSetting.ExternalAudience = ExternalAudienceScope.None;
            mailboxSettings.AutomaticRepliesSetting.ExternalReplyMessage = "";
            mailboxSettings.AutomaticRepliesSetting.InternalReplyMessage = "";
            mailboxSettings.AutomaticRepliesSetting.Status = AutomaticRepliesStatus.Disabled;
            mailboxSettings.ArchiveFolder = null;
            mailboxSettings.TimeZone = null;
            mailboxSettings.Language = null;
            mailboxSettings.WorkingHours = null;

            await exchangeService.UpdateMailboxSettings(mailboxSettings);
            mailboxSettings = await exchangeService.GetMailboxSettings();

            Assert.AreEqual(
                "",
                mailboxSettings.AutomaticRepliesSetting.ExternalReplyMessage);

            Assert.AreEqual(
                "",
                mailboxSettings.AutomaticRepliesSetting.InternalReplyMessage);

            Assert.AreEqual(
                AutomaticRepliesStatus.Disabled,
                mailboxSettings.AutomaticRepliesSetting.Status);

            Assert.AreEqual(
                ExternalAudienceScope.None,
                mailboxSettings.AutomaticRepliesSetting.ExternalAudience);
        }
    }
}
