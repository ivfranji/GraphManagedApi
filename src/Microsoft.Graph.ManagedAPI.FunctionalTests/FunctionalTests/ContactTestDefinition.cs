namespace Microsoft.Graph.ManagedAPI.FunctionalTests
{
    using System;
    using System.Collections.Generic;
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

        /// <summary>
        /// Creates the contact with categories.
        /// </summary>
        /// <param name="exchangeService">The exchange service.</param>
        public static async Task CreateContactWithCategories(ExchangeService exchangeService)
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
            contact.Categories = new List<string>()
            {
                "MyContactCategory"
            };

            await contact.SaveAsync();

            SearchFilter searchFilter = new SearchFilter.IsEqualTo(
                ContactObjectSchema.DisplayName,
                displayName);

            ContactView contactView = new ContactView(10);
            contactView.PropertySet.Add(ContactObjectSchema.Categories);
            FindItemResults<Contact> contacts = await exchangeService.FindItems<Contact>(contactView, searchFilter);

            Assert.AreEqual(
                "MyContactCategory",
                contacts.Items[0].Categories[0]);

            await contacts.Items[0].DeleteAsync();
        }
    }
}
