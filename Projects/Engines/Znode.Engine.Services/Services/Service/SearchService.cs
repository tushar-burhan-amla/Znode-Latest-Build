using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Hangfire;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Admin;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.ElasticSearch;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Observer;
using Znode.Libraries.Resources;
using Znode.Libraries.Search;

using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class SearchService : BaseService, ISearchService
    {
        #region Protected Variables.

        protected readonly IZnodeRepository<ZnodeSearchGlobalProductBoost> _globalProductBoostRepository;
        protected readonly IZnodeRepository<ZnodeSearchGlobalProductCategoryBoost> _globalProductCategoryBoostRepository;
        protected readonly IZnodeRepository<ZnodeSearchDocumentMapping> _documentMappingRepository;
        protected readonly IZnodeRepository<ZnodeCatalogIndex> _catalogIndexRepository;
        protected readonly IZnodeRepository<ZnodeSearchIndexMonitor> _searchIndexMonitorRepository;
        protected readonly IZnodeRepository<ZnodeSearchIndexServerStatu> _searchIndexServerStatusRepository;
        protected readonly IZnodeRepository<ZnodeSearchKeywordsRedirect> _searchKeywordsRedirectRepository;
        protected readonly IPublishProductHelper publishProductHelper;
        protected readonly IZnodeRepository<ZnodeSearchSynonym> _searchSynonymsRepository;
        protected const string FieldType = "field";
        protected const string CategoryType = "category";
        protected const string ProductType = "product";
        protected const int MaxPageSize = 10000;
        protected readonly IZnodeRepository<ZnodePublishStateApplicationTypeMapping> _publishStateMappingRepository;
        protected readonly IZnodeRepository<ZnodePublishState> _publishStateRepository;
        protected readonly IZnodeRepository<ZnodePublishContentPageConfigEntity> _publishCMSConfigentity;
        protected readonly IZnodeRepository<ZnodePublishSeoEntity> _publishSEOEntity;

        protected readonly IDefaultDataService defaultDataService;
        protected readonly IZnodeSearchProvider znodeSearchProvider;
        protected readonly IERPJobs _eRPJob;
        #endregion

        #region Public Variables.

        public static string SKU { get; } = "sku";
        public static string Width { get; } = "width";
        public static string Height { get; } = "height";

        #endregion

        #region Constructor
        public SearchService()
        {
            _globalProductBoostRepository = new ZnodeRepository<ZnodeSearchGlobalProductBoost>();
            _globalProductCategoryBoostRepository = new ZnodeRepository<ZnodeSearchGlobalProductCategoryBoost>();
            _documentMappingRepository = new ZnodeRepository<ZnodeSearchDocumentMapping>();
            _catalogIndexRepository = new ZnodeRepository<ZnodeCatalogIndex>();
            _searchIndexMonitorRepository = new ZnodeRepository<ZnodeSearchIndexMonitor>();
            _searchIndexServerStatusRepository = new ZnodeRepository<ZnodeSearchIndexServerStatu>();
            _searchSynonymsRepository = new ZnodeRepository<ZnodeSearchSynonym>();
            _searchKeywordsRedirectRepository = new ZnodeRepository<ZnodeSearchKeywordsRedirect>();
            publishProductHelper = ZnodeDependencyResolver.GetService<IPublishProductHelper>();
            _publishStateMappingRepository = new ZnodeRepository<ZnodePublishStateApplicationTypeMapping>();
            _publishStateRepository = new ZnodeRepository<ZnodePublishState>();
            _publishSEOEntity = new ZnodeRepository<ZnodePublishSeoEntity>(HelperMethods.Context);
            _publishCMSConfigentity = new ZnodeRepository<ZnodePublishContentPageConfigEntity>(HelperMethods.Context);

            defaultDataService = GetService<IDefaultDataService>();
            znodeSearchProvider = GetService<IZnodeSearchProvider>();
            _eRPJob = GetService<IERPJobs>();
        }
        #endregion

        #region Public Methods

        #region Indexing methods
        //Gets the portal index data.
        public virtual PortalIndexModel GetCatalogIndexData(NameValueCollection expands, FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            PageListModel pageListModel = new PageListModel(filters, null, null);
            ZnodeLogging.LogMessage("WhereClause to get catalog index data: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, pageListModel?.EntityWhereClause.WhereClause);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            return _catalogIndexRepository.GetEntity(pageListModel.EntityWhereClause.WhereClause, GetExpands(expands))?.ToModel<PortalIndexModel, ZnodeCatalogIndex>();
        }

        // To append the timestamp in current index name.
        public virtual string UpdateIndexNameWithTimestamp(string indexName)
            => indexName + DateTime.Now.ToString("MMddyyyyHHmmss");

        //Inserts data for creating index by checking revision type.
        public virtual PortalIndexModel InsertCreateIndexDataByRevisionTypes(PortalIndexModel portalIndexModel)
        {
            portalIndexModel.NewIndexName = UpdateIndexNameWithTimestamp(portalIndexModel.IndexName);
            if (string.IsNullOrEmpty(portalIndexModel.RevisionType) || portalIndexModel.RevisionType.Equals("NONE", StringComparison.OrdinalIgnoreCase))
                portalIndexModel.RevisionType = ZnodePublishStatesEnum.PRODUCTION.ToString();
            if (portalIndexModel.RevisionType == ZnodePublishStatesEnum.PRODUCTION.ToString() && GetIsPreviewEnabled())
            {
                List<string> revisionTypes = new List<string>() { ZnodePublishStatesEnum.PREVIEW.ToString(), ZnodePublishStatesEnum.PRODUCTION.ToString() };

                bool isPreviewProductionEnabled = revisionTypes.Count() > 1 ? true : false;

                if (!defaultDataService.IsIndexExists(portalIndexModel.IndexName))
                    portalIndexModel.NewIndexName = null;

                foreach (string revisionType in revisionTypes)
                {
                    portalIndexModel.RevisionType = revisionType;
                    portalIndexModel.IsPreviewProductionEnabled = isPreviewProductionEnabled;
                    portalIndexModel = InsertCreateIndexData(portalIndexModel);
                }

                return portalIndexModel;
            }

            return InsertCreateIndexData(portalIndexModel);
        }

        //Inserts data for creating index.
        public virtual PortalIndexModel InsertCreateIndexData(PortalIndexModel portalIndexModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            if (IsNull(portalIndexModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorPortalIndexModelNull);

            ZnodeSearchIndexMonitor searchIndexMonitor;

            SearchHelper searchHelper = new SearchHelper();

            int searchIndexServerStatusId = 0;

            string catalogName = portalIndexModel.CatalogName;

            //Allow to insert only if data does not exists.
            if (portalIndexModel.CatalogIndexId < 1)
            {
                //Check if index name is already used by another store.
                string indexName = _catalogIndexRepository.Table.Where(x => x.IndexName == portalIndexModel.IndexName).Select(s => s.IndexName)?.FirstOrDefault() ?? string.Empty;

                //Check if Duplicate IndexName Exist
                 IsDuplicateSearchIndexNameExist(indexName, portalIndexModel);

                portalIndexModel.CatalogName = catalogName;
                string revisionType = portalIndexModel.RevisionType;
                //Save index name in database.
                portalIndexModel = _catalogIndexRepository.Insert(portalIndexModel.ToEntity<ZnodeCatalogIndex>())?.ToModel<PortalIndexModel>();
                portalIndexModel.RevisionType = revisionType;
                catalogName = portalIndexModel.CatalogName;
                //Create index monitor entry.
                searchIndexMonitor = SearchIndexMonitorInsert(portalIndexModel);
                ZnodeLogging.LogMessage("SearchIndexMonitorId: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, searchIndexMonitor?.SearchIndexMonitorId);

                if (portalIndexModel?.CatalogIndexId > 0 && searchIndexMonitor.SearchIndexMonitorId > 0)
                {
                    portalIndexModel.SearchCreateIndexMonitorId = searchIndexMonitor.SearchIndexMonitorId;

                    ZnodeLogging.LogMessage(Admin_Resources.SuccessSearchIndexCreate, ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

                    //Start status for creating index for server name saved.   

                    searchIndexServerStatusId = searchHelper.CreateSearchIndexServerStatus(new SearchIndexServerStatusModel()
                    {
                        SearchIndexMonitorId = searchIndexMonitor.SearchIndexMonitorId,
                        ServerName = Environment.MachineName,
                        Status = ZnodeConstant.SearchIndexStartedStatus
                    }).SearchIndexServerStatusId;

                    CallSearchIndexer(portalIndexModel, searchIndexMonitor.CreatedBy, searchIndexServerStatusId);
                    portalIndexModel.StoreName = catalogName;
                    return portalIndexModel;
                }

                ZnodeLogging.LogMessage(string.Format(Admin_Resources.ErrorCreatingLogForIndexCreationForPortalId, portalIndexModel.PortalId), ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            }
            else
                searchIndexMonitor = CreateSearchIndexMonitorEntry(portalIndexModel);

            //Start status for creating index for server name saved.  
            searchIndexServerStatusId = searchHelper.CreateSearchIndexServerStatus(new SearchIndexServerStatusModel()
            {
                SearchIndexMonitorId = searchIndexMonitor.SearchIndexMonitorId,
                ServerName = Environment.MachineName,
                Status = ZnodeConstant.SearchIndexStartedStatus
            }).SearchIndexServerStatusId;


            portalIndexModel.SearchCreateIndexMonitorId = searchIndexMonitor.SearchIndexMonitorId;
            CallSearchIndexer(portalIndexModel, searchIndexMonitor.CreatedBy, searchIndexServerStatusId);
            ZnodeLogging.LogMessage("PortalIndexModel with PortalIndexId: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, portalIndexModel?.PortalIndexId);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return portalIndexModel;
        }

        //Create search index  
        public virtual void CreateIndex(string indexName, string revisionType, int catalogId, int searchIndexMonitorId, int searchIndexServerStatusId, string newIndexName, bool isPreviewProductionEnabled, bool isPublishDraftProductsOnly)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage(string.Format(Admin_Resources.IndexingStarted, indexName), ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("revisionType, catalogId, searchIndexMonitorId, searchIndexServerStatusId: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { revisionType, catalogId, searchIndexMonitorId, searchIndexServerStatusId });
            try
            {
                long indexstartTime = DateTime.Now.Ticks;

                defaultDataService.IndexingDefaultData(indexName, new SearchParameterModel() { CatalogId = catalogId, IndexStartTime = indexstartTime, SearchIndexMonitorId = searchIndexMonitorId, SearchIndexServerStatusId = searchIndexServerStatusId, revisionType = revisionType, ActiveLocales = GetActiveLocaleList(), IsPreviewEnabled = GetIsPreviewEnabled(), NewIndexName = newIndexName , IsPreviewProductionEnabled = isPreviewProductionEnabled ,IsPublishDraftProductsOnly = isPublishDraftProductsOnly });
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.IndexingStarted, indexName), ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            }
            catch (Exception ex)
            {
                SearchHelper searchHelper = new SearchHelper();
                searchHelper.UpdateSearchIndexServerStatus(new SearchIndexServerStatusModel() { SearchIndexServerStatusId = searchIndexServerStatusId, SearchIndexMonitorId = searchIndexMonitorId, ServerName = Environment.MachineName, Status = ZnodeConstant.SearchIndexFailedStatus });
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.ErrorIndexingForIndex, indexName, ex.Message), ZnodeLogging.Components.Search.ToString(), TraceLevel.Error, ex);
                UpdateSearchProfilePublishState(catalogId);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
        }

        // Updating search profile publish status if index creation fails
        protected virtual void UpdateSearchProfilePublishState(int publishCatalogId)
        {
            ExecuteSpHelper objStoredProc = new ExecuteSpHelper();
            objStoredProc.GetParameter("@PublishCatalogId", publishCatalogId, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@UserId", GetLoginUserId(), ParameterDirection.Input, SqlDbType.Int);

            objStoredProc.GetSPResultInDataSet("Znode_UpdateSearchProfileStateOnIndexCreationFailure");
        }

        //Get list of Create index monitor.
        public virtual SearchIndexMonitorListModel GetSearchIndexMonitorList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            SearchIndexMonitorListModel searchIndexMonitorList;

            sorts = new NameValueCollection();
            sorts.Add(ZnodeSearchIndexMonitorEnum.SearchIndexMonitorId.ToString(), "desc");

            UpdateCatalogIndexFilter(filters);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            IZnodeViewRepository<SearchIndexMonitorModel> objStoredProc = new ZnodeViewRepository<SearchIndexMonitorModel>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            ZnodeLogging.LogMessage("pageListModel to get serverStatusList: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IList<SearchIndexMonitorModel> serverStatusList = objStoredProc.ExecuteStoredProcedureList("Znode_GetCreateIndexServerStatus  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            searchIndexMonitorList = new SearchIndexMonitorListModel { SearchIndexMonitorList = serverStatusList.ToList() };
            searchIndexMonitorList.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("searchIndexMonitorList count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, searchIndexMonitorList?.SearchIndexMonitorList?.Count);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchIndexMonitorList;
        }

        //To update filters with catalog index Id.
        protected virtual void UpdateCatalogIndexFilter(FilterCollection filters)
        {
            if (Equals(filters?.Exists(x => string.Equals(x.Item1, ZnodeSearchIndexMonitorEnum.CatalogIndexId.ToString(), StringComparison.InvariantCultureIgnoreCase)), false))
            {
                int publishCatalogId = GetCatalogIdFromFilters(filters);
                NameValueCollection catalogIndexExpands = new NameValueCollection();

                FilterCollection catalogIndexFilters = new FilterCollection();
                UpdateFilters(catalogIndexFilters, FilterKeys.PublishCatalogId, FilterOperators.Equals, publishCatalogId.ToString());

                filters?.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.PublishCatalogId, StringComparison.InvariantCultureIgnoreCase));

                PortalIndexModel portalIndex = GetCatalogIndexData(catalogIndexExpands, catalogIndexFilters);

                UpdateFilters(filters, ZnodeSearchIndexMonitorEnum.CatalogIndexId.ToString(), FilterOperators.Equals, IsNotNull(portalIndex?.CatalogIndexId) ? portalIndex.CatalogIndexId.ToString() : "0");
            }
        }

        //Get List of Create Index Server status.
        public virtual SearchIndexServerStatusListModel GetSearchIndexServerStatusList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            SearchIndexServerStatusListModel searchIndexServerStatusList;

            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            ZnodeLogging.LogMessage("pageListModel to get searchIndexServerStatusList: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            List<ZnodeSearchIndexServerStatu> searchIndexServerStatusEntityList = _searchIndexServerStatusRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount).ToList();

            searchIndexServerStatusList = new SearchIndexServerStatusListModel { SearchIndexServerStatusList = searchIndexServerStatusEntityList.ToModel<SearchIndexServerStatusModel>().ToList() };
            searchIndexServerStatusList.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("searchIndexServerStatusList count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, searchIndexServerStatusList?.SearchIndexServerStatusList?.Count);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchIndexServerStatusList;
        }

        //Delete unused products from search index.
        public virtual bool DeleteProductData(string indexName, string revisionType, long indexstartTime)
            => defaultDataService.DeleteProductDataByRevisionType(indexName, revisionType, indexstartTime);

        public virtual bool DeleteProduct(string indexName, string znodeProductIds, string revisionType)
            => defaultDataService.DeleteProduct(indexName, znodeProductIds, revisionType);

        public virtual bool DeleteProduct(string indexName, IEnumerable<object> znodeProductIds, string revisionType, string versionId)
            => defaultDataService.DeleteProduct(indexName, znodeProductIds, revisionType, versionId);

        public virtual void CreateProduct(string indexName, List<ZnodePublishProductEntity> productEntities)
             => defaultDataService.CreateDocuments(indexName, productEntities);

        public virtual bool DeleteIndex(int catalogIndexId)
        {
            ZnodeLogging.LogMessage("catalogIndexId to delete index: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, catalogIndexId);
            ZnodeCatalogIndex catalogIndex = _catalogIndexRepository.GetById(catalogIndexId);
            return defaultDataService.DeleteIndex(catalogIndex.IndexName);
        }

        public virtual bool RenameIndex(int catalogIndexId, string oldIndexName, string newIndexName)
        {
            ZnodeLogging.LogMessage("Rename index based on : ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { catalogIndexId, oldIndexName, newIndexName });
            return defaultDataService.RenameIndexData(catalogIndexId, oldIndexName, newIndexName);
        }

        //Delete category/document from given index.
        public virtual bool DeleteCategoryForGivenIndex(string indexName, int categoryId)
            => defaultDataService.DeleteCategoryForGivenIndex(indexName, categoryId);

        //Delete catalog category products/documents from given index.
        public virtual bool DeleteCatalogCategoryProducts(string indexName, int publishCatalogId, List<int> publishCategoryIds, string revisionType, string versionId)
            => defaultDataService.DeleteCatalogCategoryProducts(indexName, publishCatalogId, publishCategoryIds, revisionType, versionId);

        #endregion

        #region Search Boosting
        //Saves product, category and field level boost.
        public virtual bool SaveBoostVales(BoostDataModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            bool saveResult = false;
            ZnodeLogging.LogMessage("BoostDataModel to save boost values: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, model);
            switch (model.BoostType.ToLower())
            {
                case ProductType:
                    saveResult = SaveProductBoostValues(model);
                    break;
                case CategoryType:
                    saveResult = SaveProductCategoryBoostValues(model);
                    break;
                case FieldType:
                    saveResult = SaveFieldsBoostValues(model);
                    break;
            }

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return saveResult;
        }

        //Deletes boost value if it is removed.
        public virtual bool DeleteBoostValue(BoostDataModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            bool isDeleted = false;
            ZnodeLogging.LogMessage("BoostDataModel with Id to delete boost value: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, model?.ID);
            if (IsNotNull(model))
            {
                switch (model?.BoostType?.ToLower())
                {
                    case ProductType:
                        isDeleted = DeleteProductBoostValues(model.ID);
                        break;
                    case CategoryType:
                        isDeleted = DeleteProductCategoryBoostValues(model.ID);
                        break;
                    case FieldType:
                        isDeleted = DeleteFieldsBoostValues(model.ID);
                        break;
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return isDeleted;
        }

        //Gets list of global product boost.
        public virtual SearchGlobalProductBoostListModel GetGlobalProductBoostList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            SearchGlobalProductBoostListModel searchGlobalProductBoostList = null;

            SetLocaleFilterIfNotPresent(ref filters);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            IZnodeViewRepository<SearchGlobalProductBoostModel> objStoredProc = new ZnodeViewRepository<SearchGlobalProductBoostModel>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            ZnodeLogging.LogMessage("pageListModel to get publishProductList: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IList<SearchGlobalProductBoostModel> publishProductList = objStoredProc.ExecuteStoredProcedureList("Znode_GetPublishProductDetail  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            searchGlobalProductBoostList = new SearchGlobalProductBoostListModel { SearchGlobalProductBoostList = publishProductList.ToList() };
            searchGlobalProductBoostList.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("publishProductList count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, publishProductList?.Count);
            SetProductBoostValue(searchGlobalProductBoostList);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchGlobalProductBoostList;
        }

        //Gets list of category level boost.
        public virtual SearchGlobalProductCategoryBoostListModel GetGlobalProductCategoryBoostList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            SearchGlobalProductCategoryBoostListModel searchGlobalProductCategoryBoostList = null;

            SetLocaleFilterIfNotPresent(ref filters);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);


            IZnodeViewRepository<SearchGlobalProductCategoryBoostModel> objStoredProc = new ZnodeViewRepository<SearchGlobalProductCategoryBoostModel>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            ZnodeLogging.LogMessage("pageListModel to get publishProductCategoryList: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IList<SearchGlobalProductCategoryBoostModel> publishProductCategoryList = objStoredProc.ExecuteStoredProcedureList("Znode_GetPublishCategoryDetail  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("publishProductCategoryList count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, publishProductCategoryList?.Count);
            searchGlobalProductCategoryBoostList = new SearchGlobalProductCategoryBoostListModel { SearchGlobalProductCategoryList = publishProductCategoryList.ToList() };
            searchGlobalProductCategoryBoostList.BindPageListModel(pageListModel);

            if (searchGlobalProductCategoryBoostList.SearchGlobalProductCategoryList.Count > 0)
                SetProductCatgoryBoostValue(searchGlobalProductCategoryBoostList);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchGlobalProductCategoryBoostList;
        }

        //Gets field level boost.
        public virtual SearchDocumentMappingListModel GetFieldBoostList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ReplaceFilterKeys(ref filters);

            int catalogId = Convert.ToInt32(filters.Find(x => x.FilterName.ToLower() == FilterKeys.ZnodeCatalogId.ToLower()).FilterValue);

            for (int index = 0; index < sorts.Keys.Count; index++)
                if (Equals(sorts.Keys.Get(index), "propertyname")) { ReplaceSortKeyName(ref sorts, "propertyname", "AttributeCode"); }

            SearchDocumentMappingListModel listModel = new SearchDocumentMappingListModel();

            filters.Add(new FilterTuple(FilterKeys.IsUseInSearch, FilterOperators.Equals, "true"));

            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            List<string> searchableFields = GetSearchableAttributes(pageListModel);

            //Getting searchable fields with boost values.
            searchableFields.ForEach(field => listModel.SearchDocumentMappingList.Add(new SearchDocumentMappingModel() { PropertyName = field }));

            if (searchableFields.Count > 0)
                SetFieldBoostValues(listModel, searchableFields, catalogId);

            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("SearchDocumentMappingList count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, listModel?.SearchDocumentMappingList?.Count);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return listModel;
        }
        #endregion

        #region Elastic search
        //Get search results.
        public virtual KeywordSearchModel FullTextSearch(SearchRequestModel model, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            IAttributeSwatchHelper attributeSwatchHelper = GetService<IAttributeSwatchHelper>();
            string swatchAttributeCode = attributeSwatchHelper.GetAttributeSwatchCodeExpands(expands);
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            var searchProvider = znodeSearchProvider;

            bool isEnableCMSPageSearch = CheckEnableCMSPageSearch(filters);

            //Get catalog current version id.
            int? versionId = GetCatalogVersionId(model.CatalogId, model.LocaleId);

            //Add catalog version id to filters.
            filters.Add(WebStoreEnum.VersionId.ToString().ToLower(), FilterOperators.Equals, Convert.ToString(versionId));

            int publishCatalogId = Convert.ToInt32(filters.FirstOrDefault(m => string.Compare(m.FilterName, WebStoreEnum.ZnodeCatalogId.ToString(), true) == 0)?.Item3);

            bool isAllowIndexing = IsAllowIndexing(publishCatalogId, versionId);

            //Bind the IsGetAllLocationsInventory from filters to expands
            BindAllLocationsFlagInExpands(filters, expands);

            // This check is required to allow backward compatibility, after upgrade if the user does not publish the catalog(after upgrade for all records null
            // will be maintained in the ZnodeParentCategoryIds column and valid values will be filled at the time of publish) and enables the "Enable Inheritance Of Child Products To Parent Category"
            // feature from the manage store screen then the search query should be created based on the category id instead of parent category ids.
            ValidateProductInheritance(model, versionId, publishCatalogId);

            IZnodeSearchRequest searchRequest = GetZnodeSearchRequest(model, filters, sorts, false, isAllowIndexing);

            GetFacetExpands(expands, searchRequest);

            IZnodeSearchResponse searchResponse = null;

            //if it is getting list of facets and category id is also 0 then add product index filter in search filter.
            if (model.CategoryId < 1)
                searchRequest.CatalogIdLocalIdDictionary.Add(ZnodeConstant.ProductIndex, new List<string>() { ZnodeConstant.One });

            string sortName = searchRequest.SortCriteria.FirstOrDefault()?.SortName.ToString()?.ToLower();

            //Method to get search response.
            searchResponse = GetSearchResponse(model, expands, ref sorts, searchProvider, searchRequest, sortName);

            //Log the keyword searched.
            if (!string.IsNullOrEmpty(model.Keyword) && !model.IsFacetList)
                ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.KeywordSearch, model.Keyword.ToLower());

            //If suggestions are fetched when product count is 0.
            GetResultFromSuggestions(model, filters, sorts, searchProvider, ref searchRequest, ref searchResponse);

            //Converts search response to keyword search model.
            KeywordSearchModel searchResult = IsNotNull(searchResponse) ? GetKeywordSearchModel(searchResponse) : new KeywordSearchModel();

            //Execute only if cms page search is enabled.
            if (isEnableCMSPageSearch)
            {
                ICMSPageSearchService cmsPageSearchService = GetService<ICMSPageSearchService>();
                searchResult.TotalCMSPageCount = cmsPageSearchService.GetSearchContentPageCount(model);
            }

            //Map search profile id for search report.
            searchResult.SearchProfileId = searchRequest.SearchProfileId;

            // Get associated categories. 
            searchResult.AssociatedCategoryIds = GetAssociatedCategoryIds(searchRequest, searchResult, model.IsProductInheritanceEnabled);
            //If allow indexing is false then fetch complete details from sql else fetch only inventory details from sql.
            if (!isAllowIndexing)
                BindProductDetails(model, expands, searchResponse, searchResult, versionId);
            else
                BindInventoryAndImageDetails(model, expands, searchResponse, searchResult);
            
            if (IsNotNull(searchResult.Products) && !string.IsNullOrEmpty(swatchAttributeCode))
                attributeSwatchHelper.GetAssociatedConfigurableProducts(searchResult, model, swatchAttributeCode);

            ZnodeLogging.LogMessage($"FullTextQuery:{searchResponse.RequestBody} and productCount {searchResult.Products?.Count}", ZnodeLogging.Components.Search.ToString(),
                TraceLevel.Info);

            ZnodeLogging.LogObject(typeof(KeywordSearchModel), searchResult, "searchResult");
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchResult;
        }

        public virtual void BindProductDetails(SearchRequestModel model, NameValueCollection expands, IZnodeSearchResponse searchResponse, KeywordSearchModel searchResult, int? versionId)
        {
            //Get required input parameters to get the data of products.
            DataTable productDetails = IsNotNull(searchResult.Products) ? GetProductFiltersForSP(searchResponse.ProductDetails) : null;

            GetExpands(model, expands, searchResult, productDetails, versionId);
        }

        //Bind details of inventory to product list.
        public virtual void BindInventoryDetails(SearchRequestModel model, NameValueCollection expands, IZnodeSearchResponse searchResponse, KeywordSearchModel searchResult, bool isSendAllLocations = false)
        {
            try
            {
                //Get required input parameters to get the data of products.
                DataTable tableDetails = IsNotNull(searchResult.Products) ? GetProductFiltersForSP(searchResponse.ProductDetails) : null;
                if (searchResult?.Products?.Count > 0)
                {
                    isSendAllLocations = GetSendLocationsFlag(expands);
                    IList<PublishCategoryProductDetailModel> productDetails = GetProductInventoryDetails(tableDetails, GetLoginUserId(), model.PortalId, isSendAllLocations);

                    searchResult?.Products?.ForEach(product =>
                    {
                        PublishCategoryProductDetailModel productSKU = productDetails?
                                    .FirstOrDefault(productdata => productdata.SKU == product.SKU);

                        if (IsNotNull(productSKU))
                        {
                            product.Quantity = productSKU.Quantity;
                            product.AllLocationQuantity = productSKU.AllLocationQuantity;
                            if (isSendAllLocations)
                                product.Inventory = GetAllLocationsInventoryForProduct(product.SKU, productDetails);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("IsUsedInSearch might be false for ProductId, ProductType, OutOfStockOptions or SKU attribute. : " + ex.Message, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
        }

        //Bind details of inventory and Image to product list.
        protected virtual void BindInventoryAndImageDetails(SearchRequestModel model, NameValueCollection expands, IZnodeSearchResponse searchResponse, KeywordSearchModel searchResult, bool isSendAllLocations = false)
        {
            try
            {
                //Get required input parameters to get the data of products.
                DataTable tableDetails = IsNotNull(searchResult.Products) ? GetProductFiltersForSP(searchResponse.ProductDetails) : null;
                if (searchResult?.Products?.Count > 0)
                {
                    isSendAllLocations = GetSendLocationsFlag(expands);
                    IList<PublishCategoryProductDetailModel> productDetails = GetProductInventoryDetails(tableDetails, GetLoginUserId(), model.PortalId, isSendAllLocations);
                    IImageHelper imageHelper = GetService<IImageHelper>();

                    searchResult?.Products?.ForEach(product =>
                    {
                        PublishCategoryProductDetailModel productSKU = productDetails?
                                    .FirstOrDefault(productdata => productdata.SKU == product.SKU);

                        if (IsNotNull(productSKU))
                        {
                            product.Quantity = productSKU.Quantity;
                            product.AllLocationQuantity = productSKU.AllLocationQuantity;
                            if (isSendAllLocations)
                                product.Inventory = GetAllLocationsInventoryForProduct(product.SKU, productDetails);
                            product.ImageSmallPath = GetProductImagePath(imageHelper, product.Attributes.FirstOrDefault(y => y.AttributeCode == ZnodeConstant.ProductImage)?.AttributeValues);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("IsUsedInSearch might be false for ProductId, ProductType, OutOfStockOptions or SKU attribute. : " + ex.Message, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
        }

        [Obsolete("This method is deprecated and will be discontinued in upcoming versions." +
        "Please use another IsAllowIndexing method which having publishCatalogId & versionId as a parameter to check for indexing.")]
        public bool IsAllowIndexing(int publishCatalogId)
        {
            IZnodeRepository<ZnodePimCatalog> _pimCatalogRepository = new ZnodeRepository<ZnodePimCatalog>();
           
            bool? IsAllowIndexing = (from pimCatalog in _pimCatalogRepository.Table 
                                     where pimCatalog.PimCatalogId == publishCatalogId
                                     select pimCatalog.IsAllowIndexing)?.FirstOrDefault();

            return IsAllowIndexing.GetValueOrDefault();
        }

        //Check whether indexing is allowed or not.
        public virtual bool IsAllowIndexing(int publishCatalogId, int? versionId)
        {
            if (IsNotNull(versionId))
            {

                ZnodePublishCatalogEntity catalogEntity = GetService<IPublishedCatalogDataService>().GetPublishCatalogListById(publishCatalogId, versionId)?.FirstOrDefault(c => c.IsAllowIndexing);
                return IsNotNull(catalogEntity) ? catalogEntity.IsAllowIndexing : false;
            }
            return false;
        }


        //Gets facet search result.
        public virtual KeywordSearchModel FacetSearch(SearchRequestModel model, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            var searchProvider = znodeSearchProvider;

            ValidateCatalogIdAndLocaleId(model);

            IZnodeSearchRequest searchRequest = GetZnodeSearchRequest(model, filters, sorts);

            IZnodeSearchResponse searchResponse = null;

            //if it is getting list of facets and category id is also 0 then add product index filter in search filter.
            if (!model.IsFacetList && model.CategoryId < 1)
                searchRequest.CatalogIdLocalIdDictionary.Add("productindex", new List<string>() { "1" });

            string sortName = searchRequest.SortCriteria.FirstOrDefault()?.SortName.ToString()?.ToLower();

            //Method to get search response.
            searchResponse = GetSearchResponse(model, expands, ref sorts, searchProvider, searchRequest, sortName, true);

            //Log the keyword searched.
            if (!string.IsNullOrEmpty(model.Keyword) && !model.IsFacetList)
                ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.KeywordSearch, model.Keyword.ToLower());

            //If suggestions are fetched when product count is 0.
            GetResultFromSuggestions(model, filters, sorts, searchProvider, ref searchRequest, ref searchResponse);

            //Converts search response to keyword search model.
            KeywordSearchModel searchResult = IsNotNull(searchResponse) ? GetKeywordSearchModel(searchResponse) : new KeywordSearchModel();

            //get expands associated to Product if the call is not for facets.
            if (!model.IsFacetList)
            {
                GetExpands(model, expands, searchResult);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchResult;
        }

        public virtual KeywordSearchModel GetSearchProfileProducts(SearchProfileModel model, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            IZnodeRepository<ZnodeSearchQueryType> _searchQueryType = new ZnodeRepository<ZnodeSearchQueryType>();

            ExecuteSpHelper objStoredProc = new ExecuteSpHelper();
            objStoredProc.GetParameter("@Keyword", model.SearchText, ParameterDirection.Input, SqlDbType.NVarChar);
            objStoredProc.GetParameter("@ProfileId", model.SearchProfileId, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@PublishCatalogId", model.PublishCatalogId, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@PortalId", GetPortalId(), ParameterDirection.Input, SqlDbType.Int);
            DataSet dataSet = objStoredProc.GetSPResultInDataSet("Znode_GetWebStoreSearchProfileTrigger");

            SearchProfileModel searchProfile = MapPublishedDataToSearchProfile(dataSet.Tables[0]);

            if (searchProfile.SearchProfileId != model.SearchProfileId)
                    return new KeywordSearchModel();            


            List<ZnodeSearchQueryType> queryType = _searchQueryType.Table.Where(x => x.SearchQueryTypeId == model.SearchQueryTypeId || x.SearchQueryTypeId == model.SearchSubQueryTypeId)?.ToList();
            ZnodeLogging.LogMessage("queryType list count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, queryType?.Count);
            var searchProvider = znodeSearchProvider;

            IZnodeSearchRequest searchRequest = GetZnodeSearchProfileRequest(model, queryType);
            UpdateSearchRequestFeatureValues(searchRequest);
            IZnodeSearchResponse searchResponse = null;
            searchResponse = searchProvider.FullTextSearch(searchRequest);
            //Converts search response to keyword search model.
            KeywordSearchModel searchResult = IsNotNull(searchResponse) ? GetKeywordSearchModel(searchResponse) : new KeywordSearchModel();

            MediaConfigurationModel configurationModel = new MediaConfigurationService().GetDefaultMediaConfiguration();
            string serverPath = GetMediaServerUrl(configurationModel);

            searchResult.Products?.ForEach(
                  x =>
                  {
                      string imageName = x.Attributes.Where(y => y.AttributeCode == ZnodeConstant.ProductImage)?.FirstOrDefault()?.AttributeValues;
                      x.ImageSmallPath = $"{serverPath}Thumbnail/{imageName}";
                  });

            ZnodeLogging.LogMessage(string.Format(Admin_Resources.FullTextQuery, model.QueryTypeName, searchResponse.RequestBody), ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            return searchResult;
        }

        protected virtual IZnodeSearchRequest GetZnodeSearchProfileRequest(SearchProfileModel model, List<ZnodeSearchQueryType> queryType)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            if (IsNull(model))
                return null;

            var searchRequest = GetService<IZnodeSearchRequest>();
            searchRequest.SearchText = model.SearchText;
            searchRequest.IndexName = GetCatalogIndexName(model.PublishCatalogId);
            searchRequest.CatalogId = model.PublishCatalogId;
            searchRequest.LocaleId = 1;
            searchRequest.PageFrom = 1;
            searchRequest.PageSize = 200;
            // The excludeQueryTypes variable contains the list of search query types which is not supported by Znode from 9.7.1 release.
            List<string> excludedQueryTypes = GetExcludedSearchQueryTypeList();
            // If other than Multi match query type has been configured then by default the Multi Match Cross query type should be considered.
            if (queryType.Where(x => excludedQueryTypes.Contains(x.QueryTypeName.ToLower())).Any())
            {
                searchRequest.QueryTypeName = ZnodeConstant.MultiMatch;
                searchRequest.SubQueryType = ZnodeConstant.MultiMatchCross;
                searchRequest.QueryClass = ZnodeConstant.MultiMatchQueryBuilder;
            }
            else
            {
                searchRequest.QueryTypeName = queryType.FirstOrDefault(x => x.SearchQueryTypeId == model.SearchQueryTypeId)?.QueryTypeName;
                searchRequest.SubQueryType = queryType.FirstOrDefault(x => x.SearchQueryTypeId == model.SearchSubQueryTypeId)?.QueryTypeName;
                searchRequest.QueryClass = queryType.FirstOrDefault(x => x.SearchQueryTypeId == model.SearchQueryTypeId)?.QueryBuilderClassName;
            }
            searchRequest.Operator = model.Operator;
            Dictionary<string, List<string>> filterAndClause = new Dictionary<string, List<string>>();

            //Default filters for search  
            if (model.PublishCatalogId > 0) filterAndClause.Add("znodecatalogid", new List<string>() { model.PublishCatalogId.ToString() });
            filterAndClause.Add("localeid", new List<string>() { "1" });
            filterAndClause.Add("isactive", new List<string>() { "true" });
            filterAndClause.Add("productindex", new List<string>() { "1" });
            filterAndClause.Add("versionid", new List<string>() { Convert.ToString(GetCatalogVersionId(model.PublishCatalogId, ZnodePublishStatesEnum.PRODUCTION)) });

            searchRequest.CatalogIdLocalIdDictionary = filterAndClause;
            searchRequest.SearchableAttribute = GetSearchableAttributeOfProfile(model.SearchableAttributesList);
            searchRequest.FeatureList = MapSearchProfileFeature(model.FeaturesList);
            searchRequest.BoostAndBuryItemLists = new List<ElasticSearchBoostAndBuryItemList>();
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchRequest;
        }

        public virtual List<ElasticSearchAttributes> GetSearchableAttributeOfProfile(List<SearchAttributesModel> searchableAttributesList)
        {
            List<ElasticSearchAttributes> list = new List<ElasticSearchAttributes>();
            if (searchableAttributesList?.Count > 0)
            {
                foreach (var test in searchableAttributesList)
                    list.Add(new ElasticSearchAttributes { AttributeCode = test.AttributeCode, BoostValue = test.BoostValue });
            }
            return list;
        }

        //Gets search suggestions for a keyword.
        public virtual KeywordSearchModel GetKeywordSearchSuggestion(SearchRequestModel model, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            var searchProvider = znodeSearchProvider;

            ZnodeLogging.LogMessage("CatalogId and LocaleId to get version Id: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { model?.CatalogId, model?.LocaleId });
            int? versionId = GetCatalogVersionId(model.CatalogId, model.LocaleId);
            ZnodeLogging.LogMessage("versionId: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, versionId);
            filters.Add(WebStoreEnum.VersionId.ToString().ToLower(), FilterOperators.Equals, Convert.ToString(versionId));

            var searchRequest = GetZnodeSearchRequest(model, filters, sorts, true);
            searchRequest.GetFacets = false;

            IZnodeSearchResponse suggestions = searchProvider.SuggestTermsFor(searchRequest);

            KeywordSearchModel searchResult = IsNotNull(suggestions) ? GetKeywordSearchModel(suggestions) : new KeywordSearchModel();

            if (IsNotNull(searchResult))
            {
                if (!model.IsAutocomplete)
                {
                    if (searchResult.Products?.Count > 0)
                        //get expands associated to Product
                        publishProductHelper.GetDataFromExpands(model.PortalId, GetExpands(expands), searchResult.Products, model.LocaleId, GetLoginUserId(), versionId ?? 0, GetProfileId());
                     
                    ZnodeLogging.LogMessage("portalId and productList count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { model?.PortalId, searchResult?.Products?.Count });
                    GetProductImagePathForSuggestions(model.PortalId, searchResult.Products);
                    //set stored based In Stock, Out Of Stock, Back Order Message.
                    SetPortalBasedDetails(model.PortalId, searchResult.Products);
                }
                else if (!string.IsNullOrWhiteSpace(model.Keyword) && IsNotNull(searchResult.Products))
                {
                    GetProductImagePathForSuggestions(model.PortalId, searchResult.Products);
                }
                //To bind the product SEO urls if isCatalogIndexSettingEnable flag is set to false.
                if (searchResult?.Products?.Count > 0)
                {
                    BindProductSeoUrl(searchResult, model, versionId);
                }
            }
            
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchResult;
        }


        /// <summary>
        /// The method will return the list of search features which should not be displayed on the search profile screen.
        /// </summary>
        /// <returns>List of string of feature list</returns>
        public virtual List<string> GetExcludedSearchFeatureList()
        {
            return new List<string> { ZnodeConstant.DfsQueryThenFetch.ToLower(), ZnodeConstant.MinimumShouldMatch.ToLower() };
        }

        /// <summary>
        /// The method will return the list of all search query types which should not be displayed on the search profile screen.
        /// </summary>
        /// <returns>List of string of all query type list</returns>
        public virtual List<string> GetAllSearchQueryTypeList()
        {
            return new List<string> { ZnodeConstant.Match.ToLower(), ZnodeConstant.MatchPhrase.ToLower(),
                                        ZnodeConstant.MatchPhrasePrefix.ToLower(), ZnodeConstant.MultiMatch.ToLower() };
        }

        /// <summary>
        /// The method will return the list of search query types which should not be displayed on the search profile screen.
        /// </summary>
        /// <returns>List of string of query type list</returns>
        public virtual List<string> GetExcludedSearchQueryTypeList()
        {
            return new List<string> { ZnodeConstant.Match.ToLower(),
                                            ZnodeConstant.MatchPhrase.ToLower(), ZnodeConstant.MatchPhrasePrefix.ToLower() };
        }
        #endregion

        //Get Seo Url details.
        public virtual SEOUrlModel GetSEOUrlDetails(string seoUrl, FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            int portalId;
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.PortalId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out portalId);
            int localeId;
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(ZnodeLocaleEnum.LocaleId.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out localeId);

            if (localeId <= 0)
            {
                IZnodeRepository<ZnodePortalLocale> _portalLocaleRepository = new ZnodeRepository<ZnodePortalLocale>();
                localeId = (_portalLocaleRepository.Table.FirstOrDefault(x => x.PortalId == portalId)?.LocaleId).GetValueOrDefault();
                filters.RemoveAll(x => x.FilterName.Equals(ZnodeLocaleEnum.LocaleId.ToString(), StringComparison.InvariantCultureIgnoreCase));
                filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString());
            }

            //Remove portal id filter.
            filters.RemoveAll(x => x.FilterName == FilterKeys.PortalId);

            // Bind SEO type Name and its SeoId.
            IPublishedPortalDataService publishedPortalDataService = GetService<IPublishedPortalDataService>();
            SEOUrlModel model = publishedPortalDataService.GetSEOEntityDetails(seoUrl, portalId, WebstoreVersionId)?.ToModel<SEOUrlModel>();


            if (IsNull(model))
                model = publishedPortalDataService.GetSEOEntityDetails(seoUrl, portalId)?.ToModel<SEOUrlModel>();



            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return (IsNotNull(model?.SeoCode) && !string.IsNullOrEmpty(model?.Name)) ? GetSEOTypes(filters, model) : new SEOUrlModel();
        }

        //Gets expands for products.
        public virtual void GetExpands(SearchRequestModel model, NameValueCollection expands, KeywordSearchModel searchResult, DataTable tableDetails = null, int? versionId = null)
        {
            if (searchResult?.Products?.Count > 0)
            {
                if (HelperUtility.IsNull(versionId))
                {
                    versionId = GetCatalogVersionId(model.CatalogId, model.LocaleId);
                }
                publishProductHelper.GetDataFromExpands(model.PortalId, GetExpands(expands), searchResult.Products, model.LocaleId, GetLoginUserId(), versionId ?? 0, GetProfileId());
                GetRequiredProductDetails(searchResult, tableDetails, GetLoginUserId(), model.PortalId, GetSendLocationsFlag(expands));
            }
        }

        //Get expands and add them to navigation properties
        public virtual List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (IsNotNull(expands) && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                {
                    //check if expand key is present or not and add it to navigation properties.
                    if (Equals(key, ZnodeConstant.Promotions)) SetExpands(ZnodeConstant.Promotions, navigationProperties);
                    if (Equals(key, ZnodeConstant.Inventory)) SetExpands(ZnodeConstant.Inventory, navigationProperties);
                    if (Equals(key, ZnodeConstant.ProductTemplate)) SetExpands(ZnodeConstant.ProductTemplate, navigationProperties);
                    if (Equals(key, ZnodeConstant.ProductReviews)) SetExpands(ZnodeConstant.ProductReviews, navigationProperties);
                    if (Equals(key, ZnodeConstant.Pricing)) SetExpands(ZnodeConstant.Pricing, navigationProperties);
                    if (Equals(key, ZnodeConstant.SEO)) SetExpands(ZnodeConstant.SEO, navigationProperties);
                    if (Equals(key, ZnodeConstant.AddOns)) SetExpands(ZnodeConstant.AddOns, navigationProperties);
                    if (Equals(key, ZnodeConstant.ConfigurableAttribute)) SetExpands(ZnodeConstant.ConfigurableAttribute, navigationProperties);
                    if (string.Equals(key, ZnodePortalUnitEnum.ZnodePortal.ToString(), StringComparison.CurrentCultureIgnoreCase)) SetExpands(ZnodePortalUnitEnum.ZnodePortal.ToString(), navigationProperties);
                    if (string.Equals(key, ZnodeCatalogIndexEnum.ZnodePublishCatalog.ToString(), StringComparison.CurrentCultureIgnoreCase)) SetExpands(ZnodeCatalogIndexEnum.ZnodePublishCatalog.ToString(), navigationProperties);
                    if (Equals(key, ZnodeConstant.AssociatedProducts)) SetExpands(ZnodeConstant.AssociatedProducts, navigationProperties);

                }
            }
            return navigationProperties;
        }

        //Converts a strings first character to lower-case.
        public virtual string FirstCharToLower(string input)
        {
            if (!string.IsNullOrEmpty(input))
                input = input.First().ToString().ToLower() + input.Substring(1);
            return input;
        }

        //Get all Attributes Codes where IsSearchable Flag is true 
        protected virtual List<string> GetSearchableAttributes(PageListModel pageListModel)
        {
            List<string> catalogAttributes = GetService<IPublishedCatalogDataService>().GetPublishCatalogAttributePagedList(pageListModel)?.Select(x => x.AttributeCode)?.Distinct()?.ToList();

            if (IsNotNull(catalogAttributes))
                pageListModel.TotalRowCount = catalogAttributes.Count;
            ZnodeLogging.LogMessage("catalogAttributes list: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, catalogAttributes);
            return catalogAttributes;
        }

        #region Synonyms
        //Create synonyms for search.
        public virtual SearchSynonymsModel CreateSearchSynonyms(SearchSynonymsModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            //Check whether the model is null or not.
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorSearchSynonymsModelNull);

            //Save data into synonyms table and maps updated values in model.
            ZnodeSearchSynonym entity = _searchSynonymsRepository.Insert(model.ToEntity<ZnodeSearchSynonym>());

            if (IsNotNull(entity))
            {
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessSynonymsWithIdCreate, model.SearchSynonymsId), ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                return entity.ToModel<SearchSynonymsModel>();
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage(Admin_Resources.ErrorSynonymsCreate, ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return model;
        }

        //Get synonyms id for search.
        public virtual SearchSynonymsModel GetSearchSynonyms(int searchSynonymsId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            if (searchSynonymsId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorSearchSynonymsIdGreaterThanOne);

            //Get search synonyms entity data & maps it into search synonyms model.
            SearchSynonymsModel searchSynonymsModel = _searchSynonymsRepository.GetById(searchSynonymsId)?.ToModel<SearchSynonymsModel>();

            ZnodeLogging.LogMessage("searchSynonymsModel: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, searchSynonymsModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchSynonymsModel;
        }

        //Update search synonyms data for search.
        public virtual bool UpdateSearchSynonyms(SearchSynonymsModel searchSynonymsModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            if (IsNull(searchSynonymsModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorSearchSynonymsModelNull);

            if (searchSynonymsModel.SearchSynonymsId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            ZnodeLogging.LogMessage("searchSynonymsModel to be updated: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, searchSynonymsModel);
            bool isUpdated = _searchSynonymsRepository.Update(searchSynonymsModel.ToEntity<ZnodeSearchSynonym>());

            //Returns true if data updated successfully.
            if (isUpdated)
            {
                ZnodeLogging.LogMessage(Admin_Resources.SuccessSearchSynonymsDataUpdate, ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                return true;
            }
            ZnodeLogging.LogMessage(Admin_Resources.ErrorSearchSynonymsDataUpdate, ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return false;
        }

        //Get list of synonyms for search.
        public virtual SearchSynonymsListModel GetSearchSynonymsList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            ZnodeLogging.LogMessage("pageListModel to get searchSynonymsEntityList: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            List<ZnodeSearchSynonym> searchSynonymsEntityList = _searchSynonymsRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.ToList();
            ZnodeLogging.LogMessage("searchSynonymsEntityList count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, searchSynonymsEntityList?.Count);

            SearchSynonymsListModel searchSynonymsList = searchSynonymsEntityList?.Count > 0 ? new SearchSynonymsListModel { SynonymsList = searchSynonymsEntityList.ToModel<SearchSynonymsModel>()?.ToList() } : new SearchSynonymsListModel { SynonymsList = new List<SearchSynonymsModel>() };
            searchSynonymsList.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchSynonymsList;
        }

        //Delete synonyms by id.
        public virtual bool DeleteSearchSynonyms(ParameterModel searchSynonymsIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            //Check synonyms ids.
            if (string.IsNullOrEmpty(searchSynonymsIds?.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorSynonymsIdLessThanOne);

            //Generates filter clause for multiple synonyms ids.
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeSearchSynonymEnum.SearchSynonymsId.ToString(), ProcedureFilterOperators.In, searchSynonymsIds.Ids));

            //Returns true if synonyms deleted successfully else return false.
            ZnodeLogging.LogMessage("searchSynonymsIds to be deleted: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, searchSynonymsIds?.Ids);
            bool isDeleted = _searchSynonymsRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
            if (isDeleted)
            {
                //Returns true if it clears synonyms form index when last synonym is deleted else return false.
                ClearSearchSynonyms(searchSynonymsIds);
            }
            ZnodeLogging.LogMessage(isDeleted ? Admin_Resources.SuccessSynonymsDelete : Admin_Resources.ErrorSynonymsDelete, ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return isDeleted;
        }
        // Check Synonym code already exist or not 
        public virtual bool IsCodeExists(HelperParameterModel parameterModel)
        {
            if (!string.IsNullOrEmpty(parameterModel.CodeField))
            {
                ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                return _searchSynonymsRepository.Table.Any(a => a.SynonymCode.Equals(parameterModel.CodeField,StringComparison.InvariantCultureIgnoreCase));
            }
            return true;
        }

        //Write synonyms.txt for search.
        public virtual bool WriteSearchFile(int publishCatalogId, bool isSynonymsFile, bool isAllDeleted = false)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            if (publishCatalogId > 0)
            {
                PortalIndexModel portalIndexModel = GetPortalIndexModelForIndexCreation(publishCatalogId);

                if (IsNull(portalIndexModel) || string.IsNullOrEmpty(portalIndexModel.IndexName))
                    throw new ZnodeException(ErrorCodes.NotFound, string.Format(Admin_Resources.ErrorIndexExistForCatalogId, publishCatalogId));

                ZnodeLogging.LogMessage("publishCatalogId, indexName, isSynonymsFile to write synonyms file: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { isSynonymsFile, portalIndexModel.IndexName, isSynonymsFile });
                bool isSynonymsUpdatedSuccessfully = defaultDataService.WriteSynonymsFile(publishCatalogId, portalIndexModel.IndexName, isSynonymsFile, isAllDeleted);

                // If synonyms are successfully updated in the index schema index creation will be initiated.
                if (isSynonymsUpdatedSuccessfully)
                    InsertCreateIndexDataByRevisionTypes(portalIndexModel);

                return isSynonymsUpdatedSuccessfully && IsNotNull(portalIndexModel) && portalIndexModel.SearchCreateIndexMonitorId > 0;
            }
            return false;
        }

        // To get the instance of PortalIndexModel which is required to initiate the index creation process.
        protected virtual PortalIndexModel GetPortalIndexModelForIndexCreation(int publishCatalogId)
        {
            // Avoided use of automapper as only specific properties are required.
            PortalIndexModel portalIndexModel = _catalogIndexRepository.Table.Where(x => x.PublishCatalogId == publishCatalogId)?.
                Select(x=> new PortalIndexModel()
                {
                    CatalogIndexId = x.CatalogIndexId,
                    IndexName = x.IndexName,
                    PublishCatalogId = x.PublishCatalogId
                })?.FirstOrDefault();

            if (IsNotNull(portalIndexModel))
            {
                // If DirectCalling property is set to true index creation will be initiated via scheduler/hangfire.
                // On click of the CreateIndex button which is available on the "Manage PIM Indexes" screen, value for DirectCalling property is getting passed as true.
                portalIndexModel.DirectCalling = true;

                // For synonyms publish, PRODUCTION revision type will be consideed by default.
                // If preview mode is also enabled then in the InsertCreateIndexDataByRevisionTypes method both PREVIEW and PRODUCTION revision types will be considered for index creation.
                portalIndexModel.RevisionType = ZnodePublishStatesEnum.PRODUCTION.ToString();
            }

            return portalIndexModel;
        }

        public virtual bool IndexCreationAfterSearchProfilePublish(int searchProfileId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            IZnodeRepository<ZnodePublishSearchProfileEntity> _publishSearchProfileEntity = new ZnodeRepository<ZnodePublishSearchProfileEntity>(HelperMethods.Context);

            int catalogId = (int) _publishSearchProfileEntity.Table.Where(x => x.SearchProfileId == searchProfileId).Select(x=>x.ZnodeCatalogId).FirstOrDefault();

            PortalIndexModel portalIndexModel = GetPortalIndexModelForIndexCreation(catalogId);
            InsertCreateIndexDataByRevisionTypes(portalIndexModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            if (IsNotNull(portalIndexModel) && portalIndexModel.SearchCreateIndexMonitorId > 0)
                return true;
            else
                return false;
        }
        #endregion

        #region Search Keywords Redirect
        //Get catalog keywords redirect list.
        public virtual SearchKeywordsRedirectListModel GetCatalogKeywordsRedirectList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            ZnodeLogging.LogMessage("pageListModel to keywordsEntityList: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            List<ZnodeSearchKeywordsRedirect> keywordsEntityList = _searchKeywordsRedirectRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.ToList();
            ZnodeLogging.LogMessage("keywordsEntityList count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, keywordsEntityList?.Count);

            SearchKeywordsRedirectListModel searchKeywordsRedirectList = keywordsEntityList?.Count > 0 ? new SearchKeywordsRedirectListModel { KeywordsList = keywordsEntityList.ToModel<SearchKeywordsRedirectModel>()?.ToList() } : new SearchKeywordsRedirectListModel { KeywordsList = new List<SearchKeywordsRedirectModel>() };

            searchKeywordsRedirectList.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchKeywordsRedirectList;
        }

        // Creates keywords and its redirected url for search.
        public virtual SearchKeywordsRedirectModel CreateSearchKeywordsRedirect(SearchKeywordsRedirectModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            //Check whether the model is null or not.
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorSearchKeywordModelNull);

            if (IsNull(model.PublishCatalogId) || model.PublishCatalogId == 0)
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PublishCatalogIdLessThanOne);

            //Save data into synonyms table and maps updated values in model.
            ZnodeSearchKeywordsRedirect entity = _searchKeywordsRedirectRepository.Insert(model.ToEntity<ZnodeSearchKeywordsRedirect>());

            if (IsNotNull(entity))
            {
                var clearCacheInitializer = new ZnodeEventNotifier<SearchKeywordsRedirectModel>(model);
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessKeywordsWithIdCreate, model.SearchKeywordsRedirectId), ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                return entity.ToModel<SearchKeywordsRedirectModel>();
            }
            ZnodeLogging.LogMessage(Admin_Resources.ErrorKeywordsCreate, ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return model;
        }

        //Get keywords details for search.
        public virtual SearchKeywordsRedirectModel GetSearchKeywordsRedirect(int searchKeywordsRedirectId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            if (searchKeywordsRedirectId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorSearchKeywordsIdGreaterThanOne);

            //Get search keywords entity data & maps it into search keywords model.
            ZnodeLogging.LogMessage("searchKeywordsRedirectId to get searchKeywordsRedirectModel: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, searchKeywordsRedirectId);
            SearchKeywordsRedirectModel searchKeywordsRedirectModel = _searchKeywordsRedirectRepository.GetById(searchKeywordsRedirectId)?.ToModel<SearchKeywordsRedirectModel>();

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchKeywordsRedirectModel;
        }

        //Update keywords for search.
        public virtual bool UpdateSearchKeywordsRedirect(SearchKeywordsRedirectModel searchKeywordsModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            if (IsNull(searchKeywordsModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorSearchKeywordModelNull);

            if (searchKeywordsModel.SearchKeywordsRedirectId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            ZnodeLogging.LogMessage("searchKeywordsModel with Id to be updated: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, searchKeywordsModel?.SearchKeywordsRedirectId);
            bool isUpdated = _searchKeywordsRedirectRepository.Update(searchKeywordsModel.ToEntity<ZnodeSearchKeywordsRedirect>());

            //Returns true if data updated successfully.
            if (isUpdated)
            {
                var clearCacheInitializer = new ZnodeEventNotifier<SearchKeywordsRedirectModel>(searchKeywordsModel);
                ZnodeLogging.LogMessage(Admin_Resources.SuccessSearchKeywordsDataUpdate, ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                return true;
            }
            ZnodeLogging.LogMessage(Admin_Resources.ErrorSearchKeywordsDataUpdate, ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return false;
        }

        //Delete keywords by ids.
        public virtual bool DeleteSearchKeywordsRedirect(ParameterModel searchKeywordsRedirectIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            //Check keywords ids.
            if (string.IsNullOrEmpty(searchKeywordsRedirectIds?.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorKeywordsIdLessThanOne);

            //Generates filter clause for multiple keywords ids.
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeSearchKeywordsRedirectEnum.SearchKeywordsRedirectId.ToString(), ProcedureFilterOperators.In, searchKeywordsRedirectIds.Ids));

            //Returns true if keywords deleted successfully else return false.
            ZnodeLogging.LogMessage("searchKeywordsRedirectIds to be deleted: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, searchKeywordsRedirectIds?.Ids);
            bool isDeleted = _searchKeywordsRedirectRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
            
            if(isDeleted)
            {
                var clearCacheInitializer = new ZnodeEventNotifier<SearchKeywordsRedirectModel>(new SearchKeywordsRedirectModel());
                ZnodeLogging.LogMessage(Admin_Resources.SuccessKeywordsDelete, ZnodeLogging.Components.Search.ToString());
            }
            else
                ZnodeLogging.LogMessage(Admin_Resources.ErrorKeywordsDelete, ZnodeLogging.Components.Search.ToString());
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return isDeleted;
        }

        //Clears synonyms form index when last synonym is deleted.
        public virtual bool ClearSearchSynonyms(ParameterModel searchSynonymsIds)
        {
            bool isSynonymText = false;
            if (searchSynonymsIds.publishCataLogId > 0)
            {
                List<ZnodeSearchSynonym> searchSynonymsEntityList = _searchSynonymsRepository.Table.Where(x => x.PublishCatalogId == searchSynonymsIds.publishCataLogId).ToList();
                if (searchSynonymsEntityList?.Count == 0)
                {
                    isSynonymText = WriteSearchFile(searchSynonymsIds.publishCataLogId, true, true);
                }
            }
            return isSynonymText;
        }

        //Check if cms search is enable or not.
        protected virtual bool CheckEnableCMSPageSearch(FilterCollection filters)
        {
            bool isEnableCMSPageSearch;

            isEnableCMSPageSearch = HelperUtility.TryParseBoolean(filters?.FirstOrDefault(x => string.Equals(x.FilterName,StoreFeature.Enable_CMS_Page_Results.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue);
            filters?.RemoveAll(x => x.FilterName.Equals(StoreFeature.Enable_CMS_Page_Results.ToString(), StringComparison.InvariantCultureIgnoreCase));

            return isEnableCMSPageSearch;
        }

        #endregion
        #endregion

        #region Protected Methods
        //Get required input parameters to get the data of products.
        public virtual DataTable GetProductFiltersForSP(List<dynamic> products)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("products list count to get product filters for SP: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, products?.Count);
            DataTable table = new DataTable("ProductTable");
            try
            {           
                DataColumn productId = new DataColumn("Id") { DataType = typeof(int), AllowDBNull = false };
                table.Columns.Add(productId);
                table.Columns.Add("ProductType", typeof(string));
                table.Columns.Add("OutOfStockOptions", typeof(string));
                table.Columns.Add("SKU", typeof(string));

                foreach (var item in products)
                    table.Rows.Add(Convert.ToInt32(item["znodeproductid"]), Convert.ToString(item["producttype"]), Convert.ToString(item["outofstockoptions"]), Convert.ToString(item["sku"]));

                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage("IsUsedInSearch might be false for ProductId, ProductType, OutOfStockOptions or SKU attribute. : " + ex.Message, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
            return table;
        }

        //Get details of category products.
        protected virtual void GetRequiredProductDetails(KeywordSearchModel searchResult, DataTable tableDetails, int userId = 0, int portalId = 0, bool isSendAllLocations = false)
        {
            IZnodeViewRepository<PublishCategoryProductDetailModel> objStoredProc = new ZnodeViewRepository<PublishCategoryProductDetailModel>();
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@UserId", userId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@currentUtcDate", HelperUtility.GetDateTime().Date, ParameterDirection.Input, DbType.String);
            IList<PublishCategoryProductDetailModel> productDetails = null;

            if (DefaultGlobalConfigSettingHelper.IsColumnEncryptionSettingEnabled)
            {
                objStoredProc.SetParameter("@ProductDetailsFromWebStore", tableDetails?.ToJson(), ParameterDirection.Input, DbType.String);
                //Gets the entity list according to where clause, order by clause and pagination
                productDetails = objStoredProc.ExecuteStoredProcedureList("Znode_GetProductInfoForWebStoreWithJSON @PortalId,@LocaleId,@UserId,@ProductDetailsFromWebStore,@currentUtcDate");
            }
            else
            {
                objStoredProc.SetTableValueParameter("@ProductDetailsFromWebStore", tableDetails, ParameterDirection.Input, SqlDbType.Structured, "dbo.ProductDetailsFromWebStore");
                objStoredProc.SetParameter("@IsAllLocation", isSendAllLocations, ParameterDirection.Input, DbType.Boolean);
                //Gets the entity list according to where clause, order by clause and pagination
                productDetails = objStoredProc.ExecuteStoredProcedureList("Znode_GetProductInfoForWebStore @PortalId,@LocaleId,@UserId,@currentUtcDate,@ProductDetailsFromWebStore,@IsAllLocation");
            }
            //Bind product details.
            BindProductDetails(searchResult, portalId, productDetails, isSendAllLocations);
        }

        //Get details of inventory to product list.
        protected virtual IList<PublishCategoryProductDetailModel> GetProductInventoryDetails(DataTable tableDetails, int userId = 0, int portalId = 0, bool isSendAllLocations = false)
        {
            IZnodeViewRepository<PublishCategoryProductDetailModel> objStoredProc = new ZnodeViewRepository<PublishCategoryProductDetailModel>();
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@UserId", userId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@currentUtcDate", HelperUtility.GetDateTime().Date, ParameterDirection.Input, DbType.String);
            objStoredProc.SetTableValueParameter("@ProductDetailsFromWebStore", tableDetails, ParameterDirection.Input, SqlDbType.Structured, "dbo.ProductDetailsFromWebStore");
            objStoredProc.SetParameter("@IsAllLocation", isSendAllLocations, ParameterDirection.Input, DbType.Boolean);
            //Gets the entity list according to where clause, order by clause and pagination
            return objStoredProc.ExecuteStoredProcedureList("Znode_GetProductWarehouseDetailInfoForWebStoreIndexing @PortalId,@LocaleId,@UserId,@currentUtcDate,@ProductDetailsFromWebStore,@IsAllLocation");
        }

        //Bind product details.
        public virtual void BindProductDetails(KeywordSearchModel searchResult, int portalId, IList<PublishCategoryProductDetailModel> productDetails, bool isSendAllLocations = false)
        {
            IImageHelper imageHelper = GetService<IImageHelper>();

            searchResult?.Products?.ForEach(product =>
            {
                PublishCategoryProductDetailModel productSKU = productDetails?
                            .FirstOrDefault(productdata => productdata.SKU == product.SKU);

                if (IsNotNull(productSKU))
                {
                    product.CurrencyCode = productSKU.CurrencyCode;
                    product.CultureCode = productSKU.CultureCode;
                    product.CurrencySuffix = productSKU.CurrencySuffix;
                    product.Quantity = productSKU.Quantity;
                    product.ReOrderLevel = productSKU.ReOrderLevel;
                    product.Rating = productSKU.Rating;
                    product.TotalReviews = productSKU.TotalReviews;
                    product.ImageSmallPath = GetProductImagePath(imageHelper, product.Attributes.FirstOrDefault(y => y.AttributeCode == ZnodeConstant.ProductImage)?.AttributeValues);
                    product.AllLocationQuantity = productSKU.AllLocationQuantity;
                    if (isSendAllLocations) product.Inventory = GetAllLocationsInventoryForProduct(product.SKU, productDetails);
                }
            });
        }

        //Get the Product Image Path.
        protected virtual string GetProductImagePath(IImageHelper imageHelper, string productImageName)
        {
            if ( IsNotNull(imageHelper) && !string.IsNullOrEmpty(productImageName))
            {
                return imageHelper.GetImageHttpPathSmall(productImageName);
            }
            return string.Empty;
        }

        //Get the type of SEO whether Product, Category or Content Page.
        protected virtual SEOUrlModel GetSEOTypes(FilterCollection filters, SEOUrlModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            //Get the type of SEO whether Product, Category or Content Page.
            switch (model.Name)
            {
                case ZnodeConstant.Product:
                    ProductSeoType(filters, model);
                    break;
                case ZnodeConstant.Category:
                    CategorySeoType(filters, model);
                    break;
                case ZnodeConstant.ContentPage:
                    ContentPageSeoType(model);
                    break;
                case ZnodeConstant.Brand:
                    BrandSeoType(model);
                    break;
                case ZnodeConstant.BlogNews:
                    BlogNewsSeoType(model);
                    break;
                default:
                    break;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return model;
        }

        protected virtual void BrandSeoType(SEOUrlModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            IZnodeRepository<ZnodeBrandDetail> _brandRepository = new ZnodeRepository<ZnodeBrandDetail>();
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeBrandDetailEnum.BrandCode.ToString(), FilterOperators.Is, model.SeoCode.ToString()));

            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            ZnodeLogging.LogMessage("WhereClause to get brand details: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, whereClauseModel?.WhereClause);
            ZnodeBrandDetail brand = _brandRepository.GetEntity(whereClauseModel.WhereClause, whereClauseModel.FilterValues);

            if (IsNotNull(brand))
            {
                model.BrandId = brand.BrandId;
                model.BrandName = brand.BrandCode;
                model.IsActive = brand.IsActive;
                model.SEOId = brand.BrandId;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
        }

        //Get BlogNews Id if Seo belongs to BlogNews.
        protected virtual void BlogNewsSeoType(SEOUrlModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            IZnodeRepository<ZnodeBlogNew> _blogNewsRepository = new ZnodeRepository<ZnodeBlogNew>();
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeBlogNewEnum.BlogNewsCode.ToString(), FilterOperators.Is, model.SeoCode));
            var whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("whereClause to get blog news: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, whereClause?.WhereClause);
            ZnodeBlogNew blogNews = _blogNewsRepository.GetEntity(whereClause.WhereClause, whereClause.FilterValues);
            if (IsNotNull(blogNews))
            {
                model.BrandId = blogNews.BlogNewsId;
                model.IsActive = blogNews.IsBlogNewsActive.Value;
                model.SEOId = blogNews.BlogNewsId;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
        }

        //Get ContentPage Id and ContentPage Name if Seo belongs to ContentPage.
        protected virtual void ContentPageSeoType(SEOUrlModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            ZnodePublishContentPageConfigEntity content = _publishCMSConfigentity.Table?.Where(x => x.PageName == model.SeoCode && x.PortalId == PortalId && x.VersionId == WebstoreVersionId).OrderByDescending(q => q.ContentPageId).FirstOrDefault(x => x.PageName == model.SeoCode);

            if (IsNotNull(content))
            {
                model.ContentPageId = content.ContentPageId;
                model.ContentPageName = content.PageName;
                if (content.IsActive && (content.ActivationDate == null || content.ActivationDate.GetValueOrDefault().Date <= GetDate())
                  && (content.ExpirationDate == null || content.ExpirationDate.GetValueOrDefault().Date >= GetDate()))
                {
                    model.IsActive = content.IsActive;
                }
                else
                {
                    model.IsActive = false;
                }
                model.SEOId = content.ContentPageId;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
        }

        //Get Category Id and Category Name if Seo belongs to Category.
        protected virtual void CategorySeoType(FilterCollection filters, SEOUrlModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            string localeId = filters.Find(x => x.FilterName.ToLower() == ZnodeLocaleEnum.LocaleId.ToString().ToLower())?.FilterValue;
            string catalogId = filters.Find(x => x.FilterName.ToLower() == ZnodePimCatalogEnum.PimCatalogId.ToString().ToLower())?.FilterValue;

            ZnodeLogging.LogMessage("localeId and catalogId to create query: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { localeId, catalogId });

            filters.Add(new FilterTuple(FilterKeys.ZnodeCategoryId, FilterOperators.Equals, model.SEOId.ToString()));
            ReplaceFilterKeys(ref filters);

            if (IsNotNull(localeId) && IsNotNull(catalogId))
            {
                ZnodePublishCategoryEntity category = GetService<IPublishedCategoryDataService>().GetCategoryListByCatalogId(int.Parse(catalogId), int.Parse(localeId))?.FirstOrDefault(x => x.CategoryCode == model.SeoCode && x.VersionId == GetCatalogVersionId(int.Parse(catalogId), int.Parse(localeId)));

                if (IsNotNull(category))
                {
                    model.CategoryId = category.ZnodeCategoryId;
                    model.CategoryName = category.Name;

                    //Make category De-Active,if category is De-Active./ActivationDate is greater than current date./ExpirationDate is less than current date.
                    if (category.IsActive &&
                    (category.ActivationDate == null || category.ActivationDate.GetValueOrDefault().Date <= HelperUtility.GetDate())
                      && (category.ExpirationDate == null || category.ExpirationDate.GetValueOrDefault().Date >= HelperUtility.GetDate()))
                    {
                        model.IsActive = category.IsActive;
                    }
                    else
                        model.IsActive = false;

                    model.SEOId = category.ZnodeCategoryId;
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
        }

        //Replaces filter key.
        protected virtual void ReplaceFilterKeys(ref FilterCollection filters)
        {
            foreach (FilterTuple tuple in filters)
            {
                if (string.Equals(tuple.Item1, FilterKeys.LocaleId, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, FilterKeys.LocaleId, FilterKeys.PublishedLocaleId); }
                if (string.Equals(tuple.Item1, FilterKeys.ZnodeCatalogId, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, FilterKeys.ZnodeCatalogId.ToLower(), FilterKeys.ZnodeCatalogId); }
                if (string.Equals(tuple.Item1, FilterKeys.CatalogId, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, FilterKeys.CatalogId.ToLower(), FilterKeys.ZnodeCatalogId); }
                if (string.Equals(tuple.Item1, ZnodeSearchDocumentMappingEnum.PublishCatalogId.ToString(), StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, ZnodeSearchDocumentMappingEnum.PublishCatalogId.ToString().ToLower(), FilterKeys.ZnodeCatalogId); }
                if (string.Equals(tuple.Item1, FilterKeys.PropertyName, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, FilterKeys.PropertyName, FilterKeys.AttributeCode); }
                if (string.Equals(tuple.Item1, $"{FilterKeys.PropertyName}|", StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, $"{FilterKeys.PropertyName}|", FilterKeys.AttributeCode); }
            }
        }

        //Get Product Id and Product Name if Seo belongs to Product.
        protected virtual void ProductSeoType(FilterCollection filters, SEOUrlModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("SeoCode to set filters: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, model?.SeoCode);
            filters.Add(new FilterTuple(FilterKeys.SKU, FilterOperators.Is, model.SeoCode));
            ReplaceFilterKeys(ref filters);
            ZnodePublishProductEntity product = GetService<IPublishedProductDataService>().GetPublishProductByFilters(filters);

            if (IsNotNull(product))
            {
                model.ProductId = product.ZnodeProductId;
                model.ProductName = product.Name;
                model.IsActive = product.IsActive;
                model.SEOId = product.ZnodeProductId;
                model.SeoCode = product.SKU;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
        }

        //Creates znode search request.
        public virtual IZnodeSearchRequest GetZnodeSearchRequest(SearchRequestModel model, FilterCollection filters, NameValueCollection sorts, bool isSuggestionList = false, bool isAllowIndexing = false)
        {
            if (IsNull(model))
                return null;

            var searchRequest = GetService<IZnodeSearchRequest>();

            List<SearchItemRuleModel> boostItems = new List<SearchItemRuleModel>();

            SearchProfileModel searchProfile = GetSearchProfileData(model, boostItems);

            searchRequest.CatalogId = model.CatalogId;
            searchRequest.LocaleId = model.LocaleId;

            int versionId = Convert.ToInt32( filters.Find(x => x.FilterName.ToLower() == WebStoreEnum.VersionId.ToString().ToLower())?.FilterValue);
            if (model.IsProductInheritanceEnabled && model.CategoryId > 0)
            {
                model.ParentCategoryIds = GetSubCategoryList(versionId, model.CategoryId);
                if(IsNull( model.ParentCategoryIds) || (model.ParentCategoryIds.Count < 1))
                    model.ParentCategoryIds = new List<int>() { model.CategoryId };
            }   
            else
                model.ParentCategoryIds = new List<int>() { model.CategoryId };
                

            searchRequest.InnerSearchKeywords = (model.InnerSearchKeywords != null) ? model.InnerSearchKeywords : null;
            searchRequest.Facets = (model.RefineBy != null) ? model.RefineBy : null;
            searchRequest.ExternalIdEnabled = model.ExternalIdEnabled;
            searchRequest.ExternalIdNotNull = model.ExternalIdNotNullCheck;
            searchRequest.IndexName = GetCatalogIndexName(model.CatalogId);
            searchRequest.SearchText = IsNotNull(model.Keyword) ? model.Keyword.Trim().ToLower() : string.Empty;
            //Remove the previous condition when the filter count greater than 1 then we are setting page number 1, 
            //due to which we are unable to check the records of other pages when multiple filters present.
            searchRequest.PageFrom = (model.PageNumber == 0) ? 1 : Convert.ToInt32(model.PageNumber);
            searchRequest.PageSize = Convert.ToInt32(model.PageSize);
            searchRequest.PostTags = "</div>";
            searchRequest.PreTags = "<div style = 'color:red' >";
            searchRequest.CatalogIdLocalIdDictionary = GetDefaultSearchableFieldsFilter(model, filters, isSuggestionList);
            searchRequest.NumberOfAggregationSize = model.NumberOfAggregationSize;
            searchRequest.boostField = model.CategoryId > 0 ? ZnodeConstant.catgoryBoost : ZnodeConstant.productBoost;
            searchRequest.SortCriteria = GetSort(sorts, model.CategoryId, model.Keyword);
            searchRequest.IsBrandSearch = model.IsBrandSearch;
            searchRequest.GetFacets = model.IsFacetList;
            //To do: This value will be user specific.
            searchRequest.SuggestionTermCount = 1;
            searchRequest.QueryTypeName = searchProfile.QueryTypeName;
            searchRequest.QueryClass = searchProfile.QueryBuilderClassName;
            searchRequest.SubQueryType = searchProfile.SubQueryType;
            searchRequest.Operator = searchProfile.Operator;
            searchRequest.AttributeList = GetCatalogAttributeList(model.CatalogId, model.LocaleId).ToModel<SearchAttributes>().ToList();
            searchRequest.SearchableAttribute = GetSearchableAttributeOfProfile(searchProfile.SearchableAttributesList);
            searchRequest.FacetableAttribute = GetSearchableAttributeOfProfile(searchProfile.FacetAttributesList);
            searchRequest.FeatureList = MapSearchProfileFeature(searchProfile.FeaturesList);
            searchRequest.FieldValueFactors = searchProfile.FieldValueFactors;
            searchRequest.BoostAndBuryItemLists = MapSearchBoostAndBuryItem(boostItems);
            searchRequest.IsAllowIndexing = isAllowIndexing;
            searchRequest.SearchProfileId = searchProfile.SearchProfileId;
            
            // To update the search query type available in the search request.
            UpdateSearchRequestQueryType(searchRequest);

            // To update the search feature values available in the search request.
            UpdateSearchRequestFeatureValues(searchRequest);
            UpdateSearchRequestEnableAccurateScoringValues(searchRequest);
            UpdateSearchRequestMinimumShouldMatchValues(searchRequest);

            return searchRequest;
        }

        protected virtual List<ElasticSearchBoostAndBuryItemList> MapSearchBoostAndBuryItem(List<SearchItemRuleModel> boostItems)
        {
            List<ElasticSearchBoostAndBuryItemList> list = new List<ElasticSearchBoostAndBuryItemList>();

            if (boostItems?.Count > 0)
            {
                foreach (SearchItemRuleModel item in boostItems)
                    list.Add(new ElasticSearchBoostAndBuryItemList
                    {
                        SearchItemKeyword = item.SearchItemKeyword,
                        SearchItemCondition = item.SearchItemCondition,
                        SearchItemValue = item.SearchItemValue,
                        SearchItemBoostValue = item.SearchItemBoostValue,
                        IsItemForAll = item.IsItemForAll,
                        SearchCatalogRuleId = item.SearchCatalogRuleId
                    });
            }

            return list;
        }

        protected virtual List<ElasticSearchFeature> MapSearchProfileFeature(List<SearchFeatureModel> featuresList)
        {
            List<ElasticSearchFeature> list = new List<ElasticSearchFeature>();

            if (featuresList?.Count > 0)
            {
                foreach (SearchFeatureModel feature in featuresList)
                    list.Add(new ElasticSearchFeature
                    {
                        FeatureCode = feature.FeatureCode,
                        SearchFeatureValue = feature.SearchFeatureValue,
                        FeatureName = feature.FeatureName,
                    });
            }
            return list;
        }

        //Gets the sort criteria.
        public virtual List<SortCriteria> GetSort(NameValueCollection sortCollection, int categoryId, string searchTerm)
        {
            if (sortCollection.HasKeys())
            {
                foreach (var key in sortCollection.AllKeys)
                {
                    var value = sortCollection.Get(key);

                    if (Equals(key, SortCriteria.SortNameEnum.ProductName.ToString().ToLower()))
                        return new List<SortCriteria>() { new SortCriteria() { SortDirection = (Equals(value, Constants.SortKeys.Ascending)) ? SortCriteria.SortDirectionEnum.ASC : SortCriteria.SortDirectionEnum.DESC, SortName = SortCriteria.SortNameEnum.ProductName } };

                    if (Equals(key, SortCriteria.SortNameEnum.Price.ToString().ToLower()))
                        return new List<SortCriteria>() { new SortCriteria() { SortDirection = (Equals(value, Constants.SortKeys.Ascending)) ? SortCriteria.SortDirectionEnum.ASC : SortCriteria.SortDirectionEnum.DESC, SortName = SortCriteria.SortNameEnum.Price } };

                    if (Equals(key, SortCriteria.SortNameEnum.HighestRated.ToString().ToLower()))
                        return new List<SortCriteria>() { new SortCriteria() { SortDirection = (Equals(value, Constants.SortKeys.Ascending)) ? SortCriteria.SortDirectionEnum.ASC : SortCriteria.SortDirectionEnum.DESC, SortName = SortCriteria.SortNameEnum.HighestRated } };

                    if (Equals(key, SortCriteria.SortNameEnum.MostReviewed.ToString().ToLower()))
                        return new List<SortCriteria>() { new SortCriteria() { SortDirection = (Equals(value, Constants.SortKeys.Ascending)) ? SortCriteria.SortDirectionEnum.ASC : SortCriteria.SortDirectionEnum.DESC, SortName = SortCriteria.SortNameEnum.MostReviewed } };

                    if (Equals(key, SortCriteria.SortNameEnum.OutOfStock.ToString().ToLower()))
                        return new List<SortCriteria>() { new SortCriteria() { SortDirection = (Equals(value, Constants.SortKeys.Ascending)) ? SortCriteria.SortDirectionEnum.ASC : SortCriteria.SortDirectionEnum.DESC, SortName = SortCriteria.SortNameEnum.OutOfStock } };

                    if (Equals(key, SortCriteria.SortNameEnum.InStock.ToString().ToLower()))
                        return new List<SortCriteria>() { new SortCriteria() { SortDirection = (Equals(value, Constants.SortKeys.Ascending)) ? SortCriteria.SortDirectionEnum.ASC : SortCriteria.SortDirectionEnum.DESC, SortName = SortCriteria.SortNameEnum.InStock } };
                }
            }

            if (categoryId > 0 && string.IsNullOrEmpty(searchTerm))
                return new List<SortCriteria>() { new SortCriteria() { SortName = SortCriteria.SortNameEnum.DisplayOrder, SortDirection = SortCriteria.SortDirectionEnum.ASC }, new SortCriteria() { SortName = SortCriteria.SortNameEnum.ProductName, SortDirection = SortCriteria.SortDirectionEnum.ASC } };
            else
                return new List<SortCriteria>();
        }

        //Gets expands for search.
        public virtual void GetFacetExpands(NameValueCollection expands, IZnodeSearchRequest request)
        {
            ExpandCategories(expands, request);
        }

        //Gets category expands for category(If category collection is required in search response.)
        protected virtual void ExpandCategories(NameValueCollection expands, IZnodeSearchRequest request)
        {
            //To do: Constants will be replaced later.
            if (!string.IsNullOrEmpty(expands.Get("Categories")))
            {
                request.GetCategoriesHierarchy = true;
            }
        }

        //Converts search response to keyword search model.
        public virtual KeywordSearchModel GetKeywordSearchModel(IZnodeSearchResponse response, IZnodeSearchRequest request = null)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            KeywordSearchModel model = new KeywordSearchModel();

            string output = JsonConvert.SerializeObject(response.ProductDetails);

            List<SearchProductModel> productList = JsonConvert.DeserializeObject<List<SearchProductModel>>(output);

            if (productList?.Count > 0)
                model.Products = productList.Count > 0 ? productList : new List<SearchProductModel>();
           
            if (response.ProductIds?.Count > 0 && model.Products?.Count > 0)
                model.Products = productList.OrderBy(d => response.ProductIds.IndexOf(d.ZnodeProductId)).ToList();

            if (response.Facets?.Count > 0)
                model.Facets = new List<SearchFacetModel>(response.Facets.
                    Select(x => new SearchFacetModel
                    {
                        DisplayOrder = x.DisplayOrder,
                        AttributeName = x.AttributeName,
                        AttributeCode = x.AttributeCode,
                        ControlTypeId = x.ControlTypeID,
                        AttributeValues = new List<SearchFacetValueModel>(x.AttributeValues.
                        Select(y => new SearchFacetValueModel
                        {
                            Label = y.AttributeValue,
                            AttributeValue = y.AttributeValue,
                            FacetCount = y.FacetCount,
                            DisplayOrder = y.displayorder
                        }).OrderBy(y => y.DisplayOrder).ToList())
                    }).OrderBy(x => x.DisplayOrder));

            if (response.CategoryItems?.Count > 0)
                model.Categories = GetCategoryFacetsFromResponse(response.CategoryItems);

            model.TotalProductCount = response.TotalProductCount;
            model.SuggestTerm = response.SuggestionTerm;
            model.IsSearchFromSuggestion = response.IsSearchFromSuggestion;
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return model;
        }

        //Converts the category collection in search request into search category model.
        protected virtual List<SearchCategoryModel> GetCategoryFacetsFromResponse(List<IZNodeSearchCategoryItem> categories)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            List<SearchCategoryModel> list = new List<SearchCategoryModel>();

            if (categories?.Count > 0)
            {
                foreach (var category in categories)
                {
                    list.Add(new SearchCategoryModel()
                    {
                        CategoryId = category.CategoryID,
                        CategoryName = category.Name,
                        Count = category.Count.GetValueOrDefault(),
                        SEOUrl = category.SEOUrl,
                        ParentCategories = IsNull(category.ParentCategory) ? null : GetCategoryFacetsFromResponse(category.ParentCategory)
                    });
                }
            }
            ZnodeLogging.LogMessage("Search category list count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, list?.Count);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return list;
        }

        //Save Product level boost values        
        protected virtual bool SaveProductBoostValues(BoostDataModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            bool saveResult = true;

            if (model.ID > 0)
            {
                ZnodeSearchGlobalProductBoost globalProductBoostEntity = _globalProductBoostRepository.Table.Where(x => x.SearchGlobalProductBoostId == model.ID).Select(x => x).AsEnumerable().FirstOrDefault();
                globalProductBoostEntity.Boost = model.Boost;
                saveResult = _globalProductBoostRepository.Update(globalProductBoostEntity);
            }
            else
                saveResult = _globalProductBoostRepository.Insert(new ZnodeSearchGlobalProductBoost { Boost = model.Boost, PublishCatalogId = model.CatalogId, PublishProductId = model.PublishProductId }).SearchGlobalProductBoostId > 0;

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return saveResult;
        }

        // Save category level boost values        
        protected virtual bool SaveProductCategoryBoostValues(BoostDataModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            var saveResult = true;

            if (model.ID > 0)
            {
                ZnodeSearchGlobalProductCategoryBoost globalProductCategoryBoostEntity = _globalProductCategoryBoostRepository.Table.Where(x => x.SearchGlobalProductCategoryBoostId == model.ID).Select(x => x).AsEnumerable().FirstOrDefault();
                globalProductCategoryBoostEntity.Boost = model.Boost;
                saveResult = _globalProductCategoryBoostRepository.Update(globalProductCategoryBoostEntity);
            }
            else
                saveResult = _globalProductCategoryBoostRepository.Insert(new ZnodeSearchGlobalProductCategoryBoost { Boost = model.Boost, PublishCatalogId = model.CatalogId, PublishProductId = model.PublishProductId, PublishCategoryId = model.PublishCategoryId }).SearchGlobalProductCategoryBoostId > 0;

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return saveResult;
        }

        // Save Field level boost values
        protected virtual bool SaveFieldsBoostValues(BoostDataModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            bool saveResult = true;

            if (model.ID > 0)
            {
                ZnodeSearchDocumentMapping fieldBoostEntity = _documentMappingRepository.Table.Where(x => x.SearchDocumentMappingId == model.ID).Select(x => x).AsEnumerable().FirstOrDefault();
                fieldBoostEntity.Boost = model.Boost;
                saveResult = _documentMappingRepository.Update(fieldBoostEntity);
            }
            else
                saveResult = _documentMappingRepository.Insert(new ZnodeSearchDocumentMapping { Boost = model.Boost, PropertyName = model.PropertyName, PublishCatalogId = model.CatalogId }).SearchDocumentMappingId > 0;

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return saveResult;
        }

        //Delete Field level boost if it is removed.
        protected virtual bool DeleteFieldsBoostValues(int documentmappingRepositoryId)
        {
            FilterCollection deleteFilter = new FilterCollection() { new FilterTuple(ZnodeSearchDocumentMappingEnum.SearchDocumentMappingId.ToString(), FilterOperators.Equals, documentmappingRepositoryId.ToString()) };
            return _documentMappingRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(deleteFilter.ToFilterDataCollection()).WhereClause);
        }

        //Delete product category boost if it is removed.
        protected virtual bool DeleteProductCategoryBoostValues(int searchGlobalProductCategoryBoostId)
        {
            FilterCollection deleteFilter = new FilterCollection() { new FilterTuple(ZnodeSearchGlobalProductCategoryBoostEnum.SearchGlobalProductCategoryBoostId.ToString(), FilterOperators.Equals, searchGlobalProductCategoryBoostId.ToString()) };
            return _globalProductCategoryBoostRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(deleteFilter.ToFilterDataCollection()).WhereClause);
        }

        //Delete product boost if it is removed.
        protected virtual bool DeleteProductBoostValues(int searchGlobalProductBoostId)
        {
            FilterCollection deleteFilter = new FilterCollection() { new FilterTuple(ZnodeSearchGlobalProductBoostEnum.SearchGlobalProductBoostId.ToString(), FilterOperators.Equals, searchGlobalProductBoostId.ToString()) };
            return _globalProductBoostRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(deleteFilter.ToFilterDataCollection()).WhereClause);
        }

        //Gets default and clause filter terms.
        public virtual Dictionary<string, List<string>> GetDefaultSearchableFieldsFilter(SearchRequestModel model, FilterCollection filters, bool isSuggestionList)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            Dictionary<string, List<string>> filterAndClause = new Dictionary<string, List<string>>();

            //Default filters for search
            model?.ParentCategoryIds?.RemoveAll(x => x <= 0);
            if (model?.ParentCategoryIds?.Count > 0)
                filterAndClause.Add(ZnodeConstant.ParentCategoryIds, model.ParentCategoryIds.Select(x => x.ToString()).ToList());
            else if (model.CategoryId > 0)
                filterAndClause.Add("categoryid", new List<string>() { model.CategoryId.ToString() });

            filters.RemoveAll(filter => filter.FilterName == FilterKeys.ProductIndex);
            filters.RemoveAll(filter => filter.FilterName.Equals(FilterKeys.IsFacet, StringComparison.OrdinalIgnoreCase));
            filters.RemoveAll(filter => filter.FilterName.Equals(FilterKeys.PortalId, StringComparison.OrdinalIgnoreCase));
            filters.RemoveAll(filter => filter.FilterName.Equals(FilterKeys.ZnodeCategoryId, StringComparison.OrdinalIgnoreCase));

            //To do: code for all operator.
            foreach (FilterTuple filter in filters)
            {
                switch (filter.FilterOperator)
                {
                    case FilterOperators.Equals:
                        filterAndClause.Add(filter.FilterName.ToLower(), new List<string>() { filter.FilterValue });
                        break;
                    case FilterOperators.In:
                        filterAndClause.Add(filter.FilterName.ToLower(), filter.FilterValue.Split(',').ToList());
                        break;
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return filterAndClause;
        }

        // Validate catalog id and locale id.
        protected virtual void ValidateCatalogIdAndLocaleId(SearchRequestModel model)
        {
            if (model.CatalogId <= 0)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorCatalogIdLessThanZero);
            if (model.LocaleId <= 0)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorLocaleIdLessThanZero);
            if (model.PortalId <= 0)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorPortalIdLessThanZero);
        }

        //Insert into ZnodeSearch indexMonitor.
        protected virtual ZnodeSearchIndexMonitor SearchIndexMonitorInsert(PortalIndexModel portalIndexModel)
        {
            return _searchIndexMonitorRepository.Insert(new ZnodeSearchIndexMonitor()
            {
                SourceId = 0,
                CatalogIndexId = portalIndexModel.CatalogIndexId,
                SourceType = "CreateIndex",
                SourceTransactionType = "INSERT",
                AffectedType = "CreateIndex",
                CreatedBy = portalIndexModel.CreatedBy,
                CreatedDate = portalIndexModel.CreatedDate,
                ModifiedBy = portalIndexModel.ModifiedBy,
                ModifiedDate = portalIndexModel.ModifiedDate
            });
        }

        protected virtual ZnodeSearchIndexMonitor CreateSearchIndexMonitorEntry(PortalIndexModel portalIndexModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("PortalIndexModel with PortalIndexId: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, portalIndexModel?.PortalIndexId);
            //Get Catalog index data by id.
            var getCatalogIndexDetail = (from aa in _catalogIndexRepository.Table
                                         where aa.CatalogIndexId == portalIndexModel.CatalogIndexId
                                         select new PortalIndexModel()
                                         {
                                             CatalogIndexId = portalIndexModel.CatalogIndexId,
                                             PublishCatalogId = aa.PublishCatalogId,
                                             IndexName = aa.IndexName,

                                         }).FirstOrDefault();
            //Check if same as earlier
            if (getCatalogIndexDetail.IndexName == portalIndexModel.IndexName)
            {
               return SearchIndexMonitorInsert(portalIndexModel);
            }
            else
            {
                //Check if index name is already used by another store.
                string indexName = _catalogIndexRepository.Table.Where(x => x.IndexName == portalIndexModel.IndexName).Select(s => s.IndexName)?.FirstOrDefault() ?? string.Empty;

                //Check if Duplicate IndexName Exist
                IsDuplicateSearchIndexNameExist(indexName, portalIndexModel);
                UpdatePublishProductEntityStatus();
                bool renameStatus = RenameIndex(portalIndexModel.CatalogIndexId, getCatalogIndexDetail.IndexName, portalIndexModel.IndexName);
                if (renameStatus)
                    _catalogIndexRepository.Update(new ZnodeCatalogIndex { CatalogIndexId = getCatalogIndexDetail.CatalogIndexId, PublishCatalogId = getCatalogIndexDetail.PublishCatalogId, IndexName = portalIndexModel.IndexName });

                else
                {
                    ZnodeSearchIndexMonitor searchIndexMonitor;
                    searchIndexMonitor = SearchIndexMonitorInsert(portalIndexModel);
                    SearchHelper searchHelper = new SearchHelper();
                    int searchIndexServerStatusId = 0;
                    searchIndexServerStatusId = searchHelper.CreateSearchIndexServerStatus(new SearchIndexServerStatusModel()
                    {
                        SearchIndexMonitorId = searchIndexMonitor.SearchIndexMonitorId,
                        ServerName = Environment.MachineName,
                        Status = ZnodeConstant.SearchIndexStartedStatus
                    }).SearchIndexServerStatusId;
                    CreateIndex(portalIndexModel.IndexName, portalIndexModel.RevisionType, portalIndexModel.PublishCatalogId, searchIndexMonitor.SearchIndexMonitorId, searchIndexServerStatusId, portalIndexModel.NewIndexName, portalIndexModel.IsPreviewProductionEnabled, portalIndexModel.IsPublishDraftProductsOnly);
                    RenameIndex(portalIndexModel.CatalogIndexId, getCatalogIndexDetail.IndexName, portalIndexModel.IndexName);
                    _catalogIndexRepository.Update(new ZnodeCatalogIndex { CatalogIndexId = getCatalogIndexDetail.CatalogIndexId, PublishCatalogId = getCatalogIndexDetail.PublishCatalogId, IndexName = portalIndexModel.IndexName });
                }
                return SearchIndexMonitorInsert(portalIndexModel);
            }
        }

        //Sets products boost value.
        protected virtual void SetProductBoostValue(SearchGlobalProductBoostListModel searchGlobalProductBoostList)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            string publishProductIds = string.Join(",", searchGlobalProductBoostList.SearchGlobalProductBoostList.Select(x => x.PublishProductId).ToList());

            if (!string.IsNullOrEmpty(publishProductIds))
            {
                FilterCollection productBoostFilter = new FilterCollection() { new FilterTuple(ZnodeSearchGlobalProductBoostEnum.PublishProductId.ToString(), FilterOperators.In, publishProductIds) };

                EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(productBoostFilter.ToFilterDataCollection());

                var productBoostList = _globalProductBoostRepository.GetEntityList(whereClause.WhereClause).ToList();

                if (productBoostList.Count > 0)
                {
                    searchGlobalProductBoostList.SearchGlobalProductBoostList.ForEach(searchGlobalProductBoost => searchGlobalProductBoost.Boost = (productBoostList.Where(productBoost => productBoost.PublishProductId == searchGlobalProductBoost.PublishProductId).Select(x => x.Boost).FirstOrDefault()));
                    searchGlobalProductBoostList.SearchGlobalProductBoostList.ForEach(searchGlobalProductBoost => searchGlobalProductBoost.SearchGlobalProductBoostId = (productBoostList.Where(productBoost => productBoost.PublishProductId == searchGlobalProductBoost.PublishProductId).Select(x => x.SearchGlobalProductBoostId).FirstOrDefault()));
                }
            }
            ZnodeLogging.LogMessage("SearchGlobalProductBoostList count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, searchGlobalProductBoostList?.SearchGlobalProductBoostList?.Count);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
        }

        //Sets category boost value.
        protected virtual void SetProductCatgoryBoostValue(SearchGlobalProductCategoryBoostListModel searchGlobalProductCategoryBoostList)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            StringBuilder productCategoryFilter = new StringBuilder("(");
            for (int i = 0; i < searchGlobalProductCategoryBoostList.SearchGlobalProductCategoryList.Count; i++)
            {
                productCategoryFilter.Append($"({ZnodeSearchGlobalProductCategoryBoostEnum.PublishCategoryId.ToString()}={searchGlobalProductCategoryBoostList.SearchGlobalProductCategoryList[i].PublishCategoryId} and {ZnodeSearchGlobalProductCategoryBoostEnum.PublishProductId.ToString()}={searchGlobalProductCategoryBoostList.SearchGlobalProductCategoryList[i].PublishProductId})");
                if (i < searchGlobalProductCategoryBoostList.SearchGlobalProductCategoryList.Count - 1)
                    productCategoryFilter.Append(" or ");
            }
            productCategoryFilter.Append(")");

            string productCategoryWhereClause = productCategoryFilter.ToString();

            if (!string.IsNullOrEmpty(productCategoryWhereClause))
            {
                var productCategoryBoostList = _globalProductCategoryBoostRepository.GetEntityList(productCategoryWhereClause).ToList();

                if (productCategoryBoostList.Count > 0)
                {
                    searchGlobalProductCategoryBoostList.SearchGlobalProductCategoryList.ForEach(searchGlobalProductCategoryBoost => searchGlobalProductCategoryBoost.Boost = (productCategoryBoostList.Where(productBoost => productBoost.PublishProductId == searchGlobalProductCategoryBoost.PublishProductId && productBoost.PublishCategoryId == searchGlobalProductCategoryBoost.PublishCategoryId).Select(x => x.Boost).FirstOrDefault()));
                    searchGlobalProductCategoryBoostList.SearchGlobalProductCategoryList.ForEach(searchGlobalProductCategoryBoost => searchGlobalProductCategoryBoost.SearchGlobalProductCategoryBoostId = (productCategoryBoostList.Where(productBoost => productBoost.PublishProductId == searchGlobalProductCategoryBoost.PublishProductId && productBoost.PublishCategoryId == searchGlobalProductCategoryBoost.PublishCategoryId).Select(x => x.SearchGlobalProductCategoryBoostId).FirstOrDefault()));
                }

            }
            ZnodeLogging.LogMessage("SearchGlobalProductCategoryList count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, searchGlobalProductCategoryBoostList?.SearchGlobalProductCategoryList?.Count);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
        }

        //Sets field boost value.
        protected virtual void SetFieldBoostValues(SearchDocumentMappingListModel listModel, List<string> searchableFields, int catalogId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            FilterCollection searchableFieldFilter = new FilterCollection() { new FilterTuple(ZnodeSearchDocumentMappingEnum.PropertyName.ToString(), FilterOperators.In, string.Join(",", searchableFields.Select(x => $"\"{x}\"").ToList())),
                                                                              new FilterTuple(ZnodeSearchDocumentMappingEnum.PublishCatalogId.ToString(), FilterOperators.Equals, catalogId.ToString())};

            EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(searchableFieldFilter.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause to get fieldBoostList: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, whereClause?.WhereClause);
            var fieldBoostList = _documentMappingRepository.GetEntityList(whereClause.WhereClause).ToList();

            if (fieldBoostList.Count > 0)
            {
                listModel.SearchDocumentMappingList.ForEach(searchFieldBoost => searchFieldBoost.Boost = (fieldBoostList.Where(fieldBoost => fieldBoost.PropertyName == searchFieldBoost.PropertyName).Select(x => x.Boost).FirstOrDefault()));
                listModel.SearchDocumentMappingList.ForEach(searchFieldBoost => searchFieldBoost.SearchDocumentMappingId = (fieldBoostList.Where(fieldBoost => fieldBoost.PropertyName == searchFieldBoost.PropertyName).Select(x => x.SearchDocumentMappingId).FirstOrDefault()));
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
        }

        //Calls application to create Index.
        protected virtual void CallSearchIndexer(PortalIndexModel portalIndexModel, int userId, int searchIndexServerStatusId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("userId and searchIndexServerStatusId to call search indexer: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { userId, searchIndexServerStatusId });
            ZnodeLogging.LogMessage(string.Format(Admin_Resources.SearchIndexerCalled, portalIndexModel.IndexName), ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            portalIndexModel.RevisionType = String.IsNullOrEmpty(portalIndexModel.RevisionType) ? "PRODUCTION" : portalIndexModel.RevisionType;
            string tokenValue = string.IsNullOrEmpty(HttpContext.Current.Request.Headers["Token"]) ? "0" : HttpContext.Current.Request.Headers["Token"];
            string apiDomainUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + "/";

           if (!portalIndexModel.DirectCalling)
            {
                CreateIndex(portalIndexModel.IndexName, portalIndexModel.RevisionType, portalIndexModel.PublishCatalogId, portalIndexModel.SearchCreateIndexMonitorId, searchIndexServerStatusId, portalIndexModel.NewIndexName, portalIndexModel.IsPreviewProductionEnabled, portalIndexModel.IsPublishDraftProductsOnly);
            }
            else
            {
                var eRPTaskSchedulerModel = new ERPTaskSchedulerModel
                {
                    SchedulerName = ZnodeConstant.SearchIndex,
                    SchedulerCallFor = ZnodeConstant.SearchIndex,
                    IsInstantJob = true
                };
                eRPTaskSchedulerModel.ExeParameters = $"Indexer,{portalIndexModel.PublishCatalogId},{portalIndexModel.IndexName},{portalIndexModel.CatalogIndexId},{portalIndexModel.SearchCreateIndexMonitorId},Manually,{apiDomainUrl},{userId},{searchIndexServerStatusId},{portalIndexModel.RevisionType},{portalIndexModel.IsPreviewProductionEnabled},{portalIndexModel.IsPublishDraftProductsOnly},{portalIndexModel.NewIndexName},{HttpContext.Current.Request.Headers["Authorization"]?.Replace("Basic ", "")},{tokenValue},{ZnodeApiSettings.RequestTimeout}";
                ZnodeLogging.LogMessage($"Arguments Passed : {eRPTaskSchedulerModel.ExeParameters}", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                bool result = _eRPJob.ConfigureJobs(eRPTaskSchedulerModel, out string hangfireJobId);

                if (result)
                    ZnodeLogging.LogMessage($"{ZnodeConstant.SearchIndex} job scheduled successfully.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                else
                    ZnodeLogging.LogMessage($"{ZnodeConstant.SearchIndex} job failed to get scheduled.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
            }

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
        }

        //Method to check if Preview mode is on or not.
        protected virtual bool GetIsPreviewEnabled()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            bool iSPreviewEnabled = (from publishState in _publishStateMappingRepository.Table
                                     join PS in _publishStateRepository.Table on publishState.PublishStateId equals PS.PublishStateId
                                     where publishState.IsActive && PS.IsActive && publishState.IsEnabled == true && PS.PublishStateCode == ZnodePublishStatesEnum.PREVIEW.ToString()
                                     select publishState.IsEnabled
                                                ).Any();

            ZnodeLogging.LogMessage("Is publish state preview is enabled : ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, iSPreviewEnabled);
            return iSPreviewEnabled;
        }

        //Get Image Path For Category List.
        protected virtual void GetProductImagePathForSuggestions(int portalId, List<SearchProductModel> productList)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            if (portalId > 0)
            {
                string imageName = string.Empty;
                IImageHelper imageHelper = GetService<IImageHelper>();
                productList?.ForEach(
                    x =>
                    {
                        imageName = x.Attributes.Where(y => y.AttributeCode == ZnodeConstant.ProductImage)?.FirstOrDefault()?.AttributeValues;
                        x.ImageSmallThumbnailPath = imageHelper.GetImageHttpPathSmallThumbnail(imageName);
                        x.ImageThumbNailPath = imageHelper.GetImageHttpPathThumbnail(imageName);
                    });
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
        }

        //Set stored based In Stock, Out Of Stock, Back Order Message.
        protected virtual void SetPortalBasedDetails(int portalId, List<SearchProductModel> searchProductModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            IPortalService _portalService = GetService<IPortalService>();
            PortalModel portalDetails = _portalService.GetPortal(portalId, null);
            searchProductModel?.ForEach(
                   x =>
                   {
                       x.InStockMessage = portalDetails.InStockMsg;
                       x.OutOfStockMessage = portalDetails.OutOfStockMsg;
                       x.BackOrderMessage = portalDetails.BackOrderMsg;
                   });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
        }

        protected virtual IZnodeSearchResponse GetPriceSortedSearchResponse(SearchRequestModel model, NameValueCollection sorts, IZnodeSearchProvider searchProvider, IZnodeSearchRequest searchRequest, string isInStock = "-1")
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            IZnodeSearchResponse searchResponse = searchProvider.FullTextSearch(searchRequest);
            DataTable productDetails = GetProductTypes(searchResponse.ProductDetails);
            //Create sku page.
            NameValueCollection priceSkuPage = new NameValueCollection();
            priceSkuPage.Add("index", searchRequest.PageFrom.ToString());
            priceSkuPage.Add("size", (Equals(searchRequest.PageSize, -1) ? int.MaxValue : searchRequest.PageSize).ToString());

            ReplaceSortKeyName(ref sorts, "price", "retailprice");
            IPriceService _priceService = GetService<IPriceService>();
            PriceSKUListModel skuPrice = _priceService.GetPagedPriceSKU(null, new FilterCollection() { new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, model.PortalId.ToString()) }, sorts, priceSkuPage, productDetails, isInStock);
            ZnodeLogging.LogMessage("PriceSKUList count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, skuPrice?.PriceSKUList?.Count);
            var SortedId = skuPrice.PriceSKUList.Select(x => x.PublishProductId).ToList();
            if (SortedId.Count > 0)
                searchResponse.ProductIds = SortedId;
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchResponse;
        }

        protected virtual DataTable GetProductTypes(List<dynamic> products)
        {
            DataTable table = new DataTable("ProductTable");
            DataColumn productId = new DataColumn("Id");
            productId.DataType = typeof(int);
            productId.AllowDBNull = false;
            table.Columns.Add(productId);
            table.Columns.Add("ProductType", typeof(string));
            table.Columns.Add("OutOfStockOptions", typeof(string));

            foreach (var item in products)
                table.Rows.Add(Convert.ToInt32(item["znodeproductid"]), Convert.ToString(item["producttype"]), Convert.ToString(item[ZnodeConstant.OutOfStockOptions.ToLower()]));

            return table;
        }

        //Gets search result from search.
        protected virtual void GetResultFromSuggestions(SearchRequestModel model, FilterCollection filters, NameValueCollection sorts, IZnodeSearchProvider searchProvider, ref IZnodeSearchRequest searchRequest, ref IZnodeSearchResponse searchResponse)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            if (model.UseSuggestion && !string.IsNullOrEmpty(searchResponse?.SuggestionTerm) && searchResponse.ProductDetails?.Count <= 0)
            {
                model.Keyword = searchResponse.SuggestionTerm;
                searchRequest = GetZnodeSearchRequest(model, filters, sorts);
                if (model.CategoryId < 1)
                    searchRequest.CatalogIdLocalIdDictionary.Add("productindex", new List<string>() { "1" });
                searchResponse = searchProvider.FullTextSearch(searchRequest);
                searchResponse.IsSearchFromSuggestion = true;
                searchResponse.SuggestionTerm = model.Keyword;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
        }

        //Method to get search response.
        public virtual IZnodeSearchResponse GetSearchResponse(SearchRequestModel model, NameValueCollection expands, ref NameValueCollection sorts, IZnodeSearchProvider searchProvider, IZnodeSearchRequest searchRequest, string sortName, bool isFacetSearch = false)
        {
            IZnodeSearchResponse searchResponse;
            ZnodeLogging.LogMessage("sortName and isFacetSearch: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { sortName, isFacetSearch });
            switch (sortName)
            {
                case ZnodeConstant.Price:
                    if (searchRequest.IsAllowIndexing)
                        searchResponse = searchProvider.FullTextSearch(searchRequest);
                    else
                        searchResponse = GetPriceSortedSearchResponse(model, sorts, searchProvider, searchRequest, "-1");
                    break;
                case ZnodeConstant.OutOfStock:
                    //Out of stock sorting cannot done in elastic search so that SortCriteria set to null 
                    searchRequest.SortCriteria = null;
                    searchResponse = GetPriceSortedSearchResponse(model, sorts, searchProvider, searchRequest, "0");
                    break;
                case ZnodeConstant.InStock:
                    //InStock sorting cannot done in elastic search so that SortCriteria set to null 
                    searchRequest.SortCriteria = null;
                    searchResponse = GetPriceSortedSearchResponse(model, sorts, searchProvider, searchRequest, "1");
                    break;
                default:
                    //All other conditions
                    if (IsNotNull(expands))
                        GetFacetExpands(expands, searchRequest);
                    searchResponse = searchProvider.FullTextSearch(searchRequest);
                    break;
            }

            return searchResponse;
        }

        public virtual string GetCatalogIndexName(int cataLogId)
        {
            ZnodeLogging.LogMessage("cataLogId to get index name: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, cataLogId);
            string cacheKey = $"CatalogIndexName_{cataLogId}";
            string indexName = Equals(HttpRuntime.Cache[cacheKey], null)
               ? GetCatalogIndexNameFromDB(cataLogId)
               : ((string)HttpRuntime.Cache.Get(cacheKey));
            return indexName;
        }

        // Fetch associated Category Ids based on the isProductInheritanceEnabled flag.
        public virtual List<int> GetAssociatedCategoryIds(IZnodeSearchRequest searchRequest, KeywordSearchModel searchResult, bool isProductInheritanceEnabled)
        {
            if(HelperUtility.IsNotNull(searchRequest))
            {
                searchRequest.Source = new string[] { ZnodeConstant.CategoryId, ZnodeConstant.ZnodeProductId, ZnodeConstant.ParentCategoryIds };
                searchRequest.PageSize = MaxPageSize;
                searchRequest.CatalogIdLocalIdDictionary?.Remove(ZnodeConstant.ProductIndex);
                IZnodeSearchResponse searchCategoryResponse = znodeSearchProvider.FullTextSearch(searchRequest);
                KeywordSearchModel searchCategoryResult = IsNotNull(searchCategoryResponse) ? GetKeywordSearchModel(searchCategoryResponse) : new KeywordSearchModel();
                List<int> productIds = searchResult?.Products?.Select(x => x.ZnodeProductId)?.ToList() ?? new List<int>();
                if (isProductInheritanceEnabled)
                    return searchCategoryResult?.Products?.Where(x => productIds.Contains(x.ZnodeProductId))?.SelectMany(x => x.ParentCategoryIds)?.ToList();
                return searchCategoryResult?.Products?.Where(x => productIds.Contains(x.ZnodeProductId))?.Select(x => x.CategoryId)?.ToList();
            }
            return new List<int>();
        }

        // Method to fetch associated Category Ids.
        public virtual List<int> GetAssociatedCategoryIds(IZnodeSearchRequest searchRequest, KeywordSearchModel searchResult)
        {
            return GetAssociatedCategoryIds(searchRequest, searchResult, false);
        }

        protected virtual string GetCatalogIndexNameFromDB(int publishCatalogId)
        {
            ZnodeLogging.LogMessage("publishCatalogId to get catalog index name: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, publishCatalogId);
            FilterCollection filter = new FilterCollection() { new FilterTuple(ZnodeCatalogIndexEnum.PublishCatalogId.ToString(), FilterOperators.Equals, publishCatalogId.ToString()) };
            EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());
            return _catalogIndexRepository.GetEntity(whereClause.WhereClause)?.IndexName;
        }

        public virtual List<ZnodePublishCatalogAttributeEntity> GetCatalogAttributeList(int cataLogId, int localeId)
        {
            string cacheKey = $"CatalogAttribute_{cataLogId}_{localeId}";
            List<ZnodePublishCatalogAttributeEntity> attributeList = Equals(HttpRuntime.Cache[cacheKey], null)
               ? GetCatalogAttributeListFromDB(cataLogId, localeId, cacheKey)
               : (List<ZnodePublishCatalogAttributeEntity>)HttpRuntime.Cache.Get(cacheKey);
            return attributeList;
        }

        protected virtual List<ZnodePublishCatalogAttributeEntity> GetCatalogAttributeListFromDB(int cataLogId, int localeId, string cacheKey)
        {
            ZnodeLogging.LogMessage("cataLogId, localeId and cacheKey to get attributeList: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { cataLogId, localeId, cacheKey });

            List<ZnodePublishCatalogAttributeEntity> attributeList = GetService<IPublishedCatalogDataService>().GetPublishCatalogAttribute(cataLogId, localeId, GetCatalogVersionId(cataLogId, localeId));

            HttpRuntime.Cache.Insert(cacheKey, attributeList);
            ZnodeLogging.LogMessage("attributeList count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, attributeList?.Count);
            return attributeList;
        }

        /// <summary>
        /// Get Search Profile Data
        /// </summary>
        /// <param name="model">Search Request Model</param>
        /// <param name="boostItems">List of Search Item Rule Model</param>
        /// <returns>Search Profile Model</returns>
        public virtual SearchProfileModel GetSearchProfileData(SearchRequestModel model, List<SearchItemRuleModel> boostItems)
        {
            string cacheKey = $"SearchProfileData";
            SearchProfileModel profileData = Equals(HttpRuntime.Cache[cacheKey], null)
               ? GetSearchProfileDataFromDB(model, boostItems)
               : ((SearchProfileModel)HttpRuntime.Cache.Get(cacheKey));
            return profileData;
        }

        protected virtual SearchProfileModel GetSearchProfileDataFromDB(SearchRequestModel model, List<SearchItemRuleModel> boostItems)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ExecuteSpHelper objStoredProc = new ExecuteSpHelper();
            objStoredProc.GetParameter("@Keyword", model.Keyword, ParameterDirection.Input, SqlDbType.NVarChar);
            objStoredProc.GetParameter("@ProfileId", model.ProfileId, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@PublishCatalogId", model.CatalogId, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@PortalId", model.PortalId, ParameterDirection.Input, SqlDbType.Int);
            DataSet dataSet = objStoredProc.GetSPResultInDataSet("Znode_GetWebStoreSearchProfileTrigger");

            SearchProfileModel searchProfile = MapPublishedDataToSearchProfile(dataSet.Tables[0]);

            if(IsNotNull(boostItems))
                 boostItems.AddRange(GetSearchBoostItemList(dataSet.Tables[1]));

            ZnodeLogging.LogMessage(string.Format(Admin_Resources.SearchProfile, searchProfile?.ProfileName), ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchProfile;
        }

        //returns the list of all inventories associated to the product for that portal.
        protected virtual List<InventorySKUModel> GetAllLocationsInventoryForProduct(string ProductSKU, IList<PublishCategoryProductDetailModel> productDetails)
        {
            List<InventorySKUModel> productInventoryList = new List<InventorySKUModel>();
            List<PublishCategoryProductDetailModel> products = productDetails?.Where(x => x.SKU == ProductSKU)?.ToList();
            foreach (PublishCategoryProductDetailModel product in products)
            {
                productInventoryList.Add(new InventorySKUModel() { Quantity = product.Quantity.GetValueOrDefault(), WarehouseName = product.WarehouseName, IsDefaultWarehouse = product.IsDefaultWarehouse });
            }
            return productInventoryList;
        }

        //Returns the value for send all locations flag
        protected virtual bool GetSendLocationsFlag(NameValueCollection expands)
        => Convert.ToBoolean(expands?.Get(ZnodeConstant.IsGetAllLocationsInventory));

        //Bind the IsGetAllLocationsInventory from filters to expands
        protected virtual void BindAllLocationsFlagInExpands(FilterCollection filters, NameValueCollection expands)
        {
            bool isSendAllStockLevels = Convert.ToBoolean(filters.FirstOrDefault(m => string.Compare(m.FilterName, ZnodeConstant.IsGetAllLocationsInventory, true) == 0)?.Item3);
            if (isSendAllStockLevels)
                expands.Add(ZnodeConstant.IsGetAllLocationsInventory, isSendAllStockLevels.ToString());
            filters.RemoveAll(x => x.FilterName.Equals(ZnodeConstant.IsGetAllLocationsInventory, StringComparison.InvariantCultureIgnoreCase));
        }

        //To get the publish catalog Id from filters.
        protected virtual int GetCatalogIdFromFilters(FilterCollection filters)
        {
            int publishCatalogId = Convert.ToInt32(filters?.FirstOrDefault(filterTuple => filterTuple.Item1 == FilterKeys.PublishCatalogId.ToString().ToLower())?.Item3);
            return publishCatalogId;
        }

        //To update filter with specified details.
        protected virtual void UpdateFilters(FilterCollection filters, string filterName, string filterOperator, string filterValue)
        {
            if (Equals(filters?.Exists(x => x.Item1 == filterName), true))
            {
                filters?.RemoveAll(x => x.Item1 == filterName);
            }
            filters?.Add(new FilterTuple(filterName, filterOperator, filterValue));
        }

        //Check if duplicate index name exist.
        protected virtual void IsDuplicateSearchIndexNameExist(string indexName, PortalIndexModel portalIndexModel)
        {
            if (!string.IsNullOrEmpty(indexName) || defaultDataService.IsIndexExists(portalIndexModel.IndexName) || GetService<ICMSPageDefaultDataService>().IsIndexExists(portalIndexModel.IndexName))
                throw new ZnodeException(ErrorCodes.DuplicateSearchIndexName, Admin_Resources.ErrorIndexNameIsInUse);
        }

        // This method will be used to update the search feature values available in the search request.
        protected virtual void UpdateSearchRequestFeatureValues(IZnodeSearchRequest searchRequest)
        {
            // As elastic search does not support Fuzziness with the Cross fields, the AutoCorrect feature needs to be set as False if the SubQueryType is Cross.
            if (searchRequest?.FeatureList?.FirstOrDefault(x => string.Equals(x.FeatureCode, ZnodeConstant.AutoCorrect, StringComparison.InvariantCultureIgnoreCase)) != null
                && string.Equals(searchRequest.SubQueryType?.Replace(" ", ""), ZnodeConstant.MultiMatchCross, StringComparison.InvariantCultureIgnoreCase))
            {
                searchRequest.FeatureList.FirstOrDefault(x => string.Equals(x.FeatureCode, ZnodeConstant.AutoCorrect, StringComparison.InvariantCultureIgnoreCase))
                    .SearchFeatureValue = ZnodeConstant.False;
            }
        }

        // This method will be used to update the search Enable Accurate Scoring values available in the search request.
        protected virtual void UpdateSearchRequestEnableAccurateScoringValues(IZnodeSearchRequest searchRequest)
        {
            if (searchRequest?.FeatureList?.FirstOrDefault(x => string.Equals(x.FeatureCode, ZnodeConstant.DfsQueryThenFetch, StringComparison.InvariantCultureIgnoreCase)) != null)
                // As Enable Accurate Scoring is not supported by Znode. So assigning null to it. So, that this should not impact the ElasticSearch results.
                searchRequest.FeatureList.FirstOrDefault(x => string.Equals(x.FeatureCode, ZnodeConstant.DfsQueryThenFetch, StringComparison.InvariantCultureIgnoreCase))
                    .SearchFeatureValue = null;
        }

        // This method will be used to update the search Minimum Should Match values available in the search request.
        protected virtual void UpdateSearchRequestMinimumShouldMatchValues(IZnodeSearchRequest searchRequest)
        {
            if (searchRequest?.FeatureList?.FirstOrDefault(x => string.Equals(x.FeatureCode, ZnodeConstant.MinimumShouldMatch, StringComparison.InvariantCultureIgnoreCase)) != null)
                // As Minimum Should Match is not supported by Znode. So assigning null to it. So, that this should not impact the ElasticSearch results.
                searchRequest.FeatureList.FirstOrDefault(x => string.Equals(x.FeatureCode, ZnodeConstant.MinimumShouldMatch, StringComparison.InvariantCultureIgnoreCase))
                    .SearchFeatureValue = null;
        }

        // This method will be used to update the search query type available in the search request.
        protected virtual void UpdateSearchRequestQueryType(IZnodeSearchRequest searchRequest)
        {
            // The excludeQueryTypes variable contains the list of search query types which is not supported by Znode from 9.7.1 release.
            List<string> excludedQueryTypes = GetExcludedSearchQueryTypeList();
            // As we are remove the support of Match, MatchPhrase, MatchPhrasePrefix query types, if old search profile having these query types then updating it with default query type i.e MultiMatch > Cross
            if (IsNotNull(searchRequest) && excludedQueryTypes.Contains(searchRequest?.QueryTypeName?.ToLower()))
            {
                searchRequest.QueryTypeName = ZnodeConstant.MultiMatch;
                searchRequest.SubQueryType = ZnodeConstant.MultiMatchCross;
            }
        }

        //To set the product SEO urls based on the isCatalogIndexSettingEnable flag
        protected virtual void BindProductSeoUrl(KeywordSearchModel searchResult, SearchRequestModel model, int? versionId)
        {
            //To bind the product SEO urls. If isCatalogIndexSettingEnable flag is set to false.
            bool isCatalogIndexSettingEnable = IsAllowIndexing(model.CatalogId, versionId);
            if (!isCatalogIndexSettingEnable)
            {
                //below code is creating comma seperated sku.
                string skus = string.Join(",", searchResult?.Products.Select(a => a.SKU));

                //To get the product SEO urls
                IList<ProductSeoModel> productSeoList = GetProductSeoUrls(skus, model.PortalId);
                if (productSeoList?.Count > 0)
                {
                    foreach (var item in productSeoList)
                    {
                        searchResult?.Products.Where(c => c.SKU.Equals(item.SKU,StringComparison.CurrentCultureIgnoreCase))?.Select(c => { c.SEOUrl = item.SEOUrl; return c; }).ToList();
                    }
                }
            }
        }

        protected void UpdatePublishProductEntityStatus()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ExecuteSpHelper objStoredProc = new ExecuteSpHelper();
            objStoredProc.GetSPResultInDataSet("Znode_ChangeStatusForIndexCreation");
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
        }

        // To check whether ZnodePublishProductEntity have valid value in ZnodeParentCategoryIds column for atleast one product.
        // This check is required to allow backward compatibility, after upgrade if the user does not publish the catalog(after upgrade for all records null
        // will be maintained in the ZnodeParentCategoryIds column and valid values will be filled at the time of publish) and enables the "Enable Inheritance Of Child Products To Parent Category"
        // feature from the manage store screen then the search query should be created based on the category id instead of parent category ids.
        protected virtual void ValidateProductInheritance(SearchRequestModel searchRequestModel, int? versionId, int publishCatalogId)
        {
            if (IsNotNull(searchRequestModel) && searchRequestModel.IsProductInheritanceEnabled && searchRequestModel.CategoryId > 0)
            {
                IZnodeRepository<ZnodePublishProductEntity> publishProductEntity = new ZnodeRepository<ZnodePublishProductEntity>(HelperMethods.Context);
                searchRequestModel.IsProductInheritanceEnabled = publishProductEntity.Table.Any(x => x.VersionId == versionId && x.ZnodeCatalogId == publishCatalogId
                && x.ZnodeParentCategoryIds != null);
            }
        }

        #region Get Product Seo Urls
        protected virtual IList<ProductSeoModel> GetProductSeoUrls(string skus, int portalId)
        {
            ZnodeLogging.LogMessage("GetProductSeoUrls executuion started: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            IZnodeViewRepository<ProductSeoModel> objStoredProc = new ZnodeViewRepository<ProductSeoModel>();
            objStoredProc.SetParameter("@Skus", skus, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);

            int status = 0;
            //selecting "SeoUrls" on the basis of Skus and PortalId.
            IList<ProductSeoModel> seoResults = objStoredProc.ExecuteStoredProcedureList("Znode_GetSeoBySku @Skus,@PortalId, @Status OUT", 2, out status);
            ZnodeLogging.LogMessage("GetProductSeoUrls execution done: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return seoResults;
        }

        // To get the search item rules from data table.
        protected virtual List<SearchItemRuleModel> GetSearchBoostItemList(DataTable boostItemTable)
        {
           return boostItemTable?.AsEnumerable().Select(m => new SearchItemRuleModel()
            {
                SearchCatalogRuleId = m.Field<int>("SearchCatalogRuleId"),
                SearchItemKeyword = m.Field<string>("SearchItemKeyword"),
                SearchItemCondition = m.Field<string>("SearchItemCondition"),
                SearchItemValue = m.Field<string>("SearchItemValue"),
                SearchItemBoostValue = m.Field<decimal?>("SearchItemBoostValue"),
                IsItemForAll = m.Field<bool>("IsItemForAll")
            }).ToList();
        }

        // Mapping Published data to search Profile Model
        protected virtual SearchProfileModel MapPublishedDataToSearchProfile(DataTable publishSearchProfileDetails)
        {
            SearchProfileModel searchProfile = new SearchProfileModel();

            PublishSearchProfileModel publishedSearchProfile = MapDetailsToPublishSearchProfileModel(publishSearchProfileDetails);
            if (IsNotNull(publishedSearchProfile))
            {
                searchProfile.FeaturesList = JsonConvert.DeserializeObject<List<SearchFeatureModel>>(publishedSearchProfile.FeaturesList);
                searchProfile.SearchableAttributesList = JsonConvert.DeserializeObject<List<SearchAttributesModel>>(publishedSearchProfile.SearchProfileAttributeMappingJson);

                searchProfile.FacetAttributesList = searchProfile.SearchableAttributesList.Where(m => m.IsFacets == true).ToList();
                searchProfile.SearchableAttributesList = searchProfile.SearchableAttributesList.Where(m => m.IsUseInSearch == true).ToList();

                searchProfile.ProfileName = publishedSearchProfile.SearchProfileName;
                searchProfile.QueryTypeName = publishedSearchProfile.QueryTypeName;
                searchProfile.SubQueryType = publishedSearchProfile.SubQueryType;
                searchProfile.QueryBuilderClassName = publishedSearchProfile.QueryBuilderClassName;
                searchProfile.Operator = publishedSearchProfile.Operator;
                searchProfile.SearchProfileId = publishedSearchProfile.SearchProfileId;

                if (!string.IsNullOrEmpty(publishedSearchProfile.FieldValueFactor))
                    searchProfile.FieldValueFactors = GetFieldValueFactorDictionary(publishedSearchProfile.FieldValueFactor);
            }
            return searchProfile;       
        }

        //Extracting Published Data from published Data table
        protected virtual PublishSearchProfileModel MapDetailsToPublishSearchProfileModel(DataTable publishSearchProfileDetails)
        {
            return publishSearchProfileDetails?.AsEnumerable().Select(m => new PublishSearchProfileModel()
            {
                PublishSearchProfileId = m.Field<int>("PublishSearchProfileEntityId"),
                SearchProfileId = m.Field<int>("SearchProfileId"),
                SearchProfileName = m.Field<string>("SearchProfileName"),
                PublishCatalogId = m.Field<int?>("ZnodeCatalogId").GetValueOrDefault(),
                FeaturesList = m.Field<string>("FeaturesList"),
                QueryTypeName = m.Field<string>("QueryTypeName"),
                SearchQueryType = m.Field<string>("SearchQueryType"),
                QueryBuilderClassName = m.Field<string>("QueryBuilderClassName"),
                SubQueryType = m.Field<string>("SubQueryType"),
                FieldValueFactor = m.Field<string>("FieldValueFactor"),
                Operator = m.Field<string>("Operator"),
                PublishStateId = m.Field<int>("PublishStateId"),
                SearchProfileAttributeMappingJson = m.Field<string>("SearchProfileAttributeMappingJson"),
                SearchProfilePublishLogId = m.Field<int>("SearchProfilePublishLogId"),
            }).ToList().FirstOrDefault();
        }

        // To get configured field value factor details
        protected virtual List<KeyValuePair<String, int>> GetFieldValueFactorDictionary(string fieldValueFactorData)
        {
            if (!string.IsNullOrEmpty(fieldValueFactorData))
            {
                List<Dictionary<object, object>> fieldValueFactors = JsonConvert.DeserializeObject<List<Dictionary<object, object>>>(fieldValueFactorData);
                List<KeyValuePair<string, int>> fieldValueList = new List<KeyValuePair<string, int>>();
                foreach (Dictionary<object, object> fieldValueData in fieldValueFactors)
                    fieldValueList.Add(new KeyValuePair<string, int>(Convert.ToString(fieldValueData["FieldName"]), Convert.ToInt32(fieldValueData["FieldValueFactor"])));

                return fieldValueList;
            }
            return null;
        }
        #endregion

        // Get product details by sku.
        public virtual PublishProductModel GetProductDetailsBySKU(SearchRequestModel model, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            KeywordSearchModel keywordSearchModel = FullTextSearch(model, expands, filters, sorts, page);
            filters?.RemoveAll(x => x.FilterName == FilterKeys.IsActive);
            filters?.RemoveAll(x => x.FilterName.Equals(FilterKeys.VersionId, comparisonType: StringComparison.CurrentCultureIgnoreCase));
            filters.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, GetPortalId().ToString());
            filters.Add(WebStoreEnum.IsActive.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue);

            int productId = 0;
            if (keywordSearchModel?.Products?.Count > 0)
            {
                productId = (keywordSearchModel.Products.FirstOrDefault(x => x.SKU.Equals(model.Keyword, comparisonType: StringComparison.CurrentCultureIgnoreCase))?.ZnodeProductId).GetValueOrDefault();
            }
             if (productId != 0)
             {
                 IPublishProductService _publishService = GetService<IPublishProductService>();
                 return _publishService.GetPublishProduct(productId, filters, expands);
             }
            return null;
        }

        //Get subcategory list
        protected virtual List<int> GetSubCategoryList(int versionId, int categoryId)
        {
            try
            {
                ZnodeLogging.LogMessage("GetSubCategoryList executuion started: CategoryId:"+ categoryId + ", VersionId:"+ versionId, ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                IZnodeViewRepository<View_ReturnBooleanWithMessage> objStoredProc = new ZnodeViewRepository<View_ReturnBooleanWithMessage>();
                objStoredProc.SetParameter("@VersionId", versionId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@ZnodeCategoryId", categoryId, ParameterDirection.Input, DbType.Int32);

                //selecting "SubcategoryId's" on the basis of VersionId and ZnodeCategoryId.
                View_ReturnBooleanWithMessage seoResults = objStoredProc.ExecuteStoredProcedureList("Znode_GetPublishChildCategory @VersionId,@ZnodeCategoryId").FirstOrDefault();
                ZnodeLogging.LogMessage("GetSubCategoryList execution done: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                if (seoResults.Status == true)
                    return seoResults.MessageDetails.Split(',').Select(int.Parse).ToList();
                else 
                {
                    ZnodeLogging.LogMessage("Error in GetSubCategoryList:" + seoResults.MessageDetails, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                    return new List<int>();
                } 
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetSubCategoryList:" + ex.Message, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                return new List<int>();
            }
        }

    }
    #endregion
}
