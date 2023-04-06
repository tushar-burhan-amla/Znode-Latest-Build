using Newtonsoft.Json;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Controllers
{
    public class SearchConfigurationController : BaseController
    {
        private readonly ISearchConfigurationAgent _searchConfigurationAgent;
        private readonly IERPTaskSchedulerAgent _erpTaskSchedulerAgent;
        private readonly string storeListAsidePanelPopup = "_AsideStorelistPanelPopup";
        private readonly string catalogListAsidePanelPopup = "_AsideCataloglistPanelPopup";
        private readonly IHelperAgent _helperAgent;

        public SearchConfigurationController(ISearchConfigurationAgent searchConfigurationAgent, IERPTaskSchedulerAgent erpTaskSchedulerAgent, IStoreAgent storeAgent, IHelperAgent helperAgent)
        {
            _searchConfigurationAgent = searchConfigurationAgent;
            _erpTaskSchedulerAgent = erpTaskSchedulerAgent;
            _helperAgent = helperAgent;
        }

        #region Public Methods
        [HttpGet]
        public virtual ActionResult CreateIndex(int publishCatalogId = 0, string catalogName = "")
        {
            FilterCollectionDataModel model = new FilterCollectionDataModel { Filters = new FilterCollection() };
            
            //Get and Set Filters from Cookies if exists.
            if (publishCatalogId == 0)
                FilterHelpers.GetFiltersFromCookies(GridListType.ZnodeSearchIndexMonitor.ToString(), model);

            PortalIndexViewModel portalIndexData = _searchConfigurationAgent.GetPortalIndexData(model.Filters, publishCatalogId);

            SetSchedulerData(portalIndexData);

            portalIndexData.SchedulerData.IndexName = portalIndexData.IndexName;
            portalIndexData.SchedulerData.CatalogIndexId = portalIndexData.CatalogIndexId;
            portalIndexData.SchedulerData.CatalogId = portalIndexData.PublishCatalogId;
            portalIndexData.SchedulerData.SchedulerCallFor = ZnodeConstant.SearchIndex;
            portalIndexData.CatalogName = string.IsNullOrEmpty(portalIndexData?.CatalogName) ? catalogName : portalIndexData?.CatalogName;
            return (Request.IsAjaxRequest()) ? PartialView("_CreateIndex", portalIndexData) : ActionView("SearchSetting", portalIndexData);
        }

        public virtual ActionResult InsertCreateIndexData(PortalIndexViewModel portalIndexModel)
        {
            ModelState.Remove("SchedulerData.SchedulerName");
            ModelState.Remove("SchedulerData.TouchPointName");
            ModelState.Remove("SchedulerData.StartDate");
            ModelState.Remove("SchedulerData.StartTime");
            ModelState.Remove("SchedulerData.CronExpression");
            if (ModelState.IsValid)
            {
                portalIndexModel = _searchConfigurationAgent.InsertCreateIndexData(portalIndexModel);

                if (portalIndexModel.HasError)
                    SetNotificationMessage(GetErrorNotificationMessage(portalIndexModel.ErrorMessage));
                SetSchedulerData(portalIndexModel);

                return ActionView("_CreateIndex", portalIndexModel);
            }
            SetSchedulerData(portalIndexModel);
            portalIndexModel.HasError = true;
            return ActionView("_CreateIndex", portalIndexModel);
        }

        public virtual ActionResult GetSearchIndexMonitor(int catalogIndexId, [ModelBinder(typeof(PageDataBinder))]FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeSearchIndexMonitor.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeSearchIndexMonitor.ToString(), model);

            SearchIndexMonitorListViewModel searchIndexMonitorList = _searchConfigurationAgent.GetSearchIndexMonitorList(catalogIndexId, model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            searchIndexMonitorList.GridModel = FilterHelpers.GetDynamicGridModel(model, searchIndexMonitorList.SearchIndexMonitorList, GridListType.ZnodeSearchIndexMonitor.ToString(), string.Empty, null, true, true, searchIndexMonitorList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            searchIndexMonitorList.GridModel.TotalRecordCount = searchIndexMonitorList.TotalResults;
            searchIndexMonitorList.CatalogIndexId = catalogIndexId;
            return ActionView("_SearchIndexMonitorList", searchIndexMonitorList);
        }

        public virtual ActionResult GetSearchIndexServerStatusList(int searchIndexMonitorId, [ModelBinder(typeof(PageDataBinder))]FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeSearchIndexServerStatus.ToString(), model);
            SearchIndexServerStatusListViewModel searchIndexServerStatusList = _searchConfigurationAgent.GetSearchIndexServerStatusList(searchIndexMonitorId, model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            searchIndexServerStatusList.GridModel = FilterHelpers.GetDynamicGridModel(model, searchIndexServerStatusList.SearchIndexServerStatusList, GridListType.ZnodeSearchIndexServerStatus.ToString(), string.Empty, null, true, true, searchIndexServerStatusList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            searchIndexServerStatusList.GridModel.TotalRecordCount = searchIndexServerStatusList.TotalResults;
            searchIndexServerStatusList.SearchIndesMonitorId = searchIndexMonitorId;
            return ActionView("_SearchIndexServerStatus", searchIndexServerStatusList);
        }

        public virtual JsonResult SaveBoostValue(int publishCatalogId, string boostType, string data)
        {
            BoostDataViewModel boostData = JsonConvert.DeserializeObject<BoostDataViewModel[]>(data)[0];

            bool result =
                _searchConfigurationAgent.SaveBoostValues(
                    new BoostDataViewModel()
                    {
                        Boost = boostData.Boost,
                        BoostType = boostType,
                        ID = boostData.ID,
                        CatalogId = publishCatalogId,
                        PublishProductId = boostData.PublishProductId,
                        PublishCategoryId = boostData.PublishCategoryId,
                        PropertyName = boostData.PropertyName
                    });
            // Below code used for reset sort.
            if (result)
                TempData[AdminConstants.ResetSort] = true;
            return Json(new { status = result }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult GetGlobalProductBoost([ModelBinder(typeof(PageDataBinder))]FilterCollectionDataModel model, int catalogId = 0)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeSearchGlobalProductBoost.ToString(), model);

            model.SortCollection = SetBoostSort(model);

            //Get global product boost list.
            SearchGlobalProductBoostListViewModel searchGlobalProductBoostList = _searchConfigurationAgent.GetGlobalProductBoostList(catalogId, model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            searchGlobalProductBoostList.Grid = FilterHelpers.GetDynamicGridModel(model, searchGlobalProductBoostList.SearchGlobalProductBoostList, GridListType.ZnodeSearchGlobalProductBoost.ToString(), string.Empty, null, true, true, searchGlobalProductBoostList?.Grid?.FilterColumn?.ToolMenuList);

            //Set the total record count
            searchGlobalProductBoostList.Grid.TotalRecordCount = searchGlobalProductBoostList.TotalResults;

            if (catalogId > 0)
                searchGlobalProductBoostList.CatalogId = catalogId;

            return PartialView("_ProductBoostSetting", searchGlobalProductBoostList);
        }

        public virtual ActionResult GetGlobalProductCategoryBoost([ModelBinder(typeof(PageDataBinder))]FilterCollectionDataModel model, int catalogId = 0)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeSearchGlobalProductCategoryBoost.ToString(), model);
            model.SortCollection = SetBoostSort(model);

            //Get global product category boost list.
            SearchGlobalProductCategoryBoostListViewModel searchGlobalProductCategoryBoostList = _searchConfigurationAgent.GetGlobalProductCategoryBoostList(catalogId, model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            searchGlobalProductCategoryBoostList.Grid = FilterHelpers.GetDynamicGridModel(model, searchGlobalProductCategoryBoostList.SearchGlobalProductCategoryList, GridListType.ZnodeSearchGlobalProductCategoryBoost.ToString(), string.Empty, null, true, true, searchGlobalProductCategoryBoostList?.Grid?.FilterColumn?.ToolMenuList);

            //Set the total record count
            searchGlobalProductCategoryBoostList.Grid.TotalRecordCount = searchGlobalProductCategoryBoostList.TotalResults;

            if (catalogId > 0)
                searchGlobalProductCategoryBoostList.CatalogId = catalogId;

            return PartialView("_CategoryBoostSetting", searchGlobalProductCategoryBoostList);
        }

        public virtual ActionResult GetFieldLevelBoost([ModelBinder(typeof(PageDataBinder))]FilterCollectionDataModel model, int catalogId = 0)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeSearchDocumentMapping.ToString(), model);
            //Get Field level boost list.
            SearchDocumentMappingListViewModel fieldLevelBoostList = _searchConfigurationAgent.GetFieldLevelBoostList(catalogId, model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            fieldLevelBoostList.Grid = FilterHelpers.GetDynamicGridModel(model, fieldLevelBoostList.SearchDocumentMappingList, GridListType.ZnodeSearchDocumentMapping.ToString(), string.Empty, null, true, true, fieldLevelBoostList?.Grid?.FilterColumn?.ToolMenuList);

            //Set the total record count
            fieldLevelBoostList.Grid.TotalRecordCount = fieldLevelBoostList.TotalResults;

            if (catalogId > 0)
                fieldLevelBoostList.CatalogId = catalogId;

            return PartialView("_FieldBoostSetting", fieldLevelBoostList);
        }

        //Get Portal List
        public virtual ActionResult GetPortalList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeUserPortalList.ToString(), model);
            StoreListViewModel storeList = _searchConfigurationAgent.GetStoreList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            storeList.GridModel = FilterHelpers.GetDynamicGridModel(model, storeList.StoreList, GridListType.ZnodeUserPortalList.ToString(), string.Empty, null, true);

            //Set the total record count
            storeList.GridModel.TotalRecordCount = storeList.TotalResults;

            return ActionView(storeListAsidePanelPopup, storeList);
        }

        public virtual ActionResult GetCatalogList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeStoreCatalog.ToString(), model);
            PortalCatalogListViewModel catalogList = _searchConfigurationAgent.GetPublishCatalogList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            catalogList.GridModel = FilterHelpers.GetDynamicGridModel(model, catalogList.PortalCatalogs, GridListType.ZnodeStoreCatalog.ToString(), string.Empty, null, true);

            //Set the total record count
            catalogList.GridModel.TotalRecordCount = catalogList.TotalResults;
            return ActionView(catalogListAsidePanelPopup, catalogList);
        }

        //Delete elastic search index
        public virtual JsonResult DeleteIndex(int catalogIndexId)
        {
            bool status = false;
            string errorMessage = Admin_Resources.ErrorFailedToDeleteSearchIndex;
            if (catalogIndexId > 0)
                status = _searchConfigurationAgent.DeleteIndex(catalogIndexId, ref errorMessage);

            return Json(new { status = status, message = status ? Admin_Resources.SuccessSearchIndexDelete : errorMessage }, JsonRequestBehavior.AllowGet);
        }
        #region Synonyms
        //Create synonyms for search.
        [HttpGet]
        public virtual ActionResult CreateSearchSynonyms(int publishCatalogId)
            => ActionView(AdminConstants.CreateEditSearchSynonyms, new SearchSynonymsViewModel { PublishCatalogId = publishCatalogId });

        //Method to add synonyms for search.
        [HttpPost]
        public virtual ActionResult CreateSearchSynonyms(SearchSynonymsViewModel searchSynonymsViewModel)
        {
            if (IsNotNull(searchSynonymsViewModel))
            {
                //Create synonyms for search.
                searchSynonymsViewModel = _searchConfigurationAgent.CreateSearchSynonyms(searchSynonymsViewModel);
                return Json(new { status = searchSynonymsViewModel.SearchSynonymsId > 0 ? true : false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }

        //Method to edit synonyms for search.
        [HttpGet]
        public virtual ActionResult EditSearchSynonyms(int searchSynonymsId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return ActionView(AdminConstants.CreateEditSearchSynonyms, _searchConfigurationAgent.GetSearchSynonyms(searchSynonymsId));
        }

        //Update synonyms data for search.
        [HttpPost]
        public virtual ActionResult EditSearchSynonyms(SearchSynonymsViewModel searchSynonymsViewModel)
        {
            if (IsNotNull(searchSynonymsViewModel))
            {
                //Create synonyms for search.
                searchSynonymsViewModel = _searchConfigurationAgent.UpdateSearchSynonyms(searchSynonymsViewModel);
                return Json(new { status = searchSynonymsViewModel.HasError ? false : true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }

        //Method to get blog/news list.
        public virtual ActionResult GetSearchSynonymsList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int catalogId = 0)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeSearchSynonymsList.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeSearchSynonymsList.ToString(), model);
            //Get synonyms list for search.
            SearchSynonymsListViewModel searchSynonymsList = _searchConfigurationAgent.GetSearchSynonymsList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            searchSynonymsList.GridModel = FilterHelpers.GetDynamicGridModel(model, searchSynonymsList?.SynonymsList, GridListType.ZnodeSearchSynonymsList.ToString(), string.Empty, null, true, true, searchSynonymsList?.GridModel?.FilterColumn?.ToolMenuList);
            searchSynonymsList.GridModel.TotalRecordCount = searchSynonymsList.TotalResults;

            if (Request.IsAjaxRequest())
                return PartialView("_SynonymsList", searchSynonymsList);
            //Returns the blog/news list.
            return ActionView("_SearchSynonymsList", searchSynonymsList);
        }

        //Delete synonyms by id.
        public virtual JsonResult DeleteSearchSynonyms(string searchSynonymsId, int publishCataLogId = 0)
        {
            if (!string.IsNullOrEmpty(searchSynonymsId))
            {
                bool status = _searchConfigurationAgent.DeleteSearchSynonyms(searchSynonymsId, publishCataLogId);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public virtual JsonResult WriteSearchFile(int publishCataLogId, bool isSynonymsFile)
        {
            string errorMessage = isSynonymsFile ? Admin_Resources.ErrorSynonymsWrite : Admin_Resources.ErrorKeywordsWrite;
            if (publishCataLogId > 0)
            {
                bool status = _searchConfigurationAgent.WriteSearchFile(publishCataLogId, isSynonymsFile, out errorMessage);
                return Json(new { status = status, message = status ? Admin_Resources.SuccessPublishSynonyms : errorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Keywords Redirect
        //Get catalog keywords list.
        public virtual ActionResult GetCatalogKeywordsList([ModelBinder(typeof(PageDataBinder))]FilterCollectionDataModel model, int catalogId = 0)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeSearchKeywordsRedirectList.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeSearchKeywordsRedirectList.ToString(), model);
            //Get Field level boost list.
            SearchKeywordsRedirectListViewModel keywordList = _searchConfigurationAgent.GetCatalogKeywordsList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            keywordList.GridModel = FilterHelpers.GetDynamicGridModel(model, keywordList.KeywordsList, GridListType.ZnodeSearchKeywordsRedirectList.ToString(), string.Empty, null, true, true, keywordList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            keywordList.GridModel.TotalRecordCount = keywordList.TotalResults;
            if (Request.IsAjaxRequest())
                return PartialView("_KeywordsList", keywordList);

            return ActionView("_SearchKeywordsList", keywordList);
        }

        //Create keywords for search.
        [HttpGet]
        public virtual ActionResult CreateSearchKeywordsRedirect(int publishCatalogId)
            => ActionView(AdminConstants.CreateEditSearchKeywordsRedirect, new SearchKeywordsRedirectViewModel { PublishCatalogId = publishCatalogId });

        //Method to add keywords for search.
        [HttpPost]
        public virtual ActionResult CreateSearchKeywordsRedirect(SearchKeywordsRedirectViewModel searchKeywordsRedirectViewModel)
        {
            if (IsNotNull(searchKeywordsRedirectViewModel))
            {
                //Create keywords for search.
                searchKeywordsRedirectViewModel = _searchConfigurationAgent.CreateSearchKeywordsRedirect(searchKeywordsRedirectViewModel);
                return Json(new { status = searchKeywordsRedirectViewModel.SearchKeywordsRedirectId > 0 ? true : false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }

        //Method to edit keywords for search.
        [HttpGet]
        public virtual ActionResult EditSearchKeywordsRedirect(int searchKeywordsRedirectId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return ActionView(AdminConstants.CreateEditSearchKeywordsRedirect, _searchConfigurationAgent.GetSearchKeywordsRedirect(searchKeywordsRedirectId));
        }

        //Update keywords data for search.
        [HttpPost]
        public virtual ActionResult EditSearchKeywordsRedirect(SearchKeywordsRedirectViewModel searchKeywordsRedirectViewModel)
        {
            if (IsNotNull(searchKeywordsRedirectViewModel))
            {
                //Create keywords for search.
                searchKeywordsRedirectViewModel = _searchConfigurationAgent.UpdateSearchKeywordsRedirect(searchKeywordsRedirectViewModel);
                return Json(new { status = searchKeywordsRedirectViewModel.HasError ? false : true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }

        //Delete keywords by id.
        public virtual JsonResult DeleteSearchKeywordsRedirect(string searchKeywordsRedirectId)
        {
            if (!string.IsNullOrEmpty(searchKeywordsRedirectId))
            {
                bool status = _searchConfigurationAgent.DeleteSearchKeywordsRedirect(searchKeywordsRedirectId);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion

        #region Private Methods
        private SortCollection SetBoostSort(FilterCollectionDataModel model)
        {
            // Below code used for after save boost value in database set default sort by boost. 
            if (HelperUtility.IsNotNull(TempData[AdminConstants.ResetSort]))
            {
                if ((bool)TempData[AdminConstants.ResetSort])
                {
                    model.SortCollection = new SortCollection();
                    TempData[AdminConstants.ResetSortAfterEdit] = true;
                }
            }
            else if (HelperUtility.IsNotNull(TempData[AdminConstants.ResetSortAfterEdit]))
            {
                if ((bool)TempData[AdminConstants.ResetSortAfterEdit])
                    model.SortCollection = new SortCollection();
            }

            return model.SortCollection;
        }

        private void SetSchedulerData(PortalIndexViewModel portalIndexModel)
        {
            int erpTaskSchedulerId = _erpTaskSchedulerAgent.GetSchedulerIdByTouchPointName($"createIndex_{portalIndexModel.IndexName}", ZnodeConstant.SearchIndex);

            if (erpTaskSchedulerId == 0)
            {
                portalIndexModel.SchedulerData = new ERPTaskSchedulerViewModel
                {
                    SchedulerFrequency = ZnodeConstant.OneTime,
                    TouchPointName = $"createIndex_{portalIndexModel.IndexName}",
                    IsEnabled = true
                };
            }
            else
                portalIndexModel.SchedulerData = _erpTaskSchedulerAgent.GetERPTaskScheduler(erpTaskSchedulerId);
        }
        #endregion

        //Check Synonym code already present in DB.
        [HttpGet]
        public virtual JsonResult IsSynonymCodeExists(string codeField)
        {
            bool isExist = true;
            if (!string.IsNullOrEmpty(codeField))
            {
                isExist = _helperAgent.IsCodeExists(codeField, CodeFieldService.SearchService.ToString(), CodeFieldService.IsCodeExists.ToString());
            }
            return Json(new { isExist = !isExist, message = Admin_Resources.ErrorSynonymCodeExist }, JsonRequestBehavior.AllowGet);
        }
    }
}
