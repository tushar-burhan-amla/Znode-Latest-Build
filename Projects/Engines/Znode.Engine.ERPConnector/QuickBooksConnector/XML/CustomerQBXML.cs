using System;
using Znode.Engine.Api.Models;

namespace Znode.Engine.ERPConnector
{
    public class CustomerQBXML : IDisposable
    {
        #region Private Variables

        private bool isDisposed;

        #endregion Private Variables

        #region Constructor

        ~CustomerQBXML()
        {
            if (!isDisposed)
                Dispose();
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// From CustomerAddRq model passed to it, It'll generate the QB XML string required for adding new customer in QB
        /// </summary>
        /// <param name="customerAddRq">CustomerAddRq type model</param>
        /// <returns>XML string required for adding new customer in QB</returns>
        public string QBCustomerRqXML(CustomerAddRq customerAddRq)
        => HelperUtility.GetQuickBooksXML<CustomerAddRq>
          (customerAddRq, QuickBooksConstants.CustomerAddRqXMLAttribute);

        /// <summary>
        /// From token number and OrderModel passed to it, It'll generate and give the XML string required for adding new customer.
        /// This Add customer QB XML can then be used as reference while adding sales order.
        /// </summary>
        /// <param name="orderModel">Znode api model used for order</param>
        /// <param name="tokenNo">Ticket/Token passed to web service</param>
        /// <returns>XML string for adding new customer in QB</returns>
        public string QBCustomerAddXMLFromOrder(OrderModel orderModel, string tokenNo)
        => QBCustomerRqXML(
           new CustomerAddRq()
           {
               requestID = tokenNo,
               CustomerAdd = GetCustomerAddByOrder(orderModel)
           });

        /// <summary>
        /// Implementation of IDisposable interface
        /// </summary>
        public void Dispose() => isDisposed = true;

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Mapping of customer model used in QuickBooks response XML string generation from znode order model for creating new customer if it's not available in QuickBooks
        /// </summary>
        /// <param name="orderModel">order model available in Znode</param>
        /// <returns>customer model used in QuickBooks response XML string</returns>
        private CustomerAdd GetCustomerAddByOrder(OrderModel orderModel)
        => new CustomerAdd()
        {
            Contact = orderModel?.PhoneNumber,
            Email = orderModel?.Email,
            FirstName = orderModel?.FirstName,
            LastName = orderModel?.LastName,
            MiddleName = string.Empty,
            Name = HelperUtility.GetCustomerNameFromOrder(orderModel),
            Phone = orderModel?.PhoneNumber,
            BillAddress = GetBillAddressFromOrder(orderModel)
        };

        /// <summary>
        /// Mapping of address model used in QuickBooks response XML string generation from znode order model for creating new billing address if it's not available in QuickBooks
        /// </summary>
        /// <param name="orderModel">order model available in Znode</param>
        /// <returns>Address model used in QuickBooks response XML string</returns>
        private Address GetBillAddressFromOrder(OrderModel orderModel)
        => new Address()
        {
            Addr1 = orderModel?.BillingAddress?.Address1,
            Addr2 = orderModel?.BillingAddress?.Address2,
            Addr3 = orderModel?.BillingAddress?.Address3,
            City = orderModel?.BillingAddress?.CityName,
            Country = orderModel?.BillingAddress?.CountryName,
            PostalCode = orderModel?.BillingAddress?.PostalCode,
            State = orderModel?.BillingAddress?.StateCode,
        };

        #endregion Private Methods
    }
}