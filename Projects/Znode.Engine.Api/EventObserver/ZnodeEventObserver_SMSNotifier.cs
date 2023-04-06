using System.Collections.Generic;
using System.Linq;

using Znode.Engine.Api.Models;
using Znode.Engine.SMS;
using Znode.Libraries.ECommerce.Utilities;

using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Api
{
    public partial class ZnodeEventObserver
    {
        private void OnNewUserCreateFromAdmin(UserModel userModel)
        {
            if (userModel?.PortalId.GetValueOrDefault() > 0)
            {
                PortalModel portalModel = userService.GetPortalDomain(userModel.PortalId.GetValueOrDefault());

                Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
                macroKeyValues.Add(ZnodeConstant.FirstName, userModel.FirstName);
                macroKeyValues.Add("#webstoreloginurl#", portalModel?.DomainUrl);

                SendSMS(userModel.PortalId.GetValueOrDefault(), ZnodeConstant.NewCustomerAccountFromAdmin, userModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
            }
        }
        private void OnAccountVerificationSuccessful(UserModel userModel)
        {
            if (userModel?.UserId > 0)
            {

                UserModel userDetails = userService.GetUserById(userModel.UserId, null);
                Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
                macroKeyValues.Add(ZnodeConstant.FirstName, userModel.FirstName);
                if (HelperUtility.IsNull(userModel.PortalId))
                    userModel.PortalId = userDetails.PortalId;
                SendSMS(userModel.PortalId.GetValueOrDefault(), ZnodeConstant.AccountVerificationSuccessful, userModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
            }
        }
        private void OnCustomerAccountActivation(UserModel userModel)
        {
            if (userModel?.UserId > 0)
            {

                UserModel userDetails = userService.GetUserById(userModel.UserId, null);
                Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
                macroKeyValues.Add(ZnodeConstant.FirstName, userModel.FirstName);
                if (HelperUtility.IsNull(userModel.PortalId))
                    userModel.PortalId = userDetails.PortalId;
                SendSMS(userModel.PortalId.GetValueOrDefault(), ZnodeConstant.CustomerAccountActivation, userModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
            }
        }
        private void OnNewUserCreateFromWebstore(UserModel userModel)
        {
            if (userModel?.PortalId.GetValueOrDefault() > 0)
            {

                PortalModel portalModel = userService.GetPortalDomain(userModel.PortalId.GetValueOrDefault());
                
                Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
                macroKeyValues.Add(ZnodeConstant.FirstName, userModel.FirstName);
                macroKeyValues.Add("#webstoreloginurl#", portalModel?.DomainUrl);

                SendSMS(userModel.PortalId.GetValueOrDefault(), ZnodeConstant.NewCustomerAccountFromAdmin, userModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
            }
        }

        private static void SendSMS(int PortalId, string smsTemplateName, string phoneNumber, Dictionary<string, string> macroKeyValues, bool smsOptIn)
        {
            ZnodeSMSContext znodeSmsContext = new ZnodeSMSContext();
            znodeSmsContext.ReceiverPhoneNumber = phoneNumber;
            znodeSmsContext.SmsTemplateName = smsTemplateName;
            znodeSmsContext.PortalId = PortalId;
            znodeSmsContext.MacroValues = macroKeyValues;
            znodeSmsContext.SMSOptIn = smsOptIn;
            IZnodeSMSManager znodeSmsManager = GetService<IZnodeSMSManager>(new ZnodeNamedParameter("ZnodeSMSContext", znodeSmsContext));
            znodeSmsManager.SendSMS(znodeSmsContext);
        }

        private void OnRegistrationAttemptUsingExistingUsername(UserModel userModel)
        {
            Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
            macroKeyValues.Add(ZnodeConstant.FirstName, userModel.FirstName);

            SendSMS(userModel.PortalId.GetValueOrDefault(), ZnodeConstant.RegistrationAttemptUsingExistingUsername, userModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
        }
        private void OnOrderPlaced(OrderModel orderModel)
        {
            string firstName = string.Empty;
            string phoneNumber = string.Empty;

            if (orderModel?.UserId > 0)
            {
                UserModel userModel = userService.GetUserById(orderModel.UserId, null);
                firstName = HelperUtility.IsNotNull(userModel.FirstName) ? userModel.FirstName : userModel.UserName;
                phoneNumber = HelperUtility.IsNotNull(orderModel.PhoneNumber) ? orderModel.PhoneNumber : userModel.PhoneNumber;
                Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
                macroKeyValues.Add(ZnodeConstant.FirstName, firstName);
                macroKeyValues.Add("#OrderId#", orderModel.OrderNumber);

                SendSMS(orderModel.PortalId, ZnodeConstant.OrderPlacedNotification, phoneNumber, macroKeyValues, userModel.SMSOptIn);

            }
        }
        private void OnShippingReceipt(OrderModel orderModel)
        {
            string itemName = string.Empty;
            string firstName = HelperUtility.IsNotNull(orderModel.FirstName) ? orderModel.FirstName : orderModel.UserName;
            string productName = orderModel.ShoppingCartModel?.ShoppingCartItems?.FirstOrDefault()?.ProductName;
            int itemCount = orderModel.ShoppingCartModel.ShoppingCartItems.Count - 1;
            UserModel userModel = userService.GetUserById(orderModel.UserId, null);
            if (HelperUtility.IsNotNull(productName))
                itemName = itemCount > 0 ? productName + " and " + itemCount + " others." : productName;

            Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
            macroKeyValues.Add("#BillingFirstName#", firstName);
            macroKeyValues.Add("#ShippingName#", orderModel.ShippingTypeName);
            macroKeyValues.Add("#Name#", itemName);

            SendSMS(orderModel.PortalId, ZnodeConstant.ShippingReceipt, orderModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
        }
        private void OnResetPassword(UserModel userModel)
        {
            if (userModel.UserId == 0)
                userModel = userService.GetUserByUsername(userModel?.UserName, userModel.PortalId.GetValueOrDefault());
            string firstName = HelperUtility.IsNotNull(userModel.FirstName) ? userModel.FirstName : userModel.UserName;
            Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
            macroKeyValues.Add(ZnodeConstant.FirstName, firstName);
            SendSMS(userModel.PortalId.GetValueOrDefault(), ZnodeConstant.ResetPasswordLink, userModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
        }
        private void OnAccountVerificationRequestInProgress(UserModel userModel)
        {
            Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
            macroKeyValues.Add(ZnodeConstant.FirstName, userModel.FirstName);
            SendSMS(userModel.PortalId.GetValueOrDefault(), ZnodeConstant.AccountVerificationRequestInProgress, userModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
        }
        private void OnBillingAccountNumberAdded(UserModel userModel)
        {
            if (userModel?.UserId > 0)
            {

                userModel = userService.GetUserById(userModel.UserId, null);
                string firstName = HelperUtility.IsNotNull(userModel.FirstName) ? userModel.FirstName : userModel.UserName;
                Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
                macroKeyValues.Add(ZnodeConstant.FirstName, firstName);
                SendSMS(userModel.PortalId.GetValueOrDefault(), ZnodeConstant.BillingAccountNumberAdded, userModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
            }
        }
        private void OnConvertCustomerToAdministrator(UserModel userModel)
        {
            if (userModel?.UserId > 0)
            {

                userModel = userService.GetUserById(userModel.UserId, null);
                string firstName = HelperUtility.IsNotNull(userModel.FirstName) ? userModel.FirstName : userModel.UserName;
                Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
                macroKeyValues.Add(ZnodeConstant.FirstName, firstName);
                SendSMS(userModel.PortalId.GetValueOrDefault(), ZnodeConstant.ConvertCustomerToAdministrator, userModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
            }
        }
        private void OnPendingOrderApproved(AccountQuoteModel accountQuote)
        {
            if (accountQuote?.UserId > 0)
            {

                UserModel userModel = userService.GetUserById(accountQuote.UserId, null);
                string firstName = HelperUtility.IsNotNull(userModel.FirstName) ? userModel.FirstName : userModel.UserName;
                Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
                macroKeyValues.Add(ZnodeConstant.FirstName, firstName);
                SendSMS(accountQuote.PortalId, ZnodeConstant.PendingOrderApproved, userModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
            }
        }
        private void OnPendingOrderRejected(AccountQuoteModel accountQuote)
        {
            if (accountQuote?.UserId > 0)
            {

                UserModel userModel = userService.GetUserById(accountQuote.UserId, null);
                string firstName = HelperUtility.IsNotNull(userModel.FirstName) ? userModel.FirstName : userModel.UserName;
                Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
                macroKeyValues.Add(ZnodeConstant.FirstName, firstName);
                SendSMS(accountQuote.PortalId, ZnodeConstant.PendingOrderRejected, userModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
            }
        }
        private void OnPendingOrderStatusNotification(ShoppingCartModel shoppingCart)
        {
            UserModel userModel = userService.GetUserById(shoppingCart.UserId.GetValueOrDefault(), null);
            Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
            macroKeyValues.Add(ZnodeConstant.FirstName, shoppingCart.UserDetails.FirstName);
            SendSMS(shoppingCart.PortalId, ZnodeConstant.PendingOrderStatusNotification, shoppingCart.UserDetails.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
        }
        private void OnPendingOrderStatusNotification(AccountQuoteModel accountModel)
        {
            UserModel userModel = userService.GetUserById(accountModel.UserId, null);
            Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
            macroKeyValues.Add(ZnodeConstant.FirstName, accountModel.FirstName);
            SendSMS(accountModel.PortalId, ZnodeConstant.PendingOrderApproved, accountModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
        }
        private void OnCancelledOrderReceipt(OrderModel orderModel)
        {
            if (orderModel?.UserId > 0)
            {
                UserModel userModel = userService.GetUserById(orderModel.UserId, null);
                string firstName = HelperUtility.IsNotNull(orderModel.FirstName) ? orderModel.FirstName : orderModel.UserName;
                Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
                macroKeyValues.Add(ZnodeConstant.FirstName, firstName);
                SendSMS(orderModel.PortalId, ZnodeConstant.ResetPassword, orderModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
            }
        }
        private void OnResendOrderReceipt(OrderModel orderModel)
        {
            if (orderModel?.UserId > 0)
            {
                UserModel userModel = userService.GetUserById(orderModel.UserId, null);
                string firstName = HelperUtility.IsNotNull(orderModel.FirstName) ? orderModel.FirstName : orderModel.UserName;
                Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
                macroKeyValues.Add(ZnodeConstant.FirstName, firstName);
                SendSMS(orderModel.PortalId, ZnodeConstant.ResendOrderReceipt, orderModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
            }
        }
        private void OnNotificationForLowInventory(OrderModel orderModel)
        {
            if (orderModel?.UserId > 0)
            {
                UserModel userModel = userService.GetUserById(orderModel.UserId, null);
                Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
                macroKeyValues.Add(ZnodeConstant.FirstName, orderModel.FirstName);
                SendSMS(orderModel.PortalId, ZnodeConstant.LowInventoryOrderNotification, orderModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
            }
        }
        private void OnReturnStatusNotificationForCustomer(RMAReturnModel returnModel)
        {
            bool isSMSOptIn = userService.IsSMSOptIn(returnModel.UserId);
            Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
            macroKeyValues.Add(ZnodeConstant.FirstName, returnModel.FirstName);
            SendSMS(returnModel.PortalId, ZnodeConstant.EmailTemplateReturnStatusNotificationForCustomer, returnModel.PhoneNumber, macroKeyValues, isSMSOptIn);
        }
        private void OnRefundProcessedNotificationForCustomer(RMAReturnModel returnModel)
        {
            bool isSMSOptIn = userService.IsSMSOptIn(returnModel.UserId);
            Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
            macroKeyValues.Add(ZnodeConstant.FirstName, returnModel.FirstName);
            SendSMS(returnModel.PortalId, ZnodeConstant.EmailTemplateRefundProcessedNotificationForCustomer, returnModel.PhoneNumber, macroKeyValues, isSMSOptIn);
        }
        private void OnReturnRequestNotificationForCustomer(RMAReturnModel returnModel)
        {
            bool isSMSOptIn = userService.IsSMSOptIn(returnModel.UserId);
            Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
            macroKeyValues.Add(ZnodeConstant.FirstName, returnModel.FirstName);
            SendSMS(returnModel.PortalId, ZnodeConstant.EmailTemplateReturnRequestNotificationForCustomer, returnModel.PhoneNumber, macroKeyValues, isSMSOptIn);
        }
        private void OnCustomerFeedbackNotification(WebStoreCaseRequestModel caseRequestModel)
        {
            if (caseRequestModel?.UserId.GetValueOrDefault() > 0)
            {
                UserModel userModel = userService.GetUserById(caseRequestModel.UserId.GetValueOrDefault(), null);
                Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
                macroKeyValues.Add(ZnodeConstant.FirstName, caseRequestModel.FirstName);
                SendSMS(caseRequestModel.PortalId, ZnodeConstant.CustomerFeedbackNotification, userModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);

            }
        }
        private void OnServiceRequestMessage(WebStoreCaseRequestModel caseRequestModel)
        {
            if (caseRequestModel?.UserId.GetValueOrDefault() > 0)
            {

                UserModel userModel = userService.GetUserById(caseRequestModel.UserId.GetValueOrDefault(), null);
                Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
                macroKeyValues.Add(ZnodeConstant.FirstName, caseRequestModel.FirstName);
                SendSMS(caseRequestModel.PortalId, ZnodeConstant.ServiceRequestMessage, userModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
            }
        }
        private void OnContactUs(WebStoreCaseRequestModel caseRequestModel)
        {
            if (caseRequestModel?.UserId.GetValueOrDefault() > 0)
            {

                UserModel userModel = userService.GetUserById(caseRequestModel.UserId.GetValueOrDefault(), null);
                Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
                macroKeyValues.Add(ZnodeConstant.FirstName, caseRequestModel.FirstName);
                SendSMS(caseRequestModel.PortalId, ZnodeConstant.ContactUs, userModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
            }
        }
        private void OnTrackingLinks(ReferralCommissionModel referralCommissionModel)
        {
            if (referralCommissionModel?.UserId > 0)
            {

                UserModel userModel = userService.GetUserById(referralCommissionModel.UserId, null);
                Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
                macroKeyValues.Add(ZnodeConstant.FirstName, userModel.FirstName);
                SendSMS(referralCommissionModel.PortalId, ZnodeConstant.TrackingLinks, userModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
            }
        }
        private void OnQuoteRequestAcknowledgementToUser(QuoteResponseModel quoteResponseModel)
        {
            UserModel userModel = userService.GetUserById(quoteResponseModel.UserId, null);
            Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
            macroKeyValues.Add(ZnodeConstant.FirstName, quoteResponseModel.FirstName);
            SendSMS(quoteResponseModel.PortalId, ZnodeConstant.QuoteRequestAcknowledgementToUser, quoteResponseModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
        }
        private void OnProductKeyOrderReceipt(OrderModel orderModel)
        {
            if (orderModel?.UserId > 0)
            {

                UserModel userModel = userService.GetUserById(orderModel.UserId, null);
                Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
                macroKeyValues.Add(ZnodeConstant.FirstName, userModel.FirstName);
                SendSMS(orderModel.PortalId, ZnodeConstant.ServiceRequestMessage, userModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
            }
        }
        private void OnRemainingVoucherBalance(OrderModel orderModel)
        {
            if (orderModel?.UserId > 0)
            {
                UserModel userModel = userService.GetUserById(orderModel.UserId, null);
                Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
                macroKeyValues.Add(ZnodeConstant.FirstName, userModel.FirstName);
                SendSMS(orderModel.PortalId, ZnodeConstant.RemainingVoucherBalance, userModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
            }
        }
        private void OnVoucherExpirationReminder(GiftCardModel giftCardModel)
        {
            if (giftCardModel?.UserId.GetValueOrDefault() > 0)
            {

                UserModel userModel = userService.GetUserById(giftCardModel.UserId.GetValueOrDefault(), null);
                Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
                macroKeyValues.Add(ZnodeConstant.FirstName, userModel.FirstName);
                SendSMS(giftCardModel.PortalId, ZnodeConstant.VoucherExpirationReminder, userModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
            }
        }
        private void OnIssueVoucher(GiftCardModel giftCardModel)
        {
            if (giftCardModel?.UserId.GetValueOrDefault() > 0)
            {

                UserModel userModel = userService.GetUserById(giftCardModel.UserId.GetValueOrDefault(), null);
                Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
                macroKeyValues.Add(ZnodeConstant.FirstName, userModel.FirstName);
                SendSMS(giftCardModel.PortalId, ZnodeConstant.IssueVoucher, userModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
            }
        }
        private void OnQuoteConvertedToOrder(OrderModel orderModel)
        {
            if (orderModel?.UserId > 0)
            {

                UserModel userModel = userService.GetUserById(orderModel.UserId, null);
                Dictionary<string, string> macroKeyValues = new Dictionary<string, string>();
                macroKeyValues.Add(ZnodeConstant.FirstName, userModel.FirstName);
                SendSMS(orderModel.PortalId, ZnodeConstant.QuoteConvertedToOrder, userModel.PhoneNumber, macroKeyValues, userModel.SMSOptIn);
            }
        }

    }

}