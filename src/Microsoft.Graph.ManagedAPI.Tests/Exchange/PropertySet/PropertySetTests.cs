namespace Microsoft.Graph.ManagedAPI.Tests.Exchange.PropertySet
{
    using System;
    using Microsoft.Graph.Exchange;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Property set tests.
    /// </summary>
    [TestClass]
    public class PropertySetTests
    {
        /// <summary>
        /// Test item property set.
        /// </summary>
        [TestMethod]
        public void Test_ItemPropertySet()
        {
            ItemPropertySet itemProperty = new ItemPropertySet(new MessageObjectSchema(), MessageObjectSchema.Subject, MessageObjectSchema.CreatedDateTime);
            Assert.IsNull(itemProperty.GetUrlQuery());

            itemProperty.Add(MessageObjectSchema.Body);
            Assert.AreEqual(
                "$select=Id,Subject,CreatedDateTime,Body",
                itemProperty.GetUrlQuery());

            Assert.ThrowsException<ArgumentException>(() =>
            {
                itemProperty.Add(MailFolderObjectSchema.ChildFolders);
            });

            itemProperty.Add(new ExtendedPropertyDefinition(MapiPropertyType.String, 0x0D7A));

            Assert.AreEqual(
                "$select=Id,Subject,CreatedDateTime,Body&$expand=SingleValueExtendedProperties($filter=Id eq 'String 0x0D7A')",
                itemProperty.GetUrlQuery());

            itemProperty.Expand(MessageObjectSchema.Attachments);

            Assert.AreEqual(
                "$select=Id,Subject,CreatedDateTime,Body&$expand=SingleValueExtendedProperties($filter=Id eq 'String 0x0D7A'),Attachments",
                itemProperty.GetUrlQuery());

            itemProperty.Expand(new ExtendedPropertyDefinition(MapiPropertyType.DoubleArray, 0xABCD));
            itemProperty.Expand(MessageObjectSchema.InternetMessageHeaders);

            Assert.AreEqual(
                "$select=Id,Subject,CreatedDateTime,Body&$expand=SingleValueExtendedProperties($filter=Id eq 'String 0x0D7A'),MultiValueExtendedProperties($filter=Id eq 'DoubleArray 0xABCD'),Attachments,InternetMessageHeaders",
                itemProperty.GetUrlQuery());

            itemProperty.Expand(new ExtendedPropertyDefinition(MapiPropertyType.Binary, 0x1234));
            itemProperty.Expand(new ExtendedPropertyDefinition(MapiPropertyType.BinaryArray, 0x1235));

            Assert.AreEqual(
                "$select=Id,Subject,CreatedDateTime,Body&$expand=SingleValueExtendedProperties($filter=Id eq 'String 0x0D7A' or Id eq 'Binary 0x1234'),MultiValueExtendedProperties($filter=Id eq 'DoubleArray 0xABCD' or Id eq 'BinaryArray 0x1235'),Attachments,InternetMessageHeaders",
                itemProperty.GetUrlQuery());
        }
    }
}
