namespace Microsoft.Graph.Exchange
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Graph.CoreAuth;
    using Microsoft.Graph.CoreHttp;
    using Microsoft.Graph.CoreJson;
    using Microsoft.Graph.Exception;
    using Microsoft.Graph.Identities;
    using Microsoft.Graph.GraphModel;
    using Microsoft.Graph.Logging;
    using Microsoft.Graph.Search;
    using Microsoft.Graph.Utilities;

    /// <summary>
    /// Exchange service.
    /// </summary>
    public class ExchangeService : IEntityService
    {
        /// <summary>
        /// Exchange user.
        /// </summary>
        private User exchangeUser;

        /// <summary>
        /// Create new instance of <see cref="ExchangeService"/> for specific identity.
        /// </summary>
        /// <param name="graphIdentity">Graph identity.</param>
        /// <param name="authorizationProvider">Authorization provider.</param>
        /// <param name="userAgent">User agent.</param>
        /// <param name="beta">Use beta endpoint.</param>
        internal ExchangeService(IGraphIdentity graphIdentity, IAuthorizationProvider authorizationProvider, Converter jsonConverter, string userAgent, bool beta)
        {
            this.Identity = graphIdentity;
            this.HttpRequestContext = new HttpRequestContext()
            {
                AuthorizationProvider = authorizationProvider,
            };

            this.JsonConverter = jsonConverter;
            this.UserAgent = userAgent;
            this.Preferences = new Preferences();
            this.Beta = beta;
        }

        /// <summary>
        /// Json converter.
        /// </summary>
        private Converter JsonConverter { get; }

        /// <summary>
        /// Http request context.
        /// </summary>
        internal HttpRequestContext HttpRequestContext { get; }
        
        #region Logging

        /// <summary>
        /// Log writer.
        /// </summary>
        public ILogWriter LogWriter
        {
            get { return this.HttpRequestContext.LogWriter; }
            set
            {
                this.HttpRequestContext.LogWriter = value;
                if (null == value)
                {
                    // in case it is null, it will revert to default 
                    // implementation, in that case turn off logging
                    this.HttpRequestContext.LogWriter.LoggingEnabled = false;
                }
            }
        }

        /// <summary>
        /// Indicate if logging enabled.
        /// </summary>
        public bool LoggingEnabled
        {
            get { return this.LogWriter.LoggingEnabled; }
            set { this.LogWriter.LoggingEnabled = value; }
        }

        /// <summary>
        /// Log flag.
        /// </summary>
        public LogFlag LogFlag
        {
            get { return this.LogWriter.LogFlag; }
            set
            {
                if (value == LogFlag.None)
                {
                    this.LoggingEnabled = false;
                }

                this.LogWriter.LogFlag = value;
            }
        }

        #endregion

        /// <summary>
        /// Graph identity.
        /// </summary>
        public IGraphIdentity Identity { get; }

        /// <summary>
        /// User agent associated with service.
        /// </summary>
        public string UserAgent { get; }

        /// <summary>
        /// Beta endpoint.
        /// </summary>
        public bool Beta { get; }

        /// <summary>
        /// Preferences to be sent to the server.
        /// </summary>
        public Preferences Preferences { get; }

        #region MailFolders operations

        /// <summary>
        /// Find all folders under specified root with specified <see cref="MailFolderView"/>.
        /// </summary>
        /// <param name="parentFolderName">Root of the search.</param>
        /// <param name="folderView">Mail folder view.</param>
        /// <returns></returns>
        public async Task<FindMailFolderResults> FindFolders(WellKnownFolderName parentFolderName, MailFolderView folderView)
        {
            return await this.FindFolders(
                parentFolderName.ToString(),
                folderView);
        }

        /// <summary>
        /// Find all folders under specified root.
        /// </summary>
        /// <param name="parentFolderId">Parent folder id.</param>
        /// <param name="folderView">Folder view.</param>
        /// <returns></returns>
        public async Task<FindMailFolderResults> FindFolders(string parentFolderId, MailFolderView folderView)
        {
            return await this.FindFolders(
                parentFolderId,
                null,
                folderView);
        }

        /// <summary>
        /// Find all folders under specified root with particular search filter.
        /// </summary>
        /// <param name="parentFolderName">Parent folder name.</param>
        /// <param name="searchFilter">Search filter.</param>
        /// <param name="folderView">Folder view.</param>
        /// <returns></returns>
        public async Task<FindMailFolderResults> FindFolders(WellKnownFolderName parentFolderName, SearchFilter searchFilter, MailFolderView folderView)
        {
            return await this.FindFolders(
                parentFolderName.ToString(), 
                searchFilter, 
                folderView);
        }

        /// <summary>
        /// Find folders under particular folder id.
        /// </summary>
        /// <param name="parentFolderId">Parent folder id.</param>
        /// <param name="searchFilter">Search filter.</param>
        /// <param name="folderView">Folder view.</param>
        /// <returns></returns>
        public async Task<FindMailFolderResults> FindFolders(string parentFolderId, SearchFilter searchFilter, MailFolderView folderView)
        {
            parentFolderId.ThrowIfNullOrEmpty(nameof(parentFolderId));
            folderView.ThrowIfNull(nameof(folderView));

            IUrlQuery urlQuery = this.GetUrlQuery(folderView, searchFilter);
            GraphUri requestUri = new GraphUri(
                this.Identity, 
                new EntityPath(
                    parentFolderId,
                    typeof(MailFolder)),
                this.Beta);

            requestUri.AddSegment(nameof(MailFolder.ChildFolders));
            requestUri.AddQuery(urlQuery);

            HttpResponse httpResponse = await this.ExecuteGetRequest(requestUri);
            PageResponseCollection<MailFolder> folders = this.JsonConverter.Convert<PageResponseCollection<MailFolder>>(httpResponse.Content);

            if (null == folders)
            {
                return null;
            }

            folders.RegisterEntityService(this);
            return new FindMailFolderResults(folders);
        }

        /// <summary>
        /// Sync folder hierarchy.
        /// </summary>
        /// <param name="propertySet">Property set.</param>
        /// <param name="syncState">Sync state.</param>
        /// <returns></returns>
        public async Task<ChangeCollection<MailFolderChange>> SyncFolderHierarchy(MailFolderPropertySet propertySet, string syncState)
        {
            // TEMP TEMP TODO: Remove it
            this.Preferences.Add("odata.maxpagesize=2");
            GraphUri graphUri = this.CreateGraphUri(
                new EntityPath("delta", typeof(MailFolder)),
                    null);

            PageResponseCollection<MailFolder> response = await this.InvokeHttpSyncRequest<MailFolder>(
                graphUri, 
                propertySet, 
                syncState);

            SyncStateQuery<MailFolder> syncCheckpoint = new SyncStateQuery<MailFolder>(response);
            ChangeCollection<MailFolderChange> changeCollection = new ChangeCollection<MailFolderChange>()
            {
                SyncState = syncCheckpoint.Serialize()
            };

            foreach (MailFolder mailFolder in response.Value)
            {
                changeCollection.Items.Add(new MailFolderChange(mailFolder));
            }

            changeCollection.MoreAvailable = response.HasNextLink;
            return changeCollection;
        }

        #endregion

        #region OutlookItem operations

        /// <summary>
        /// Find outlook items.
        /// </summary>
        /// <typeparam name="T">Type of item.</typeparam>
        /// <param name="itemView">Item view.</param>
        /// <returns></returns>
        public async Task<FindItemResults<T>> FindItems<T>(ViewBase itemView) where T : OutlookItem
        {
            return await this.FindItems<T>(
                itemView, 
                null);
        }

        /// <summary>
        /// Find outlook items.
        /// </summary>
        /// <typeparam name="T">Type of item.</typeparam>
        /// <param name="itemView">Item view.</param>
        /// <param name="searchFilter">Search filter.</param>
        /// <returns></returns>
        public async Task<FindItemResults<T>> FindItems<T>(ViewBase itemView, SearchFilter searchFilter) where T : OutlookItem
        {
            return await this.FindItems<T>(
                null, 
                itemView, 
                searchFilter);
        }

        /// <summary>
        /// Find message items.
        /// </summary>
        /// <param name="parentFolderId">Parent folder id.</param>
        /// <param name="itemView">Item view.</param>
        /// <returns></returns>
        public async Task<FindItemResults<Message>> FindItems(WellKnownFolderName parentFolderId, ViewBase itemView)
        {
            return await this.FindItems(
                parentFolderId,
                itemView,
                null);
        }

        /// <summary>
        /// Find message items.
        /// </summary>
        /// <param name="parentFolderId">Parent folder id.</param>
        /// <param name="itemView">Item view.</param>
        /// <returns></returns>
        public async Task<FindItemResults<Message>> FindItems(string parentFolderId, ViewBase itemView)
        {
            return await this.FindItems(
                parentFolderId, 
                itemView, 
                null);
        }

        /// <summary>
        /// Find message items.
        /// </summary>
        /// <param name="parentFolderId">Parent folder id.</param>
        /// <param name="itemView">Item view.</param>
        /// <param name="searchFilter">Search filter.</param>
        /// <returns></returns>
        public async Task<FindItemResults<Message>> FindItems(WellKnownFolderName parentFolderId, ViewBase itemView, SearchFilter searchFilter)
        {
            return await this.FindItems(
                parentFolderId.ToString(),
                itemView,
                searchFilter);
        }

        /// <summary>
        /// Find message items.
        /// </summary>
        /// <param name="parentFolderId">Parent folder id.</param>
        /// <param name="itemView">Item view.</param>
        /// <param name="searchFilter">Search filter</param>
        /// <returns></returns>
        public async Task<FindItemResults<Message>> FindItems(string parentFolderId, ViewBase itemView, SearchFilter searchFilter)
        {
            parentFolderId.ThrowIfNullOrEmpty(nameof(parentFolderId));
            return await this.FindItems<Message>(
                parentFolderId, 
                itemView, 
                searchFilter);
        }

        /// <summary>
        /// Find message rule.
        /// </summary>
        /// <param name="itemView">Item view.</param>
        /// <param name="searchFilter">Search filter.</param>
        /// <returns></returns>
        public async Task<FindItemResults<MessageRule>> FindItems(MessageRuleView itemView, SearchFilter searchFilter)
        {
            return await this.FindItems<MessageRule>(
                null,
                itemView,
                searchFilter);
        }

        /// <summary>
        /// Find outlook category.
        /// </summary>
        /// <param name="itemView">Item view.</param>
        /// <param name="searchFilter">Search filter.</param>
        /// <returns></returns>
        public async Task<FindItemResults<OutlookCategory>> FindItems(OutlookCategoryView itemView, SearchFilter searchFilter)
        {
            return await this.FindItems<OutlookCategory>(
                null,
                itemView,
                searchFilter);
        }

        /// <summary>
        /// Find items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parentFolderId">Parent folder id.</param>
        /// <param name="itemView">Item view.</param>
        /// <param name="searchFilter">Search filter.</param>
        /// <returns></returns>
        private async Task<FindItemResults<T>> FindItems<T>(string parentFolderId, ViewBase itemView, SearchFilter searchFilter) where T : Entity
        {
            itemView.ThrowIfNull(nameof(itemView));
            Type typeOfT = typeof(T);
            if (typeOfT.IsAbstract)
            {
                throw new ArgumentException("Please specify concrete type.");
            }

            itemView.ValidateViewTypeSupported(typeOfT);
            IUrlQuery urlQuery = itemView;
            if (null != searchFilter)
            {
                urlQuery = new CompositeQuery(new IUrlQuery[]
                {
                    urlQuery,
                    searchFilter
                });
            }

            GraphUri graphUri = null;
            if (string.IsNullOrEmpty(parentFolderId))
            {
                graphUri = this.CreateGraphUri(
                    new EntityPath(typeOfT),
                    urlQuery);
            }
            else
            {
                graphUri = this.CreateGraphUri(
                    new EntityPath(parentFolderId, typeof(MailFolder)),
                    urlQuery,
                    new EntityPath(itemView.ViewType));
            }

            HttpResponse httpResponse = await this.ExecuteGetRequest(graphUri);
            PageResponseCollection<T> items = this.JsonConverter.Convert<PageResponseCollection<T>>(httpResponse.Content);
            items.RegisterEntityService(this);
            return new FindItemResults<T>(items);
        }

        #endregion

        #region Sync items
        
        /// <summary>
        /// Sync folder items.
        /// </summary>
        /// <param name="parentFolderId">Parent folder id.</param>
        /// <param name="propertySet">Property set.</param>
        /// <param name="syncState">Sync state.</param>
        /// <returns></returns>
        public async Task<ChangeCollection<MessageChange>> SyncFolderItems(string parentFolderId, MessagePropertySet propertySet, string syncState)
        {
            parentFolderId.ThrowIfNullOrEmpty(nameof(parentFolderId));
            GraphUri graphUri = this.CreateGraphUri(
                new EntityPath(parentFolderId,typeof(MailFolder)),
                null,
                new EntityPath("delta", typeof(Message)));
            
            PageResponseCollection<Message> response = await this.InvokeHttpSyncRequest<Message>(
                graphUri,
                propertySet,
                syncState);

            SyncStateQuery<Message> syncCheckpoint = new SyncStateQuery<Message>(response);
            ChangeCollection<MessageChange> changeCollection = new ChangeCollection<MessageChange>()
            {
                SyncState = syncCheckpoint.Serialize()
            };

            foreach (Message message in response.Value)
            {
                changeCollection.Items.Add(new MessageChange(message));
            }

            changeCollection.MoreAvailable = response.HasNextLink;
            return changeCollection;
        }

        #endregion

        #region User centric methods

        /// <summary>
        /// Get user availability.
        /// </summary>
        /// <param name="schedules">Email address of attendees.</param>
        /// <param name="startTime">Start time.</param>
        /// <param name="endTime">End time.</param>
        /// <param name="availabilityViewInterval">Availability interval.</param>
        /// <returns></returns>
        public async Task<IList<ScheduleInformation>> GetUserAvailability(IList<string> schedules, DateTimeTimeZone startTime, DateTimeTimeZone endTime, int availabilityViewInterval)
        {
            Dictionary<string, object> customRequestPayload = new Dictionary<string, object>();
            customRequestPayload.Add(nameof(schedules), schedules);
            customRequestPayload.Add(nameof(endTime), endTime);
            customRequestPayload.Add(nameof(startTime), startTime);
            customRequestPayload.Add(nameof(availabilityViewInterval), availabilityViewInterval);
            string payload = this.JsonConverter.Convert(customRequestPayload);

            GraphUri graphUri = this.CreateGraphUri(
                new EntityPath("GetSchedule", "Calendar"),
                null);

            HttpResponse httpResponse = await this.ExecutePostRequest(
                graphUri,
                payload);

            IList<ScheduleInformation> response =
                this.JsonConverter.Convert<IList<ScheduleInformation>>(httpResponse.Content);

            return response;
        }

        /// <summary>
        /// Get current user.
        /// </summary>
        /// <returns></returns>
        public async Task<User> GetCurrentUser()
        {
            if (null != this.exchangeUser)
            {
                return this.exchangeUser;
            }

            this.exchangeUser = await this.GetAsync<User>(
                new EntityPath(
                    this.Identity.Id, 
                    typeof(User)));

            return this.exchangeUser;
        }

        /// <summary>
        /// Pull mailbox settings from user mailbox.
        /// </summary>
        /// <returns></returns>
        public async Task<MailboxSettings> GetMailboxSettings()
        {
            GraphUri graphUri = this.CreateGraphUri(
                new EntityPath(nameof(MailboxSettings)),
                null);

            HttpResponse response = await this.ExecuteGetRequest(graphUri);
            return this.JsonConverter.Convert<MailboxSettings>(response.Content);
        }

        /// <summary>
        /// Pull mailbox settings from user mailbox.
        /// </summary>
        /// <returns></returns>
        public async Task<MailboxSettings> UpdateMailboxSettings(MailboxSettings mailboxSettings)
        {
            mailboxSettings.ThrowIfNull(nameof(mailboxSettings));

            GraphUri graphUri = this.CreateGraphUri(
                new EntityPath(nameof(MailboxSettings)),
                null);

            string rawMailboxSettings = this.JsonConverter.Convert(mailboxSettings);
            HttpResponse response = await this.ExecutePatchRequest(
                graphUri,
                rawMailboxSettings);

            return this.JsonConverter.Convert<MailboxSettings>(response.Content);
        }

        #endregion

        #region IEntityService contract

        /// <summary>
        /// Invoke method async.
        /// </summary>
        /// <param name="methodName">Method to invoke.</param>
        /// <param name="entity">Entity against method is invoked.</param>
        /// <param name="additionalParameters">Additional parameters.</param>
        /// <returns></returns>
        public async Task InvokeAsync(string methodName, Entity entity, Dictionary<string, object> additionalParameters)
        {
            GraphUri graphUri = new GraphUri(
                this.Identity,
                entity.EntityPath,
                methodName,
                this.Beta);

            string payload = this.JsonConverter.Convert(
                entity, 
                additionalParameters, 
                false);

            await this.ExecutePostRequest(
                graphUri,
                payload);
        }

        /// <summary>
        /// Invoke method async.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodName">Method to invoke.</param>
        /// <param name="entity">Entity against method is invoked.</param>
        /// <param name="additionalParameters">Additional parameters.</param>
        /// <returns></returns>
        public async Task<T> InvokeAsync<T>(string methodName, Entity entity, Dictionary<string, object> additionalParameters)
        {
            GraphUri graphUri = new GraphUri(
                this.Identity,
                entity.EntityPath,
                methodName,
                this.Beta);

            string payload = this.JsonConverter.Convert(
                entity, 
                additionalParameters,
                false);

            HttpResponse httpResponse = await this.ExecutePostRequest(
                graphUri,
                payload);

            T obj = this.JsonConverter.Convert<T>(httpResponse.Content);
            if (obj is Entity objEntity)
            {
                objEntity.ActionInvoker = this;
                objEntity.EntityService = this;
            }

            return obj;
        }

        /// <summary>
        /// Create folder async.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="destination">Destination.</param>
        /// <returns></returns>
        public async Task<Entity> CreateAsync(Entity entity, Entity destination)
        {
            return await this.CreateAsync(
                entity,
                destination.EntityPath);
        }

        /// <summary>
        /// Create folder async.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="destination">Destination.</param>
        /// <returns></returns>
        public async Task<Entity> CreateAsync(Entity entity, EntityPath destination)
        {
            // TODO: Validation if entity can be created under destination
            // TODO: Investigate if possible for ChildTypeConverter to figure out destination type.
            GraphUri graphUri = this.CreateGraphUri(
                destination,
                null);

            string postPayload = this.JsonConverter.Convert(
                entity,
                null,
                false);

            HttpResponse httpResponse = null;
            if (entity is MailFolder)
            {
                graphUri.AddSegment(nameof(MailFolder.ChildFolders));
            }
            else if (entity is Message)
            {
                graphUri.AddSegment(nameof(MailFolder.Messages));
            }

            httpResponse = await this.ExecutePostRequest(
                graphUri, 
                postPayload);

            return this.Convert(
                httpResponse, 
                entity);
        }

        /// <summary>
        /// Delete entity in async fashion.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task DeleteAsync(Entity entity)
        {
            entity.ThrowIfNull(nameof(entity));
            GraphUri requestUri = new GraphUri(
                this.Identity, 
                entity.EntityPath, 
                this.Beta);

            HttpResponse httpResponse = await this.ExecuteDeleteRequest(requestUri);
        }

        /// <summary>
        /// Update object.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Entity> UpdateAsync(Entity entity)
        {
            entity.ThrowIfNull(nameof(entity));
            string patchPayload = this.JsonConverter.Convert(
                entity, 
                null, 
                false);

            // TODO: Investigate if possible for ChildTypeConverter to figure out destination type.
            GraphUri graphUri = this.CreateGraphUri(
                entity.EntityPath,
                null);

            HttpResponse httpResponse = await this.ExecutePatchRequest(
                graphUri, 
                patchPayload);

            return this.Convert(
                httpResponse, 
                entity);
        }

        /// <summary>
        /// Get object from server.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="entityPath">Entity path to retrieve.</param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(EntityPath entityPath) where T : Entity
        {
            entityPath.ThrowIfNull(nameof(entityPath));
            GraphUri requestUri = new GraphUri(
                this.Identity, 
                entityPath, 
                this.Beta);

            HttpResponse httpResponse = await this.ExecuteGetRequest(requestUri);

            T obj = this.JsonConverter.Convert<T>(httpResponse.Content);
            obj.EntityService = this;
            obj.ActionInvoker = this;

            return obj;
        }

        /// <summary>
        /// Invoke navigation property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityPath"></param>
        /// <param name="pageQuery"></param>
        /// <returns></returns>
        public async Task<FindEntityResults<T>> Navigate<T>(EntityPath entityPath, PageQuery pageQuery) where T : Entity
        {
            entityPath.ThrowIfNull(nameof(entityPath));
            pageQuery.ThrowIfNull(nameof(pageQuery));

            GraphUri graphUri = new GraphUri(
                this.Identity,
                entityPath,
                this.Beta);

            graphUri.AddQuery(pageQuery);

            HttpResponse httpResponse = await this.ExecuteGetRequest(graphUri);
            PageResponseCollection<T> responseCollection = this.JsonConverter.Convert<PageResponseCollection<T>>(httpResponse.Content);
            responseCollection.RegisterEntityService(this);

            return new FindEntityResults<T>(responseCollection);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Execute GET request against specific Graph Uri.
        /// </summary>
        /// <param name="graphUri">Graph Uri.</param>
        /// <returns></returns>
        private async Task<HttpResponse> ExecuteGetRequest(GraphUri graphUri)
        {
            return await this.InvokeHttpRequest(
                HttpRequestType.Get,
                graphUri);
        }

        /// <summary>
        /// Execute post request.
        /// </summary>
        /// <param name="graphUri"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        private async Task<HttpResponse> ExecutePostRequest(GraphUri graphUri, string payload)
        {
            return await this.InvokeHttpRequest(
                HttpRequestType.Post,
                graphUri,
                payload);
        }

        /// <summary>
        /// Execute post request.
        /// </summary>
        /// <param name="graphUri"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        private async Task<HttpResponse> ExecutePatchRequest(GraphUri graphUri, string payload)
        {
            return await this.InvokeHttpRequest(
                HttpRequestType.Patch,
                graphUri,
                payload);
        }

        /// <summary>
        /// Execute delete request.
        /// </summary>
        /// <param name="graphUri">Graph uri.</param>
        /// <returns></returns>
        private async Task<HttpResponse> ExecuteDeleteRequest(GraphUri graphUri)
        {
            return await this.InvokeHttpRequest(
                HttpRequestType.Delete,
                graphUri);
        }

        /// <summary>
        /// Create and invoke http request.
        /// </summary>
        /// <param name="requestType">Request type.</param>
        /// <param name="graphUri">Graph uri.</param>
        /// <param name="payload">Payload.</param>
        /// <returns></returns>
        private async Task<HttpResponse> InvokeHttpRequest(HttpRequestType requestType, GraphUri graphUri, string payload = null)
        {
            HttpRequest httpRequest = null;
            switch (requestType)
            {
                case HttpRequestType.Get:
                    httpRequest = HttpRequest.Get(
                        graphUri,
                        this.HttpRequestContext);
                    break;

                case HttpRequestType.Delete:
                    httpRequest = HttpRequest.Delete(
                        graphUri,
                        this.HttpRequestContext);
                    break;

                case HttpRequestType.Post:
                    httpRequest = HttpRequest.Post(
                        graphUri,
                        payload,
                        this.HttpRequestContext);
                    break;

                case HttpRequestType.Patch:
                    httpRequest = HttpRequest.Patch(
                        graphUri,
                        payload,
                        this.HttpRequestContext);
                    break;

                default:
                    throw new NotImplementedException(requestType.ToString());
            }

            using (httpRequest)
            {
                this.CustomizeHttpRequest(httpRequest);
                HttpResponse httpResponse = await httpRequest.GetHttpResponseAsync();
                if (!httpResponse.Success)
                {
                    RootExceptionObject rootExceptionObject = this.JsonConverter.Convert<RootExceptionObject>(httpResponse.Error);
                    throw new ServiceException(rootExceptionObject);
                }

                return httpResponse;
            }
        }

        /// <summary>
        /// Invoke http sync request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="graphUri">Graph uri.</param>
        /// <param name="propertySet">Property set.</param>
        /// <param name="syncState">Sync state.</param>
        /// <returns></returns>
        private async Task<PageResponseCollection<T>> InvokeHttpSyncRequest<T>(GraphUri graphUri, PropertySet propertySet, string syncState) where T : Entity
        {
            IUrlQuery urlQuery = null;
            if (null != propertySet)
            {
                urlQuery = propertySet;
            }

            if (!string.IsNullOrEmpty(syncState))
            {
                SyncStateQuery<MailFolder> syncStateQuery = SyncStateQuery<MailFolder>.Deserialize(syncState);
                if (null == urlQuery)
                {
                    urlQuery = syncStateQuery;
                }
                else
                {
                    urlQuery = new CompositeQuery(new IUrlQuery[] { urlQuery, syncStateQuery });
                }
            }

            if (null != urlQuery)
            {
                graphUri.AddQuery(urlQuery);
            }

            HttpResponse httpResponse = await this.ExecuteGetRequest(graphUri);
            PageResponseCollection<T> response = this.JsonConverter.Convert<PageResponseCollection<T>>(httpResponse.Content);
            response.RegisterEntityService(this);

            return response;
        }

        /// <summary>
        /// Customize http request.
        /// </summary>
        /// <param name="httpRequest"></param>
        private void CustomizeHttpRequest(HttpRequest httpRequest)
        {
            httpRequest.AdditionalHttpHeaders = this.GetPreferHttpHeader();
            if (!string.IsNullOrEmpty(this.UserAgent))
            {
                httpRequest.UserAgent = this.UserAgent;
            }
        }

        /// <summary>
        /// Get prefer http header.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetPreferHttpHeader()
        {
            if (this.Preferences.Count > 0)
            {
                string preferValue = string.Join(",", this.Preferences);
                Dictionary<string, string> preferHeader = new Dictionary<string, string>();
                preferHeader.Add("Prefer", preferValue);

                return preferHeader;
            }

            return null;
        }

        /// <summary>
        /// Create Url query.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="searchFilter">Search filter.</param>
        /// <returns></returns>
        private IUrlQuery GetUrlQuery(ViewBase view, SearchFilter searchFilter)
        {
            if (null != searchFilter)
            {
                SearchFilter.AndFilterCollection filterCollection = new SearchFilter.AndFilterCollection();
                filterCollection.AddFilter(searchFilter);

                return new CompositeQuery(new IUrlQuery[] { view, filterCollection });
            }

            return view;

        }

        /// <summary>
        /// Create graph uri.
        /// </summary>
        /// <param name="entityPath">Entity path.</param>
        /// <param name="urlQuery">Url query.</param>
        /// <param name="subEntityPath">Sub entity path.</param>
        /// <returns></returns>
        private GraphUri CreateGraphUri(EntityPath entityPath, IUrlQuery urlQuery, EntityPath subEntityPath = null)
        {
            entityPath.ThrowIfNull(nameof(entityPath));
            if (null != subEntityPath)
            {
                entityPath.SubEntity = subEntityPath;
            }

            GraphUri graphUri = new GraphUri(
                this.Identity,
                entityPath,
                this.Beta);

            if (null != urlQuery)
            {
                graphUri.AddQuery(urlQuery);
            }

            return graphUri;
        }

        /// <summary>
        /// Convert http response to correct entity.
        /// </summary>
        /// <param name="httpResponse">Http response.</param>
        /// <param name="entity">Entity.</param>
        /// <returns></returns>
        private Entity Convert(HttpResponse httpResponse, Entity entity)
        {
            if (entity is MailFolder)
            {
                return this.JsonConverter.Convert<MailFolder>(httpResponse.Content);
            }
            else if (entity is Message)
            {
                return this.JsonConverter.Convert<Message>(httpResponse.Content);
            }

            else if (entity is Contact)
            {
                return this.JsonConverter.Convert<Contact>(httpResponse.Content);
            }

            else if (entity is Event)
            {
                return this.JsonConverter.Convert<Event>(httpResponse.Content);
            }

            else if (entity is OutlookTask)
            {
                return this.JsonConverter.Convert<OutlookTask>(httpResponse.Content);
            }

            else if (entity is MessageRule)
            {
                return this.JsonConverter.Convert<MessageRule>(httpResponse.Content);
            }

            else if (entity is OutlookCategory)
            {
                return this.JsonConverter.Convert<OutlookCategory>(httpResponse.Content);
            }

            throw new NotImplementedException($"Create entity '{entity.GetType().Name}' not implemented.");
        }

        #endregion
    }
}
