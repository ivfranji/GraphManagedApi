namespace Microsoft.Graph
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Action invoker contract.
    /// </summary>
    public interface IActionInvoker
    {
        /// <summary>
        /// Invoke graph model method.
        /// </summary>
        /// <param name="methodName">Method to invoke.</param>
        /// <param name="entity">Entity.</param>
        /// <param name="additionalParameters">Additional parameters.</param>
        /// <returns></returns>
        Task InvokeAsync(string methodName, Entity entity, Dictionary<string, object> additionalParameters);

        /// <summary>
        /// Invoke async.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="entity"></param>
        /// <param name="additionalParameters"></param>
        /// <returns></returns>
        Task<T> InvokeAsync<T>(string methodName, Entity entity, Dictionary<string, object> additionalParameters);

    }
}
