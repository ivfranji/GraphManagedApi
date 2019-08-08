namespace Microsoft.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    using Microsoft.Graph.ChangeTracking;
    using Newtonsoft.Json;

    /// <summary>
    /// Extended entity.
    /// </summary>
    public partial class Entity : IPropertyChangeTracking
    {
        /// <summary>
        /// Property bag.
        /// </summary>
        protected PropertyBag propertyBag;

        /// <summary>
        /// <see cref="IsNew"/> property definition.
        /// </summary>
        private readonly PropertyDefinition isNewPropDef = new PropertyDefinition(
            nameof(Entity.IsNew),
            typeof(bool),
            false);

        /// <summary>
        /// <see cref="EntityPath"/> property definition.
        /// </summary>
        private readonly PropertyDefinition entityPathPropDef = new PropertyDefinition(
            nameof(Entity.EntityPath),
            typeof(EntityPath),
            false);

        /// <summary>
        /// <see cref="ActionInvoker"/> property definition.
        /// </summary>
        private readonly PropertyDefinition actionInvokerPropDef = new PropertyDefinition(
            nameof(Entity.ActionInvoker),
            typeof(IActionInvoker),
            false);

        /// <summary>
        /// <see cref="EntityService"/> property definition.
        /// </summary>
        private readonly PropertyDefinition entityServicePropDef = new PropertyDefinition(
            nameof(Entity.EntityService),
            typeof(IEntityService),
            false);

        /// <summary>
        /// Entity self property definition.
        /// </summary>
        private PropertyDefinition entitySelf;

        /// <summary>
        /// Entity type.
        /// </summary>
        private Type entityType;

        /// <summary>
        /// Object schema.
        /// </summary>
        private ObjectSchema objectSchema;

        /// <summary>
        /// Create entity and initialize property bag.
        /// </summary>
        protected Entity()
        {
            this.entityType = this.GetType();
            Type schemaType = Assembly.GetExecutingAssembly().GetType(
                this.entityType.FullName + "ObjectSchema");
            if (schemaType != null)
            {
                object instance = Activator.CreateInstance(schemaType);
                if (instance is ObjectSchema)
                {
                    this.objectSchema = instance as ObjectSchema;
                    this.propertyBag = new PropertyBag(this.objectSchema);
                }
            }
            else
            {
                throw new NullReferenceException(
                    $"Cannot find schema definition '{this.GetType().FullName}ObjectSchema'.");
            }

            this.entitySelf = new PropertyDefinition(
                nameof(Entity),
                this.entityType,
                false);

            this.RegisterNonTrackingProperties();
            this.IsNew = true;
            this.EntityPath = new EntityPath(this.entityType);
        }

        /// <summary>
        /// Create new instance of <see cref="Entity"/>
        /// </summary>
        /// <param name="entityService">Entity service.</param>
        protected Entity(IEntityService entityService)
            : this()
        {
            this.EntityService = entityService;
            this.ActionInvoker = entityService;
        }

        /// <summary>
        /// Indicate if object is new - not downloaded from server.
        /// </summary>
        public bool IsNew
        {
            get { return (bool)this.propertyBag[isNewPropDef]; }
            set { this.propertyBag[isNewPropDef] = value; }
        }

        /// <summary>
        /// Returns container in case if new or full id path in case
        /// not new.
        /// </summary>
        public EntityPath EntityPath
        {
            get { return (EntityPath)this.propertyBag[entityPathPropDef]; }
            private set { this.propertyBag[entityPathPropDef] = value; }
        }

        /// <summary>
        /// Gets or sets additional data.
        /// </summary>
        [JsonExtensionData(ReadData = true, WriteData = true)]
        public IDictionary<string, object> NonDeclaredProperties { get; set; }

        /// <summary>
        /// Method invoker.
        /// </summary>
        internal IActionInvoker ActionInvoker
        {
            get { return (IActionInvoker)this.propertyBag[actionInvokerPropDef]; }
            set { this.propertyBag[actionInvokerPropDef] = value; }
        }

        /// <summary>
        /// Entity service.
        /// </summary>
        internal IEntityService EntityService
        {
            get { return (IEntityService)this.propertyBag[entityServicePropDef]; }
            set { this.propertyBag[entityServicePropDef] = value; }
        }

        /// <summary>
        /// Delete entity from the server.
        /// </summary>
        public async Task DeleteAsync()
        {
            this.ThrowIfNewValidationFails();
            this.ThrowOnNullEntityService();

            await this.EntityService.DeleteAsync(this);
            this.propertyBag.Clear();
        }

        /// <summary>
        /// Update entity.
        /// </summary>
        public async Task UpdateAsync()
        {
            this.ThrowIfNewValidationFails();
            this.ThrowOnNullEntityService();

            Entity entity = await this.EntityService.UpdateAsync(this);
            IEntityService entityService = this.EntityService;

            this.propertyBag = entity.propertyBag;
            this.RegisterNonTrackingProperties();
            this.ActionInvoker = entityService;
            this.EntityService = entityService;
        }

        /// <summary>
        /// Create new entity.
        /// </summary>
        /// <param name="destination">Entity destination.</param>
        public async Task SaveAsync(Entity destination)
        {
            IEntityService entityService = this.GetPreSaveEntity();
            Entity entity = await this.EntityService.CreateAsync(this, destination);
            this.ConfigurePostSaveEntity(
                entityService, 
                entity.propertyBag);
        }

        /// <summary>
        /// Save entity in async fashion.
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            EntityPath path = new EntityPath(this.entityType);
            IEntityService entityService = this.GetPreSaveEntity();
            Entity entity = await this.EntityService.CreateAsync(this, path);
            this.ConfigurePostSaveEntity(
                entityService, 
                entity.propertyBag);
        }

        /// <summary>
        /// ToString impl.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.Id))
            {
                return this.entityType.Name;
            }

            return $"{this.entityType.Name} | {this.Id}";
        }

        /// <summary>
        /// Called when deserialization finished.
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.propertyBag.ResetChangeFlag();
            this.IsNew = false;
            this.EntityPath = new EntityPath(this);
        }

        /// <summary>
        /// Get pre save entity service.
        /// </summary>
        /// <returns></returns>
        private IEntityService GetPreSaveEntity()
        {
            this.ThrowIfNewValidationFails(true);
            this.ThrowOnNullEntityService();
            return this.EntityService;
        }

        /// <summary>
        /// Configure post save entity.
        /// </summary>
        /// <param name="entityService">Entity service.</param>
        /// <param name="propertyBag">Property bag.</param>
        private void ConfigurePostSaveEntity(IEntityService entityService, PropertyBag propertyBag)
        {
            this.propertyBag = propertyBag;
            this.RegisterNonTrackingProperties();
            this.IsNew = false;
            this.ActionInvoker = entityService;
            this.EntityService = entityService;
        }

        /// <summary>
        /// Throws if <see cref="IsNew"/> flag doesn't match.
        /// </summary>
        /// <param name="allowNew">Should allow new.</param>
        private void ThrowIfNewValidationFails(bool allowNew = false)
        {
            if (this.IsNew != allowNew)
            {
                throw new InvalidOperationException("Cannot delete entity that yet isn't created on the server.");
            }
        }

        /// <summary>
        /// Throws in <see cref="EntityService"/> is null.
        /// </summary>
        private void ThrowOnNullEntityService()
        {
            if (null == this.EntityService)
            {
                throw new ArgumentNullException(nameof(this.EntityService));
            }
        }

        /// <summary>
        /// Registers non-tracking properties. These are per instance.
        /// </summary>
        private void RegisterNonTrackingProperties()
        {
            this.propertyBag.AddNonTrackedProperty(isNewPropDef);
            this.propertyBag.AddNonTrackedProperty(entityPathPropDef);
            this.propertyBag.AddNonTrackedProperty(actionInvokerPropDef);
            this.propertyBag.AddNonTrackedProperty(entityServicePropDef);
            this.propertyBag.AddNonTrackedProperty(this.entitySelf);
            this.propertyBag[this.entitySelf] = this;
        }

        /// <summary>
        /// Get changed properties.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PropertyDefinition> GetChangedProperties()
        {
            return this.propertyBag.GetChangedProperties();
        }

        /// <summary>
        /// Get property.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[PropertyDefinition key]
        {
            get { return this.propertyBag[key]; }
        }

        /// <summary>
        /// Get property.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                {
                    throw new ArgumentNullException(nameof(key));
                }

                PropertyDefinition propertyDefinition = this.objectSchema[key];
                return this.propertyBag[propertyDefinition];
            }
        }
    }
}
