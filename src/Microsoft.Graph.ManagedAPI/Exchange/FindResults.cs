namespace Microsoft.Graph.Exchange
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Find results contract.
    /// </summary>
    /// <typeparam name="T">Underlying type.</typeparam>
    public abstract class FindResults<T>
    {
        /// <summary>
        /// Mail folders.
        /// </summary>
        Collection<T> resultsCollection;

        /// <summary>
        /// Next page offset.
        /// </summary>
        private int? nextPageOffset;

        /// <summary>
        /// More folders available.
        /// </summary>
        private bool moreAvailable;

        /// <summary>
        /// Create new instance of <see cref="FindResults{T}"/>
        /// </summary>
        protected FindResults()
        {
            this.resultsCollection = new Collection<T>();
            this.moreAvailable = false;
        }

        /// <summary>
        /// Results collection.
        /// </summary>
        protected Collection<T> ResultsCollection
        {
            get { return this.resultsCollection; }
        }

        /// <summary>
        /// Total count.
        /// </summary>
        public int TotalCount
        {
            get { return this.resultsCollection.Count; }
        }

        /// <summary>
        /// More items available.
        /// </summary>
        public bool MoreAvailable
        {
            get { return this.moreAvailable; }
            internal set { this.moreAvailable = value; }
        }

        /// <summary>
        /// Next page offset.
        /// </summary>
        public int? NextPageOffset
        {
            get { return this.nextPageOffset; }
            internal set { this.nextPageOffset = value; }
        }
    }

    public class FindEntityResults<T> : FindResults<T> where T : Entity
    {
        internal FindEntityResults(PageResponseCollection<T> response)
        {
            foreach (T item in response.Value)
            {
                this.ResultsCollection.Add(item);
            }

            this.MoreAvailable = response.HasNextLink;
        }

        /// <summary>
        /// Items.
        /// </summary>
        public Collection<T> Items
        {
            get { return this.ResultsCollection; }
        }
    }
}
