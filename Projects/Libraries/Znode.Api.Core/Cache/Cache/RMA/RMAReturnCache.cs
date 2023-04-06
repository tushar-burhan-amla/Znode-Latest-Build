using Newtonsoft.Json;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
namespace Znode.Engine.Api.Cache
{
    public class RMAReturnCache : BaseCache, IRMAReturnCache
    {
        #region Private Variable
        private readonly IRMAReturnService _service;
        #endregion

        #region Constructor
        public RMAReturnCache(IRMAReturnService rmaReturnService)
        {
            _service = rmaReturnService;
        }
        #endregion

        #region Public Methods
        //Get order details by order number for return.
        public virtual string GetOrderDetailsForReturn(int userId, string orderNumber, bool isFromAdmin = false)
        {
            string data = string.Empty;
            if (userId > 0 && !string.IsNullOrEmpty(orderNumber))
            {
                //Get data from service
                OrderModel order = _service.GetOrderDetailsForReturn(userId, orderNumber, isFromAdmin);
                if (IsNotNull(order))
                {
                    OrderResponse response = new OrderResponse { Order = order };
                    data = JsonConvert.SerializeObject(response);
                }
            }
            return data;
        }

        //Get Return List.
        public virtual string GetReturnList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                RMAReturnListModel returnList = _service.GetReturnList(Expands, Filters, Sorts, Page);
                if (returnList?.Returns?.Count > 0)
                {
                    RMAReturnListResponse response = new RMAReturnListResponse { ReturnList = returnList };

                    response.MapPagingDataFromModel(returnList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get order return details by return number
        public virtual string GetReturnDetails(string rmaReturnNumber)
        {
            string data = string.Empty;
            if (!string.IsNullOrEmpty(rmaReturnNumber))
            {
                //Get data from service
                RMAReturnModel returnDetails = _service.GetReturnDetails(rmaReturnNumber, Expands);
                if (IsNotNull(returnDetails))
                {
                    RMAReturnResponse response = new RMAReturnResponse { Return = returnDetails };
                    data = JsonConvert.SerializeObject(response);
                }
            }
            return data;
        }

        //Get List of Return States
        public virtual string GetReturnStatusList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                RMAReturnStateListModel list = _service.GetReturnStatusList(Expands, Filters, Sorts, Page);
                if (list?.ReturnStates?.Count > 0)
                {
                    RMAReturnStateListResponse response = new RMAReturnStateListResponse { ReturnStates = list.ReturnStates };
                    response.MapPagingDataFromModel(list);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get order return details for admin by return number
        public virtual string GetReturnDetailsForAdmin(string rmaReturnNumber)
        {
            string data = string.Empty;
            if (!string.IsNullOrEmpty(rmaReturnNumber))
            {
                //Get data from service
                RMAReturnModel returnDetails = _service.GetReturnDetailsForAdmin(rmaReturnNumber);
                if (IsNotNull(returnDetails))
                {
                    RMAReturnResponse response = new RMAReturnResponse { Return = returnDetails };
                    data = JsonConvert.SerializeObject(response);
                }
            }
            return data;
        }
        #endregion
    }
}