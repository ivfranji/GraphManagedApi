namespace Microsoft.Graph.Logging
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Default log writer.
    /// </summary>
    internal class ConsoleLogWriter : ILogWriter
    {
        /// <summary>
        /// Lazy singleton instance.
        /// </summary>
        private static Lazy<ConsoleLogWriter> instance = new Lazy<ConsoleLogWriter>(() => new ConsoleLogWriter());

        /// <summary>
        /// Text writer.
        /// </summary>
        private readonly TextWriter textWriter;

        /// <summary>
        /// Create new instance of <see cref="DefaultLogWriter"/>
        /// </summary>
        private ConsoleLogWriter()
        {
            this.LoggingEnabled = false;
            this.LogFlag = LogFlag.None;
            this.textWriter = Console.Out;
        }

        /// <summary>
        /// Lazy public instance.
        /// </summary>
        public static Lazy<ConsoleLogWriter> Instance
        {
            get { return ConsoleLogWriter.instance; }
        }

        ///<inheritdoc cref="ILogWriter.Log"/>
        public Task Log(string logType, string logMessage)
        {
            this.textWriter.Write(logMessage);
            return Task.FromResult(0);
        }

        ///<inheritdoc cref="ILogWriter.LoggingEnabled"/>
        public bool LoggingEnabled { get; set; }

        ///<inheritdoc cref="ILogWriter.LogFlag"/>
        public LogFlag LogFlag { get; set; }
    }
}
