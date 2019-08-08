namespace Microsoft.Graph.ManagedAPI.Tests.ChangeTracking
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.Graph.ChangeTracking;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Property definition tests.
    /// </summary>
    [TestClass]
    public class PropertyDefinitionTests
    {
        /// <summary>
        /// Test property definition behavior.
        /// </summary>
        [TestMethod]
        public void Test_PropertyDefinitionBehavior()
        {
            PropertyDefinition propDef = new PropertyDefinition(
                "Recipients",
                typeof(IList<string>));

            Assert.AreEqual(
                "Recipients",
                propDef.Name);

            Assert.IsTrue(propDef.IsEnumerable);
            Assert.IsNotNull(propDef.DefaultValue);

            Assert.AreEqual(
                typeof(IList<string>),
                propDef.Type);

            Assert.AreEqual(
                typeof(string),
                propDef.IEnumerableUnderlyingType);

            Assert.IsTrue(propDef.TrackChanges);
            Assert.IsFalse(propDef.ChangeTrackable); // change trackable property is one implementing IPropertyChangeTracking.

            propDef = new PropertyDefinition(
                "ItemCount",
                typeof(int),
                false);

            // value type should be instantiated with respective
            // default value.
            Assert.AreEqual(
                0, 
                propDef.DefaultValue);

            Assert.IsFalse(propDef.IsEnumerable);
            Assert.IsNull(propDef.IEnumerableUnderlyingType);
            Assert.IsFalse(propDef.TrackChanges);
            Assert.IsFalse(propDef.ChangeTrackable);
        }

        /// <summary>
        /// Test navigation property activation.
        /// </summary>
        [TestMethod]
        public void Test_PropertyDefinitionNavigationActivation()
        {
            PropertyDefinition propDef = new PropertyDefinition(
                "Messages",
                typeof(NavigationProperty<Message>));

            Assert.IsTrue(propDef.IsNavigationProperty);
            Assert.AreEqual(
                typeof(Message),
                propDef.NavigationPropertyUnderlyingType);

            Assert.IsFalse(propDef.IsEnumerable);

            Assert.AreEqual(
                typeof(NavigationProperty<Message>),
                propDef.Type);

            PropertyBag propBag = new PropertyBag(new MessageObjectSchema());
            NavigationProperty<Message> navigationProperty = (NavigationProperty<Message>) propDef.ActivateNavigationProperty(propBag);

            Assert.AreEqual(
                "Messages",
                navigationProperty.RelativePath);
        }

        /// <summary>
        /// Test list activation.
        /// </summary>
        [TestMethod]
        public void Test_PropertyDefinitionListActivation()
        {
            PropertyDefinition propDef = new PropertyDefinition(
                "Events",
                typeof(IList<Event>));

            Assert.IsTrue(propDef.Type == typeof(IList<Event>));
            Assert.IsTrue(propDef.IsEnumerable);
            Assert.IsTrue(propDef.IEnumerableUnderlyingType == typeof(Event));
            object observable = propDef.ActivateObservableList(null);
            Assert.IsInstanceOfType(observable, typeof(ObservableCollection<Event>));

            IList<Event> outlookItems = new List<Event>();
            outlookItems.Add(new Event());
            observable = propDef.ActivateObservableList(outlookItems);
            Assert.IsInstanceOfType(observable, typeof(ObservableCollection<Event>));

            ObservableCollection<Event> observableCollection = (ObservableCollection<Event>) observable;
            Assert.IsTrue(observableCollection.Count == 1);

            IList<object> eventCollection = propDef.ActivateList(observableCollection);
            Assert.IsTrue(eventCollection.Count == 1);
        }
    }
}
