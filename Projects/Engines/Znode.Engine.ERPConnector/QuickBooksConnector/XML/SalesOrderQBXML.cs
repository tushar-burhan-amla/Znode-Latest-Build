using System;
using Znode.Engine.Api.Models;

namespace Znode.Engine.ERPConnector
{
    public class SalesOrderQBXML : IDisposable
    {
        #region Private Methods

        private bool isDisposed;

        #endregion Private Methods

        #region Constructor

        ~SalesOrderQBXML()
        {
            if (!isDisposed)
                Dispose();
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Generate and gets the QB XML string required for adding new sales order item in QB from SalesOrderAddRq model passed to it
        /// </summary>
        /// <param name="salesOrderAddRq">SalesOrderAddRq type model</param>
        /// <returns>QB XML string required for adding new sales order item in QB</returns>
        public string QBSalesOrderAddRqXML(SalesOrderAddRq salesOrderAddRq)
        => HelperUtility.GetQuickBooksXML<SalesOrderAddRq>
        (salesOrderAddRq, QuickBooksConstants.SalesOrderAddRqXMLAttribute);

        /// <summary>
        /// Generate and gets the QB XML string required for adding new sales order in QB from OrderModel model and token passed to it
        /// </summary>
        /// <param name="orderModel">Znode OrderModel type used while ordering.</param>
        /// <param name="ticket">Ticket/Token passed to web service</param>
        /// <returns>QB XML string required for adding new sales order in QB</returns>
        public string QBSalesOrderAddXMLFromOrder(OrderModel orderModel, string ticket)
        {
            SalesOrderAddRq salesOrderAddRq = GetSalesOrderAddRq(orderModel, ticket);
            return QBSalesOrderAddRqXML(salesOrderAddRq);
        }

        /// <summary>
        /// Implementation of IDispossable interface
        /// </summary>
        public void Dispose() => isDisposed = true;

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Maps object from znode order model to QuickBooks SalesOrderAdd Request model
        /// </summary>
        /// <param name="orderModel">znode order model from which order mapping needs to be done</param>
        /// <param name="ticket">token passed in header of web request</param>
        /// <returns>Mapped QuickBooks SalesOrderAdd Request model</returns>
        private SalesOrderAddRq GetSalesOrderAddRq(OrderModel orderModel, string ticket)
        => new SalesOrderAddRq()
        {
            requestID = ticket,
            SalesOrderAdd = GetSalesOrderAddByOrder(orderModel)
        };

        /// <summary>
        /// Maps object from znode order model to QuickBooks SalesOrderAdd model
        /// </summary>
        /// <param name="orderModel">znode order model from which order mapping needs to be done</param>
        /// <returns>Mapped QuickBooks SalesOrderAdd Request model</returns>
        private SalesOrderAdd GetSalesOrderAddByOrder(OrderModel orderModel)
        => new SalesOrderAdd()
        {
            //Old Ex: FI-170620-104423-420
            RefNumber = GetOrderRefNumber(orderModel),
            BillAddress = GetBillAddressByOrder(orderModel),
            CustomerRef = GetCustomerRefByOrder(orderModel),
            SalesOrderLineAdd = ((new InventoryQBXML()).QBLineItemsAddXMLFromOrder(orderModel))?.ToArray(),
            ShipAddress = GetShippingAddressByOrder(orderModel),
            TxnDate = orderModel.OrderDate.ToString(QuickBooksConstants.DateFormat),
            Memo = QuickBooksConstants.DefaultNAText
        };

        /// <summary>
        /// Maps object from znode order model to QuickBooks shipping address model
        /// </summary>
        /// <param name="orderModel">znode order model from which order mapping needs to be done</param>
        /// <returns>Mapped QuickBooks shipping address model</returns>
        private Address GetShippingAddressByOrder(OrderModel orderModel)
        => new Address()
        {
            Addr1 = orderModel?.ShippingAddress?.Address1,
            Addr2 = orderModel?.ShippingAddress?.Address2,
            Addr3 = orderModel?.ShippingAddress?.Address3,
            City = orderModel?.ShippingAddress?.CityName,
            Country = orderModel?.ShippingAddress?.CountryName,
            PostalCode = orderModel?.ShippingAddress?.PostalCode,
            State = orderModel?.ShippingAddress?.StateCode
        };

        /// <summary>
        /// Gives reference type for mapping customer with sales order within QuickBooks from Znode Order model
        /// </summary>
        /// <param name="orderModel">znode order model from which order mapping needs to be done</param>
        /// <returns>Mapped customer reference type</returns>
        private Ref GetCustomerRefByOrder(OrderModel orderModel)
        => new Ref()
        {
            FullName = HelperUtility.GetCustomerNameFromOrder(orderModel),
        };

        /// <summary>
        /// Maps object from znode order model to QuickBooks billing address model
        /// </summary>
        /// <param name="orderModel">znode order model from which order mapping needs to be done</param>
        /// <returns>Mapped QuickBooks billing address model</returns>
        private Address GetBillAddressByOrder(OrderModel orderModel)
        => new Address()
        {
            Addr1 = orderModel?.BillingAddress?.Address1,
            Addr2 = orderModel?.BillingAddress?.Address2,
            Addr3 = orderModel?.BillingAddress?.Address3,
            City = orderModel?.BillingAddress?.CityName,
            Country = orderModel?.BillingAddress?.CountryName,
            PostalCode = orderModel?.BillingAddress?.PostalCode,
            State = orderModel?.BillingAddress?.StateCode
        };

        /// <summary>
        /// Gets reference number to be set in QuickBooks from Znode order model
        /// </summary>
        /// <param name="orderModel">znode order model from which order mapping needs to be done</param>
        /// <returns>Reference number to be set in QuickBooks</returns>
        private string GetOrderRefNumber(OrderModel orderModel)
        => orderModel.OmsOrderId.ToString();

        #endregion Private Methods
    }
}