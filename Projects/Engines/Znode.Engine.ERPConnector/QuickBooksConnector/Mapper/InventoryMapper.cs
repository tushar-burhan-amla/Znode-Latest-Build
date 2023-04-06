using System;
using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Engine.ERPConnector
{
    public class InventoryMapper : IDisposable
    {
        #region Private Variables

        private bool isDisposed;

        #endregion Private Variables

        #region Constructor

        ~InventoryMapper()
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
        /// Generate and gets list of the QB XML string required for adding new inventory line item associated with particular order model in QB from OrderModel model and token passed to it.
        /// </summary>
        /// <param name="orderModel">Znode OrderModel type used while ordering.</param>
        /// <param name="token">Ticket/Token passed to web service</param>
        /// <returns>list of the XML string required for adding new inventory line item associated with particular order model in QB</returns>
        public List<string> SalesOrderInventoryItemsQBXML(OrderModel orderModel, string token)
        => (new InventoryQBXML()).
            QBInventoryItemsAddXMLFromOrder(orderModel, token);

        #endregion Public Methods
    }
}