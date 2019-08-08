namespace Microsoft.Graph.Exchange
{
    using Microsoft.Graph.Utilities;

    public abstract class SyncChange<T> where T : Entity
    {
        /// <summary>
        /// Removed key.
        /// </summary>
        private const string removedKey = "@removed";

        protected SyncChange(T item)
        {
            item.ThrowIfNull(nameof(item));

            this.Item = item;
            this.ChangeType = ChangeType.Created;

            if (this.Item.NonDeclaredProperties != null &&
                this.Item.NonDeclaredProperties.ContainsKey(SyncChange<T>.removedKey))
            {
                this.ChangeType = ChangeType.Deleted;
            }
        }

        public  T Item { get; }

        public ChangeType ChangeType { get; protected set; }
    }
}