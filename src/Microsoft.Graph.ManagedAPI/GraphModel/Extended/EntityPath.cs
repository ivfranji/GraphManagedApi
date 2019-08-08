namespace Microsoft.Graph
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represent path to the specific entity.
    /// </summary>
    public class EntityPath
    {
        /// <summary>
        /// Create type path mapping.
        /// </summary>
        private static readonly Lazy<Dictionary<Type, string>> TypePathMapping = new Lazy<Dictionary<Type, string>>(EntityPath.InitializeTypeMapping);

        /// <summary>
        /// Create new instance of <see cref="EntityPath"/>
        /// </summary>
        /// <param name="entity">Entity.</param>
        internal EntityPath(Entity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (string.IsNullOrEmpty(entity.Id))
            {
                throw new ArgumentNullException("entity.Id");
            }

            this.Id = entity.Id;
            this.RootContainer = EntityPath.TypePathMapping.Value[entity.GetType()];
        }

        /// <summary>
        /// Create new instance of <see cref="EntityPath"/>
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <param name="entityType">Type of entity.</param>
        internal EntityPath(string id, Type entityType)
            : this(entityType)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            this.Id = id;
        }

        /// <summary>
        /// Create new instance of <see cref="EntityPath"/>
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        internal EntityPath(Type entityType)
        {
            if (null == entityType)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            if (!entityType.IsSubclassOf(typeof(Entity)))
            {
                throw new ArgumentException("Type must derive from 'Microsoft.Graph.Entity'.");
            }

            this.Id = string.Empty;
            this.RootContainer = EntityPath.TypePathMapping.Value[entityType];
        }

        /// <summary>
        /// Sub entity. For an instance, if this is /users/user@domain.com, subEntity can be /MailFolders/Inbox, and sub entity of that can be /Messages
        /// </summary>
        public EntityPath SubEntity { get; set; }

        /// <summary>
        /// Entity id.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Returns root container for a 
        /// </summary>
        public string RootContainer { get; }

        /// <summary>
        /// Indicate if path is root container.
        /// </summary>
        public bool IsRootContainer
        {
            get { return string.IsNullOrEmpty(this.Id); }
        }

        /// <summary>
        /// Path of the entity. 
        /// </summary>
        public string Path
        {
            get
            {
                if (null != this.SubEntity)
                {
                    return $"{this.GetPath()}/{this.SubEntity.Path}";
                }

                return this.GetPath();
            }
        }

        /// <summary>
        /// ToString impl.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Path;
        }

        /// <summary>
        /// Equals impl.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is EntityPath entityPath)
            {
                return this.Equals(entityPath);
            }

            return false;
        }

        /// <summary>
        /// Check for equality.
        /// </summary>
        /// <param name="other">Other object.</param>
        /// <returns></returns>
        protected bool Equals(EntityPath other)
        {
            return string.Equals(this.Id, other.Id) &&
                   string.Equals(this.RootContainer, other.RootContainer);
        }

        /// <summary>
        /// Get hash code override. 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.Id != null ? this.Id.GetHashCode() : 0) * 397) ^
                       (this.RootContainer != null ? this.RootContainer.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Type mapping initializer.
        /// </summary>
        /// <returns></returns>
        private static Dictionary<Type, string> InitializeTypeMapping()
        {
            Dictionary<Type, string> typeMapping = new Dictionary<Type, string>();
            Type entityType = typeof(Entity);
            foreach (Type type in entityType.Assembly.GetTypes())
            {
                if (type.IsSubclassOf(entityType) &&
                    !type.IsAbstract)
                {
                    string name = $"{type.Name}s"; // adding 's' at the end to signify path like /MailFolders, /Messages...

                    // for some exchange entities graph is generating
                    // different paths and those paths are addressed here.
                    if (type == typeof(EventMessage))
                    {
                        name = "Messages";
                    }

                    else if (type == typeof(OutlookTask))
                    {
                        name = "Outlook/Tasks";
                    }

                    else if (type == typeof(MessageRule))
                    {
                        name = "MailFolders/Inbox/MessageRules";
                    }

                    else if (type == typeof(OutlookCategory))
                    {
                        name = "Outlook/MasterCategories";
                    }

                    typeMapping.Add(type, name);
                }
            }

            return typeMapping;
        }

        /// <summary>
        /// Returns path for this entity.
        /// </summary>
        /// <returns></returns>
        private string GetPath()
        {
            if (this.IsRootContainer)
            {
                return this.RootContainer;
            }

            return $"{this.RootContainer}/{this.Id}";
        }
    }
}
