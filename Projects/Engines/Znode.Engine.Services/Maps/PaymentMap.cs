using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Services.Maps
{
    public static class PaymentMap
    {
        public static ZnodePayment ToZnodePayment(PaymentModel model, AddressModel shippingAddress = null, AddressModel billingAddress = null)
        {
            ZnodePayment znodePayment = new ZnodePayment();
            if (IsNotNull(model))
            {
                znodePayment.AuthCode = model.AuthorizationCode;
                znodePayment.IsRecurringBillingExists = model.RecurringBillingExists;
                znodePayment.PaymentName = model.PaymentName;
                znodePayment.PaymentDisplayName = model.PaymentDisplayName;
                znodePayment.PaymentTypeId = model?.PaymentSetting?.PaymentTypeId == 0 ? null : model?.PaymentSetting?.PaymentTypeId;
                znodePayment.PaymentTypeName = string.IsNullOrEmpty(model?.PaymentSetting?.PaymentTypeName) ? null : model?.PaymentSetting?.PaymentTypeName;
                znodePayment.PaymentSettingId = model?.PaymentSetting?.PaymentSettingId == 0 ? null : model?.PaymentSetting?.PaymentSettingId;
                znodePayment.SaveCardData = model.SaveCardData;
                znodePayment.SubscriptionID = model.SubscriptionId;
                znodePayment.TokenId = model.TokenId;
                znodePayment.TransactionID = model.TransactionId;
                znodePayment.UseToken = model.UseToken;
                znodePayment.IsPreAuthorize = model.IsPreAuthorize;
                znodePayment.PaymentExternalId = model.PaymentExternalId;
                znodePayment.BillingAddress = model.BillingAddress;
            }

            if (IsNotNull(shippingAddress))
                znodePayment.ShippingAddress = shippingAddress;
            else
                znodePayment.ShippingAddress = model?.ShippingAddress;

            if (IsNotNull(billingAddress))
                znodePayment.BillingAddress = billingAddress;
            else
                znodePayment.BillingAddress = model?.BillingAddress;

            return znodePayment;
        }

    }
}
