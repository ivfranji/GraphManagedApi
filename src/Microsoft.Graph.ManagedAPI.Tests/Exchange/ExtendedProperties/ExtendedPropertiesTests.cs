namespace Microsoft.Graph.ManagedAPI.UnitTests.Exchange
{
    using System;
    using Microsoft.Graph.Exchange;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    /// <summary>
    /// Extended properties tests.
    /// </summary>
    [TestClass]
    public class ExtendedPropertiesTests
    {
        /// <summary>
        /// Tests the extended property to single value extended property.
        /// </summary>
        [TestMethod]
        public void Test_ExtendedPropertyToSingleValueExtendedProperty()
        {
            ExtendedPropertyDefinition extendedPropertyDefinition = new ExtendedPropertyDefinition(
                MapiPropertyType.String,
                0x1234);

            SingleValueLegacyExtendedProperty singleValueLegacyExtendedProperty =
                (SingleValueLegacyExtendedProperty) extendedPropertyDefinition;

            Assert.AreEqual(
                "String 0x1234",
                singleValueLegacyExtendedProperty.Id);

            Assert.IsNull(singleValueLegacyExtendedProperty.Value);
        }

        /// <summary>
        /// Tests the type of the extended property to single value extended property throw exception on wrong.
        /// </summary>
        [TestMethod]
        public void Test_ExtendedPropertyToSingleValueExtendedPropertyThrowExceptionOnWrongType()
        {
            ExtendedPropertyDefinition extendedPropertyDefinition = new ExtendedPropertyDefinition(
                MapiPropertyType.StringArray,
                0x1234);

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                SingleValueLegacyExtendedProperty singleValueLegacyExtendedProperty =
                    (SingleValueLegacyExtendedProperty) extendedPropertyDefinition;
            });
        }

        /// <summary>
        /// Tests the type of the extended property to single value extended property throw exception on wrong.
        /// </summary>
        [TestMethod]
        public void Test_ExtendedPropertyToSingleValueExtendedPropertyWithGuid()
        {
            Guid g = Guid.NewGuid();
            ExtendedPropertyDefinition extendedPropertyDefinition = new ExtendedPropertyDefinition(
                MapiPropertyType.Binary,
                "Prop",
                g);

            SingleValueLegacyExtendedProperty singleValueLegacyExtendedProperty =
                (SingleValueLegacyExtendedProperty)extendedPropertyDefinition;
            Assert.AreEqual(
                $"Binary {{{g}}} Name Prop",
                singleValueLegacyExtendedProperty.Id);

            Assert.IsNull(singleValueLegacyExtendedProperty.Value);
        }
    }
}
