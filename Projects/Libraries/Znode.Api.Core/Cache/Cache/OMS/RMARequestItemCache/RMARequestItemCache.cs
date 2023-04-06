using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Znode.Engine.Api.Cache
{
    public class RMARequestItemCache : BaseCache, IRMARequestItemCache
    {
        #region Private Variables
        private readonly IRMARequestItemService _service;
        #endregion

        #region Constructor
        public RMARequestItemCache(IRMARequestItemService rmaRequestItemService)
        {
            _service = rmaRequestItemService;
        }
        #endregion

        public virtual string GetRMARequestItems(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (Equals(data, null))
            {
                RMARequestItemListModel list = _service.GetRMARequestItemList(Expands, Filters, Sorts, Page);
                if (list.RMARequestItemList.Count > 0)
                {
                    RMARequestItemListResponse response = new RMARequestItemListResponse { RMARequestItems = list.RMARequestItemList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
                else
                {
                    data = JsonConvert.SerializeObject(new RMARequestItemListResponse { RMARequestItems = new List<RMARequestItemModel>() });
                }
            }
            return data;
        }

        public virtual string GetRMARequestItemsForGiftCard(string routeUri, string routeTemplate, string orderLineItems)
        {
            string data = GetFromCache(routeUri);
            if (Equals(data, null))
            {
                RMARequestItemListModel list = _service.GetRMARequestItemsForGiftCard(orderLineItems);
                if (list.RMARequestItemList.Count > 0)
                {
                    RMARequestItemListResponse response = new RMARequestItemListResponse { RMARequestItems = list.RMARequestItemList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
                else
                {
                    data = JsonConvert.SerializeObject(new RMARequestItemListResponse { RMARequestItems = new List<RMARequestItemModel>() });
                }
            }
            return data;
        }
    }
}