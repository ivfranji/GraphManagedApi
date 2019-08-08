namespace Microsoft.Graph.CoreHttp
{
    /// <summary>
    /// Retry handler options.
    /// </summary>
    internal class HttpRetryOptions
    {
        /// <summary>
        /// Default retry options.
        /// </summary>
        private static HttpRetryOptions defaultHttpRetryOptions = new HttpRetryOptions(
            3,
            HttpRetryOptions.DefaultDelaySeconds);

        /// <summary>
        /// Default delay.
        /// </summary>
        private const int DefaultDelaySeconds = 5;

        /// <summary>
        /// Create new instance of <see cref="HttpRetryOptions"/>
        /// </summary>
        /// <param name="retryCount"></param>
        public HttpRetryOptions(int retryCount, int delaySeconds)
        {
            this.RetryCount = retryCount;
            this.DelaySeconds = delaySeconds;
        }

        /// <summary>
        /// Retry count
        /// </summary>
        public int RetryCount
        {
            get;
        }

        /// <summary>
        /// Default delay;
        /// </summary>
        public int DelaySeconds
        {
            get;
        }

        /// <summary>
        /// Default retry options.
        /// </summary>
        internal static HttpRetryOptions DefaultHttpRetryOptions
        {
            get
            {
                return HttpRetryOptions.defaultHttpRetryOptions;
            }
        }
    }
}