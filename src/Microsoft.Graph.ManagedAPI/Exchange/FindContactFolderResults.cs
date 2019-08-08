namespace Microsoft.Graph.Exchange
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.Graph.Utilities;

    /// <summary>
    /// Find contact folder result.
    /// </summary>
    public class FindContactFolderResults : FindResults<ContactFolder>, IEnumerable<ContactFolder>
    {
        /// <summary>
        /// Create new instance of <see cref="FindContactFolderResults"/>
        /// </summary>
        internal FindContactFolderResults(PageResponseCollection<ContactFolder> responseCollection)
        {
            responseCollection.ThrowIfNull(nameof(responseCollection));
            foreach (ContactFolder contactFolder in responseCollection.Value)
            {
                this.ContactFolders.Add(contactFolder);
            }

            this.MoreAvailable = responseCollection.HasNextLink;
        }

        /// <summary>
        /// Contact folders.
        /// </summary>
        public Collection<ContactFolder> ContactFolders
        {
            get { return this.ResultsCollection; }
        }

        /// <summary>
        /// Get enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ContactFolder> GetEnumerator()
        {
            return this.ContactFolders.GetEnumerator();
        }

        /// <summary>
        /// Get enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}