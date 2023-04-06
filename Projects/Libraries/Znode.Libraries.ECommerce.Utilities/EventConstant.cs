using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Libraries.ECommerce.Utilities
{
    public struct EventConstant
    {
        public const string OnAccountVerificationSuccessful = "OnAccountVerificationSuccessful";
        public const string OnNewCustomerAccountFromAdmin = "OnNewCustomerAccountFromAdmin";
        public const string OnNewCustomerAccountFromWebstore = "OnNewCustomerAccountFromWebstore";
        public const string OnRegistrationAttemptUsingExistingUsername = "OnRegistrationAttemptUsingExistingUsername";
        public const string OnResetPassword = "OnResetPassword";
        public const string OnOrderCancelled = "OnOrderCancelled";
        public const string OnOrderShipped = "OnShippingReceipt";
        public const string OnOrderPlaced = "OnOrderPlaced";
        public const string OnNotificationForLowInventory = "OnNotificationForLowInventory";
        public const string OnAccountVerificationRequestInProgress = "OnAccountVerificationRequestInProgress";
        public const string OnReturnStatusNotificationForCustomer = "OnReturnStatusNotificationForCustomer";
        public const string OnRefundProcessedNotificationForCustomer = "OnRefundProcessedNotificationForCustomer";
        public const string OnReturnRequestNotificationForCustomer = "OnReturnRequestNotificationForCustomer";
        public const string OnCustomerAccountActivation = "OnCustomerAccountActivation";
        public const string OnCustomerFeedbackNotification = "OnCustomerFeedbackNotification";
        public const string OnServiceRequestMessage = "OnServiceRequestMessage";
        public const string OnTrackingLinks = "OnTrackingLinks";
        public const string OnProductKeyOrderReceipt = "OnProductKeyOrderReceipt";
        public const string OnQuoteConvertedToOrder = "OnQuoteConvertedToOrder";
        public const string OnQuoteRequestAcknowledgementToUser = "OnQuoteRequestAcknowledgementToUser";
        public const string OnBillingAccountNumberAdded = "OnBillingAccountNumberAdded";
        public const string OnConvertCustomerToAdministrator = "OnConvertCustomerToAdministrator";
        public const string OnContactUs = "OnContactUs";
        public const string OnRemainingVoucherBalance = "OnRemainingVoucherBalance";
        public const string OnVoucherExpirationReminder = "OnVoucherExpirationReminder";
        public const string OnIssueVoucher = "OnIssueVoucher";
        public const string OnPendingOrderApproved = "OnPendingOrderApproved";
        public const string OnPendingOrderRejected = "OnPendingOrderRejected";
        public const string OnPendingOrderStatusNotification = "OnPendingOrderStatusNotification";
        public const string OnResendOrderReceipt = "OnResendOrderReceipt";
    }
}
