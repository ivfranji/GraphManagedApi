namespace Microsoft.Graph.ManagedAPI.Tests.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Graph.ChangeTracking;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Property bag tests.
    /// </summary>
    [TestClass]
    public class PropertyBagTests
    {
        /// <summary>
        /// Property bag default behavior.
        /// </summary>
        [TestMethod]
        public void Test_PropertyBagBehavior()
        {
            PropertyBag propertyBag = new PropertyBag(new MessageObjectSchema());

            int changedPropsCount = propertyBag.GetChangedProperties().Count();
            Assert.AreEqual(
                0,
                changedPropsCount);

            Assert.IsNotNull(propertyBag[MessageObjectSchema.SingleValueExtendedProperties]);
            IList<SingleValueLegacyExtendedProperty> props = (IList<SingleValueLegacyExtendedProperty>) propertyBag[MessageObjectSchema.SingleValueExtendedProperties];
            Assert.AreEqual(
                0,
                props.Count);

            props.Add(new SingleValueLegacyExtendedProperty()
            {
                Id = "PropId",
                Value = "A"
            });

            changedPropsCount = propertyBag.GetChangedProperties().Count();
            Assert.AreEqual(
                1,
                changedPropsCount);

            propertyBag.ResetChangeFlag();
            changedPropsCount = propertyBag.GetChangedProperties().Count();
            Assert.AreEqual(
                0,
                changedPropsCount);

            Assert.IsNull(propertyBag[MessageObjectSchema.Id]);

            propertyBag[MessageObjectSchema.Id] = "ABCD";
            changedPropsCount = propertyBag.GetChangedProperties().Count();
            Assert.AreEqual(
                1,
                changedPropsCount);

            Assert.IsInstanceOfType(
                propertyBag[MessageObjectSchema.Id], 
                typeof(string));

            string id = propertyBag[MessageObjectSchema.Id] as string;
            Assert.AreEqual(
                "ABCD",
                id);

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                object o = propertyBag[null];
            });

            Assert.ThrowsException<KeyNotFoundException>(() =>
            {
                object o = propertyBag[MailFolderObjectSchema.MessageRules];
            });

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                propertyBag[MessageObjectSchema.Body] = "Body is not string...";
            });
        }

        /// <summary>
        /// Test property bag defaults.
        /// </summary>
        [TestMethod]
        public void Test_PropertyBagDefaults()
        {
            PropertyBag propertyBag = new PropertyBag(new MessageObjectSchema());
            Assert.AreEqual(
                0,
                propertyBag.GetChangedProperties().Count());

            foreach (FieldInfo fieldInfo in typeof(MessageObjectSchema).GetFields(
                BindingFlags.Static | 
                BindingFlags.Public | 
                BindingFlags.FlattenHierarchy))
            {
                PropertyDefinition propertyDefinition = fieldInfo.GetValue(null) as PropertyDefinition;
                if (null != propertyDefinition)
                {
                    if (propertyDefinition.IsEnumerable)
                    {
                        Assert.IsNotNull(propertyBag[propertyDefinition]);
                        Assert.IsTrue(propertyDefinition.IsEnumerable);
                    }

                    else if (propertyDefinition.IsNavigationProperty)
                    {
                        Assert.IsNotNull(propertyBag[propertyDefinition]);
                    }

                    else
                    {
                        Assert.AreEqual(
                            propertyDefinition.DefaultValue,
                            propertyBag[propertyDefinition]);
                    }
                }
                else
                {
                    Assert.Fail("Schema shouldn't contain anything but static props.");
                }
            }
        }
    }
}