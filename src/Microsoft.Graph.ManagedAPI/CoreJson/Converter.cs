namespace Microsoft.Graph.CoreJson
{
    using System.Collections.Generic;
    using Microsoft.Graph.ChangeTracking;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Json converter.
    /// </summary>
    internal class Converter
    {
        /// <summary>
        /// Converter settings.
        /// </summary>
        private readonly JsonSerializerSettings serializerSettings;

        /// <summary>
        /// Create new instance of <see cref="Converter"/>
        /// </summary>
        internal Converter()
        {
            this.serializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            };

            this.serializerSettings.Converters.Add(new ListConverter());

            this.StringEnumSerializer = new JsonSerializer()
            {
                Converters = { new StringEnumConverter() },
                NullValueHandling = NullValueHandling.Ignore,
            };
        }

        /// <summary>
        /// String enum serializer.
        /// </summary>
        private JsonSerializer StringEnumSerializer { get; }

        /// <summary>
        /// Convert json string to desired object.
        /// </summary>
        /// <typeparam name="T">Type to convert to.</typeparam>
        /// <param name="json">Json string.</param>
        /// <returns></returns>
        internal T Convert<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(
                json,
                this.serializerSettings);
        }

        /// <summary>
        /// Serialize property change tracking to string.
        /// </summary>
        /// <param name="changeTracker">Change tracker.</param>
        /// <param name="additionalProperties">Additional properties.</param>
        /// <param name="appendRootObject">Append root object.</param>
        /// <returns></returns>
        internal string Convert(IPropertyChangeTracking changeTracker, Dictionary<string, object> additionalProperties, bool appendRootObject = true)
        {
            /*
               Two behaviors appendRootObject covers.
               1. appendRootObject = false

                {
                    "Property1": "Value",
                    "Property2": "Value",
                    etc..
                }

                2. appendRootObject = true
                It appends object name. For an instance, object Message with two properties

                {
                    "message": {
                        "Property1": "Value",
                        "Property2": "Value",
                        etc...
                    }
                }
            */

            JObject rootObject = new JObject();
            if (appendRootObject)
            {
                rootObject.Add(
                    changeTracker.GetType().Name,
                    this.BuildObjectFromIPropertyChangeTracking(changeTracker));
            }
            else
            {
                rootObject = this.BuildObjectFromIPropertyChangeTracking(changeTracker);
            }

            // Additional properties aren't part of initial object, for example "Comment" in SendMail.
            if (null != additionalProperties && additionalProperties.Count > 0)
            {
                foreach (KeyValuePair<string, object> additionalProperty in additionalProperties)
                {
                    rootObject.Add(
                        additionalProperty.Key,
                        JToken.FromObject(additionalProperty.Value));
                }
            }

            return JsonConvert.SerializeObject(rootObject);
        }

        /// <summary>
        /// Build object from change tracker.
        /// </summary>
        /// <param name="changeTracker">Change tracker.</param>
        /// <param name="appendOdataType">Append odata type.</param>
        /// <param name="odataType">odata type.</param>
        /// <returns></returns>
        private JObject BuildObjectFromIPropertyChangeTracking(IPropertyChangeTracking changeTracker, bool appendOdataType = false, string odataType = null)
        {
            JObject jObject = new JObject();
            if (appendOdataType)
            {
                if (!string.IsNullOrEmpty(odataType))
                {
                    jObject["@odata.type"] = odataType;
                }
            }

            foreach (PropertyDefinition changedProperty in changeTracker.GetChangedProperties())
            {
                object propertyValue = changeTracker[changedProperty];
                if (changedProperty.ChangeTrackable)
                {
                    if (changedProperty.IsEnumerable)
                    {
                        IList<object> list = changedProperty.ActivateList(changeTracker[changedProperty]);
                        
                        JArray jArray = new JArray();
                        foreach (object entry in list)
                        {
                            jArray.Add(
                                this.BuildObjectFromIPropertyChangeTracking(
                                    entry as IPropertyChangeTracking,
                                    changedProperty.IEnumerableUnderlyingType.IsAbstract,
                                    changeTracker["ODataType"] as string));
                        }

                        jObject.Add(
                            changedProperty.Name,
                            jArray);
                    }
                    else
                    {
                        jObject[changedProperty.Name] = JToken.FromObject(
                            this.BuildObjectFromIPropertyChangeTracking(propertyValue as IPropertyChangeTracking,
                                changedProperty.Type.IsAbstract,
                                changeTracker["ODataType"] as string),
                            this.StringEnumSerializer);
                    }
                }
                else
                {
                    jObject[changedProperty.Name] = JToken.FromObject(
                        propertyValue,
                        this.StringEnumSerializer);
                }
            }

            return jObject;
        }
    }
}
