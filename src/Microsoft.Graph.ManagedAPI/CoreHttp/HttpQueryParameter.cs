namespace Microsoft.Graph.CoreHttp
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Query parameter.
    /// </summary>
    internal class HttpQueryParameter : IDictionary<string, object>
    {
        /// <summary>
        /// Underlying dictionary.
        /// </summary>
        private Dictionary<string, object> parameters;

        /// <summary>
        /// Create new instance of <see cref="QueryParameter"/>
        /// </summary>
        /// <param name="query">Query.</param>
        public HttpQueryParameter(string query)
        {
            this.parameters = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(query))
            {
                query = query.TrimStart('?');
                foreach (string queryPart in query.Split('&'))
                {
                    string[] nameValuePair = queryPart.Split(new []{ '=' }, 2);
                    string value = string.Empty;
                    if (nameValuePair.Length == 2)
                    {
                        value = nameValuePair[1];
                    }

                    this.parameters.Add(
                        nameValuePair[0],
                        value);
                }
            }
        }

        /// <summary>
        /// Indexer.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns></returns>
        public object this[string key]
        {
            get { return this.parameters[key]; }
            set { this.parameters[key] = value; }
        }

        /// <summary>
        /// Keys.
        /// </summary>
        public ICollection<string> Keys
        {
            get { return this.parameters.Keys; }
        }

        /// <summary>
        /// Values.
        /// </summary>
        public ICollection<object> Values
        {
            get { return this.parameters.Values; }
        }

        /// <summary>
        /// Count of the items.
        /// </summary>
        public int Count
        {
            get { return this.parameters.Count; }
        }

        /// <summary>
        /// Is readonly - false.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Add item in dictionary.
        /// </summary>
        /// <param name="item">Item to add.</param>
        public void Add(KeyValuePair<string, object> item)
        {
            this.Add(
                item.Key,
                item.Value);
        }

        /// <summary>
        /// Add key / value to dictionary.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public void Add(string key, object value)
        {
            this.parameters.Add(
                key,
                value);
        }

        /// <summary>
        /// Get enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return this.parameters.GetEnumerator();
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
        /// Clear dictionary.
        /// </summary>
        public void Clear()
        {
            this.parameters.Clear();
        }

        /// <summary>
        /// Remove specific key from dictionary.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        /// <returns>True if key removed, false otherwise.</returns>
        public bool Remove(KeyValuePair<string, object> item)
        {
            return this.Remove(item.Key);
        }

        /// <summary>
        /// Remove specific key from dictionary.
        /// </summary>
        /// <param name="key">Key to remove.</param>
        /// <returns>True if key removed, false otherwise.</returns>
        public bool Remove(string key)
        {
            return this.parameters.Remove(key);
        }

        /// <summary>
        /// Contains item. It will check if key of the item exists in dictionary, not value.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <returns>True if contains specific key.</returns>
        public bool Contains(KeyValuePair<string, object> item)
        {
            return this.ContainsKey(item.Key);
        }

        /// <summary>
        /// Contains key.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>True if contains specific key.</returns>
        public bool ContainsKey(string key)
        {
            return this.parameters.ContainsKey(key);
        }

        /// <summary>
        /// Try get value.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <returns></returns>
        public bool TryGetValue(string key, out object value)
        {
            return this.parameters.TryGetValue(
                key,
                out value);
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="array">Not implemented.</param>
        /// <param name="arrayIndex">Not implemented.</param>
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Translate parameter to query string.
        /// </summary>
        /// <returns></returns>
        public string ToQueryString()
        {
            if (this.Count == 0)
            {
                return string.Empty;
            }

            return string.Join(
                "&",
                this.GetKeyValuePair());
        }

        /// <summary>
        /// Format KeyValue pair into key=value format.
        /// It omit key if value is null.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> GetKeyValuePair()
        {
            foreach (KeyValuePair<string, object> keyPair in this.parameters)
            {
                if (null == keyPair.Value)
                {
                    continue;
                }

                yield return $"{keyPair.Key}={keyPair.Value}";
            }
        }
    }
}
