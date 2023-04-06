using System;
using Znode.Engine.Api.Models;

namespace Znode.Engine.ERPConnector
{
    public class CustomerMapper : IDisposable
    {
        #region Private Variables

        private bool isDisposed;

        #endregion Private Variables

        #region Constructor

        ~CustomerMapper()
        {
            if (!isDisposed)
                Dispose();
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Implementation of IDisposable interface
        /// </summary>
        public void Dispose() => isDisposed = true;

        /// <summary>
        /// Generate and give the XML string required for adding new customer from token number and OrderModel passed to it.
        /// </summary>
        /// <param name="orderModel">Znode api model used for order</param>
        /// <param name="token">Ticket/Token passed to web service</param>
        /// <returns>XML string for adding new customer in QB</returns>
        public string SalesOrderCustomerQBXML(OrderModel orderModel, string token)
        => (new CustomerQBXML()).
            QBCustomerAddXMLFromOrder(orderModel, token);

        #endregion Public Methods
    }
}