namespace Microsoft.Graph.Exchange
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Contains list of preferences requested by client.
    /// </summary>
    public class Preferences : ICollection<string>
    {
        /// <summary>
        /// List of preferences.
        /// </summary>
        private IList<string> preferencesList;

        /// <summary>
        /// Create new instance of <see cref="Preferences"/>
        /// </summary>
        internal Preferences()
        {
            this.preferencesList = new List<string>();
        }

        /// <summary>
        /// Get enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<string> GetEnumerator()
        {
            return this.preferencesList.GetEnumerator();
        }

        /// <summary>
        /// Get enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Add item to the list.
        /// </summary>
        /// <param name="item">Item to add.</param>
        public void Add(string item)
        {
            if (string.IsNullOrEmpty(item))
            {
                return;
            }

            if (this.Contains(item))
            {
                return;
            }

            this.preferencesList.Add(item);
        }

        /// <summary>
        /// Clear preference list.
        /// </summary>
        public void Clear()
        {
            this.preferencesList.Clear();
        }

        /// <summary>
        /// Check if already contains item.
        /// </summary>
        /// <param name="item">item to check.</param>
        /// <returns></returns>
        public bool Contains(string item)
        {
            return this.preferencesList.Contains(item);
        }

        /// <summary>
        /// Copy to.
        /// </summary>
        /// <param name="array">Array to copy to.</param>
        /// <param name="arrayIndex">Start index.</param>
        public void CopyTo(string[] array, int arrayIndex)
        {
            this.preferencesList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Remove entry from the list.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <returns></returns>
        public bool Remove(string item)
        {
            return this.preferencesList.Remove(item);
        }

        /// <summary>
        /// Item count.
        /// </summary>
        public int Count
        {
            get { return this.preferencesList.Count; }
        }

        /// <summary>
        /// Not read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }
    }
}
