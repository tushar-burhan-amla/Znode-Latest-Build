using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class PIMAttributeFamilyCache : BaseCache, IPIMAttributeFamilyCache
    {
        #region Private Variable
        private readonly IPIMAttributeFamilyService _service;
        #endregion

        #region Constructor
        public PIMAttributeFamilyCache(IPIMAttributeFamilyService service)
        {
            _service = service;
        }
        #endregion

        #region Public Methods
        public virtual string GetAttributeFamilyList(string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PIMAttributeFamilyListModel list = _service.GetAttributeFamilyList(Expands, Filters, Sorts, Page);
                if (list?.PIMAttributeFamilies?.Count > 0)
                {
                    //Create Response
                    PIMAttributeFamilyListResponse response = new PIMAttributeFamilyListResponse { PIMAttributeFamilies = list.PIMAttributeFamilies };

                    //Apply Pagination Parameters
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetAssignedAttributeGroups(string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PIMAttributeGroupListModel list = _service.GetAssignedAttributeGroups(Expands, Filters, Sorts, Page);
                if (list?.AttributeGroupList?.Count > 0)
                {
                    //Create Response
                    PIMAttributeGroupListResponse response = new PIMAttributeGroupListResponse { AttributeGroups = list.AttributeGroupList };

                    //Apply Pagination Parameters
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetAttributeFamily(int attributeFamilyId, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                PIMAttributeFamilyModel attributeFamily = _service.GetAttributeFamily(attributeFamilyId);
                if (!Equals(attributeFamily, null))
                {
                    PIMAttributeFamilyResponse response = new PIMAttributeFamilyResponse { PIMAttributeFamily = attributeFamily };
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
                PIMAttributeGroupListModel list = _service.GetUnAssignedAttributeGroups(Filters, Sorts, Page);
                if (list?.AttributeGroupList?.Count > 0)
                {
                    //Create Response
                    PIMAttributeGroupListResponse response = new PIMAttributeGroupListResponse { AttributeGroups = list.AttributeGroupList };

                    //Apply Pagination Parameters
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetFamilyLocale(int attributeFamilyId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PIMFamilyLocaleListModel locales = _service.GetFamilyLocale(attributeFamilyId);
                if (!Equals(locales, null))
                {
                    //Create response.
                    PIMFamilyLocaleListResponse response = new PIMFamilyLocaleListResponse { FamilyLocales = locales };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get attributes list associated to group.
        public virtual string GetAssignedAttributes(string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PIMAttributeListModel list = _service.GetAssignedAttributes(Expands, Filters, Sorts, Page);
                if (list?.Attributes?.Count > 0)
                {
                    //Create Response
                    PIMAttributeListResponse response = new PIMAttributeListResponse { Attributes = list.Attributes };

                    //Apply Pagination Parameters
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get list of unassigned attributes.
        public virtual string GetUnAssignedAttributes(string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PIMAttributeListModel list = _service.GetUnAssignedAttributes(Filters, Sorts, Page);
                if (list?.Attributes?.Count > 0)
                {
                    //Create Response
                    PIMAttributeListResponse response = new PIMAttributeListResponse { Attributes = list.Attributes };

                    //Apply Pagination Parameters
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}