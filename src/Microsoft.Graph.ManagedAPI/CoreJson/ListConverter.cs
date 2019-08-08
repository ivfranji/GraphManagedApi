namespace Microsoft.Graph.CoreJson
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// List converter. It is used when there is IList deserialization with
    /// 'value' property. For example:
    /// {
    ///   "someProp": "2",
    ///   "value": [
    ///   ]
    /// }
    ///
    /// It will strip everything besides value and will instantiate and populate
    /// list with entries from property 'value'.
    /// </summary>
    internal class ListConverter : JsonConverter
    {
        /// <summary>
        /// Not used.
        /// </summary>
        /// <param name="writer">Writer.</param>
        /// <param name="value">Value.</param>
        /// <param name="serializer">Serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Read json and converts it.
        /// </summary>
        /// <param name="reader">Reader.</param>
        /// <param name="objectType">Object type.</param>
        /// <param name="existingValue">Existing value.</param>
        /// <param name="serializer">Serializer.</param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray jArray = null;
            if (reader.TokenType == JsonToken.StartArray)
            {
                jArray = JArray.Load(reader);

            }
            else
            {
                JObject jsonObject = JObject.Load(reader);
                JToken jToken = jsonObject.GetValue(
                    "value",
                    StringComparison.OrdinalIgnoreCase);

                if (null == jToken)
                {
                    return serializer.Deserialize(reader, objectType);
                }

                jArray = jToken as JArray;
            }

            if (null == jArray)
            {
                return serializer.Deserialize(reader, objectType);
            }

            Type type = objectType.GetGenericArguments()[0];
            Type listType = typeof(List<>);
            Type genericListType = listType.MakeGenericType(type);
            object instance = Activator.CreateInstance(genericListType);
            serializer.Populate(
                jArray.CreateReader(), 
                instance);

            return instance;
        }

        /// <summary>
        /// Convert only IList.
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            if (!objectType.Name.StartsWith(
                "IList`1",
                StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return objectType.GetGenericArguments()[0].FullName.StartsWith("Microsoft.Graph.");
        }
    }
}
