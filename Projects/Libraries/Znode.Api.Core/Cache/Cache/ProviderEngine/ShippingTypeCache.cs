using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class ShippingTypeCache : BaseCache, IShippingTypeCache
    {
        #region Private Variables
        private readonly IShippingTypeService _ShippingTypeService;
        #endregion

        #region Constructor
        public ShippingTypeCache(IShippingTypeService ShippingTypeService)
        {
            _ShippingTypeService = ShippingTypeService;
        }
        #endregion

        #region Public Methods
        public virtual string GetShippingTypeList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ShippingTypeListModel list = _ShippingTypeService.GetShippingTypeList(Filters, Sorts, Page);

                //If list count is greater than 0 then Create a list response for ShippingType and insert into cache.
                if (list?.ShippingTypeList?.Count > 0)
                {
                    ShippingTypeListResponse response = new ShippingTypeListResponse { ShippingTypeList = list.ShippingTypeList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetShippingType(int shippingTypeId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);

            if (string.IsNullOrEmpty(data))
            {
                ShippingTypeModel ShippingType = _ShippingTypeService.GetShippingType(shippingTypeId);

                //If ShippingType has data then Create a response for ShippingType and insert into cache.
                if (!Equals(ShippingType, null))
                {
                    ShippingTypeResponse response = new ShippingTypeResponse { ShippingType = ShippingType };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetAllShippingTypesNotInDatabase(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ShippingTypeListModel list = _ShippingTypeService.GetAllShippingTypesNotInDatabase();

                //If list count is greater than 0 then Create a list response for ShippingType and insert into cache.
                if (list?.ShippingTypeList?.Count > 0)
                {
                    ShippingTypeListResponse response = new ShippingTypeListResponse { ShippingTypeList = list.ShippingTypeList };
                    response.MapPagingDataFromModel(list);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}