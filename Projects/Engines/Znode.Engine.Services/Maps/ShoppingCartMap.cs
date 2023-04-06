using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Admin;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.ShoppingCart;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Entities = Znode.Libraries.ECommerce.Entities;
namespace Znode.Engine.Services.Maps
{
    public class ShoppingCartMap : IShoppingCartMap
    {

        private readonly IShoppingCartItemMap shoppingCartItemMap;

        public ShoppingCartMap()
        {
            shoppingCartItemMap = GetService<IShoppingCartItemMap>();
        }

        public virtual ShoppingCartModel ToModel(ZnodeShoppingCart znodeCart, IImageHelper objImage = null)
        {
            if (IsNull(znodeCart))
                return new ShoppingCartModel();

            ShoppingCartModel model = new ShoppingCartModel
            {
                Discount = znodeCart.Discount,
                GiftCardAmount = znodeCart.GiftCardAmount,
                GiftCardApplied = znodeCart.IsGiftCardApplied,
                GiftCardMessage = znodeCart.GiftCardMessage,
                GiftCardNumber = znodeCart.GiftCardNumber,
                GiftCardValid = znodeCart.IsGiftCardValid,
                GiftCardBalance = znodeCart.GiftCardBalance,
                MultipleShipToEnabled = znodeCart.IsMultipleShipToAddress,
                OrderLevelDiscount = znodeCart.OrderLevelDiscount,
                OrderLevelShipping = znodeCart.OrderLevelShipping,
                OrderLevelTaxes = znodeCart.OrderLevelTaxes,
                ProfileId = znodeCart.ProfileId,
                PortalId = znodeCart.PortalId.GetValueOrDefault(),
                UserId = znodeCart.UserId,
                SalesTax = znodeCart.SalesTax,
                ShippingCost = IsNotNull(znodeCart.Shipping) ? znodeCart.ShippingCost : 0,
                ShippingDifference = IsNotNull(znodeCart.Shipping) ? znodeCart.ShippingDifference : 0,
                ShippingDiscount = IsNotNull(znodeCart.Shipping) ? znodeCart.Shipping.ShippingDiscount : 0,
                ShippingHandlingCharges = IsNotNull(znodeCart.Shipping?.ShippingHandlingCharge == 0) ? znodeCart.ShippingHandlingCharges : znodeCart.Shipping.ShippingHandlingCharge,
                SubTotal = znodeCart.SubTotal,
                TaxCost = znodeCart.TaxCost,
                Shipping = ShippingMap.ToModel(znodeCart.Shipping),
                TaxRate = znodeCart.TaxRate,
                Total = znodeCart.Total,
                LocaleId = znodeCart.LocalId,
                PublishedCatalogId = znodeCart.PublishedCatalogId,
                OmsOrderId = znodeCart.OrderId,
                ShippingId = znodeCart.Shipping.ShippingID,
                CSRDiscountAmount = znodeCart.CSRDiscountAmount,
                CSRDiscountDescription = znodeCart.CSRDiscountDescription,
                CSRDiscountApplied = znodeCart.CSRDiscountApplied,
                CSRDiscountMessage = znodeCart.CSRDiscountMessage,
                CurrencyCode = znodeCart.CurrencyCode,
                CultureCode = znodeCart.CultureCode,
                CurrencySuffix = znodeCart.CurrencySuffix,
                CookieMappingId = new ZnodeEncryption().EncryptData(znodeCart.CookieMappingId.ToString()),
                CustomShippingCost = znodeCart.CustomShippingCost,
                CustomTaxCost = znodeCart.CustomTaxCost,
                IsLineItemReturned = znodeCart.IsLineItemReturned,
                EstimateShippingCost = znodeCart.EstimateShippingCost,
                Custom1 = znodeCart.Custom1,
                Custom2 = znodeCart.Custom2,
                Custom3 = znodeCart.Custom3,
                Custom4 = znodeCart.Custom4,
                Custom5 = znodeCart.Custom5,
                TotalAdditionalCost = znodeCart.TotalAdditionalCost,
                IsAllowWithOtherPromotionsAndCoupons = znodeCart.IsAllowWithOtherPromotionsAndCoupons,
                IsCalculateVoucher = znodeCart.IsCalculateVoucher,
                IsAllVoucherRemoved = znodeCart.IsAllVoucherRemoved,
                IsShippingRecalculate = znodeCart.IsShippingRecalculate,
                ReturnCharges=znodeCart.ReturnCharges,
                InvalidOrderLevelShippingDiscount = IsNotNull(znodeCart?.InvalidOrderLevelShippingDiscount)
                                                   ? znodeCart?.InvalidOrderLevelShippingDiscount : new List<OrderPromotionModel>(),
                InvalidOrderLevelShippingPromotion = IsNotNull(znodeCart?.InvalidOrderLevelShippingPromotion)
                                                   ? znodeCart?.InvalidOrderLevelShippingPromotion : new List<PromotionModel>(),
                IsShippingDiscountRecalculate = znodeCart.IsShippingDiscountRecalculate,
                IsCalculateTaxAfterDiscount = znodeCart.IsCalculateTaxAfterDiscount,
                InHandDate = znodeCart.InHandDate,
                OrderTotalWithoutVoucher = znodeCart.OrderTotalWithoutVoucher,
                IsOldOrder = znodeCart.IsOldOrder,
                FreeShipping = znodeCart.FreeShipping,
                ImportDuty = znodeCart.ImportDuty,
                TaxSummaryList = znodeCart.TaxSummaryList,
                TaxMessageList = znodeCart.TaxMessageList,
                CSRDiscountEdited = znodeCart.CSRDiscountEdited,
                IsPricesInclusiveOfTaxes = znodeCart.IsPricesInclusiveOfTaxes,
                IsPendingOrderRequest = znodeCart.IsPendingOrderRequest,
                AvataxIsSellerImporterOfRecord = znodeCart.AvataxIsSellerImporterOfRecord
            };

            model.OrderLevelDiscountDetails = znodeCart.OrderLevelDiscountDetails?.DistinctBy(x => x.DiscountCode)?.ToList();

            foreach (Entities.ZnodeCoupon coupon in znodeCart?.Coupons)
                model.Coupons.Add(CouponMap.ToModel(coupon));

            foreach (Entities.ZnodeVoucher voucher in znodeCart?.Vouchers)
                model.Vouchers.Add(VoucherMap.ToVoucherModel(voucher));
         
            if ((model.ShippingCost.Equals(0) && (model.Shipping.ShippingDiscount > 0) && (model.Shipping.ShippingId > 0)) || model.Shipping.ShippingDiscountApplied)
                model.FreeShipping = true;

            int FreeShippingItemsCount = 0;
            int NonFreeShippingItemsCount = 0;
            foreach (ZnodeShoppingCartItem cartItem in znodeCart.ShoppingCartItems)
            {
                model.ShoppingCartItems.Add(shoppingCartItemMap.ToModel(cartItem, znodeCart, objImage));
                OrderAttributeModel orderAttributeModel = cartItem.Product.Attributes.Find(x => string.Equals(x.AttributeCode, "IsDownloadable", StringComparison.InvariantCultureIgnoreCase) && string.Equals(x.AttributeValue, "true", StringComparison.InvariantCultureIgnoreCase));
                if (cartItem.Product.FreeShippingInd || orderAttributeModel != null)
                    FreeShippingItemsCount++;
                else NonFreeShippingItemsCount++;
            }
            if (NonFreeShippingItemsCount <= 0 && FreeShippingItemsCount > 0) model.FreeShipping = true;
            return model;
        }

        public virtual ZnodeShoppingCart ToZnodeShoppingCart(ShoppingCartModel model, UserAddressModel userDetails = null, List<string> expands = null)
        {
            if (IsNull(model))
                return null;

            ZnodeShoppingCart znodeCart = (ZnodeShoppingCart)GetService<IZnodeShoppingCart>();

            znodeCart.GiftCardAmount = model.GiftCardAmount;
            znodeCart.GiftCardMessage = model.GiftCardMessage;
            znodeCart.GiftCardNumber = model.GiftCardNumber ?? String.Empty;
            znodeCart.IsGiftCardApplied = model.GiftCardApplied;
            znodeCart.IsGiftCardValid = model.GiftCardValid;
            znodeCart.OrderLevelDiscount = model.OrderLevelDiscount;
            znodeCart.OrderLevelShipping = model.OrderLevelShipping;
            znodeCart.OrderLevelTaxes = model.OrderLevelTaxes;
            znodeCart.PayerId = model.PayerId;
            znodeCart.Payment = PaymentMap.ToZnodePayment(model.Payment, (model.MultipleShipToEnabled) ? model?.ShoppingCartItems?[0].ShippingAddress : model.ShippingAddress, model.BillingAddress);
            znodeCart.ProfileId = model.ProfileId;
            znodeCart.SalesTax = model.SalesTax;
            znodeCart.Shipping = ShippingMap.ToZnodeShipping(model.Shipping);
            znodeCart.TaxRate = model.TaxRate;
            znodeCart.Token = model.Token;
            znodeCart.PortalId = model.PortalId;
            znodeCart.LoginUserName = model?.UserDetails?.UserName ?? string.Empty;
            znodeCart.OrderId = model.OmsOrderId;
            znodeCart.OrderDate = model.OrderDate;
            znodeCart.UserId = model.UserId;
            znodeCart.LocalId = model.LocaleId;
            znodeCart.CookieMappingId = !string.IsNullOrEmpty(model.CookieMappingId) ? Convert.ToInt32(new ZnodeEncryption().DecryptData(model.CookieMappingId)) : 0;
            znodeCart.PublishedCatalogId = model.PublishedCatalogId;
            znodeCart.CSRDiscountAmount = model.CSRDiscountAmount;
            znodeCart.CSRDiscountDescription = model.CSRDiscountDescription;
            znodeCart.CSRDiscountApplied = model.CSRDiscountApplied;
            znodeCart.CSRDiscountMessage = model.CSRDiscountMessage;
            znodeCart.CurrencyCode = model.CurrencyCode;
            znodeCart.CultureCode = model.CultureCode;
            znodeCart.CurrencySuffix = model.CurrencySuffix;
            znodeCart.OrderAttribute = model.OrderAttribute;
            znodeCart.PersonaliseValuesList = model.ShoppingCartItems.Count > 0 ? model?.ShoppingCartItems?[0].PersonaliseValuesList : new System.Collections.Generic.Dictionary<string, object>();
            znodeCart.CustomShippingCost = model.CustomShippingCost;
            znodeCart.CustomTaxCost = model.CustomTaxCost;
            znodeCart.ExternalId = model.ExternalId;
            znodeCart.IsLineItemReturned = model.IsLineItemReturned;
            znodeCart.IsCchCalculate = model.IsCchCalculate;
            znodeCart.ReturnItemList = model.ReturnItemList;
            znodeCart.Custom1 = model.Custom1;
            znodeCart.Custom2 = model.Custom2;
            znodeCart.Custom3 = model.Custom3;
            znodeCart.Custom4 = model.Custom4;
            znodeCart.Custom5 = model.Custom5;
            znodeCart.EstimateShippingCost = model.EstimateShippingCost;
            znodeCart.TotalAdditionalCost = model.TotalAdditionalCost.GetValueOrDefault();
            znodeCart.IsAllowWithOtherPromotionsAndCoupons = model.IsAllowWithOtherPromotionsAndCoupons;
            znodeCart.PublishStateId = model.PublishStateId;
            znodeCart.IpAddress = model.IpAddress;
            znodeCart.InHandDate = model.InHandDate;
            znodeCart.JobName = model.JobName;
            znodeCart.ShippingConstraintCode = model.ShippingConstraintCode;
            znodeCart.IsCalculatePromotionAndCoupon = model.IsCalculatePromotionAndCoupon;
            znodeCart.IsShippingRecalculate = model.IsShippingRecalculate;
            znodeCart.ShippingDiscount = IsNotNull(model.Shipping) ? model.Shipping.ShippingDiscount : 0;
            znodeCart.ShippingHandlingCharges = IsNotNull(model?.Shipping?.ShippingHandlingCharge) && (model?.Shipping?.ShippingHandlingCharge != 0) ? model.Shipping.ShippingHandlingCharge : model.ShippingHandlingCharges;
             znodeCart.ReturnCharges = model.ReturnCharges;
            znodeCart.IsRemoveShippingDiscount = model.IsRemoveShippingDiscount;
            znodeCart.InvalidOrderLevelShippingDiscount = model.InvalidOrderLevelShippingDiscount;
            znodeCart.InvalidOrderLevelShippingPromotion = model.InvalidOrderLevelShippingPromotion;
            znodeCart.IsShippingDiscountRecalculate = model.IsShippingDiscountRecalculate;
            znodeCart.IsOldOrder = model.IsOldOrder;
            znodeCart.IsCalculateTaxAfterDiscount = model.IsCalculateTaxAfterDiscount;
            znodeCart.IsCallLiveShipping = model.IsCallLiveShipping ? false : model.IsOldOrder;
            znodeCart.SkipShippingCalculations = model.SkipShippingCalculations;
            znodeCart.FreeShipping = model.FreeShipping;
            znodeCart.BusinessIdentificationNumber = model.UserDetails?.BusinessIdentificationNumber;
            znodeCart.IsPricesInclusiveOfTaxes = model.IsPricesInclusiveOfTaxes;
            znodeCart.IsTaxExempt = model.IsTaxExempt;
            znodeCart.AvataxIsSellerImporterOfRecord = model.AvataxIsSellerImporterOfRecord;
            
            znodeCart.CSRDiscountEdited = model.CSRDiscountEdited;
            if (IsNull(znodeCart.Payment))
                znodeCart.Payment = new Entities.ZnodePayment();

            znodeCart.Payment.BillingAddress = model?.BillingAddress ?? new AddressModel();

            if (Equals(model.ShippingAddress, null))
                model.ShippingAddress = model?.Payment?.ShippingAddress ?? new AddressModel();


            if (IsNotNull(userDetails))
                znodeCart.UserAddress = userDetails;

            IPublishProductHelper publishProductHelper = GetService<IPublishProductHelper>();

            List<string> skus = new List<string>();

            foreach (ShoppingCartItemModel item in model.ShoppingCartItems)
            {
                if (!string.IsNullOrEmpty(item.ConfigurableProductSKUs))
                {
                    skus.Add(item.ConfigurableProductSKUs.ToLower());
                }
                else if (item?.GroupProducts?.Count > 0)
                {
                    skus.Add(item.GroupProducts[0]?.Sku.ToLower());
                }

                skus.Add(item.SKU.ToLower());
            }
            skus = skus.Distinct().ToList();

            List<PublishedConfigurableProductEntityModel> configEntities;

            if (IsNull(expands))
                expands = new List<string> { ZnodeConstant.Promotions, ZnodeConstant.Pricing, ZnodeConstant.Inventory, ZnodeConstant.SEO };

            int catalogVersionId = publishProductHelper.GetCatalogVersionId(model.PublishedCatalogId,model.LocaleId);

            bool includeInactiveProduct = false;

            //If order status is cancelled then allow inactive products for cancellation.
            if (string.Equals(model.OrderStatus, ZnodeOrderStatusEnum.CANCELED.ToString(), StringComparison.InvariantCultureIgnoreCase))
                includeInactiveProduct = true;

            List<PublishProductModel> cartLineItemsProductData = publishProductHelper.GetDataForCartLineItems(skus, model.PublishedCatalogId, model.LocaleId, expands, model.UserId.GetValueOrDefault(), model.PortalId, catalogVersionId, out configEntities, includeInactiveProduct, model.OmsOrderId.GetValueOrDefault());

            List<TaxClassRuleModel> lstTaxClassSKUs = publishProductHelper.GetTaxRules(skus);

            List<ZnodePimDownloadableProduct> lstDownloadableProducts = new ZnodeRepository<ZnodePimDownloadableProduct>().Table.Where(x => skus.Contains(x.SKU)).ToList();

            znodeCart.AddtoShoppingBagV2(model, cartLineItemsProductData, catalogVersionId, lstTaxClassSKUs, lstDownloadableProducts, configEntities, expands);

            if (IsNotNull(model?.Coupons))
            {
                foreach (CouponModel coupon in model.Coupons)
                    znodeCart.Coupons.Add(CouponMap.ToZnodeCoupon(coupon));
            }

            if (IsNotNull(model?.Vouchers))
            {
                foreach (VoucherModel voucher in model.Vouchers)
                    znodeCart.Vouchers.Add(VoucherMap.ToZnodeVoucher(voucher));
            }
            znodeCart.IsCalculateVoucher = model.IsCalculateVoucher;
            znodeCart.IsAllVoucherRemoved = model.IsAllVoucherRemoved;
            znodeCart.IsQuoteOrder = model.IsQuoteOrder;
            znodeCart.IsPendingOrderRequest = model.IsPendingOrderRequest;
            if(IsNotNull(model.OmsOrderId) && model.OmsOrderId > 0 && model.ShippingCost > 0 && znodeCart.ShippingCost == 0.0M && !model.IsOldOrder)
            {
                znodeCart.ShippingCost = model.ShippingCost;
            }
            return znodeCart;
        }
    }
}
