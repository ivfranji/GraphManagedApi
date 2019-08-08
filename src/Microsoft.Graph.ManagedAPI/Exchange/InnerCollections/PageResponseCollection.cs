namespace Microsoft.Graph.Exchange
{
    using Newtonsoft.Json;

    /// <summary>
    /// Response collection with a Value page.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class PageResponseCollection<T> where T : Entity
    {
        /// <summary>
        /// Create new instance of <see cref="PageResponseCollection{T}"/>
        /// </summary>
        internal PageResponseCollection()
        {
            this.Value = new ResponseCollection<T>();
        }

        /// <summary>
        /// Odata context.
        /// </summary>
        [JsonProperty(PropertyName = "@odata.context")]
        internal string ODataContext { get; set; }

        /// <summary>
        /// Odata next link.
        /// </summary>
        [JsonProperty(PropertyName = "@odata.nextLink")]
        internal string NextLink { get; set; }

        /// <summary>
        /// Delta link.
        /// </summary>
        [JsonProperty(PropertyName = "@odata.deltaLink")]
        internal string DeltaLink { get; set; }

        /// <summary>
        /// Value collection.
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        internal ResponseCollection<T> Value { get; }
        
        /// <summary>
        /// Has delta link.
        /// </summary>
        internal bool HasDeltaLink
        {
            get { return !string.IsNullOrEmpty(this.DeltaLink); }
        }

        /// <summary>
        /// Has next link
        /// </summary>
        internal bool HasNextLink
        {
            get { return !string.IsNullOrEmpty(this.NextLink); }
        }

        /// <summary>
        /// Register entity service with entities.
        /// </summary>
        internal void RegisterEntityService(IEntityService entityService)
        {
            foreach (T entity in this.Value)
            {
                entity.ActionInvoker = entityService;
                entity.EntityService = entityService;
            }
        }
    }
}
