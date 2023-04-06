using System;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore.Maps
{
    public class PaymentViewModelMap
    {
        public static SubmitPaymentModel ToModel(ShoppingCartModel shoppingCart, SubmitOrderViewModel submitPaymentViewModel)
        {
            SubmitPaymentModel model = new SubmitPaymentModel();

            if (!Equals(shoppingCart, null) && !Equals(shoppingCart.BillingAddress, null))
            {
                model.BillingCity = shoppingCart.BillingAddress?.CityName;
                model.BillingFirstName = shoppingCart.BillingAddress?.FirstName;
                model.BillingLastName = shoppingCart.BillingAddress?.LastName;
                model.BillingCountryCode = shoppingCart.BillingAddress?.CountryName;
                model.BillingName = shoppingCart.BillingAddress?.DisplayName;
                model.BillingPhoneNumber = shoppingCart.BillingAddress?.PhoneNumber;
                model.BillingPostalCode = shoppingCart.BillingAddress?.PostalCode;
                model.BillingStateCode = !string.IsNullOrEmpty(shoppingCart?.BillingAddress?.StateCode) ? shoppingCart?.BillingAddress?.StateCode : shoppingCart?.BillingAddress?.StateName;
                model.BillingStreetAddress1 = shoppingCart?.BillingAddress?.Address1;
                model.BillingStreetAddress2 = shoppingCart?.BillingAddress?.Address2;
                model.BillingEmailId = string.IsNullOrEmpty(model.BillingEmailId) ? shoppingCart.UserDetails?.Email : model.BillingEmailId;
                model.CompanyName = shoppingCart?.BillingAddress?.CompanyName;
            }
            else if (!Equals(shoppingCart.ShippingAddress, null))
            {
                model.BillingCity = shoppingCart.ShippingAddress?.CityName;
                model.BillingFirstName = shoppingCart.ShippingAddress?.FirstName;
                model.BillingLastName = shoppingCart.ShippingAddress?.LastName;
                model.BillingCountryCode = shoppingCart.ShippingAddress?.CountryName;
                model.BillingName = shoppingCart.ShippingAddress?.DisplayName;
                model.BillingPhoneNumber = shoppingCart.ShippingAddress?.PhoneNumber;
                model.BillingPostalCode = shoppingCart.ShippingAddress?.PostalCode;
                model.BillingStateCode = !string.IsNullOrEmpty(shoppingCart?.BillingAddress?.StateCode) ? shoppingCart?.BillingAddress?.StateCode : shoppingCart?.ShippingAddress?.StateName;
                model.BillingStreetAddress1 = shoppingCart.ShippingAddress?.Address1;
                model.BillingStreetAddress2 = shoppingCart.ShippingAddress?.Address2;
                model.BillingEmailId = shoppingCart?.BillingAddress?.EmailAddress;
                model.CompanyName = shoppingCart?.BillingAddress?.CompanyName;
            }

            model.GatewayCurrencyCode = !string.IsNullOrEmpty(shoppingCart.CurrencyCode) ? shoppingCart.CurrencyCode : ZnodeConstant.UnitedStatesSuffix;
            model.OrderId = !string.IsNullOrEmpty(shoppingCart.OrderNumber) ? shoppingCart.OrderNumber : "1";
            model.CyberSourceToken = submitPaymentViewModel.CyberSourceToken;
            if (!Equals(shoppingCart.ShippingAddress, null))
            {
                model.ShippingCity = shoppingCart.ShippingAddress?.CityName;
                model.ShippingFirstName = shoppingCart.ShippingAddress?.FirstName;
                model.ShippingLastName = shoppingCart.ShippingAddress?.LastName;
                model.ShippingCountryCode = shoppingCart.ShippingAddress?.CountryName;
                model.ShippingPhoneNumber = shoppingCart.ShippingAddress?.PhoneNumber;
                model.ShippingPostalCode = shoppingCart.ShippingAddress?.PostalCode;
                model.ShippingStateCode = !string.IsNullOrEmpty(shoppingCart?.ShippingAddress?.StateCode) ? shoppingCart.ShippingAddress.StateCode : shoppingCart?.ShippingAddress?.StateName;
                model.ShippingStreetAddress1 = shoppingCart.ShippingAddress?.Address1;
                model.ShippingStreetAddress2 = shoppingCart.ShippingAddress?.Address2;
                model.CardType = submitPaymentViewModel.CardType;
            }

            model.PaymentApplicationSettingId = submitPaymentViewModel.PaymentApplicationSettingId;
            model.PaymentSettingId = submitPaymentViewModel.PaymentSettingId;
            model.PaymentCode = submitPaymentViewModel.PaymentCode;
            model.CustomerProfileId = submitPaymentViewModel.CustomerProfileId;
            model.CustomerPaymentProfileId = submitPaymentViewModel.CustomerPaymentId;
            model.PaymentToken = !string.IsNullOrEmpty(submitPaymentViewModel.PayPalToken) ? submitPaymentViewModel.PayPalToken : submitPaymentViewModel.PaymentToken;
            model.CustomerGUID = submitPaymentViewModel.CustomerGuid;
            model.CustomerShippingAddressId = submitPaymentViewModel.CustomerShippingAddressId;
            model.CardSecurityCode = submitPaymentViewModel.CardSecurityCode;
            model.IsACHPayment = submitPaymentViewModel.IsACHPayment;
            model.TransactionId = submitPaymentViewModel.TransactionId;
            model.UserId = submitPaymentViewModel.UserId;
            model.IsSaveCreditCard = submitPaymentViewModel.IsSaveCreditCard;
            model.CardNumber = submitPaymentViewModel.CreditCardNumber;
            model.SubTotal = GetLocaleWiseAmount(shoppingCart.SubTotal.GetValueOrDefault());
            model.Total = GetLocaleWiseAmount(shoppingCart.Total.GetValueOrDefault());
            model.TaxCost = GetLocaleWiseAmount(shoppingCart.TaxCost);
            model.GiftCardAmount = GetLocaleWiseAmount(shoppingCart.GiftCardAmount);
            model.ShippingCost = GetLocaleWiseAmount(shoppingCart.ShippingCost);
            model.Discount = GetLocaleWiseAmount(shoppingCart.Discount);
            model.ShippingHandlingCharges = GetLocaleWiseAmount(shoppingCart.ShippingHandlingCharges);
            model.ShippingDiscount = GetLocaleWiseAmount(shoppingCart.ShippingDiscount);
            model.CSRDiscountAmount = GetLocaleWiseAmount(shoppingCart.CSRDiscountAmount);
            model.CardHolderName = submitPaymentViewModel.CardHolderName;
            model.PaymentGUID = submitPaymentViewModel.PaymentGUID;
            model.GatewayCode = submitPaymentViewModel.GatewayCode;
            model.IsOrderFromAdmin = submitPaymentViewModel.IsOrderFromAdmin;
            model.PortalName = submitPaymentViewModel.PortalName;
            //Amazon pay properties.
            model.AmazonOrderReferenceId = submitPaymentViewModel?.AmazonOrderReferenceId;
            if (!Equals(submitPaymentViewModel.PaymentType, null) && submitPaymentViewModel.PaymentType.Equals(ZnodeConstant.AmazonPay.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                model.ReturnUrl = submitPaymentViewModel?.AmazonPayReturnUrl;
                model.CancelUrl = submitPaymentViewModel?.AmazonPayCancelUrl;
            }
            else
            {
                model.ReturnUrl = submitPaymentViewModel.PayPalReturnUrl;
                model.CancelUrl = submitPaymentViewModel.PayPalCancelUrl;
            }

            foreach (ShoppingCartItemModel item in shoppingCart.ShoppingCartItems)
            {
                int cartCount = shoppingCart.ShoppingCartItems.Count;
                CartItemModel objCartItem = new CartItemModel();
                objCartItem.ProductName = item.ProductName;
                objCartItem.ProductAmount = item.UnitPrice;
                objCartItem.ProductDescription = item.Description;
                objCartItem.ProductNumber = item.SKU;
                objCartItem.Quantity = GetQuantity(item);
                model.CartItems.Add(objCartItem);
            }
            return model;

        }

        public static SubmitPaymentModel ToAmazonPaySubmitPayModel(PaymentSettingModel paymentSettingModel, string amazonOrderReferenceId, string total, string accessToken)
        {
            SubmitPaymentModel model = new SubmitPaymentModel();
            model.PaymentCode = paymentSettingModel.PaymentCode;
            model.PaymentSettingId = paymentSettingModel.PaymentSettingId;
            model.AmazonOrderReferenceId = amazonOrderReferenceId;
            model.GatewayLoginPassword = paymentSettingModel.GatewayPassword;
            model.GatewayTransactionKey = paymentSettingModel.TransactionKey;
            model.GatewayLoginName = paymentSettingModel.GatewayUsername;
            model.Total = total;
            model.AccessToken = accessToken;
            return model;
        }
        public static AddressViewModel ToAddressViewModel(SubmitPaymentModel submitPaymentModel)
        {
            AddressViewModel model = new AddressViewModel();
            model.StateCode = submitPaymentModel?.BillingStateCode;
            model.PostalCode = submitPaymentModel?.BillingPostalCode;
            model.EmailAddress = submitPaymentModel?.BillingEmailId;
            model.FirstName = submitPaymentModel?.BillingName;
            model.LastName = submitPaymentModel?.BillingName;
            model.Address1 = submitPaymentModel?.BillingStreetAddress1;
            model.Address2 = submitPaymentModel?.BillingStreetAddress2;
            model.PhoneNumber = submitPaymentModel?.BillingPhoneNumber;
            model.StateName = submitPaymentModel?.BillingStateCode;
            model.CountryName = submitPaymentModel?.BillingCountryCode;
            model.CityName = submitPaymentModel?.BillingCity;
            return model;
        }
        public static PaymentModel ToPaymentModel(ShoppingCartModel model, PaymentSettingModel paymentSetting, string paymentName)
        {
            if (HelperUtility.IsNull(paymentSetting))
                paymentSetting = new PaymentSettingModel();

            return model.Payment = new PaymentModel
            {
                BillingAddress = model.BillingAddress,
                ShippingAddress = model.ShippingAddress,
                PaymentSetting = paymentSetting,
                PaymentDisplayName = paymentSetting.PaymentDisplayName,
                PaymentName = paymentName,
                IsPreAuthorize = paymentSetting.PreAuthorize,
                TestMode = paymentSetting.TestMode,
                PaymentExternalId = !string.IsNullOrEmpty(model?.Payment?.PaymentExternalId) ? model.Payment.PaymentExternalId : paymentSetting.PaymentExternalId
            };
        }

        //to get quantity of the product
        private static decimal GetQuantity(ShoppingCartItemModel product)
        {
            decimal quantity = 0;
            if (product?.GroupProducts?.Count > 0)
            {
                foreach (AssociatedProductModel group in product?.GroupProducts)
                {
                    quantity += group.Quantity;
                }
            }
            else
            {
                quantity = product.Quantity;
            }
            return quantity;
        }

        //to get locale wise amount
        private static string GetLocaleWiseAmount(decimal amount)
        {
            string formattedAmount = Convert.ToString(amount);
            if (!string.IsNullOrEmpty(formattedAmount) && formattedAmount.Contains(","))
                formattedAmount = formattedAmount.Replace(",", ".");
            return formattedAmount;
        }

        //Convert the model to Payment Model.
        public static SubmitPaymentModel ToModel(ShoppingCartModel shoppingCart, SubmitQuoteViewModel submitPaymentViewModel)
        {
            SubmitPaymentModel model = new SubmitPaymentModel();

            if (!Equals(shoppingCart, null) && !Equals(shoppingCart.BillingAddress, null))
            {
                model.BillingCity = shoppingCart.BillingAddress?.CityName;
                model.BillingFirstName = shoppingCart.BillingAddress?.FirstName;
                model.BillingLastName = shoppingCart.BillingAddress?.LastName;
                model.BillingCountryCode = shoppingCart.BillingAddress?.CountryName;
                model.BillingName = shoppingCart.BillingAddress?.DisplayName;
                model.BillingPhoneNumber = shoppingCart.BillingAddress?.PhoneNumber;
                model.BillingPostalCode = shoppingCart.BillingAddress?.PostalCode;
                model.BillingStateCode = !string.IsNullOrEmpty(shoppingCart?.BillingAddress?.StateCode) ? shoppingCart?.BillingAddress?.StateCode : shoppingCart?.BillingAddress?.StateName;
                model.BillingStreetAddress1 = shoppingCart?.BillingAddress?.Address1;
                model.BillingStreetAddress2 = shoppingCart?.BillingAddress?.Address2;
                model.BillingEmailId = shoppingCart?.BillingAddress?.EmailAddress;
                model.CompanyName = shoppingCart?.BillingAddress?.CompanyName;
            }
            else
            {
                model.BillingCity = shoppingCart.ShippingAddress?.CityName;
                model.BillingFirstName = shoppingCart.ShippingAddress?.FirstName;
                model.BillingLastName = shoppingCart.ShippingAddress?.LastName;
                model.BillingCountryCode = shoppingCart.ShippingAddress?.CountryName;
                model.BillingName = shoppingCart.ShippingAddress?.DisplayName;
                model.BillingPhoneNumber = shoppingCart.ShippingAddress?.PhoneNumber;
                model.BillingPostalCode = shoppingCart.ShippingAddress?.PostalCode;
                model.BillingStateCode = !string.IsNullOrEmpty(shoppingCart?.BillingAddress?.StateCode) ? shoppingCart?.BillingAddress?.StateCode : shoppingCart?.ShippingAddress?.StateName;
                model.BillingStreetAddress1 = shoppingCart.ShippingAddress?.Address1;
                model.BillingStreetAddress2 = shoppingCart.ShippingAddress?.Address2;
                model.BillingEmailId = shoppingCart?.BillingAddress?.EmailAddress;
                model.CompanyName = shoppingCart?.BillingAddress?.CompanyName;
            }

            model.GatewayCurrencyCode = !string.IsNullOrEmpty(shoppingCart.CurrencyCode) ? shoppingCart.CurrencyCode : ZnodeConstant.UnitedStatesSuffix;
            model.OrderId = shoppingCart.OrderNumber;
            model.ShippingCity = shoppingCart.ShippingAddress?.CityName;
            model.ShippingFirstName = shoppingCart.ShippingAddress?.FirstName;
            model.ShippingLastName = shoppingCart.ShippingAddress?.LastName;
            model.ShippingCountryCode = shoppingCart.ShippingAddress?.CountryName;
            model.ShippingPhoneNumber = shoppingCart.ShippingAddress?.PhoneNumber;
            model.ShippingPostalCode = shoppingCart.ShippingAddress?.PostalCode;
            model.ShippingStateCode = shoppingCart.ShippingAddress?.StateCode;
            model.ShippingStreetAddress1 = shoppingCart.ShippingAddress?.Address1;
            model.ShippingStreetAddress2 = shoppingCart.ShippingAddress?.Address2;

            model.CardType = submitPaymentViewModel.CardType;
            model.PaymentCode = submitPaymentViewModel.PaymentCode;
            model.PaymentSettingId = submitPaymentViewModel.PaymentSettingId.GetValueOrDefault();
            model.CustomerProfileId = submitPaymentViewModel.CustomerProfileId;
            model.CustomerPaymentProfileId = submitPaymentViewModel.CustomerPaymentId;
            model.PaymentToken = submitPaymentViewModel.PaymentToken;
            model.CustomerGUID = submitPaymentViewModel.CustomerGuid;
            model.CardHolderName = submitPaymentViewModel.CardHolderName;
            model.PaymentGUID = submitPaymentViewModel.PaymentGUID;
            model.GatewayCode = submitPaymentViewModel.GatewayCode;

            model.SubTotal = GetLocaleWiseAmount(shoppingCart.SubTotal.GetValueOrDefault());
            model.Total = GetLocaleWiseAmount(shoppingCart.Total.GetValueOrDefault());
            model.TaxCost = GetLocaleWiseAmount(shoppingCart.TaxCost);
            model.GiftCardAmount = GetLocaleWiseAmount(shoppingCart.GiftCardAmount);
            model.ShippingCost = GetLocaleWiseAmount(shoppingCart.ShippingCost);
            model.Discount = GetLocaleWiseAmount(shoppingCart.Discount);
            //Amazon pay properties.
            model.AmazonOrderReferenceId = submitPaymentViewModel?.AmazonOrderReferenceId;
            if (!Equals(submitPaymentViewModel.PaymentType, null) && submitPaymentViewModel.PaymentType.Equals(ZnodeConstant.AmazonPay.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                model.ReturnUrl = submitPaymentViewModel?.AmazonPayReturnUrl;
                model.CancelUrl = submitPaymentViewModel?.AmazonPayCancelUrl;
            }
            else
            {
                model.ReturnUrl = submitPaymentViewModel.PayPalReturnUrl;
                model.CancelUrl = submitPaymentViewModel.PayPalCancelUrl;
            }

            foreach (ShoppingCartItemModel item in shoppingCart.ShoppingCartItems)
            {
                int cartCount = shoppingCart.ShoppingCartItems.Count;
                CartItemModel objCartItem = new CartItemModel();
                objCartItem.ProductName = item.ProductName;
                objCartItem.ProductAmount = item.UnitPrice;
                objCartItem.ProductDescription = item.Description;
                objCartItem.ProductNumber = item.SKU;
                objCartItem.Quantity = GetQuantity(item);
                model.CartItems.Add(objCartItem);
            }
            model.PortalName = submitPaymentViewModel.PortalName;
            return model;

        }
    }
}
