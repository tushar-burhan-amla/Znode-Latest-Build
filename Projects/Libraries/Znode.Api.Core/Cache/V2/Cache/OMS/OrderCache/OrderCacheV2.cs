using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Api.Models.Responses.V2;
using Znode.Engine.Api.Models.V2;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class OrderCacheV2 : OrderCache, IOrderCacheV2
    {
        #region Private Variables
        private readonly IOrderServiceV2 _orderService;
        #endregion

        #region Constructor
        public OrderCacheV2(IOrderServiceV2 orderService) : base(orderService)
        {
            _orderService = orderService;
        }
        #endregion

        public virtual string GetOrderListV2(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                OrdersListModelV2 orderList = _orderService.GetOrderListV2(Expands, Filters, Sorts, Page);
                if (orderList?.Orders?.Count > 0)
                {
                    OrderListResponseV2 response = new OrderListResponseV2 { OrderList = orderList };

                    response.MapPagingDataFromModel(orderList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetOrderByOrderNumber(string orderNumber, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                OrderModel order = _orderService.GetOrderByOrderNumber(orderNumber, Filters, Expands);
                if (HelperUtility.IsNotNull(order))
                {
                    OrderResponse response = new OrderResponse { Order = order };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
    }
}
