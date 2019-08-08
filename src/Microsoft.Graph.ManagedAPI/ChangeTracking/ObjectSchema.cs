namespace Microsoft.Graph.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Object schema definition.
    /// </summary>
    public abstract class ObjectSchema
    {
        /// <summary>
        /// Properties.
        /// </summary>
        private Dictionary<string, PropertyDefinition> properties;

        /// <summary>
        /// Create new instance of <see cref="ObjectSchema"/>
        /// </summary>
        protected ObjectSchema()
        {
            this.properties = new Dictionary<string, PropertyDefinition>();
            Type schemaType = this.GetType();
            foreach (FieldInfo fieldInfo in schemaType.GetFields(
                BindingFlags.Static |
                BindingFlags.FlattenHierarchy |
                BindingFlags.Public))
            {
                PropertyDefinition propertyDefinition = fieldInfo.GetValue(null) as PropertyDefinition;
                if (null != propertyDefinition &&
                    !this.properties.ContainsKey(fieldInfo.Name))
                {
                    this.properties.Add(fieldInfo.Name, propertyDefinition);
                }
            }
        }

        /// <summary>
        /// Get key from schema.
        /// </summary>
        /// <param name="key">Key name.</param>
        /// <returns></returns>
        public PropertyDefinition this[string key]
        {
            get
            {
                if (this.properties.ContainsKey(key))
                {
                    return this.properties[key];
                }

                throw new KeyNotFoundException(key);
            }
        }

        /// <summary>
        /// Values in bag.
        /// </summary>
        internal IEnumerable<PropertyDefinition> Values
        {
            get
            {
                return this.properties.Values;
            }
        }

        /// <summary>
        /// Keys in the bag.
        /// </summary>
        internal IEnumerable<string> Keys
        {
            get
            {
                return this.properties.Keys;
            }
        }

        /// <summary>
        /// Test if schema contains key.
        /// </summary>
        /// <param name="name">Key name.</param>
        /// <returns></returns>
        internal bool ContainsKey(string name)
        {
            return this.properties.ContainsKey(name);
        }
    }
}
