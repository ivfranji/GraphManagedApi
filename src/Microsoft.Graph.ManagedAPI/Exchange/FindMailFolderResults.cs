namespace Microsoft.Graph.Exchange
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.Graph.Utilities;

    /// <summary>
    /// Find folder results.
    /// </summary>
    public class FindMailFolderResults : FindResults<MailFolder>, IEnumerable<MailFolder>
    {
        /// <summary>
        /// Create new instance of <see cref="FindMailFolderResults"/>
        /// </summary>
        internal FindMailFolderResults(PageResponseCollection<MailFolder> responseCollection)
        {
            responseCollection.ThrowIfNull(nameof(responseCollection));
            foreach (MailFolder mailFolder in responseCollection.Value)
            {
                this.MailFolders.Add(mailFolder);
            }

            this.MoreAvailable = responseCollection.HasNextLink;
        }

        /// <summary>
        /// Mail folders.
        /// </summary>
        public Collection<MailFolder> MailFolders
        {
            get { return this.ResultsCollection; }
        }

        /// <summary>
        /// Get enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<MailFolder> GetEnumerator()
        {
            return this.MailFolders.GetEnumerator();
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
