namespace Microsoft.Graph.Exception
{
    using System;

    /// <summary>
    /// Service exception object.
    /// </summary>
    public class ServiceException : Exception
    {
        /// <summary>
        /// Create new instance of <see cref="ServiceException"/>
        /// </summary>
        /// <param name="exceptionObject">Exception object.</param>
        internal ServiceException(RootExceptionObject exceptionObject)
        {
            this.Error = exceptionObject.Error;
            this.Message = this.Error?.Message;
        }

        /// <summary>
        /// Error message.
        /// </summary>
        public override string Message { get; }

        /// <summary>
        /// Service error.
        /// </summary>
        public Error Error { get; }

        /// <summary>
        /// ToString impl.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{this.Error?.Code} | {this.Message}";
        }
    }
}