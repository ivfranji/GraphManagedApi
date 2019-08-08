namespace Microsoft.Graph.Exchange
{
    using System;
    using System.Text;
    using Microsoft.Graph.CoreHttp;
    using Microsoft.Graph.Exchange;
    using Microsoft.Graph.Utilities;

    /// <summary>
    /// Sync state query builder.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class SyncStateQuery<T> : IUrlQuery where T : Entity
    {
        /// <summary>
        /// Delta token.
        /// </summary>
        private const string deltaToken = "$deltatoken";

        /// <summary>
        /// Skip token.
        /// </summary>
        private const string skipToken = "$skiptoken";

        /// <summary>
        /// Raw token.
        /// </summary>
        private string rawToken;

        /// <summary>
        /// Token type.
        /// </summary>
        private TokenType tokenType;

        /// <summary>
        /// Create new instance of <see cref="SyncStateQuery{T}"/>
        /// </summary>
        /// <param name="pageResponseCollection"></param>
        internal SyncStateQuery(PageResponseCollection<T> pageResponseCollection)
        {
            pageResponseCollection.ThrowIfNull(nameof(pageResponseCollection));
            if (pageResponseCollection.HasDeltaLink)
            {
                this.Initialize(
                    pageResponseCollection.DeltaLink,
                    TokenType.DeltaToken);
            }

            else if (pageResponseCollection.HasNextLink)
            {
                this.Initialize(
                    pageResponseCollection.NextLink,
                    TokenType.SkipToken);
            }

            else
            {
                throw new ArgumentException("Page response doesn't contain paging link.");
            }
        }

        /// <summary>
        /// Create new instance of <see cref="SyncStateQuery{T}"/>
        /// </summary>
        private SyncStateQuery()
        {
        }

        /// <summary>
        /// Raw token.
        /// </summary>
        internal string RawToken
        {
            get { return this.rawToken; }
        }

        /// <summary>
        /// Token type.
        /// </summary>
        internal TokenType Type
        {
            get { return this.tokenType; }
        }

        /// <summary>
        /// Get Url query.
        /// </summary>
        /// <returns></returns>
        public string GetUrlQuery()
        {
            return $"{this.GetTokenTypeKey(this.tokenType)}={this.rawToken}";
        }

        /// <summary>
        /// Deserialize raw sync state.
        /// </summary>
        /// <param name="rawSyncState"></param>
        /// <returns></returns>
        internal static SyncStateQuery<T> Deserialize(string rawSyncState)
        {
            rawSyncState.ThrowIfNullOrEmpty(nameof(rawSyncState));
            byte[] rawSyncStateBytes = Convert.FromBase64String(rawSyncState);
            SyncStateQuery<T> syncStateQuery = new SyncStateQuery<T>();
            syncStateQuery.tokenType = (TokenType)(int)rawSyncStateBytes[0];
            
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < rawSyncStateBytes.Length; i++)
            {
                sb.Append((char)rawSyncStateBytes[i]);
            }

            syncStateQuery.rawToken = sb.ToString();
            return syncStateQuery;
        }

        /// <summary>
        /// Serialize token into string.
        /// </summary>
        /// <returns></returns>
        internal string Serialize()
        {
            if (string.IsNullOrEmpty(this.rawToken))
            {
                return null;
            }

            int rawTokenSize = Encoding.UTF8.GetByteCount(this.rawToken);
            byte[] rawTokenBytes = Encoding.UTF8.GetBytes(this.rawToken);

            byte[] bytes = new byte[rawTokenSize + 1];
            bytes[0] = (byte)this.tokenType;

            for (int i = 0; i < rawTokenSize; i++)
            {
                bytes[i + 1] = rawTokenBytes[i];
            }

            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Get token type key.
        /// </summary>
        /// <param name="typeOfToken">Type of token.</param>
        /// <returns></returns>
        private string GetTokenTypeKey(TokenType typeOfToken)
        {
            return typeOfToken == TokenType.DeltaToken
                ? SyncStateQuery<T>.deltaToken
                : SyncStateQuery<T>.skipToken;
        }

        /// <summary>
        /// Initialize sync state.
        /// </summary>
        /// <param name="link"></param>
        /// <param name="typeOfToken"></param>
        private void Initialize(string link, TokenType typeOfToken)
        {
            string token = this.GetTokenTypeKey(typeOfToken);
            Uri deltaUri = new Uri(link);
            HttpQueryParameter queryParams = new HttpQueryParameter(deltaUri.Query);
            if (!queryParams.ContainsKey(token))
            {
                throw new ArgumentException($"Link doesn't contain delta token: '{token}'.");
            }

            this.rawToken = (string)queryParams[token];
            this.tokenType = typeOfToken;
        }

        /// <summary>
        /// Type of the token.
        /// </summary>
        [Flags]
        internal enum TokenType
        {
            /// <summary>
            /// Unknown token.
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// Skip token.
            /// </summary>
            SkipToken = 1,

            /// <summary>
            /// Delta token.
            /// </summary>
            DeltaToken = 2
        }
    }
}
