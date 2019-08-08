namespace Microsoft.Graph.Exchange
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Change collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ChangeCollection<T> : FindResults<T>, IEnumerable<T>
    {
        /// <summary>
        /// Create new instance of <see cref="ChangeCollection{T}"/>
        /// </summary>
        internal ChangeCollection()
        {
        }

        /// <summary>
        /// Sync state.
        /// </summary>
        public string SyncState { get; internal set; }

        /// <summary>
        /// Items.
        /// </summary>
        public Collection<T> Items
        {
            get { return this.ResultsCollection; }
        }

        /// <summary>
        /// Get enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        /// <summary>
        /// Get enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
