namespace Microsoft.Graph.Exchange
{
    using System.Collections;
    using System.Collections.Generic;

    internal class ResponseCollection<T> : IList<T>
    {
        public ResponseCollection()
        {
            this.Result = new List<T>();
        }

        public ResponseCollection(IList<T> result)
        {
            if (null == result)
            {
                this.Result = new List<T>();
            }
            else
            {
                this.Result = result;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.Result.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Add(T item)
        {
            this.Result.Add(item);
        }

        public void Clear()
        {
            this.Result.Clear();
        }

        public bool Contains(T item)
        {
            return this.Result.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.Result.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return this.Result.Remove(item);
        }

        public int Count
        {
            get { return this.Result.Count; }
        }

        public bool IsReadOnly
        {
            get { return this.Result.IsReadOnly; }
        }

        public int IndexOf(T item)
        {
            return this.Result.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            this.Result.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

        public T this[int index]
        {
            get { return this.Result[index]; }
            set { this.Result[index] = value; }
        }

        public IList<T> Result { get; }
    }
}
