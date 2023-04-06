using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class GlobalAttributeFamilyCache : BaseCache, IGlobalAttributeFamilyCache
    {

        #region Private Variables
        private readonly IGlobalAttributeFamilyService _service;
        #endregion

        #region Constructor
        public GlobalAttributeFamilyCache(IGlobalAttributeFamilyService globalAttributeFamilyService)
        {
            _service = globalAttributeFamilyService;
        }
        #endregion

        #region Public Methods
        //Get global attribute family list
        public virtual string List(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                GlobalAttributeFamilyListModel list = _service.List(Expands, Filters, Sorts, Page);
                if (list?.AttributeFamilyList?.Count > 0)
                {
                    //Create response.
                    GlobalAttributeFamilyListResponse response = new GlobalAttributeFamilyListResponse { AttributeFamily = list.AttributeFamilyList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Gets global attribute family
        public virtual string GetAttributeFamily(string familyCode, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                GlobalAttributeFamilyModel attributeFamily = _service.GetAttributeFamily(familyCode);
                if (HelperUtility.IsNotNull(attributeFamily))
                {
                    //Create response.
                    GlobalAttributeFamilyResponse response = new GlobalAttributeFamilyResponse { AttributeFamily = attributeFamily };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get assigned attribute groups
        public virtual string GetAssignedAttributeGroups(string familyCode, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                GlobalAttributeGroupListModel associatedGroups = _service.GetAssignedAttributeGroups(familyCode);
                if (associatedGroups?.AttributeGroupList?.Count > 0)
                {
                    //Get response and insert it into cache.
                    GlobalAttributeGroupListResponse response = new GlobalAttributeGroupListResponse { AttributeGroups = associatedGroups.AttributeGroupList };

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        // Get unassigned attribute groups
        public virtual string GetUnassignedAttributeGroups(string familyCode, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                GlobalAttributeGroupListModel unassociatedGroups = _service.GetUnassignedAttributeGroups(familyCode);
                if (unassociatedGroups?.AttributeGroupList?.Count > 0)
                {
                    //Get response and insert it into cache.
                    GlobalAttributeGroupListResponse response = new GlobalAttributeGroupListResponse { AttributeGroups = unassociatedGroups.AttributeGroupList };

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get attribute family locale list.
        public virtual string GetAttributeFamilyLocales(string familyCode, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                GlobalAttributeFamilyLocaleListModel locales = _service.GetAttributeFamilyLocale(familyCode);
                if (HelperUtility.IsNotNull(locales))
                {
                    //Create response.
                    GlobalAttributeFamilyListResponse response = new GlobalAttributeFamilyListResponse { AttributeFamilyLocales = locales };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion
    }
}
