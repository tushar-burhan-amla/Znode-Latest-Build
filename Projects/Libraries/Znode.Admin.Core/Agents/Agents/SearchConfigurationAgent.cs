using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents
{
    public class SearchConfigurationAgent : BaseAgent, ISearchConfigurationAgent
    {
        #region Private Variables
        private readonly ISearchClient _searchClient;
        private readonly IEcommerceCatalogClient _ecommerceCatalogClient;
        private readonly IPortalClient _portalClient;
        private readonly ICMSPageSearchClient _cmsPagesearchClient;
        #endregion

        #region Constructor
        public SearchConfigurationAgent(ISearchClient searchClient, IEcommerceCatalogClient ecommerceCatalogClient, IPortalClient portalClient, ICMSPageSearchClient cmsPageSearchClient)
        {
            _searchClient = GetClient<ISearchClient>(searchClient);
            _ecommerceCatalogClient = GetClient<IEcommerceCatalogClient>(ecommerceCatalogClient);
            _portalClient = GetClient<IPortalClient>(portalClient);
            _cmsPagesearchClient = GetClient<ICMSPageSearchClient>(cmsPageSearchClient);
        }
        #endregion

        #region Public Method
        //Gets the portal index data.
        [Obsolete("This method is deprecated and will be discontinued in upcoming versions." +
        " This method is marked as obsolute because filters will be required along with publishCatalogId" +
        " Please use overload of this method which have filters and publishCatalogId as parameters ")]
        public virtual PortalIndexViewModel GetPortalIndexData(int publishCatalogId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter :", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { publishCatalogId = publishCatalogId });
            if (publishCatalogId > 0)
                return GetSearchIndexByCatalogId(publishCatalogId);
            else
                return GetFirstCatalogIndexData();
        }

        private PortalIndexViewModel GetFirstCatalogIndexData()
        {
            PublishCatalogModel catalogDetails = _ecommerceCatalogClient.GetPublishCatalogList(null, null, null, null, null)?.PublishCatalogs?.OrderBy(m => m.CatalogName).FirstOrDefault();

            if (IsNotNull(catalogDetails))
                return GetSearchIndexByCatalogId(catalogDetails.PublishCatalogId, catalogDetails.CatalogName);

            return new PortalIndexViewModel();
        }

        //Gets the portal index data.
        public virtual PortalIndexViewModel GetPortalIndexData(FilterCollection filters, int publishCatalogId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter :", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { publishCatalogId = publishCatalogId });

            List<PublishCatalogModel> publishCatalogList = _ecommerceCatalogClient.GetPublishCatalogList(null, null, null, null, null).PublishCatalogs;

            if (publishCatalogId == 0)
            {
                int publishCatalogIdFromFilter = GetCatalogIdFromFilters(filters);
                publishCatalogId = publishCatalogIdFromFilter > 0 ? publishCatalogIdFromFilter : (publishCatalogList?.OrderBy(m => m.CatalogName).FirstOrDefault()?.PublishCatalogId).GetValueOrDefault();
            }
            UpdateFilters(filters, FilterKeys.PublishCatalogId, FilterOperators.Equals, publishCatalogId.ToString());

            string catalogName = publishCatalogList?.FirstOrDefault(x => x.PublishCatalogId == publishCatalogId)?.CatalogName;

            //To remove unwanted filters
            filters?.RemoveAll(x => x.Item1 != FilterKeys.PublishCatalogId);

            PortalIndexViewModel portalIndexData = _searchClient.GetCatalogIndexData(new ExpandCollection() {}, filters)?.ToViewModel<PortalIndexViewModel>() ?? new PortalIndexViewModel();

            portalIndexData.PublishCatalogId = publishCatalogId;
            portalIndexData.CatalogName = catalogName;

            return portalIndexData;
        }

        //Inserts data for create index.
        public virtual PortalIndexViewModel InsertCreateIndexData(PortalIndexViewModel portalIndexViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            if (IsNotNull(portalIndexViewModel))
            {
                try
                {
                    portalIndexViewModel = _searchClient.InsertCreateIndexData(portalIndexViewModel.ToModel<PortalIndexModel>()).ToViewModel<PortalIndexViewModel>();
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
                    if (ex.ErrorCode == ErrorCodes.DuplicateSearchIndexName)
                    {
                        portalIndexViewModel.HasError = true;
                        portalIndexViewModel.ErrorMessage = Admin_Resources.ErrorIndexNameInUse;
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                }
            }

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            return portalIndexViewModel;
        }

        //Gets the search index monitor list.
        public virtual SearchIndexMonitorListViewModel GetSearchIndexMonitorList(int catalogIndexId, ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter :", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { catalogIndexId = catalogIndexId, expands = expands, filters = filters, sortCollection = sortCollection });

            if (IsNull(filters))
                filters = new FilterCollection();

            int publishCatalogId = 0;

            if (Equals(filters?.Exists(x => x.Item1 == FilterKeys.PublishCatalogId), false))
            {
                List<PublishCatalogModel> publishCatalogList = _ecommerceCatalogClient.GetPublishCatalogList(null, null, null, null, null).PublishCatalogs;
                publishCatalogId = (publishCatalogList?.OrderBy(m => m.CatalogName).FirstOrDefault()?.PublishCatalogId).GetValueOrDefault();
                UpdateFilters(filters, FilterKeys.PublishCatalogId, FilterOperators.Equals, publishCatalogId.ToString());
            }
            else
                publishCatalogId = GetCatalogIdFromFilters(filters);

            SearchIndexMonitorListModel searchIndexMonitorList = _searchClient.GetSearchIndexMonitorList(filters, expands, sortCollection, page, recordPerPage);
            SearchIndexMonitorListViewModel searchIndexMonitorListViewModel = new SearchIndexMonitorListViewModel { SearchIndexMonitorList = searchIndexMonitorList?.SearchIndexMonitorList?.ToViewModel<SearchIndexMonitorViewModel>().ToList() };
            SetListPagingData(searchIndexMonitorListViewModel, searchIndexMonitorList);

            searchIndexMonitorListViewModel.PublishCatalogId = publishCatalogId;

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
			return searchIndexMonitorListViewModel?.SearchIndexMonitorList?.Count > 0 ? searchIndexMonitorListViewModel : new SearchIndexMonitorListViewModel() { SearchIndexMonitorList = new List<SearchIndexMonitorViewModel>(), PublishCatalogId= publishCatalogId };
        }

        //Gets search index server status list.
        public virtual SearchIndexServerStatusListViewModel GetSearchIndexServerStatusList(int searchIndexMonitorId, ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter :", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { searchIndexMonitorId = searchIndexMonitorId, expands = expands, filters = filters, sortCollection = sortCollection });

            if (IsNull(filters))
                filters = new FilterCollection();


            filters.RemoveAll(filter => filter.FilterName == ZnodeSearchIndexServerStatuEnum.SearchIndexMonitorId.ToString());
            filters.Add(new FilterTuple(ZnodeSearchIndexServerStatuEnum.SearchIndexMonitorId.ToString(), FilterOperators.Equals, searchIndexMonitorId.ToString()));

            SearchIndexServerStatusListModel searchIndexServerStatusList = _searchClient.GetSearchIndexServerStatusList(filters, expands, sortCollection, page, recordPerPage);
            SearchIndexServerStatusListViewModel searchIndexServerStatusListViewModel = new SearchIndexServerStatusListViewModel { SearchIndexServerStatusList = searchIndexServerStatusList?.SearchIndexServerStatusList?.ToViewModel<SearchIndexServerStatusViewModel>().ToList() };
            SetListPagingData(searchIndexServerStatusListViewModel, searchIndexServerStatusList);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchIndexServerStatusListViewModel?.SearchIndexServerStatusList?.Count > 0 ? searchIndexServerStatusListViewModel : new SearchIndexServerStatusListViewModel() { SearchIndexServerStatusList = new List<SearchIndexServerStatusViewModel>() };
        }

        //Gets field level boost list.
        public virtual SearchDocumentMappingListViewModel GetFieldLevelBoostList(int catalogId, ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter :", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { catalogId = catalogId, expands = expands, filters = filters, sortCollection = sortCollection });

            List<PublishCatalogModel> publishCatalogList = _ecommerceCatalogClient.GetPublishCatalogList(null, null, null, null, null).PublishCatalogs;

            sortCollection = SetBoostSortCollection(sortCollection);

            if (catalogId <= 0)
                catalogId = (publishCatalogList?.First()?.PublishCatalogId).GetValueOrDefault();

            filters.RemoveAll(filter => filter.FilterName == ZnodeSearchGlobalProductBoostEnum.PublishCatalogId.ToString());
            filters.Add(new FilterTuple(ZnodeSearchGlobalProductBoostEnum.PublishCatalogId.ToString(), FilterOperators.Equals, catalogId.ToString()));

            SearchDocumentMappingListModel fieldLevelBoostList = _searchClient.GetFieldLevelBoostList(filters, expands, sortCollection, page, recordPerPage);
            SearchDocumentMappingListViewModel fieldLevelBoostListViewModel = new SearchDocumentMappingListViewModel { SearchDocumentMappingList = fieldLevelBoostList?.SearchDocumentMappingList?.ToViewModel<SearchDocumentMappingViewModel>().ToList() };
            if (fieldLevelBoostListViewModel?.SearchDocumentMappingList?.Count > 0)
            {
                SetListPagingData(fieldLevelBoostListViewModel, fieldLevelBoostList);
                fieldLevelBoostListViewModel.CatalogId = catalogId;
                fieldLevelBoostListViewModel.CatalogName = publishCatalogList?.FirstOrDefault(x => x.PublishCatalogId == catalogId)?.CatalogName;
                return fieldLevelBoostListViewModel;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return new SearchDocumentMappingListViewModel() { SearchDocumentMappingList = fieldLevelBoostListViewModel.SearchDocumentMappingList, CatalogId = catalogId, CatalogName = publishCatalogList?.FirstOrDefault(x => x.PublishCatalogId == catalogId)?.CatalogName };
        }

        //Gets global product boost list.
        public virtual SearchGlobalProductBoostListViewModel GetGlobalProductBoostList(int catalogId, ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter :", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { catalogId = catalogId, expands = expands, filters = filters, sortCollection = sortCollection });

            List<PublishCatalogModel> publishCatalogList = _ecommerceCatalogClient.GetPublishCatalogList(null, null, null, null, null).PublishCatalogs;

            sortCollection = SetBoostSortCollection(sortCollection);

            if (catalogId <= 0)
                catalogId = publishCatalogList.First().PublishCatalogId;

            filters.RemoveAll(filter => filter.FilterName == ZnodeSearchGlobalProductBoostEnum.PublishCatalogId.ToString());
            filters.Add(new FilterTuple(ZnodeSearchGlobalProductBoostEnum.PublishCatalogId.ToString(), FilterOperators.Equals, catalogId.ToString()));

            SearchGlobalProductBoostListModel globalProductBoost = _searchClient.GetGlobalProductBoostList(filters, expands, sortCollection, page, recordPerPage);
            SearchGlobalProductBoostListViewModel globalProductBoostListViewModel = new SearchGlobalProductBoostListViewModel { SearchGlobalProductBoostList = globalProductBoost?.SearchGlobalProductBoostList?.ToViewModel<SearchGlobalProductBoostViewModel>().ToList() };

            if (globalProductBoostListViewModel?.SearchGlobalProductBoostList?.Count > 0)
            {
                globalProductBoostListViewModel.CatalogId = catalogId;
                globalProductBoostListViewModel.CatalogName = publishCatalogList?.FirstOrDefault(x => x.PublishCatalogId == catalogId)?.CatalogName;
                SetListPagingData(globalProductBoostListViewModel, globalProductBoost);
                return globalProductBoostListViewModel;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return new SearchGlobalProductBoostListViewModel() { CatalogId = catalogId, CatalogName = publishCatalogList?.FirstOrDefault(x => x.PublishCatalogId == catalogId)?.CatalogName };
        }

        //Gets global product boost list.
        public virtual SearchGlobalProductCategoryBoostListViewModel GetGlobalProductCategoryBoostList(int catalogId, ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter :", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { catalogId = catalogId, expands = expands, filters = filters, sortCollection = sortCollection });

            List<PublishCatalogModel> publishCatalogList = _ecommerceCatalogClient.GetPublishCatalogList(null, null, null, null, null).PublishCatalogs;

            sortCollection = SetBoostSortCollection(sortCollection);

            if (catalogId <= 0)
                catalogId = (publishCatalogList?.First()?.PublishCatalogId).GetValueOrDefault();

            filters.RemoveAll(filter => filter.FilterName == ZnodeSearchGlobalProductBoostEnum.PublishCatalogId.ToString());
            filters.Add(new FilterTuple(ZnodeSearchGlobalProductBoostEnum.PublishCatalogId.ToString(), FilterOperators.Equals, catalogId.ToString()));

            SearchGlobalProductCategoryBoostListModel globalProductCategoryBoost = _searchClient.GetGlobalProductCategoryBoostList(filters, expands, sortCollection, page, recordPerPage);
            SearchGlobalProductCategoryBoostListViewModel globalProductCategoryBoostListViewModel = new SearchGlobalProductCategoryBoostListViewModel { SearchGlobalProductCategoryList = globalProductCategoryBoost?.SearchGlobalProductCategoryList?.ToViewModel<SearchGlobalProductCategoryBoostViewModel>().ToList() };

            if (globalProductCategoryBoostListViewModel?.SearchGlobalProductCategoryList?.Count > 0)
            {
                SetListPagingData(globalProductCategoryBoostListViewModel, globalProductCategoryBoost);
                globalProductCategoryBoostListViewModel.CatalogId = catalogId;
                globalProductCategoryBoostListViewModel.CatalogName = publishCatalogList?.FirstOrDefault(x => x.PublishCatalogId == catalogId)?.CatalogName;
                return globalProductCategoryBoostListViewModel;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return new SearchGlobalProductCategoryBoostListViewModel() { CatalogId = catalogId, CatalogName = publishCatalogList?.FirstOrDefault(x => x.PublishCatalogId == catalogId)?.CatalogName };
        }

        //Saves boost data.
        public virtual bool SaveBoostValues(BoostDataViewModel boostData)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            if (IsNotNull(boostData))
            {
                bool result = false;
                if (boostData.Boost > 0)
                    result = _searchClient.SaveBoostValues(boostData.ToModel<BoostDataModel>());
                if (boostData.Boost == 0)
                    result = _searchClient.DeleteBoostValue(boostData.ToModel<BoostDataModel>());

                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                return result;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return false;
        }

        //Get the list of all stores.
        public virtual StoreListViewModel GetStoreList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = default(int?), int? pageSize = default(int?))
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            PortalListModel storeList = _portalClient.GetPortalList(new ExpandCollection { ZnodePortalEnum.ZnodeDomains.ToString().ToLower() }, filters, sorts, pageIndex, pageSize);
            StoreListViewModel storeListViewModel = new StoreListViewModel { StoreList = storeList?.PortalList?.ToViewModel<StoreViewModel>().ToList() };

            //Set paging data.
            SetListPagingData(storeListViewModel, storeList);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return storeListViewModel;
        }

        //Get the list of all publish catalog.
        public virtual PortalCatalogListViewModel GetPublishCatalogList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? recordPerPage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            PublishCatalogListModel catalogList = _ecommerceCatalogClient.GetPublishCatalogList(expands, filters, sorts, pageIndex, recordPerPage);

            PortalCatalogListViewModel listViewModel = new PortalCatalogListViewModel { PortalCatalogs = catalogList?.PublishCatalogs?.ToViewModel<PortalCatalogViewModel>().ToList() };
            SetListPagingData(listViewModel, catalogList);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return listViewModel;
        }

        //Delete elastic search index.
        public virtual bool DeleteIndex(int catalogIndexId, ref string errorMessage)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                return _searchClient.DeleteIndex(catalogIndexId);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.NotFound:
                        errorMessage = Admin_Resources.ErrorSearchIndexNotExist;
                        return false;
                    default:
                        errorMessage = Admin_Resources.ErrorFailedToDeleteSearchIndex;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                errorMessage = Admin_Resources.ErrorFailedToDeleteSearchIndex;
                return false;
            }
        }

        #region Synonyms
        //Save synonyms data for search.
        public virtual SearchSynonymsViewModel CreateSearchSynonyms(SearchSynonymsViewModel viewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                SearchSynonymsModel searchSynonymsModel = _searchClient.CreateSearchSynonyms(viewModel?.ToModel<SearchSynonymsModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                return IsNotNull(searchSynonymsModel) ? searchSynonymsModel.ToViewModel<SearchSynonymsViewModel>() : new SearchSynonymsViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
                return new SearchSynonymsViewModel { HasError = true, ErrorMessage = ex.ErrorMessage };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                return (SearchSynonymsViewModel)GetViewModelWithErrorMessage(viewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Get synonyms data for search.
        public virtual SearchSynonymsViewModel GetSearchSynonyms(int searchSynonymsId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter :", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { searchSynonymsId = searchSynonymsId });

            if (searchSynonymsId > 0)
            {
                SearchSynonymsModel model = (_searchClient.GetSearchSynonyms(searchSynonymsId, null));

                //Maps synonyms model to synonyms view model
                return IsNotNull(model) ? model.ToViewModel<SearchSynonymsViewModel>() : new SearchSynonymsViewModel();
            }
            return new SearchSynonymsViewModel();
        }

        //Update synonyms data for search.
        public virtual SearchSynonymsViewModel UpdateSearchSynonyms(SearchSynonymsViewModel viewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                SearchSynonymsModel searchSynonymsModel = _searchClient.UpdateSearchSynonyms(viewModel.ToModel<SearchSynonymsModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                return IsNotNull(searchSynonymsModel) ? searchSynonymsModel.ToViewModel<SearchSynonymsViewModel>() : new SearchSynonymsViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
                return new SearchSynonymsViewModel { HasError = true, ErrorMessage = ex.ErrorMessage };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                return (SearchSynonymsViewModel)GetViewModelWithErrorMessage(viewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        //Get synonyms list for search.
        [Obsolete("This method is deprecated and will be discontinued in upcoming versions." +
        " This method is marked as obsolute because catalogId will be fetched from the filters" +
        " Please use overload of this method which does not have catalogId as parameters ")]
        public virtual SearchSynonymsListViewModel GetSearchSynonymsList(int catalogId, ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage)
        {

            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter :", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { catalogId = catalogId, expands = expands, filters = filters, sortCollection = sortCollection });

            List<PublishCatalogModel> publishCatalogList = _ecommerceCatalogClient.GetPublishCatalogList(null, null, null, null, null).PublishCatalogs;
            ZnodeLogging.LogMessage("publishCatalogList count :", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { publishCatalogListCount = publishCatalogList?.Count });


            if (catalogId == 0)
                catalogId = (publishCatalogList?.OrderBy(m => m.CatalogName).FirstOrDefault()?.PublishCatalogId).GetValueOrDefault();

            if (filters.Count < 1)
            {
                filters.Add(ZnodeCatalogIndexEnum.PublishCatalogId.ToString(), FilterOperators.Equals, catalogId.ToString());
            }
            else
            {
                if (filters.Exists(x => x.FilterName == ZnodeCatalogIndexEnum.PublishCatalogId.ToString()))
                {
                    filters.RemoveAll(x => x.FilterName == ZnodeCatalogIndexEnum.PublishCatalogId.ToString());
                    filters.Add(new FilterTuple(ZnodeCatalogIndexEnum.PublishCatalogId.ToString(), FilterOperators.Equals, catalogId.ToString()));
                }
            }

            SearchSynonymsListModel listModel = _searchClient.GetSearchSynonymsList(filters, expands, sortCollection, page, recordPerPage);


            SearchSynonymsListViewModel listViewModel = new SearchSynonymsListViewModel { SynonymsList = listModel?.SynonymsList?.ToViewModel<SearchSynonymsViewModel>().ToList() };

            if (listViewModel.SynonymsList?.Count > 0)
            {
                listViewModel.CatalogId = catalogId;
                listViewModel.CatalogName = publishCatalogList?.FirstOrDefault(x => x.PublishCatalogId == catalogId)?.CatalogName;
                SetListPagingData(listViewModel, listModel);
            }

            //Set tool menu for synonyms grid view.
            SetSearchSynonymsListToolMenu(listViewModel);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            return listViewModel.SynonymsList?.Count > 0 ? listViewModel
                           : new SearchSynonymsListViewModel { SynonymsList = new List<SearchSynonymsViewModel>(), CatalogId = catalogId, CatalogName = publishCatalogList?.FirstOrDefault(x => x.PublishCatalogId == catalogId)?.CatalogName };
        }

        //Get synonyms list for search.
        public virtual SearchSynonymsListViewModel GetSearchSynonymsList(ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage)
        {

			ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
			ZnodeLogging.LogMessage("Input parameter :", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sortCollection = sortCollection });

			List<PublishCatalogModel> publishCatalogList = _ecommerceCatalogClient.GetPublishCatalogList(null, null, null, null, null).PublishCatalogs;
			ZnodeLogging.LogMessage("publishCatalogList count :", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { publishCatalogListCount = publishCatalogList?.Count });

            int catalogId = GetCatalogIdFromFilters(filters);

            if (catalogId == 0)
            {
                catalogId = (publishCatalogList?.OrderBy(m => m.CatalogName).FirstOrDefault()?.PublishCatalogId).GetValueOrDefault();
                UpdateFilters(filters, FilterKeys.PublishCatalogId, FilterOperators.Equals, catalogId.ToString());
            }
            string catalogName = publishCatalogList?.FirstOrDefault(x => x.PublishCatalogId == catalogId)?.CatalogName;                  

            SearchSynonymsListModel listModel = _searchClient.GetSearchSynonymsList(filters, expands, sortCollection, page, recordPerPage);

            SearchSynonymsListViewModel listViewModel = new SearchSynonymsListViewModel { SynonymsList = listModel?.SynonymsList?.ToViewModel<SearchSynonymsViewModel>().ToList() };

            if (listViewModel.SynonymsList?.Count > 0)
            {
                listViewModel.CatalogId = catalogId;
                listViewModel.CatalogName = catalogName;
                SetListPagingData(listViewModel, listModel);
            }

            //Set tool menu for synonyms grid view.
            SetSearchSynonymsListToolMenu(listViewModel);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

			return listViewModel.SynonymsList?.Count > 0 ? listViewModel
                           : new SearchSynonymsListViewModel { SynonymsList = new List<SearchSynonymsViewModel>(), CatalogId = catalogId, CatalogName = catalogName };
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

        //Delete synonyms by ids.
        public virtual bool DeleteSearchSynonyms(string searchSynonymsIds, int publishCataLogId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            if (!string.IsNullOrEmpty(searchSynonymsIds))
            {
                try
                {
                    return _searchClient.DeleteSearchSynonyms(new ParameterModel { Ids = searchSynonymsIds, publishCataLogId = publishCataLogId });
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                    return false;
                }
            }
            return false;
        }

        //Write synonyms.txt for search.
        public virtual bool WriteSearchFile(int publishCatalogId, bool isSynonymsFile, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            try
            {
                bool isSuccess = _searchClient.WriteSearchFile(publishCatalogId, isSynonymsFile);
                if(!isSuccess)
                {
                    errorMessage = Admin_Resources.PublishSynonymsError;
                }
                return isSuccess;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.NotFound:
                        errorMessage = Admin_Resources.IndexNameNotExist;
                        return false;
                    case ErrorCodes.ProcessingFailed:
                        errorMessage = Admin_Resources.NoRecordsForSynonyms;
                        return false;
                    default:
                        errorMessage = Admin_Resources.PublishSynonymsError;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                errorMessage = isSynonymsFile ? Admin_Resources.ErrorSynonymsWrite : Admin_Resources.ErrorKeywordsWrite;
                return false;
            }
        }

        #endregion

        #region Search Keywords Redirect
        //Get Catalog keywords redirect list.
        [Obsolete("This method is deprecated and will be discontinued in upcoming versions." +
        " This method is marked as obsolete because catalogId will be fetched from the filters" +
        " Please use overload of this method which does not have catalogId as parameters ")]
        public SearchKeywordsRedirectListViewModel GetCatalogKeywordsList(int catalogId, ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter :", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { catalogId = catalogId, expands = expands, filters = filters, sortCollection = sortCollection });

            List<PublishCatalogModel> publishCatalogList = _ecommerceCatalogClient.GetPublishCatalogList(null, null, null, null, null).PublishCatalogs;

            if (catalogId == 0)
                catalogId = (publishCatalogList?.OrderBy(m => m.CatalogName).FirstOrDefault()?.PublishCatalogId).GetValueOrDefault();

            if (filters?.Count < 1)
            {
                filters.Add(ZnodeCatalogIndexEnum.PublishCatalogId.ToString(), FilterOperators.Equals, catalogId.ToString());
            }
            else
            {
                if (filters.Exists(x => x.FilterName == ZnodeCatalogIndexEnum.PublishCatalogId.ToString()))
                {
                    filters.RemoveAll(x => x.FilterName == ZnodeCatalogIndexEnum.PublishCatalogId.ToString());
                    filters.Add(new FilterTuple(ZnodeCatalogIndexEnum.PublishCatalogId.ToString(), FilterOperators.Equals, catalogId.ToString()));
                }
            }
            SearchKeywordsRedirectListModel list = _searchClient.GetCatalogKeywordsRedirectList(filters, expands, sortCollection, page, recordPerPage);

            SearchKeywordsRedirectListViewModel listViewModel = new SearchKeywordsRedirectListViewModel { KeywordsList = list?.KeywordsList?.ToViewModel<SearchKeywordsRedirectViewModel>().ToList() };

            if (listViewModel.KeywordsList?.Count > 0)
            {
                listViewModel.CatalogId = catalogId;
                listViewModel.CatalogName = publishCatalogList?.FirstOrDefault(x => x.PublishCatalogId == catalogId)?.CatalogName;
                SetListPagingData(listViewModel, list);
            }

            //Set tool menu for keywords and urls grid view.
            SetKeywordsListToolMenu(listViewModel);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            return listViewModel?.KeywordsList?.Count > 0 ? listViewModel
                : new SearchKeywordsRedirectListViewModel { KeywordsList = new List<SearchKeywordsRedirectViewModel>(), CatalogId = catalogId, CatalogName = publishCatalogList?.FirstOrDefault(x => x.PublishCatalogId == catalogId)?.CatalogName };
        }

        //Get Catalog keywords redirect list.
        public SearchKeywordsRedirectListViewModel GetCatalogKeywordsList(ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage)
        {
			ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
			ZnodeLogging.LogMessage("Input parameter :", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sortCollection = sortCollection });

			List<PublishCatalogModel> publishCatalogList = _ecommerceCatalogClient.GetPublishCatalogList(null, null, null, null, null).PublishCatalogs;
            
            int catalogId = GetCatalogIdFromFilters(filters);

            if (catalogId == 0)
            {
                catalogId = (publishCatalogList?.OrderBy(m => m.CatalogName).FirstOrDefault()?.PublishCatalogId).GetValueOrDefault();
                UpdateFilters(filters, FilterKeys.PublishCatalogId, FilterOperators.Equals, catalogId.ToString());
            }
            string catalogName = publishCatalogList?.FirstOrDefault(x => x.PublishCatalogId == catalogId)?.CatalogName;
            
            SearchKeywordsRedirectListModel list = _searchClient.GetCatalogKeywordsRedirectList(filters, expands, sortCollection, page, recordPerPage);

            SearchKeywordsRedirectListViewModel listViewModel = new SearchKeywordsRedirectListViewModel { KeywordsList = list?.KeywordsList?.ToViewModel<SearchKeywordsRedirectViewModel>().ToList() };

            if (listViewModel.KeywordsList?.Count > 0)
            {
                listViewModel.CatalogId = catalogId;
                listViewModel.CatalogName = catalogName;
                SetListPagingData(listViewModel, list);
            }

            //Set tool menu for keywords and urls grid view.
            SetKeywordsListToolMenu(listViewModel);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

			return listViewModel?.KeywordsList?.Count > 0 ? listViewModel
                : new SearchKeywordsRedirectListViewModel { KeywordsList = new List<SearchKeywordsRedirectViewModel>(), CatalogId = catalogId, CatalogName = catalogName };
        }

        //Create keywords for search.
        public virtual SearchKeywordsRedirectViewModel CreateSearchKeywordsRedirect(SearchKeywordsRedirectViewModel viewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                SearchKeywordsRedirectModel searchKeywordsRedirectModel = _searchClient.CreateSearchKeywordsRedirect(viewModel?.ToModel<SearchKeywordsRedirectModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                return IsNotNull(searchKeywordsRedirectModel) ? searchKeywordsRedirectModel.ToViewModel<SearchKeywordsRedirectViewModel>() : new SearchKeywordsRedirectViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
                return new SearchKeywordsRedirectViewModel { HasError = true, ErrorMessage = ex.ErrorMessage };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                return (SearchKeywordsRedirectViewModel)GetViewModelWithErrorMessage(viewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Get keywords details for search.
        public virtual SearchKeywordsRedirectViewModel GetSearchKeywordsRedirect(int searchKeywordsRedirectId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            if (searchKeywordsRedirectId > 0)
            {
                SearchKeywordsRedirectModel model = (_searchClient.GetSearchKeywordsRedirect(searchKeywordsRedirectId, null));

                //Maps keywords model to keywords view model.
                return IsNotNull(model) ? model.ToViewModel<SearchKeywordsRedirectViewModel>() : new SearchKeywordsRedirectViewModel();
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return new SearchKeywordsRedirectViewModel();
        }

        //Update keywords data for search.
        public virtual SearchKeywordsRedirectViewModel UpdateSearchKeywordsRedirect(SearchKeywordsRedirectViewModel viewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                SearchKeywordsRedirectModel searchKeywordsRedirectModel = _searchClient.UpdateSearchKeywordsRedirect(viewModel.ToModel<SearchKeywordsRedirectModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                return IsNotNull(searchKeywordsRedirectModel) ? searchKeywordsRedirectModel.ToViewModel<SearchKeywordsRedirectViewModel>() : new SearchKeywordsRedirectViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
                return new SearchKeywordsRedirectViewModel { HasError = true, ErrorMessage = ex.ErrorMessage };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                return (SearchKeywordsRedirectViewModel)GetViewModelWithErrorMessage(viewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        //Delete keywords by id.
        public virtual bool DeleteSearchKeywordsRedirect(string searchKeywordsRedirectIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            if (!string.IsNullOrEmpty(searchKeywordsRedirectIds))
            {
                try
                {
                    return _searchClient.DeleteSearchKeywordsRedirect(new ParameterModel { Ids = searchKeywordsRedirectIds });
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                    return false;
                }
            }
            return false;
        }
        #endregion

        #region CMSIndex

        //Get the portal index data.
        public virtual CMSPortalContentPageIndexViewModel GetCmsPageIndexData(int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter :", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { portalId = portalId });
            if (portalId > 0)
                return GetCmsPageSearchIndexByPortalId(portalId);
            else
                return GetFirstCmsPageIndexData();
        }

        //Get the portal index data.
        public virtual CMSPortalContentPageIndexViewModel GetCmsPageIndexData(FilterCollection filters, int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter :", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { portalId = portalId });
           
            List<PortalModel> portalDetailslist = _portalClient.GetPortalList(null, null, null, null, null)?.PortalList;

            if (portalId == 0)
            {
                int portalIdFromFilter = GetPortalIdFromFilters(filters);
                portalId = portalIdFromFilter > 0 ? portalIdFromFilter : (portalDetailslist?.OrderBy(m => m.StoreName).FirstOrDefault()?.PortalId).GetValueOrDefault();
            }
            UpdateFilters(filters, FilterKeys.PortalId, FilterOperators.Equals, portalId.ToString());

            string storeName = portalDetailslist?.FirstOrDefault(x => x.PortalId == portalId)?.StoreName;

            //To remove unwanted filters
            filters?.RemoveAll(x => x.Item1 != FilterKeys.PortalId);

            CMSPortalContentPageIndexViewModel cmsPageIndexData = _cmsPagesearchClient.GetCmsPageIndexData(new ExpandCollection() { ZnodeCMSSearchIndexEnum.PortalId.ToString(), ZnodeCMSSearchIndexEnum.PortalId.ToString() }, filters)?.ToViewModel<CMSPortalContentPageIndexViewModel>() ?? new CMSPortalContentPageIndexViewModel();

            cmsPageIndexData.PortalId = portalId;
            cmsPageIndexData.StoreName = storeName;

            return cmsPageIndexData;
        }

        //To get the portalId from filters.
        protected virtual int GetPortalIdFromFilters(FilterCollection filters)
        {
            int portalId = Convert.ToInt32(filters?.FirstOrDefault(filterTuple => filterTuple.Item1 == FilterKeys.PortalId.ToString().ToLower())?.Item3);
            return portalId;
        }

        //Insert data for create index.
        public virtual CMSPortalContentPageIndexViewModel InsertCreateCmsPageIndexData(CMSPortalContentPageIndexViewModel cmsPortalContentPageIndexViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            if (IsNotNull(cmsPortalContentPageIndexViewModel))
            {
                try
                {
                    cmsPortalContentPageIndexViewModel = _cmsPagesearchClient.InsertCreateCmsPageIndexData(cmsPortalContentPageIndexViewModel.ToModel<CMSPortalContentPageIndexModel>()).ToViewModel<CMSPortalContentPageIndexViewModel>();
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        case ErrorCodes.DuplicateSearchIndexName:
                            {
                                cmsPortalContentPageIndexViewModel.HasError = true;
                                cmsPortalContentPageIndexViewModel.ErrorMessage = Admin_Resources.ErrorIndexNameInUse;
                                break;
                            }
                        case ErrorCodes.StoreNotPublished:
                            {
                                cmsPortalContentPageIndexViewModel.HasError = true;
                                cmsPortalContentPageIndexViewModel.ErrorMessage = Admin_Resources.StoreNotPublished;
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                }
            }

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            return cmsPortalContentPageIndexViewModel;
        }

        //Get the search index monitor list.
        public virtual CMSSearchIndexMonitorListViewModel GetCmsPageSearchIndexMonitorList(int cmsSearchIndexId, int portalId, ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter :", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new
            {
                cmsSearchIndexId = cmsSearchIndexId,
                expands = expands,
                filters = filters,
                sortCollection = sortCollection
            });
            
            if (Equals(filters?.Exists(x => x.Item1 == FilterKeys.PortalId), false))
            {
                UpdateFilters(filters, FilterKeys.PortalId, FilterOperators.Equals, portalId.ToString());
            }
            filters.RemoveAll(filter => filter.FilterName == ZnodeCMSSearchIndexMonitorEnum.CMSSearchIndexId.ToString());
             
            if (IsNull(sortCollection))
                sortCollection = new SortCollection();

            sortCollection.Add(ZnodeCMSSearchIndexMonitorEnum.CMSSearchIndexMonitorId.ToString(), SortDirections.Descending);

            CMSSearchIndexMonitorListModel cmsSearchIndexMonitorList = _cmsPagesearchClient.GetCmsPageSearchIndexMonitorList(filters, expands, sortCollection, page, recordPerPage);
            CMSSearchIndexMonitorListViewModel cmsSearchIndexMonitorListViewModel = new CMSSearchIndexMonitorListViewModel
            {
                CMSSearchIndexMonitorList = cmsSearchIndexMonitorList?.CMSSearchIndexMonitorList?.ToViewModel<CMSSearchIndexMonitorViewModel>().ToList(),
                PortalId = cmsSearchIndexMonitorList.PortalId,
                CMSSearchIndexId = cmsSearchIndexMonitorList.CMSSearchIndexId
            };
            SetListPagingData(cmsSearchIndexMonitorListViewModel, cmsSearchIndexMonitorList);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return cmsSearchIndexMonitorListViewModel?.CMSSearchIndexMonitorList?.Count > 0 ? cmsSearchIndexMonitorListViewModel : new CMSSearchIndexMonitorListViewModel() { CMSSearchIndexMonitorList = new List<CMSSearchIndexMonitorViewModel>(), PortalId = cmsSearchIndexMonitorList.PortalId, CMSSearchIndexId = cmsSearchIndexMonitorList.CMSSearchIndexId };
        }
#endregion

#endregion

#region Private Method
//Gets the search index for a portal ID.
private PortalIndexViewModel GetSearchIndexByCatalogId(int publishCatalogId, string catalogName = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            PortalIndexViewModel portalIndexData = null;
            if (publishCatalogId > 0)
            {
                FilterCollection filters = new FilterCollection() { new FilterTuple(FilterKeys.PublishCatalogId, FilterOperators.Equals, publishCatalogId.ToString()) };

                portalIndexData = _searchClient.GetCatalogIndexData(new ExpandCollection() { ZnodeCatalogIndexEnum.ZnodePublishCatalog.ToString(), ZnodeCatalogIndexEnum.ZnodePublishCatalog.ToString() }, filters)?.ToViewModel<PortalIndexViewModel>() ?? new PortalIndexViewModel();

                portalIndexData.PublishCatalogId = publishCatalogId;

                if (!string.IsNullOrEmpty(catalogName))
                    portalIndexData.CatalogName = catalogName;
            }

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return portalIndexData;
        }

        // Set boost sort.
        private SortCollection SetBoostSortCollection(SortCollection sortCollection)
        {
			ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

			if (IsNull(sortCollection))
                sortCollection = new SortCollection();

            if (Equals(sortCollection?.Count, 0))
                sortCollection.Add(SortKeys.Boost, SortDirections.Descending);

			ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

			return sortCollection;
        }

        //Set tool menu for search synonyms list grid view.
        private void SetSearchSynonymsListToolMenu(SearchSynonymsListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('SearchSynonymsDeletePopUp')", ControllerName = "SearchConfiguration", ActionName = "DeleteSearchSynonyms" });
            }
        }

        //Set tool menu for search keywords list grid view.
        private void SetKeywordsListToolMenu(SearchKeywordsRedirectListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('KeywordsDeletePopUp')", ControllerName = "SearchConfiguration", ActionName = "DeleteSearchKeywordsRedirect" });
            }
        }

        //Get the CMS page search index for a portal ID.
        private CMSPortalContentPageIndexViewModel GetCmsPageSearchIndexByPortalId(int portalId, string storeName = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            if (portalId > 0)
            {
                CMSPortalContentPageIndexViewModel cmsPageIndexData = new CMSPortalContentPageIndexViewModel();

                FilterCollection filters = new FilterCollection() { new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, portalId.ToString()) };

                cmsPageIndexData = _cmsPagesearchClient.GetCmsPageIndexData(new ExpandCollection() { ZnodeCMSSearchIndexEnum.PortalId.ToString(), ZnodeCMSSearchIndexEnum.PortalId.ToString() }, filters)?.ToViewModel<CMSPortalContentPageIndexViewModel>() ?? new CMSPortalContentPageIndexViewModel();

                cmsPageIndexData.PortalId = portalId;

                if (!string.IsNullOrEmpty(storeName))
                    cmsPageIndexData.StoreName = storeName;

                return cmsPageIndexData;
            }

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return new CMSPortalContentPageIndexViewModel();
        }

        //Get the CMS page search index for a first portal.
        private CMSPortalContentPageIndexViewModel GetFirstCmsPageIndexData()
        {
            SortCollection sorts = new SortCollection();
            sorts.Add(ZnodePortalEnum.StoreName.ToString(), SortDirections.Ascending);

            //pagesize and pageindex is set as one to get single portal record 
            PortalModel portalDetails = _portalClient.GetPortalList(null, null, sorts, Convert.ToInt32(ZnodeConstant.One), Convert.ToInt32(ZnodeConstant.One))?.PortalList?.FirstOrDefault();            

            if (IsNotNull(portalDetails))
                return GetCmsPageSearchIndexByPortalId(portalDetails.PortalId, portalDetails.StoreName);

            return new CMSPortalContentPageIndexViewModel();
        }

        #endregion
    }
}