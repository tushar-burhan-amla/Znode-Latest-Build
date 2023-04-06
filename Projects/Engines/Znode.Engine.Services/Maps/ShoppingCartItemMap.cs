using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;


namespace Znode.Engine.Services.Maps
{
    //This mapper code needs to Refactor.
    //It will get refactored as it is getting used.
    //To Do.
    public class ShoppingCartItemMap : IShoppingCartItemMap
    {
        public ShoppingCartItemMap()
        {
        }

        public virtual ShoppingCartItemModel ToModel(Znode.Libraries.ECommerce.ShoppingCart.ZnodeShoppingCartItem znodeCartItem, ZnodeShoppingCart znodeCart, IImageHelper objImage = null)
        {
            if (HelperUtility.IsNotNull(znodeCartItem))
            {
                ShoppingCartItemModel cartItem = new ShoppingCartItemModel
                {
                    Description = string.IsNullOrEmpty(znodeCartItem.Description) || string.IsNullOrWhiteSpace(znodeCartItem.Description) ? znodeCartItem.PromoDescription : znodeCartItem.Description,
                    ExtendedPrice = znodeCartItem.ExtendedPrice,
                    Quantity = znodeCartItem.Quantity,
                    ExternalId = znodeCartItem.ExternalId,
                    ParentProductId = znodeCartItem.ParentProductId,
                    ShippingCost = znodeCartItem.CustomShippingCost > 0 ? znodeCartItem.CustomShippingCost : znodeCartItem.ShippingCost,
                    UnitPrice = znodeCartItem.UnitPrice,
                    CustomUnitPrice = znodeCartItem.CustomUnitPrice,
                    PartialRefundAmount = znodeCartItem.PartialRefundAmount,
                    TaxCost = znodeCartItem.TaxCost,
                    ProductCode = znodeCartItem.ProductCode,
                    ProductType = znodeCartItem.ProductType,
                    ImagePath = GetImagePath(znodeCartItem.Image, znodeCart.PortalId.GetValueOrDefault(), objImage),
                    QuantityOnHand = znodeCartItem.Product.QuantityOnHand,
                    MaxQuantity = znodeCartItem.Product.MaxQty,
                    SeoPageName = znodeCartItem.Product.SEOURL,
                    MinQuantity = znodeCartItem.Product.MinQty,
                    TrackInventory = znodeCartItem.Product.TrackInventoryInd,
                    AllowBackOrder = znodeCartItem.Product.AllowBackOrder,
                    InsufficientQuantity = CheckProductPrice(znodeCartItem.InsufficientQuantity, znodeCartItem.Product),
                    PersonaliseValuesDetail = znodeCartItem.PersonaliseValuesDetail,
                    PersonaliseValuesList = znodeCartItem.PersonaliseValuesList,
                    UOM = znodeCartItem.UOM,
                    IsEditStatus = znodeCartItem.IsEditStatus,
                    IsActive = znodeCartItem.IsActive,
                    IsItemStateChanged = znodeCartItem.IsItemStateChanged,
                    IsSendEmail = znodeCartItem.IsSendEmail,
                    OmsOrderStatusId = znodeCartItem.OrderStatusId,
                    OrderLineItemStatus = znodeCartItem.OrderStatus,
                    OmsOrderId = znodeCartItem.OmsOrderId,
                    OmsOrderLineItemsId = znodeCartItem.OmsOrderLineItemId,
                    TrackingNumber = znodeCartItem.TrackingNumber,
                    IsAllowedTerritories = znodeCartItem.IsAllowedTerritories,
                    AutoAddonSKUs = znodeCartItem.AutoAddonSKUs,
                    OmsSavedcartLineItemId = znodeCartItem.OmsSavedCartLineItemId,
                    Custom1 = znodeCartItem.Custom1,
                    Custom2 = znodeCartItem.Custom2,
                    Custom3 = znodeCartItem.Custom3,
                    Custom4 = znodeCartItem.Custom4,
                    Custom5 = znodeCartItem.Custom5,
                    GroupId = znodeCartItem.GroupId,
                    ShipSeperately = znodeCartItem.ShipSeperately,
                    Sequence = znodeCartItem.Sequence,
                    GroupSequence = znodeCartItem.GroupSequence,
                    OrderLineItemRelationshipTypeId = znodeCartItem.OrderLineItemRelationshipTypeId,
                    ParentOmsSavedcartLineItemId = znodeCartItem.ParentOmsSavedCartLineItemId,
                    CustomText = znodeCartItem.CustomText,
                    AdditionalCost = znodeCartItem.AdditionalCost,
                    AssociatedAddOnProducts = znodeCartItem.AssociatedAddOnProducts,
                    CreatedBy = znodeCartItem.CreatedBy,
                    CreatedDate = znodeCartItem.CreatedDate,
                    ModifiedBy = znodeCartItem.ModifiedBy,
                    ModifiedDate = znodeCartItem.ModifiedDate,
                    PerQuantityLineItemDiscount = znodeCartItem.PerQuantityLineItemDiscount,
                    ParentOmsOrderLineItemsId = znodeCartItem.ParentOmsOrderLineItemsId,
                    PerQuantityCSRDiscount = znodeCartItem.PerQuantityCSRDiscount,
                    PerQuantityShippingCost = znodeCartItem.PerQuantityShippingCost,
                    PerQuantityOrderLevelDiscountOnLineItem = znodeCartItem.PerQuantityOrderLevelDiscountOnLineItem,
                    PerQuantityShippingDiscount = znodeCartItem.PerQuantityShippingDiscount,
                    ProductLevelTax = znodeCartItem.ProductLevelTax,
                    PaymentStatusId = znodeCartItem.PaymentStatusId,
                    PromotionalPrice = znodeCartItem.PromotionalPrice,
                    ShipDate = znodeCartItem.ShipDate,
                    PerQuantityVoucherAmount = znodeCartItem.PerQuantityVoucherAmount,
                    TaxRuleId = znodeCartItem.TaxRuleId,
                    InitialPrice = (znodeCartItem.InitialPrice == 0) ? znodeCartItem.UnitPrice : znodeCartItem.InitialPrice,
                    IsPriceEdit = znodeCartItem.IsPriceEdit,
                    InitialShippingCost = znodeCartItem.InitialShippingCost,
                    CustomShippingCost = znodeCartItem.CustomShippingCost,
                    ImportDuty = znodeCartItem.Product.ImportDuty
                };
                BindProductDetails(cartItem, znodeCartItem.Product);

                if (znodeCartItem.OmsQuoteId > 0)
                {
                    //Map ZNodeShoppingCartItem to ShoppingCartItemModel.
                    ToShoppingCartItemModel(cartItem, znodeCartItem);
                }

                return cartItem;
            }
            return new ShoppingCartItemModel();
        }

        public virtual string GetImagePath(string imageName, int portalId, IImageHelper objImage = null)
        {
            IImageHelper image = HelperUtility.IsNull(objImage) ? GetService<IImageHelper>(): objImage;
            return image.GetImageHttpPathThumbnail(imageName);
        }
        //to get product type comma separated skus
        public virtual string GetProductTypeSKUs(ZnodeProductBaseEntity product, ZnodeCartItemRelationshipTypeEnum productType)
        {
            return string.Join(",", GetAssociateProducts(product, productType)?.Select(x => x.Sku));
        }

        //to get associated product list from product entity
        public virtual List<AssociatedProductModel> GetAssociateProducts(ZnodeProductBaseEntity product, ZnodeCartItemRelationshipTypeEnum productType)
        {
            List<AssociatedProductModel> products = new List<AssociatedProductModel>();
            switch (productType)
            {
                case ZnodeCartItemRelationshipTypeEnum.AddOns:
                    foreach (ZnodeProductBaseEntity item in product.ZNodeAddonsProductCollection)
                    {
                        products.Add(new AssociatedProductModel
                        {
                            Sku = item.SKU,
                            ProductId = item.ProductID,
                            Quantity = item.SelectedQuantity,
                            OrdersDiscount = item.OrdersDiscount
                        });
                    }

                    return products;
                case ZnodeCartItemRelationshipTypeEnum.Bundles:
                    foreach (ZnodeProductBaseEntity item in product.ZNodeBundleProductCollection)
                    {
                        products.Add(new AssociatedProductModel
                        {
                            Sku = item.SKU,
                            ProductId = item.ProductID
                        });
                    }

                    return products;
                case ZnodeCartItemRelationshipTypeEnum.Configurable:
                    foreach (ZnodeProductBaseEntity item in product.ZNodeConfigurableProductCollection)
                    {
                        products.Add(new AssociatedProductModel
                        {
                            Sku = item.SKU,
                            ProductId = item.ProductID
                        });
                    }

                    return products;
                case ZnodeCartItemRelationshipTypeEnum.Group:
                    foreach (ZnodeProductBaseEntity item in product.ZNodeGroupProductCollection)
                    {
                        products.Add(new AssociatedProductModel
                        {
                            Sku = item.SKU,
                            ProductId = item.ProductID,
                            Quantity = item.SelectedQuantity,
                            ProductName = item.Name,
                            UnitPrice = GetUnitPriceForGroupProduct(item),
                            MinimumQuantity = item.MinQty,
                            MaximumQuantity = item.MaxQty,
                            OrdersDiscount = item.OrdersDiscount
                        });
                    }

                    return products;
                default:
                    return products;
            }
        }

        //Get tier price.
        public virtual decimal GetUnitPriceForGroupProduct(ZnodeProductBaseEntity item)
        {
            decimal finalPrice = 0.00M;
            foreach (ZnodeProductTierEntity productTieredPrice in item.ZNodeTieredPriceCollection)
            {
                //check if tier quantity is valid or not.
                if (item.SelectedQuantity >= productTieredPrice.MinQuantity && item.SelectedQuantity < productTieredPrice.MaxQuantity)
                {
                    finalPrice = productTieredPrice.Price;
                    break;
                }
            }

            finalPrice = (finalPrice > 0 ? finalPrice : HelperUtility.IsNotNull(item.SalePrice) ? item.SalePrice : item.RetailPrice).GetValueOrDefault();

            return finalPrice;
        }
        //to get group product list from product entity
        public virtual List<AssociatedProductModel> GetUnitPriceForGroupProduct(ZnodeProductBaseEntity product, ZnodeCartItemRelationshipTypeEnum productType)
        {
            return GetAssociateProducts(product, productType);
        }

        //to get group product list from product entity
        public virtual List<AssociatedProductModel> GetGroupProducts(ZnodeProductBaseEntity product, ZnodeCartItemRelationshipTypeEnum productType)
        {
            return GetAssociateProducts(product, productType);
        }

        //Bind the details of product of cartitem.
        public virtual void BindProductDetails(ShoppingCartItemModel cartItem, ZnodeProductBaseEntity znodeProduct)
        {
            if (HelperUtility.IsNotNull(znodeProduct))
            {
                cartItem.ProductId = znodeProduct.ProductID;
                cartItem.SKU = znodeProduct.SKU;
                cartItem.IsActive = znodeProduct.IsActive;
                cartItem.QuantityOnHand = znodeProduct.QuantityOnHand;

                if (string.IsNullOrEmpty(znodeProduct.ShoppingCartDescription) || string.IsNullOrWhiteSpace(znodeProduct.ShoppingCartDescription))
                {
                    if (string.IsNullOrEmpty(cartItem.Description) || string.IsNullOrWhiteSpace(cartItem.Description))
                    {
                        cartItem.CartDescription = znodeProduct.Description;
                    }
                    else
                    {
                        cartItem.CartDescription = cartItem.Description;
                    }
                }
                else
                {
                    cartItem.CartDescription = znodeProduct.ShoppingCartDescription;
                }

                cartItem.ImageMediumPath = znodeProduct.ImageFile;
                cartItem.ProductName = znodeProduct.Name;
                cartItem.ProductDiscountAmount = znodeProduct.DiscountAmount;
                cartItem.SeoPageName = znodeProduct.SEOURL;
                cartItem.DownloadableProductKey = znodeProduct.DownloadableProductKey;
                cartItem.AddOnProductSKUs = GetProductTypeSKUs(znodeProduct, ZnodeCartItemRelationshipTypeEnum.AddOns);
                cartItem.BundleProductSKUs = GetProductTypeSKUs(znodeProduct, ZnodeCartItemRelationshipTypeEnum.Bundles);
                cartItem.ConfigurableProductSKUs = GetProductTypeSKUs(znodeProduct, ZnodeCartItemRelationshipTypeEnum.Configurable);
                cartItem.GroupProducts = GetGroupProducts(znodeProduct, ZnodeCartItemRelationshipTypeEnum.Group);
                cartItem.AssociatedAddOnProducts = GetAssociateProducts(znodeProduct, ZnodeCartItemRelationshipTypeEnum.AddOns);
                SetProductAttributes(cartItem, znodeProduct.Attributes);
                if (HelperUtility.IsNull(cartItem.Product))
                    cartItem.Product = new PublishProductModel() { PublishProductId = znodeProduct.ProductID };                    

                if(cartItem.Product.PublishProductId == znodeProduct.ProductID)
                {
                    cartItem.Product.SKU = znodeProduct.SKU;
                    cartItem.Product.OrdersDiscount = znodeProduct.OrdersDiscount;
                    cartItem.Product.HST = znodeProduct.HST;
                    cartItem.Product.PST = znodeProduct.PST;
                    cartItem.Product.GST = znodeProduct.GST;
                    cartItem.Product.VAT = znodeProduct.VAT;
                    cartItem.Product.SalesTax = znodeProduct.SalesTax;
                    cartItem.Product.ImportDuty = znodeProduct.ImportDuty;
                    cartItem.Product.ShippingCost = znodeProduct.ShippingCost;
                    cartItem.Product.DiscountAmount = znodeProduct.DiscountAmount;
                }                                
            }
        }

        //Map ZNodeShoppingCartItem to ShoppingCartItemModel.
        public virtual void ToShoppingCartItemModel(ShoppingCartItemModel cartItem, ZnodeShoppingCartItem znodeCartItem)
        {
            cartItem.OmsQuoteId = znodeCartItem.OmsQuoteId;
            cartItem.OmsQuoteLineItemId = znodeCartItem.OmsQuoteLineItemId;
            cartItem.ParentOmsQuoteLineItemId = znodeCartItem.ParentOmsQuoteLineItemId;
            cartItem.OrderLineItemRelationshipTypeId = znodeCartItem.OrderLineItemRelationshipTypeId;
            cartItem.CustomText = znodeCartItem.CustomText;
            cartItem.CartAddOnDetails = znodeCartItem.CartAddOnDetails;
            cartItem.Quantity = znodeCartItem.Quantity;
            cartItem.OmsSavedcartLineItemId = znodeCartItem.OmsSavedCartLineItemId;
            cartItem.SKU = znodeCartItem.Product.SKU;
            cartItem.ParentOmsSavedcartLineItemId = znodeCartItem.ParentOmsSavedCartLineItemId;
        }

        //to check product price greater than zero
        public virtual bool CheckProductPrice(bool insufficientQuantity, ZnodeProductBaseEntity product)
        {
            if (!insufficientQuantity && !Equals(product, null))
            {
                if (product?.ZNodeAddonsProductCollection?.Count > 0)
                {
                    if (!product.IsPriceExist)
                    {
                        insufficientQuantity = true;
                    }
                    foreach (ZnodeProductBaseEntity addon in product.ZNodeAddonsProductCollection)
                    {
                        if (!addon.IsPriceExist)
                        {
                            insufficientQuantity = true;
                        }
                    }
                }
                else if (product?.ZNodeGroupProductCollection?.Count > 0)
                {
                    foreach (ZnodeProductBaseEntity group in product.ZNodeGroupProductCollection)
                    {
                        if (!group.IsPriceExist)
                        {
                            insufficientQuantity = true;
                        }
                    }
                }
                else if (product?.ZNodeConfigurableProductCollection?.Count > 0)
                {
                    if (!product.IsPriceExist)
                    {
                        foreach (ZnodeProductBaseEntity config in product.ZNodeConfigurableProductCollection)
                        {
                            if (!config.IsPriceExist)
                            {
                                insufficientQuantity = true;
                            }
                        }
                    }
                }
                else if (HelperUtility.IsNotNull(product) && !product.IsPriceExist)
                {
                    insufficientQuantity = true;
                }
            }
            return insufficientQuantity;
        }

        public virtual void SetProductAttributes(ShoppingCartItemModel cartItem, List<OrderAttributeModel> orderAttributeModel)
        {
            cartItem.ProductAttributes = new List<PublishAttributeModel>();

            //Get default Cart Attributes to be displayed on the Cart Page.
            string cartAttribute = DefaultGlobalConfigSettingHelper.DefaultCartAttribute;
            List<string> lstCartAttribute = !string.IsNullOrEmpty(cartAttribute) ? cartAttribute.Split(',').ToList() : null;

            //Bind cart attributes.
            if (HelperUtility.IsNotNull(lstCartAttribute) && lstCartAttribute.Count > 0)
            {
                cartItem.ProductAttributes = (from item in orderAttributeModel
                                              where lstCartAttribute.Any(s => s.Equals(item.AttributeCode, StringComparison.OrdinalIgnoreCase))
                                              select new PublishAttributeModel
                                              {
                                                  AttributeCode = item.AttributeCode,
                                                  AttributeValues = item.AttributeValue,
                                                  AttributeValueCode = item.AttributeValueCode
                                              }).ToList();
            }
        }
    }
}
