using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class PIMAttributeCache : BaseCache, IPIMAttributeCache
    {
        #region Private Variables
        private readonly IPIMAttributeService _service;
        #endregion

        #region Constructor
        public PIMAttributeCache(IPIMAttributeService attributesService)
        {
            _service = attributesService;
        }
        #endregion

        #region Public Methods
        //Get PIM Attribute
        public virtual string GetAttributeList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PIMAttributeListModel list = _service.GetAttributeList(Expands, Filters, Sorts, Page);
                if (list?.Attributes?.Count > 0)
                {
                    //Create response.
                    PIMAttributeListResponse response = new PIMAttributeListResponse { Attributes = list.Attributes };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Gets PIM Attribute by ID
        public virtual string GetAttribute(int id, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PIMAttributeModel attribute = _service.GetAttributeById(id, Expands);
                if (!Equals(attribute, null))
                {
                    //Create response.
                    PIMAttributeResponse response = new PIMAttributeResponse { Attribute = attribute };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get attribute types list
        public virtual string GetAttributeTypes(bool isCategory,string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //AttributeTypes list
                PIMAttributeTypeListModel list = _service.GetAttributeTypes(isCategory);
                if (list?.Types?.Count > 0)
                {
                    //Get response and insert it into cache.
                    PIMAttributeListResponse response = new PIMAttributeListResponse { AttributeTypes = list.Types };
                    response.MapPagingDataFromModel(list);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetInputValidations(int typeId,int attributeId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //AttributeTypes list
                var list = _service.GetInputValidations(typeId, attributeId);
                if (list?.InputValidations?.Count > 0)
                {
                    //Get response and insert it into cache.
                    PIMAttributeListResponse response = new PIMAttributeListResponse { InputValidations = list.InputValidations };
                    response.MapPagingDataFromModel(list);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get attribute types list
        public virtual string FrontEndProperties(int pimAttributeId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PIMFrontPropertiesModel frontEndProperties = _service.FrontEndProperties(pimAttributeId);
                if (!Equals(frontEndProperties, null))
                {
                    //Create response.
                    PIMFrontPropertiesResponse response = new PIMFrontPropertiesResponse { FrontEndProperties = frontEndProperties };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetAttributeLocale(int pimAttributeId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PIMAttributeLocaleListModel locales = _service.GetAttributeLocale(pimAttributeId);
                if (!Equals(locales, null))
                {
                    //Create response.
                    PIMAttributeLocaleListResponce response = new PIMAttributeLocaleListResponce { AttributeLocales = locales.Locales };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetDefaultValues(int pimAttributeId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PIMAttributeDefaultValueListModel defaultValues = _service.GetDefaultValues(pimAttributeId);
                if (!Equals(defaultValues, null))
                {
                    //Create response.
                    PIMAttributeLocaleListResponce response = new PIMAttributeLocaleListResponce { DefaultValues = defaultValues.DefaultValues };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }


        
        #endregion
    }
}