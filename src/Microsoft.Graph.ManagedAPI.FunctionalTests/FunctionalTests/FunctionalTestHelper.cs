namespace Microsoft.Graph.ManagedAPI.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Graph.Exchange;

    /// <summary>
    /// Test helpers.
    /// </summary>
    internal static class FunctionalTestHelpers
    {
        /// <summary>
        /// Delete folder if it exist.
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="exchangeService"></param>
        /// <param name="folderRoot"></param>
        internal static async Task DeleteFolderIfExist(string folderName, ExchangeService exchangeService, WellKnownFolderName folderRoot)
        {
            FindMailFolderResults findFolders = await exchangeService.FindFolders(
                folderRoot,
                new MailFolderView());

            foreach (MailFolder mailFolder in findFolders)
            {
                if (mailFolder.DisplayName == folderName)
                {
                    await mailFolder.DeleteAsync();
                }
            }
        }

        /// <summary>
        /// Create folder
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="exchangeService"></param>
        /// <returns></returns>
        internal static async Task<MailFolder> CreateFolder(string folderName, ExchangeService exchangeService, WellKnownFolderName folderRoot)
        {
            MailFolder folder = new MailFolder(exchangeService);
            folder.DisplayName = folderName;
            MailFolder mailFolderRoot = await exchangeService.GetAsync<MailFolder>(
                new EntityPath(
                    folderRoot.ToString(), 
                    typeof(MailFolder)));

            await  folder.SaveAsync(mailFolderRoot);
            return folder;
        }

        /// <summary>
        /// Create message in the folder.
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="mailFolder"></param>
        /// <param name="exchangeService"></param>
        internal static async Task CreateMessage(int messageId, MailFolder mailFolder, ExchangeService exchangeService)
        {
            Message message = new Message(exchangeService);
            message.Subject = $"Test msg {messageId}";
            message.Body = new ItemBody()
            {
                ContentType = BodyType.Html,
                Content = $"This is test message for sync {messageId}"
            };

            message.ToRecipients = new List<Recipient>()
            {
                new Recipient()
                {
                    EmailAddress = new EmailAddress()
                    {
                        Address = $"abc{messageId}@def.com"
                    }
                }
            };

            await message.SaveAsync(mailFolder);
        }

        /// <summary>
        /// Get formatted date/time.
        /// </summary>
        /// <param name="hoursToAdd"></param>
        /// <returns></returns>
        internal static DateTime GetFormattedDateTime(int hoursToAdd = 2)
        {
            DateTime dateTime = DateTime.UtcNow.AddHours(hoursToAdd);
            DateTime roundDateTime = new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute - (dateTime.Minute % 15),
                0);

            return roundDateTime;
        }
    }
}
