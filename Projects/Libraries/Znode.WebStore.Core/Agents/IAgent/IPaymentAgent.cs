using System.Collections.Generic;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using System;

namespace Znode.Engine.WebStore.Agents
{
    public interface IPaymentAgent
    {

        /// <summary>
        /// Get PaymentGatway Name.
        /// </summary>
        /// <param name="paymentSettingId">Payment Setting Id/param>
        /// <param name="quoteNumber">Quote Number/param>
        /// <param name="skipCalculations">Recalculations will be skipped in case of pending order/param>
        /// <returns></returns>
        PaymentDetailsViewModel GetPaymentDetails(int paymentSettingId, string quoteNumber = "", bool skipCalculations = false, bool isUsedForOfflinePayment = false, decimal remainingOrderAmount = 0);

        /// <summary>
        /// Get Payment Setting.
        /// </summary>
        /// <param name="paymenSettingId">Id To get Payment Setting</param>
        /// <param name="portalId">Optional portal Id</param>
        /// <returns>payment Setting View Model</returns>
        PaymentSettingViewModel GetPaymentSetting(int paymentSettingId, int portalId = 0);

        /// <summary>
        /// This method will use to call the payment and process the order
        /// </summary>
        /// <param name="submitPaymentModel">Submit Payment Model</param>
        /// <returns>GatewayResponseModel</returns>
        GatewayResponseModel ProcessPayNow(SubmitPaymentModel submitPaymentModel);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string GetOrderTotal();

        /// <summary>
        /// This method will use to call the amazon payment checkout and process the order
        /// </summary>
        /// <param name="submitPaymentModel">Submit Payment Model</param>
        /// <returns>GatewayResponseModel</returns>
        GatewayResponseModel ProcessAmazonPay(SubmitPaymentModel submitPaymentModel);

        /// <summary>
        /// This method will use to call the Paypal Express Checkout payment and process the order
        /// </summary>
        /// <param name="submitPaymentModel">Submit Payment Model</param>
        /// <returns>GatewayResponseModel</returns>
        GatewayResponseModel ProcessPayPal(SubmitPaymentModel submitPaymentModel);

        /// <summary>
        /// Get saved credit card details by customers GUID
        /// </summary>        
        /// <param name="customersGUID">string customersGUID</param>
        /// <returns>PaymentMethodCCDetailsListModel</returns>
        PaymentMethodCCDetailsListModel GetPaymentCreditCardDetails(string customersGUID);

        /// <summary>
        /// Get saved credit card count by customers GUID  
        /// </summary>
        /// <param name="customersGUID">string customersGUID</param>
        /// <returns>Count of saved credit card.</returns>
        int GetSaveCreditCardCount(string customersGUID);

        /// <summary>
        /// Delete saved credit cards.
        /// </summary>
        /// <param name="paymentGUID">paymentGUID</param>
        /// <returns>true or false</returns>
        bool DeleteSavedCreditCardDetail(string paymentGUID);

        /// <summary>
        /// call PayPal payment finalize method in Payment Application
        /// </summary>
        /// <param name="submitPaymentModel"></param>
        /// <returns></returns>
        GatewayResponseModel FinalizePayPalProcess(SubmitPaymentModel submitPaymentModel);

        /// <summary>
        /// Get AmazonPay address details.
        /// </summary>
        /// <param name="model">SubmitPaymentModel</param>
        /// <returns>SubmitPaymentModel</returns>
        SubmitPaymentModel GetAmazonPayAddressDetails(SubmitPaymentModel model);

        /// <summary>
        /// Get the amount total in string format as per locale
        /// </summary>
        /// <param name="orderTotal">order total</param>
        /// <returns>Order total in string format</returns>
        string FormatOrderTotal(decimal? orderTotal);

        [Obsolete("This method is not in use now, as a new method has been introduced with the name GetPaymentSettingByUserDetailsFromCache to the payment settings")]
        List<PaymentSettingModel> GetPaymentSettingListFromCache(int portalId, int profileId, FilterCollection filters, SortCollection sort, bool isUsedForOfflinePayment = false);
        /// <summary>
        /// Get Payment Setting details from the cache. If data not present in the cache then from the database.
        /// </summary>
        /// <param name="paymentSettingId">This parameter contains the PaymentSettingId to fetch the data. </param>
        /// <param name="portalId">This parameter contains the portal Id values.</param>
        /// <param name="isPaymentApplication">This parameter contains the boolean value. 
        /// When this parameter is true then data fetch from the payment API otherwise data fetch from API.</param>
        /// <returns></returns>
        PaymentSettingModel GetPaymentSettingFromCache(int paymentSettingId, int portalId, bool isPaymentApplication = false);

        /// <summary>
        /// Get Payment Setting details by code from the cache. If data not present in the cache then from the database.
        /// </summary>
        /// <param name="paymentCode">This parameter contains payment code and used to fetch the data using that code.</param>
        /// <returns></returns>
        PaymentSettingModel GetPaymentSettingByPaymentCodeFromCache(string paymentCode);

        /// <summary>
        /// Get Payment AuthToken.
        /// </summary>
        /// <returns>AuthToken string</returns>
        string GetPaymentAuthToken(string userOrSessionId, bool fromAdminApp);

        /// <summary>
        /// Get Cart View Model.
        /// </summary>
        /// <returns>CartViewModel</returns>
        CartViewModel GetCartViewModel();

        /// <summary>
        /// This method will be used to get order total and recalculate if isOABRequired is true.
        /// <param name="isOABRequired">isOABRequired</param>
        /// <param name="isPendingOrderRequest">isPendingOrderRequest</param>
        /// <param name="remainingOrderAmount">remainingOrderAmount</param>
        /// </summary>
        /// <returns>total</returns>
        string GetRecalculatedOrderTotal(bool isOABRequired, bool isPendingOrderRequest, decimal remainingOrderAmount = 0);

        /// <summary>
        /// To get the payment settings by user details.
        /// </summary>
        /// <param name="userPaymentSettingModel">UserPaymentSettingModel</param>
        /// <param name="isUsedForOfflinePayment">isUsedForOfflinePayment</param>
        /// <returns>Payment setting list</returns>
        List<PaymentSettingModel> GetPaymentSettingByUserDetailsFromCache(UserPaymentSettingModel userPaymentSettingModel, bool isUsedForOfflinePayment = false, bool isQuotes = false);

        /// <summary>
        /// To get the populated UserPaymentSettingModel.
        /// </summary>
        /// <returns>UserPaymentSettingModel</returns>
        UserPaymentSettingModel GetUserPaymentSettingDetails();

        /// <summary>
        /// To get the Gateway Token.
        /// </summary>
        /// <returns>PaymentTokenViewModel</returns>
        PaymentTokenViewModel GetPaymentGatewayToken(PaymentTokenViewModel paymentTokenModel);
    }
}
