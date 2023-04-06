using System;
using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.V2;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services.Maps.V2
{
    public static class ShoppingCartMapV2
    {
        public static void CreateOrderToShoppingCartModel(CreateOrderModelV2 model, ShoppingCartModel shoppingCart)
        {
            shoppingCart.ShippingId = model.ShippingOptionId;
            shoppingCart.ShippingAddressId = model.ShippingAddressId;
            shoppingCart.BillingAddressId = model.BillingAddressId;
            shoppingCart.Shipping.ShippingId = model.ShippingOptionId;
            shoppingCart.OrderDate = model.OrderDate;
            shoppingCart.GiftCardNumber = model.GiftCard;
            model?.PromotionCouponCode?.ForEach(x => { shoppingCart?.Coupons?.Add(new CouponModel { Code = x }); });
            shoppingCart.OrderLevelShipping = shoppingCart.Shipping.ShippingHandlingCharge;
            if (HelperUtility.IsNotNull(shoppingCart.BillingAddress))
                shoppingCart.BillingAddress.IsGuest = model.IsGuest;

            if (HelperUtility.IsNotNull(shoppingCart.ShippingAddress))
                shoppingCart.ShippingAddress.IsGuest = model.IsGuest;
            if (HelperUtility.IsNotNull(shoppingCart.UserDetails))
                shoppingCart.UserDetails.IsGuestUser = model.IsGuest;
            shoppingCart.ProfileId = model.ProfileId;
            shoppingCart.CreditCardNumber = model.CreditCardNumber;
            shoppingCart.CardType = model.CreditCardType;
        }

        public static void CreateOrderToShoppingCartModel(ShoppingCartCalculateRequestModelV2 model, ShoppingCartModel shoppingCart)
        {
            shoppingCart.ShippingId = model.ShippingOptionId;
            if (HelperUtility.IsNotNull(shoppingCart.Shipping))
                shoppingCart.Shipping.ShippingId = model.ShippingOptionId;

            shoppingCart.GiftCardNumber = model.GiftCard;
            model?.PromotionCouponCode?.ForEach(x => { shoppingCart?.Coupons?.Add(new CouponModel { Code = x }); });
            shoppingCart.OrderLevelShipping = shoppingCart.Shipping?.ShippingHandlingCharge == 0 ? 0 : Convert.ToDecimal(shoppingCart.Shipping?.ShippingHandlingCharge);

            if (HelperUtility.IsNotNull(shoppingCart.BillingAddress))
                shoppingCart.BillingAddress.IsGuest = model.IsGuest;

            if (HelperUtility.IsNotNull(shoppingCart.ShippingAddress))
                shoppingCart.ShippingAddress.IsGuest = model.IsGuest;

            shoppingCart.UserDetails.IsGuestUser = model.IsGuest;
            shoppingCart.ProfileId = model.ProfileId;
        }

        public static void MapPayment(CreateOrderModelV2 model, ShoppingCartModel shoppingCart)
        {
            shoppingCart.Payment = new PaymentModel();
            if (HelperUtility.IsNotNull(shoppingCart.ShippingAddress))
            {
                shoppingCart.Payment.ShippingAddress = shoppingCart.ShippingAddress;
                shoppingCart.Payment.ShippingAddress.IsGuest = model.IsGuest;
            }

            if (HelperUtility.IsNotNull(shoppingCart.BillingAddress))
            {
                shoppingCart.Payment.BillingAddress = shoppingCart.BillingAddress;
                shoppingCart.Payment.BillingAddress.IsGuest = model.IsGuest;
            }
            IPaymentSettingService _paymentService = GetService<IPaymentSettingService>();

            NameValueCollection _expand = new NameValueCollection
            {
                { ZnodePaymentSettingEnum.ZnodePaymentType.ToString().ToLower(), ZnodePaymentSettingEnum.ZnodePaymentType.ToString() }
            };
            shoppingCart.Payment.PaymentSetting = _paymentService.GetPaymentSetting(model.PaymentOptionId, _expand);

            if (HelperUtility.IsNotNull(shoppingCart.Payment.PaymentSetting))
            {
                shoppingCart.Payment.PaymentSetting.ProfileId = shoppingCart.ProfileId;
                shoppingCart.Payment.PaymentDisplayName = shoppingCart.Payment.PaymentSetting.PaymentDisplayName;
                shoppingCart.Payment.PaymentName = shoppingCart.Payment.PaymentSetting.PaymentName;
            }

            shoppingCart.Payment.TransactionId = model.PaymentTransactionId;
            shoppingCart.Token = model.PaymentTransactionId;
            shoppingCart.Shipping.ShippingCountryCode = shoppingCart.Payment?.ShippingAddress?.CountryName;
        }

        public static void MapPayment(ShoppingCartCalculateRequestModelV2 model, ShoppingCartModel shoppingCart)
        {
            shoppingCart.Payment = new PaymentModel();
            shoppingCart.Payment.BillingAddress = shoppingCart.BillingAddress;

            if (HelperUtility.IsNotNull(shoppingCart.ShippingAddress))
            {
                shoppingCart.Payment.ShippingAddress = shoppingCart.ShippingAddress;
                shoppingCart.Payment.ShippingAddress.IsGuest = model.IsGuest;
            }
            shoppingCart.Payment.PaymentSetting = new PaymentSettingModel();
            shoppingCart.Payment.PaymentSetting.ProfileId = shoppingCart.ProfileId;

            if (HelperUtility.IsNull(shoppingCart.Shipping))
                shoppingCart.Shipping = new OrderShippingModel();
            shoppingCart.Shipping.ShippingCountryCode = shoppingCart.Payment?.ShippingAddress?.CountryName;
        }

        public static CartParameterModel ToCartParameterModel(CreateOrderModelV2 model)
        {
            return new CartParameterModel
            {
                LocaleId = model.LocaleId,
                CookieMappingId = model.CookieMappingId,
                UserId = model.UserId,
                PortalId = model.PortalId,
                PublishedCatalogId = model.PublishedCatalogId,
                Custom1 = model.Custom1,
                Custom2 = model.Custom2,
                Custom3 = model.Custom3,
                Custom4 = model.Custom4,
                Custom5 = model.Custom5
            };
        }

        public static CartParameterModel ToCartParameterModel(ShoppingCartCalculateRequestModelV2 model)
        {
            return new CartParameterModel
            {
                LocaleId = model.LocaleId,
                CookieMappingId = model.CookieMappingId,
                UserId = model.UserId,
                PortalId = model.PortalId,
                PublishedCatalogId = model.PublishedCatalogId
            };
        }
    }
}
