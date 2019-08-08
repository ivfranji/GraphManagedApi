namespace Microsoft.Graph.Exception
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Root exception object.
    /// </summary>
    internal class RootExceptionObject
    {
        /// <summary>
        /// Error.
        /// </summary>
        [JsonProperty("error")]
        internal Error Error { get; set; }
    }

    /// <summary>
    /// Error object.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Error code.
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; internal set; }

        /// <summary>
        /// Error message.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; internal set; }

        /// <summary>
        /// Inner error property.
        /// </summary>
        [JsonProperty("innerError")]
        public InnerError InnerError { get; internal set; }
    }

    /// <summary>
    /// Inner error.
    /// </summary>
    public class InnerError
    {
        /// <summary>
        /// Request id.
        /// </summary>
        [JsonProperty("request-id")]
        public string RequestId { get; internal set; }

        /// <summary>
        /// Date.
        /// </summary>
        [JsonProperty("date")]
        public DateTime Date { get; internal set; }

        /// <summary>
        /// Error.
        /// </summary>
        [JsonProperty("error")]
        public Error Error { get; internal set; }
    }
}
