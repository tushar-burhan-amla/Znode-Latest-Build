using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class AttributeGroupCache : BaseCache, IAttributeGroupCache
    {
        #region Private Variables
        private readonly IAttributeGroupService _service;
        #endregion

        #region Constructor
        public AttributeGroupCache(IAttributeGroupService attributeGroupService)
        {
            _service = attributeGroupService;
        }
        #endregion

        #region Public Methods

        //Get attribute group using attributeGroupId.
        public virtual string GetAttributeGroup(int attributeGroupId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                var attributeGroup = _service.GetAttributeGroup(attributeGroupId);
                if (!Equals(attributeGroup, null))
                {
                    AttributeGroupResponse response = new AttributeGroupResponse { AttributeGroup = attributeGroup };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get a list of attribute groups.
        public virtual string GetAttributeGroups(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get Menu list
                var list = _service.GetAttributeGroupList(Expands, Filters, Sorts, Page);
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

        public virtual string GetAssignedAttributes(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                var list = _service.GetAssignedAttributes(Expands, Filters, Sorts, Page);
                if (list?.AttributeGroupMappers?.Count > 0)
                {
                    //Create response.
                    AttributeGroupMapperListResponse response = new AttributeGroupMapperListResponse { AttributeGroupMappers = list.AttributeGroupMappers };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetAttributeGroupLocales(int attributeGroupId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                AttributeGroupLocaleListModel locales = _service.GetAttributeGroupLocale(attributeGroupId, Expands);
                if (!Equals(locales, null))
                {
                    //Create response.
                    AttributeGroupListResponse response = new AttributeGroupListResponse { AttributeGroupLocales = locales };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Gets Attribute Group by ID
        public virtual string UnAssignedAttributes(int attributeGroupId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                AttributesListDataModel unAssignedAttribute = _service.GetUnAssignedAttributes(attributeGroupId,Expands,  Sorts, Page);
                if (!Equals(unAssignedAttribute, null))
                {
                    //Create response.
                    AttributeListResponse response = new AttributeListResponse { Attributes = unAssignedAttribute.Attributes };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}