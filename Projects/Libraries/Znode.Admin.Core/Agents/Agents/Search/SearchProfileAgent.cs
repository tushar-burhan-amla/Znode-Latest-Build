using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Znode.Admin.Core.Areas.Search.ViewModels;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
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
    public class SearchProfileAgent : BaseAgent, ISearchProfileAgent
    {
        #region Private Variables
        private readonly ISearchProfileClient _searchProfileClient;
        private readonly IProfileClient _profileClient;
        private readonly IEcommerceCatalogClient _ecommerceCatalogClient;
        #endregion

        #region Constructor
        public SearchProfileAgent(ISearchProfileClient searchProfileClient, IEcommerceCatalogClient ecommerceCatalogClient, IProfileClient profileClient)
        {
            _searchProfileClient = GetClient(searchProfileClient);
            _ecommerceCatalogClient = GetClient(ecommerceCatalogClient);
            _profileClient = GetClient(profileClient);
        }
        #endregion

        #region Public Methods
        //Gets SearchProfile List
        [Obsolete("This method is deprecated and will be discontinued in upcoming versions." +
        " This method is marked as obsolute because catalogId will be fetched from the filters" +
        " Please use overload of this method which does not have catalogId and catalogName as parameters ")]
        public virtual SearchProfileListViewModel GetSearchProfileList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int catalogId, string catalogName)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Expands = expands, Filters = filters, Sorts = sorts, catalogId = catalogId, catalogName = catalogName });
            ////Get the sort collection for Search Profile id desc.
            sorts = HelperMethods.SortDesc(ZnodeSearchProfileEnum.SearchProfileId.ToString(), sorts);
            SearchProfileListModel searchProfileList = _searchProfileClient.GetSearchProfileList(expands, filters, sorts, pageIndex, pageSize);
            SearchProfileListViewModel listViewModel = new SearchProfileListViewModel { SearchProfileList = searchProfileList?.SearchProfileList?.ToViewModel<SearchProfileViewModel>().ToList(), PublishCatalogId = catalogId, CatalogName = catalogName };
            SetListPagingData(listViewModel, searchProfileList);

            //Set the Tool Menus for Search Profile List Grid View.
            SetSearchProfileToolMenus(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchProfileList?.SearchProfileList?.Count > 0 ? listViewModel : new SearchProfileListViewModel() { SearchProfileList = new List<SearchProfileViewModel>(), PublishCatalogId = catalogId, CatalogName = catalogName };
        }

        //Gets SearchProfile List
        public virtual SearchProfileListViewModel GetSearchProfileList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Expands = expands, Filters = filters, Sorts = sorts });

            // To get the search profiles for all catalogs
            filters?.Remove(filters?.FirstOrDefault(x => string.Equals(x.FilterName, ZnodeConstant.PublishCatalogId, StringComparison.OrdinalIgnoreCase)));

            //Get the sort collection for Search Profile id desc.
            sorts = HelperMethods.SortDesc(ZnodeSearchProfileEnum.SearchProfileId.ToString(), sorts);
            SearchProfileListModel searchProfileList = _searchProfileClient.GetSearchProfileList(expands, filters, sorts, pageIndex, pageSize);
            SearchProfileListViewModel listViewModel = new SearchProfileListViewModel { SearchProfileList = searchProfileList?.SearchProfileList?.ToViewModel<SearchProfileViewModel>().ToList()};
            SetListPagingData(listViewModel, searchProfileList);

            //Set the Tool Menus for Search Profile List Grid View.
            SetSearchProfileToolMenus(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchProfileList?.SearchProfileList?.Count > 0 ? listViewModel : new SearchProfileListViewModel() { SearchProfileList = new List<SearchProfileViewModel>()};
        }

        // Get the catalog list which are not associated with any search profile.
        public virtual List<AutoComplete> GetCatalogListForSearchProfile()
        {
            TypeaheadResponselistModel typeaheadResponselistModel =   _searchProfileClient.GetCatalogListForSearchProfile().ToViewModel<TypeaheadResponselistModel>();
            List<AutoComplete> list = new List<AutoComplete>();

            typeaheadResponselistModel?.Typeaheadlist?.ForEach(x =>
            {
                list.Add(new AutoComplete
                {
                    text = x.Name,
                    value = Convert.ToString(x.Id),
                    Name = x.Name,
                    Id = x.Id,
                    DisplayText = x.Name
                });
            });
            return list;
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

        public virtual SearchProfileViewModel CreateSearchProfile(SearchProfileViewModel model, List<string> fieldNames, List<int> fieldValues)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            try
            {
                model.FieldValueFactors = GetFieldValueDictionary(fieldNames, fieldValues);

                SearchProfileModel searchProfileModel = _searchProfileClient.Create(model.ToModel<SearchProfileModel>());
                if (searchProfileModel.SearchProfileId > 0)
                {
                    model.SearchProfileId = searchProfileModel.SearchProfileId;
                    ZnodeLogging.LogMessage("SearchProfileId:", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { SearchProfileId = model?.SearchProfileId });
                }
                else
                    return (SearchProfileViewModel)GetViewModelWithErrorMessage(model, Admin_Resources.ErrorSearchProfileAlreadyExists);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (SearchProfileViewModel)GetViewModelWithErrorMessage(model, Admin_Resources.ErrorSearchProfileAlreadyExists);
                    default:
                        return (SearchProfileViewModel)GetViewModelWithErrorMessage(model, Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                return (SearchProfileViewModel)GetViewModelWithErrorMessage(model, Admin_Resources.ErrorFailedToCreate);
            }
            return model;
        }

        ////Get Search Profile By search profile id.
        //public SearchProfileViewModel GetSearchProfile(int profileId)
        //{
        //    if (profileId > 0)
        //    {
        //        SearchProfileViewModel searchProfile = _searchProfileClient.GetSearchProfile(profileId).ToViewModel<SearchProfileViewModel>();
        //        searchProfile.FieldList = GetFiledValueList();
        //        return searchProfile;
        //    }
        //    return new SearchProfileViewModel() { FieldList = GetFiledValueList() };
        //}

        //Get Search Profile By search profile id.
        public virtual SearchProfileViewModel GetSearchProfile(int profileId)
          => profileId > 0 ? _searchProfileClient.GetSearchProfile(profileId).ToViewModel<SearchProfileViewModel>() : new SearchProfileViewModel();

        //Get Tab structure for search profile.
        public virtual SearchProfileViewModel GetTabularStructure(int profileId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            SearchProfileViewModel searchProfileViewModel = new SearchProfileViewModel();
            searchProfileViewModel = _searchProfileClient.GetSearchProfile(profileId)?.ToViewModel<SearchProfileViewModel>();

            TabViewListModel tabList = new TabViewListModel();
            SetTabData("Search Profile", $"/Search/Search/Edit?searchProfileId={profileId}&catalogId={searchProfileViewModel?.PublishCatalogId}&catalogName={searchProfileViewModel.CatalogName}", tabList);
            SetTabData("Facets", $"/Search/Search/GetAssociatedCatalogAttributes?searchProfileId={profileId}&catalogId={searchProfileViewModel?.PublishCatalogId}&catalogName={searchProfileViewModel.CatalogName}", tabList);
            tabList.MaintainAllTabData = false;

            searchProfileViewModel.Tabs = tabList;
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchProfileViewModel;
        }

        //updates the search profile view model
        public virtual SearchProfileViewModel Update(SearchProfileViewModel searchProfileViewModel, List<string> fieldNames, List<int> fieldValues)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            try
            {
                searchProfileViewModel.FieldValueFactors = GetFieldValueDictionary(fieldNames, fieldValues);

                SearchProfileModel searchProfileModel = _searchProfileClient.UpdateSearchProfile(searchProfileViewModel.ToModel<SearchProfileModel>());
                return IsNotNull(searchProfileModel) ? searchProfileModel.ToViewModel<SearchProfileViewModel>() : (SearchProfileViewModel)GetViewModelWithErrorMessage(new SearchProfileViewModel(), Admin_Resources.UpdateErrorMessage);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (SearchProfileViewModel)GetViewModelWithErrorMessage(searchProfileViewModel, Admin_Resources.ErrorSearchProfileAlreadyExists);
                    default:
                        return (SearchProfileViewModel)GetViewModelWithErrorMessage(searchProfileViewModel, Admin_Resources.UpdateErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                return (SearchProfileViewModel)GetViewModelWithErrorMessage(searchProfileViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        private List<KeyValuePair<string, int>> GetFieldValueDictionary(List<string> fieldNames, List<int> fieldValues)
        {
            if (fieldNames?.Count() > 0)
            {
                List<KeyValuePair<string, int>> fieldValueFactors = new List<KeyValuePair<string, int>>();
                for (int i = 0; i < fieldNames.Count; i++)
                {
                    if (string.IsNullOrEmpty(fieldNames[i]))
                    {
                        fieldNames.RemoveAt(i);
                        fieldValues.RemoveAt(i);
                        continue;
                    }
                    fieldValueFactors.Add(new KeyValuePair<string, int>(fieldNames[i], fieldValues[i]));
                }
                ZnodeLogging.LogMessage("Field value dictionary list count:", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { FieldValueDictionaryListCount = fieldValueFactors?.Count });
                return fieldValueFactors;
            }
            return new List<KeyValuePair<string, int>>();
        }

        //deletes search profile
        public virtual bool DeleteSearchProfile(string searchProfileId,bool isDeletePublishSearchProfile, ref string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            try
            {
                return _searchProfileClient.DeleteSearchProfile(new ParameterModel { Ids = searchProfileId , IsDeletePublishSearchProfile = isDeletePublishSearchProfile });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.DefaultDataDeletionError:
                        {
                            errorMessage = Admin_Resources.ErrorDefaultProfileDelete;
                            return false;
                        }
                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Gets unassociated portal list
        public virtual StoreListViewModel GetUnAssociatedPortalList(FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage, int searchProfileId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            filters.RemoveAll(filter => filter.FilterName == ZnodePortalSearchProfileEnum.SearchProfileId.ToString());
            filters.Add(new FilterTuple(ZnodePortalSearchProfileEnum.SearchProfileId.ToString(), FilterOperators.Equals, searchProfileId.ToString()));

            PortalListModel storeList = _searchProfileClient.GetUnAssociatedPortalList(null, filters, sortCollection, page, recordPerPage);

            StoreListViewModel storeListViewModel = new StoreListViewModel { StoreList = storeList?.PortalList?.ToViewModel<StoreViewModel>().ToList(), SearchProfileId = searchProfileId };
            storeListViewModel?.StoreList?.ForEach(item => { item.UrlEncodedStoreName = HttpUtility.UrlEncode(item.StoreName); });

            //Set the Tool Menus for Store List Grid View.
            SetStoreListToolMenus(storeListViewModel);

            SetListPagingData(storeListViewModel, storeList);
            //return storeListViewModel;
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return storeListViewModel;
        }

        //Gets list of search features 
        public virtual SearchProfileViewModel GetSearchProfileDetails()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            //if (publishCatalogId <= 0)
            //{
            //    List<PublishCatalogModel> publishCatalogList = _ecommerceCatalogClient.GetPublishCatalogList(null, null, null, null, null).PublishCatalogs;
            //    publishCatalogId = (publishCatalogList?.First()?.PublishCatalogId).GetValueOrDefault();
            //}
            //FilterCollection filters = new FilterCollection() { new FilterTuple(ZnodeSearchGlobalProductBoostEnum.PublishCatalogId.ToString(), FilterOperators.Equals, publishCatalogId.ToString()) };

            SearchProfileModel searchProfile = _searchProfileClient.GetSearchProfileDetails(new FilterCollection());

            SearchProfileViewModel searchProfileViewModel = searchProfile?.ToViewModel<SearchProfileViewModel>();
            //searchProfileViewModel.FieldList = GetFiledValueList(searchProfile.FieldList, searchProfile.FieldValueFactors);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchProfileViewModel;
        }

        public virtual List<SearchProfileProductViewModel> GetSearchProfileProduct(SearchProfileViewModel model, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            try
            {
                errorMessage = string.Empty;
                KeywordSearchModel searchResult = _searchProfileClient.GetSearchProfileProducts(model.ToModel<SearchProfileModel>(), null, null, null);
                return searchResult.Products?.Count > 0 ? searchResult.Products.ToViewModel<SearchProfileProductViewModel>().ToList() : new List<SearchProfileProductViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.InvalidData:
                        errorMessage = "Index name can not be blank.";
                        return new List<SearchProfileProductViewModel>();
                    case ErrorCodes.NotFound:
                        errorMessage = Admin_Resources.ErrorSearchIndexNotExist;
                        return new List<SearchProfileProductViewModel>();
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                errorMessage = "Search index does not exist.";
                return new List<SearchProfileProductViewModel>();
            }
            errorMessage = string.Empty;
            return new List<SearchProfileProductViewModel>();
        }

        //Set default search profile.
        public virtual bool SetDefaultSearchProfile(int portalId, int searchProfileId, int publishCatalogId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            try
            {
                return _searchProfileClient.SetDefaultSearchProfile(new PortalSearchProfileViewModel() { PortalId = portalId, SearchProfileId = searchProfileId, PublishCatalogId = publishCatalogId }?.ToModel<PortalSearchProfileModel>());
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                return false;
            }
        }

        public virtual bool AssociatePortalToSearchProfile(int searchProfileId, string portalIds, bool isAssociate, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            message = string.Empty;
            try
            {
                bool isAssociated = _searchProfileClient.AssociatePortalToSearchProfile(new SearchProfileParameterModel { Ids = portalIds, SearchProfileId = searchProfileId, IsAssociate = isAssociate, PortalSearchProfileIds = portalIds });
                message = Admin_Resources.ErrorFailedToAssociatePortalToSearchProfile;
                return isAssociated;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.DefaultDataDeletionError:
                        {
                            message = Admin_Resources.ErrorDefaultPortalSearchProfileDelete;
                            return false;
                        }
                    default:
                        {
                            return false;
                        }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Get Published Catalog List
        public virtual List<SelectListItem> GetPublishedCatalogList(int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            PortalCatalogListModel catalogList = _ecommerceCatalogClient.GetAssociatedPortalCatalog(new ParameterModel { Ids = portalId.ToString() });

            List<SelectListItem> selectedCatalogList = new List<SelectListItem>();
            if (portalId > 0 && catalogList?.PortalCatalogs?.Count > 0)
                catalogList?.PortalCatalogs?.ForEach(item => { selectedCatalogList.Add(new SelectListItem() { Text = item.CatalogName, Value = item.PublishCatalogId.ToString() }); });

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return selectedCatalogList;
        }

        //Get Features list by query id.
        public virtual List<SearchFeatureViewModel> GetFeaturesByQueryId(int queryId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            List<SearchFeatureModel> featureList = _searchProfileClient.GetFeaturesByQueryId(queryId);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return featureList?.Count > 0 ? featureList.ToViewModel<SearchFeatureViewModel>().ToList() : new List<SearchFeatureViewModel>();
        }

        //Get Searchable Attributes based on publishCatalogId
        public virtual SearchAttributesListViewModel GetCatalogBasedAttributes(string associatedAttributes, int publishCatalogId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { Expands = expands, Filters = filters, Sorts = sorts, associatedAttributes = associatedAttributes, publishCatalogId = publishCatalogId });
            SearchAttributesListViewModel listViewModel = new SearchAttributesListViewModel() { SearchAttributeList = new List<SearchAttributesViewModel>(), PublishCatalogId = publishCatalogId, AssociatedAttributes = associatedAttributes };

            if (publishCatalogId <= 0)
                publishCatalogId = BindCatalogDetails(listViewModel).GetValueOrDefault();

            if (!string.IsNullOrEmpty(associatedAttributes))
                associatedAttributes = new string(associatedAttributes.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());

            //Get the sort collection for Attribute Codes desc.
            sorts = HelperMethods.SortDesc(FilterKeys.AttributeCode.ToString(), sorts);
            if (filters?.Count <= 0)
                filters = new FilterCollection() { new FilterTuple(FilterKeys.PublishCatalogId, FilterOperators.Equals, publishCatalogId.ToString()) };
            else
                filters.Add(new FilterTuple(FilterKeys.PublishCatalogId, FilterOperators.Equals, publishCatalogId.ToString()));
            SearchAttributesListModel searchAttributeList = _searchProfileClient.GetCatalogBasedAttributes(new ParameterModel() { Ids = associatedAttributes }, expands, filters, sorts, pageIndex, pageSize);

            searchAttributeList?.SearchAttributeList?.ForEach(x => x.IsNgramEnabled = true);

            listViewModel.SearchAttributeList = searchAttributeList?.SearchAttributeList?.ToViewModel<SearchAttributesViewModel>()?.ToList();
            filters?.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.AttributeCode, StringComparison.InvariantCultureIgnoreCase));

            SetListPagingData(listViewModel, searchAttributeList);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return listViewModel;
        }

        #region Search Triggers
        //Gets Search Profile Triggers List
        public virtual SearchTriggersListViewModel GetSearchProfileTriggerList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int searchProfileId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            //Get the sort collection for Search Profile trigger id desc.
            sorts = HelperMethods.SortDesc(ZnodeSearchProfileTriggerEnum.SearchProfileTriggerId.ToString(), sorts);
            ZnodeLogging.LogMessage("Input parameters expands, filters, sort collection and searchProfileId:", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { Expands = expands, Filters = filters, Sorts = sorts, searchProfileId = searchProfileId });
            //Set search trigger filters.
            SetTriggerFilters(filters, searchProfileId);

            SearchTriggersListModel searchTriggerList = _searchProfileClient.GetSearchProfileTriggerList(expands, filters, sorts, pageIndex, pageSize);
            SearchTriggersListViewModel listViewModel = new SearchTriggersListViewModel { SearchTriggerList = searchTriggerList?.SearchTriggersList?.ToViewModel<SearchTriggersViewModel>()?.ToList() };
            listViewModel.SearchTriggerList?.ForEach(item => { item.UserProfileList = GetProfileList(item.ProfileId ?? 0); item.UserProfile = item.ProfileId.ToString(); });
            SetListPagingData(listViewModel, searchTriggerList);

            //Set the Tool Menus for Search Profile Trigger List Grid View.
            SetSearchProfileTriggerToolMenus(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchTriggerList?.SearchTriggersList?.Count() > 0 ? listViewModel : new SearchTriggersListViewModel() { SearchTriggerList = new List<SearchTriggersViewModel>() };
        }

        public virtual bool CreateSearchProfileTriggers(SearchTriggersViewModel searchTriggersViewModel, ref string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            try
            {
                return _searchProfileClient.CreateSearchProfileTriggers(searchTriggersViewModel?.ToModel<SearchTriggersModel>());
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.DuplicateQuantityError:
                        {
                            errorMessage = "Combination of Keyword and User Profile already exists";
                            return false;
                        }
                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                return false;
            }
        }

        public virtual SearchTriggersViewModel GetSearchProfileTrigger(int searchProfileTriggerId)
            => searchProfileTriggerId > 0 ? _searchProfileClient.GetSearchProfileTriggers(searchProfileTriggerId)?.ToViewModel<SearchTriggersViewModel>() : new SearchTriggersViewModel();

        //Updates the search profile trigger view model.
        public virtual bool UpdateSearchProfileTriggers(int searchProfileTriggerId, string data, ref string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters searchProfileTriggerId and data:", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { searchProfileTriggerId = searchProfileTriggerId, data = data });
            try
            {
                SearchTriggersViewModel searchTriggersViewModel = JsonConvert.DeserializeObject<SearchTriggersViewModel[]>(data)[0];
                if (string.IsNullOrEmpty(searchTriggersViewModel?.Keyword) && string.IsNullOrEmpty(searchTriggersViewModel?.UserProfile))
                {
                    message = Admin_Resources.TriggerNoDataUpdateError;
                    return false;
                }

                return _searchProfileClient.UpdateSearchProfileTriggers(searchTriggersViewModel?.ToModel<SearchTriggersModel>());
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        {
                            message = Admin_Resources.TriggerAlreadyExists;
                            return false;
                        }
                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Deletes search profile trigger.
        public virtual bool DeleteSearchProfileTriggers(string searchProfileTriggerId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            try
            {
                return _searchProfileClient.DeleteSearchProfileTriggers(new ParameterModel { Ids = searchProfileTriggerId });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Get the User Profiles.
        public virtual List<SelectListItem> GetProfileList(int profileId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(ZnodeProfileEnum.ParentProfileId.ToString(), FilterOperators.Equals, "null");

            return ToProfileListItems(_profileClient.GetProfileList(filters, null, null, null)?.Profiles, profileId);
        }

        public virtual SearchProfilePortalListViewModel GetSearchProfilePortalList(ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage, int searchProfileId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            filters.RemoveAll(filter => filter.FilterName == ZnodePortalSearchProfileEnum.SearchProfileId.ToString());
            filters.Add(new FilterTuple(ZnodePortalSearchProfileEnum.SearchProfileId.ToString(), FilterOperators.Equals, searchProfileId.ToString()));
            ZnodeLogging.LogMessage("Input parameters expands, filters, sort collection and searchProfileId:", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { Expands = expands, Filters = filters, Sorts = sortCollection, searchProfileId = searchProfileId });

            SearchProfilePortalListModel searchProfileList = _searchProfileClient.GetSearchProfilePortalList(expands, filters, sortCollection, page, recordPerPage);
            SearchProfilePortalListViewModel listViewModel = new SearchProfilePortalListViewModel { SearchProfilePortalList = searchProfileList?.SearchProfilePortalList?.ToViewModel<SearchProfilePortalViewModel>().ToList() };
            SetListPagingData(listViewModel, searchProfileList);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchProfileList?.SearchProfilePortalList?.Count > 0 ? listViewModel : new SearchProfilePortalListViewModel() { SearchProfilePortalList = new List<SearchProfilePortalViewModel>() };
        }
        #endregion

        #region Search Facets
        public virtual SearchAttributesListViewModel GetAssociatedUnAssociatedCatalogAttributes(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int searchProfileId, bool isAssociated)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            SearchAttributesListViewModel listViewModel = new SearchAttributesListViewModel() { SearchAttributeList = new List<SearchAttributesViewModel>(), SearchProfileId = searchProfileId };

            if (listViewModel.SearchProfileId > 0)
            {
                //Get the sort collection for gift card id desc.
                sorts = HelperMethods.SortDesc(ZnodeSearchProfileAttributeMappingEnum.SearchProfileAttributeMappingId.ToString(), sorts);
                ZnodeLogging.LogMessage("Input parameters expands, filters, sort collection and searchProfileId:", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { Expands = expands, Filters = filters, Sorts = sorts, searchProfileId = searchProfileId });

                //Set filters for search attributes.
                SetAttributeFilter(filters, isAssociated, listViewModel.SearchProfileId);

                SearchAttributesListModel searchAttributeList = _searchProfileClient.GetAssociatedUnAssociatedCatalogAttributes(expands, filters, sorts, pageIndex, pageSize);
                listViewModel.SearchAttributeList = searchAttributeList?.SearchAttributeList?.ToViewModel<SearchAttributesViewModel>()?.ToList();
                SetListPagingData(listViewModel, searchAttributeList);
            }

            if (isAssociated)
                //Set the Tool Menus for search attributes Grid View.
                SetSearchAttributeToolMenus(listViewModel);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return listViewModel;
        }

        //Associate UnAssociated search attributes to search profile.
        public virtual bool AssociateAttributesToProfile(int searchProfileId, string attributeCode)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            try
            {
                return _searchProfileClient.AssociateAttributesToProfile(new SearchAttributesModel { AttributeCode = attributeCode, SearchProfileId = searchProfileId });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //UnAssociate search attributes from search profile.
        public virtual bool UnAssociateAttributesFromProfile(string searchProfilesAttributeMappingIds, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            errorMessage = Admin_Resources.UnassignError;
            try
            {
                return _searchProfileClient.UnAssociateAttributesFromProfile(new ParameterModel { Ids = searchProfilesAttributeMappingIds });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.InvalidData:
                        errorMessage = Admin_Resources.TextInvalidData;
                        break;
                    case ErrorCodes.AssociationDeleteError:
                        errorMessage = Admin_Resources.ErrorDefaultProfileDelete;
                        break;
                    case ErrorCodes.RestrictSystemDefineDeletion:
                        errorMessage = Admin_Resources.ErrorDefaultProfileDelete;
                        break;
                    default:
                        errorMessage = Admin_Resources.UnassignError;
                        break;
                }
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                errorMessage = Admin_Resources.UnassignError;
                return false;
            }
        }
        #endregion

        public virtual SearchProfileViewModel GetfieldValuesList(int publishCatalogId, int searchProfileId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            SearchProfileModel model = _searchProfileClient.GetfieldValuesList(publishCatalogId, searchProfileId);
            SearchProfileViewModel searchProfileModel = model.ToViewModel<SearchProfileViewModel>();
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchProfileModel;
        }

        // To add the attribute code filter which will be used to fetch the default searchable attributes for a search profile.
        public virtual void SetFiltersForDefaultSearchableAttributes(FilterCollection filters)
        {
            List<string> attributeCodes = new List<string> { ZnodeConstant.ProductName, ZnodeConstant.ProductSKU, ZnodeConstant.Brand};
            filters?.Add(FilterKeys.AttributeCode, FilterOperators.In, string.Join(",", attributeCodes.Select(code => $"\"{code}\"")));
        }
        #endregion

        #region Private Methods

        //Set the Tool Menus for Store List Grid View.
        private void SetStoreListToolMenus(StoreListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('StoreDeletePopup');", ControllerName = "Store", ActionName = "DeleteStore" });
            }
        }

        //Set the Tool Menus for Search Profile List Grid View.
        private void SetSearchProfileToolMenus(SearchProfileListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new Models.GridModel();
                model.GridModel.FilterColumn = new Models.FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<Models.ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('DeleteSearchProfilePopup')", ControllerName = "Search", ActionName = "Delete" });
            }
        }

        // Bind Profile list.
        private List<SelectListItem> ToProfileListItems(IEnumerable<ProfileModel> model, int profileId)
        {
            List<SelectListItem> lstProfile = new List<SelectListItem>();
            if (IsNotNull(model))
            {
                lstProfile = (from item in model
                              orderby item.ProfileName ascending
                              select new SelectListItem
                              {
                                  Text = item.ProfileName,
                                  Value = item.ProfileId.ToString(),
                                  Selected = item.ProfileId == profileId
                              }).ToList();
            }
            lstProfile.Insert(0, new SelectListItem() { Text = "", Value = "" });
            ZnodeLogging.LogMessage("ProfileListItems count:", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { ProfileListItemsCount = lstProfile?.Count });
            return lstProfile;
        }

        //Set the Tool Menus for Search Profile Trigger List Grid View.
        private void SetSearchProfileTriggerToolMenus(SearchTriggersListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new Models.GridModel();
                model.GridModel.FilterColumn = new Models.FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<Models.ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('SearchTriggerDeletePopup')", ControllerName = "Search", ActionName = "DeleteSearchTriggers" });
                ZnodeLogging.LogMessage("Search profile trigger tools menu list count:", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { ToolMenuListCount = model.GridModel.FilterColumn.ToolMenuList.Count });
            }
        }

        //Get Default PublishCatalogId 
        private int? BindCatalogDetails(SearchAttributesListViewModel listViewModel)
            => _ecommerceCatalogClient.GetPublishCatalogList(null, null, null, null, null)?.PublishCatalogs?.FirstOrDefault().PublishCatalogId;

        //Set search trigger filters.
        private void SetTriggerFilters(FilterCollection filters, int searchProfileId)
        {
            if (HelperUtility.IsNull(filters))
                filters = new FilterCollection();

            filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeSearchProfileTriggerEnum.SearchProfileId.ToString(), StringComparison.CurrentCultureIgnoreCase));
            if (searchProfileId > 0)
                filters.Add(ZnodeSearchProfileTriggerEnum.SearchProfileId.ToString(), FilterOperators.Equals, searchProfileId.ToString());
        }

        private void SetSearchProfilePortalToolMenus(SearchProfilePortalListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new Models.GridModel();
                model.GridModel.FilterColumn = new Models.FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<Models.ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('SearchPortalDeletePopup')", ControllerName = "Search", ActionName = "UnAssociatePortalToSearchProfile" });
                ZnodeLogging.LogMessage("Search profile portal tools menu list count:", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { ToolMenuListCount = model.GridModel?.FilterColumn?.ToolMenuList.Count });
            }
        }

        //Set the tool menus for search attribute list grid view.
        private void SetSearchAttributeToolMenus(SearchAttributesListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new Models.GridModel();
                model.GridModel.FilterColumn = new Models.FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<Models.ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('DeletePopup')", ControllerName = "Search", ActionName = "UnAssociateAttributesFromProfile" });
                ZnodeLogging.LogMessage("Search attribute tool menus list count:", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { ToolMenuListCount = model?.GridModel?.FilterColumn?.ToolMenuList?.Count });
            }
        }

        //Set filters for search attributes.
        private void SetAttributeFilter(FilterCollection filters, bool isAssociated, int searchProfileId)
        {
            if (IsNull(filters))
                filters = new FilterCollection();

            filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeSearchProfileAttributeMappingEnum.SearchProfileId.ToString(), StringComparison.CurrentCultureIgnoreCase));
            filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.IsAssociated, StringComparison.CurrentCultureIgnoreCase));

            filters.Add(FilterKeys.IsAssociated, FilterOperators.Equals, isAssociated.ToString());

            if (searchProfileId > 0)
                filters.Add(new FilterTuple(ZnodeSearchProfileAttributeMappingEnum.SearchProfileId.ToString(), FilterOperators.Equals, searchProfileId.ToString()));
        }

        #endregion

        public virtual bool PublishSearchProfile(int searchProfileId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            bool publishStatus = false;

            if(searchProfileId> 0)
                 publishStatus =  _searchProfileClient.PublishSearchProfile(searchProfileId);
          
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return publishStatus;
        }

    }
}
