namespace Microsoft.Graph.CoreJson
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Child type converter.
    /// </summary>
    internal class ChildTypeConverter : JsonConverter
    {
        /// <summary>
        /// Caches odata types to strong types.
        /// </summary>
        private static readonly ConcurrentDictionary<string, Type> ODataTypeMapping = new ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Odata type key.
        /// </summary>
        private const string ODataTypeKey = "@odata.type";

        /// <summary>
        /// Create default instance of <see cref="ChildTypeConverter"/>
        /// </summary>
        public ChildTypeConverter()
        {
        }
        
        /// <summary>
        /// Not implemented. This converter doesn't support writing.
        /// </summary>
        /// <param name="writer">Writer.</param>
        /// <param name="value">Value.</param>
        /// <param name="serializer">Serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs child type conversion.
        /// </summary>
        /// <param name="reader">Reader.</param>
        /// <param name="objectType">Object type.</param>
        /// <param name="existingValue">Existing value.</param>
        /// <param name="serializer">Serializer.</param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            object objectInstance = null;
            JToken odataType = jsonObject.GetValue(ChildTypeConverter.ODataTypeKey);
            if (null == odataType)
            {
                objectInstance = this.InstantiateType(
                    objectType.AssemblyQualifiedName,
                    null);
            }
            else
            {
                string typeName = this.GetTypeName(odataType);
                objectInstance = this.GetObjectInstance(typeName, objectType);
            }

            if (null == objectInstance)
            {
                throw new InvalidOperationException("Cannot deserialize type");
            }

            using (JsonReader jsonReader = this.GetReader(reader, jsonObject))
            {
                serializer.Populate(jsonReader, objectInstance);
                return objectInstance;
            }
        }

        /// <summary>
        /// Returns true for all types. Only abstract classes should be
        /// decorated with this attribute. It is not supposed to be used
        /// as deserializer default value for converter.
        /// </summary>
        /// <param name="objectType">Object type.</param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        /// <summary>
        /// Cannot write.
        /// </summary>
        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// Get value from cache.
        /// </summary>
        /// <param name="typeName">Type name.</param>
        /// <param name="type">Type.</param>
        /// <returns></returns>
        internal bool GetFromCache(string typeName, out Type type)
        {
            return ChildTypeConverter.ODataTypeMapping.TryGetValue(typeName, out type);
        }

        /// <summary>
        /// Instantiate type.
        /// </summary>
        /// <param name="typeName">Type name.</param>
        /// <param name="assembly">Assembly.</param>
        /// <returns></returns>
        private object InstantiateType(string typeName, Assembly assembly)
        {
            Type type = null;
            if (null != assembly)
            {
                type = assembly.GetType(typeName);
            }
            else
            {
                type = Type.GetType(typeName);
            }

            return this.InstantiateType(type);
        }

        /// <summary>
        /// Instantiate type. 
        /// </summary>
        /// <param name="type">Type.</param>
        /// <returns></returns>
        private object InstantiateType(Type type)
        {
            if (null == type)
            {
                return null;
            }

            try
            {
                ConstructorInfo constructorInfo = type.GetTypeInfo().DeclaredConstructors
                    .FirstOrDefault(constructor => !constructor.GetParameters().Any() && !constructor.IsStatic);
                if (null == constructorInfo)
                {
                    return null;
                }

                return constructorInfo.Invoke(new object[] { });
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        /// <summary>
        /// Get type name from odata.property.
        /// </summary>
        /// <param name="jtoken"></param>
        /// <returns></returns>
        private string GetTypeName(JToken jtoken)
        {
            string odataTypeString = jtoken.ToString();
            odataTypeString = odataTypeString.TrimStart('#');

            string[] typeSegments = odataTypeString.Split('.');
            for (int i = 0; i < typeSegments.Length; i++)
            {
                typeSegments[i] = typeSegments[i].Substring(0, 1).ToUpperInvariant() + typeSegments[i].Substring(1);
            }

            return string.Join(
                ".",
                typeSegments);
        }

        /// <summary>
        /// This is fallback method so it will only search for Microsoft.Graph.* types
        /// </summary>
        /// <param name="contextString"></param>
        /// <returns></returns>
        private string GetTypeNameFromContextString(string contextString)
        {
            contextString = contextString.Substring(0, 1).ToUpperInvariant() + contextString.Substring(1);
            return "Microsoft.Graph." + contextString;
        }

        /// <summary>
        /// Create reader ready for populate object.
        /// </summary>
        /// <param name="reader">Reader.</param>
        /// <param name="jsonObject">Json object.</param>
        /// <returns></returns>
        private JsonReader GetReader(JsonReader reader, JObject jsonObject)
        {
            JsonReader jsonReader = jsonObject.CreateReader();
            jsonReader.Culture = reader.Culture;
            jsonReader.DateParseHandling = reader.DateParseHandling;
            jsonReader.DateTimeZoneHandling = reader.DateTimeZoneHandling;
            jsonReader.FloatParseHandling = reader.FloatParseHandling;
            jsonReader.CloseInput = false;

            return jsonReader;
        }

        /// <summary>
        /// Get object instance.
        /// </summary>
        /// <param name="typeName">Type name.</param>
        /// <param name="objectType">Object type.</param>
        /// <returns></returns>
        private object GetObjectInstance(string typeName, Type objectType)
        {
            object objectInstance = null;
            if (this.GetFromCache(typeName, out Type typeInstance))
            {
                objectInstance = this.InstantiateType(typeInstance);
            }
            else
            {
                objectInstance = this.InstantiateType(
                    typeName,
                    objectType.Assembly);
            }

            if (null == objectInstance)
            {
                objectInstance = this.InstantiateType(
                    objectType.AssemblyQualifiedName,
                    null);
            }

            if (null != objectInstance && null == typeInstance)
            {
                ChildTypeConverter.ODataTypeMapping.TryAdd(
                    typeName,
                    objectInstance.GetType());
            }

            return objectInstance;
        }
    }
}
