using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Znode.Admin.Core.Areas.Search.ViewModels;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Controllers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Areas.Search.Controllers
{
    public class SearchController : BaseController
    {
        #region Private Variables
        private readonly ISearchProfileAgent _searchProfileAgent;
        private readonly IStoreAgent _storeAgent;
        private readonly ISearchBoostAndBuryAgent _searchBoostAndBuryAgent;
        private readonly ITypeaheadAgent _typeaheadAgent;
        private readonly string AsideCatalogListPanel = "_asideCatalogListPanel";
        #endregion

        #region Constructor
        public SearchController(IStoreAgent storeAgent, ISearchProfileAgent searchProfileAgent, ISearchBoostAndBuryAgent searchBoostAndBuryAgent, ITypeaheadAgent typeahead)
        {
            _searchProfileAgent = searchProfileAgent;
            _storeAgent = storeAgent;
            _searchBoostAndBuryAgent = searchBoostAndBuryAgent;
            _typeaheadAgent = typeahead;
        }
        #endregion

        #region Public Methods

        #region Search Profile

        //Get Search Profile List
        public virtual ActionResult GetSearchProfiles([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int catalogId = 0, string catalogName = "", bool isAttributesProfile = false)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(isAttributesProfile ? GridListType.ZnodeSearchProfileAttribute.ToString() : GridListType.ZnodeSearchProfile.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(isAttributesProfile ? GridListType.ZnodeSearchProfileAttribute.ToString() : GridListType.ZnodeSearchProfile.ToString(), model);
            
            SearchProfileListViewModel SearchProfilesList = _searchProfileAgent.GetSearchProfileList(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            //Get the grid model.
            SearchProfilesList.GridModel = FilterHelpers.GetDynamicGridModel(model, SearchProfilesList?.SearchProfileList, isAttributesProfile ? GridListType.ZnodeSearchProfileAttribute.ToString() : GridListType.ZnodeSearchProfile.ToString(), string.Empty, null, true, true, SearchProfilesList?.GridModel?.FilterColumn?.ToolMenuList);
            SearchProfilesList.GridModel.TotalRecordCount = SearchProfilesList.TotalResults;
            if (Request.IsAjaxRequest())
                return PartialView("_List", SearchProfilesList);
            //Returns the search profile list.
            return ActionView(isAttributesProfile ? "_SearchAttributeProfile" : "List", SearchProfilesList);
        }

        //Creates search profile
        public virtual ActionResult CreateSearchProfile()
        {
            SearchProfileViewModel searchProfileViewModel = _searchProfileAgent.GetSearchProfileDetails();

            return ActionView("_CreateSearchProfile", searchProfileViewModel);
        }

        //Creates search profile
        [HttpPost]
        public virtual ActionResult CreateSearchProfile(SearchProfileViewModel model, List<string> fieldNames, List<int> fieldValues)
        {
            if (ModelState.IsValid)
            {
                model = _searchProfileAgent.CreateSearchProfile(model, fieldNames, fieldValues);
                if (!model.HasError && model.SearchProfileId > 0)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                    return RedirectToAction<SearchController>(x => x.GetTabStructureForProfile(model.SearchProfileId));
                }
                SetNotificationMessage(GetErrorNotificationMessage(model.ErrorMessage));
            }
            //Bind Search Profile Details List.
            model = _searchProfileAgent.GetSearchProfileDetails();
            return ActionView("_CreateSearchProfile", model);
        }

        //Edit Search Profile
        [HttpGet]
        public virtual ActionResult GetTabStructureForProfile(int searchProfileId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return ActionView("_GetTabStructure", _searchProfileAgent.GetTabularStructure(searchProfileId));
        }

        //Edit Search Profile
        [HttpGet]
        public virtual ActionResult Edit(int searchProfileId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            SearchProfileViewModel model = _searchProfileAgent.GetSearchProfile(searchProfileId);
            return ActionView("_EditSearchProfile", model);
        }

        [HttpPost]
        public virtual ActionResult Edit(SearchProfileViewModel searchProfileViewModel, List<string> fieldNames, List<int> fieldValues)
        {
            if (ModelState.IsValid)
            {
                SearchProfileViewModel searchProfile = _searchProfileAgent.Update(searchProfileViewModel, fieldNames, fieldValues);
                string notificationMessage = Admin_Resources.UpdateMessage;
                if (searchProfileViewModel.PublishRequired && !searchProfile.HasError)
                {
                    if (_searchProfileAgent.PublishSearchProfile(searchProfileViewModel.SearchProfileId))
                        notificationMessage += Admin_Resources.PublishSearchProfileSuccess;
                    else
                    {
                        searchProfile.HasError = true;
                        searchProfileViewModel.ErrorMessage = Admin_Resources.ErrorPublishSearchProfile;
                    }                        
                }
                SetNotificationMessage(searchProfile.HasError
                ? GetErrorNotificationMessage(searchProfileViewModel.ErrorMessage)
                : GetSuccessNotificationMessage(notificationMessage));
            }
            else
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
            return RedirectToAction<SearchController>(x => x.GetTabStructureForProfile(searchProfileViewModel.SearchProfileId));
        }

        //Delete SearchProfile.      
        public virtual JsonResult Delete(string searchProfileId, bool isDeletePublishSearchProfile)
        {
            bool status = false;
            string errorMessage = Admin_Resources.DeleteErrorMessage;
            if (!string.IsNullOrEmpty(searchProfileId))
                status = _searchProfileAgent.DeleteSearchProfile(searchProfileId, isDeletePublishSearchProfile, ref errorMessage);
            if(!isDeletePublishSearchProfile && status)
                return Json(new { status = status, message = Admin_Resources.DeleteSearchProfilePreserveIndexSuccess}, JsonRequestBehavior.AllowGet);
            return Json(new { status = status, message = status ? Admin_Resources.DeleteSearchProfileDeleteIndexSuccess : errorMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public virtual ActionResult GetSearchProduct(string searchText, SearchProfileViewModel searchProfileViewModel)
        {
            string errorMessage = string.Empty;
            searchProfileViewModel.SearchText = searchText;
            List<SearchProfileProductViewModel> productList = _searchProfileAgent.GetSearchProfileProduct(searchProfileViewModel, out errorMessage);

            ViewBag.ErrorMessage = errorMessage;
            return ActionView("SearchProfileProductList", productList);
        }

        [HttpGet]
        public virtual ActionResult GetFeaturesByQueryId(int queryId)
        {
            List<SearchFeatureViewModel> featureList = _searchProfileAgent.GetFeaturesByQueryId(queryId);
            return ActionView("_FeatureList", featureList);
        }

        //Get Catalog List
        public virtual ActionResult GetCatalogList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeStoreCatalog.ToString(), model);

            PortalCatalogListViewModel catalogList = _storeAgent.GetPublishCatalogList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            catalogList.GridModel = FilterHelpers.GetDynamicGridModel(model, catalogList.PortalCatalogs, GridListType.ZnodeStoreCatalog.ToString(), string.Empty, null, true);

            catalogList.GridModel.TotalRecordCount = catalogList.TotalResults;
            return ActionView(AsideCatalogListPanel, catalogList);
        }

        //Set default search profile.
        public virtual JsonResult SetDefaultSearchProfile(int portalId = 0, int searchProfileId = 0, int publishCatalogId = 0)
        {
            string message = Admin_Resources.SetDefaultError;
            if (searchProfileId > 0 && portalId > 0)
            {
                bool status = _searchProfileAgent.SetDefaultSearchProfile(portalId, searchProfileId, publishCatalogId);
                return Json(new { status = status, message = status ? Admin_Resources.SetDefault : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = message }, JsonRequestBehavior.AllowGet);
        }

        #region Search Facets
        public virtual ActionResult GetCatalogBasedAttributes([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int publishCatalogId = 0, string associatedAttributes = null, bool isAddAttributes = false)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeUnAssociatedSearchAttributes.ToString(), model);
            SearchAttributesListViewModel searchAttributeList;
           
            if (isAddAttributes)
            {
                searchAttributeList = _searchProfileAgent.GetCatalogBasedAttributes(associatedAttributes, publishCatalogId, model?.Expands, model?.Filters, model?.SortCollection, model?.Page, model?.RecordPerPage);
                searchAttributeList.GridModel = FilterHelpers.GetDynamicGridModel(model, searchAttributeList.SearchAttributeList, GridListType.ZnodeUnAssociatedSearchAttributes.ToString(), string.Empty, null, true);
                searchAttributeList.GridModel.TotalRecordCount = searchAttributeList.TotalResults;
                return PartialView("_addSearchableAttributesPopup", searchAttributeList);
            }

            // While creating a new search profile, by default product name and SKU searchable fields will be displayed.
            _searchProfileAgent.SetFiltersForDefaultSearchableAttributes(model.Filters);
            searchAttributeList = _searchProfileAgent.GetCatalogBasedAttributes(associatedAttributes, publishCatalogId, model?.Expands, model?.Filters, model?.SortCollection, model?.Page, model?.RecordPerPage);

            return PartialView("_SearchableAttributes", new SearchProfileViewModel() { SearchableAttributesList = searchAttributeList.SearchAttributeList.OrderBy(x => x.DisplayOrder).ToList(), PublishCatalogId = publishCatalogId });
        }

        //Get Search facets list associated to search profile.
        public virtual ActionResult GetAssociatedCatalogAttributes([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int searchProfileId = 0)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeSearchAttributes.ToString(), model);
            SearchAttributesListViewModel searchAttributeList = _searchProfileAgent.GetAssociatedUnAssociatedCatalogAttributes(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, searchProfileId, true);

            //Get the grid model.
            searchAttributeList.GridModel = FilterHelpers.GetDynamicGridModel(model, searchAttributeList?.SearchAttributeList, GridListType.ZnodeSearchAttributes.ToString(), string.Empty, null, true, true, searchAttributeList?.GridModel?.FilterColumn?.ToolMenuList);
            searchAttributeList.GridModel.TotalRecordCount = searchAttributeList.TotalResults;
            searchAttributeList.SearchProfileId = searchProfileId;
            return PartialView("_SearchfacetsAttributes", searchAttributeList);
        }

        //Get Search facets list associated to search profile.
        public virtual ActionResult GetUnAssociatedCatalogAttributes([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int searchProfileId = 0)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeUnAssociatedSearchAttributes.ToString(), model);
            SearchAttributesListViewModel searchAttributeList = _searchProfileAgent.GetAssociatedUnAssociatedCatalogAttributes(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, searchProfileId, false);

            //Get the grid model.
            searchAttributeList.GridModel = FilterHelpers.GetDynamicGridModel(model, searchAttributeList?.SearchAttributeList, GridListType.ZnodeUnAssociatedSearchAttributes.ToString(), string.Empty, null, true, true, searchAttributeList?.GridModel?.FilterColumn?.ToolMenuList);
            searchAttributeList.GridModel.TotalRecordCount = searchAttributeList.TotalResults;
            searchAttributeList.SearchProfileId = searchProfileId;
            return PartialView("_AttributesAsidePanel", searchAttributeList);
        }

        //Associate UnAssociated search attributes to search profile.
        public virtual JsonResult AssociateAttributesToProfile(int searchProfileId, string attributeCode)
        {
            SetNotificationMessage(_searchProfileAgent.AssociateAttributesToProfile(searchProfileId, attributeCode)
                ? GetSuccessNotificationMessage("Attribute associated to profile successfully") : GetErrorNotificationMessage(Admin_Resources.ErrorFailedToAssign));
            return Json(JsonRequestBehavior.AllowGet);
        }

        //UnAssociate search attributes from search profile.
        public virtual JsonResult UnAssociateAttributesFromProfile(string searchProfileAttributeMappingId)
        {
            string message = Admin_Resources.UnassignError;
            if (!string.IsNullOrEmpty(searchProfileAttributeMappingId))
            {
                bool status = _searchProfileAgent.UnAssociateAttributesFromProfile(searchProfileAttributeMappingId, out message);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Search Profile Triggers
        public virtual ActionResult GetSearchProfilesTriggers([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int searchProfileId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeSearchProfileTriggers.ToString(), model);
            SearchTriggersListViewModel searchTriggers = _searchProfileAgent.GetSearchProfileTriggerList(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, searchProfileId);
            //Get the grid model.
            searchTriggers.GridModel = FilterHelpers.GetDynamicGridModel(model, searchTriggers?.SearchTriggerList, GridListType.ZnodeSearchProfileTriggers.ToString(), string.Empty, null, true, true, searchTriggers?.GridModel?.FilterColumn?.ToolMenuList);
            searchTriggers.GridModel.TotalRecordCount = searchTriggers.TotalResults;
            searchTriggers.SearchProfileId = searchProfileId;

            //Returns the search profile list.
            return ActionView("_SearchProfileTriggerList", searchTriggers);
        }

        //Creates search profile
        public virtual ActionResult CreateSearchProfileTriggers(int searchProfileId)
           => ActionView("_CreateProfileTriggers", new SearchTriggersViewModel() { SearchProfileId = searchProfileId, UserProfileList = _searchProfileAgent.GetProfileList(0) });

        //Creates search profile
        [HttpPost]
        public virtual JsonResult CreateSearchProfileTriggers(SearchTriggersViewModel searchTriggersViewModel)
        {
            string errorMessage = Admin_Resources.ErrorFailedToCreate;
            bool status = false;
            if (ModelState.IsValid)
            {
                status = _searchProfileAgent.CreateSearchProfileTriggers(searchTriggersViewModel, ref errorMessage);
            }

            return Json(new { status = status, message = status ? Admin_Resources.RecordAddedSuccessMessage : errorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Edit Search Profile Triggers
        public virtual JsonResult EditSearchProfileTrigger(int searchProfileTriggerId, int searchProfileId, string data)
        {
            string message = Admin_Resources.UpdateErrorMessage;
            if (ModelState.IsValid && IsNotNull(data))
            {
                bool status = _searchProfileAgent.UpdateSearchProfileTriggers(searchProfileTriggerId, data, ref message);
                return Json(new { status = status, message = status ? Admin_Resources.UpdateMessage : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Delete SearchProfile.      
        public virtual JsonResult DeleteSearchTriggers(string searchProfileTriggerId)
        {
            bool status = false;
            if (!string.IsNullOrEmpty(searchProfileTriggerId))
                status = _searchProfileAgent.DeleteSearchProfileTriggers(searchProfileTriggerId);

            return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : Admin_Resources.DeleteErrorMessage }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public virtual ActionResult GetSearchProfilePortalList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int searchProfileId = 0)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeSearchPortalProfile.ToString(), model);
            SearchProfilePortalListViewModel portalList = _searchProfileAgent.GetSearchProfilePortalList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, searchProfileId);

            portalList.GridModel = FilterHelpers.GetDynamicGridModel(model, portalList.SearchProfilePortalList, GridListType.ZnodeSearchPortalProfile.ToString(), string.Empty, null, true, true, portalList?.GridModel?.FilterColumn?.ToolMenuList);

            portalList.GridModel.TotalRecordCount = portalList.TotalResults;
            portalList.SearchProfileId = searchProfileId;
            return ActionView(portalList);
        }

        public virtual JsonResult AssociatePortalToSearchProfile(int searchProfileId, string portalIds)
        {
            string message = Admin_Resources.ErrorFailedToAssociateSearchProfileToPortal;
            bool isAssociated = _searchProfileAgent.AssociatePortalToSearchProfile(searchProfileId, portalIds, true, out message);
            SetNotificationMessage(isAssociated ?
                GetSuccessNotificationMessage(Admin_Resources.SuccessToAssociateSearchProfileToPortal) : GetErrorNotificationMessage(message));
            return Json(new { status = isAssociated, message = isAssociated ? Admin_Resources.SuccessToAssociateSearchProfileToPortal : message }, JsonRequestBehavior.AllowGet);
        }

        public virtual JsonResult UnAssociatePortalToSearchProfile(string portalSearchProfileId = null, int searchProfileId = 0)
        {

            string message = Admin_Resources.UnassignError;
            if (!string.IsNullOrEmpty(portalSearchProfileId))
            {
                bool status = _searchProfileAgent.AssociatePortalToSearchProfile(searchProfileId, portalSearchProfileId, false, out message);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = message }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult GetUnAssociatedPortalList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int searchProfileId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeSearchProfilePortal.ToString(), model);
            StoreListViewModel storeList = _searchProfileAgent.GetUnAssociatedPortalList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, searchProfileId);

            //Get the grid model.
            storeList.GridModel = FilterHelpers.GetDynamicGridModel(model, storeList.StoreList, GridListType.ZnodeSearchProfilePortal.ToString(), string.Empty, null, true);

            //Set the total record count
            storeList.GridModel.TotalRecordCount = storeList.TotalResults;

            return ActionView("_AsideStorelistPanelPopup", storeList);
        }

        //Get Field Value List by catalog id.
        public virtual ActionResult GetfieldValuesList(int publishCatalogId, int searchProfileId = 0)
        {
            SearchProfileViewModel featureList = _searchProfileAgent.GetfieldValuesList(publishCatalogId, searchProfileId);
            return ActionView("_FieldValueRow", featureList);
        }

        #endregion

        #region Boost And Bury
        //Get Search boost and bury rules List
        public virtual ActionResult GetBoostAndBuryRules([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int catalogId = 0, string catalogName = "")
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeSearchCatalogRule.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeSearchCatalogRule.ToString(), model);

            SearchBoostAndBuryRuleListViewModel ruleList = _searchBoostAndBuryAgent.GetBoostAndBuryRules(model);

            //Get the grid model.
            ruleList.GridModel = FilterHelpers.GetDynamicGridModel(model, ruleList?.SearchBoostAndBuryRuleList, GridListType.ZnodeSearchCatalogRule.ToString(), string.Empty, null, true, true, ruleList?.GridModel?.FilterColumn?.ToolMenuList);
            ruleList.GridModel.TotalRecordCount = ruleList.TotalResults;

            if (Request.IsAjaxRequest())
                return PartialView("_GetBoostAndBuryRules", ruleList);
            //Returns the search profile list.
            return ActionView("GetBoostAndBuryRules", ruleList);
        }

        // Create boost and bury rule.
        public virtual ActionResult CreateBoostAndBuryRule(int publishCatalogId, string catalogName = "")
          => ActionView("_CreateBoostAndBuryRule", _searchBoostAndBuryAgent.BindPageDropdown(new SearchBoostAndBuryRuleViewModel() { PublishCatalogId = publishCatalogId, CatalogName = HttpUtility.UrlDecode(catalogName), StartDate = GetDate() }));

        // Create boost and bury rule.
        [HttpPost]
        public virtual ActionResult CreateBoostAndBuryRule(SearchBoostAndBuryRuleViewModel searchBoostAndBuryRuleViewModel)
        {
            if (ModelState.IsValid)
            {
                searchBoostAndBuryRuleViewModel = _searchBoostAndBuryAgent.CreateBoostAndBuryRule(searchBoostAndBuryRuleViewModel);

                return Json(new { status = !searchBoostAndBuryRuleViewModel.HasError, publishCatalogId = searchBoostAndBuryRuleViewModel.PublishCatalogId, catalogName = searchBoostAndBuryRuleViewModel.CatalogName, message = !searchBoostAndBuryRuleViewModel.HasError ? Admin_Resources.RecordCreationSuccessMessage : searchBoostAndBuryRuleViewModel.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = false, publishCatalogId = searchBoostAndBuryRuleViewModel.PublishCatalogId, catalogName = searchBoostAndBuryRuleViewModel.CatalogName, message = Admin_Resources.ErrorFailedToCreate }, JsonRequestBehavior.AllowGet);
        }

        //Get boost and bury rule
        [HttpGet]
        public virtual ActionResult UpdateBoostAndBuryRule(int searchCatalogRuleId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            SearchBoostAndBuryRuleViewModel model = _searchBoostAndBuryAgent.GetBoostAndBuryRule(searchCatalogRuleId);

            if (IsNotNull(model))
                _searchBoostAndBuryAgent.BindPageDropdown(model);

            return ActionView("_editBoostAndBuryRule", model);
        }

        //Update case request.
        [HttpPost]
        public virtual ActionResult UpdateBoostAndBuryRule(SearchBoostAndBuryRuleViewModel searchBoostAndBuryRuleViewModel)
        {
            if (ModelState.IsValid)
            {
                searchBoostAndBuryRuleViewModel = _searchBoostAndBuryAgent.UpdateBoostAndBuryRule(searchBoostAndBuryRuleViewModel);
                return Json(new { status = !searchBoostAndBuryRuleViewModel.HasError, publishCatalogId = searchBoostAndBuryRuleViewModel.PublishCatalogId, catalogName = searchBoostAndBuryRuleViewModel.CatalogName, message = !searchBoostAndBuryRuleViewModel.HasError ? Admin_Resources.UpdateMessage : Admin_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = false, publishCatalogId = searchBoostAndBuryRuleViewModel.PublishCatalogId, catalogName = searchBoostAndBuryRuleViewModel.CatalogName, message = Admin_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Delete catalog search rule.      
        public virtual JsonResult DeleteCatalogSearchRule(string searchCatalogRuleId)
        {
            bool status = false;
            string errorMessage = Admin_Resources.DeleteErrorMessage;
            if (!string.IsNullOrEmpty(searchCatalogRuleId))
                status = _searchBoostAndBuryAgent.DeleteCatalogSearchRule(searchCatalogRuleId, out errorMessage);

            return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : errorMessage }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult PauseCatalogSearchRule(string searchCatalogRuleId, bool isPause)
        {
            string message = string.Empty;

            bool status = _searchBoostAndBuryAgent.PauseCatalogSearchRule(searchCatalogRuleId, !isPause, out message);

            if (status && isPause)
                message = Admin_Resources.SearchRuleRestartSuccess;
            else if (status && !isPause)
                message = Admin_Resources.SearchRulePauseSuccess;
            else
                message = string.IsNullOrEmpty(message) ? Admin_Resources.ErrorMessageFailedToPauseOrRestartRule : message;

            return Json(new { status = status, message }, JsonRequestBehavior.AllowGet);

        }

        //Checks weather rule name exists.
        [HttpPost]
        public virtual JsonResult IsRuleNameExist(string ruleName, int publishCatalogId, int searchCatalogRuleId = 0)
            => Json(!_searchBoostAndBuryAgent.IsRuleNameExist(ruleName, publishCatalogId, searchCatalogRuleId), JsonRequestBehavior.AllowGet);

        //Get Auto suggestion for boost and bury.
        public virtual ActionResult GetAutoSuggestion(string query, string fieldName, int publishCatalogId)
        => Json(_searchBoostAndBuryAgent.GetAutoSuggestion(query, fieldName, publishCatalogId), JsonRequestBehavior.AllowGet);
        #endregion

        //Get Suggestions for type(Store,Catalog).
        [HttpGet]
        public virtual JsonResult GetSuggestions(string type, string fieldname, string query)
        => Json(_typeaheadAgent.GetAutocompleteList(query, type, fieldname), JsonRequestBehavior.AllowGet);

        //To get the catalog list that is not associated with any of the search profiles.
        [HttpGet]
        public virtual JsonResult GetUnassociatedCatalogList()
        {
           return Json(_searchProfileAgent.GetCatalogListForSearchProfile(), JsonRequestBehavior.AllowGet);
        }

        //Publish Search Profile based on the search Profile Id
        [HttpGet]
        public virtual JsonResult PublishSearchProfile(int searchProfileId)
        {
           bool publishStatus =_searchProfileAgent.PublishSearchProfile(searchProfileId);

            return Json(new { status = publishStatus, message = publishStatus ? Admin_Resources.PublishSearchProfileSuccess : Admin_Resources.ErrorPublishSearchProfile }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
