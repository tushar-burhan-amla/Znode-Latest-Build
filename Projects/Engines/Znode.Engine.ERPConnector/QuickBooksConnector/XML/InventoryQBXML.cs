using System;
using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Engine.ERPConnector
{
    public class InventoryQBXML : IDisposable
    {
        #region Private Methods

        private bool isDisposed;

        #endregion Private Methods

        #region Constructor

        ~InventoryQBXML()
        {
            if (!isDisposed)
                Dispose();
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Generate and gets the QB XML string required for adding new inventory item in QB from ItemInventoryAddRq model passed to it
        /// </summary>
        /// <param name="itemInventoryAddRq">ItemInventoryAddRq type model</param>
        /// <returns>XML string required for adding new inventory item in QB</returns>
        public string QBItemInventoryRqXML(ItemInventoryAddRq itemInventoryAddRq)
        => HelperUtility.GetQuickBooksXML<ItemInventoryAddRq>
        (itemInventoryAddRq, QuickBooksConstants.ItemInventoryAddRqXMLAttribute);

        /// <summary>
        /// Generate and gets list of the QB XML string required for adding new inventory line item associated with particular order model in QB from OrderModel model and token passed to it
        /// </summary>
        /// <param name="orderModel">Znode OrderModel type used while ordering.</param>
        /// <param name="token">Ticket/Token passed to web service</param>
        /// <returns>list of the XML string required for adding new inventory line item associated with particular order model in QB</returns>
        public List<string> QBInventoryItemsAddXMLFromOrder(OrderModel orderModel, string token)
        {
            List<string> _salesOrderLineItemsXML = new List<string>();
            if (orderModel?.OrderLineItems != null)
            {
                orderModel?.OrderLineItems?.ForEach(AddOrderLineItemToQBXMLStringList(token, _salesOrderLineItemsXML));
            }
            return _salesOrderLineItemsXML;
        }

        /// <summary>
        /// Generate and get list of the SalesOrderLineAdd type object associated with particular order model passed to it
        /// </summary>
        /// <param name="orderModel">Znode OrderModel type used while ordering.</param>
        /// <returns>List of the SalesOrderLineAdd type object</returns>
        public List<SalesOrderLineAdd> QBLineItemsAddXMLFromOrder(OrderModel orderModel)
        {
            List<SalesOrderLineAdd> _salesOrderLineAdd = new List<SalesOrderLineAdd>();

            orderModel?.OrderLineItems?.ForEach(AddOrderLineItemToQBLineItemList(_salesOrderLineAdd));
            return _salesOrderLineAdd;
        }

        /// <summary>
        /// Implementation of IDisposable interface
        /// </summary>
        public void Dispose() => isDisposed = true;

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Adds the Znode order line item to quick book order line item list
        /// </summary>
        /// <param name="salesOrderLineAddList">List in which sales order line item need to be added</param>
        /// <returns>Action type delegation for znode order line item model</returns>
        private Action<OrderLineItemModel> AddOrderLineItemToQBLineItemList(List<SalesOrderLineAdd> salesOrderLineAddList)
        {
            return orderLineItemModel =>
            {
                SalesOrderLineAdd salesOrderLineAdd = GetSalesOrderLineAdd(orderLineItemModel);
                salesOrderLineAddList.Add(salesOrderLineAdd);
            };
        }

        /// <summary>
        /// Gives list of xml string with new inventory items available for given order line item model in it.
        /// </summary>
        /// <param name="token">Token received for web request header</param>
        /// <param name="salesOrderLineItemsXMLList">list of xml string with new inventory items available for given order line item model in it</param>
        /// <returns>Action type delegation for znode order line item model</returns>
        private Action<OrderLineItemModel> AddOrderLineItemToQBXMLStringList(string token, List<string> salesOrderLineItemsXMLList)
        {
            return orderLineItemModel =>
            {
                ItemInventoryAddRq itemInventoryAddRq = GetItemInventoryAddReqByOrderLineItem(token, orderLineItemModel);
                salesOrderLineItemsXMLList.Add(QBItemInventoryRqXML(itemInventoryAddRq));
            };
        }

        /// <summary>
        /// Mapping of sales order line item used in QuickBooks response XML string generation from znode order line item model for creating new order if it's not available in QuickBooks
        /// </summary>
        /// <param name="orderLineItem">znode order line item model</param>
        /// <returns>Sales order line item used in QuickBooks response XML string</returns>
        private SalesOrderLineAdd GetSalesOrderLineAdd(OrderLineItemModel orderLineItem)
        => new SalesOrderLineAdd()
        {
            Amount = (orderLineItem?.Price * orderLineItem?.Quantity)?.ToString("F"),
            Desc = QuickBooksConstants.DefaultNAText,
            ItemRef = GetItemRef(orderLineItem),
            Quantity = orderLineItem?.Quantity.ToString("F")
        };

        /// <summary>
        /// Mapping of Znode OrderLineItemModel with ItemInventoryAdd request
        /// </summary>
        /// <param name="token">Token received from header of web request</param>
        /// <param name="orderLineItemModel">Instance of Znode OrderLineItemModel to be mapped</param>
        /// <returns>Instance of mapped QuickBooks ItemInventoryAdd Request object</returns>
        private ItemInventoryAddRq GetItemInventoryAddReqByOrderLineItem(string token, OrderLineItemModel orderLineItemModel)
        => new ItemInventoryAddRq()
        {
            requestID = token,
            ItemInventoryAdd = new ItemInventoryAdd()
            {
                AssetAccountRef = GetDefaultAssetAccountRef(),
                COGSAccountRef = GetDefaultCOGSAccountRef(),
                IncomeAccountRef = GetDefaultIncomeAccountRef(),
                Name = orderLineItemModel.Sku,
                SalesDesc = QuickBooksConstants.DefaultNAText,
                SalesPrice = orderLineItemModel.Price.ToString("F")
            }
        };

        /// <summary>
        /// Creates instance for mapping QuickBooks item reference for order line item with item inventory.
        /// </summary>
        /// <param name="orderLineItemModel">Znode order line item model from which reference mapping needs to be done</param>
        /// <returns>Mapping for QuickBooks item reference for order line item with item inventory.</returns>
        private Ref GetItemRef(OrderLineItemModel orderLineItemModel)
        => new Ref()
        {
            FullName = orderLineItemModel.Sku
        };

        /// <summary>
        /// Gives reference model for default asset account type which is already available in QuickBooks.
        /// </summary>
        /// <returns>Reference model for default asset account type which is already available in QuickBooks.</returns>
        private Ref GetDefaultAssetAccountRef()
        => new Ref()
        {
            FullName = QuickBooksConstants.AssetAccountType
        };

        /// <summary>
        /// Gives reference model for default income account type which is already available in QuickBooks.
        /// </summary>
        /// <returns>Reference model for default income account type which is already available in QuickBooks.</returns>
        private Ref GetDefaultIncomeAccountRef()
        => new Ref()
        {
            FullName = QuickBooksConstants.IncomeAccountType
        };

        /// <summary>
        /// Gives reference model for default COGS account type which is already available in QuickBooks.
        /// </summary>
        /// <returns>Reference model for default asset account type which is already available in QuickBooks.</returns>
        private Ref GetDefaultCOGSAccountRef()
        => new Ref()
        {
            FullName = QuickBooksConstants.COGSAccountType
        };

        #endregion Private Methods
    }
}