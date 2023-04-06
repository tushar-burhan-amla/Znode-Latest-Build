using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class PIMAttributeGroupCache : BaseCache, IPIMAttributeGroupCache
    {
        #region Private Variables
        private readonly IPIMAttributeGroupService _service;
        #endregion

        #region Constructor
        public PIMAttributeGroupCache(IPIMAttributeGroupService pimAttributeGroupService)
        {
            _service = pimAttributeGroupService;
        }
        #endregion

        #region Public Methods
        //Get PIM Attribute groups
        public virtual string GetAttributeGroupList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PIMAttributeGroupListModel list = _service.GetAttributeGroupList(Expands, Filters, Sorts, Page);
                if (list?.AttributeGroupList?.Count > 0)
                {
                    //Create response.
                    PIMAttributeGroupListResponse response = new PIMAttributeGroupListResponse { AttributeGroups = list.AttributeGroupList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Gets PIM Attribute Group by ID
        public virtual string GetAttributeGroup(int id, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PIMAttributeGroupModel attributeGroup = _service.GetAttributeGroupById(id, Expands);
                if (!Equals(attributeGroup, null))
                {
                    //Create response.
                    PIMAttributeGroupResponse response = new PIMAttributeGroupResponse { AttributeGroup = attributeGroup };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Gets PIM Attribute Group by ID
        public virtual string AssignedAttributes(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PIMAttributeGroupMapperListModel attributeGroup = _service.GetAssignedAttributes(Expands, Filters, Sorts, Page);
                if (!Equals(attributeGroup, null))
                {
                    //Create response.
                    PIMAttributeGroupListResponse response = new PIMAttributeGroupListResponse { AttributeGroupMappers = attributeGroup};
                    response.MapPagingDataFromModel(attributeGroup);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Gets PIM Attribute Group by ID
        public virtual string UnAssignedAttributes(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PIMAttributeListModel unAssignedAttribute = _service.GetUnAssignedAttributes(Expands, Filters, Sorts, Page);
                if (!Equals(unAssignedAttribute, null))
                {
                    //Create response.
                    PIMAttributeListResponse response = new PIMAttributeListResponse { Attributes = unAssignedAttribute.Attributes };
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
                PIMAttributeGroupLocaleListModel locales = _service.GetAttributeGroupLocale(attributeGroupId, Expands);
                if (!Equals(locales, null))
                {
                    //Create response.
                    PIMAttributeGroupListResponse response = new PIMAttributeGroupListResponse { AttributeGroupLocales = locales };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}