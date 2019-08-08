namespace Microsoft.Graph.Exchange
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Utilities;

    /// <summary>
    /// Find item results.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FindItemResults<T> : FindResults<T> where T : Entity
    {
        /// <summary>
        /// Find item results.
        /// </summary>
        internal FindItemResults(PageResponseCollection<T> pageResponseCollection)
            : base()
        {
            pageResponseCollection.ThrowIfNull(nameof(pageResponseCollection));
            foreach (T item in pageResponseCollection.Value)
            {
                this.Items.Add(item);
            }

            this.MoreAvailable = pageResponseCollection.HasNextLink;
        }

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
            return this.ResultsCollection.GetEnumerator();
        }
    }
}
