namespace Microsoft.Graph.ManagedAPI.Tests.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Graph.ChangeTracking;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PropertyChangeTest
    {
        /// <summary>
        /// Test collection properties.
        /// </summary>
        [TestMethod]
        public void Test_CollectionProperties()
        {
            Message msg = new Message();

            // collections are always instantiated in background.
            Assert.IsNotNull(msg.ToRecipients);
            Assert.AreEqual(
                0,
                msg.GetChangedProperties().Count());

            msg.ToRecipients = new List<Recipient>();

            Assert.AreEqual(
                1,
                msg.GetChangedProperties().Count());

            foreach (PropertyDefinition propertyDefinition in msg.GetChangedProperties())
            {
                Assert.AreEqual(
                    "ToRecipients",
                    propertyDefinition.Name);
            }

            msg.BccRecipients.Add(new Recipient());
            Assert.AreEqual(
                2,
                msg.GetChangedProperties().Count());
        }

        /// <summary>
        /// Inner property tracking.
        /// </summary>
        [TestMethod]
        public void Test_InnerPropertyTracking()
        {
            ItemAttachment itemAttachment = new ItemAttachment()
            {
                Name = "Item attach",
                Item = new Event()
                {
                    Subject = "Lets meet?",
                    Attendees = new List<Attendee>()
                    {
                        new Attendee()
                        {
                            EmailAddress = new EmailAddress()
                            {
                                Address = "attendee@domain.com"
                            }
                        }
                    },

                    Body = new ItemBody()
                    {
                        ContentType = BodyType.Html,
                        Content = "Meeting!"
                    }
                }
            };

            Assert.AreEqual(
                2,
                itemAttachment.GetChangedProperties().Count());

            Assert.AreEqual(
                3,
                itemAttachment.Item.GetChangedProperties().Count());
        }

        /// <summary>
        /// Test inner property list tracking.
        /// </summary>
        [TestMethod]
        public void Test_InnerPropertyListTracking()
        {
            DateTime startTime = DateTimeHelper.GetFormattedDateTime();
            DateTime endTime = DateTimeHelper.GetFormattedDateTime(2);
            string timeZone = "Central European Standard Time";

            Message msg = new Message();
            msg.Subject = "Test item attachment";
            ItemAttachment attach = new ItemAttachment()
            {
                Name = "Event Item",
                Item = new Event()
                {
                    Attendees = new List<Attendee>()
                    {
                        new Attendee()
                        {
                            EmailAddress = new EmailAddress() { Address = "attendee1@t.com" }
                        }
                    },
                    Body = new ItemBody()
                    {
                        ContentType = BodyType.Html,
                        Content = "Lets meet up"
                    },

                    Start = new DateTimeTimeZone()
                    {
                        DateTime = startTime.ToString(),
                        TimeZone = timeZone.ToString()
                    },

                    End = new DateTimeTimeZone()
                    {
                        DateTime = endTime.ToString(),
                        TimeZone = timeZone
                    }
                }
            };

            msg.Attachments.Add(attach);

            Assert.AreEqual(
                2,
                msg.GetChangedProperties().Count());

            Assert.AreEqual(
                2,
                msg.Attachments[0].GetChangedProperties().Count());

            Assert.AreEqual(
                4,
                ((ItemAttachment)msg.Attachments[0]).Item.GetChangedProperties().Count());
        }
    }
}