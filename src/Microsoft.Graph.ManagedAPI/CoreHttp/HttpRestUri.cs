namespace Microsoft.Graph.CoreHttp
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represent rest Uri.
    /// </summary>
    internal class HttpRestUri
    {
        /// <summary>
        /// 'immutable' part of the uri.
        /// </summary>
        private Uri immutableBaseUri;

        /// <summary>
        /// query parameters.
        /// </summary>
        private HttpQueryParameter queryParameter;

        /// <summary>
        /// Segments.
        /// </summary>
        private IList<string> segments;

        /// <summary>
        /// Create new instance of <see cref="HttpRestUri"/>
        /// </summary>
        /// <param name="restUri">Rest uri.</param>
        public HttpRestUri(string restUri)
            : this(new Uri(restUri))
        {
        }

        /// <summary>
        /// Create new instance of <see cref="HttpRestUri"/>
        /// </summary>
        /// <param name="restUri">Rest uri.</param>
        public HttpRestUri(Uri restUri)
        {
            if (restUri == null)
            {
                throw new ArgumentNullException(nameof(restUri));
            }

            this.immutableBaseUri = restUri;
            this.queryParameter = new HttpQueryParameter("");
            this.segments = new List<string>();
        }

        /// <summary>
        /// Add query parameter to the uri.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public void AddQueryParameter(string key, object value)
        {
            this.queryParameter.Add(
                key,
                value);
        }

        /// <summary>
        /// Add query or update existing values.
        /// </summary>
        /// <param name="query">Query.</param>
        public void AddQuery(string query)
        {
            HttpQueryParameter param = new HttpQueryParameter(query);
            foreach (KeyValuePair<string, object> pair in param)
            {
                // if query exist, update it.
                if (this.queryParameter.Contains(pair))
                {
                    this.queryParameter[pair.Key] = pair.Value;
                }
                else
                {
                    this.queryParameter.Add(pair.Key, pair.Value);
                }
            }
        }

        /// <summary>
        /// Remove query parameter from uri.
        /// </summary>
        /// <param name="key">Key.</param>
        public void RemoveQueryParameter(string key)
        {
            this.queryParameter.Remove(key);
        }

        /// <summary>
        /// Add segment to the uri.
        /// </summary>
        /// <param name="segment">Segment.</param>
        public void AddSegment(string segment)
        {
            this.segments.Add(segment);
        }

        /// <summary>
        /// Remove segment from the uri.
        /// </summary>
        /// <param name="segment"></param>
        public void RemoveSegment(string segment)
        {
            this.segments.Remove(segment);
        }

        /// <summary>
        /// Try to get segment from specific position.
        /// </summary>
        /// <param name="segmentPosition">Segment position.</param>
        /// <param name="segment">Segment.</param>
        /// <returns>True if segment found.</returns>
        public bool TryGetSegment(int segmentPosition, out string segment)
        {
            segment = string.Empty;
            if (segmentPosition >= this.immutableBaseUri.Segments.Length + this.segments.Count)
            {
                return false;
            }

            if (segmentPosition >= this.immutableBaseUri.Segments.Length)
            {
                segmentPosition = segmentPosition - this.immutableBaseUri.Segments.Length;
                segment = this.FormatSegment(
                    this.segments[segmentPosition]);
            }
            else
            {
                segment = this.FormatSegment(
                    this.immutableBaseUri.Segments[segmentPosition]);
            }

            return true;
        }

        /// <summary>
        /// Cast conversion from <see cref="HttpRestUri"/> to <see cref="Uri"/>
        /// </summary>
        /// <param name="restUri">Rest uri.</param>
        public static implicit operator Uri(HttpRestUri restUri)
        {
            return restUri.GetUri();
        }

        /// <summary>
        /// Cast from <see cref="Uri"/> to <see cref="HttpRestUri"/>
        /// </summary>
        /// <param name="restUri">Rest uri.</param>
        public static implicit operator HttpRestUri(Uri restUri)
        {
            return new HttpRestUri(restUri);
        }

        /// <summary>
        /// Create uri.
        /// </summary>
        /// <returns></returns>
        private Uri GetUri()
        {
            UriBuilder uriBuilder = new UriBuilder(this.immutableBaseUri);
            HttpQueryParameter immutableQueryParameter = new HttpQueryParameter(uriBuilder.Query);

            if (this.queryParameter.Count > 0)
            {
                foreach (KeyValuePair<string, object> pair in this.queryParameter)
                {
                    if (immutableQueryParameter.ContainsKey(pair.Key))
                    {
                        // dont touch it, it should remain 'immutable'.
                        continue;
                    }

                    immutableQueryParameter.Add(
                        pair.Key,
                        pair.Value);
                }
            }

            uriBuilder.Query = immutableQueryParameter.ToQueryString();
            List<string> immutableSegments = new List<string>();
            if (this.immutableBaseUri.Segments.Length > 0)
            {
                foreach (string segment in this.immutableBaseUri.Segments)
                {
                    immutableSegments.Add(segment.TrimEnd('/'));
                }
            }

            if (this.segments.Count > 0)
            {
                immutableSegments.AddRange(this.segments);
            }

            uriBuilder.Path = string.Join("/", immutableSegments).TrimStart('/');

            return uriBuilder.Uri;
        }

        /// <summary>
        /// Format segment.
        /// </summary>
        /// <param name="segment">Segment.</param>
        /// <returns>Segment without trailing '/'</returns>
        private string FormatSegment(string segment)
        {
            if (segment != "/")
            {
                return segment.TrimEnd('/');
            }

            return segment;
        }
    }
}
