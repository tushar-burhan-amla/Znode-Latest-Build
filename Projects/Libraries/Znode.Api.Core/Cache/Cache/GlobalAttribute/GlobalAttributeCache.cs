using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class GlobalAttributeCache : BaseCache, IGlobalAttributeCache
    {
        #region Private Variables
        private readonly IGlobalAttributeService _service;
        #endregion

        #region Constructor
        public GlobalAttributeCache(IGlobalAttributeService attributesService)
        {
            _service = attributesService;
        }
        #endregion

        #region Public Methods
        public virtual string GetInputValidations(int typeId, int attributeId, string routeUri, string routeTemplate)
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
                    GlobalAttributeListResponse response = new GlobalAttributeListResponse { InputValidations = list.InputValidations };
                    response.MapPagingDataFromModel(list);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get list of global attributes.
        public virtual string GetAttributeList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                GlobalAttributeListModel list = _service.GetAttributeList(Expands, Filters, Sorts, Page);
                if (list?.Attributes?.Count > 0)
                {
                    //Create response.
                    GlobalAttributeListResponse response = new GlobalAttributeListResponse { Attributes = list.Attributes };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Gets global attribute by ID
        public virtual string GetAttribute(int id, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                GlobalAttributeModel attribute = _service.GetAttributeById(id, Expands);
                if (!Equals(attribute, null))
                {
                    //Create response.
                    GlobalAttributeResponse response = new GlobalAttributeResponse { Attribute = attribute };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get attribute locale data.
        public virtual string GetAttributeLocale(int globalAttributeId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                GlobalAttributeLocaleListModel locales = _service.GetAttributeLocale(globalAttributeId);
                if (!Equals(locales, null))
                {
                    //Create response.
                    GlobalAttributeLocaleListResponse response = new GlobalAttributeLocaleListResponse { AttributeLocales = locales.Locales };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetDefaultValues(int globalAttributeId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                GlobalAttributeDefaultValueListModel defaultValues = _service.GetDefaultValues(globalAttributeId);
                if (!Equals(defaultValues, null))
                {
                    //Create response.
                    GlobalAttributeLocaleListResponse response = new GlobalAttributeLocaleListResponse { DefaultValues = defaultValues.DefaultValues };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}
