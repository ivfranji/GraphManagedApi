namespace Microsoft.Graph.ManagedAPI.Tests.QueryAndView
{
    using System;
    using Microsoft.Graph.Exchange;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// View tests.
    /// </summary>
    [TestClass]
    public class ViewTests
    {
        /// <summary>
        /// Test contact view.
        /// </summary>
        [TestMethod]
        public void Test_ContactView()
        {
            ViewBase contactView = new ContactView(10);
            Assert.AreEqual(
                typeof(Contact),
                contactView.ViewType);

            Assert.AreEqual(
                "$top=10&$skip=0",
                contactView.GetUrlQuery());

            contactView.Offset += 8;
            contactView.PropertySet.Expand(ContactObjectSchema.Manager);

            Assert.AreEqual(
                "$top=10&$skip=8&$expand=Manager",
                contactView.GetUrlQuery());

            contactView.ValidateViewTypeSupported(typeof(Contact));

            Assert.ThrowsException<ArgumentException>((() =>
            {
                contactView.ValidateViewTypeSupported(typeof(MailFolder));
            }));
        }

        /// <summary>
        /// Test event view.
        /// </summary>
        [TestMethod]
        public void Test_EventView()
        {
            ViewBase eventView = new EventView();
            Assert.AreEqual(
                typeof(Event),
                eventView.ViewType);

            Assert.AreEqual(
                "$top=10&$skip=0",
                eventView.GetUrlQuery());

            eventView.PageSize += 8;
            eventView.PropertySet.Expand(EventObjectSchema.Type);
            eventView.PropertySet.Expand(new ExtendedPropertyDefinition(MapiPropertyType.String, 0x1234));
            eventView.PropertySet.Add(EventObjectSchema.Body);

            Assert.AreEqual(
                "$top=18&$skip=0&$select=Id,Start,End,Location,Body&$expand=SingleValueExtendedProperties($filter=Id eq 'String 0x1234'),Type",
                eventView.GetUrlQuery());

            eventView.ValidateViewTypeSupported(typeof(Event));

            Assert.ThrowsException<ArgumentException>((() =>
            {
                eventView.ValidateViewTypeSupported(typeof(Contact));
            }));
        }

        /// <summary>
        /// Test message view.
        /// </summary>
        [TestMethod]
        public void Test_MessageView()
        {
            ViewBase messageView = new MessageView();
            Assert.AreEqual(
                typeof(Message),
                messageView.ViewType);

            Assert.AreEqual(
                "$top=10&$skip=0",
                messageView.GetUrlQuery());

            Assert.ThrowsException<ArgumentException>(() =>
            {
                messageView.PropertySet.Expand(EventObjectSchema.Type);
            });

            messageView.PropertySet.Expand(new ExtendedPropertyDefinition(MapiPropertyType.String, 0x1234));
            messageView.PropertySet.Add(MessageObjectSchema.Body);
            messageView.PageSize += 6;
            messageView.Offset += 12;

            Assert.AreEqual(
                "$top=16&$skip=12&$select=Id,IsRead,Subject,ParentFolderId,CreatedDateTime,Body&$expand=SingleValueExtendedProperties($filter=Id eq 'String 0x1234')",
                messageView.GetUrlQuery());

            messageView.ValidateViewTypeSupported(typeof(Message));

            Assert.ThrowsException<ArgumentException>((() =>
            {
                messageView.ValidateViewTypeSupported(typeof(Contact));
            }));
        }
    }
}
