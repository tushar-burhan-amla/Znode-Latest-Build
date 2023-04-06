using System;
using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Api.Models.Responses.Search;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class SearchProfileCache : BaseCache, ISearchProfileCache
    {
        private readonly ISearchProfileService _service;

        public SearchProfileCache(ISearchProfileService searchProfileService)
        {
            _service = searchProfileService;
        }

        //get search profile list
        public string GetSearchProfilesList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get Search Profile list.
                SearchProfileListModel list = _service.GetSearchProfileList(Expands, Filters, Sorts, Page);
                if (IsNotNull(list))
                {
                    //Create response.
                    SearchProfileListResponse response = new SearchProfileListResponse { SearchProfileList = list.SearchProfileList, PublishCatalogId = list.PublishCatalogId, CatalogName = list.CatalogName };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get SearchProfile by search profile id.
        public string GetSearchProfile(int searchProfileId, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                SearchProfileModel searchProfileModel = _service.GetSearchProfile(searchProfileId);
                if (IsNotNull(searchProfileModel))
                    data = InsertIntoCache(routeUri, routeTemplate, new SearchProfileResponse { SearchProfile = searchProfileModel });
            }
            return data;
        }

        //Gets list of search features
        public virtual string GetSearchProfileDetails(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Account list
                SearchProfileModel details = _service.GetSearchProfileDetails(Filters);
                if (!Equals(details, null))
                {
                    //Get response and insert it into cache.
                    SearchProfileResponse response = new SearchProfileResponse { SearchProfile = details };

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public string GetFeaturesByQueryId(int queryId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get GiftCard list.
                List<SearchFeatureModel> list = _service.GetFeaturesByQueryId(queryId);
                if (IsNotNull(list))
                {
                    //Create response.
                    SearchFeaturesListResponse response = new SearchFeaturesListResponse { SearchFeaturesList = list };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #region Search Triggers
        //Gets search profile trigger list.
        public string GetSearchTriggerList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get Search Profile list.
                SearchTriggersListModel list = _service.GetSearchTriggersList(Expands, Filters, Sorts, Page);
                if (IsNotNull(list))
                {
                    //Create response.
                    SearchTriggersListResponse response = new SearchTriggersListResponse { SearchTriggersList = list.SearchTriggersList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Search Profile Trigger on the basis of searchProfileTriggerId.
        public string GetSearchTrigger(int searchProfileTriggerId, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                SearchTriggersModel searchTriggerModel = _service.GetSearchTrigger(searchProfileTriggerId);
                if (IsNotNull(searchTriggerModel))
                    data = InsertIntoCache(routeUri, routeTemplate, new SearchTriggersResponse { SearchTrigger = searchTriggerModel });
            }
            return data;
        }
        #endregion

        #region Search Facets
        //Get search catalog attribute list.
        public string GetAssociatedUnAssociatedCatalogAttributes(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                SearchAttributesListModel list = _service.GetAssociatedUnAssociatedCatalogAttributes(Expands, Filters, Sorts, Page);
                if (IsNotNull(list))
                {
                    //Create response.
                    SearchAttributesListResponse response = new SearchAttributesListResponse { SearchAttributesList = list.SearchAttributeList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        public string SearchProfilePortalList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get Search Profile list.
                SearchProfilePortalListModel list = _service.SearchProfilePortalList(Expands, Filters, Sorts, Page);
                if (IsNotNull(list))
                {
                    //Create response.
                    SearchProfilePortalListResponse response = new SearchProfilePortalListResponse { SearchProfilePortalList = list.SearchProfilePortalList};

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Gets catalog based attributes
        public string GetCatalogBasedAttributes(ParameterModel associatedAttributes, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get GiftCard list.
                SearchAttributesListModel list = _service.GetCatalogBasedSearchableAttributes(associatedAttributes, Expands, Filters, Sorts, Page);
                if (IsNotNull(list))
                {
                    //Create response.
                    SearchAttributesListResponse response = new SearchAttributesListResponse { SearchAttributesList = list?.SearchAttributeList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get UnAssociated Portal List
        public string GetUnAssociatedPortalList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get Search Profile list.
                PortalListModel list = _service.GetUnAssociatedPortalList(Expands, Filters, Sorts, Page);
                if (list?.PortalList?.Count > 0)
                {
                    //Create response.
                    PortalListResponse response = new PortalListResponse { PortalList = list.PortalList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Field value list by catalog id.
        public string GetFieldValuesList(int publishCatalogId, int searchProfileId, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);

            if (string.IsNullOrEmpty(data))
            {
                SearchProfileModel searchProfileModel = _service.GetFieldValuesList(publishCatalogId, searchProfileId);
                if (IsNotNull(searchProfileModel))
                    data = InsertIntoCache(routeUri, routeTemplate, new SearchProfileResponse { SearchProfile = searchProfileModel });
            }
            return data;
        }
    }
}
