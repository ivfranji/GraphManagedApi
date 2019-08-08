namespace Microsoft.Graph.Logging
{
    using System;

    /// <summary>
    /// Log flag.
    /// </summary>
    [Flags]
    public enum LogFlag : int
    {
        /// <summary>
        /// Log nothing.
        /// </summary>
        None = 0,

        /// <summary>
        /// Log request.
        /// </summary>
        Request = 1,

        /// <summary>
        /// Log request headers.
        /// </summary>
        RequestHeaders = 2,

        /// <summary>
        /// Log response.
        /// </summary>
        Response = 4,

        /// <summary>
        /// Log response headers.
        /// </summary>
        ResponseHeaders = 8,

        /// <summary>
        /// Log everything.
        /// </summary>
        All = LogFlag.Request | LogFlag.RequestHeaders | LogFlag.Response | LogFlag.ResponseHeaders
    }
}
