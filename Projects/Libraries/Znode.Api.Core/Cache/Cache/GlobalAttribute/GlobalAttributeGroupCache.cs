using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class GlobalAttributeGroupCache : BaseCache, IGlobalAttributeGroupCache
    {

        #region Private Variables
        private readonly IGlobalAttributeGroupService _service;
        #endregion

        #region Constructor
        public GlobalAttributeGroupCache(IGlobalAttributeGroupService globalAttributeGroupService)
        {
            _service = globalAttributeGroupService;
        }
        #endregion

        #region Public Methods
        //Get global attribute groups.
        public virtual string GetAttributeGroupList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                GlobalAttributeGroupListModel list = _service.GetAttributeGroupList(Expands, Filters, Sorts, Page);
                if (list?.AttributeGroupList?.Count > 0)
                {
                    //Create response.
                    GlobalAttributeGroupListResponse response = new GlobalAttributeGroupListResponse { AttributeGroups = list.AttributeGroupList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Gets global attribute group by id.
        public virtual string GetAttributeGroup(int id, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                GlobalAttributeGroupModel attributeGroup = _service.GetAttributeGroupById(id, Expands);
                if (IsNotNull(attributeGroup))
                {
                    //Create response.
                    GlobalAttributeGroupResponse response = new GlobalAttributeGroupResponse { AttributeGroup = attributeGroup };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get attribute group locale list.
        public virtual string GetAttributeGroupLocales(int attributeGroupId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                GlobalAttributeGroupLocaleListModel locales = _service.GetAttributeGroupLocale(attributeGroupId, Expands);
                if (IsNotNull(locales))
                {
                    //Create response.
                    GlobalAttributeGroupListResponse response = new GlobalAttributeGroupListResponse { AttributeGroupLocales = locales };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get assigned global attribute group by id.
        public virtual string AssignedAttributes(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                GlobalAttributeGroupMapperListModel attributeGroup = _service.GetAssignedAttributes(Expands, Filters, Sorts, Page);
                if (IsNotNull(attributeGroup))
                {
                    //Create response.
                    GlobalAttributeGroupListResponse response = new GlobalAttributeGroupListResponse { AttributeGroupMappers = attributeGroup };
                    response.MapPagingDataFromModel(attributeGroup);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get unassigned global attribute group by id.
        public virtual string UnAssignedAttributes(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                GlobalAttributeListModel unAssignedAttribute = _service.GetUnAssignedAttributes(Expands, Filters, Sorts, Page);
                if (IsNotNull(unAssignedAttribute))
                {
                    //Create response.
                    GlobalAttributeListResponse response = new GlobalAttributeListResponse { Attributes = unAssignedAttribute.Attributes };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}
