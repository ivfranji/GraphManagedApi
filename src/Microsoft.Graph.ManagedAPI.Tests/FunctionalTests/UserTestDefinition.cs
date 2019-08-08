namespace Microsoft.Graph.ManagedAPI.Tests.FunctionalTests
{
    using System.Threading.Tasks;
    using Microsoft.Graph.Exchange;

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
    }
}
