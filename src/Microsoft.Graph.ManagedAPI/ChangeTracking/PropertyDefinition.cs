namespace Microsoft.Graph.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Property definition.
    /// </summary>
    public class PropertyDefinition
    {
        /// <summary>
        /// Default value.
        /// </summary>
        private object defaultValue;

        /// <summary>
        /// IEnumerable type interface name.
        /// </summary>
        private const string EnumerableTypeInterfaceName = "IEnumerable";

        /// <summary>
        /// Create new instance of <see cref="PropertyDefinition"/>.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="type">Property type.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <param name="trackChanges">Track property changes.</param>
        internal PropertyDefinition(string name, Type type, object defaultValue, bool trackChanges = true)
            : this(name, type, trackChanges)
        {
            this.defaultValue = defaultValue;
        }

        /// <summary>
        /// Create new instance of <see cref="PropertyDefinition"/>
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="type">Property type.</param>
        /// <param name="trackChanges">Track property changes.</param>
        internal PropertyDefinition(string name, Type type, bool trackChanges = true)
        {
            this.Name = name;
            this.Type = type;

            // value types should be instantiated with their
            // respective underlying default value.
            if (this.Type.IsValueType)
            {
                this.defaultValue = Activator.CreateInstance(this.Type);
            }
            else
            {
                this.defaultValue = null;
            }

            this.TrackChanges = trackChanges;
        }

        /// <summary>
        /// Property name.
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// Type.
        /// </summary>
        public Type Type
        {
            get;
        }

        /// <summary>
        /// Indicate if changes should be tracked for this property.
        /// </summary>
        internal bool TrackChanges { get; set; }

        /// <summary>
        /// Default value.
        /// </summary>
        public object DefaultValue
        {
            get
            {
                if (this.IsEnumerable)
                {
                    return this.ActivateObservableList(
                        this.IEnumerableUnderlyingType,
                        null);
                }

                return this.defaultValue;
            }
        }

        /// <summary>
        /// Indicate if definition implements IEnumerable.
        /// </summary>
        public bool IsEnumerable
        {
            get
            {
                return this.IsIEnumerable();
            }
        }

        /// <summary>
        /// Returns IEnumerable underlying type if definition is IEnumerable,
        /// otherwise null.
        /// </summary>
        public Type IEnumerableUnderlyingType
        {
            get
            {
                if (!this.IsEnumerable)
                {
                    return null;
                }

                return this.Type.GetGenericArguments()[0];
            }
        }

        /// <summary>
        /// Return underlying type stored in navigation property.
        /// </summary>
        public Type NavigationPropertyUnderlyingType
        {
            get
            {
                if (!this.IsNavigationProperty)
                {
                    return null;
                }

                return this.Type.GetGenericArguments()[0];
            }
        }

        /// <summary>
        /// Indicate property is navigation.
        /// </summary>
        public bool IsNavigationProperty
        {
            get
            {
                if (this.Type.IsGenericType)
                {
                    return this.Type.GetGenericTypeDefinition() == typeof(NavigationProperty<>);
                }

                return false;
            }
        }

        /// <summary>
        /// Indicate if property definition has change tracking implemented.
        /// </summary>
        public bool ChangeTrackable
        {
            get
            {
                if (this.IsEnumerable)
                {
                    return this.ChangeTrackingImplemented(this.IEnumerableUnderlyingType);
                }

                return this.ChangeTrackingImplemented(this.Type);
            }
        }

        /// <summary>
        /// Hash code.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^
                   this.Type.GetHashCode();
        }

        /// <summary>
        /// Equals impl.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is PropertyDefinition propertyDefinition)
            {
                return this.Name == propertyDefinition.Name &&
                       this.Type == propertyDefinition.Type;
            }

            return false;
        }

        /// <summary>
        /// ToString impl.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Validate if it can store particular type.
        /// </summary>
        /// <param name="type">Type to validate.</param>
        /// <returns></returns>
        internal bool CanStoreType(Type type)
        {
            if (type.IsSubclassOf(this.Type))
            {
                return true;
            }

            if (type == this.Type)
            {
                return true;
            }

            if (this.Type.IsInterface &&
                this.ImplementsInterface(type, this.Type))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Activate navigation property.
        /// </summary>
        /// <param name="propertyBag"></param>
        /// <returns></returns>
        internal object ActivateNavigationProperty(PropertyBag propertyBag)
        {
            if (!this.IsNavigationProperty)
            {
                throw new ArgumentException($"Type is not navigation: '{this.Type.FullName}'.");
            }

            return Activator.CreateInstance(
                this.Type,
                new object[] { propertyBag, this.Name });

        }

        /// <summary>
        /// Activate observable list.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal object ActivateObservableList(object value)
        {
            return this.ActivateObservableList(
                this.IEnumerableUnderlyingType,
                value);
        }

        /// <summary>
        /// Activate list.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal IList<object> ActivateList(object value)
        {
            return (IList<object>) this.ActivateObservableList(
                typeof(object),
                value);
        }

        /// <summary>
        /// Indicate if current property definition type implements IEnumerable.
        /// </summary>
        /// <returns></returns>
        private bool IsIEnumerable()
        {
            if (this.Type.IsInterface &&
                this.Type.IsGenericType)
            {
                return this.Type.GetInterface(PropertyDefinition.EnumerableTypeInterfaceName) != null;
            }

            return false;
        }

        /// <summary>
        /// Activate observable list.
        /// </summary>
        /// <param name="type">Observable list type.</param>
        /// <param name="value">Observable list value. If null, create empty list.</param>
        /// <returns></returns>
        private object ActivateObservableList(Type type, object value)
        {
            if (!this.IsEnumerable)
            {
                throw new InvalidOperationException("Cannot activate ObservableList on non-IEnumerable type.");
            }

            Type observableCollectionType = typeof(ObservableCollection<>);
            Type constructedObservableCollection = observableCollectionType.MakeGenericType(type);
            if (null == value)
            {
                return Activator.CreateInstance(constructedObservableCollection);
            }
            else
            {
                return Activator.CreateInstance(
                    constructedObservableCollection,
                    value);
            }
        }

        /// <summary>
        /// Validate if change tracking is implemented for a type.
        /// </summary>
        /// <param name="type">Type to validate.</param>
        /// <returns></returns>
        private bool ChangeTrackingImplemented(Type type)
        {
            return this.ImplementsInterface(
                type,
                typeof(IPropertyChangeTracking));
        }

        /// <summary>
        /// Test if particular type implements interface.
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="interfaceType">Interface.</param>
        /// <returns></returns>
        private bool ImplementsInterface(Type type, Type interfaceType)
        {
            foreach (Type implInterface in type.GetInterfaces())
            {
                if (implInterface == interfaceType)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
