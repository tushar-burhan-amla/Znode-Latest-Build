using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class AttributeFamilyCache : BaseCache, IAttributeFamilyCache
    {
        #region Private Variables
        private readonly IAttributeFamilyService _service;
        #endregion

        #region Constructor
        public AttributeFamilyCache(IAttributeFamilyService familyService)
        {
            _service = familyService;
        }
        #endregion

        #region Public Methods
        //Get the list of attribute families.
        public virtual string GetAttributeFamilyList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                AttributeFamilyListModel list = _service.GetAttributeFamilyList(Expands, Filters, Sorts, Page);
                if (list?.AttributeFamilies?.Count > 0)
                {
                    //Create response.
                    AttributeFamilyListResponse response = new AttributeFamilyListResponse { AttributeFamilies = list.AttributeFamilies };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetAssignedAttributeGroups(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                AttributeGroupListModel list = _service.GetAssignedAttributeGroups(Expands, Filters, Sorts, Page);
                if (list?.AttributeGroups?.Count > 0)
                {
                    //Create response.
                    AttributeGroupListResponse response = new AttributeGroupListResponse { AttributeGroups = list.AttributeGroups };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetAttributeFamily(int attributeFamilyId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                AttributeFamilyModel attributeFamily = _service.GetAttributeFamily(attributeFamilyId);
                if (!Equals(attributeFamily, null))
                {
                    //Create response.
                    AttributeFamilyResponse response = new AttributeFamilyResponse { AttributeFamily = attributeFamily };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetUnAssignedAttributeGroups(string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                AttributeGroupListModel list = _service.GetUnAssignedAttributeGroups(Filters, Sorts, Page);
                if (list?.AttributeGroups?.Count > 0)
                {
                    //Create Response
                    AttributeGroupListResponse response = new AttributeGroupListResponse { AttributeGroups = list.AttributeGroups };

                    //Apply Pagination Parameters
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #region Family Locale
        public virtual string GetFamilyLocale(int attributeFamilyId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                FamilyLocaleListModel locales = _service.GetFamilyLocale(attributeFamilyId);
                if (!Equals(locales, null))
                {
                    //Create response.
                    FamilyLocaleListResponse response = new FamilyLocaleListResponse { FamilyLocales = locales };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #endregion
    }
}