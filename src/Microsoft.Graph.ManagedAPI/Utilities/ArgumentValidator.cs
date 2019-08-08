namespace Microsoft.Graph.Utilities
{
    using System;

    /// <summary>
    /// Contains common argument validators.
    /// </summary>
    internal static class ArgumentValidator
    {
        /// <summary>
        /// Throws if string null or empty.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="argName">Name of argument.</param>
        internal static void ThrowIfNullOrEmpty(this string value, string argName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(argName);
            }
        }

        /// <summary>
        /// Throw if specified object null.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="argName">Name of argument.</param>
        internal static void ThrowIfNull(this object value, string argName)
        {
            if (null == value)
            {
                throw new ArgumentNullException(argName);
            }
        }

        /// <summary>
        /// Throw if specified guid is empty.
        /// </summary>
        /// <param name="guid">Guid to validate.</param>
        /// <param name="argName">Arg name.</param>
        internal static void ThrowIfGuidEmpty(this Guid guid, string argName)
        {
            if (guid == Guid.Empty)
            {
                throw new ArgumentNullException(argName);
            }
        }

        /// <summary>
        /// Throw if array null or empty.
        /// </summary>
        /// <param name="array">Array.</param>
        /// <param name="argName">Arg name.</param>
        internal static void ThrowIfNullOrEmptyArray(this object[] array, string argName)
        {
            if (null == array || array.Length == 0)
            {
                throw new ArgumentNullException(argName);
            }
        }
    }
}
