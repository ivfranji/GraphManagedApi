namespace Microsoft.Graph.ManagedAPI.Tests.FunctionalTests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Graph.Exchange;
    using Microsoft.Graph.Search;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Contact test definition.
    /// </summary>
    internal static class ContactTestDefinition
    {
        /// <summary>
        /// CRUD operations for contact.
        /// </summary>
        /// <param name="exchangeService"></param>
        public static async Task CreateReadUpdateDeleteContact(ExchangeService exchangeService)
        {
            string displayName = Guid.NewGuid().ToString();
            Contact contact = new Contact(exchangeService);
            contact.DisplayName = displayName;
            contact.Department = "Dept";
            contact.GivenName = "First Name";
            contact.EmailAddresses.Add(new TypedEmailAddress()
            {
                Address = "test@test.com"
            });

            await contact.SaveAsync();

            SearchFilter searchFilter = new SearchFilter.IsEqualTo(
                ContactObjectSchema.DisplayName,
                displayName);

            ContactView contactView = new ContactView(10);
            FindItemResults<Contact> contacts = await exchangeService.FindItems<Contact>(contactView, searchFilter);

            Assert.AreEqual(
                1,
                contacts.TotalCount);

            contact.AssistantName = "Contact Assistant";
            await contact.UpdateAsync();
            contacts = await exchangeService.FindItems<Contact>(contactView, searchFilter);

            Assert.AreEqual(
                1,
                contacts.TotalCount);

            Assert.AreEqual(
                "Contact Assistant",
                contacts.Items[0].AssistantName);

            await contact.DeleteAsync();
            contacts = await exchangeService.FindItems<Contact>(contactView, searchFilter);
            Assert.AreEqual(
                0,
                contacts.TotalCount);
        }
    }
}
