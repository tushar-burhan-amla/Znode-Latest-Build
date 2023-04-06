using System;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services.Maps
{
    public class PaymentViewModelMap
    {
        public static SubmitPaymentModel ToModel(ShoppingCartModel shoppingCart, ConvertQuoteToOrderModel convertToOrderModel)
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
                //model.CardType = submitPaymentViewModel.CardType;
                model.PortalName = shoppingCart.PortalName;
            }

            model.PaymentApplicationSettingId = convertToOrderModel.PaymentDetails.PaymentSettingId;
            model.PaymentSettingId = convertToOrderModel.PaymentDetails.PaymentSettingId;
            model.PaymentCode = convertToOrderModel.PaymentDetails.PaymentCode;
            model.CustomerProfileId = convertToOrderModel.PaymentDetails.CustomerProfileId;
            model.CustomerPaymentProfileId = convertToOrderModel.PaymentDetails.CustomerPaymentId;
            model.PaymentToken = convertToOrderModel.PaymentDetails.PaymentToken; // !string.IsNullOrEmpty(submitPaymentViewModel.PayPalToken) ? submitPaymentViewModel.PayPalToken : submitPaymentViewModel.PaymentToken;
            model.CustomerGUID = convertToOrderModel.PaymentDetails.CustomerGuid;
            model.PaymentGUID = convertToOrderModel.PaymentDetails.PaymentGUID;
            model.GatewayCode = convertToOrderModel.PaymentDetails.GatewayCode;
            
            model.CyberSourceToken = convertToOrderModel.PaymentDetails.CyberSourceToken;  // !string.IsNullOrEmpty(submitPaymentViewModel.PayPalToken) ? submitPaymentViewModel.PayPalToken : submitPaymentViewModel.PaymentToken;
            model.CustomerShippingAddressId = convertToOrderModel.PaymentDetails.CustomerShippingAddressId;
            //model.CardSecurityCode = submitPaymentViewModel.CardSecurityCode;

            model.SubTotal = GetLocaleWiseAmount(shoppingCart.SubTotal.GetValueOrDefault());
            model.Total = GetLocaleWiseAmount(shoppingCart.Total.GetValueOrDefault());
            model.TaxCost = GetLocaleWiseAmount(shoppingCart.TaxCost);
            model.GiftCardAmount = GetLocaleWiseAmount(shoppingCart.GiftCardAmount);
            model.ShippingCost = GetLocaleWiseAmount(shoppingCart.ShippingCost);
            model.ShippingHandlingCharges = GetLocaleWiseAmount(shoppingCart.ShippingHandlingCharges);
            model.Discount = GetLocaleWiseAmount(shoppingCart.Discount);
            model.TransactionId = convertToOrderModel.PaymentDetails.TransactionId;
            model.IsSaveCreditCard = convertToOrderModel.PaymentDetails.IsSaveCreditCard;
            model.UserId = convertToOrderModel.UserId;
            model.CardNumber = convertToOrderModel.PaymentDetails.CreditCardNumber;
            //Amazon pay properties.
            model.AmazonOrderReferenceId = convertToOrderModel?.PaymentDetails.AmazonOrderReferenceId;
            if (!Equals(convertToOrderModel.PaymentDetails.PaymentType, null) && convertToOrderModel.PaymentDetails.PaymentType.Equals(ZnodeConstant.AmazonPay.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                model.ReturnUrl = convertToOrderModel?.PaymentDetails.AmazonPayReturnUrl;
                model.CancelUrl = convertToOrderModel?.PaymentDetails.AmazonPayCancelUrl;
            }
            else
            {
                model.ReturnUrl = convertToOrderModel.PaymentDetails.PayPalReturnUrl;
                model.CancelUrl = convertToOrderModel.PaymentDetails.PayPalCancelUrl;
                model.PaymentToken = convertToOrderModel.PaymentDetails.PayPalToken;
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

        //Map Details for SubmitPaymentModel
        public static SubmitPaymentModel ToModel(ShoppingCartModel shoppingCart, PayInvoiceModel payInvoiceModel)
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
            }

            model.PaymentApplicationSettingId = payInvoiceModel.PaymentDetails.PaymentSettingId;
            model.PaymentSettingId = payInvoiceModel.PaymentDetails.PaymentSettingId;
            model.PaymentCode = payInvoiceModel.PaymentDetails.PaymentCode;
            model.CustomerProfileId = payInvoiceModel.PaymentDetails.CustomerProfileId;
            model.CustomerPaymentProfileId = payInvoiceModel.PaymentDetails.CustomerPaymentId;
            model.PaymentToken = payInvoiceModel.PaymentDetails.PaymentToken; 
            model.CustomerGUID = payInvoiceModel.PaymentDetails.CustomerGuid;
            model.CustomerShippingAddressId = payInvoiceModel.PaymentDetails.CustomerShippingAddressId;
            if (!Equals(payInvoiceModel.PaymentDetails.GatewayCode, null) && payInvoiceModel.PaymentDetails.GatewayCode.Equals(ZnodeConstant.CyberSource.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                model.CyberSourceToken = payInvoiceModel.PaymentDetails.CyberSourceToken;  // !string.IsNullOrEmpty(submitPaymentViewModel.PayPalToken) ? submitPaymentViewModel.PayPalToken : submitPaymentViewModel.PaymentToken;
                model.CustomerShippingAddressId = payInvoiceModel.PaymentDetails.CustomerShippingAddressId;
                model.PaymentGUID = payInvoiceModel.PaymentDetails.PaymentGUID;
            }

            if (!Equals(payInvoiceModel.PaymentDetails.GatewayCode, null) && payInvoiceModel.PaymentDetails.GatewayCode.Equals(ZnodeConstant.AuthorizeNet.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                model.TransactionId = payInvoiceModel.PaymentDetails.TransactionId;
                model.IsSaveCreditCard = payInvoiceModel.PaymentDetails.IsSaveCreditCard;
                model.UserId = payInvoiceModel.UserId;
                model.CardNumber = payInvoiceModel.PaymentDetails.CreditCardNumber;
            }

            model.SubTotal = GetLocaleWiseAmount(shoppingCart.SubTotal.GetValueOrDefault());
            model.Total = GetLocaleWiseAmount(shoppingCart.Total.GetValueOrDefault());
            model.TaxCost = GetLocaleWiseAmount(shoppingCart.TaxCost);
            model.GiftCardAmount = GetLocaleWiseAmount(shoppingCart.GiftCardAmount);
            model.ShippingCost = GetLocaleWiseAmount(shoppingCart.ShippingCost);
            model.ShippingHandlingCharges = GetLocaleWiseAmount(shoppingCart.ShippingHandlingCharges);
            model.Discount = GetLocaleWiseAmount(shoppingCart.Discount);

            //Amazon pay properties.
            model.AmazonOrderReferenceId = payInvoiceModel?.PaymentDetails.AmazonOrderReferenceId;
            if (!Equals(payInvoiceModel.PaymentDetails.PaymentType, null) && payInvoiceModel.PaymentDetails.PaymentType.Equals(ZnodeConstant.AmazonPay.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                model.ReturnUrl = payInvoiceModel?.PaymentDetails.AmazonPayReturnUrl;
                model.CancelUrl = payInvoiceModel?.PaymentDetails.AmazonPayCancelUrl;
            }
            else
            {
                model.ReturnUrl = payInvoiceModel.PaymentDetails.PayPalReturnUrl;
                model.CancelUrl = payInvoiceModel.PaymentDetails.PayPalCancelUrl;
                model.PaymentToken = payInvoiceModel.PaymentDetails.PayPalToken;
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
    }

}
