namespace Microsoft.Graph.Logging
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines interface for log writer.
    /// </summary>
    public interface ILogWriter
    {
        /// <summary>
        /// Log message.
        /// </summary>
        /// <param name="logType">Log type.</param>
        /// <param name="logMessage">Log message.</param>
        Task Log(string logType, string logMessage);

        /// <summary>
        /// Indicate if logging is enabled.
        /// </summary>
        bool LoggingEnabled { get; set; }

        /// <summary>
        /// Log flag.
        /// </summary>
        LogFlag LogFlag { get; set; }
    }
}
