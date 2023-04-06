using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Models;
using Znode.Engine.Services;
using ZNode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.QuickBook
{
    public class SalesOrderMappings : IDisposable
    {
        #region Private Variables

        private bool isDisposed;
        private readonly IOrderService _orderService;

        #endregion Private Variables

        #region Constructor

        public SalesOrderMappings()
        {
            _orderService = new OrderService();
        }

        ~SalesOrderMappings()
        {
            if (!isDisposed)
                Dispose();
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Generate list of QB XML string that is to be imported in QB
        /// This list will be having above things :
        ///     1. Add customer QB xml string which is to be used in adding new sales order
        ///     2. Add inventory QB xml string that is add item QB XML which is to be used as order line item while adding new sales order
        ///     3. Add sales order QB xml string
        /// </summary>
        /// <param name="token">Ticket/Token passed to web service</param>
        /// <returns>list of QB XML string</returns>
        public List<string> GetSalesOrderAddRqXML(string token)
        {
            List<string> orderAddRqXML = new List<string>();
            CustomerQBXML customerQBXML = new CustomerQBXML();
            InventoryQBXML inventoryQBXML = new InventoryQBXML();
            SalesOrderQBXML salesOrderQBXML = new SalesOrderQBXML();

            (GetQuickBookObject())?.Orders?.OrderBy(o => o.OmsOrderId).ToList().ForEach(
                AddOrderDataToQBXMLStringList(token,
                orderAddRqXML,
                customerQBXML,
                inventoryQBXML,
                salesOrderQBXML));

            return orderAddRqXML;
        }

        /// <summary>
        /// Implementation of IDisposibale interface
        /// </summary>
        public void Dispose() => isDisposed = true;

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Inserts customer, inventory item and sales order related data into the string list used for holding QB XML string request queue
        /// </summary>
        /// <param name="token"></param>
        /// <param name="orderAddRqXML">List of add order requesting XML string Queue</param>
        /// <param name="customerQBXML">Instance of an object for XML mapping regarding customer</param>
        /// <param name="inventoryQBXML">Instance of an object for XML mapping regarding item inventory</param>
        /// <param name="salesOrderQBXML">Instance of an object for XML mapping regarding sales order</param>
        /// <returns>Action type for OrderModel</returns>
        private Action<OrderModel> AddOrderDataToQBXMLStringList(string token, List<string> orderAddRqXML, CustomerQBXML customerQBXML, InventoryQBXML inventoryQBXML, SalesOrderQBXML salesOrderQBXML)
        {
            return order =>
            {
                //Insert Customer
                orderAddRqXML.Add(customerQBXML.QBCustomerAddXMLFromOrder(order, token));

                OrderModel orderDetails = GetOrderDetails(order);

                //Insert Inventory
                orderAddRqXML.AddRange(inventoryQBXML.QBInventoryItemsAddXMLFromOrder(orderDetails, token));

                //Insert Sales Order
                orderAddRqXML.Add(salesOrderQBXML.QBSalesOrderAddXMLFromOrder(orderDetails, token));
            };
        }

        /// <summary>
        /// Gives data to be imported from ZNode to QuickBook. It return list of orders in ZNode
        /// </summary>
        /// <returns>Decorated instance for list of orders in ZNode</returns>
        private OrdersListModel GetQuickBookObject()
        {
            string lastRefNumber = (new TextFileUtility()).GetLastRefNumberFromTextFile();
            if (!string.IsNullOrEmpty(lastRefNumber))
            {
                FilterCollection filters = GetDefaultFilteration(lastRefNumber);

                NameValueCollection sorts = GetDefaultSorts();

                NameValueCollection page = GetDefaultPagination();

                return _orderService.GetOrderList(null, filters, sorts, page);
            }
            else
                return _orderService.GetOrderList(null, null, null, null);
        }

        /// <summary>
        /// Default sorting data required for requesting order list from ZNode
        /// </summary>
        /// <returns>Name value collection instance with sorting information</returns>
        private static NameValueCollection GetDefaultSorts()
        {
            NameValueCollection sorts = new NameValueCollection();
            sorts.Add(ZnodeOmsOrderEnum.OmsOrderId.ToString(), QuickBookConstants.AscSortOrder);
            return sorts;
        }

        /// <summary>
        /// Default filtration required for requesting order list from ZNode
        /// </summary>
        /// <param name="lastRefNumber">last reference number inserted in QuickBook</param>
        /// <returns>Instance of filter collection with all desired data</returns>
        private FilterCollection GetDefaultFilteration(string lastRefNumber)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(ZnodeOmsOrderEnum.OmsOrderId.ToString(), FilterOperators.GreaterThan, lastRefNumber);
            return filters;
        }

        /// <summary>
        /// Default pagination data required for requesting order list from ZNode
        /// </summary>
        /// <returns>Name value collection instance with pagination information</returns>
        private NameValueCollection GetDefaultPagination()
        {
            NameValueCollection page = new NameValueCollection();
            page.Add(Services.Constants.PageKeys.Index.ToString(), QuickBookConstants.DefaultPaginationStartPage);
            page.Add(Services.Constants.PageKeys.Size.ToString(), QuickBookConstants.DefaultPaginationPageSize);
            return page;
        }

        /// <summary>
        /// Request order details for given order to ZNode. This is needed for requesting OrderLineItems for respective orders.
        /// </summary>
        /// <param name="order">Instance of order model required for requesting it's respective order details along with it's order lin items</param>
        /// <returns>Instance of an order model filled with data associated with order line items</returns>
        private OrderModel GetOrderDetails(OrderModel order)
        {
            //Expands for cascading Order Line Items
            NameValueCollection expand = new NameValueCollection();
            expand.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsOrderLineItems.ToString().ToLower(), ExpandKeys.OrderLineItems);

            OrderModel orderDetails = _orderService.GetOrderById(order.OmsOrderId, null, expand);
            orderDetails.UserName = order.UserName;//There is error in GetOrderById, After this call username does not staying equal

            return orderDetails;
        }

        #endregion Private Methods
    }
}