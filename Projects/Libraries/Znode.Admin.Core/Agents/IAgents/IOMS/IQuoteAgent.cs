using System;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Admin.Agents
{
    public interface IQuoteAgent
    {
        /// <summary>
        /// Get Quote List.
        /// </summary>
        /// <param name="FilterCollectionDataModel">Filter Collection model</param>
        /// <returns>Return Quote List in QuoteListViewModel model.</returns>
        QuoteListViewModel GetQuoteList(FilterCollectionDataModel model);

        /// <summary>
        /// Get Quote Details.
        /// </summary>
        /// <param name="omsQuoteId">Quote Id</param>
        /// <returns>QuoteResponseViewModel</returns>
        QuoteResponseViewModel GetQuoteDetails(int omsQuoteId);

        /// <summary>
        /// Get Quote Status List
        /// </summary>
        /// <param name="quoteStatus">respective quoteStatus</param>
        /// param name="omsQuoteId">omsQuoteId</param>
        /// <returns>OrderStatusList</returns>
        OrderStatusList BindManageQuoteStatus( string quoteStatus, int omsQuoteId);

        /// <summary>
        /// Update Quote Status in session.
        /// </summary>
        /// <param name="OrderStatusList">depending on selection selectedItemId</param>
        /// <returns>OrderStatusList </returns>
        OrderStatusList UpdateQuoteStatus(OrderStatusList quoteStatus);

        /// <summary>
        /// Get Quote state value by id
        /// </summary>
        /// <param name="omsQuoteStateId">omsQuoteStateId</param>
        /// <returns>isEdit</returns>
        bool GetQuoteStateValueById(int omsQuoteStateId);

        /// <summary>
        /// Update quote Shipping Account Number.
        /// </summary>
        /// <param name="ShippingAccountNumber">Shipping Account Number</param>
        /// <returns>boolean Update Status</returns>
        bool UpdateShippingAccountNumber(int omsQuoteId, string shippingAccountNumber);

        /// <summary>
        /// Update quote Shipping Method.
        /// </summary>
        /// <param name="ShippingMethod">ShippingMethod</param>
        /// <returns>boolean Update Status</returns>
        bool UpdateShippingMethod(int omsQuoteId, string shippingMethod);

        /// <summary>
        /// Update In hand date.
        /// </summary>
        /// <param name="omsQuoteId"></param>
        /// <param name="InHandDate"></param>
        /// <returns>boolean Update Status</returns>
        bool UpdateInHandDate(int omsQuoteId, DateTime InHandDate);

        /// <summary>
        /// Update Quote Expiration date.
        /// </summary>
        /// <param name="omsQuoteId"></param>
        /// <param name="QuoteExpirationDate"></param>
        /// <returns>boolean Update Status</returns>
        bool UpdateQuoteExpirationDate(int omsQuoteId, DateTime QuoteExpirationDate);


        /// <summary>
        /// Get Shipping options for manage Quote
        /// </summary>
        /// <returns>ShippingListViewModel</returns>
        ShippingListViewModel GetShippingListForManage(int omsQuoteId);

        /// <summary>
        /// Get calculated cart on shipping change.
        /// </summary>
        /// <param name="ShippingId">ShippingId</param>
        /// <param name="omsQuoteId">omsQuoteId</param>
        /// <returns>QuoteCartViewModel</returns>
        QuoteCartViewModel GetShippingChargesForManage(int omsQuoteId, int shippingId);

        /// <summary>
        /// Update shipping cost
        /// </summary>
        /// <param name="shippingCost">shippingCost to update</param>
        /// <returns>QuoteCartViewModel</returns>
        QuoteCartViewModel UpdateShippingHandling(int omsQuoteId, string shippingCost);

        /// <summary>
        /// Get Cart method get the existing shopping cart.
        /// </summary>
        /// <param name="omsQuoteId">omsQuoteId</param>
        /// <returns>QuoteCartViewModel</returns>
        QuoteCartViewModel GetQuoteShoppingCart(int omsQuoteId);

        /// <summary>
        /// Get User Address.
        /// </summary>
        /// <param name="ManageAddressModel">ManageAddressModel</param>
        /// <returns>AddressViewModel</returns>
        AddressViewModel GetUserAddressForManageQuote(ManageAddressModel addressDetail);

        /// <summary>
        /// Get Address Default Data.
        /// </summary>
        /// <param name="accountId">account id</param>
        /// <param name="ManageAddressModel">ManageAddressModel</param>
        /// <returns>AddressViewModel</returns>
        AddressViewModel GetAddressDefaultData(ManageAddressModel addressDetail, int accountId);

        /// <summary>
        /// Update Customer Address And Calculate.
        /// </summary>
        /// <param name="addressViewModel">AddressViewModel</param>
        /// <returns>QuoteSessionModel</returns>
        QuoteSessionModel UpdateCustomerAddressAndCalculate(AddressViewModel addressViewModel);

        /// <summary>
        /// Map Updated Customer Address.
        /// </summary>
        /// <param name="omsQuoteId">omsQuoteId</param>
        /// <returns>CustomerInfoViewModel</returns>
        CustomerInfoViewModel MapUpdatedCustomerAddress(int omsQuoteId);

        /// <summary>
        /// Remove shopping cart item
        /// </summary>
        /// <param name="omsQuoteId"></param>
        /// <param name="guid">guid</param>
        /// <returns>true/false</returns>
        bool RemoveQuoteCartItem(int omsQuoteId, string guid);

        /// <summary>
        /// Calculate Quote shopping cart
        /// </summary>
        /// <param name="omsQuoteId">omsQuoteId</param>
        /// <returns>QuoteCartViewModel</returns>
        QuoteCartViewModel CalculateShoppingCart(int omsQuoteId);

        /// <summary>
        /// Update the Quantity and Unit Price in the shopping cart item.
        /// </summary>
        /// <param name="quoteDataModel">data to be update.</param>
        /// <returns>QuoteCartViewModel</returns>
        QuoteCartViewModel UpdateQuoteLineItemDetails(ManageOrderDataModel quoteDataModel);

        /// <summary>
        /// Update Tax Exempt
        /// </summary>
        /// <param name="omsQuoteId">omsQuoteId</param>
        /// <param name="isTaxExempt">bool</param>
        /// <returns>QuoteCartViewModel</returns>
        QuoteCartViewModel UpdateTaxExemptForManage(int omsQuoteId, bool isTaxExempt);

        /// <summary>
        /// Get Quote Details for Print.
        /// </summary>
        /// <param name="omsQuoteId">Quote Id</param>
        /// <returns>QuoteResponseViewModel</returns>
        QuoteResponseViewModel PrintManageQuote(int omsQuoteId);

        /// <summary>
        /// Update Job Name
        /// </summary>
        /// <param name="omsQuoteId">int</param>
        /// <param name="jobName">string</param>
        /// <returns>bool</returns>
        bool UpdateJobName(int omsQuoteId, string jobName);

        /// <summary>
        /// Update Shipping Constraint Code
        /// </summary>
        /// <param name="omsQuoteId">int</param>
        /// <param name="shippingConstraintCode">string</param>
        /// <returns>bool</returns>
        bool UpdateShippingConstraintCode(int omsQuoteId, string shippingConstraintCode);

        /// <summary>
        /// Update existing Quote.
        /// </summary>
        /// <param name="omsQuoteId">omsQuoteId</param>
        /// <param name="additionalNote">additionalNote</param>
        /// <returns>QuoteResponseViewModel</returns>
        BooleanModel UpdateQuote(int omsQuoteId, string additionalNote);

        /// <summary>
        /// Get create quote details
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns></returns>
        QuoteCreateViewModel GetCreateQuoteDetails(int portalId = 0);

        /// <summary>
        /// Submit Quote
        /// </summary>
        /// <param name="quoteCreateViewModel"></param>
        /// <returns></returns>
        QuoteCreateViewModel SubmitQuote(QuoteCreateViewModel quoteCreateViewModel);

        /// <summary>
        /// Convert quote to order.
        /// </summary>
        /// <param name="convertToOrderModel"></param>
        /// <returns>OrdersViewModel</returns>
        OrdersViewModel SaveAndConvertQuoteToOrder(ConvertQuoteToOrderViewModel convertToOrderModel);
    }
}
