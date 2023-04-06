

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Admin;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.ElasticSearch;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using Znode.Libraries.Search;

using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class CMSPageSearchService : BaseService, ICMSPageSearchService
    {
        #region Protected Variables

        protected readonly IZnodeRepository<ZnodeCMSSearchIndex> _cmsSearchIndexRepository;
        protected readonly IZnodeRepository<ZnodeCMSSearchIndexMonitor> _cmsSearchIndexMonitorRepository;
        protected readonly IZnodeRepository<ZnodePublishStateApplicationTypeMapping> _publishStateMappingRepository;
        protected readonly IZnodeRepository<ZnodePublishState> _publishStateRepository;
        protected readonly IZnodeRepository<ZnodeCMSSearchIndexServerStatu> _cmsSearchIndexServerStatusRepository;
        protected readonly IZnodeRepository<ZnodePortal> _portalRepository;
        protected readonly IZnodeRepository<ZnodePortalFeature> _portalFeatureRepository;
        protected readonly IZnodeRepository<ZnodePortalFeatureMapper> _portalFeatureMapperRepository;

        protected readonly ICMSPageDefaultDataService cmsPageDefaultDataService;

        #endregion

        #region Constructor
        public CMSPageSearchService()
        {
            _cmsSearchIndexRepository = new ZnodeRepository<ZnodeCMSSearchIndex>();
            _cmsSearchIndexMonitorRepository = new ZnodeRepository<ZnodeCMSSearchIndexMonitor>();
            _publishStateMappingRepository = new ZnodeRepository<ZnodePublishStateApplicationTypeMapping>();
            _publishStateRepository = new ZnodeRepository<ZnodePublishState>();
            _cmsSearchIndexServerStatusRepository = new ZnodeRepository<ZnodeCMSSearchIndexServerStatu>();
            _portalRepository = new ZnodeRepository<ZnodePortal>();
            _portalFeatureRepository = new ZnodeRepository<ZnodePortalFeature>();
            _portalFeatureMapperRepository = new ZnodeRepository<ZnodePortalFeatureMapper>();

            cmsPageDefaultDataService = GetService<ICMSPageDefaultDataService>();
        }
        #endregion

        #region CMS Page index    

        //Insert data of CMS pages for creating index by checking revision type.
        public virtual CMSPortalContentPageIndexModel InsertCreateCmsPageIndexDataByRevisionTypes(CMSPortalContentPageIndexModel cmsPortalContentPageIndex)
        {
            if (string.IsNullOrEmpty(cmsPortalContentPageIndex.RevisionType) || cmsPortalContentPageIndex.RevisionType.Equals("NONE", StringComparison.OrdinalIgnoreCase))
                cmsPortalContentPageIndex.RevisionType = ZnodePublishStatesEnum.PRODUCTION.ToString();
            if (cmsPortalContentPageIndex.RevisionType == ZnodePublishStatesEnum.PRODUCTION.ToString() && IsPreviewEnabled())
            {
                List<string> RevisionTypes = new List<string>() { ZnodePublishStatesEnum.PREVIEW.ToString(), ZnodePublishStatesEnum.PRODUCTION.ToString() };

                foreach (string revisionType in RevisionTypes)
                {
                    cmsPortalContentPageIndex.RevisionType = revisionType;
                    cmsPortalContentPageIndex = InsertCreateCmsPageIndexData(cmsPortalContentPageIndex);
                }

                return cmsPortalContentPageIndex;
            }

            return InsertCreateCmsPageIndexData(cmsPortalContentPageIndex);
        }

        //Insert data for Cms page creating index.
        public virtual CMSPortalContentPageIndexModel InsertCreateCmsPageIndexData(CMSPortalContentPageIndexModel cmsPortalContentPageIndex)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(cmsPortalContentPageIndex))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorPortalIndexModelNull);

            if (IsCMSPageResultsFeatureEnabled(cmsPortalContentPageIndex.PortalId))
            {
                ZnodeCMSSearchIndexMonitor cmsSearchIndexMonitor;

                SearchHelper searchHelper = new SearchHelper();

                int cmsSearchIndexServerStatusId = 0;

                string storeName = cmsPortalContentPageIndex.StoreName;

               //If index name is not present in model then get CMS page indexname based on storecode
                if (string.IsNullOrEmpty(cmsPortalContentPageIndex.IndexName))
                {
                    cmsPortalContentPageIndex.IndexName = GetCMSPageIndexName(cmsPortalContentPageIndex.PortalId);
                }               

                if (cmsPortalContentPageIndex.CMSSearchIndexId < 1)
                {
                    CMSPortalContentPageIndexModel cMSPortalContentPageIndexModel = GetCMSPortalContentPageIndexModelByPortalId(cmsPortalContentPageIndex.PortalId);
                    cmsPortalContentPageIndex.CMSSearchIndexId = IsNotNull(cMSPortalContentPageIndexModel) ? cMSPortalContentPageIndexModel.CMSSearchIndexId : 0;
                }

                //Allow to insert only if data does not exists.
                if (cmsPortalContentPageIndex.CMSSearchIndexId < 1)
                {
                    //Check if index name is already used by another store.
                    string indexName = _cmsSearchIndexRepository.Table.FirstOrDefault(x => x.IndexName == cmsPortalContentPageIndex.IndexName)?.IndexName ?? string.Empty;

                    if (!cmsPortalContentPageIndex.IsFromStorePublish)
                    {
                        if (!string.IsNullOrEmpty(indexName))
                        {
                            //Check if Duplicate IndexName Exist
                            IsDuplicateSearchIndexNameExist(indexName, cmsPortalContentPageIndex);
                        }                       
                    }            

                    string revisionType = cmsPortalContentPageIndex.RevisionType;

                     indexName = GetCMSPortalContentPageIndexModelByPortalId(cmsPortalContentPageIndex.PortalId)?.IndexName;
                    //insert and update index name in database.
                    if (!string.IsNullOrEmpty(indexName))
                    {
                        bool isUpdated = _cmsSearchIndexRepository.Update(cmsPortalContentPageIndex.ToEntity<ZnodeCMSSearchIndex>());
                        if (isUpdated)
                        {
                            cmsPortalContentPageIndex = GetCMSPortalContentPageIndexModelByPortalId(cmsPortalContentPageIndex.PortalId);
                        }
                    }
                    else
                    {
                        cmsPortalContentPageIndex = _cmsSearchIndexRepository.Insert(cmsPortalContentPageIndex.ToEntity<ZnodeCMSSearchIndex>())?.ToModel<CMSPortalContentPageIndexModel>();
                    }
                    cmsPortalContentPageIndex.RevisionType = revisionType;

                    //Create index monitor entry.
                    cmsSearchIndexMonitor = CMSSearchIndexMonitorInsert(cmsPortalContentPageIndex);
                    ZnodeLogging.LogMessage("CMSSearchIndexMonitorId: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, cmsSearchIndexMonitor?.CMSSearchIndexMonitorId);

                    if (cmsPortalContentPageIndex.CMSSearchIndexId > 0 && cmsSearchIndexMonitor?.CMSSearchIndexMonitorId > 0)
                    {
                        cmsPortalContentPageIndex.CMSSearchIndexMonitorId = cmsSearchIndexMonitor.CMSSearchIndexMonitorId;

                        ZnodeLogging.LogMessage(Admin_Resources.SuccessSearchIndexCreate, ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

                        //Start status for creating index for server name saved.
                        cmsSearchIndexServerStatusId = searchHelper.CreateCmsPageSearchIndexServerStatus(new CMSSearchIndexServerStatusModel()
                        {
                            CMSSearchIndexMonitorId = cmsSearchIndexMonitor.CMSSearchIndexMonitorId,
                            ServerName = Environment.MachineName,
                            Status = ZnodeConstant.SearchIndexStartedStatus
                        }).CMSSearchIndexServerStatusId;

                        CreateCmsPageIndex(cmsPortalContentPageIndex.IndexName, cmsPortalContentPageIndex.RevisionType, cmsPortalContentPageIndex.PortalId, cmsSearchIndexMonitor.CMSSearchIndexMonitorId, cmsSearchIndexServerStatusId);

                        cmsPortalContentPageIndex.StoreName = storeName;
                        return cmsPortalContentPageIndex;
                    }

                    ZnodeLogging.LogMessage(string.Format(Admin_Resources.ErrorCreatingLogForIndexCreationForPortalId, cmsPortalContentPageIndex.PortalId), ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                }
                else
                    cmsSearchIndexMonitor = CreateCMSPageSearchIndexMonitorEntry(cmsPortalContentPageIndex);

                //Start status for creating index for server name saved.  
                cmsSearchIndexServerStatusId = searchHelper.CreateCmsPageSearchIndexServerStatus(new CMSSearchIndexServerStatusModel()
                {
                    CMSSearchIndexMonitorId = cmsSearchIndexMonitor.CMSSearchIndexMonitorId,
                    ServerName = Environment.MachineName,
                    Status = ZnodeConstant.SearchIndexStartedStatus
                }).CMSSearchIndexServerStatusId;

                cmsPortalContentPageIndex.CMSSearchIndexMonitorId = cmsSearchIndexMonitor.CMSSearchIndexMonitorId;
                CreateCmsPageIndex(cmsPortalContentPageIndex.IndexName, cmsPortalContentPageIndex.RevisionType, cmsPortalContentPageIndex.PortalId, cmsSearchIndexMonitor.CMSSearchIndexMonitorId, cmsSearchIndexServerStatusId);
                ZnodeLogging.LogMessage("CmsPortalContentPageIndexModel with CMSSearchIndexId: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, cmsPortalContentPageIndex?.CMSSearchIndexId);
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            }
            else
            {
                cmsPortalContentPageIndex.IsDisabledCMSPageResults = true;
                ZnodeLogging.LogMessage(Admin_Resources.DisabledCMSPageResults, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
            return cmsPortalContentPageIndex;
        }

        //Rename the index
        public virtual bool RenameCmsPageIndex(int cmsSearchIndexId, string oldIndexName, string newIndexName)
        {
            ZnodeLogging.LogMessage("Rename index based of Cms Search on : ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { cmsSearchIndexId, oldIndexName, newIndexName });
            return cmsPageDefaultDataService.RenameCmsPageIndex(cmsSearchIndexId, oldIndexName, newIndexName);
        }

        //Create CMS page search index  
        public virtual void CreateCmsPageIndex(string indexName, string revisionType, int portalId, int cmsSearchIndexMonitorId, int cmsSearchIndexServerStatusId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage(string.Format(Admin_Resources.IndexingStarted, indexName), ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("revisionType, portalId, cmsSearchIndexMonitorId, cmsSearchIndexServerStatusId: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { revisionType, portalId, cmsSearchIndexMonitorId, cmsSearchIndexServerStatusId });
            try
            {
                long indexstartTime = DateTime.Now.Ticks;

                cmsPageDefaultDataService.IndexingDefaultData(indexName, new SearchCMSPagesParameterModel() { PortalId = portalId, IndexStartTime = indexstartTime, CMSSearchIndexMonitorId = cmsSearchIndexMonitorId, CMSSearchIndexServerStatusId = cmsSearchIndexServerStatusId, RevisionType = revisionType, ActiveLocales = GetPortalActiveLocals(portalId) });
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.IndexingStarted, indexName), ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

                //delete values from index before indexstartTime.
                DeleteCmsPagesDataByRevisionType(indexName, revisionType, indexstartTime);
            }
            catch (Exception ex)
            {
                SearchHelper searchHelper = new SearchHelper();
                searchHelper.UpdateCmsPageSearchIndexServerStatus(new CMSSearchIndexServerStatusModel() { CMSSearchIndexServerStatusId = cmsSearchIndexServerStatusId, CMSSearchIndexMonitorId = cmsSearchIndexMonitorId, ServerName = Environment.MachineName, Status = ZnodeConstant.SearchIndexFailedStatus });
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.ErrorIndexingForIndex, indexName, ex.Message), ZnodeLogging.Components.Search.ToString(), TraceLevel.Error, ex);
                throw;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
        }

        //Get list of Create index monitor of CMS page.
        public virtual CMSSearchIndexMonitorListModel GetCmsPageSearchIndexMonitorList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            CMSSearchIndexMonitorListModel cmsSearchIndexMonitorList;
            int cmsSearchIndexId = 0; 
            int portalId = 0;
            if (IsNotNull(filters))
            {
                FilterCollection indexFilters = new FilterCollection();
                indexFilters.Add(filters.FirstOrDefault(x => x.Item1.Equals(ZnodePortalEnum.PortalId.ToString().ToLower())));

                CMSPortalContentPageIndexModel cmsPortalIndexModel = GetCmsPagesIndexData(expands, indexFilters);
                cmsSearchIndexId = IsNotNull(cmsPortalIndexModel) ? cmsPortalIndexModel.CMSSearchIndexId : 0;

                filters.Add(new FilterTuple(ZnodeCMSSearchIndexMonitorEnum.CMSSearchIndexId.ToString(), FilterOperators.Equals, cmsSearchIndexId.ToString()));

                portalId = Convert.ToInt32(filters.Find(x => x.FilterName == ZnodePortalEnum.PortalId.ToString().ToLower())?.Item3);
                filters.RemoveAll(filter => filter.FilterName == ZnodePortalEnum.PortalId.ToString().ToLower());
            }
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            IZnodeViewRepository<CMSSearchIndexMonitorModel> objStoredProc = new ZnodeViewRepository<CMSSearchIndexMonitorModel>();

            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            ZnodeLogging.LogMessage("pageListModel to get cmsServerStatusList: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IList<CMSSearchIndexMonitorModel> cmsServerStatusList = objStoredProc.ExecuteStoredProcedureList("Znode_GetCreateCMSPageIndexServerStatus  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            cmsSearchIndexMonitorList = new CMSSearchIndexMonitorListModel { CMSSearchIndexMonitorList = cmsServerStatusList.ToList(), PortalId = portalId, CMSSearchIndexId = cmsSearchIndexId };
            cmsSearchIndexMonitorList.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("cmsSearchIndexMonitorList count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, cmsSearchIndexMonitorList?.CMSSearchIndexMonitorList?.Count);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return cmsSearchIndexMonitorList;
        }

        //Get CMS page index data.
        public virtual CMSPortalContentPageIndexModel GetCmsPagesIndexData(NameValueCollection expands, FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            PageListModel pageListModel = new PageListModel(filters, null, null);
            ZnodeLogging.LogMessage("WhereClause to get cms pages index data: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, pageListModel?.EntityWhereClause.WhereClause);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            return _cmsSearchIndexRepository.GetEntity(pageListModel.EntityWhereClause.WhereClause)?.ToModel<CMSPortalContentPageIndexModel, ZnodeCMSSearchIndex>();
        }

        //Delete unused pages from CMS page index.
        public virtual bool DeleteCmsPagesDataByRevisionType(string indexName, string revisionType, long indexstartTime)
            => cmsPageDefaultDataService.DeleteCmsPagesDataByRevisionType(indexName, revisionType, indexstartTime);

        //After content publish it creates index for CMS pages of store.
        public virtual void CreateIndexForPortalCMSPages(int portalId, string targetPublishState, bool IsFromStorePublish = false, string publishContent = null)
        {
            try
            {
                ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { portalId = portalId, revisionType = targetPublishState });

                if (IsCMSPageResultsFeatureEnabled(portalId) && (IsNull(publishContent) || (publishContent?.Contains(ZnodePublishContentTypeEnum.CmsContent.ToString())).GetValueOrDefault()))
                {
                    FilterCollection filter = new FilterCollection() { new FilterTuple(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()) };

                    CMSPortalContentPageIndexModel cmsPortalContentPageIndex = GetCmsPagesIndexData(null, filter);
                    if (cmsPortalContentPageIndex?.CMSSearchIndexId > 0)
                    {
                        cmsPortalContentPageIndex.CreatedBy = GetLoginUserId();
                        cmsPortalContentPageIndex.ModifiedBy = GetLoginUserId();
                        cmsPortalContentPageIndex.RevisionType = targetPublishState;
                        cmsPortalContentPageIndex.IsFromStorePublish = IsFromStorePublish;

                        InsertCreateCmsPageIndexDataByRevisionTypes(cmsPortalContentPageIndex);

                        if (!CheckCMSPageIndexCreationSucceed(Convert.ToInt32(cmsPortalContentPageIndex?.CMSSearchIndexMonitorId)))
                            throw new ZnodeException(ErrorCodes.CreationFailed, "Create CMS Pages Index failed.");
                        ZnodeLogging.LogMessage(string.Format(PIM_Resources.SuccessCreateIndexForPortal, portalId), ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                    }
                    else
                    {
                        CMSPortalContentPageIndexModel cmsPortalContentPageIndexModel = InsertCreateCmsPageIndexDataByRevisionTypes(new CMSPortalContentPageIndexModel() { IndexName = GetCMSPageIndexName(portalId), PortalId = portalId, CreatedBy = GetLoginUserId(), ModifiedBy = GetLoginUserId(), RevisionType = targetPublishState, IsFromStorePublish = IsFromStorePublish });

                        if (!CheckCMSPageIndexCreationSucceed(Convert.ToInt32(cmsPortalContentPageIndexModel?.CMSSearchIndexMonitorId)))
                            throw new ZnodeException(ErrorCodes.CreationFailed, "Create CMS Pages Index failed.");
                        ZnodeLogging.LogMessage(string.Format(PIM_Resources.SuccessCreateIndexForPortal, portalId), ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                    }
                }
                else
                {
                    ZnodeLogging.LogMessage(Admin_Resources.DisabledCMSPageResults, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                }
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                throw;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.ErrorCreateIndexForPortal, portalId), ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
        }
        #endregion

        #region CMS page search request
        //Get CMS full text result from the keyword.
        public virtual CMSKeywordSearchModel FullTextContentPageSearch(CMSPageSearchRequestModel model, FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            if(IsNull(model))
            {
                throw new ZnodeException(ErrorCodes.NullModel, Api_Resources.ExceptionMessageSearchModelNull);
            }

            if (string.IsNullOrEmpty(model.Keyword))
            {
                throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.ExceptionMessageSearchKeyword);
            }

            //Create instace of ElasticSearchProvider 
            IZnodeSearchProvider searchProvider = GetService<IZnodeSearchProvider>();

            //Get store current version id.
            int? versionId = WebstoreVersionId.GetValueOrDefault();

            //Add store version id to filters.
            filters.Add(WebStoreEnum.VersionId.ToString().ToLower(), FilterOperators.Equals, Convert.ToString(versionId));

            //Get request model require to pass elasticsearch. #step 1
            IZnodeCMSPageSearchRequest searchRequest = GetZnodeCMSSearchRequest(model, filters);

            IZnodeCMSPageSearchResponse searchResponse = null;
           
            //Method to get search response. #step 2
            searchResponse = searchProvider.FullTextContentPageSearch(searchRequest);

            //Convert search response to keyword search model. #step 3
            CMSKeywordSearchModel searchResult = IsNotNull(searchResponse) ? GetCMSKeywordSearchModel(searchResponse) : new CMSKeywordSearchModel();

            //Logging activity
            ZnodeLogging.LogMessage($"CMS page keyword search : {model.Keyword.ToLower()}, CMS page full text query: {searchResponse.RequestBody} and CMS page count {searchResult.CMSPages?.Count}", ZnodeLogging.Components.Search.ToString(),TraceLevel.Info);
            ZnodeLogging.LogObject(typeof(KeywordSearchModel), searchResult, "cmsSearchResult");
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            return searchResult;
        }

        //Get search cms page count
        public virtual int GetSearchContentPageCount(SearchRequestModel model)
        {
            try
            {
                if (IsNull(model) || string.IsNullOrEmpty(model.Keyword))
                {
                    return 0;
                }

                FilterCollection filters = CreateFilterForCMSPages(model);

                //Create instace of ElasticSearchProvider 
                IZnodeSearchProvider searchProvider = GetService<IZnodeSearchProvider>();

                //Get request model require to pass elasticsearch. #step 1
                IZnodeCMSPageSearchRequest searchRequest = GetZnodeCMSSearchRequest(MapSearchRequestToCMSSearchRequest(model), filters);

                IZnodeCMSPageSearchResponse searchResponse = null;

                //Method to get search response. #step 2
                searchResponse = searchProvider.GetSearchContentPageCount(searchRequest);

                return searchResponse.TotalCMSPageCount;

            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                return 0;
            }
        }


        public CMSPageSearchRequestModel MapSearchRequestToCMSSearchRequest(SearchRequestModel model)
        {
            CMSPageSearchRequestModel csmPageSearchRequestModel = new CMSPageSearchRequestModel();

            csmPageSearchRequestModel.LocaleId = model.LocaleId;
            csmPageSearchRequestModel.PortalId = model.PortalId;
            csmPageSearchRequestModel.ProfileId = Convert.ToInt32(model.ProfileId);
            csmPageSearchRequestModel.PageNumber = Convert.ToInt32(model.PageNumber);
            csmPageSearchRequestModel.PageSize = Convert.ToInt32(model.PageSize);
            csmPageSearchRequestModel.Keyword = model.Keyword;

            return csmPageSearchRequestModel;
        }

        //Create znode cms pages search request.
        public IZnodeCMSPageSearchRequest GetZnodeCMSSearchRequest(CMSPageSearchRequestModel model, FilterCollection filters)
        {
            //Create instace of ElasticCMSPageSearchRequest 
            IZnodeCMSPageSearchRequest searchRequest = GetService<IZnodeCMSPageSearchRequest>();

            searchRequest.LocaleId = model.LocaleId;
            searchRequest.PortalId = model.PortalId;
            searchRequest.ProfileId = model.ProfileId;
            searchRequest.IndexName = GetCMSPagesIndexName(model.PortalId);
            searchRequest.PageFrom = (model.PageNumber == 0) ? 1 : model.PageNumber;
            searchRequest.PageSize = model.PageSize;
            searchRequest.SearchText = model.Keyword.Trim().ToLower();
            searchRequest.Operator = "and";

            //Bind filters which will require for the where condition in elastic search
            searchRequest.FilterDictionary = GetDefaultCMSPageSearchableFieldsFilter(filters);
            searchRequest.QueryClass = "MultiMatchQueryBuilder";

            //Searchable field pass to elasticsearch where it will search keyword in that pass field.
            searchRequest.SearchableAttribute = GetCMSPageSearchableAttribute();

            // Valid values can be assigned to this property to get results based on the configured source. 
            // Assigned an empty string to avoid collapse parameter in search request.
            searchRequest.Source = new string[] { };

            return searchRequest;
        }

        //Convert cms page search response to keyword search model.
        public CMSKeywordSearchModel GetCMSKeywordSearchModel(IZnodeCMSPageSearchResponse response)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            CMSKeywordSearchModel cmsKeywordSearchModel = new CMSKeywordSearchModel();

            string output = JsonConvert.SerializeObject(response.CMSPageDetails);

            List<SearchCMSPageModel> cmsPageList = JsonConvert.DeserializeObject<List<SearchCMSPageModel>>(output);

            cmsKeywordSearchModel.CMSPages = cmsPageList?.Count > 0 ? cmsPageList : new List<SearchCMSPageModel>();

            cmsKeywordSearchModel.TotalCMSPageCount = response.TotalCMSPageCount;

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            return cmsKeywordSearchModel;
        }


        //Get default and clause filter terms.
        public Dictionary<string, List<string>> GetDefaultCMSPageSearchableFieldsFilter(FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            Dictionary<string, List<string>> filterAndClause = new Dictionary<string, List<string>>();
 
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

        //Get filter list for Cms page search
        public virtual FilterCollection CreateFilterForCMSPages(SearchRequestModel model)
        {
            FilterCollection filters = new FilterCollection();

            //Get store current version id.
            int? versionId = WebstoreVersionId.GetValueOrDefault();
            int[] profileIds = { Convert.ToInt32(model.ProfileId), 0 };

            filters.Add(WebStoreEnum.VersionId.ToString().ToLower(), FilterOperators.Equals, Convert.ToString(versionId));
            filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, Convert.ToString(model.LocaleId));
            filters.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, Convert.ToString(model.PortalId));
            filters.Add(WebStoreEnum.IsActive.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue);
            filters.Add(WebStoreEnum.ProfileId.ToString(), FilterOperators.In, string.Join(",", profileIds));

            return filters;
        }

        #endregion

        #region Protected methods        
        //Insert into ZnodeCMSSearch indexMonitor.
        protected virtual ZnodeCMSSearchIndexMonitor CMSSearchIndexMonitorInsert(CMSPortalContentPageIndexModel cmsPortalContentPageIndex)
        {
            return _cmsSearchIndexMonitorRepository.Insert(new ZnodeCMSSearchIndexMonitor()
            {
                SourceId = 0,
                CMSSearchIndexId = cmsPortalContentPageIndex.CMSSearchIndexId,
                SourceType = "CreateIndex",
                SourceTransactionType = "INSERT",
                AffectedType = "CreateIndex",
                CreatedBy = cmsPortalContentPageIndex.CreatedBy,
                CreatedDate = cmsPortalContentPageIndex.CreatedDate,
                ModifiedBy = cmsPortalContentPageIndex.ModifiedBy,
                ModifiedDate = cmsPortalContentPageIndex.ModifiedDate
            });
        }

        //Create CMSPageSearchIndexMonitor entry
        protected virtual ZnodeCMSSearchIndexMonitor CreateCMSPageSearchIndexMonitorEntry(CMSPortalContentPageIndexModel cmsPortalContentPageIndex)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("PortalIndexModel with PortalIndexId: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, cmsPortalContentPageIndex?.CMSSearchIndexId);

            //Get CMS index data by id.
            CMSPortalContentPageIndexModel getCmsPageIndexDetail = _cmsSearchIndexRepository.Table.FirstOrDefault(x => x.CMSSearchIndexId == cmsPortalContentPageIndex.CMSSearchIndexId)?.ToModel<CMSPortalContentPageIndexModel>();

            //Check if same as earlier
            if (getCmsPageIndexDetail?.IndexName == cmsPortalContentPageIndex.IndexName)
            {
                return CMSSearchIndexMonitorInsert(cmsPortalContentPageIndex);
            }
            else
            {
                //Check if index name is already used by another store.
                string indexName = _cmsSearchIndexRepository.Table.FirstOrDefault(x => x.IndexName == cmsPortalContentPageIndex.IndexName)?.IndexName ?? string.Empty;

                if (!cmsPortalContentPageIndex.IsFromStorePublish)
                {
                    //Check if Duplicate IndexName Exist
                    IsDuplicateSearchIndexNameExist(indexName, cmsPortalContentPageIndex);
                }

                bool renameStatus = RenameCmsPageIndex(cmsPortalContentPageIndex.CMSSearchIndexId, getCmsPageIndexDetail?.IndexName, cmsPortalContentPageIndex.IndexName);

                if (renameStatus)
                    _cmsSearchIndexRepository.Update(new ZnodeCMSSearchIndex { CMSSearchIndexId = getCmsPageIndexDetail.CMSSearchIndexId, PortalId = getCmsPageIndexDetail.PortalId, IndexName = cmsPortalContentPageIndex.IndexName });

                else
                {
                    ZnodeCMSSearchIndexMonitor cmsSearchIndexMonitor;
                    cmsSearchIndexMonitor = CMSSearchIndexMonitorInsert(cmsPortalContentPageIndex);
                    SearchHelper searchHelper = new SearchHelper();

                    int cmsSearchIndexServerStatusId = 0;

                    cmsSearchIndexServerStatusId = searchHelper.CreateCmsPageSearchIndexServerStatus(new CMSSearchIndexServerStatusModel()
                    {
                        CMSSearchIndexMonitorId = cmsSearchIndexMonitor.CMSSearchIndexMonitorId,
                        ServerName = Environment.MachineName,
                        Status = ZnodeConstant.SearchIndexStartedStatus
                    }).CMSSearchIndexServerStatusId;

                    CreateCmsPageIndex(cmsPortalContentPageIndex.IndexName, cmsPortalContentPageIndex.RevisionType, cmsPortalContentPageIndex.PortalId, cmsSearchIndexMonitor.CMSSearchIndexMonitorId, cmsSearchIndexServerStatusId);

                    RenameCmsPageIndex(cmsPortalContentPageIndex.CMSSearchIndexId, getCmsPageIndexDetail.IndexName, cmsPortalContentPageIndex.IndexName);

                    _cmsSearchIndexRepository.Update(new ZnodeCMSSearchIndex { CMSSearchIndexId = getCmsPageIndexDetail.CMSSearchIndexId, PortalId = getCmsPageIndexDetail.PortalId, IndexName = cmsPortalContentPageIndex.IndexName });
                }
                return CMSSearchIndexMonitorInsert(cmsPortalContentPageIndex);
            }
        }

        //Method to check if Preview mode is on or not.
        protected virtual bool IsPreviewEnabled()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            bool iSPreviewEnabled = (from publishState in _publishStateMappingRepository.Table
                                     join PS in _publishStateRepository.Table on publishState.PublishStateId equals PS.PublishStateId
                                     where publishState.IsActive && PS.IsActive && publishState.IsEnabled && PS.PublishStateCode == ZnodePublishStatesEnum.PREVIEW.ToString()
                                     select publishState.IsEnabled).Any();

            ZnodeLogging.LogMessage("Is publish state preview is enabled : ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, iSPreviewEnabled);
            return iSPreviewEnabled;
        }

        //Get the active locals of portal
        protected virtual List<LocaleModel> GetPortalActiveLocals(int portalId)
        {
            List<LocaleModel> ActiveLocales = new List<LocaleModel>();
            if (portalId > 0)
            {
                IPortalService portalService = GetService<IPortalService>();

                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()));
                filters.Add(new FilterTuple(ZnodeLocaleEnum.IsActive.ToString(), FilterOperators.Equals, ZnodeConstant.One));

                ActiveLocales = portalService.LocaleList(null, filters, null, null)?.Locales;
            }

            return ActiveLocales;
        }

        //Check whether or not index creation completed or not. 
        protected virtual bool CheckCMSPageIndexCreationSucceed(int cmsSearchIndexMonitorId)
        {
            var cmsPageSearchIndexServerStatus = _cmsSearchIndexServerStatusRepository.Table.FirstOrDefault(x => x.CMSSearchIndexMonitorId == cmsSearchIndexMonitorId);
            if (!string.IsNullOrEmpty(cmsPageSearchIndexServerStatus?.Status) && !cmsPageSearchIndexServerStatus.Status.Equals("Complete", StringComparison.InvariantCultureIgnoreCase))
                return false;
            return true;
        }

        //Get CMS page indexname based on storename
        protected virtual string GetCMSPageIndexName(int portalId)
        {
            if (portalId > 0)
            {
                string storeCode = RegexQuery(_portalRepository.Table.FirstOrDefault(x => x.PortalId == portalId)?.StoreCode);
                return $"{storeCode.ToLower()}cmspageindex";
            }
            return string.Empty;
        }

        protected virtual string RegexQuery(string storeCode)
        {
            storeCode = Regex.Replace(storeCode, @"\s+", ""); // Regex to remove spaces 
            storeCode = Regex.Replace(storeCode, @"[^a-zA-Z0-9_]{1,255}\+", string.Empty); //Regex to have alphabets and numbers up to limit 255.
            storeCode = Regex.Replace(storeCode, @"[?!^#\/*?<>..|.]", string.Empty);//Regex to remove the special character that are mentioned .
            storeCode = Regex.Replace(storeCode, @"(?i)([?!^(+_)])", string.Empty); //Regex to remove the special character if they are at starting position.
            storeCode = storeCode.StartsWith("-") ? storeCode.Replace('-', ' ') : storeCode; //To remove '-'.
            return storeCode;
        }

        //Method to check if CMS Page Results feature is enable or disabled for a portal.
        protected virtual bool IsCMSPageResultsFeatureEnabled(int portalId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
          
            bool isCMSPageResultsEnabled = ((from feature in _portalFeatureRepository.Table
                                        join mapper in _portalFeatureMapperRepository.Table on feature.PortalFeatureId equals mapper.PortalFeatureId
                                        where feature.PortalFeatureName.ToLower() == StoreFeature.Enable_CMS_Page_Results.ToString().ToLower()
                                        && mapper.PortalId == portalId
                                        select (mapper.PortalFeatureMapperId))?.FirstOrDefault() ?? 0) > 0;

            ZnodeLogging.LogMessage("Is CMS page results feature is enabled : ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, isCMSPageResultsEnabled);
            return isCMSPageResultsEnabled;
        }

        #region CMS page search request

        //Get list of searchable attribute 
        protected virtual List<ElasticSearchAttributes> GetCMSPageSearchableAttribute()
        {
            List<ElasticSearchAttributes> elasticSearchAttributes = new List<ElasticSearchAttributes>();

            string searchableAttributes = DefaultGlobalConfigSettingHelper.CMSPageSearchableAttributes;

            if (!string.IsNullOrEmpty(searchableAttributes))
            {
                foreach (string attribute in searchableAttributes.Split(','))
                    elasticSearchAttributes.Add(new ElasticSearchAttributes { AttributeCode = attribute, BoostValue = null });
            }
            return elasticSearchAttributes;
        }

        //Get CMS page search index name from database
        protected virtual string GetCMSPageIndexNameFromDB(int portalId)
        {
            FilterCollection filter = new FilterCollection() { new FilterTuple(ZnodeCMSSearchIndexEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()) };
            EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());
            return _cmsSearchIndexRepository.GetEntity(whereClause.WhereClause)?.IndexName;
        }

        //Get CMS page search index name for portal
        protected virtual string GetCMSPagesIndexName(int portalId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("PortalId to get cms page index name : ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, portalId);

            string cacheKey = $"CMSPageIndexName_{portalId}";
            string indexName = Equals(HttpRuntime.Cache[cacheKey], null)
               ? GetCMSPageIndexNameFromDB(portalId)
               : ((string)HttpRuntime.Cache.Get(cacheKey));

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return indexName;
        }

        //Get CMS Portal ContentPage Index Model By PortalId
        protected virtual CMSPortalContentPageIndexModel GetCMSPortalContentPageIndexModelByPortalId(int portalId)
        {
            CMSPortalContentPageIndexModel cmsPortalContentPageIndex = _cmsSearchIndexRepository.Table.FirstOrDefault(x => x.PortalId == portalId)?.ToModel<CMSPortalContentPageIndexModel>();
            return cmsPortalContentPageIndex;
        }

        //Check if duplicate index name exist.
        protected void IsDuplicateSearchIndexNameExist(string indexName, CMSPortalContentPageIndexModel cmsPortalContentPageIndex)
        {
            if (!string.IsNullOrEmpty(indexName) || GetService<IDefaultDataService>().IsIndexExists(cmsPortalContentPageIndex.IndexName) || cmsPageDefaultDataService.IsIndexExists(cmsPortalContentPageIndex.IndexName))
                throw new ZnodeException(ErrorCodes.DuplicateSearchIndexName, Admin_Resources.ErrorIndexNameIsInUse);
        }
        #endregion
        #endregion
    }
}
