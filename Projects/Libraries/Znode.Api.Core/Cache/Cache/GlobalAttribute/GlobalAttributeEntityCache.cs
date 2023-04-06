using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class GlobalAttributeEntityCache : BaseCache, IGlobalAttributeEntityCache
    {
        private readonly IGlobalAttributeGroupEntityService _service;

        public GlobalAttributeEntityCache(IGlobalAttributeGroupEntityService attributeEntityService)
        {
            _service = attributeEntityService;
        }

        public virtual string GetAllEntityList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Entity list
                GlobalEntityListModel globalEntity = _service.GetGlobalEntityList();
                if (globalEntity?.GlobalEntityList?.Count > 0)
                {
                    //Get response and insert it into cache.
                    GlobalEntityListResponse response = new GlobalEntityListResponse { GlobalEntityList = globalEntity.GlobalEntityList };

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetAssignedEntityAttributeGroups(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                GlobalAttributeGroupListModel entityGroup = _service.GetAssignedAttributeGroups(Expands, Filters, Sorts, Page);
                if (entityGroup?.AttributeGroupList?.Count > 0)
                {
                    //Get response and insert it into cache.
                    AttributeEntityGroupListResponse response = new AttributeEntityGroupListResponse { AttributeEntityGroupList = entityGroup.AttributeGroupList };

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetUnAssignedEntityAttributeGroups(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                GlobalAttributeGroupListModel entityGroup = _service.GetUnAssignedAttributeGroups(Expands, Filters, Sorts, Page);
                if (entityGroup?.AttributeGroupList?.Count > 0)
                {
                    //Get response and insert it into cache.
                    AttributeEntityGroupListResponse response = new AttributeEntityGroupListResponse { AttributeEntityGroupList = entityGroup.AttributeGroupList };

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Attribute Values based on the Entity Id.
        public virtual string GetEntityAttributeDetails(int entityId, string entityType, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Entity Attribute Values
                GlobalAttributeEntityDetailsModel entityValues = _service.GetEntityAttributeDetails(entityId, entityType);
                if (IsNotNull(entityValues))
                {
                    //Get response and insert it into cache.
                    GlobalAttributeEntityResponse response = new GlobalAttributeEntityResponse { EntityDetails = entityValues };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get publish global attributes.
        public virtual string GetGlobalEntityAttributes(int entityId, string entityType, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Entity Attribute Values.
                GlobalSelectedAttributeEntityDetailsModel entityValues = _service.GetGlobalEntityAttributes(entityId, entityType, Filters);
                if (IsNotNull(entityValues))
                {
                    //Get response and insert it into cache.
                    GlobalSelectedAttributeEntityResponse response = new GlobalSelectedAttributeEntityResponse { EntityDetails = entityValues };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //gets the global attributes based on the passed familyCode for setting the values for default container variant.
        public virtual string GetGlobalAttributesForDefaultVariantData(string familyCode, string entityType, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Entity Attribute Values
                GlobalAttributeEntityDetailsModel entityValues = _service.GetGlobalAttributesForDefaultVariantData(familyCode, entityType);
                if (IsNotNull(entityValues))
                {
                    //Get response and insert it into cache.
                    GlobalAttributeEntityResponse response = new GlobalAttributeEntityResponse { EntityDetails = entityValues };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Global Attribute details on the basis of Variant id and localeid
        public virtual string GetGlobalAttributesForAssociatedVariant(int variantId, string entityType, string routeUri, string routeTemplate, int localeId = 0)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Entity Attribute Values
                GlobalAttributeEntityDetailsModel entityValues = _service.GetGlobalAttributesForAssociatedVariant(variantId, entityType, localeId);
                if (IsNotNull(entityValues))
                {
                    //Get response and insert it into cache.
                    GlobalAttributeEntityResponse response = new GlobalAttributeEntityResponse { EntityDetails = entityValues };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
    }
}