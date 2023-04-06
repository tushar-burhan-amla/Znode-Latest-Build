using Avalara.AvaTax.RestClient;
using System;
using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Entities;

namespace Znode.Engine.Taxes
{
    public interface IAvataxHelper
    {
        /// <summary>
        /// Get avatax settings based on portal Id.
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <returns>Returns populated AvataxSettings model</returns>
        AvataxSettings GetAvataxSetting(int portalId);

        /// <summary>
        /// To get the order number based on order Id.
        /// </summary>
        /// <param name="omsOrderId">Order Id</param>
        /// <returns>Returns the data for order number.</returns>
        string GetOrderNumber(int omsOrderId);

        /// <summary>
        /// To set avatax settings based on portal Id.
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <returns>Returns populated AvataxSettings model.</returns>
        AvataxSettings SetAvataxSettings(int portalId = 0);

        /// <summary>
        /// To get the order state based on order Id.
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <param name="orderNumber">Order number</param>
        /// <param name="createdDate">Order created date</param>
        /// <returns>Order state</returns>
        string GetOrderState(int orderId, out string orderNumber, out DateTime createdDate);

        /// <summary>
        /// To check whether tax is exempt.
        /// </summary>
        /// <param name="shoppingCart">ZnodeShoppingCart</param>
        /// <param name="avalaraTaxRequest">CreateTransactionModel</param>
        /// <returns>Returns the tax exempt status.</returns>
        bool IsTaxExempt(ZnodeShoppingCart shoppingCart, CreateTransactionModel avalaraTaxRequest);

        /// <summary>
        /// To get the currency code by portal Id.
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <returns>Returns the currency code</returns>
        string GetCurrencyCode(int portalId);

        /// <summary>
        /// To get the value of IsCalculateTaxAfterDiscount flag.
        /// </summary>
        /// <returns>Returns the value of IsCalculateTaxAfterDiscount flag.</returns>
        bool IsCalculateTaxAfterDiscount();

        /// <summary>
        /// Get the portal entity via portal Id.
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <returns>Returns the data for ZnodePortal.</returns>
        ZnodePortal GetPortalEntity(int currentPortalId);

        /// <summary>
        /// To get the order number and date based on order Id.
        /// </summary>
        /// <param name="omsOrderId">Order Id</param>
        /// <param name="orderDate">Order created date</param>
        /// <returns>Order number</returns>
        string GetOrderNumber(int omsOrderId, out DateTime orderDate);

        /// <summary>
        /// To get the line item tax override amount for returns.
        /// </summary>
        /// <param name="shoppingCart">ZnodeShoppingCart</param>
        /// <param name="returnItemList">List of ReturnOrderLineItemModel</param>
        /// <returns>List of TaxAmountOverrideModel</returns>
        List<TaxAmountOverrideModel> GetLineItemTaxOverrideAmount(ZnodeShoppingCart shoppingCart, List<ReturnOrderLineItemModel> returnItemList);

        /// <summary>
        /// To get the line item tax override amount for order cancellation.
        /// </summary>
        /// <param name="shoppingCart">ZnodeShoppingCart</param>
        /// <returns>List of TaxAmountOverrideModel</returns>
        List<TaxAmountOverrideModel> GetLineItemTaxOverrideAmount(ZnodeShoppingCart shoppingCart);

        /// <summary>
        /// Checking value store in databse table against the omsOrderId
        /// </summary>
        /// <param name="omsOrderId">Order id.</param>
        /// <returns>Value of isSellerImporterOfDuty.</returns>
        bool? IsSellerImporterOfDuty(int omsOrderId);

        /// <summary>
        /// If isSellerImporterOfRecord is store null in table then update its value. 
        /// </summary>
        /// <param name="omsOrderId">Order id.</param>
        /// <param name="isSellerImporterOfRecord">Value of isSellerImporterOfRecord.</param>
        void UpdateIsSellerImporterOfDuty(int omsOrderId, bool isSellerImporterOfRecord);

    }
}
