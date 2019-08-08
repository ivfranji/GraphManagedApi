namespace Microsoft.Graph.Exchange
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Graph.ChangeTracking;
    using Microsoft.Graph.Utilities;

    /// <summary>
    /// Property set for an object.
    /// </summary>
    public abstract class PropertySet : IUrlQuery
    {
        /// <summary>
        /// First class properties.
        /// </summary>
        protected List<PropertyDefinition> firstClassProperties;

        /// <summary>
        /// Requested properties - first class will be included automatically.
        /// </summary>
        private List<PropertyDefinition> requestedProperties;

        /// <summary>
        /// Object schema for current view.
        /// </summary>
        private ObjectSchema objectSchema;

        /// <summary>
        /// Expandable property set.
        /// </summary>
        private ExpandablePropertySet expandablePropertySet;

        /// <summary>
        /// Create new instance of <see cref="PropertySet"/>
        /// </summary>
        /// <param name="objectSchema">Object schema.</param>
        protected PropertySet(ObjectSchema objectSchema)
        {
            objectSchema.ThrowIfNull(nameof(objectSchema));
            this.objectSchema = objectSchema;
            this.firstClassProperties = new List<PropertyDefinition>();
            this.firstClassProperties.Add(EntityObjectSchema.Id);
            this.requestedProperties = new List<PropertyDefinition>();
        }

        /// <summary>
        /// Url query empty.
        /// </summary>
        internal bool UrlQueryEmpty
        {
            get
            {
                return this.requestedProperties.Count == 0 &&
                       this.expandablePropertySet == null;
            }
        }

        /// <summary>
        /// First class properties.
        /// </summary>
        public IReadOnlyCollection<PropertyDefinition> FirstClassProperties
        {
            get { return this.firstClassProperties.AsReadOnly(); }
        }

        /// <summary>
        /// Add property to the set.
        /// </summary>
        /// <param name="propertyDefinition">Property definition.</param>
        public void Add(PropertyDefinition propertyDefinition)
        {
            this.ValidatePropertyDefinition(propertyDefinition);
            if (!this.requestedProperties.Contains(propertyDefinition))
            {
                this.requestedProperties.Add(propertyDefinition);
            }
        }

        /// <summary>
        /// Expand property.
        /// </summary>
        /// <param name="propertyDefinition"></param>
        public void Expand(PropertyDefinition propertyDefinition)
        {
            this.ValidatePropertyDefinition(propertyDefinition);
            if (null == this.expandablePropertySet)
            {
                this.expandablePropertySet = new ExpandablePropertySet();
            }

            this.expandablePropertySet.Add(propertyDefinition);
        }

        /// <summary>
        /// Expand property.
        /// </summary>
        /// <param name="propertyDefinition"></param>
        public void Expand(ExtendedPropertyDefinition extendedPropertyDefinition)
        {
            extendedPropertyDefinition.ThrowIfNull(nameof(extendedPropertyDefinition));
            if (null == this.expandablePropertySet)
            {
                this.expandablePropertySet = new ExpandablePropertySet();
            }

            this.expandablePropertySet.Add(extendedPropertyDefinition);
        }

        /// <summary>
        /// Extended property definition.
        /// </summary>
        /// <param name="extendedPropertyDefinition"></param>
        public void Add(ExtendedPropertyDefinition extendedPropertyDefinition)
        {
            this.Expand(extendedPropertyDefinition);
        }

        /// <summary>
        /// Get url query.
        /// </summary>
        /// <returns></returns>
        public string GetUrlQuery()
        {
            if (this.UrlQueryEmpty)
            {
                return null;
            }

            IUrlQuery urlQuery = null;
            if (null != this.expandablePropertySet)
            {
                urlQuery = this.expandablePropertySet;
            }

            if (this.requestedProperties.Count == 0)
            {
                return urlQuery.GetUrlQuery();
            }

            PropertyDefinition[] properties = new PropertyDefinition[this.requestedProperties.Count + this.firstClassProperties.Count];
            int counter = 0;

            // always add first class properties if at least one property added.
            for (int i = 0; i < this.firstClassProperties.Count; i++)
            {
                properties[counter++] = this.firstClassProperties[i];
            }

            for (int i = 0; i < this.requestedProperties.Count; i++)
            {
                properties[counter++] = this.requestedProperties[i];
            }

            SelectQuery selectQuery = new SelectQuery(properties);
            if (null != urlQuery)
            {
                CompositeQuery compositeQuery = new CompositeQuery(new IUrlQuery[] {selectQuery, urlQuery});
                return compositeQuery.GetUrlQuery();
            }

            return selectQuery.GetUrlQuery();
        }

        /// <summary>
        /// Validate property definition.
        /// </summary>
        /// <param name="propertyDefinition"></param>
        private void ValidatePropertyDefinition(PropertyDefinition propertyDefinition)
        {
            propertyDefinition.ThrowIfNull(nameof(propertyDefinition));
            if (!this.objectSchema.ContainsKey(propertyDefinition.Name))
            {
                throw new ArgumentException($"Schema '{this.objectSchema.GetType().Namespace}' doesn't contain definition for property '{propertyDefinition.Name}'.");
            }
        }
    }
}
