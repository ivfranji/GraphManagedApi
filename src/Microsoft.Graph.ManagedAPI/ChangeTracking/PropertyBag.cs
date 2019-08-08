namespace Microsoft.Graph.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    /// <summary>
    /// Property bag.
    /// </summary>
    public class PropertyBag
    {
        /// <summary>
        /// Object schema.
        /// </summary>
        private readonly ObjectSchema objectSchema;

        /// <summary>
        /// Properties.
        /// </summary>
        private Dictionary<PropertyDefinition, ObjectChangeTracking> properties;

        /// <summary>
        /// Create new instance of <see cref="PropertyBag"/>
        /// </summary>
        /// <param name="objectSchema">Object schema.</param>
        internal PropertyBag(ObjectSchema objectSchema)
        {
            this.objectSchema = objectSchema ?? throw new ArgumentNullException(nameof(objectSchema));
            this.InitializeBag();
        }

        /// <summary>
        /// Object indexer.
        /// </summary>
        /// <param name="key">Property definition.</param>
        /// <returns></returns>
        public object this[PropertyDefinition key]
        {
            get
            {
                if (null == key)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (this.properties.ContainsKey(key))
                {
                    return this.properties[key].Value;
                }

                throw new KeyNotFoundException(key.Name);
            }
            set
            {
                if (this.properties.ContainsKey(key))
                {
                    if (value != null)
                    {
                        Type setValueType = value.GetType();
                        if (key.IsEnumerable)
                        {
                            this.InitializeCollectionProperty(
                                key,
                                value);

                            this.properties[key].Changed = true;
                        }
                        else if (!key.CanStoreType(setValueType))
                        {
                            throw new InvalidOperationException($"Attempted to store wrong type to the dictionary. Expected type: '{key.Type.FullName}'. Actual type: '{setValueType.FullName}'");
                        }
                        else
                        {
                            // value can be stored only if type matches or incoming type
                            // is subclass of this property.
                            this.properties[key].Value = value;
                        }
                    }
                    else
                    {
                        this.properties[key].Value = key.DefaultValue;
                    }
                }

                else
                {
                    throw new KeyNotFoundException(key.Name);
                }
            }
        }

        /// <summary>
        /// Clear property bag.
        /// </summary>
        public void Clear()
        {
            this.InitializeBag();
        }

        /// <summary>
        /// Retrieve list of changed properties.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PropertyDefinition> GetChangedProperties()
        {
            foreach (KeyValuePair<PropertyDefinition, ObjectChangeTracking> changeTracking in this.properties)
            {
                if (changeTracking.Key.TrackChanges &&
                    changeTracking.Value.Changed)
                {
                    yield return changeTracking.Key;
                }
            }
        }

        /// <summary>
        /// Reset change tracking, all properties set as no changes has been made.
        /// </summary>
        internal void ResetChangeFlag()
        {
            foreach (KeyValuePair<PropertyDefinition, ObjectChangeTracking> changeTracking in properties)
            {
                changeTracking.Value.Changed = false;
            }
        }

        /// <summary>
        /// Adds non-tracked property into collection.
        /// </summary>
        /// <param name="propertyDefinition">Property definition.</param>
        internal void AddNonTrackedProperty(PropertyDefinition propertyDefinition)
        {
            if (null == propertyDefinition)
            {
                throw new ArgumentNullException(nameof(propertyDefinition));
            }

            if (propertyDefinition.TrackChanges)
            {
                throw new ArgumentException("Cannot add trackable property.");
            }

            if (!this.properties.ContainsKey(propertyDefinition))
            {
                this.properties.Add(
                    propertyDefinition,
                    new ObjectChangeTracking(propertyDefinition.DefaultValue));
            }
        }

        /// <summary>
        /// Try get key.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        /// <param name="propertyDefinition">Property definition.</param>
        /// <returns></returns>
        internal bool TryGetKey(string propertyName, out PropertyDefinition propertyDefinition)
        {
            propertyDefinition = null;
            foreach (KeyValuePair<PropertyDefinition, ObjectChangeTracking> property in this.properties)
            {
                if (property.Key.Name == propertyName)
                {
                    propertyDefinition = property.Key;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Initialize property bag.
        /// </summary>
        private void InitializeBag()
        {
            this.properties = new Dictionary<PropertyDefinition, ObjectChangeTracking>();
            foreach (string key in this.objectSchema.Keys)
            {
                PropertyDefinition propertyDefinition = objectSchema[key];
                if (propertyDefinition.IsEnumerable)
                {
                    this.InitializeCollectionProperty(
                        propertyDefinition,
                        null);
                }
                else
                {
                    this.properties[propertyDefinition] = new ObjectChangeTracking(propertyDefinition.DefaultValue);
                }
                
                if (propertyDefinition.IsNavigationProperty)
                {
                    propertyDefinition.TrackChanges = false;
                    this.properties[propertyDefinition].Value = propertyDefinition.ActivateNavigationProperty(this);
                }
            }
        }

        /// <summary>
        /// Initialize collection property and value.
        /// </summary>
        /// <param name="def">Collection property definition.</param>
        /// <param name="value">Value, if null, create empty collection.</param>
        private void InitializeCollectionProperty(PropertyDefinition def, object value)
        {
            this.properties[def] = new ObjectChangeTracking(
                def.ActivateObservableList(value));
            INotifyCollectionChanged notifyCollectionChanged = (INotifyCollectionChanged)this.properties[def].Value;
            this.properties[def].RegisterListChangeListener(notifyCollectionChanged);
        }

        /// <summary>
        /// Track object changes.
        /// </summary>
        private class ObjectChangeTracking
        {
            /// <summary>
            /// Object value.
            /// </summary>
            private object value;

            /// <summary>
            /// Observable collection.
            /// </summary>
            private INotifyCollectionChanged CollectionChanged { get; set; }

            /// <summary>
            /// Create new instance of <see cref="ObjectChangeTracking"/>
            /// </summary>
            /// <param name="objectValue">Object value.</param>
            public ObjectChangeTracking(object objectValue)
            {
                this.value = objectValue;
                this.Changed = false;
            }

            /// <summary>
            /// Object value.
            /// </summary>
            public object Value
            {
                get { return this.value; }
                set
                {
                    this.value = value;
                    this.Changed = true;
                }
            }

            /// <summary>
            /// Indicate if object changed.
            /// </summary>
            public bool Changed { get; set; }

            /// <summary>
            /// Register collection changed.
            /// </summary>
            /// <param name="collectionChanged"></param>
            public void RegisterListChangeListener(INotifyCollectionChanged collectionChanged)
            {
                this.CollectionChanged = collectionChanged;
                this.CollectionChanged.CollectionChanged += (sender, args) => { this.Changed = true; };
            }
        }
    }
}
