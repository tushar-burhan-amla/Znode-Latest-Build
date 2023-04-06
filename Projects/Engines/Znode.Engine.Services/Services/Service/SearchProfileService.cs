using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;

using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.ElasticSearch;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class SearchProfileService : BaseService, ISearchProfileService
    {
        #region Protected Variables
        protected readonly IZnodeRepository<ZnodeSearchProfile> _searchProfileRepository;
        protected readonly IZnodeRepository<ZnodeSearchProfileFeatureMapping> _searchProfileFeatureMappingRepository;
        protected readonly IZnodeRepository<ZnodeSearchQueryType> _searchQueryTypeRepository;
        protected readonly IZnodeRepository<ZnodePortalCatalog> _portalCatalogRepository;
        protected readonly IZnodeRepository<ZnodeSearchProfileAttributeMapping> _searchProfileAttributeMappingRepository;
        protected readonly IZnodeRepository<ZnodePortalSearchProfile> _portalSearchProfileMappingRepository;
        protected readonly IZnodeRepository<ZnodePublishCatalogSearchProfile> _catalogSearchProfileMappingRepository;
        protected readonly IZnodeRepository<ZnodeSearchProfileTrigger> _searchProfileTriggerRepository;
        protected readonly IZnodeRepository<ZnodeCatalogIndex> _catalogIndexRepository;

        protected readonly IPublishedCatalogDataService publishedCatalogDataService;
        protected readonly ISearchService searchService;
        #endregion

        #region Mapping character filter
        // To specify the mapping for a underscore, underscore will be replaced with the empty string at the time of preprocessing by the mapping character filter.
        private readonly string underscoreMapping = "_ => ";

        // To specify the mapping for a hyphen, hyphen will be replaced with the empty string at the time of preprocessing by the mapping character filter.
        private readonly string hyphenMapping = "- => ";
        #endregion Mapping character filter

        #region Constructor
        public SearchProfileService()
        {
            _searchProfileRepository = new ZnodeRepository<ZnodeSearchProfile>();
            _searchProfileFeatureMappingRepository = new ZnodeRepository<ZnodeSearchProfileFeatureMapping>();
            _searchQueryTypeRepository = new ZnodeRepository<ZnodeSearchQueryType>();
            _portalCatalogRepository = new ZnodeRepository<ZnodePortalCatalog>();
            _searchProfileAttributeMappingRepository = new ZnodeRepository<ZnodeSearchProfileAttributeMapping>();
            _portalSearchProfileMappingRepository = new ZnodeRepository<ZnodePortalSearchProfile>();
            _catalogSearchProfileMappingRepository = new ZnodeRepository<ZnodePublishCatalogSearchProfile>();
            _searchProfileTriggerRepository = new ZnodeRepository<ZnodeSearchProfileTrigger>();
            _catalogIndexRepository = new ZnodeRepository<ZnodeCatalogIndex>();
            publishedCatalogDataService = GetService<IPublishedCatalogDataService>();
            searchService = GetService<ISearchService>();

        }
        #endregion

        #region Public Methods
        //gets search Profiles list
        public virtual SearchProfileListModel GetSearchProfileList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            //set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            IZnodeViewRepository<SearchProfileModel> objStoredProc = new ZnodeViewRepository<SearchProfileModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            ZnodeLogging.LogMessage("pageListModel to get searchProfileList: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IList<SearchProfileModel> searchProfileList = objStoredProc.ExecuteStoredProcedureList("Znode_GetSearchProfileList @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("searchProfileList count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, searchProfileList?.Count);
            SearchProfileListModel profileListModel = new SearchProfileListModel();
            profileListModel.SearchProfileList = searchProfileList?.Count > 0 ? searchProfileList?.ToList() : null;

            profileListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return profileListModel;
        }

        //Get search profile portal list
        public virtual SearchProfilePortalListModel SearchProfilePortalList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            IZnodeViewRepository<SearchProfilePortalModel> objStoredProc = new ZnodeViewRepository<SearchProfilePortalModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            ZnodeLogging.LogMessage("pageListModel to get searchProfileList: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IList<SearchProfilePortalModel> searchProfileList = objStoredProc.ExecuteStoredProcedureList("Znode_GetPortalSearchProfile @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("searchProfileList count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, searchProfileList?.Count);
            SearchProfilePortalListModel profileListModel = new SearchProfilePortalListModel();
            profileListModel.SearchProfilePortalList = searchProfileList?.Count > 0 ? searchProfileList?.ToList() : new List<SearchProfilePortalModel>();

            profileListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return profileListModel;
        }

        //Associates portal to search profile
        public virtual bool AssociatePortalToSearchProfile(SearchProfileParameterModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            bool status = false;
            if (model.IsAssociate)
            {
                DataTable portalIdsData = GetPortalIdInDataTable(model.Ids);
                ExecuteSpHelper objStoredProc = new ExecuteSpHelper();
                objStoredProc.GetParameter("@SearchProfileId", model.SearchProfileId, ParameterDirection.Input, SqlDbType.Int);
                objStoredProc.GetParameter("@UserId", GetLoginUserId(), ParameterDirection.Input, SqlDbType.Int);
                ZnodeLogging.LogMessage("SP parameters - SearchProfileId and LoginUserId: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { model?.SearchProfileId, GetLoginUserId() });
                DataSet dataSet = null;
                if (DefaultGlobalConfigSettingHelper.IsColumnEncryptionSettingEnabled)
                {
                    objStoredProc.GetParameter("@UserPortalList", portalIdsData?.ToJson(), ParameterDirection.Input, SqlDbType.NVarChar);
                    dataSet = objStoredProc.GetSPResultInDataSet("Znode_InsertUpdatePortalSearchProfileWithJSON");
                }
                else
                {
                    objStoredProc.SetTableValueParameter("@UserPortalList", portalIdsData, ParameterDirection.Input, SqlDbType.Structured,"dbo.TransferId");
                    dataSet = objStoredProc.GetSPResultInDataSet("Znode_InsertUpdatePortalSearchProfile");
                }
                DataTable result = dataSet.Tables[0];

                foreach (DataRow row in result.Rows)
                {
                    status = Convert.ToBoolean(row["Status"]);
                }
            }
            else
            {
                FilterCollection filter = new FilterCollection();
                filter.Add(ZnodePortalSearchProfileEnum.PortalSearchProfileId.ToString(), FilterOperators.In, model.PortalSearchProfileIds);
                filter.Add(new FilterTuple(ZnodePortalSearchProfileEnum.IsDefault.ToString(), ProcedureFilterOperators.Equals, FilterKeys.ActiveTrueValue));

                ZnodeLogging.LogMessage("PortalSearchProfileIds to get defaultPortalProfileIds list: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, model?.PortalSearchProfileIds);
                IEnumerable<int> defaultPortalProfileIds = _portalSearchProfileMappingRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause)?.Select(x => x.PortalSearchProfileId);
                ZnodeLogging.LogMessage("defaultPortalProfileIds list count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, defaultPortalProfileIds?.Count());
                filter.RemoveAll(x => string.Equals(x.FilterName, ZnodePortalSearchProfileEnum.IsDefault.ToString(), StringComparison.CurrentCultureIgnoreCase));
                if (defaultPortalProfileIds?.Count() > 0)
                {
                    List<int> portalProfileIds = model.Ids.Split(',').Select(int.Parse)?.AsEnumerable().Except(defaultPortalProfileIds)?.ToList();
                    if (portalProfileIds?.Count() > 0)
                    {
                        FilterCollection finalFilter = new FilterCollection();
                        finalFilter.Add(ZnodePortalSearchProfileEnum.PortalSearchProfileId.ToString(), ProcedureFilterOperators.In, string.Join(",", portalProfileIds));

                        ZnodeLogging.LogMessage("Portal profiles with Ids to be deleted: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, portalProfileIds);
                        status = _portalSearchProfileMappingRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);   
                    }
                    //Default profiles delete exception.
                    ZnodeLogging.LogMessage(Admin_Resources.ErrorDefaultAssociatedPortalsDelete, ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                    throw new ZnodeException(ErrorCodes.DefaultDataDeletionError, Admin_Resources.ErrorDefaultAssociatedPortalsDelete);
                }
                status = _portalSearchProfileMappingRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return status;
        }

        //Get search profile by its Id
        public virtual SearchProfileModel GetSearchProfile(int searchProfileId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            if (searchProfileId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorIdLessThanOne);

            ExecuteSpHelper objStoredProc = new ExecuteSpHelper();
            objStoredProc.GetParameter("@SearchProfileId", searchProfileId, ParameterDirection.Input, SqlDbType.Int);

            ZnodeLogging.LogMessage("searchProfileId to get search profile: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, searchProfileId);
            DataSet searchProfile = objStoredProc.GetSPResultInDataSet("Znode_GetSearchProfileDetails");

            SearchProfileModel searchProfileModel = new SearchProfileModel();
            searchProfileModel.SearchProfileId = searchProfileId;

            //binds required details from dataset to search profile model
            BindSearchProfileDetailsFromDataSet(searchProfile, searchProfileModel);

            //Get Fields For Search Profile
            GetFieldValueList(searchProfileModel.PublishCatalogId, searchProfileModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchProfileModel;
        }

        //creates search profile 
        public virtual SearchProfileModel Create(SearchProfileModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelCanNotBeNull);

            View_ReturnBooleanWithMessage SearchProfile = InsertUpdateSearchProfile(model);
            ZnodeLogging.LogMessage(IsNotNull(SearchProfile) ? Admin_Resources.SuccessSearchProfileCreate : Admin_Resources.ErrorSearchProfileCreate, ZnodeLogging.Components.Search.ToString());
            model.SearchProfileId = SearchProfile.Id;

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return model;
        }

        //Update Search Profile.
        public virtual bool UpdateSearchProfile(SearchProfileModel searchProfileModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            if (IsNull(searchProfileModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (searchProfileModel.SearchProfileId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.IdCanNotBeLessThanOne);

            View_ReturnBooleanWithMessage SearchProfile = InsertUpdateSearchProfile(searchProfileModel);
            ZnodeLogging.LogMessage(SearchProfile != null ? string.Format(Admin_Resources.SuccessSearchProfileUpdate, searchProfileModel.SearchProfileId) : Admin_Resources.ErrorSearchProfileUpdate, ZnodeLogging.Components.Search.ToString());
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return (bool)SearchProfile?.Status.GetValueOrDefault();
        }

        //Delete SearchProfile  by searchProfileId.
        public virtual bool DeleteSearchProfile(ParameterModel searchProfile)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            bool status = false;
            if (IsNull(searchProfile) || searchProfile?.Ids?.Count() < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorIdLessThanOne);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeSearchProfileEnum.SearchProfileId.ToString(), ProcedureFilterOperators.In, searchProfile?.Ids?.ToString()));
            filters.Add(new FilterTuple(ZnodePortalSearchProfileEnum.IsDefault.ToString(), ProcedureFilterOperators.Equals, FilterKeys.ActiveTrueValue));

            filters.RemoveAll(x => string.Equals(x.FilterName, ZnodePortalSearchProfileEnum.IsDefault.ToString(), StringComparison.CurrentCultureIgnoreCase));

            List<int> searchProfileIds = searchProfile?.Ids?.Split(',')?.Select(int.Parse)?.ToList();
            if (searchProfileIds?.Count > 0)
            {
                foreach (int searchProfileId in searchProfileIds)
                {
                    FilterCollection finalFilter = new FilterCollection();
                    finalFilter.Add(ZnodeSearchProfileEnum.SearchProfileId.ToString(), ProcedureFilterOperators.In, string.Join(",", searchProfileIds));
                    int publishCatalogId = (_catalogSearchProfileMappingRepository.Table.FirstOrDefault(x => x.SearchProfileId == searchProfileId)?.PublishCatalogId).GetValueOrDefault();

                    ZnodeLogging.LogMessage("Search profile with Ids to be deleted: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, searchProfileIds);
                    status = DeleteSearchProfiles(searchProfileId);
                    
                    if (status && searchProfile.IsDeletePublishSearchProfile)
                    {
                        ZnodeCatalogIndex catalogIndex = _catalogIndexRepository.Table.FirstOrDefault(x => x.PublishCatalogId == publishCatalogId);

                        GetService<ISearchService>().InsertCreateIndexDataByRevisionTypes(new PortalIndexModel()
                        {
                            IndexName = catalogIndex.IndexName,
                            RevisionType = ZnodePublishStatesEnum.PRODUCTION.ToString(),
                            PublishCatalogId = publishCatalogId,
                            DirectCalling = true,
                            CatalogIndexId = catalogIndex.CatalogIndexId,
                            NewIndexName = searchService.UpdateIndexNameWithTimestamp(catalogIndex.IndexName)
                        });
                    }
                }
            }

            ZnodeLogging.LogMessage("Search profile with Ids to be deleted: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, searchProfile?.Ids.ToString());
            ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessSearchProfileDelete : Admin_Resources.ErrorSearchProfileDelete, ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return status;
        }

        //Get details for creating search profile
        public virtual SearchProfileModel GetSearchProfileDetails(FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            SearchProfileModel searchProfileModel = new SearchProfileModel();
            // The excludedQueryTypes variable contains the list of search query types which should not be displayed on the search profile screen.
            List<string> excludedQueryTypes = searchService.GetAllSearchQueryTypeList();
            List<ZnodeSearchQueryType> queryTypes = _searchQueryTypeRepository.Table
                                                    ?.Where(x => !excludedQueryTypes.Contains(x.QueryTypeName.Replace(" ", "").ToLower()))
                                                    ?.OrderBy(x => x.DisplayOrder).ToList();
            ZnodeLogging.LogMessage("SearchQueryType list count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, queryTypes?.Count);

            ZnodeSearchQueryType queries = queryTypes.FirstOrDefault(x => string.Equals(x.QueryTypeName.Replace(" ", ""), ZnodeConstant.MultiMatchCross, StringComparison.InvariantCultureIgnoreCase));
            searchProfileModel.SearchQueryTypeId = queries.ParentSearchQueryTypeId ?? 0;
            searchProfileModel.SearchSubQueryTypeId = queries.SearchQueryTypeId;
            searchProfileModel.QueryTypeName = queries.QueryTypeName;
            //returns Feature List depending on Query Selected 
            searchProfileModel.FeaturesList = GetFeaturesByQueryId(queries.ParentSearchQueryTypeId ?? 0);
            if (searchProfileModel.FeaturesList.Count() > 0)
            {
                GetDefaultValueForCharacterFilter(searchProfileModel);
                GetDefaultValueForNgramSettings(searchProfileModel);
            }                

            searchProfileModel.QueryTypes = queryTypes?.ToModel<SearchQueryTypeModel>().ToList();

            return searchProfileModel;
        }

        // To assign default values for character filters.
        protected virtual void GetDefaultValueForCharacterFilter(SearchProfileModel searchProfileModel)
        {
            SearchFeatureModel characterFilter = searchProfileModel?.FeaturesList?.FirstOrDefault(x => string.Equals(x.FeatureCode ,ZnodeConstant.CharacterFilter));
            characterFilter.SearchFeatureValue = characterFilter?.SearchFeatureValue ?? underscoreMapping + "," + hyphenMapping;
        }

        // To assign default values for Ngram settings
        protected virtual void GetDefaultValueForNgramSettings(SearchProfileModel searchProfileModel)
        {
            SearchFeatureModel minGramFeature = searchProfileModel?.FeaturesList?.FirstOrDefault(x => string.Equals(x.FeatureCode, ZnodeConstant.MinGram));
            minGramFeature.SearchFeatureValue = minGramFeature?.SearchFeatureValue ?? ZnodeConstant.ngramMinimumTokenLength.ToString();
            
            SearchFeatureModel maxGramFeature = searchProfileModel?.FeaturesList?.FirstOrDefault(x => string.Equals(x.FeatureCode, ZnodeConstant.MaxGram));
            maxGramFeature.SearchFeatureValue = maxGramFeature?.SearchFeatureValue ?? ZnodeConstant.ngramMaximumTokenLength.ToString();
        }

        //Get Field value list by catalog id.
        public virtual SearchProfileModel GetFieldValuesList(int publishCatalogId, int searchProfileId)
        {
            SearchProfileModel model = new SearchProfileModel();

            GetFieldValueList(publishCatalogId, model);

            return model;
        }

        //Set default search profile.
        public virtual bool SetDefaultSearchProfile(PortalSearchProfileModel portalSearchProfileModel)
        {
            if (portalSearchProfileModel?.SearchProfileId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorIdLessThanOne);

            bool status = false;

            ZnodePortalSearchProfile portalSearchProfileEntity = _portalSearchProfileMappingRepository.Table.FirstOrDefault(x => x.SearchProfileId == portalSearchProfileModel.SearchProfileId && x.PortalId == portalSearchProfileModel.PortalId && x.PublishCatalogId == portalSearchProfileModel.PublishCatalogId);
            if (IsNotNull(portalSearchProfileEntity))
            {
                portalSearchProfileEntity.IsDefault = true;
                List<ZnodePortalSearchProfile> znodePortalSearchProfiles = _portalSearchProfileMappingRepository.Table.Where(x => x.IsDefault && x.PublishCatalogId == portalSearchProfileModel.PublishCatalogId && x.PortalId == portalSearchProfileModel.PortalId)?.ToList();
                if (znodePortalSearchProfiles?.Count() > 0)
                {
                    foreach (ZnodePortalSearchProfile znodeProfileEntity in znodePortalSearchProfiles)
                    {
                        znodeProfileEntity.IsDefault = false;
                        status = _portalSearchProfileMappingRepository.Update(znodeProfileEntity);
                    }
                }
                else
                    status = true;

                if (status)
                    status = _portalSearchProfileMappingRepository.Update(portalSearchProfileEntity);
            }
            ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessDefaultProfileUpdate : Admin_Resources.ErrorDefaultProfileUpdate, ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            return status;
        }

        //returns Feature List depending on Query Selected 
        public virtual List<SearchFeatureModel> GetFeaturesByQueryId(int queryId, int searchProfileId = 0)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            IZnodeViewRepository<SearchFeatureModel> objStoredProc = new ZnodeViewRepository<SearchFeatureModel>();
            objStoredProc.SetParameter("@SearchProfileId", searchProfileId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@SearchQueryTypeId", queryId, ParameterDirection.Input, DbType.Int32);

            ZnodeLogging.LogMessage("searchProfileId and queryId to get FeaturesList: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { searchProfileId, queryId });
            List<SearchFeatureModel> featuresList = objStoredProc.ExecuteStoredProcedureList("Znode_GetSearchQueryTypeWiseFeatureDetails @SearchProfileId,@SearchQueryTypeId").ToList();
            // The excludeSearchFeatures variable contains the list of search features which should not be displayed on the search profile screen.
            List<string> excludedFeaturesList = searchService.GetExcludedSearchFeatureList();
            featuresList?.RemoveAll(x => excludedFeaturesList.Contains(x.FeatureCode.ToLower()));
            ZnodeLogging.LogMessage("FeaturesList count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, featuresList?.Count);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return featuresList;
        }

        //Get all Attributes Codes where IsSearchable Flag is true 
        public virtual SearchAttributesListModel GetCatalogBasedSearchableAttributes(ParameterModel associatedAttributes, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            int publishCatalogId = Convert.ToInt32(filters?.Find(x => string.Equals(x.FilterName, FilterKeys.PublishCatalogId, StringComparison.CurrentCultureIgnoreCase))?.Item3);
            filters?.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.PublishCatalogId, StringComparison.CurrentCultureIgnoreCase));

            //set paging parameters.
            ReplaceFilterKeys(ref filters);
            ReplaceSortKeys(ref sorts);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            filters.Add("IsUseInSearch", FilterOperators.Equals, ZnodeConstant.TrueValue);
            filters.Add("AttributeTypeName", FilterOperators.NotEquals, $"\"{ZnodeConstant.NumberType}\"");

            List<ZnodePublishCatalogAttributeEntity> Attributes = GetAttributesFromQuery(publishCatalogId, associatedAttributes, sorts, page, filters);
            ZnodeLogging.LogMessage("Attributes list: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, Attributes?.Count);
            SearchAttributesListModel searchableAttributes = new SearchAttributesListModel() { SearchAttributeList = Attributes?.ToModel<SearchAttributesModel>().ToList() };

            if(IsNotNull(Attributes))
                pageListModel.TotalRowCount = Attributes.Count;
            searchableAttributes.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchableAttributes;
        }

        //Gets unassociated portal List
        public virtual PortalListModel GetUnAssociatedPortalList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            int searchProfileId = Convert.ToInt32(filters?.Find(x => string.Equals(x.FilterName, ZnodePortalSearchProfileEnum.SearchProfileId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Item3);
            FilterCollection ProfileIdFilter = new FilterCollection() { new FilterTuple(ZnodePortalSearchProfileEnum.SearchProfileId.ToString(), FilterOperators.Equals, searchProfileId.ToString()) };
            List<int> SearchProfilePortals = SearchProfilePortalList(null, ProfileIdFilter, null, null)?.SearchProfilePortalList?.Select(x => x.PortalId)?.Distinct().ToList();
            filters?.RemoveAll(x => string.Equals(x.FilterName, ZnodePortalSearchProfileEnum.SearchProfileId.ToString(), StringComparison.CurrentCultureIgnoreCase));
            if (SearchProfilePortals?.Count() > 0)
            {
                string associatedPortals = string.Join(",", SearchProfilePortals);
                filters?.Add(new FilterTuple(FilterKeys.PortalId.ToString(), FilterOperators.NotIn, associatedPortals));
            }

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            IZnodeViewRepository<PortalModel> objStoredProc = new ZnodeViewRepository<PortalModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            ZnodeLogging.LogMessage("pageListModel and LoginUserId to get publishPortalLogs list: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString(), GetLoginUserId() });
            List<PortalModel> publishPortalLogs = objStoredProc.ExecuteStoredProcedureList("Znode_GetStoreList @WhereClause,@Rows,@PageNo,@Order_By,@UserId,@RowCount OUT", 5, out pageListModel.TotalRowCount)?.ToList();
            ZnodeLogging.LogMessage("publishPortalLogs list count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, publishPortalLogs?.Count);

            PortalListModel unAssociatedPortalList = new PortalListModel { PortalList = publishPortalLogs };
            unAssociatedPortalList.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return unAssociatedPortalList;
        }

        #region Search Triggers
        //Gets List of Search Profiles Triggers
        public virtual SearchTriggersListModel GetSearchTriggersList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            //set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            IZnodeViewRepository<SearchTriggersModel> objStoredProc = new ZnodeViewRepository<SearchTriggersModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            ZnodeLogging.LogMessage("pageListModel to get searchProfileTriggerList: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IList<SearchTriggersModel> searchProfileTriggerList = objStoredProc.ExecuteStoredProcedureList("Znode_GetSearchProfileTrigger @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("searchProfileTriggerList count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, searchProfileTriggerList?.Count);
            SearchTriggersListModel searchTriggersListModel = new SearchTriggersListModel();
            searchTriggersListModel.SearchTriggersList = searchProfileTriggerList?.Count > 0 ? searchProfileTriggerList?.ToList() : null;

            searchTriggersListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchTriggersListModel;
        }

        //Gets search profile triggers based on provided 
        public virtual SearchTriggersModel GetSearchTrigger(int searchProfileTriggerId)
           => searchProfileTriggerId > 0 ? _searchProfileTriggerRepository.GetById(searchProfileTriggerId)?.ToModel<SearchTriggersModel>() : new SearchTriggersModel();

        //Create Search Profile triggers
        public virtual bool CreateSearchTriggers(SearchTriggersModel searchTriggersModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            if (IsNull(searchTriggersModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelCanNotBeNull);

            IZnodeViewRepository<SearchTriggersModel> objStoredProc = new ZnodeViewRepository<SearchTriggersModel>();
            objStoredProc.SetParameter("@SearchProfileId", searchTriggersModel.SearchProfileId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@IsConfirmation", searchTriggersModel.IsConfirmation, ParameterDirection.Input, DbType.Boolean);

            DataTable keywordList = ConvertKeywordListToDataTable(searchTriggersModel.Keyword);
            DataTable userProfileList = ConvertProfileArrayToDataTable(searchTriggersModel.ProfileIds);
            if (DefaultGlobalConfigSettingHelper.IsColumnEncryptionSettingEnabled)
            {
                objStoredProc.SetParameter("@KeywordList", keywordList?.ToJson(), ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@UserProfileList", userProfileList?.ToJson(), ParameterDirection.Input, DbType.String);
                searchTriggersModel = objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdateSearchProfileTriggerWithJSON @SearchProfileId,@KeywordList,@UserProfileList,@UserId,@IsConfirmation")?.FirstOrDefault();
            }
            else
            {
                objStoredProc.SetTableValueParameter("@KeywordList", keywordList, ParameterDirection.Input, SqlDbType.Structured, "dbo.SelectColumnList");
                objStoredProc.SetTableValueParameter("@UserProfileList", userProfileList, ParameterDirection.Input, SqlDbType.Structured, "dbo.TransferId");
                searchTriggersModel = objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdateSearchProfileTrigger @SearchProfileId,@KeywordList,@UserProfileList,@UserId,@IsConfirmation")?.FirstOrDefault();
            }
            ZnodeLogging.LogMessage("SP parameters: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { searchTriggersModel?.SearchProfileId, GetLoginUserId(), searchTriggersModel?.IsConfirmation});
            if (searchTriggersModel?.Status == false)
                throw new ZnodeException(ErrorCodes.DuplicateQuantityError, Admin_Resources.ErrorDuplicateUserProfileAndKeywordCombinationAdd);
            else
                return true;
        }

        //Deletes search profile triggers.
        public virtual bool DeleteSearchTriggers(ParameterModel searchProfileTriggerId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(searchProfileTriggerId?.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorIdLessThanOne);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeSearchProfileTriggerEnum.SearchProfileTriggerId.ToString(), ProcedureFilterOperators.In, searchProfileTriggerId?.Ids?.ToString()));

            ZnodeLogging.LogMessage("searchProfileTriggerIds to be deleted: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, searchProfileTriggerId?.Ids?.ToString());
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return _searchProfileTriggerRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
        }

        //Updates search profile triggers.
        public virtual bool UpdateSearchTriggers(SearchTriggersModel searchTriggersModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            bool status = false;
            if (IsNull(searchTriggersModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelCanNotBeNull);

            if (searchTriggersModel?.SearchProfileTriggerId > 0)
            {
                FilterCollection filters = new FilterCollection();
                filters.Add(ZnodeSearchProfileTriggerEnum.SearchProfileTriggerId.ToString(), FilterOperators.NotEquals, searchTriggersModel.SearchProfileTriggerId.ToString());
                if (!string.IsNullOrEmpty(searchTriggersModel.UserProfile))
                {
                    filters.Add(ZnodeSearchProfileTriggerEnum.SearchProfileId.ToString(), FilterOperators.Equals, searchTriggersModel.UserProfile);
                    searchTriggersModel.ProfileId = Convert.ToInt32(searchTriggersModel.UserProfile);
                }

                if (!string.IsNullOrEmpty(searchTriggersModel.Keyword))
                    filters.Add(ZnodeSearchProfileTriggerEnum.Keyword.ToString(), FilterOperators.Is, searchTriggersModel.Keyword);

                EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

                //Check whether already exists.
                ZnodeSearchProfileTrigger znodeSearchProfileTrigger = _searchProfileTriggerRepository.GetEntity(whereClause.WhereClause, whereClause.FilterValues);

                if (IsNotNull(znodeSearchProfileTrigger))
                    throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.TriggerAlreadyExists);
                else
                    status = _searchProfileTriggerRepository.Update(searchTriggersModel.ToEntity<ZnodeSearchProfileTrigger>());
            }
            ZnodeLogging.LogMessage(status ? string.Format(Admin_Resources.SuccessProfileTriggerUpdate, searchTriggersModel.SearchProfileTriggerId) : Admin_Resources.ErrorProfileTriggerUpdate, ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return status;
        }
        #endregion

        #region Search Facets
        //Gets List of Search Attributes.
        public virtual SearchAttributesListModel GetAssociatedUnAssociatedCatalogAttributes(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ReplaceSortKeyName(ref sorts, FilterKeys.AttributeName.ToLower(), FilterKeys.AttributeCode);

            string isAssociated = string.Empty;
            string attributeName = string.Empty;
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            SearchAttributesListModel listModel = new SearchAttributesListModel();

            int searchProfileId = Convert.ToInt32(filters.Find(x => string.Equals(x.FilterName, ZnodeSearchProfileEnum.SearchProfileId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.FilterValue);
            if (searchProfileId > 0)
            {
                //Get isAssociated and attributeName from filters.
                GetFilterValues(filters, ref isAssociated, ref attributeName);

                int publishCatalogId = (_catalogSearchProfileMappingRepository.Table.FirstOrDefault(x => x.SearchProfileId == searchProfileId)?.PublishCatalogId).GetValueOrDefault();

                //Get associated attributes.
                ZnodeLogging.LogMessage("pageListModel and attributeName to get SearchAttributeList: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString(), attributeName });
                listModel.SearchAttributeList = GetSearchAttributeList(filters, pageListModel, attributeName);

                //If isAssociated is false get unassociated attribute values.
                if (!string.IsNullOrEmpty(isAssociated) && !Convert.ToBoolean(isAssociated) && publishCatalogId > 0)
                    listModel.SearchAttributeList = GetPublishAttributes(filters, listModel.SearchAttributeList, pageListModel, attributeName, publishCatalogId);
                else
                {
                    string attributeCodes = string.Join(",", listModel.SearchAttributeList.Select(x => x.AttributeCode).Select(x => $"\"{x}\""));
                    FilterCollection filter = new FilterCollection();
                    filter.Add("AttributeCode", FilterOperators.In, attributeCodes);
                    listModel.SearchAttributeList = !string.IsNullOrEmpty(attributeCodes) ? MapAttributeNames(publishCatalogId, listModel.SearchAttributeList, filter) : listModel.SearchAttributeList ;
                }
                ZnodeLogging.LogMessage("SearchAttributeList count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, listModel?.SearchAttributeList?.Count);
            }
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Associate UnAssociated search attributes to search profile.
        public virtual bool AssociateAttributesToProfile(SearchAttributesModel searchAttributesModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            if (IsNull(searchAttributesModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorSearchAttributesModelNull);

            ZnodeLogging.LogMessage("SearchProfileId and attributeCode: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { searchAttributesModel?.SearchProfileId, searchAttributesModel?.AttributeCode });
            List<ZnodeSearchProfileAttributeMapping> searchAttributesInsert = new List<ZnodeSearchProfileAttributeMapping>();
            List<ZnodeSearchProfileAttributeMapping> searchAttributesUpdate = new List<ZnodeSearchProfileAttributeMapping>();

            List<ZnodeSearchProfileAttributeMapping> znodeSearchProfileAttributeMappings = _searchProfileAttributeMappingRepository.Table.Where(x => searchAttributesModel.AttributeCode.Contains(x.AttributeCode) && x.SearchProfileId.Equals(searchAttributesModel.SearchProfileId)).ToList();

            if (IsNotNull(znodeSearchProfileAttributeMappings))
            {
                foreach (string attributeCode in searchAttributesModel.AttributeCode.Split(','))
                {
                    ZnodeSearchProfileAttributeMapping znodeSearchProfileAttribute = znodeSearchProfileAttributeMappings.FirstOrDefault(x => x.AttributeCode.Equals(attributeCode));
                    if (IsNotNull(znodeSearchProfileAttribute))
                    {
                        znodeSearchProfileAttribute.IsFacets = true;
                        searchAttributesUpdate.Add(znodeSearchProfileAttribute);
                    }
                    else
                        searchAttributesInsert.Add(new ZnodeSearchProfileAttributeMapping { SearchProfileId = searchAttributesModel.SearchProfileId, AttributeCode = attributeCode, IsFacets = true });

                }
            }
            else
            {
                foreach (string attributeCode in searchAttributesModel.AttributeCode.Split(','))
                {
                    searchAttributesInsert.Add(new ZnodeSearchProfileAttributeMapping { SearchProfileId = searchAttributesModel.SearchProfileId, AttributeCode = attributeCode, IsFacets = true });
                }
            }

            bool isSuccessfull = false;

            if (searchAttributesInsert.Count() > 0)
                isSuccessfull = _searchProfileAttributeMappingRepository.Insert(searchAttributesInsert)?.Count() > 0;

            if (searchAttributesUpdate.Count() > 0)
                isSuccessfull = _searchProfileAttributeMappingRepository.BatchUpdate(searchAttributesUpdate);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            return isSuccessfull;
        }

        //UnAssociate each attributes from search profile.
        public virtual bool UnAssociateAttributesFromProfile(ParameterModel searchProfilesAttributeMappingIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(searchProfilesAttributeMappingIds?.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorSearchProfilesAttributeMappingIdsLessThanOne);

            FilterCollection filter = new FilterCollection();

            List<ZnodeSearchProfileAttributeMapping> searchAttributesUpdate = new List<ZnodeSearchProfileAttributeMapping>();
            string searchProfilesAttributeMappingIdCollection = null;

            List<ZnodeSearchProfileAttributeMapping> znodeSearchProfileAttributeMappings = _searchProfileAttributeMappingRepository.Table.Where(x => searchProfilesAttributeMappingIds.Ids.Contains(x.SearchProfileAttributeMappingId.ToString())).ToList();

            foreach (string searchProfilesAttributeMappingId in searchProfilesAttributeMappingIds.Ids.Split(','))
            {
                if (IsNotNull(znodeSearchProfileAttributeMappings))
                {
                    ZnodeSearchProfileAttributeMapping znodeSearchProfileAttributeMapping = znodeSearchProfileAttributeMappings.FirstOrDefault(x => x.SearchProfileAttributeMappingId.Equals(Convert.ToInt32(searchProfilesAttributeMappingId)));
                    if (IsNotNull(znodeSearchProfileAttributeMapping) && znodeSearchProfileAttributeMapping.IsUseInSearch)
                    {
                        znodeSearchProfileAttributeMapping.IsFacets = false;
                        searchAttributesUpdate.Add(znodeSearchProfileAttributeMapping);
                    }
                    else
                        searchProfilesAttributeMappingIdCollection = IsNull(searchProfilesAttributeMappingIdCollection) ? searchProfilesAttributeMappingId : searchProfilesAttributeMappingIdCollection + ',' + searchProfilesAttributeMappingId;
                }
            }
            bool isSuccessfull = false;

            if (IsNotNull(searchProfilesAttributeMappingIdCollection))
            {
                filter.Add(new FilterTuple(ZnodeSearchProfileAttributeMappingEnum.SearchProfileAttributeMappingId.ToString(), ProcedureFilterOperators.In, searchProfilesAttributeMappingIdCollection));
                isSuccessfull = _searchProfileAttributeMappingRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
            }

            if (searchAttributesUpdate.Count() > 0)
                isSuccessfull = _searchProfileAttributeMappingRepository.BatchUpdate(searchAttributesUpdate);

            ZnodeLogging.LogMessage("searchProfilesAttributeMappingIds to be deleted: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, searchProfilesAttributeMappingIds?.Ids);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return isSuccessfull;
        }
        #endregion
        #endregion

        #region Private Methods
        //Get Search Attribute List.
        private List<SearchAttributesModel> GetSearchAttributeList(FilterCollection filters, PageListModel pageListModel, string attributeName)
        {
            if (!string.IsNullOrEmpty(attributeName))
                filters.Add(FilterKeys.AttributeCode, FilterOperators.Contains, attributeName);
            filters.Add(ZnodeSearchProfileAttributeMappingEnum.IsFacets.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue);

            ZnodeLogging.LogMessage("pageListModel to get SearchAttributes list: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            return _searchProfileAttributeMappingRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.ToModel<SearchAttributesModel>()?.ToList();
        }

        //Get isAssociated and attributeName from filters.
        private void GetFilterValues(FilterCollection filters, ref string isAssociated, ref string attributeName)
        {
            isAssociated = filters.Find(x => string.Equals(x.FilterName, FilterKeys.IsAssociated, StringComparison.CurrentCultureIgnoreCase))?.FilterValue;
            filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.IsAssociated, StringComparison.CurrentCultureIgnoreCase));

            attributeName = filters.Find(x => string.Equals(x.FilterName, FilterKeys.AttributeName, StringComparison.CurrentCultureIgnoreCase) || string.Equals(x.FilterName, $"{FilterKeys.AttributeName}|", StringComparison.CurrentCultureIgnoreCase))?.FilterValue;
            filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.AttributeName, StringComparison.CurrentCultureIgnoreCase) || string.Equals(x.FilterName, $"{FilterKeys.AttributeName}|", StringComparison.CurrentCultureIgnoreCase));
        }

        //Get unassociated facetable attributes.
        private List<SearchAttributesModel> GetPublishAttributes(FilterCollection filters, List<SearchAttributesModel> searchAttributeList, PageListModel pageListModel, string attributeName, int publishCatalogId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeSearchProfileAttributeMappingEnum.SearchProfileId.ToString(), StringComparison.CurrentCultureIgnoreCase));
            int? versionId = GetCatalogVersionId(publishCatalogId, ZnodePublishStatesEnum.PRODUCTION);
            if (searchAttributeList?.Count() > 0)
            {
                string attributeCodes = string.Join(",", searchAttributeList.Select(x => x.AttributeCode).Select(x => $"\"{x}\""));
                filters.Add(ZnodeSearchProfileAttributeMappingEnum.AttributeCode.ToString(), FilterOperators.NotIn, attributeCodes);
            }
            if (!string.IsNullOrEmpty(attributeName))
                filters.Add(FilterKeys.AttributeName, FilterOperators.Is, attributeName);
            filters.Add(WebStoreEnum.ZnodeCatalogId.ToString(), FilterOperators.Equals, publishCatalogId.ToString());
            filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, GetDefaultLocaleId().ToString());
            filters.Add("VersionId", FilterOperators.Equals, Convert.ToString(versionId));

            //Get Existing Catalog Attribute Configuration.
            ZnodeLogging.LogMessage("pageListModel to get domainListEntity: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return publishedCatalogDataService.GetPublishCatalogAttributePagedList(pageListModel)?.ToModel<SearchAttributesModel>()?.ToList();
        }

        //Map Attribute Names to Attribute List
        protected virtual List<SearchAttributesModel> MapAttributeNames(int publishCatalogId, List<SearchAttributesModel> searchableAttributesList,FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("publishCatalogId and query to get Attributes list: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { publishCatalogId, filters });
            List<ZnodePublishCatalogAttributeEntity> Attributes = GetAttributesFromQuery(publishCatalogId, null, null, null, filters);

            if (searchableAttributesList?.Count == 0)
                searchableAttributesList = Attributes.ToModel<SearchAttributesModel>().ToList();
            searchableAttributesList?.ForEach(destination =>
            {
                ZnodePublishCatalogAttributeEntity source = Attributes?
                            .FirstOrDefault(s => s.AttributeCode.Equals(destination.AttributeCode, StringComparison.InvariantCultureIgnoreCase));
                if (IsNotNull(source))
                {
                    destination.AttributeName = string.IsNullOrEmpty(source.AttributeName) ? source.AttributeCode : source.AttributeName;
                    destination.DisplayOrder = source.DisplayOrder;
                }
            });

            ZnodeLogging.LogMessage("searchableAttributesList count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, searchableAttributesList?.Count);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchableAttributesList;
        }

        //Insert Update Search Profile
        private View_ReturnBooleanWithMessage InsertUpdateSearchProfile(SearchProfileModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            IZnodeViewRepository<View_ReturnBooleanWithMessage> objStoredProc = new ZnodeViewRepository<View_ReturnBooleanWithMessage>();

            objStoredProc.SetParameter("@SearchProfileId", model.SearchProfileId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@ProfileName", model.ProfileName, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@SearchQueryTypeId", model.SearchQueryTypeId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@SearchSubQueryTypeId", model.SearchSubQueryTypeId.ToString(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PublishCatalogId", model.PublishCatalogId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Operator", model.Operator, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@IsDefault", false, ParameterDirection.Input, DbType.Boolean);

            View_ReturnBooleanWithMessage SearchProfile = null;
            DataTable SearchProfileFeatureList = ConvertFeatureListToDataTable(model.FeaturesList);
            DataTable SearchProfileAttributeList = ConvertAttributeListToDataTable(model.SearchableAttributesList);
            DataTable SearchProfileFieldValue = ConvertValuesFactorListToDataTable(model.FieldValueFactors);

            if (DefaultGlobalConfigSettingHelper.IsColumnEncryptionSettingEnabled)
            {
                objStoredProc.SetParameter("@SearchProfileFeatureList", SearchProfileFeatureList?.ToJson(), ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@SearchProfileAttributeList", SearchProfileAttributeList?.ToJson(), ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@SearchProfileFieldValue", SearchProfileFieldValue?.ToJson(), ParameterDirection.Input, DbType.String);

                SearchProfile = objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdateSearchProfileWithJSON @SearchProfileId,@ProfileName,@SearchQueryTypeId,@SearchSubQueryTypeId,@SearchProfileFeatureList,@SearchProfileAttributeList,@UserId,@PublishCatalogId,@Operator,@IsDefault,@SearchProfileFieldValue")?.FirstOrDefault();
            }
            else
            {
                objStoredProc.SetTableValueParameter("@SearchProfileFeatureList", SearchProfileFeatureList, ParameterDirection.Input, SqlDbType.Structured, "dbo.SearchProfileFeatureList");
                objStoredProc.SetTableValueParameter("@SearchProfileAttributeList", SearchProfileAttributeList, ParameterDirection.Input, SqlDbType.Structured, "dbo.SearchProfileAttributeList");
                objStoredProc.SetTableValueParameter("@SearchProfileFieldValue", SearchProfileFieldValue, ParameterDirection.Input, SqlDbType.Structured, "dbo.SearchProfileFieldValueFactor");

                SearchProfile = objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdateSearchProfile @SearchProfileId,@ProfileName,@SearchQueryTypeId,@SearchSubQueryTypeId,@SearchProfileFeatureList,@SearchProfileAttributeList,@UserId,@PublishCatalogId,@Operator,@IsDefault,@SearchProfileFieldValue")?.FirstOrDefault();
            }
            if (IsNotNull(SearchProfile) && SearchProfile.MessageDetails.Contains(Admin_Resources.AlreadyExists))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorSearchProfileAlreadyExists);
            ZnodeLogging.LogMessage("SearchProfile: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, SearchProfile);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return SearchProfile;
        }

        //binds required details from dataset to search profile model
        protected virtual void BindSearchProfileDetailsFromDataSet(DataSet searchProfile, SearchProfileModel searchProfileModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            if (searchProfile.Tables?.Count > 0)
            {
                List<SearchAttributesModel> searchAttributes = searchProfile.Tables[1]?.AsEnumerable().Select(m => new SearchAttributesModel()
                {
                    AttributeCode = m.Field<string>("AttributeCode"),
                    BoostValue = m.Field<int?>("BoostValue"),
                    IsFacets = m.Field<bool>("IsFacets"),
                    IsUseInSearch = m.Field<bool>("IsUseInSearch"),
                    IsNgramEnabled =  IsNull (m.Field<bool?>("IsNgramEnabled"))? false : m.Field<bool>("IsNgramEnabled"),
                }).ToList();

                // The excludeSearchFeatures variable contains the list of search features which should not be displayed on the search profile screen.
                List<string> excludedFeatureList = searchService.GetExcludedSearchFeatureList();
                List<SearchFeatureModel> featureList = searchProfile.Tables[0]?.AsEnumerable()
                    ?.Where(x => !excludedFeatureList.Contains(x.Field<string>("FeatureCode").ToLower()))
                    ?.Select(m => new SearchFeatureModel()
                    {

                        FeatureName = m.Field<string>("FeatureName"),
                        FeatureCode = m.Field<string>("FeatureCode"),
                        SearchFeatureId = m.Field<int>("SearchFeatureId"),
                        SearchFeatureValue = m.Field<string>("SearchFeatureValue"),
                        ControlType = m.Field<string>("ControlType"),
                        HelpDescription = m.Field<string>("HelpDescription"),
                        ParentSearchFeatureId = m.Field<int?>("ParentSearchFeatureId"),
                        IsAdvancedFeature = m.Field<bool>("IsAdvanceFeature")
                    }).ToList();

                DataTable profileData = searchProfile.Tables[3];

                foreach (DataRow row in profileData.Rows)
                {
                    searchProfileModel.ProfileName = Convert.ToString(row["ProfileName"]);
                    searchProfileModel.QueryTypeName = Convert.ToString(row["QueryTypeName"]);
                    searchProfileModel.SubQueryType = Convert.ToString(row["SubQueryTypeName"]);
                    searchProfileModel.QueryBuilderClassName = Convert.ToString(row["QueryBuilderClassName"]);
                    searchProfileModel.Operator = Convert.ToString(row["Operator"]);
                    searchProfileModel.PublishCatalogId = Convert.ToInt32(row["PublishCatalogId"]);
                    searchProfileModel.CatalogName = Convert.ToString(row["CatalogName"]);
                    searchProfileModel.SearchQueryTypeId = Convert.ToInt32(row["SearchQueryTypeId"]);
                    searchProfileModel.SearchSubQueryTypeId = Convert.ToInt32(row["SearchSubQueryTypeId"]);
                }

                List<KeyValuePair<string, int>> fieldList = searchProfile.Tables[4]?.AsEnumerable().Select(m => new KeyValuePair<string, int>(m.Field<string>("FieldName"), m.Field<int>("FieldValueFactor"))).ToList();
                searchProfileModel.FeaturesList = featureList;
                searchProfileModel.SearchableAttributesList = searchAttributes;
                searchProfileModel.FieldValueFactors = fieldList;
            }
            // The excludeQueryTypes variable contains the list of search query types which should not be displayed on the search profile screen.
            List<string> excludedQueryTypes = searchService.GetAllSearchQueryTypeList();
            searchProfileModel.QueryTypes = _searchQueryTypeRepository.Table
                                            ?.Where(x => !excludedQueryTypes.Contains(x.QueryTypeName.Replace(" ", "").ToLower()))
                                            ?.OrderBy(x => x.DisplayOrder)?.ToModel<SearchQueryTypeModel>().ToList();

            FilterCollection filters = new FilterCollection();
            filters.Add("IsUseInSearch", FilterOperators.Equals, ZnodeConstant.TrueValue);

            searchProfileModel.SearchableAttributesList = MapAttributeNames(searchProfileModel.PublishCatalogId, searchProfileModel.SearchableAttributesList, filters)?.OrderBy(x => x.DisplayOrder)?.ToList();
            ZnodeLogging.LogMessage("SearchableAttributesList count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, searchProfileModel?.SearchableAttributesList?.Count);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
        }

        //Get catalogs from query
        private List<ZnodePublishCatalogAttributeEntity> GetAttributesFromQuery(int publishCatalogId, ParameterModel associatedAttributes, NameValueCollection sorts, NameValueCollection page, FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            List<ZnodePublishCatalogAttributeEntity> attributes = publishedCatalogDataService.GetPublishCatalogAttributePagedList(new PageListModel(GetFiltersForCatalogAttribute(associatedAttributes, publishCatalogId, filters), sorts, page));

            ZnodeLogging.LogMessage("Attributes list count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, attributes?.Count);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return attributes;
        }

        //Get filters for catalog attribute
        protected virtual FilterCollection GetFiltersForCatalogAttribute(ParameterModel associatedAttributes, int publishCatalogId, FilterCollection filters)
        {
            filters.Add("ZnodeCatalogId", FilterOperators.Equals, publishCatalogId.ToString());
            filters.Add("LocaleId", FilterOperators.Equals, GetDefaultLocaleId().ToString());

            if (associatedAttributes?.Ids?.Count() > 0)
                filters.Add("AttributeCode", FilterOperators.NotIn, string.Join(",", associatedAttributes?.Ids.Split(',').Select(x => $"\"{x}\"")));

            int? versionId = GetCatalogVersionId(publishCatalogId, ZnodePublishStatesEnum.PRODUCTION);
            if (versionId != null)
                filters.Add("VersionId", FilterOperators.In, versionId.ToString());

            return filters;

        }

        //Replace Filter Keys
        private void ReplaceFilterKeys(ref FilterCollection filters)
        {
            if (filters?.Count <= 0)
                return;
            if(IsNotNull(filters))
            {
                foreach (FilterTuple tuple in filters)
                {
                    if (string.Equals(tuple.Item1, FilterKeys.AttributeName, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, FilterKeys.AttributeName, FilterKeys.PublishedAttributeName); }
                    if (string.Equals(tuple.Item1, FilterKeys.AttributeCode, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, FilterKeys.AttributeCode, FilterKeys.AttributeCode); }
                }
            }
            ReplaceFilterKeysForOr(ref filters);
        }

        //Replace Filter Keys
        private void ReplaceFilterKeysForOr(ref FilterCollection filters)
        {
            foreach (FilterTuple tuple in filters)
            {
                if (tuple.Item1.Contains("|"))
                {
                    List<string> newValues = new List<string>();
                    foreach (var item in tuple.Item1.Split('|'))
                    {
                        if (string.Equals(item, FilterKeys.AttributeName, StringComparison.OrdinalIgnoreCase)) { newValues.Add(FilterKeys.PublishedAttributeName); }
                        else if (string.Equals(item, FilterKeys.AttributeCode, StringComparison.OrdinalIgnoreCase)) { newValues.Add(FilterKeys.AttributeCode.ToLower()); }
                        else newValues.Add(item);
                    }
                    ReplaceFilterKeyName(ref filters, tuple.Item1, string.Join("|", newValues));
                }
            }
        }

        //Replace Product Filter Keys
        private void ReplaceSortKeys(ref NameValueCollection sorts)
        {
            if (sorts == null || sorts.Count == 0)
                return;
            foreach (string key in sorts.Keys)
            {
                if (string.Equals(key, FilterKeys.AttributeName, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, FilterKeys.AttributeName.ToLower(), FilterKeys.PublishedAttributeName); }
                if (string.Equals(key, FilterKeys.AttributeCode, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, FilterKeys.AttributeCode.ToLower(), FilterKeys.AttributeCode); }
            }
        }

        //Converts Searchable Attributes List to Data Table
        private DataTable ConvertAttributeListToDataTable(List<SearchAttributesModel> searchableAttributesList)
        {
            DataTable table = new DataTable("SearchProfileAttributeList");
            table.Columns.Add("AttributeCode", typeof(string));
            table.Columns.Add("IsFacets", typeof(bool));
            table.Columns.Add("IsUseInSearch", typeof(bool));
            table.Columns.Add("BoostValue", typeof(int));
            table.Columns.Add("IsNgramEnabled", typeof(bool));

            foreach (SearchAttributesModel model in searchableAttributesList)
                table.Rows.Add(model.AttributeCode, model.IsFacets, model.IsUseInSearch, model.BoostValue,model.IsNgramEnabled);

            return table;
        }

        //Converts Searchable Features List to Data Table
        private DataTable ConvertFeatureListToDataTable(List<SearchFeatureModel> featuresList)
        {
            DataTable table = new DataTable("SearchProfileFeatureList");
            DataColumn SearchProfileFeatureId = new DataColumn("SearchProfileFeatureId");
            table.Columns.Add(SearchProfileFeatureId);
            table.Columns.Add("SearchFeatureValue", typeof(string));

            foreach (SearchFeatureModel model in featuresList)
                table.Rows.Add(model.SearchFeatureId, model.SearchFeatureValue);

            return table;
        }

        //Converts Searchable Attributes List to Data Table
        private DataTable ConvertKeywordListToDataTable(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                DataTable table = new DataTable("KeywordList");
                table.Columns.Add("Keyword", typeof(string));

                foreach (string model in keyword.Split(','))
                    table.Rows.Add(model);
                return table;
            }
            return null;
        }

        //Converts Searchable Attributes List to Data Table
        private DataTable ConvertProfileArrayToDataTable(string[] profileIds)
        {
            if (profileIds?.Count() > 0 && !string.IsNullOrEmpty(profileIds[0]))
            {
                DataTable table = new DataTable("UserProfileList");
                table.Columns.Add("ProfileId", typeof(int));

                foreach (string model in profileIds)
                    table.Rows.Add(model);

                return table;
            }
            return null;
        }

        private DataTable GetPortalIdInDataTable(string portalId)
        {
            DataTable table = new DataTable("UserPortalList");
            table.Columns.Add("PortalId", typeof(string));

            foreach (string model in portalId.Split(','))
                table.Rows.Add(model);

            return table;
        }

        //Delete search profiles and its mappings.
        private bool DeleteSearchProfiles(int searchProfileId)
        {
            bool status = false;

            ExecuteSpHelper objStoredProc = new ExecuteSpHelper();
            objStoredProc.GetParameter("@SearchProfileId", searchProfileId, ParameterDirection.Input, SqlDbType.Int);

            DataSet dataset = objStoredProc.GetSPResultInDataSet("Znode_DeleteSearchProfile");
            DataTable result = dataset?.Tables[0];

            foreach (DataRow row in result.Rows)
            {
                status = Convert.ToBoolean(row["Status"]);
            }
            return status;
        }

        private DataTable ConvertValuesFactorListToDataTable(List<KeyValuePair<string, int>> fieldValueFactors)
        {
            DataTable table = new DataTable("SearchProfileFieldValueFactor");
            table.Columns.Add("FieldName", typeof(string));
            table.Columns.Add("FieldValueFactor", typeof(int));

            foreach (KeyValuePair<string, int> model in fieldValueFactors)
                table.Rows.Add(model.Key, model.Value);

            return table;
        }

        private void GetFieldValueList(int publishCatalogId, SearchProfileModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            try
            {
                string indexName = GetService<ISearchService>().GetCatalogIndexName(publishCatalogId);

                if (string.IsNullOrEmpty(indexName))
                    model.IsIndexExist = false;

                string indexPointingToAlias = GetService<IDefaultDataService>().GetIndicesPointingToAlias(indexName)?.FirstOrDefault();

                if (!string.IsNullOrEmpty(indexPointingToAlias))
                    indexName = indexPointingToAlias;

                ZnodeLogging.LogMessage("indexName to get fieldList: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, indexName);
                List<string> fieldList = GetService<IElasticSearchBaseService>().FieldValueList(indexName, "number");
                ZnodeLogging.LogMessage("fieldList count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, fieldList?.Count);

                if (fieldList.Count > 0)
                {
                    List<FieldValueModel> fieldValueModelList = new List<FieldValueModel>();
                    List<ZnodePublishCatalogAttributeEntity> attributeList = publishedCatalogDataService.GetPublishCatalogAttributeList(new PageListModel(GetFiltersForFieldValueList(publishCatalogId), null, null));

                    ZnodeLogging.LogMessage("attributeList count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, attributeList?.Count);

                    foreach (string item in fieldList)
                    {
                        FieldValueModel fieldValueModel = new FieldValueModel();
                        fieldValueModel.AttributeCode = item;
                        switch (item)
                        {
                            case "rating":
                                fieldValueModel.AttributeName = "Ratings";
                                break;
                            case "totalreviewcount":
                                fieldValueModel.AttributeName = "Total Reviews";
                                break;
                            case "productprice":
                                fieldValueModel.AttributeName = "Price";
                                break;
                            default:
                                fieldValueModel.AttributeName = attributeList.FirstOrDefault(x => x.AttributeCode.Equals(item, StringComparison.InvariantCultureIgnoreCase))?.AttributeName;
                                break;
                        }
                        fieldValueModelList.Add(fieldValueModel);
                    }
                    model.FieldValueList = fieldValueModelList;
                }
                model.IsIndexExist = true;
                model.PublishCatalogId = publishCatalogId;
            }
            catch (ZnodeException)
            {
                model.PublishCatalogId = publishCatalogId;
                model.IsIndexExist = false;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
        }

        // Get filters
        private FilterCollection GetFiltersForFieldValueList(int publishCatalogId)
        {
            FilterCollection filter = new FilterCollection();
            filter.Add("ZnodeCatalogId", FilterOperators.Equals, publishCatalogId.ToString());
            filter.Add("LocaleId", FilterOperators.Equals, GetDefaultLocaleId().ToString());
            filter.Add("IsUseInSearch", FilterOperators.Equals, ZnodeConstant.TrueValue);
            return filter;
        }

        #endregion

        public virtual bool PublishSearchProfile(int searchProfileId)
        {
            bool status = false;

            if (searchProfileId > 0)
            {
                ZnodeLogging.LogMessage("Execution Started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

                ExecuteSpHelper objStoredProc = new ExecuteSpHelper();

                objStoredProc.GetParameter("@SearchProfileId", searchProfileId, ParameterDirection.Input, SqlDbType.Int);
                objStoredProc.GetParameter("@UserId", GetLoginUserId(), ParameterDirection.Input, SqlDbType.Int);

                DataSet dataSet = objStoredProc.GetSPResultInDataSet("Znode_PublishSearchProfileEntity");

                DataTable result = dataSet?.Tables[0];

                foreach (DataRow row in result.Rows)
                {
                    status = Convert.ToBoolean(row["Status"]);
                }
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

                return status;
            }
            return status;
        }

        // To get the catalog list that is not associated with any of the search profiles.
        public virtual TypeaheadResponselistModel GetCatalogListForSearchProfile()
        {
            ZnodeRepository<ZnodePublishCatalogEntity> _publishCatalogEntity = new ZnodeRepository<ZnodePublishCatalogEntity>(HelperMethods.Context);
            IZnodeRepository<ZnodePublishCatalogSearchProfile> _publishCatalogSearchProfile = new ZnodeRepository<ZnodePublishCatalogSearchProfile>();

            List<int> publishCatalogIds = _publishCatalogSearchProfile.Table.Select(x => x.PublishCatalogId)?.Distinct().ToList();
            List<PublishCatalogModel> publishCatalogList = (from publishCatalog in _publishCatalogEntity.Table.Where(y => !publishCatalogIds.Contains(y.ZnodeCatalogId))
                                                           select new PublishCatalogModel {CatalogName= publishCatalog.CatalogName, PublishCatalogId = publishCatalog.ZnodeCatalogId })?.Distinct()?.ToList()?.OrderBy(x => x.CatalogName)?.ToList();
            TypeaheadResponselistModel catalogList = new TypeaheadResponselistModel();
            catalogList.Typeaheadlist = new List<TypeaheadResponseModel>();
            foreach (PublishCatalogModel catalog in publishCatalogList)
            {
                catalogList.Typeaheadlist.Add(new TypeaheadResponseModel
                {
                    DisplayText = catalog.CatalogName,
                    Id = catalog.PublishCatalogId,
                    Name = catalog.CatalogName,
                });
            }
            return catalogList;
        }
    }
}
