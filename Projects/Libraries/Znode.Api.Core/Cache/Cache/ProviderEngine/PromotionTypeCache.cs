using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class PromotionTypeCache : BaseCache, IPromotionTypeCache
    {
        #region Private Variables
        private readonly IPromotionTypeService _promotionTypeService;
        #endregion

        #region Constructor
        public PromotionTypeCache(IPromotionTypeService promotionTypeService)
        {
            _promotionTypeService = promotionTypeService;
        }
        #endregion

        #region Public Methods
        public virtual string GetPromotionTypeList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PromotionTypeListModel list = _promotionTypeService.GetPromotionTypeList(Filters, Sorts, Page);

                //If list count is greater than 0 then Create a list response for  PromotionType and insert into cache.
                if (list?.PromotionTypes?.Count > 0)
                {
                    PromotionTypeListResponse response = new PromotionTypeListResponse { PromotionTypeList = list.PromotionTypes };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetPromotionType(int promotionTypeId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);

            if (string.IsNullOrEmpty(data))
            {
                PromotionTypeModel PromotionType = _promotionTypeService.GetPromotionType(promotionTypeId);

                //If  PromotionType has data then Create a response for  PromotionType and insert into cache.
                if (!Equals(PromotionType, null))
                {
                    PromotionTypeResponse response = new PromotionTypeResponse { PromotionType = PromotionType };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetAllPromotionTypesNotInDatabase(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PromotionTypeListModel list = _promotionTypeService.GetAllPromotionTypesNotInDatabase();

                //If list count is greater than 0 then Create a list response for  PromotionType and insert into cache.
                if (list?.PromotionTypes?.Count > 0)
                {
                    PromotionTypeListResponse response = new PromotionTypeListResponse { PromotionTypeList = list.PromotionTypes };
                    response.MapPagingDataFromModel(list);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}