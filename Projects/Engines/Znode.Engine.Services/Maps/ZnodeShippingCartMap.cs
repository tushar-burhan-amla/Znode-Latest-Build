using System;
using System.Collections.Generic;
using System.Linq;

using Znode.Engine.Api.Models;
using Znode.Libraries.Admin;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.ShoppingCart;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

using Entities = Znode.Libraries.ECommerce.Entities;

namespace Znode.Engine.Services.Maps
{
    public class ZnodeShippingCartMap : IZnodeShippingCartMap
    {
        #region Protected Variables

        protected readonly ZnodeShoppingCart _znodeShoppingCart;

        #endregion

        #region Constructor

        public ZnodeShippingCartMap()
        {
            _znodeShoppingCart = (ZnodeShoppingCart)GetService<IZnodeShoppingCart>();
        }
        #endregion

        #region Public Properties

        protected virtual int? PortalId
        {
            get
            {
                return _znodeShoppingCart.PortalId;
            }
            set
            {
                _znodeShoppingCart.PortalId = value;
            }
        }
        #endregion

        #region Public Methods

        //Perform mapping to shipping ZnodeShoppingCart model for shipping
        public virtual ZnodeShoppingCart ToZnodeShippingShoppingCart(ShoppingCartModel model, List<string> expands = null)
        {
            _znodeShoppingCart.OrderLevelShipping = model.OrderLevelShipping;
            _znodeShoppingCart.ProfileId = model.ProfileId;
            _znodeShoppingCart.PortalId = model.PortalId;
            _znodeShoppingCart.UserId = model.UserId;
            _znodeShoppingCart.LocalId = model.LocaleId;
            _znodeShoppingCart.CookieMappingId = !string.IsNullOrEmpty(model.CookieMappingId) ? Convert.ToInt32(new ZnodeEncryption().DecryptData(model.CookieMappingId)) : 0;
            _znodeShoppingCart.PublishedCatalogId = model.PublishedCatalogId;
            _znodeShoppingCart.CurrencyCode = model.CurrencyCode;
            _znodeShoppingCart.CultureCode = model.CultureCode;
            _znodeShoppingCart.CurrencySuffix = model.CurrencySuffix;
            _znodeShoppingCart.CustomShippingCost = model.CustomShippingCost;
            _znodeShoppingCart.EstimateShippingCost = model.EstimateShippingCost;
            _znodeShoppingCart.IsAllowWithOtherPromotionsAndCoupons = model.IsAllowWithOtherPromotionsAndCoupons;
            _znodeShoppingCart.PublishStateId = model.PublishStateId;
            _znodeShoppingCart.Payment = ToZnodePayment(model.Payment, (model.MultipleShipToEnabled) ? model?.ShoppingCartItems?[0].ShippingAddress : model.ShippingAddress, model.BillingAddress);
            _znodeShoppingCart.IsCalculatePromotionAndCoupon = model.IsCalculatePromotionAndCoupon;

            _znodeShoppingCart.IsShippingRecalculate = model.IsShippingRecalculate;
            _znodeShoppingCart.ShippingDiscount = IsNotNull(model.Shipping) ? model.Shipping.ShippingDiscount : 0;
            _znodeShoppingCart.ShippingHandlingCharges = IsNotNull(model.Shipping.ShippingHandlingCharge == 0) ? model.ShippingHandlingCharges : model.Shipping.ShippingHandlingCharge;
            _znodeShoppingCart.ReturnCharges = model.ReturnCharges;
            _znodeShoppingCart.IsRemoveShippingDiscount = model.IsRemoveShippingDiscount;
            _znodeShoppingCart.InvalidOrderLevelShippingDiscount = model.InvalidOrderLevelShippingDiscount;
            _znodeShoppingCart.InvalidOrderLevelShippingPromotion = model.InvalidOrderLevelShippingPromotion;
            _znodeShoppingCart.IsShippingDiscountRecalculate = model.IsShippingDiscountRecalculate;
            _znodeShoppingCart.IsOldOrder = model.IsOldOrder;
            _znodeShoppingCart.IsCalculateTaxAfterDiscount = model.IsCalculateTaxAfterDiscount;
            _znodeShoppingCart.IsCallLiveShipping = model.IsCallLiveShipping ? false : model.IsOldOrder;
            _znodeShoppingCart.SkipShippingCalculations = model.SkipShippingCalculations;
            _znodeShoppingCart.FreeShipping = model.FreeShipping;

            if (Equals(model.ShippingAddress, null))
                model.ShippingAddress = model?.Payment?.ShippingAddress ?? new AddressModel();

            int catalogVersionId;
            List<PublishProductModel> cartLineItemsPublishProductList = GetShoppingCartItemPublishProduct(model, expands, out catalogVersionId);

            AddToShippingShoppingBag(model, cartLineItemsPublishProductList, catalogVersionId, expands);

            _znodeShoppingCart.IsCalculateVoucher = model.IsCalculateVoucher;
            _znodeShoppingCart.IsAllVoucherRemoved = model.IsAllVoucherRemoved;
            _znodeShoppingCart.IsQuoteOrder = model.IsQuoteOrder;
            _znodeShoppingCart.IsPendingOrderRequest = model.IsPendingOrderRequest;
            _znodeShoppingCart.OrderId = model.OmsOrderId;
            return _znodeShoppingCart;

        }

        //Create the shopping bag for shipping
        public virtual void AddToShippingShoppingBag(ShoppingCartModel shoppingCartModel, List<PublishProductModel> cartLineItemsPublishProductList, int catalogVersionId, List<string> expands = null)
        {
            List<ZnodeShoppingCartItem> znodeShoppingCartItemList = new List<ZnodeShoppingCartItem>();

            //Set Portal Id in Context Header, to avoid loop based calls.
            _znodeShoppingCart.SetPortalIdInRequestHeader();

            List<ZnodeOmsSavedCartLineItem> parentProductCartLineItemsList = GetService<IZnodeOrderHelper>().GetParentSavedCartLineItem(shoppingCartModel.ShoppingCartItems.Select(x => x.ParentOmsSavedcartLineItemId.GetValueOrDefault())?.Distinct()?.ToList());

            foreach (ShoppingCartItemModel cartLineItem in shoppingCartModel.ShoppingCartItems.OrderBy(c => c.GroupSequence))
            {
                if (string.IsNullOrEmpty(cartLineItem.SKU))
                    return;

                BindShippingShoppingCartLineItemDetails(cartLineItem, shoppingCartModel, cartLineItemsPublishProductList, parentProductCartLineItemsList, znodeShoppingCartItemList, catalogVersionId);
            }

            //List of child product sku available in cart
            List<string> shoppinCartProductSkus = znodeShoppingCartItemList?.Where(x => x.ParentProductSKU != null)?.Select(x => x.ParentProductSKU)?.ToList();

            if (IsNotNull(shoppinCartProductSkus))
            {
                znodeShoppingCartItemList.Where(cartItem => cartItem.Product != null)?.ToList().ForEach(znodeCartItem =>
                {
                    _znodeShoppingCart.ZnodeShoppingCartItemCollection.Add(znodeCartItem);
                });
            }
        }

        #endregion

        #region Protected Methods 
        //Create the payment model
        protected virtual Entities.ZnodePayment ToZnodePayment(PaymentModel model, AddressModel shippingAddress, AddressModel billingAddress)
        {
            Entities.ZnodePayment znodePayment = new Entities.ZnodePayment();
            znodePayment.ShippingAddress = shippingAddress ?? model?.ShippingAddress;
            znodePayment.BillingAddress = billingAddress ?? new AddressModel();
            return znodePayment;
        }

        //Get publish data of cart item from publish entity
        protected virtual List<PublishProductModel> GetShoppingCartItemPublishProduct(ShoppingCartModel model, List<string> expands, out int catalogVersionId)
        {
            IPublishProductHelper _publishProductHelper = GetService<IPublishProductHelper>();

            List<string> productSKUList = new List<string>();

            foreach (ShoppingCartItemModel item in model.ShoppingCartItems)
            {
                if (!string.IsNullOrEmpty(item.ConfigurableProductSKUs))
                {
                    productSKUList.Add(item.ConfigurableProductSKUs.ToLower());
                }
                else if (item?.GroupProducts?.Count > 0)
                {
                    productSKUList.Add(item.GroupProducts[0]?.Sku.ToLower());
                }
                productSKUList.Add(item.SKU.ToLower());
            }
            productSKUList = productSKUList.Distinct().ToList();

            List<PublishedConfigurableProductEntityModel> configEntities;

            catalogVersionId = _publishProductHelper.GetCatalogVersionId(model.PublishedCatalogId, model.LocaleId);

            List<PublishProductModel> cartLineItemsPublishProductList = _publishProductHelper.GetDataForCartLineItems(productSKUList, model.PublishedCatalogId, model.LocaleId, expands, model.UserId.GetValueOrDefault(), model.PortalId, catalogVersionId, out configEntities,  omsOrderId: model.OmsOrderId.GetValueOrDefault());

            return cartLineItemsPublishProductList;
        }

        //Create the single cart line item model
        protected virtual void BindShippingShoppingCartLineItemDetails(ShoppingCartItemModel model, ShoppingCartModel shoppingCartModel, List<PublishProductModel> cartLineItemsPublishProductList, List<ZnodeOmsSavedCartLineItem> parentProductCartLineItemsList, List<ZnodeShoppingCartItem> znodeShoppingCartItemList, int catalogVersionId)
        {
            ZnodeShoppingCartItem znodeShoppingCartItem = MapShoppingCartItemModel(model, model.ShippingAddress);

            _znodeShoppingCart.MapShoppingCartOtherData(model, model.ShippingAddress, znodeShoppingCartItem);

            ZnodeOmsSavedCartLineItem parentCartItem = parentProductCartLineItemsList?.FirstOrDefault(x => x.OmsSavedCartLineItemId == model.ParentOmsSavedcartLineItemId);

            string parentSKU = model.SKU;
            string parentSKUProductName = string.Empty;
            string configParentSKU = !string.IsNullOrEmpty(model.ConfigurableProductSKUs) ? model.SKU : "";

            model.SKU = !string.IsNullOrEmpty(model.ConfigurableProductSKUs) ? model.ConfigurableProductSKUs : model?.GroupProducts.Count > 0 ? model?.GroupProducts[0]?.Sku : model.SKU;

            if (IsNotNull(parentCartItem) && model?.GroupProducts?.Count > 0)
            {
                parentSKU = parentCartItem.SKU;
                parentSKUProductName = parentCartItem.ProductName;
            }
            if (string.IsNullOrEmpty(parentSKUProductName))
                parentSKUProductName = model?.ProductName;

            //If Quote Id is greater than zero, bind ShoppingCartItemModel properties to ZNodeShoppingCartItem.
            if (model.OmsQuoteId > 0)
                BindShoppingCartItemModel(model, znodeShoppingCartItem, parentSKU);

            PublishProductModel publishProductModel = GetPublishProductModel(model, shoppingCartModel.LocaleId, shoppingCartModel.PublishedCatalogId, shoppingCartModel.OmsOrderId.GetValueOrDefault(), catalogVersionId, configParentSKU);

            BindShippingShoppingCartLineItemProductDetails(znodeShoppingCartItem,
                publishProductModel,
                cartLineItemsPublishProductList.FirstOrDefault(x => x.SKU == model.SKU),
                cartLineItemsPublishProductList,
                parentSKU, shoppingCartModel.UserId.GetValueOrDefault(),
                parentSKUProductName,
                shoppingCartModel.ProfileId.GetValueOrDefault(),
                catalogVersionId,
                shoppingCartModel.OmsOrderId.GetValueOrDefault()
                );

            znodeShoppingCartItem.PromotionalPrice = model.PromotionalPrice;
            znodeShoppingCartItem.OmsSavedCartLineItemId = model.OmsSavedcartLineItemId.GetValueOrDefault();
            znodeShoppingCartItemList.Add(znodeShoppingCartItem);
        }

        //Map shopping cart item model.
        protected virtual ZnodeShoppingCartItem MapShoppingCartItemModel(ShoppingCartItemModel model, AddressModel shippingAddress)
        {
            this.PortalId = IsNotNull(this.PortalId) ? this.PortalId : _znodeShoppingCart.GetHeaderPortalId();

            return new ZnodeShoppingCartItem(shippingAddress)
            {
                ExternalId = model.ExternalId,
                Quantity = model.Quantity,
                ShippingCost = model.ShippingCost,
                ShippingOptionId = model.ShippingOptionId,
                ParentProductId = model.ParentProductId,
                InsufficientQuantity = model.InsufficientQuantity,
                CustomUnitPrice = model.CustomUnitPrice,
                IsActive = model.IsActive,
                OmsOrderId = model.OmsOrderId,
                OmsOrderLineItemId = model.OmsOrderLineItemsId,
                AutoAddonSKUs = model.AutoAddonSKUs,
                Custom1 = model.Custom1,
                Custom2 = model.Custom2,
                Custom3 = model.Custom3,
                Custom4 = model.Custom4,
                Custom5 = model.Custom5,
                ParentProductSKU = model.GroupProducts?.Count > 0 ? model.GroupProducts.FirstOrDefault()?.Sku : model.ParentProductSKU,
                SKU = model.ParentProductId == 0 ? model.SKU : (model.OrderLineItemRelationshipTypeId != (int)ZnodeCartItemRelationshipTypeEnum.Simple ? (model.ConfigurableProductSKUs ?? model.AddOnProductSKUs ?? model.BundleProductSKUs) : model.SKU),
                GroupId = model.GroupId,
                OrderLineItemRelationshipTypeId = model.OrderLineItemRelationshipTypeId,
                OmsSavedCartLineItemId = model.OmsSavedcartLineItemId.GetValueOrDefault(),
                ShipSeperately = model.ShipSeperately,
                Sequence = model.Sequence,
                GroupSequence = model.GroupSequence,
                ParentOmsSavedCartLineItemId = model.ParentOmsSavedcartLineItemId.GetValueOrDefault(),
                AdditionalCost = model.AdditionalCost,
                ShipDate = model.ShipDate
            };
        }

        //Map ShoppingCartItemModel to AccountQuoteLineItemModel.
        protected virtual void BindShoppingCartItemModel(ShoppingCartItemModel model, ZnodeShoppingCartItem znodeCartItem, string parentSKU)
        {
            znodeCartItem.OmsQuoteId = model.OmsQuoteId;
            znodeCartItem.OmsQuoteLineItemId = model.OmsQuoteLineItemId;
            znodeCartItem.ParentOmsQuoteLineItemId = model.ParentOmsQuoteLineItemId;
            znodeCartItem.OrderLineItemRelationshipTypeId = model.OrderLineItemRelationshipTypeId;
            znodeCartItem.CartAddOnDetails = model.CartAddOnDetails;
            znodeCartItem.SKU = parentSKU;
            znodeCartItem.Quantity = model.Quantity;
            znodeCartItem.OmsSavedCartLineItemId = model.OmsSavedcartLineItemId.GetValueOrDefault();
            znodeCartItem.ParentOmsSavedCartLineItemId = model.ParentOmsSavedcartLineItemId.GetValueOrDefault();
        }

        protected virtual PublishProductModel GetPublishProductModel(ShoppingCartItemModel model, int localeId, int publishedCatalogId, int omsOrderId, int catalogVersionId, string configParentSKU = "")
        {
            return new PublishProductModel
            {
                SKU = !string.IsNullOrEmpty(model.ConfigurableProductSKUs) ? model.ConfigurableProductSKUs : model.SKU,
                Quantity = model.Quantity,
                LocaleId = localeId,
                ParentPublishProductId = model.ParentProductId > 0 && (!string.IsNullOrEmpty(model.ConfigurableProductSKUs) || !string.IsNullOrEmpty(model.AutoAddonSKUs)) ? model.ParentProductId : GetParentProductId(!string.IsNullOrEmpty(configParentSKU) ? configParentSKU : model.SKU, publishedCatalogId, localeId, omsOrderId),
                PublishedCatalogId = publishedCatalogId,
                AddonProductSKUs = !string.IsNullOrEmpty(model.AddOnProductSKUs) ? model.AddOnProductSKUs : string.Empty,
                BundleProductSKUs = !string.IsNullOrEmpty(model.BundleProductSKUs) ? model.BundleProductSKUs : string.Empty,
                ConfigurableProductSKUs = !string.IsNullOrEmpty(model.ConfigurableProductSKUs) ? model.ConfigurableProductSKUs : string.Empty,
                GroupProductSKUs = model?.GroupProducts?.Count > 0 ? model.GroupProducts : new List<AssociatedProductModel>(),
                AssociatedAddOnProducts = model.AssociatedAddOnProducts
            };
        }

        //Bind the details cart line item
        protected virtual void BindShippingShoppingCartLineItemProductDetails(ZnodeShoppingCartItem znodeCartItem, PublishProductModel productModel, PublishProductModel cartLineItemsPublishProduct, List<PublishProductModel> cartLineItemsPublishProductList, string parentSKU, int userId, string parentSKUProductName, int profileId, int catalogVersionId, int omsOrderId = 0)
        {
            if (IsNotNull(cartLineItemsPublishProduct) && IsNotNull(znodeCartItem))
            {
                bool isGroupProduct = productModel.GroupProductSKUs.Count > 0;

                cartLineItemsPublishProduct.GroupProductSKUs = productModel.GroupProductSKUs;
                cartLineItemsPublishProduct.ConfigurableProductId = productModel.ParentPublishProductId;
                cartLineItemsPublishProduct.ParentPublishProductId = productModel.ParentPublishProductId;
                productModel.PublishProductId = cartLineItemsPublishProduct.PublishProductId;

                ZnodeProduct baseProduct = GetShippingCartItemProductDetails(cartLineItemsPublishProduct, this.PortalId.GetValueOrDefault(), productModel.LocaleId, userId, profileId, isGroupProduct, parentSKU, parentSKUProductName);

                znodeCartItem.ProductCode = cartLineItemsPublishProduct.Attributes.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.ProductCode)?.AttributeValues;
                znodeCartItem.ProductType = cartLineItemsPublishProduct.Attributes.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.ProductType)?.SelectValues?.FirstOrDefault()?.Code;

                znodeCartItem.Product = new ZnodeProductBase(baseProduct, znodeCartItem.ShippingAddress);

                znodeCartItem.Product.ZNodeAddonsProductCollection = GetZnodeProductAddonsDetailsForShipping(productModel, baseProduct.AddOns, productModel.PublishedCatalogId, productModel.LocaleId, userId, profileId, catalogVersionId, omsOrderId);
                znodeCartItem.Product.ZNodeBundleProductCollection = GetZnodeProductBundlesDetailsForShipping(productModel, productModel.PublishedCatalogId, productModel.LocaleId, userId, profileId, catalogVersionId, omsOrderId);
                znodeCartItem.Product.ZNodeConfigurableProductCollection = GetZnodeProductConfigurablesForShipping(productModel.ConfigurableProductSKUs, productModel.PublishedCatalogId, productModel.LocaleId, userId, profileId, catalogVersionId, productModel.ParentPublishProductId, productModel.Quantity.GetValueOrDefault(), cartLineItemsPublishProductList, omsOrderId);
                znodeCartItem.Product.ZNodeGroupProductCollection = GetZnodeProductGroupForShipping(productModel.GroupProductSKUs, productModel.PublishedCatalogId, productModel.LocaleId, userId, profileId, catalogVersionId, omsOrderId);

                znodeCartItem.Quantity = _znodeShoppingCart.GetProductQuantity(znodeCartItem, productModel.Quantity.GetValueOrDefault());
                znodeCartItem.ParentProductId = productModel.ParentPublishProductId;
                znodeCartItem.UOM = baseProduct.UOM;
                znodeCartItem.ParentProductSKU = znodeCartItem.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Group)
                                       ? znodeCartItem.ParentProductSKU : cartLineItemsPublishProduct.SKU;
                znodeCartItem.Product.SKU = !string.IsNullOrEmpty(parentSKU) && (!string.IsNullOrEmpty(productModel.ConfigurableProductSKUs) || isGroupProduct) ? parentSKU : cartLineItemsPublishProduct.SKU;

                znodeCartItem.Product.Container = _znodeShoppingCart.GetAttributeValueByCode(znodeCartItem, cartLineItemsPublishProduct, ZnodeConstant.ShippingContainer);
                znodeCartItem.Product.Size = _znodeShoppingCart.GetAttributeValueByCode(znodeCartItem, cartLineItemsPublishProduct, ZnodeConstant.ShippingSize);
                znodeCartItem.Product.PackagingType = cartLineItemsPublishProduct.Attributes.Where(x => x.AttributeCode == ZnodeConstant.PackagingType)?.FirstOrDefault()?.SelectValues[0]?.Value;

                znodeCartItem.Product.OrdersDiscount = cartLineItemsPublishProduct.OrdersDiscount;
                znodeCartItem.Product.DiscountAmount = cartLineItemsPublishProduct.DiscountAmount;
                znodeCartItem.Product.ShippingCost = cartLineItemsPublishProduct.ShippingCost.GetValueOrDefault();
                znodeCartItem.Product.SelectedQuantity = znodeCartItem.Quantity;
            }
        }

        //Bind the details to product model of cart line item
        protected virtual ZnodeProduct GetShippingCartItemProductDetails(PublishProductModel publishProduct, int portalId, int localeId, int userId, int profileId, bool isGroupProduct = false, string parentSKU = "", string parentSKUProductName = "", bool isBundleProduct = false)
        {
            if ((IsNull(publishProduct)))
                return null;

            bool isProductHasRetailPrice = true;
            if (IsNull(publishProduct.RetailPrice))
            {
                isProductHasRetailPrice = false;
                _znodeShoppingCart.GetParentProductPriceDetails(publishProduct, portalId, localeId, parentSKU, userId, profileId);
            }

            if (isBundleProduct && !isProductHasRetailPrice)
            {
                publishProduct.SKU = !String.IsNullOrEmpty(publishProduct.ConfigurableProductSKU) ? publishProduct.ConfigurableProductSKU : publishProduct.SKU;
            }

            ZnodeProduct product = new ZnodeProduct
            {
                ProductID = publishProduct.PublishProductId,
                Name = isGroupProduct ? parentSKUProductName : publishProduct.Name,
                SKU = isGroupProduct ? parentSKU : publishProduct.SKU,
                SalePrice = publishProduct.SalesPrice,
                RetailPrice = publishProduct.RetailPrice.GetValueOrDefault(),
                QuantityOnHand = publishProduct.Quantity.GetValueOrDefault(),
                ZNodeTieredPriceCollection = _znodeShoppingCart.GetZnodeProductTierPrice(publishProduct),
                AddOns = publishProduct.AddOns,
                IsActive = publishProduct.IsActive,
            };

            if (publishProduct.Attributes?.Count > 0)
            {
                product.FreeShippingInd = _znodeShoppingCart.GetBooleanProductAttributeValue(publishProduct, ZnodeConstant.FreeShipping);
                product.ShipSeparately = _znodeShoppingCart.GetBooleanProductAttributeValue(publishProduct, ZnodeConstant.ShipSeparately);
                product.MinQty = _znodeShoppingCart.GetDecimalProductAttributeValue(publishProduct, ZnodeConstant.MinimumQuantity);
                product.MaxQty = _znodeShoppingCart.GetDecimalProductAttributeValue(publishProduct, ZnodeConstant.MaximumQuantity);
                product.ShippingRuleTypeCode = publishProduct.Attributes?.Where(x => x.AttributeCode == ZnodeConstant.ShippingCost)?.FirstOrDefault()?.SelectValues?.FirstOrDefault()?.Code;
                product.Height = _znodeShoppingCart.GetDecimalProductAttributeValue(publishProduct, ZnodeConstant.Height);
                product.Width = _znodeShoppingCart.GetDecimalProductAttributeValue(publishProduct, ZnodeConstant.Width);
                product.Length = _znodeShoppingCart.GetDecimalProductAttributeValue(publishProduct, ZnodeConstant.Length);
                product.Weight = _znodeShoppingCart.GetDecimalProductAttributeValue(publishProduct, ZnodeConstant.Weight);
                product.UOM = publishProduct.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.UOM)?.SelectValues?.FirstOrDefault()?.Value;
                product.Container = publishProduct.Attributes.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.ShippingContainer)?.SelectValues[0]?.Value;
                product.Size = publishProduct.Attributes.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.ShippingSize)?.SelectValues[0]?.Code;
                product.PackagingType = publishProduct.Attributes.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.PackagingType)?.SelectValues[0]?.Value;
            }

            //to apply product promotional price
            product.ApplyPromotion();

            _znodeShoppingCart.SetProductAttributes(product, publishProduct);

            return product;
        }

        //To get Addon products collection 
        protected virtual ZnodeGenericCollection<Entities.ZnodeProductBaseEntity> GetZnodeProductAddonsDetailsForShipping(PublishProductModel productModel, List<WebStoreAddOnModel> addOns, int publishedCatalogId, int localeId, int userId, int profileId, int catalogVersionId, int omsOrderId = 0)
        {
            ZnodeGenericCollection<Entities.ZnodeProductBaseEntity> addonsProductCollection = new ZnodeGenericCollection<Entities.ZnodeProductBaseEntity>();
            IPublishedProductDataService _publishProductHelper = GetService<IPublishedProductDataService>();

            if (IsNull(productModel.AssociatedAddOnProducts))
            {
                productModel.AssociatedAddOnProducts = new List<AssociatedProductModel>();
                productModel.AddonProductSKUs?.Split(',').ToList().ForEach(x => productModel.AssociatedAddOnProducts.Add(new AssociatedProductModel { Sku = x, Quantity = Convert.ToDecimal(productModel.Quantity), OrderLineItemRelationshipTypeId = (int)ZnodeCartItemRelationshipTypeEnum.AddOns }));
            }

            if (IsNotNull(productModel.AssociatedAddOnProducts) && productModel.AssociatedAddOnProducts.Count > 0)
            {
                if (IsNull(addOns))
                {
                    addOns = GetService<IPublishProductHelper>().GetAddOnsData(productModel.PublishProductId, productModel.ParentPublishProductId, productModel.PortalId, localeId, catalogVersionId, userId, productModel.ZnodeCatalogId);
                }

                foreach (AssociatedProductModel item in productModel.AssociatedAddOnProducts)
                {
                    foreach (string sku in item.Sku?.Split(','))
                    {
                        ZnodePublishProductEntity product = _publishProductHelper.GetPublishProductBySKU(sku, publishedCatalogId, localeId, catalogVersionId, omsOrderId);
                        if (IsNotNull(product))
                        {
                            ZnodeProduct addonproduct = GetShippingCartItemProductDetails(product?.ToModel<PublishProductModel>(), this.PortalId.GetValueOrDefault(), localeId, userId, profileId);
                            addonproduct.SelectedQuantity = item.Quantity;
                            addonproduct.OrdersDiscount = item.OrdersDiscount;
                            addonsProductCollection.Add(new Entities.ZnodeProductTypeEntity(addonproduct));
                        }
                    }
                }
            }
            return addonsProductCollection;
        }

        //To get bundle product collection
        protected virtual ZnodeGenericCollection<Entities.ZnodeProductBaseEntity> GetZnodeProductBundlesDetailsForShipping(PublishProductModel productModel, int publishedCatalogId, int localeId, int userId, int profileId, int catalogVersionId, int omsOrderId = 0)
        {
            ZnodeGenericCollection<Entities.ZnodeProductBaseEntity> bundleProductCollection = new ZnodeGenericCollection<Entities.ZnodeProductBaseEntity>();

            if (HelperUtility.IsNotNull(productModel) && !string.IsNullOrEmpty(productModel.BundleProductSKUs))
            {
                List<string> bundles = productModel.BundleProductSKUs.Split(',').ToList<string>();

                List<PublishProductModel> productList = _znodeShoppingCart.GetPublishProductBySKUs(bundles, publishedCatalogId, localeId, catalogVersionId)?.ToModel<PublishProductModel>()?.ToList();
                foreach (string item in bundles)
                {
                    PublishProductModel product = productList?.FirstOrDefault(x => x.SKU == item);
                    if (IsNotNull(product))
                    {
                        ZnodeProduct addonproduct = GetShippingCartItemProductDetails(product, this.PortalId.GetValueOrDefault(), localeId, 0, profileId, false, string.Empty, string.Empty, isBundleProduct: true);
                        bundleProductCollection.Add(new Entities.ZnodeProductTypeEntity(addonproduct));
                    }
                }
            }
            return bundleProductCollection;
        }

        //To get configurable product collection 
        protected virtual ZnodeGenericCollection<Entities.ZnodeProductBaseEntity> GetZnodeProductConfigurablesForShipping(string configurableProductSKUs, int publishedCatalogId, int localeId, int userId, int profileId, int catalogVersionId, int parentProductId, decimal productQuantity, List<PublishProductModel> cartLineItemsProductData, int omsOrderId = 0)
        {
            ZnodeGenericCollection<Entities.ZnodeProductBaseEntity> configurableProductCollection = new ZnodeGenericCollection<Entities.ZnodeProductBaseEntity>();
            if (!string.IsNullOrEmpty(configurableProductSKUs))
            {
                List<string> configurable = configurableProductSKUs.Split(',').ToList<string>();
                foreach (string item in configurable)
                {
                    PublishProductModel product = cartLineItemsProductData.FirstOrDefault(x => x.SKU == item);
                    if (IsNotNull(product))
                    {
                        product.ParentPublishProductId = parentProductId;
                        ZnodeProduct configureproduct = GetShippingCartItemProductDetails(product, this.PortalId.GetValueOrDefault(), localeId, userId, 0);
                        configureproduct.SelectedQuantity = productQuantity;
                        configurableProductCollection.Add(new Entities.ZnodeProductTypeEntity(configureproduct));

                    }
                }
            }
            return configurableProductCollection;
        }

        //To get group product collection 
        protected virtual ZnodeGenericCollection<Entities.ZnodeProductBaseEntity> GetZnodeProductGroupForShipping(List<AssociatedProductModel> groupProducts, int publishedCatalogId, int localeId, int userId, int profileId, int catalogVersionId, int omsOrderId = 0)
        {
            ZnodeGenericCollection<Entities.ZnodeProductBaseEntity> groupProductCollection = new ZnodeGenericCollection<Entities.ZnodeProductBaseEntity>();

            foreach (AssociatedProductModel item in groupProducts)
            {
                ZnodePublishProductEntity product = GetService<IPublishedProductDataService>().GetPublishProductBySKU(item.Sku, publishedCatalogId, localeId, catalogVersionId, omsOrderId);
                if (IsNotNull(product))
                {
                    ZnodeProduct groupProduct = GetShippingCartItemProductDetails(product?.ToModel<PublishProductModel>(), this.PortalId.GetValueOrDefault(), localeId, userId, profileId);
                    groupProduct.SelectedQuantity = item.Quantity;
                    groupProduct.OmsSavedCartLineItemId = item.OmsSavedCartLineItemId;
                    groupProduct.OrdersDiscount = item.OrdersDiscount;
                    groupProductCollection.Add(new Entities.ZnodeProductTypeEntity(groupProduct));
                }
            }
            return groupProductCollection;
        }

        protected virtual int GetParentProductId(string sku, int publishedCatalogId, int localeId, int catalogVersionId, int omsOrderId = 0)
        {
            return (GetService<IPublishedProductDataService>().GetPublishProductBySKU(sku, publishedCatalogId, localeId, catalogVersionId, omsOrderId)?.ZnodeProductId).GetValueOrDefault();
        }
        #endregion

    }
}