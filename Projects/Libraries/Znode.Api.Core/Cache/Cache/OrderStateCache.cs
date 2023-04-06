using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class OrderStateCache : BaseCache, IOrderStateCache
    {
        #region Private Variables
        private readonly IOrderStateService _service;
        #endregion

        #region Constructor
        public OrderStateCache(IOrderStateService service)
        {
            _service = service;
        }
        #endregion

        #region Public Method
        public virtual string GetOrderStates(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                OrderStateListModel list = _service.GetOrderStates(Expands, Filters, Sorts, Page);
                if (list?.OrderStates?.Count > 0)
                {
                    OrderStateListResponse response = new OrderStateListResponse { OrderStates = list.OrderStates };
                    response.MapPagingDataFromModel(list);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}