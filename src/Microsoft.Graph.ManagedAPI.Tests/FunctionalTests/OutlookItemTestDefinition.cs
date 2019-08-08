namespace Microsoft.Graph.ManagedAPI.Tests.FunctionalTests
{
    using System.Threading.Tasks;
    using Microsoft.Graph.Exchange;
    using Search;

    internal static class OutlookItemTestDefinition
    {
        /// <summary>
        /// Find event items.
        /// </summary>
        /// <param name="exchangeService">Exchange service.</param>
        /// <returns></returns>
        public static async Task FindEventItems(ExchangeService exchangeService)
        {
            ExtendedPropertyDefinition prop = new ExtendedPropertyDefinition(
                MapiPropertyType.String,
                0x001A);

            ViewBase itemView = new EventView();
            itemView.PropertySet.Add(prop);

            FindItemResults<Event> events = await exchangeService.FindItems<Event>(itemView);
        }

        /// <summary>
        /// Find message items.
        /// </summary>
        /// <param name="exchangeService"></param>
        /// <returns></returns>
        public static async Task FindMessageItems(ExchangeService exchangeService)
        {
            MailFolder inboxFolder = await exchangeService.GetAsync<MailFolder>(
                new EntityPath(
                    WellKnownFolderName.DeletedItems.ToString(), 
                    typeof(MailFolder)));

            SearchFilter searchFilter = new SearchFilter.IsEqualTo(
                MessageObjectSchema.ParentFolderId,
                inboxFolder.Id);

            ViewBase itemView = new MessageView(7);
            FindItemResults<Message> messages = await exchangeService.FindItems<Message>(itemView, searchFilter);
        }

        /// <summary>
        /// Find message items.
        /// </summary>
        /// <param name="exchangeService"></param>
        /// <returns></returns>
        public static async Task FindContactItems(ExchangeService exchangeService)
        {
            ViewBase itemView = new ContactView(7);
            FindItemResults<Contact> messages = await exchangeService.FindItems<Contact>(itemView);
        }
    }
}
