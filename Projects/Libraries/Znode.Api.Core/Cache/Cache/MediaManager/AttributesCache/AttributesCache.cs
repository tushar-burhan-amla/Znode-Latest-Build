using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class AttributesCache : BaseCache, IAttributesCache
    {
        #region Global Variable
        private readonly IAttributesService _service;
        #endregion

        #region Default Constructor
        public AttributesCache(IAttributesService attributesService)
        {
            _service = attributesService;
        }
        #endregion

        #region Attribute 
        //Get Attribute 
        public virtual string GetAttribute(int attributeId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get attribute by attribute id.
                var attribute = _service.GetAttribute(attributeId);
                if (!Equals(attribute, null))
                {
                    AttributesResponses response = new AttributesResponses { Attribute = attribute };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        //Get Attributes list
        public virtual string GetAttributes(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Attribute list
                var list = _service.GetAttributeList(Expands, Filters, Sorts, Page);
                if (list?.Attributes?.Count > 0)
                {
                    //Get response and insert it into cache.
                    AttributeListResponse response = new AttributeListResponse { Attributes = list.Attributes };
                    response.MapPagingDataFromModel(list);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        //Get attribute types list
        public virtual string GetAttributeTypes(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //AttributeTypes list
                var list = _service.GetAttributeTypeList();
                if (list?.AttributeTypes?.Count > 0)
                {
                    //Get response and insert it into cache.
                    AttributeListResponse response = new AttributeListResponse { AttributeTypes = list.AttributeTypes };
                    response.MapPagingDataFromModel(list);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get attribute inputvalidation list
        public virtual string GetInputValidations(int attributeTypeId, int attributeId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //AttributeTypes list
                var list = _service.GetAttributesInputValidations(attributeTypeId, attributeId);
                if (list?.InputValidations?.Count > 0)
                {
                    //Get response and insert it into cache.
                    AttributeListResponse response = new AttributeListResponse { InputValidations = list.InputValidations };
                    response.MapPagingDataFromModel(list);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get attribute validations list
        public virtual string GetDefaultValues(int AttributeId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                var defaultValues = _service.GetDefaultValues(AttributeId);
                if (!Equals(defaultValues, null))
                {
                    //Create response.
                    AttributeListResponse response = new AttributeListResponse { DefaultValues = defaultValues.DefaultValues };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
      
        #region Attribute Locale

        //Get Attribute local by attributelocal Id
        public virtual string GetAttributeLocale(int attributeId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get attribute local by attribute id.
                var attribute = _service.GetAttributeLocalByAttributeId(attributeId);
                if (!Equals(attribute, null))
                {
                    AttributeLocaleListResponse response = new AttributeLocaleListResponse { AttributeLocales = attribute.AttributeLocalList };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }

            return data;
        }

        #endregion
    }
}