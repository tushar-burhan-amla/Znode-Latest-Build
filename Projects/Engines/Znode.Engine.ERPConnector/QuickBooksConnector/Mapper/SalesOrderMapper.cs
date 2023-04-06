using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.ERPConnector
{
    public class SalesOrderMapper : BaseERP
    {
        #region Private Variables

        private bool isDisposed;
        private readonly IOrderClient _orderClient;
        #endregion Private Variables

        #region Constructor

        public SalesOrderMapper()
        {
            _orderClient = GetClient<OrderClient>();
        }

        ~SalesOrderMapper()
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

            (GetQuickBooksObject())?.Orders?.OrderBy(o => o.OmsOrderId).ToList().ForEach(
                AddOrderDataToQBXMLStringList(token,
                orderAddRqXML,
                customerQBXML,
                inventoryQBXML,
                salesOrderQBXML));

            return orderAddRqXML;
        }

        /// <summary>
        /// Implementation of IDisposable interface
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
        /// Gives data to be imported from Znode to QuickBooks. It return list of orders in Znode
        /// </summary>
        /// <returns>Decorated instance for list of orders in Znode</returns>
        private OrdersListModel GetQuickBooksObject()
        {
            string lastRefNumber = (new TextFileUtility()).GetLastRefNumberFromTextFile();
            if (!string.IsNullOrEmpty(lastRefNumber))
            {
                FilterCollection filters = GetDefaultFiltration(lastRefNumber);

                SortCollection sorts = GetDefaultSorts();
                return _orderClient.GetOrderList(null, filters, sorts, QuickBooksConstants.DefaultPaginationStartPage, QuickBooksConstants.DefaultPaginationPageSize);
            }
            else
                return _orderClient.GetOrderList(null, null, null, null, null);
        }

        /// <summary>
        /// Set display order sort if not present
        /// </summary>
        /// <param name="sorts">sorts</param>
        private static SortCollection GetDefaultSorts()
        {
            SortCollection sorts = new SortCollection();
            sorts.Add(ZnodeOmsOrderEnum.OmsOrderId.ToString(), QuickBooksConstants.AscSortOrder);
            return sorts;
        }
        /// <summary>
        /// Default filtration required for requesting order list from Znode
        /// </summary>
        /// <param name="lastRefNumber">last reference number inserted in QuickBooks</param>
        /// <returns>Instance of filter collection with all desired data</returns>
        private FilterCollection GetDefaultFiltration(string lastRefNumber)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(ZnodeOmsOrderEnum.OmsOrderId.ToString(), FilterOperators.GreaterThan, lastRefNumber);
            return filters;
        }

        /// <summary>
        /// Request order details for given order to Znode. This is needed for requesting OrderLineItems for respective orders.
        /// </summary>
        /// <param name="order">Instance of order model required for requesting it's respective order details along with it's order lin items</param>
        /// <returns>Instance of an order model filled with data associated with order line items</returns>
        private OrderModel GetOrderDetails(OrderModel order)
        {
            //Expands for cascading Order Line Items
            ExpandCollection expand = new ExpandCollection();
            expand.Add(ZnodeOmsOrderDetailEnum.ZnodeOmsOrderLineItems.ToString().ToLower());
            expand.Add(ExpandKeys.OrderLineItems);

            OrderModel orderDetails = _orderClient.GetOrderById(order.OmsOrderId, expand);
            orderDetails.UserName = order.UserName;//There is error in GetOrderById, After this call username does not staying equal

            return orderDetails;
        }

        #endregion Private Methods
    }
}