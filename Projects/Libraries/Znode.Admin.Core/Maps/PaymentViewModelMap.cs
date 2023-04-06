using Znode.Engine.Api.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Admin.Extensions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Maps
{
    public class PaymentViewModelMap
    {

        //Map PaymentSettingCredentialsModel to PaymentSettingsModel
        public static PaymentSettingViewModel ToPaymentSettingsViewModel(PaymentSettingCredentialModel paymentSettingCredentialsModel, PaymentSettingViewModel paymentSettingViewModel, bool isTestMode)
        {
            if (!Equals(paymentSettingCredentialsModel, null))
            {
                paymentSettingViewModel.TestMode = isTestMode;
                paymentSettingViewModel.GatewayPassword = paymentSettingCredentialsModel.GatewayPassword;
                paymentSettingViewModel.GatewayUsername = paymentSettingCredentialsModel.GatewayUsername;
                paymentSettingViewModel.TransactionKey = paymentSettingCredentialsModel.TransactionKey;
                paymentSettingViewModel.Vendor = paymentSettingCredentialsModel.Vendor;
                paymentSettingViewModel.Partner = paymentSettingCredentialsModel.Partner;

            }
            else
            {
                paymentSettingViewModel.TestMode = isTestMode;
                paymentSettingViewModel.GatewayPassword = string.Empty;
                paymentSettingViewModel.GatewayUsername = string.Empty;
                paymentSettingViewModel.TransactionKey = string.Empty;
                paymentSettingViewModel.Vendor = string.Empty;
                paymentSettingViewModel.Partner = string.Empty;
            }
            return paymentSettingViewModel;
        }

        //TO DO: Need to Refactor this method.
        public static SubmitPaymentModel ToModel(ShoppingCartModel shoppingCart, SubmitPaymentViewModel submitPaymentViewModel)
        {
            SubmitPaymentModel model = new SubmitPaymentModel();

            if (!Equals(shoppingCart, null) && !Equals(shoppingCart.BillingAddress, null))
            {
                model.BillingCity = shoppingCart.BillingAddress.CityName;
                model.BillingFirstName = shoppingCart.BillingAddress.FirstName;
                model.BillingLastName = shoppingCart.BillingAddress.LastName;
                model.BillingCountryCode = shoppingCart.BillingAddress.CountryName;
                model.BillingName = shoppingCart.BillingAddress.DisplayName;
                model.BillingPhoneNumber = shoppingCart.BillingAddress.PhoneNumber;
                model.BillingPostalCode = shoppingCart.BillingAddress.PostalCode;
                model.BillingStateCode = !string.IsNullOrEmpty(shoppingCart.BillingAddress.StateCode) ? shoppingCart.BillingAddress.StateCode : shoppingCart.BillingAddress.StateName;
                model.BillingStreetAddress1 = shoppingCart.BillingAddress.Address1;
                model.BillingStreetAddress2 = shoppingCart.BillingAddress.Address2;
                model.CompanyName = shoppingCart?.BillingAddress?.CompanyName;
            }
            else
            {
                model.BillingCity = shoppingCart.ShippingAddress.CityName;
                model.BillingFirstName = shoppingCart.ShippingAddress.FirstName;
                model.BillingLastName = shoppingCart.ShippingAddress.LastName;
                model.BillingCountryCode = shoppingCart.ShippingAddress.CountryName;
                model.BillingName = shoppingCart.ShippingAddress.DisplayName;
                model.BillingPhoneNumber = shoppingCart.ShippingAddress.PhoneNumber;
                model.BillingPostalCode = shoppingCart.ShippingAddress.PostalCode;
                model.BillingStateCode = !string.IsNullOrEmpty(shoppingCart.ShippingAddress.StateCode) ? shoppingCart.ShippingAddress.StateCode : shoppingCart.ShippingAddress.StateName;
                model.BillingStreetAddress1 = shoppingCart.ShippingAddress.Address1;
                model.BillingStreetAddress2 = shoppingCart.ShippingAddress.Address2;
                model.CompanyName = shoppingCart?.ShippingAddress?.CompanyName;
            }
            model.BillingEmailId = string.IsNullOrEmpty(model.BillingEmailId) ? shoppingCart.UserDetails?.Email : model.BillingEmailId;
            model.SubTotal = shoppingCart.SubTotal.ToString();
            model.Total = shoppingCart.Total.ToPriceRoundOff();
            model.OrderId = shoppingCart.OrderNumber;
            model.TaxCost = shoppingCart.TaxCost.ToString();
            model.GiftCardAmount = shoppingCart.GiftCardAmount.ToString();
            model.GatewayCurrencyCode = !string.IsNullOrEmpty(shoppingCart.CurrencyCode) ? shoppingCart.CurrencyCode : ZnodeConstant.UnitedStatesSuffix;
            model.ShippingCity = shoppingCart.ShippingAddress.CityName;
            model.ShippingFirstName = shoppingCart.ShippingAddress.FirstName;
            model.ShippingLastName = shoppingCart.ShippingAddress.LastName;
            model.ShippingCountryCode = shoppingCart.ShippingAddress.CountryName;
            model.ShippingPhoneNumber = shoppingCart.ShippingAddress.PhoneNumber;
            model.ShippingPostalCode = shoppingCart.ShippingAddress.PostalCode;
            model.ShippingStateCode = !string.IsNullOrEmpty(shoppingCart.ShippingAddress.StateCode) ? shoppingCart.ShippingAddress.StateCode : shoppingCart.ShippingAddress.StateName;
            model.ShippingStreetAddress1 = shoppingCart.ShippingAddress.Address1;
            model.ShippingStreetAddress2 = shoppingCart.ShippingAddress.Address2;
            model.ShippingCost = shoppingCart.ShippingCost.ToString();
            model.Discount = shoppingCart.Discount.ToString();
            model.PaymentCode = submitPaymentViewModel.PaymentCode;
            model.PaymentSettingId = submitPaymentViewModel.PaymentSettingId;
            model.CustomerProfileId = submitPaymentViewModel.CustomerProfileId;
            model.CustomerPaymentProfileId = submitPaymentViewModel.CustomerPaymentId;
            model.CustomerShippingAddressId = submitPaymentViewModel.CustomerShippingAddressId;
            model.CardSecurityCode = submitPaymentViewModel.CardSecurityCode;
            model.PaymentToken = submitPaymentViewModel.PaymentToken;
            model.CyberSourceToken = submitPaymentViewModel.CyberSourceToken;
            model.CustomerGUID = submitPaymentViewModel.CustomerGuid;
            model.CardType = submitPaymentViewModel.CardType;
            model.TransactionId = submitPaymentViewModel.TransactionId;
            model.IsSaveCreditCard = submitPaymentViewModel.IsSaveCreditCard;
            model.CardNumber = submitPaymentViewModel.CreditCardNumber;
            model.UserId = submitPaymentViewModel.UserId;
            model.CardHolderName = submitPaymentViewModel.CardHolderName;
            model.PaymentGUID = submitPaymentViewModel.PaymentGUID;
            model.GatewayCode = submitPaymentViewModel.GatewayCode;
            model.GatewayPreAuthorize = shoppingCart.Payment.IsPreAuthorize;
            model.IsACHPayment = submitPaymentViewModel.IsACHPayment;
            model.IsOrderFromAdmin = submitPaymentViewModel.IsOrderFromAdmin;
            model.PaymentMethodNonce = submitPaymentViewModel.PaymentMethodNonce; //This field is specific for braintree PCI compliance.

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
    }
}