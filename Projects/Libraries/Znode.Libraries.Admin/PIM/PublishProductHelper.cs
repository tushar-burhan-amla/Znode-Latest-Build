using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Engine.Promotions;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Observer;
using System.Collections.Concurrent;
using System.Configuration;
using System.Diagnostics;

namespace Znode.Libraries.Admin
{
    public class PublishProductHelper : ZnodeBusinessBase, IPublishProductHelper
    {
        #region Private Variables
        private readonly ZnodeRepository<ZnodeTaxClassSKU> _znodeTaxClassSKU;
        private readonly ZnodeRepository<ZnodeTaxClass> _znodeTaxClass;
        private readonly ZnodeRepository<ZnodeTaxRule> _znodeTaxRule;
        private readonly ZnodeRepository<ZnodeTaxRuleType> _znodeTaxRuleType;
        private readonly IZnodeRepository<ZnodeLocale> _znodeLocaleRepository;
        private readonly string _AdditionalAttributes = "AdditionalAttributes";
        private readonly IZnodeRepository<ZnodePimAddOnProduct> _pimAddOnProductRepository;
        private readonly IZnodeRepository<ZnodePimAddonGroupLocale> _pimAddonGroupLocaleRepository;

        private readonly IZnodeRepository<ZnodePublishConfigurableProductEntity> _publishConfigurableProductEntity;

        #endregion

        #region Constructor
        public PublishProductHelper()
        {
            _znodeTaxClassSKU = new ZnodeRepository<ZnodeTaxClassSKU>();
            _znodeTaxClass = new ZnodeRepository<ZnodeTaxClass>();
            _znodeTaxRule = new ZnodeRepository<ZnodeTaxRule>();
            _znodeTaxRuleType = new ZnodeRepository<ZnodeTaxRuleType>();
            _znodeLocaleRepository = new ZnodeRepository<ZnodeLocale>();
            _pimAddOnProductRepository = new ZnodeRepository<ZnodePimAddOnProduct>();
            _pimAddonGroupLocaleRepository = new ZnodeRepository<ZnodePimAddonGroupLocale>();
            _publishConfigurableProductEntity = new ZnodeRepository<ZnodePublishConfigurableProductEntity>(HelperMethods.Context);
        }
        #endregion

        #region Public Methods




        //ToDo-
        //Get Pricing Associated to SKU.
        public virtual List<PriceSKUModel> GetPricingBySKUs(IEnumerable<string> skus, int portalId, int userId = 0, int profileId = 0, int omsOrderId = 0, bool isBundleProduct = false, string bundleProductParentSKU = "")
        {
            IZnodeViewRepository<PriceSKUModel> skuPrice = new ZnodeViewRepository<PriceSKUModel>();
            skuPrice.SetParameter("@SKU", string.Join(",", skus), ParameterDirection.Input, DbType.String);
            skuPrice.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            skuPrice.SetParameter("@currentUtcDate", HelperUtility.GetDateTime().Date, ParameterDirection.Input, DbType.DateTime);
            skuPrice.SetParameter("@UserId", userId, ParameterDirection.Input, DbType.Int32);
            skuPrice.SetParameter("@ProfileId", profileId, ParameterDirection.Input, DbType.Int32);
            List<PriceSKUModel> model = null;

            if (isBundleProduct)
            {
                skuPrice.SetParameter("@IsBundleProduct", isBundleProduct, ParameterDirection.Input, DbType.Boolean);
                skuPrice.SetParameter("@ParentSKU", bundleProductParentSKU, ParameterDirection.Input, DbType.String);
                model = skuPrice.ExecuteStoredProcedureList("Znode_GetPublishProductPricingBySkuBundle @SKU,@PortalId,@currentUtcDate,@UserId,@ProfileId,@IsBundleProduct,@ParentSKU")?.ToList();
            }
            else
            {
                skuPrice.SetParameter("@OmsOrderId", omsOrderId, ParameterDirection.Input, DbType.Int32);
                skuPrice.SetParameter("@IsFetchPriceFromOrder", true, ParameterDirection.Input, DbType.Boolean);
                model = skuPrice.ExecuteStoredProcedureList("Znode_GetPublishProductPricingBySku @SKU,@PortalId,@currentUtcDate,@UserId,@ProfileId,@OmsOrderId,@IsFetchPriceFromOrder")?.ToList();
            }
            if (model?.Count == 1)
            {
                ERPInitializer<PriceSKUModel> _erpInc = new ERPInitializer<PriceSKUModel>(new PriceSKUModel() { SKU = model.FirstOrDefault().SKU }, "PricingRealTime");
                List<PriceSKUModel> pricingModel = (List<PriceSKUModel>)_erpInc.Result;
                if (HelperUtility.IsNotNull(pricingModel))
                {
                    model.FirstOrDefault().RetailPrice = pricingModel.FirstOrDefault().RetailPrice;
                    model.FirstOrDefault().SalesPrice = pricingModel.FirstOrDefault().SalesPrice;
                }
            }

            ModifySKUPriceListDetails(model);
            return model;
        }        //Get Inventory Associated to SKU.
        public virtual List<InventorySKUModel> GetInventoryBySKUs(IEnumerable<string> skus, int portalId)
        {
            //Znode code to check ZnodeInventory table for available inventories.
            IZnodeViewRepository<InventorySKUModel> skuInventory = new ZnodeViewRepository<InventorySKUModel>();
            skuInventory.SetParameter("@SKUs", string.Join(",", skus), ParameterDirection.Input, DbType.String);
            skuInventory.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            List<InventorySKUModel> model = skuInventory.ExecuteStoredProcedureList("Znode_GetInventoryBySkus @SKUs,@PortalId")?.ToList();
            if (model?.Count() == 1)
            {
                ERPInitializer<InventorySKUModel> _erpInc = new ERPInitializer<InventorySKUModel>(new InventorySKUModel() { SKU = model.FirstOrDefault()?.SKU }, "Inventory");
                List<InventorySKUModel> inventoryModel = (List<InventorySKUModel>)_erpInc.Result;
                if (HelperUtility.IsNotNull(inventoryModel))
                    model.FirstOrDefault().Quantity = inventoryModel.FirstOrDefault().Quantity;
            }
            //Inventory realtime call
            return RealTimeProductInventoryCall(model, skus, portalId);
        }


        //Get Promotions Associated to Publish Product Id.
        public virtual List<ProductPromotionModel> GetPromotionByPublishProductIds(IEnumerable<int> publishProductIds, int userId = 0, int portalId = 0)
        {
            IZnodeViewRepository<ProductPromotionModel> skuPromotion = new ZnodeViewRepository<ProductPromotionModel>();
            skuPromotion.SetParameter("@PublishProductIds", string.Join(",", publishProductIds), ParameterDirection.Input, DbType.String);
            skuPromotion.SetParameter("@UserId", userId, ParameterDirection.Input, DbType.Int32);
            skuPromotion.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            return skuPromotion.ExecuteStoredProcedureList("Znode_GetPromotionByPublishProductId @PublishProductIds,@UserId,@PortalId")?.ToList();
        }

        //Maps Price to Products.
        public virtual void MapPrice(PublishProductListModel publishProductListModel, List<PriceSKUModel> priceSKU)
        {
            if (priceSKU?.Count > 0)
            {
                publishProductListModel.PublishProducts.ForEach(product =>
                {
                    PriceSKUModel productSKU = priceSKU
                                .FirstOrDefault(productdata => productdata.SKU == product.SKU);

                    if (HelperUtility.IsNotNull(productSKU))
                    {
                        product.SalesPrice = productSKU.SalesPrice;
                        product.RetailPrice = productSKU.RetailPrice;
                        product.CultureCode = productSKU.CultureCode;
                        product.CurrencyCode = productSKU.CurrencyCode;
                        product.CurrencySuffix = productSKU.CurrencySuffix;
                    }

                    List<PriceSKUModel> productPriceSKUs = priceSKU.Where(x => string.Equals(x.SKU, product.SKU, StringComparison.InvariantCultureIgnoreCase)).ToList();

                    //Get tier price.
                    GetTierPriceData(product, productPriceSKUs);
                    //Get Promotional price.
                    GetPromotionalPrice(product);
                });
            }
        }

        //Maps Price to Products.
        public virtual void MapPrice(List<SearchProductModel> productList, List<PriceSKUModel> priceSKU)
        {
            if (priceSKU?.Count > 0)
            {
                productList.ForEach(product =>
                {
                    PriceSKUModel productSKU = priceSKU
                                .Where(productdata => productdata.SKU == product.SKU)
                                ?.FirstOrDefault();
                    if (HelperUtility.IsNotNull(productSKU))
                    {
                        product.SalesPrice = productSKU.SalesPrice;
                        product.RetailPrice = productSKU.RetailPrice;
                        product.CultureCode = productSKU.CultureCode;
                        product.CurrencyCode = productSKU.CurrencyCode;
                        product.CurrencySuffix = productSKU.CurrencySuffix;
                    }
                    GetPromotionalPrice(product);
                });
            }
        }

        //Maps inventory to Products.
        public virtual void MapInventory(PublishProductListModel publishProductListModel, List<InventorySKUModel> inventory)
        {
            if (inventory?.Count > 0)
            {
                publishProductListModel.PublishProducts.ForEach(product =>
                {
                    InventorySKUModel productinventory = inventory
                                .FirstOrDefault(productdata => productdata.SKU == product.SKU);
                    if (HelperUtility.IsNotNull(productinventory))
                    {
                        product.Quantity = productinventory.Quantity;
                        product.ReOrderLevel = productinventory.ReOrderLevel;
                    }
                });
            }
        }

        //Maps inventory to Products.
        public virtual void MapInventory(List<SearchProductModel> productList, List<InventorySKUModel> inventory)
        {
            if (inventory?.Count > 0)
            {
                productList.ForEach(product =>
                {
                    InventorySKUModel productinventory = inventory
                                .FirstOrDefault(productdata => productdata.SKU == product.SKU);
                    if (HelperUtility.IsNotNull(productinventory))
                    {
                        product.Quantity = productinventory.Quantity;
                        product.ReOrderLevel = productinventory.ReOrderLevel;
                    }
                });
            }
        }

        //Get product seo and product reviews
        public virtual void GetProductsSEOAndReviews(int portalId, PublishProductListModel publishProductListModel, int localeId, int? catalogVersionId = null)
        {
            List<string> SKUs = publishProductListModel?.PublishProducts?.Select(y => y.SKU).Distinct().ToList();
            List<ZnodePublishSeoEntity> publishSEOList = GetPublishSEODetailsForList(portalId, ZnodeConstant.Product, localeId, SKUs, catalogVersionId);
            MapProductData(publishProductListModel?.PublishProducts, publishSEOList);
        }

        //Get products addon
        public virtual List<PublishProductModel> GetProductsAddOn(List<PublishProductModel> publishProducts, int portalId, int userId, int localeId, int? catalogVersionId = null)
        {
            publishProducts?.ForEach(publishProduct =>
            {
                publishProduct.AddOns = GetAddOnsData(publishProduct.PublishProductId, publishProduct.ConfigurableProductId, portalId, localeId, catalogVersionId, userId, publishProduct.ZnodeCatalogId);
            });
            return publishProducts;
        }

        //Get product seo
        public virtual void GetProductsSEODetails(int portalId, PublishProductListModel publishProductListModel, int localeId, int? catalogVersionId = null)
        {
            List<string> SKUs = publishProductListModel?.PublishProducts?.Select(y => y.SKU).Distinct().ToList();
            List<SEODetailsModel> znodeCMSSEODetails = GetSEODetailsForList(portalId, ZnodeConstant.Product);
            List<ZnodePublishSeoEntity> publishSEOList = GetPublishSEODetailsForList(portalId, ZnodeConstant.Product, localeId, SKUs, catalogVersionId);
            MapSEOProductData(publishProductListModel?.PublishProducts, publishSEOList, znodeCMSSEODetails);
        }

        //Get product seo and product reviews
        public virtual void GetProductsSEOAndReviews(int portalId, List<SearchProductModel> productList, int localeId, int catalogVersionId)
        {
            List<string> SKUs = productList?.Select(y => y.SKU).Distinct().ToList();
            List<ZnodePublishSeoEntity> publishSEOList = GetPublishSEODetailsForList(portalId, ZnodeConstant.Product, localeId, SKUs, catalogVersionId);
            //Map Product Data.
            MapSearchProductData(productList, publishSEOList);
        }

        //get expands associated to Product
        public virtual void GetDataFromExpands(int portalId, List<string> navigationProperties, PublishProductModel publishProduct, int localeId, string whereClause = "", int userId = 0, int? catalogVersionId = null, int? webstoreVersionId = null, int profileId = 0, int omsOrderId = 0)
        {
            if (publishProduct?.PublishProductId > 0 && (navigationProperties?.Count > 0))
            {
                foreach (string key in navigationProperties)
                {
                    switch (key.ToLower())
                    {
                        case ZnodeConstant.Promotions:
                            //get Promotions Associated to Publish Product Id
                            GetProductPromotions(publishProduct, userId, portalId);
                            break;
                        case ZnodeConstant.Inventory:
                            //get Warehouse Address Associated to SKU
                            if (portalId > 0)
                                GetProductInventory(publishProduct, portalId);
                            break;
                        case ZnodeConstant.Pricing:
                            //get pricing associated to sku
                            if (portalId > 0)
                                GetProductPriceData(publishProduct, portalId, userId, profileId, omsOrderId);
                            break;
                        case ZnodeConstant.ProductTemplate:
                            //Get Template associate to product detail page.
                            if (portalId > 0)
                                GetProductPageTemplate(publishProduct, portalId, webstoreVersionId);
                            break;                      
                        case ZnodeConstant.AddOns:
                            //Get Add Ons for product.
                            if (portalId > 0)
                                publishProduct.AddOns = GetAddOnsData(publishProduct.PublishProductId, publishProduct.ConfigurableProductId, portalId, localeId, catalogVersionId, userId, publishProduct.ZnodeCatalogId, omsOrderId: omsOrderId);
                            break;
                        case ZnodeConstant.SEO:
                            //Get Add Ons for product.
                            if (portalId > 0)
                                GetProductsSEO(publishProduct, portalId, localeId, whereClause, catalogVersionId);
                            break;
                        case ZnodeConstant.ProductBrand:
                            //Get brand data for product.
                            if (portalId > 0)
                                GetBrandDataForProduct(publishProduct, portalId, webstoreVersionId);
                            break;
                        case ZnodeConstant.ProductReviews:
                            //Get Product customer reviews.
                            GetProductCustomerReviews(publishProduct, portalId);
                            break;
                        case ZnodeConstant.AssociatedProducts:
                            //Get the associated products                                                        
                            GetAssociatedProducts(publishProduct, portalId, userId, navigationProperties.Contains(ZnodeConstant.Pricing), profileId);
                            break;                        
                        default:
                            break;
                    }
                }
            }
        }

        //Get Product price data by skus.
        public virtual void GetProductPriceData(PublishProductModel publishProduct, int portalId, int userId, int profileId, int omsOrderId = 0)
        {
            string parentSKU = string.Empty;

            if(HelperUtility.IsNotNull(publishProduct))
            {
                string associateProductSKu = publishProduct.GroupProductSKUs?.Count > 0 ? publishProduct.GroupProductSKUs.FirstOrDefault().Sku : publishProduct.ConfigurableProductSKU;

                List<string> skuList = new List<string> { publishProduct.SKU, associateProductSKu };
                List<PriceSKUModel> priceSKU = GetPricingBySKUs(skuList, portalId, userId, profileId, omsOrderId);

                if (priceSKU?.Count > 0)
            {
                PriceSKUModel priceData = null;

                if (!string.IsNullOrEmpty(publishProduct.ConfigurableProductSKU))
                    parentSKU = publishProduct.ConfigurableProductSKU;
                else
                    parentSKU = publishProduct.SKU;

                PriceSKUModel parentProductPrice = priceSKU.FirstOrDefault(x => x.SKU.Equals(parentSKU, StringComparison.InvariantCultureIgnoreCase));
                if (HelperUtility.IsNotNull(parentProductPrice))
                {
                    priceData = parentProductPrice;
                    priceSKU.RemoveAll(x => !(x.SKU.Equals(parentSKU, StringComparison.InvariantCultureIgnoreCase)));
                }
                else
                    priceData = priceSKU.FirstOrDefault();

                publishProduct.SalesPrice = priceData.SalesPrice;
                publishProduct.RetailPrice = priceData.RetailPrice;
                publishProduct.CurrencyCode = priceData.CurrencyCode;
                publishProduct.CurrencySuffix = priceData.CurrencySuffix;
                publishProduct.CultureCode = priceData.CultureCode;
                    //Get tier price.
                    GetTierPriceData(publishProduct, priceSKU);

                GetPromotionalPrice(publishProduct);
            }
        }
        }

        //Get product customer reviews.
        public virtual void GetProductCustomerReviews(PublishProductModel publishProduct, int portalId)
        {
            //Get Promotions Associated to Publish Product Id
            publishProduct.ProductReviews = GetProductReviews(portalId, publishProduct.SKU);

            //Get Product average rating from total product reviews.
            if (publishProduct?.ProductReviews?.Count > 0)
                publishProduct.Rating = Math.Round((decimal)publishProduct.ProductReviews.Sum(x => x.Rating) / publishProduct.ProductReviews.Count, 2);

        }

        //Get Template associate to product detail page.
        public virtual void GetProductPageTemplate(PublishProductModel publishProduct, int portalId, int? webstoreVersionId = null)
        {
            string cacheKey = $"ProductPageEntity_{portalId}_{webstoreVersionId}";
            List<ZnodePublishProductPageEntity> pageEntities = Equals(HttpRuntime.Cache[cacheKey], null)
               ? GetProductPageTemplateAndInsertIntoDB(portalId, webstoreVersionId, cacheKey)
               : ((List<ZnodePublishProductPageEntity>)HttpRuntime.Cache.Get(cacheKey));
            string productType = publishProduct.IsConfigurableProduct ? ZnodeConstant.ConfigurableProduct : publishProduct.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.ProductType)?.SelectValues?.FirstOrDefault()?.Code;
            publishProduct.ProductTemplateName = pageEntities.FirstOrDefault(x => x.ProductType == productType && x.PortalId == portalId)?.TemplateName;
        }

        //Get Product Template and store into cache.
        private List<ZnodePublishProductPageEntity> GetProductPageTemplateAndInsertIntoDB(int portalId, int? webstoreVersionId, string cacheKey)
        {

            IZnodeRepository<ZnodePublishProductPageEntity> _publishProductPageEntity = new ZnodeRepository<ZnodePublishProductPageEntity>(HelperMethods.Context);
            List<ZnodePublishProductPageEntity> pageEntities = _publishProductPageEntity.Table.Where(x => x.PortalId == portalId && x.VersionId == webstoreVersionId)?.ToList();
            if (pageEntities.Count > 0)
                HttpRuntime.Cache.Insert(cacheKey, pageEntities);
            return pageEntities;
        }

        //Get brand data for products.
        public virtual void GetBrandDataForProduct(PublishProductModel publishProduct, int? catalogVersionId = null)
        {
            IZnodeRepository<ZnodeBrandDetail> _brandDetails = new ZnodeRepository<ZnodeBrandDetail>();
            IZnodeRepository<ZnodeCMSSEODetail> _brandSeoDetail = new ZnodeRepository<ZnodeCMSSEODetail>();
            string brandCode = publishProduct.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.Brand)?.SelectValues?.FirstOrDefault()?.Code;

            if (!string.IsNullOrEmpty(brandCode))
            {

                BrandModel brandData = (from brandDetails in _brandDetails.Table
                                        join brandSeoDetail in _brandSeoDetail.Table on brandDetails.BrandId equals brandSeoDetail.SEOId
                                        join seoType in new ZnodeRepository<ZnodeCMSSEOType>().Table on brandSeoDetail.CMSSEOTypeId equals seoType.CMSSEOTypeId
                                        where seoType.Name == ZnodeConstant.Brand
                                        where brandDetails.BrandCode == brandCode && brandSeoDetail.CMSSEOTypeId == seoType.CMSSEOTypeId
                                        select new BrandModel()
                                        {
                                            SEOFriendlyPageName = brandSeoDetail.SEOUrl,
                                            IsActive = brandDetails.IsActive,
                                            BrandId = (int)brandSeoDetail.SEOId
                                        }).FirstOrDefault();

                if (HelperUtility.IsNotNull(brandData))
                {

                    publishProduct.BrandId = brandData.BrandId;
                    publishProduct.BrandSeoUrl = brandData.SEOFriendlyPageName;
                    publishProduct.IsBrandActive = brandData.IsActive;
                }
            }
        }

        //Get Portal brand data for products.
        public virtual void GetBrandDataForProduct(PublishProductModel publishProduct, int portalId, int? webstoreVersionId)
        {
            string brandCode = publishProduct?.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.Brand)?.SelectValues?.FirstOrDefault()?.Code;

            if (!string.IsNullOrEmpty(brandCode))
            {
                if (HelperUtility.IsNull(webstoreVersionId) || webstoreVersionId == 0)
                {
                    webstoreVersionId = GetWebstoreVersionId(publishProduct, portalId);
                }
                IZnodeRepository<ZnodePublishPortalBrandEntity> _publishPortalBrandEntity = new ZnodeRepository<ZnodePublishPortalBrandEntity>(HelperMethods.Context);

                ZnodePublishPortalBrandEntity brandData = _publishPortalBrandEntity.Table.FirstOrDefault(x => x.PortalId == portalId && x.BrandCode == brandCode && x.LocaleId == publishProduct.LocaleId && x.VersionId == webstoreVersionId && x.IsActive);

                BrandModel brandModel = brandData?.ToModel<BrandModel>();
                if (HelperUtility.IsNotNull(brandModel))
                {
                    publishProduct.BrandId = brandModel.BrandId;
                    publishProduct.BrandSeoUrl = brandModel.SEOFriendlyPageName;
                    publishProduct.IsBrandActive = brandModel.IsActive;
                }
            }
        }

        //Get Product Inventory details.
        public virtual void GetProductInventory(PublishProductModel publishProduct, int portalId)
        {
            List<string> skus = new List<string>();
            string mainProductSku = string.IsNullOrEmpty(publishProduct.ConfigurableProductSKU) ? publishProduct.SKU : publishProduct.ConfigurableProductSKU;
            skus.Add(mainProductSku);
            if (publishProduct?.PublishBundleProducts?.Count > 0)
            {
                skus.AddRange(publishProduct.PublishBundleProducts.Select(d => d.SKU));
            }
            List<InventorySKUModel> inventories = GetInventoryBySKUs(skus, portalId);
            if (inventories.Count > 0)
            {
                BindProductInventoryDetails(publishProduct, mainProductSku, inventories);
            }
        }

        protected virtual void BindProductInventoryDetails(PublishProductModel publishProduct, string mainProductSku, List<InventorySKUModel> inventories)
        {
            InventorySKUModel inventory = inventories?.FirstOrDefault(d => d.SKU == mainProductSku);
            publishProduct.Quantity = inventory?.Quantity;
            publishProduct.ReOrderLevel = inventory?.ReOrderLevel;
            publishProduct.DefaultWarehouseName = inventory?.WarehouseName;
            publishProduct.DefaultInventoryCount = inventory?.DefaultInventoryCount;

            int inventoryCount = 0;
            publishProduct.DefaultInventoryCount = Int32.TryParse(publishProduct.DefaultInventoryCount?.Split('.').First(), out inventoryCount) ? inventoryCount.ToString() : publishProduct.DefaultInventoryCount;
            //Get inventories for associated bundle products.
            if (publishProduct?.PublishBundleProducts?.Count > 0)
            {
                foreach (AssociatedPublishedBundleProductModel inv in publishProduct.PublishBundleProducts)
                {
                    InventorySKUModel inventorySKUModel = inventories?.FirstOrDefault(d => d.SKU == inv.SKU);
                    inv.Quantity = inventorySKUModel?.Quantity;
                    inv.ReOrderLevel = inventorySKUModel?.ReOrderLevel;
                    inv.DefaultWarehouseName = inventorySKUModel?.WarehouseName;
                    inv.DefaultInventoryCount = inventorySKUModel?.DefaultInventoryCount;
                    int inventoryChildCount = 0;
                    inv.DefaultInventoryCount = Int32.TryParse(inv.DefaultInventoryCount?.Split('.').First(), out inventoryChildCount) ? inventoryChildCount.ToString() : inv.DefaultInventoryCount;
                }
            }
        }

        //Get Promotions for product.
        public virtual void GetProductPromotions(PublishProductModel publishProduct, int userId = 0, int portalId = 0) =>
            publishProduct.Promotions = GetPromotionByPublishProductIds(new List<int> { publishProduct.PublishProductId }, userId, portalId);

        //Get customer product review of product.
        [Obsolete]
        public virtual List<CustomerReviewModel> GetProductReviews(int publishProductId, int portalId)
        {
            IZnodeRepository<ZnodeCMSCustomerReview> _productReviewRepository = new ZnodeRepository<ZnodeCMSCustomerReview>();
            return MapProductReviews(_productReviewRepository.Table.Where(x => x.PublishProductId == publishProductId && x.Status == "A" && x.PortalId == portalId).ToList());
        }

        //Get customer product review of product using sku.
        public virtual List<CustomerReviewModel> GetProductReviews(int portalId, string sku)
        {
            IZnodeRepository<ZnodeCMSCustomerReview> _productReviewRepository = new ZnodeRepository<ZnodeCMSCustomerReview>();
            return MapProductReviews(_productReviewRepository.Table.Where(x => x.SKU == sku && x.Status == "A" && x.PortalId == portalId).ToList());
        }

        //get products associated to categories from expands
        public virtual void GetDataFromExpands(int portalId, List<string> navigationProperties, PublishProductListModel publishProductListModel, int localeId, int userId = 0, int catalogVersionId = 0, int profileId = 0)
        {
            if (publishProductListModel?.PublishProducts?.Count > 0 && (navigationProperties?.Count > 0))
            {
                foreach (string key in navigationProperties)
                {
                    switch (key.ToLower())
                    {
                        case ZnodeConstant.Promotions:
                            //Get Promotions Associated to Publish Product Id.
                            GetProductPromotionsForProductList(publishProductListModel, userId, portalId);
                            break;
                        case ZnodeConstant.Inventory:
                            //Get Warehouse Address Associated to SKU.
                            if (portalId > 0)
                                MapInventory(publishProductListModel, GetInventoryBySKUs(publishProductListModel.PublishProducts.Select(x => x.SKU), portalId));
                            break;
                        case ZnodeConstant.Pricing:
                            //Get pricing associated to sku.
                            if (portalId > 0)
                                //Maps Price to Products.
                                MapPrice(publishProductListModel, GetPricingBySKUs(publishProductListModel.PublishProducts.Select(x => x.SKU), portalId, userId));
                            break;
                        case ZnodeConstant.SEO:
                            //Get Product SEO and reviews.
                            if (portalId > 0)
                                GetProductsSEOAndReviews(portalId, publishProductListModel, localeId, catalogVersionId);
                            break;
                        case ZnodeConstant.AdminSEO:
                            //Get Product SEO and reviews.
                            if (portalId > 0)
                                GetProductsSEODetails(portalId, publishProductListModel, localeId);
                            break;
                        case ZnodeConstant.ConfigurableAttribute:
                            GetConfigurableAttibuteForList(publishProductListModel, localeId, catalogVersionId);
                            break;
                        case ZnodeConstant.AssociatedProducts:
                            publishProductListModel.PublishProducts = GetAssociatedProducts(publishProductListModel.PublishProducts, portalId, userId, localeId, catalogVersionId);
                            break;
                        case ZnodeConstant.WishlistAddOns:
                            //Get Add Ons for product.
                            if (portalId > 0)
                                publishProductListModel.PublishProducts = GetProductsAddOn(publishProductListModel?.PublishProducts, portalId, userId, localeId, catalogVersionId);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        //get products associated to categories from expands
        public virtual void GetDataFromExpands(int portalId, List<string> navigationProperties, List<SearchProductModel> searchProducts, int localeId, int userId = 0, int catalogVersionId = 0, int profileId = 0)
        {
            if (searchProducts?.Count > 0 && (navigationProperties?.Count > 0))
            {
                foreach (string key in navigationProperties)
                {
                    switch (key.ToLower())
                    {
                        case ZnodeConstant.Promotions:
                            //Get Promotions Associated to Publish Product Id.
                            GetProductPromotionsForProductList(searchProducts, userId, portalId);
                            break;
                        case ZnodeConstant.Inventory:
                            //Get Warehouse Address Associated to SKU.
                            if (portalId > 0)
                                MapInventory(searchProducts, GetInventoryBySKUs(searchProducts.Select(x => x.SKU), portalId));
                            break;
                        case ZnodeConstant.Pricing:
                            //Get pricing associated to sku.
                            if (portalId > 0)
                                //Maps Price to Products.
                                MapPrice(searchProducts, GetPricingBySKUs(searchProducts.Select(x => x.SKU), portalId, userId));
                            break;
                        case ZnodeConstant.SEO:
                            //Get Product SEO and reviews.
                            if (portalId > 0)
                                GetProductsSEOAndReviews(portalId, searchProducts, localeId, catalogVersionId);
                            break;
                        case ZnodeConstant.ConfigurableAttribute:
                            GetConfigurableAttibuteForList(searchProducts, localeId);
                            break;
                        case ZnodeConstant.AssociatedProducts:
                            searchProducts = GetAssociatedProducts(searchProducts, portalId, userId, localeId, catalogVersionId);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        //get products associated to categories from expands
        public virtual void GetCartDataFromExpands(int portalId, List<string> navigationProperties, PublishProductListModel publishProductListModel, int localeId, int userId = 0, int catalogVersionId = 0, int omsOrderId = 0)
        {
            if (publishProductListModel?.PublishProducts?.Count > 0 && (navigationProperties?.Count > 0))
            {
                GetAdditionalProductData(publishProductListModel, navigationProperties, localeId, portalId, userId, omsOrderId);

                foreach (string key in navigationProperties)
                {
                    switch (key.ToLower())
                    {
                       case ZnodeConstant.SEO:
                            //Get Product SEO and reviews.
                            if (portalId > 0)
                                GetProductsSEOAndReviews(portalId, publishProductListModel, localeId, catalogVersionId);
                            break;
                        case ZnodeConstant.AdminSEO:
                            //Get Product SEO and reviews.
                            if (portalId > 0)
                                GetProductsSEODetails(portalId, publishProductListModel, localeId);
                            break;
                        case ZnodeConstant.ConfigurableAttribute:
                            GetConfigurableAttibuteForList(publishProductListModel, localeId, catalogVersionId);
                            break;
                        case ZnodeConstant.AssociatedProducts:
                            publishProductListModel.PublishProducts = GetAssociatedProducts(publishProductListModel.PublishProducts, portalId, userId, localeId, catalogVersionId);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void GetAssociateProducts(List<SearchProductModel> searchProducts, int catalogVersionId = 0)
        {

            searchProducts.ForEach(x =>
            {
                x.AssociatedProducts = new List<AssociatedProductsModel>();

                //Get Configurable Product

                List<PublishedConfigurableProductEntityModel> configEntity = _publishConfigurableProductEntity.Table.Where(p => p.ZnodeProductId == x.ZnodeProductId)?.ToModel<PublishedConfigurableProductEntityModel>()?.ToList();
                configEntity = catalogVersionId > 0 ? configEntity.Where(p => p.VersionId == catalogVersionId)?.ToList() : configEntity;
                if (!Equals(configEntity, null))
                {
                    //Get filter
                    FilterCollection filters = GetConfigurableProductFilter(x.LocaleId, configEntity, catalogVersionId);

                    //Get Product list by first attribute code.
                    List<PublishedProductEntityModel> productList = GetProductList(filters)?.ToModel<PublishedProductEntityModel>()?.ToList();
                    if (productList?.Count > 0)
                    {
                        var _products = (from p in productList
                                         join c in configEntity on p.ZnodeProductId equals c.AssociatedZnodeProductId
                                         select new AssociatedProductsModel
                                         {
                                             PublishProductId = p.ZnodeProductId,
                                             SKU = p.SKU,
                                             DisplayOrder = c.AssociatedProductDisplayOrder,
                                             OMSColorSwatchText = GetOMSSwatchTextAttributeValue(p, nameof(AssociatedProductsModel.OMSColorCode)),
                                             OMSColorCode = GetOMSCodeAttributeValue(p, nameof(AssociatedProductsModel.OMSColorCode)),
                                             OMSColorValue = GetOMSValueTextAttributeValue(p, nameof(AssociatedProductsModel.OMSColorCode)),
                                             OMSColorPath = GetOMSValuePathAttributeValue(p, nameof(AssociatedProductsModel.OMSColorCode))
                                         }).ToList();
                        x.AssociatedProducts.AddRange(_products);

                    }
                    filters = null;
                }
                else
                {
                    IZnodeRepository<ZnodePublishGroupProductEntity> _publishGroupProductEntity = new ZnodeRepository<ZnodePublishGroupProductEntity>(HelperMethods.Context);
                    //Get Configurable Product
                    List<ZnodePublishGroupProductEntity> groupEntity = _publishGroupProductEntity.Table.Where(p => p.ZnodeProductId == x.ZnodeProductId)?.ToList();

                    if (!Equals(groupEntity, null) && groupEntity?.Count > 0)
                    {
                        //Get filter
                        FilterCollection filters = GetGroupProductFilter(x.LocaleId, groupEntity);

                        //Get Product list by first attribute code.
                        List<PublishedProductEntityModel> productList = GetProductList(filters)?.ToModel<PublishedProductEntityModel>()?.ToList();

                        if (productList?.Count > 0)
                        {
                            var _products = (from p in productList
                                             select new AssociatedProductsModel
                                             {
                                                 PublishProductId = p.ZnodeProductId,
                                                 SKU = p.SKU,
                                                 OMSColorSwatchText = GetOMSSwatchTextAttributeValue(p, nameof(AssociatedProductsModel.OMSColorCode)),
                                                 OMSColorCode = GetOMSCodeAttributeValue(p, nameof(AssociatedProductsModel.OMSColorCode)),
                                                 OMSColorValue = GetOMSValueTextAttributeValue(p, nameof(AssociatedProductsModel.OMSColorCode)),
                                                 OMSColorPath = GetOMSValuePathAttributeValue(p, nameof(AssociatedProductsModel.OMSColorCode))
                                             }).ToList();
                            x.AssociatedProducts.AddRange(_products);
                        }
                    }
                }

            });
        }

        //Gets the list of published products
        protected virtual List<ZnodePublishProductEntity> GetProductList(FilterCollection filters)
        {

            try
            {
                IZnodeRepository<ZnodePublishProductEntity> _publishProductEntity = new ZnodeRepository<ZnodePublishProductEntity>(HelperMethods.Context);
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollections());
                return  _publishProductEntity.GetEntityListWithoutOrderBy(whereClauseModel.WhereClause, whereClauseModel.FilterValues)?.ToList();
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetProductList method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Get promotions Associated to Publish Product Ids
        public virtual void GetProductPromotionsForProductList(PublishProductListModel publishProductListModel, int userId = 0, int portalId = 0)
        {
            //Get promotions Associated to Publish Product Ids
            List<ProductPromotionModel> promotions = GetPromotionByPublishProductIds(publishProductListModel.PublishProducts.Select(p => p.PublishProductId), userId, portalId);

            //map Promotions to products
            publishProductListModel.PublishProducts.ForEach(
                x => x.Promotions = promotions?.Where(s => s.PublishProductId == x.PublishProductId)?.ToList());
        }

        //Get promotions Associated to Publish Product Ids
        public virtual void GetProductPromotionsForProductList(List<SearchProductModel> productList, int userId = 0, int portalId = 0)
        {
            //Get promotions Associated to Znode Product Ids
            List<ProductPromotionModel> promotions = GetPromotionByPublishProductIds(productList.Select(p => p.ZnodeProductId), userId, portalId);

            //map Promotions to products
            productList.ForEach(
                x => x.Promotions = promotions?.Where(s => s.PublishProductId == x.ZnodeProductId)?.ToList());
        }

        //Get add ons data.
        public virtual List<WebStoreAddOnModel> GetAddOnsData(int publishProductId, int configurableProductId, int portalId, int localeId, int? catalogVersionId, int userId, int publishCatalogId, int profileId = 0, int omsOrderId = 0)
        {
            List<WebStoreAddOnModel> AddOns = new List<WebStoreAddOnModel>();
            if (catalogVersionId <= 0 || catalogVersionId == null)
            {
                catalogVersionId = GetCatalogVersionId(publishCatalogId, localeId);
            }
            if (publishProductId > 0 || configurableProductId > 0)
            {

                FilterCollection filters = new FilterCollection();
                filters.Add("LocaleId", FilterOperators.Equals, localeId.ToString());

                if (configurableProductId > 0)
                    filters.Add("ZnodeProductId", FilterOperators.Equals, configurableProductId.ToString());
                else
                    filters.Add("ZnodeProductId", FilterOperators.Equals, publishProductId.ToString());
                if (catalogVersionId.HasValue && catalogVersionId.Value > 0)
                    filters.Add("VersionId", FilterOperators.Equals, catalogVersionId.ToString());

                List<ZnodePublishAddonEntity> addonGroups = GetAddOnList(filters)?.OrderBy(x => x.DisplayOrder)?.ToList();


                List<string> groupNameList = addonGroups.GroupBy(x => x.GroupName).Select(grp => grp.First().GroupName).ToList();

                foreach (string groupName in groupNameList)
                {
                    WebStoreAddOnModel AddOn = addonGroups.Where(addongroup => addongroup.GroupName == groupName).Select(x => new WebStoreAddOnModel()
                    {
                        DisplayType = x.DisplayType,
                        DisplayOrder = (from item in _pimAddOnProductRepository.Table
                                        join itemDetailsLocale in _pimAddonGroupLocaleRepository.Table on item.PimAddonGroupId equals itemDetailsLocale.PimAddonGroupId
                                        where item.PimProductId == publishProductId && itemDetailsLocale.AddonGroupName == groupName
                                        select item).FirstOrDefault()?.DisplayOrder ?? 99,
                        IsRequired = string.Equals(x.RequiredType, WebStoreEnum.Required.ToString(), StringComparison.InvariantCultureIgnoreCase) ? true : false,
                        IsAutoAddon = string.Equals(x.RequiredType, WebStoreEnum.Auto.ToString(), StringComparison.InvariantCultureIgnoreCase) ? true : false,
                        GroupName = x.GroupName
                    }).FirstOrDefault();

                    string[] associatedProductIds = addonGroups.Where(addongroup => addongroup.GroupName == groupName).Select(x => x.AssociatedZnodeProductId.ToString()).ToArray();

                    GetAddOnValues(localeId, portalId, string.Join(",", associatedProductIds), AddOn, userId, groupName, catalogVersionId, profileId, omsOrderId);

                    if (AddOn?.AddOnValues?.Count > 0)
                        AddOns.Add(AddOn);
                }
                //Associate display order to addon values.
                foreach (ZnodePublishAddonEntity item in addonGroups)
                {
                    AddOns.ForEach(addon => addon.AddOnValues.ForEach(x =>
                    {
                        if (x.PublishProductId == item.AssociatedZnodeProductId && x.GroupName == item.GroupName)
                        {
                            x.DisplayOrder = item.AssociatedProductDisplayOrder;
                            x.IsDefault = item.IsDefault;
                        }
                    }));
                }
                //Sort addon values by display order.
                AddOns.ForEach(x => x.AddOnValues = x.AddOnValues.OrderBy(addonvalue => addonvalue.DisplayOrder).ToList());              
                AddOns.Sort((x, y) => x.DisplayOrder.CompareTo(y.DisplayOrder));
            }
            return AddOns;
        }

        public virtual List<ZnodePublishAddonEntity> GetAddOnList(FilterCollection filters)
        {
            IZnodeRepository<ZnodePublishAddonEntity> _publishAddonEntity = new ZnodeRepository<ZnodePublishAddonEntity>(HelperMethods.Context);
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollections());
            return _publishAddonEntity.GetEntityListWithoutOrderBy(whereClauseModel.WhereClause, whereClauseModel.FilterValues)?.ToList();
        }

        public virtual List<WebStoreAddOnModel> GetAddOnProductsData(int[] publishProductIds, int configurableProductId, int portalId, int localeId, int? catalogVersionId, int userId, int publishCatalogId, int profileId = 0)
        {
            List<WebStoreAddOnModel> AddOns = new List<WebStoreAddOnModel>();
            if (catalogVersionId <= 0 || catalogVersionId == null)
                catalogVersionId = GetCatalogVersionId(publishCatalogId, localeId);

            if (publishProductIds?.Count() > 0)
            {

                FilterCollection filters = new FilterCollection();
                filters.Add("LocaleId", FilterOperators.Equals, localeId.ToString());

                if (configurableProductId > 0)
                    filters.Add("ZnodeProductId", FilterOperators.Equals, configurableProductId.ToString());
                else
                     filters.Add("ZnodeProductId", FilterOperators.In, string.Join(",", publishProductIds) );
                if (catalogVersionId.HasValue && catalogVersionId.Value > 0)
                    filters.Add("VersionId", FilterOperators.Equals, catalogVersionId.ToString());

                List<ZnodePublishAddonEntity> addongroups = GetAddOnList(filters)?.OrderBy(x => x.DisplayOrder)?.ToList();


                List<string> groupNameList = addongroups.GroupBy(x => x.GroupName).Select(grp => grp.First().GroupName).ToList();

                foreach (string groupName in groupNameList)
                {
                    WebStoreAddOnModel AddOn = addongroups.Where(addongroup => addongroup.GroupName == groupName).Select(x => new WebStoreAddOnModel()
                    {
                        DisplayType = x.DisplayType,
                        DisplayOrder = (from item in _pimAddOnProductRepository.Table
                                        join itemDetailsLocale in _pimAddonGroupLocaleRepository.Table on item.PimAddonGroupId equals itemDetailsLocale.PimAddonGroupId
                                        join publishProductId in publishProductIds on item.PimProductId equals publishProductId
                                        where itemDetailsLocale.AddonGroupName == groupName
                                        select item).FirstOrDefault().DisplayOrder,
                        IsRequired = string.Equals(x.RequiredType, WebStoreEnum.Required.ToString(), StringComparison.InvariantCultureIgnoreCase) ? true : false,
                        IsAutoAddon = string.Equals(x.RequiredType, WebStoreEnum.Auto.ToString(), StringComparison.InvariantCultureIgnoreCase) ? true : false,
                        GroupName = x.GroupName
                    }).FirstOrDefault();

                    string[] associatedProductIds = addongroups.Where(addongroup => addongroup.GroupName == groupName).Select(x => x.AssociatedZnodeProductId.ToString()).ToArray();

                    GetAddOnValues(localeId, portalId, string.Join(",", associatedProductIds), AddOn, userId, groupName, catalogVersionId);

                    if (AddOn?.AddOnValues?.Count > 0)
                        AddOns.Add(AddOn);
                }
                //Associate display order to addon values.
                foreach (ZnodePublishAddonEntity item in addongroups)
                {
                    AddOns.ForEach(addon => addon.AddOnValues.ForEach(x =>
                    {
                        if (x.PublishProductId == item.AssociatedZnodeProductId && x.GroupName == item.GroupName)
                        {
                            x.DisplayOrder = item.AssociatedProductDisplayOrder;
                            x.IsDefault = item.IsDefault;
                        }
                    }));
                }
                //Sort addon values by display order.
                AddOns.ForEach(x => x.AddOnValues = x.AddOnValues.OrderBy(addonvalue => addonvalue.DisplayOrder).ToList());
                AddOns.Sort((x, y) => x.DisplayOrder.CompareTo(y.DisplayOrder));
            }
            return AddOns;
        }

        // Get tax class id by sku and country code.
        public virtual int GetTaxClassId(string sKU, string countryCode)
        {
            int? taxClassId = (from taxClass in _znodeTaxClass.Table
                               join taxClassSKU in _znodeTaxClassSKU.Table on taxClass.TaxClassId equals taxClassSKU.TaxClassId
                               join taxRule in _znodeTaxRule.Table on taxClass.TaxClassId equals taxRule.TaxClassId
                               join taxRuleType in _znodeTaxRuleType.Table on taxRule.TaxRuleTypeId equals taxRuleType.TaxRuleTypeId

                               where
                                taxClass.IsActive && taxRuleType.IsActive && taxClassSKU.SKU == sKU && (taxRule.DestinationCountryCode == countryCode || taxRule.DestinationCountryCode == null)

                               select taxClassSKU.TaxClassId).FirstOrDefault();

            return taxClassId ?? 0;
        }

        // Get tax class id by sku and country code.
        public virtual List<TaxClassRuleModel> GetTaxRules(List<string> sKUs)
        {
            try
            { 
            List<TaxClassRuleModel> taxClassSKUs = null;
            if (sKUs?.Count > 0)
            {
                DataTable table = new DataTable("SKU");
                table.Columns.Add("SKU", typeof(string));
                foreach (string sku in sKUs)
                    table.Rows.Add(sku);
                ExecuteSpHelper executeSpHelper = new ExecuteSpHelper();
                executeSpHelper.SetTableValueParameter("@SKU", table, ParameterDirection.Input, SqlDbType.Structured, "dbo.SelectColumnList");
                DataSet taxSKUs = executeSpHelper.GetSPResultInDataSet("Znode_GetTaxRule");
                ConvertDataTableToList dt = new ConvertDataTableToList();
                taxClassSKUs = dt.ConvertDataTable<TaxClassRuleModel>(taxSKUs?.Tables[0]);
            }
            return taxClassSKUs;
        }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetTaxRules method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Get Associated product for configurable type.
        public virtual List<PublishProductModel> GetAssociatedProducts(int productId, int localeId, int? catalogVersionId, List<PublishedConfigurableProductEntityModel> configEntity)
        {
            try
            {
                //Check if entity is not null.
                if (HelperUtility.IsNotNull(configEntity) && configEntity?.Count > 0)
                {
                    FilterCollection filters = GetConfigurableProductFilter(localeId, configEntity, catalogVersionId);
                    //Get associated product list.
                    List<PublishProductModel> associatedProducts = GetProductList(filters).GroupBy(g => g.SKU).Select(s => s.FirstOrDefault())?.ToModel<PublishProductModel>()?.ToList();

                    List<PublishProductModel> newassociatedProducts = new List<PublishProductModel>();

                    //Assign Display order to associated product list.
                    associatedProducts?.ForEach(d =>
                    {
                        PublishedConfigurableProductEntityModel configurableProductEntity = configEntity
                                    .FirstOrDefault(s => s.AssociatedZnodeProductId == d.ProductId);
                        d.DisplayOrder = HelperUtility.IsNotNull(configurableProductEntity) ? configurableProductEntity.AssociatedProductDisplayOrder : 999;
                        foreach (PublishAttributeModel attribute in d.Attributes?.Where(x => x.IsConfigurable))
                        {
                            attribute.AttributeValues = attribute.SelectValues.FirstOrDefault()?.Value;
                        }
                        newassociatedProducts.Add(d);
                    });

                    //Sort list according to display order.
                    newassociatedProducts = newassociatedProducts.OrderBy(x => x.DisplayOrder)?.ToList();

                    return newassociatedProducts;
                }
                return null;
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetAssociatedProducts method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Map Configurable attributes.
        public virtual List<PublishAttributeModel> MapConfigurableAttributeData(List<List<PublishAttributeModel>> attributeList, List<PublishedProductEntityModel> products)
        {
            try
            {
                List<PublishAttributeModel> ConfigurableAttributeList = new List<PublishAttributeModel>();
                List<ConfigurableAttributeModel> attributesList = new List<ConfigurableAttributeModel>();

                if (HelperUtility.IsNotNull(attributeList))
                {
                    foreach (List<PublishAttributeModel> attributeEntityList in attributeList)
                    {
                        attributesList.Clear();
                        PublishAttributeModel attributesModel = new PublishAttributeModel();
                        foreach (PublishAttributeModel attributeValue in attributeEntityList)
                        {
                            //Check if attribute already exist in list.
                            if (!AlreadyExist(attributesList, attributeValue.AttributeValues).GetValueOrDefault())
                            {
                                ConfigurableAttributeModel attribute = new ConfigurableAttributeModel();
                                attribute.AttributeValue = attributeValue.AttributeValues;
                                attributesList.Add(attribute);
                            }
                        }

                        //Set Attribute details. adn to configurable attribute list.
                        attributesModel.AttributeName = attributeEntityList.FirstOrDefault().AttributeName;
                        attributesModel.AttributeCode = attributeEntityList.FirstOrDefault().AttributeCode;
                        attributesModel.IsConfigurable = attributeEntityList.FirstOrDefault().IsConfigurable;
                        attributesModel.ConfigurableAttribute.AddRange(attributesList);
                        ConfigurableAttributeList.Add(attributesModel);
                    }
                }
                return ConfigurableAttributeList;
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in MapConfigurableAttributeData method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Get Configurable product variants.
        public virtual List<List<PublishAttributeModel>> GetConfigurableAttributes(List<PublishProductModel> productList, List<string> ConfigurableAttributeCodes = null)
        {
            if (productList?.Count > 0 && HelperUtility.IsNotNull(ConfigurableAttributeCodes))
            {
                //Get configurable product Attribute ,//assigned display order of product to configurable attribute to display it on webstore depend on display order.
                IEnumerable<PublishAttributeModel> Attributes = productList?.SelectMany(x => x.Attributes?.Where(y => y.IsConfigurable && !string.IsNullOrEmpty(y.AttributeValues) && ConfigurableAttributeCodes.Contains(y.AttributeCode)));

                return Attributes?.GroupBy(u => u.AttributeCode)?.Select(grp => grp?.ToList())?.Distinct()?.ToList();
            }
            return null;
        }

        //Get current catalog version id by catalog id.
        public virtual int GetCatalogVersionId(int publishCatalogId, int localeId = 0)
        {
            try
            {
                IZnodeRepository<ZnodePublishVersionEntity> _versionEntity = new ZnodeRepository<ZnodePublishVersionEntity>(HelperMethods.Context);
                ZnodePublishStatesEnum contentState = GetPortalPublishState();

                localeId = localeId > 0 ? localeId : getDefaultLocale();

                int? version = (from versionEntity in _versionEntity.Table.Where(x => x.ZnodeCatalogId == publishCatalogId && x.LocaleId == localeId && x.RevisionType == contentState.ToString() && x.IsPublishSuccess) select versionEntity.VersionId).FirstOrDefault();

                return version ?? 0;
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetCatalogVersionId method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        public virtual int getDefaultLocale()
            => _znodeLocaleRepository.Table.Where(x => x.IsDefault).Any() ? _znodeLocaleRepository.Table.FirstOrDefault(x => x.IsDefault).LocaleId : 0;

        //Get Content State for this portal.
        public ZnodePublishStatesEnum GetPortalPublishState()
        {
            const string headerPublishState = "Znode-PublishState";
            ZnodePublishStatesEnum publishState;
            var headers = HttpContext.Current.Request.Headers;
            Enum.TryParse(headers[headerPublishState], true, out publishState);

            if (publishState == 0)
            {
                //If state not found in request header. Try to achieve the same using DomainName header of the same request.
                ApplicationTypesEnum applicationType = GetApplicationTypeForDomain();

                if (applicationType != 0)
                {
                    publishState = GetPublishStateFromApplicationType(applicationType);

                    if (publishState != 0)
                        return publishState;
                }

                //Fall back to the default content state.
                publishState = GetDefaultPublishState();
            }

            return publishState;
        }

        public virtual void GetAssociatedProducts(List<PublishProductModel> publishProductList, int portalId, int userId, bool getPricing)
        {
            publishProductList.ForEach(x =>
            {
                GetAssociatedProducts(x, portalId, userId, getPricing);
            });
        }

        public virtual void GetAssociatedProducts(PublishProductModel publishProduct, int portalId, int userId, bool getPricing = false, int profileId = 0)
        {
            ConcurrentDictionary<string, string> additionalAttributesDictionary = new ConcurrentDictionary<string, string>();
            string requiredAdditionalAttributes = ConfigurationManager.AppSettings[_AdditionalAttributes];

            publishProduct.AssociatedProducts = new List<AssociatedProductsModel>();

            //Get Configurable Product
            List<PublishedConfigurableProductEntityModel> configEntity = _publishConfigurableProductEntity.Table.Where(x => x.ZnodeProductId == publishProduct.PublishProductId)?.ToModel<PublishedConfigurableProductEntityModel>()?.ToList();
            if (!Equals(configEntity, null) && configEntity?.Count > 0)
            {
                int catalogVersionId = GetCatalogVersionId(configEntity[0].ZnodeCatalogId);
                configEntity = configEntity.Where(x => x.VersionId == catalogVersionId).ToList();

                //Get filter
                FilterCollection filters = GetConfigurableProductFilter(publishProduct.LocaleId, configEntity, catalogVersionId);
                List<PublishedProductEntityModel> products = GetProductList(filters)?.ToModel<PublishedProductEntityModel>()?.ToList();

                List<PriceSKUModel> pricingBySKU = null;

                if (getPricing)
                    pricingBySKU = GetPricingBySKUs(string.Join(",", products.Select(x => x.SKU).ToArray()), portalId, userId, profileId);

                List<AssociatedProductsModel> _products = (from p in products
                                                           select new AssociatedProductsModel
                                                           {
                                                               PublishProductId = p.ZnodeProductId,
                                                               PimProductId = p.ZnodeProductId,
                                                               SKU = p.SKU,
                                                               RetailPrice = GetPricingInfoFromList(nameof(AssociatedProductsModel.RetailPrice), pricingBySKU, p.SKU),
                                                               SalesPrice = GetPricingInfoFromList(nameof(AssociatedProductsModel.SalesPrice), pricingBySKU, p.SKU),
                                                               CurrencyCode = GetCurrencyInfoFromList(nameof(AssociatedProductsModel.CurrencyCode), pricingBySKU, p.SKU),
                                                               CurrencySuffix = GetCurrencyInfoFromList(nameof(AssociatedProductsModel.CurrencySuffix), pricingBySKU, p.SKU),
                                                               AdditionalAttributes = GetAdditionalAttributeValues(p, requiredAdditionalAttributes, additionalAttributesDictionary)
                                                           }).ToList();

                publishProduct.AssociatedProducts.AddRange(_products);
            }
        }

        protected ApplicationTypesEnum GetApplicationTypeForDomain()
        {
            try
            {
                IZnodeRepository<ZnodeDomain> _domainRepository = new ZnodeRepository<ZnodeDomain>();

                ApplicationTypesEnum applicationType = 0;

                string domainName = GetPortalDomainName();

                if (!string.IsNullOrEmpty(domainName))
                {
                    FilterDataCollection filters = new FilterDataCollection();
                    filters.Add(FilterKeys.DomainName, FilterOperators.Equals, "\"" + domainName + "\"");

                    ZnodeDomain domain = _domainRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters).WhereClause);

                    if (HelperUtility.IsNotNull(domain))
                        Enum.TryParse(domain.ApplicationType, true, out applicationType);
                }
                return applicationType;

            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetApplicationTypeForDomain method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        private string GetPortalDomainName()
        {
            const string headerDomainName = "Znode-DomainName";
            var headers = HttpContext.Current.Request.Headers;
            string domainName = headers[headerDomainName];
            return domainName;
        }

        //Get Content state mapped to supplied application type.
        protected ZnodePublishStatesEnum GetPublishStateFromApplicationType(ApplicationTypesEnum applicationType)
        {
            List<PublishStateMappingModel> applicationTypeMappings = GetAvailablePublishStateMappings();

            ZnodePublishStatesEnum publishState = 0;

            if (HelperUtility.IsNotNull(applicationTypeMappings))
            {
                string publishStateCode = applicationTypeMappings.Where(x => x.ApplicationType == applicationType.ToString() && x.IsEnabled)?.FirstOrDefault()?.PublishState;

                if (!string.IsNullOrEmpty(publishStateCode))
                    Enum.TryParse(publishStateCode, out publishState);
            }

            return publishState;
        }

        protected List<PublishStateMappingModel> GetAvailablePublishStateMappings()
        {
            if (Equals(HttpRuntime.Cache["PublishStateMappings"], null))
            {
                IZnodeRepository<ZnodePublishStateApplicationTypeMapping> _publishStateMappingRepository = new ZnodeRepository<ZnodePublishStateApplicationTypeMapping>();
                IZnodeRepository<ZnodePublishState> _publishStateRepository = new ZnodeRepository<ZnodePublishState>();

                List<PublishStateMappingModel> publishStateMappings = (from publishState in _publishStateMappingRepository.Table
                                                                       join PS in _publishStateRepository.Table on publishState.PublishStateId equals PS.PublishStateId
                                                                       where publishState.IsActive
                                                                       select new PublishStateMappingModel
                                                                       {
                                                                           PublishStateMappingId = publishState.PublishStateMappingId,
                                                                           ApplicationType = publishState.ApplicationType,
                                                                           PublishStateCode = PS.PublishStateCode,
                                                                           Description = publishState.Description,
                                                                           IsDefault = PS.IsDefaultContentState,
                                                                           IsEnabled = publishState.IsEnabled,
                                                                           PublishStateId = publishState.PublishStateId,
                                                                           PublishState = PS.PublishStateCode
                                                                       }).ToList();

                HttpRuntime.Cache.Insert("PublishStateMappings", publishStateMappings);
            }

            return (List<PublishStateMappingModel>)HttpRuntime.Cache.Get("PublishStateMappings");
        }

        protected ZnodePublishStatesEnum GetDefaultPublishState()
        {
            if (Equals(HttpRuntime.Cache["DefaultPublishState"], null))
            {
                ZnodePublishStatesEnum publishState = FetchDefaultPublishState();
                HttpRuntime.Cache.Insert("DefaultPublishState", publishState);
            }

            return (ZnodePublishStatesEnum)HttpRuntime.Cache.Get("DefaultPublishState");
        }

        private ZnodePublishStatesEnum FetchDefaultPublishState()
        {
            IZnodeRepository<ZnodePublishState> _publishStateRepository = new ZnodeRepository<ZnodePublishState>();
            string publishStateCode = _publishStateRepository.Table.Where(x => x.IsContentState && x.IsDefaultContentState)?.FirstOrDefault()?.PublishStateCode;

            ZnodePublishStatesEnum publishState;

            if (!string.IsNullOrEmpty(publishStateCode) && Enum.TryParse(publishStateCode, true, out publishState))
                return publishState;
            else
                return ZnodePublishStatesEnum.PRODUCTION;
        }

        public virtual List<PublishProductModel> GetDataForCartLineItems(List<string> sku, int catalogId, int localeId, List<string> navigationProperties, int userId, int portalId, int versionId, out List<PublishedConfigurableProductEntityModel> configurableProductEntities, bool includeInactiveProduct = false, int omsOrderId = 0)
        {

            FilterCollection filters = new FilterCollection();
            if(sku?.Count > 0)
                filters.Add("SKULower", FilterOperators.In, string.Join(",", sku.Select(x => $"\"{x}\"")));
            filters.Add("ZnodeCatalogId", FilterOperators.Equals, catalogId.ToString());
            filters.Add("LocaleId", FilterOperators.Equals, localeId.ToString());
            filters.Add("VersionId", FilterOperators.Equals, versionId.ToString());

            if (!includeInactiveProduct)
                filters.Add("IsActive", FilterOperators.Equals, "true");

            List<ZnodePublishProductEntity> productList = sku?.Count > 0 ? GetProductList(filters) : new List<ZnodePublishProductEntity>();

            PublishProductListModel listModel = new PublishProductListModel();

            listModel.PublishProducts = productList.ToModel<PublishProductModel>().ToList();

            List<int> lstIds = productList.Select(x => x.ZnodeProductId)?.Distinct().ToList();

            List<PublishedConfigurableProductEntityModel> configurableProducts = GetConfigurableProductEntity(lstIds, versionId);

            List<PublishedConfigurableProductEntityModel> _collection = configurableProducts.Where(x => lstIds.Contains(x.AssociatedZnodeProductId)).ToList();

            List<int> parentIds = _collection.Select(x => x.ZnodeProductId)?.Distinct().ToList();

            if (parentIds?.Count > 0)
            {
                listModel.PublishProducts.ForEach(x =>
                {
                    int? parentId = _collection.FirstOrDefault(y => y.AssociatedZnodeProductId == x.PublishProductId)?.ZnodeProductId;

                    if (!Equals(parentId, null))
                    {
                        x.ParentPublishProductSKU = productList.FirstOrDefault(y => y.ZnodeProductId == parentId)?.SKU;
                        x.ParentPublishProductId = Convert.ToInt32(parentId);
                    }
                });
            }

            GetCartDataFromExpands(portalId, navigationProperties, listModel, localeId, userId, versionId, omsOrderId);

            configurableProductEntities = configurableProducts;
            return listModel.PublishProducts;
        }


        #endregion

        #region Private Method

        //Get Configurable Attributes For List.
        protected virtual void GetConfigurableAttibuteForList(PublishProductListModel productList, int localeId, int catalogVersionId) =>
            productList.PublishProducts.ForEach(x => GetAttribute(x.PublishProductId, localeId, x.Attributes, catalogVersionId));

        //Get Configurable Attributes For List.
        protected virtual void GetConfigurableAttibuteForList(List<SearchProductModel> productList, int localeId) =>
            productList.ForEach(x => GetAttribute(x.ZnodeProductId, localeId, x.Attributes));

        //Get configurable attributes of product
        protected virtual void GetAttribute(int productId, int localeId, List<PublishAttributeModel> attribute, int catalogVersionId = 0)
        {

            List<PublishedConfigurableProductEntityModel> configEntity = GetConfigurableProductEntity(productId, catalogVersionId);

            if (attribute.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.ProductType).SelectValues.FirstOrDefault().Code.Equals(ZnodeConstant.ConfigurableProduct, StringComparison.InvariantCultureIgnoreCase))
            {
                List<PublishProductModel> associatedProducts = GetAssociatedProducts(productId, localeId, catalogVersionId, configEntity);
                //Get associated configurable product Attribute list.
                if (configEntity?.Count > 0)
                {
                    List<PublishAttributeModel> attributeList = MapConfigurableAttributeData(GetConfigurableAttributes(associatedProducts, configEntity?.FirstOrDefault().ConfigurableAttributeCodes), null);

                    foreach (var item in attributeList)
                        attribute.RemoveAll(x => x.AttributeCode == item.AttributeCode);

                    attribute.AddRange(attributeList);
                }
            }
        }

        //Get Attribute value already exist
        protected virtual bool? AlreadyExist(List<ConfigurableAttributeModel> ConfigurableAttributeList, string value) =>
         ConfigurableAttributeList?.Any(x => x.AttributeValue == value);

        protected static FilterCollection GetConfigurableProductFilter(int localeId, List<PublishedConfigurableProductEntityModel> configEntity, int? versionId = 0)
        {
            FilterCollection filters = new FilterCollection();
            List<string> znodeProductIds = configEntity?.Select(x => x.AssociatedZnodeProductId.ToString())?.ToList();

            if(znodeProductIds?.Count > 0)
                filters.Add(WebStoreEnum.ZnodeProductId.ToString(), FilterOperators.In, string.Join(",", znodeProductIds));
            filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString());
            filters.Add(WebStoreEnum.IsActive.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue);
            if (versionId > 0)
                filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.Equals, Convert.ToString(versionId));
            return filters;
        }

        protected static FilterCollection GetGroupProductFilter(int localeId, List<ZnodePublishGroupProductEntity> groupEntity)
        {
            FilterCollection filters = new FilterCollection();
            //Associated product ids.
            filters.Add(WebStoreEnum.ZnodeProductId.ToString(), FilterOperators.In, string.Join(",", groupEntity?.Select(x => x.AssociatedZnodeProductId.ToString())?.ToArray()));
            filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString());
            filters.Add(WebStoreEnum.IsActive.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue);
            return filters;
        }

        //Map product data.
        protected virtual void MapProductData(List<PublishProductModel> publishProducts, List<ZnodePublishSeoEntity> details)
        {
            try
            {
                if (publishProducts?.Count > 0)
                {
                    publishProducts.ForEach(product =>
                    {
                        ZnodePublishSeoEntity productDetails = details
                                    .FirstOrDefault(productdata => productdata.SEOCode == product.SKU);
                        if (HelperUtility.IsNotNull(productDetails))
                        {
                            product.SEODescription = productDetails.SEODescription;
                            product.SEOKeywords = productDetails.SEOKeywords;
                            product.SEOTitle = productDetails.SEOTitle;
                            product.SEOUrl = productDetails.SEOUrl;
                        }
                    });
                }
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in MapProductData method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Map product data.
        protected virtual void MapSEOProductData(List<PublishProductModel> publishProducts, List<ZnodePublishSeoEntity> details, List<SEODetailsModel> znodeCMSSEODetails)
        {
            try
            {
                if (publishProducts?.Count > 0)
                {
                    publishProducts.ForEach(product =>
                    {
                        ZnodePublishSeoEntity productDetails = details
                                    .FirstOrDefault(productdata => productdata.SEOCode == product.SKU);
                        SEODetailsModel seoDetails = znodeCMSSEODetails
                                  .FirstOrDefault(productdata => productdata.SEOCode == product.SKU);
                        product.PublishStatus = seoDetails?.IsPublish ?? false ? ZnodeConstant.Published : ZnodeConstant.Draft;
                        if (product.PublishStatus == ZnodeConstant.Published && HelperUtility.IsNotNull(productDetails))
                        {
                            product.SEODescription = productDetails.SEODescription;
                            product.SEOKeywords = productDetails.SEOKeywords;
                            product.SEOTitle = productDetails.SEOTitle;
                            product.SEOUrl = productDetails.SEOUrl;
                        }
                        else
                        {
                            if (HelperUtility.IsNotNull(seoDetails))
                            {
                                product.SEODescription = seoDetails.SEODescription;
                                product.SEOKeywords = seoDetails.SEOKeywords;
                                product.SEOTitle = seoDetails.SEOTitle;
                                product.SEOUrl = seoDetails.SEOUrl;
                            }
                        }
                    });
                }
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in MapSEOProductData method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Map product data.
        protected virtual void MapSearchProductData(List<SearchProductModel> searchProducts, List<ZnodePublishSeoEntity> details)
        {
            if (searchProducts?.Count > 0)
            {
                searchProducts.ForEach(product =>
                {
                    ZnodePublishSeoEntity productDetails = details
                                .FirstOrDefault(productdata => productdata.SEOCode == product.SKU);
                    if (HelperUtility.IsNotNull(productDetails))
                    {
                        product.SEODescription = productDetails.SEODescription;
                        product.SEOKeywords = productDetails.SEOKeywords;
                        product.SEOTitle = productDetails.SEOTitle;
                        product.SEOUrl = productDetails.SEOUrl;
                    }
                });
            }
        }

        protected virtual List<CustomerReviewModel> MapProductReviews(List<ZnodeCMSCustomerReview> productReviews)
        {
            List<CustomerReviewModel> customerReviewList = new List<CustomerReviewModel>();
            if (productReviews?.Count > 0)
            {
                //Sort review for newest review first.
                productReviews = productReviews.OrderByDescending(x => x.CreatedDate).ToList();
                foreach (ZnodeCMSCustomerReview customerReview in productReviews)
                    customerReviewList.Add(customerReview.ToModel<CustomerReviewModel>());

            }
            return customerReviewList;
        }

        protected virtual CustomerReviewModel ToModel(ZnodeCMSCustomerReview productReviews)
         => new CustomerReviewModel
         {
             CMSCustomerReviewId = productReviews.CMSCustomerReviewId,
             PublishProductId = productReviews.PublishProductId,
             Status = productReviews.Status,
             Rating = productReviews.Rating,
             UserName = productReviews.UserName,
             UserId = productReviews.UserId,
             UserLocation = productReviews.UserLocation,
             Headline = productReviews.Headline,
             CreatedDate = productReviews.CreatedDate,
             Comments = productReviews.Comments,
         };

        //Get addon values of addon group.
        protected virtual void GetAddOnValues(int localeId, int portalId, string associatedproductIds, WebStoreAddOnModel AddOn, int userId, string addonGroupName, int? catalogVersionId = null, int profileId = 0, int omsOrderId = 0)
        {
            FilterCollection filter = new FilterCollection();
            filter.Add(WebStoreEnum.ZnodeProductId.ToString(), FilterOperators.In, associatedproductIds);
            filter.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString());
            filter.Add(WebStoreEnum.IsActive.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue);

            if (catalogVersionId.HasValue && catalogVersionId.Value > 0)
                filter.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.Equals, Convert.ToString(catalogVersionId.Value));

            if (!filter.Any(filterItem => (filterItem.FilterName == FilterKeys.ZnodeCategoryIds && (filterItem.FilterOperator == FilterOperators.Equals || filterItem.FilterOperator == FilterOperators.In) && !string.IsNullOrEmpty(filterItem.FilterValue.Trim()))))
                filter.Add(new FilterTuple(FilterKeys.ProductIndex, FilterOperators.Equals, ZnodeConstant.DefaultPublishProductIndex.ToString()));

            List<ZnodePublishProductEntity> productList = GetProductList(filter);

            if (productList?.Count > 0)
            {
                List<InventorySKUModel> inventory = GetInventoryBySKUs(productList.Select(x => x.SKU), portalId);
                List<PriceSKUModel> priceSKU = GetPricingBySKUs(productList.Select(x => x.SKU), portalId, userId, profileId, omsOrderId);

                AddOn.AddOnValues = productList.ToModel<WebStoreAddOnValueModel>().ToList();
                if (AddOn.IsAutoAddon)
                    AddOn.AutoAddonSKUs = string.Join(",", productList.Select(x => x.SKU));
                AddOn.AddOnValues.ForEach(x => x.GroupName = addonGroupName);
                MapAddOnPriceAndInventory(AddOn, inventory, priceSKU);
            }
        }

        //Map addon values price and inventory.
        protected virtual void MapAddOnPriceAndInventory(WebStoreAddOnModel AddOn, List<InventorySKUModel> inventory, List<PriceSKUModel> priceSKU)
        {
            AddOn.AddOnValues.ForEach(product =>
            {
                PriceSKUModel productPrice = priceSKU
                            .FirstOrDefault(productdata => productdata.SKU == product.SKU);

                InventorySKUModel productInventory = inventory
                            .FirstOrDefault(productdata => productdata.SKU == product.SKU);

                if (HelperUtility.IsNotNull(productPrice))
                {
                    product.RetailPrice = productPrice.RetailPrice;
                    product.SalesPrice = HelperUtility.IsNull(productPrice.SalesPrice) ? productPrice.RetailPrice : productPrice.SalesPrice;
                    product.CurrencyCode = productPrice.CurrencyCode;
                    product.CultureCode = productPrice.CultureCode;
                }

                if (HelperUtility.IsNotNull(productInventory))
                {
                    product.Quantity = productInventory?.Quantity;
                    product.ReOrderLevel = productInventory?.ReOrderLevel;
                }
            });
        }

        //Get tier price data for publish product.
        protected virtual void GetTierPriceData(PublishProductModel publishProduct, List<PriceSKUModel> priceSKU)
        {
            try
            {
                //Null check for tier pricing.
                if (HelperUtility.IsNull(publishProduct?.TierPriceList))
                    publishProduct.TierPriceList = new List<PriceTierModel>();
                int Count = 1;

                //Bind tier pricing data for product if any.
                foreach (PriceSKUModel tierPriceSKU in priceSKU)
                {
                    if (HelperUtility.IsNotNull(tierPriceSKU.TierPrice) && HelperUtility.IsNotNull(tierPriceSKU.TierQuantity))
                    {
                        PriceTierModel tierPrice = new PriceTierModel();
                        tierPrice.Price = tierPriceSKU.TierPrice;
                        tierPrice.Quantity = tierPriceSKU.TierQuantity;
                        tierPrice.MinQuantity = tierPriceSKU.TierQuantity;
                        tierPrice.Custom1 = tierPriceSKU.Custom1;
                        tierPrice.Custom2 = tierPriceSKU.Custom2;
                        tierPrice.Custom3 = tierPriceSKU.Custom3;
                        if (HelperUtility.IsNotNull(priceSKU.ElementAtOrDefault(Count)))
                            tierPrice.MaxQuantity = priceSKU.ElementAt(Count).TierQuantity;
                        else
                            tierPrice.MaxQuantity = decimal.MaxValue;

                        publishProduct?.TierPriceList.Add(tierPrice);
                        Count++;
                    }
                }
                ModifyTierPriceListDetails(publishProduct?.TierPriceList);
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetTierPriceData method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Get tier price data for publish product.
        protected virtual void GetTierPriceData(PublishProductModel publishProduct, IList<PublishCategoryProductDetailModel> productDetails)
        {
            string associateProductSKu = publishProduct?.GroupProductSKUs?.Count > 0 ? publishProduct?.GroupProductSKUs.FirstOrDefault().Sku : publishProduct.ConfigurableProductSKU;
            List<string> skuList = new List<string> { publishProduct.SKU, associateProductSKu };

            List<PublishCategoryProductDetailModel> lstProductPriceDetail = productDetails?.Where(x => skuList.Contains(x.SKU)).ToList();

            //Null check for tier pricing.
            if (HelperUtility.IsNull(publishProduct?.TierPriceList))
                publishProduct.TierPriceList = new List<PriceTierModel>();
            int Count = 1;

            //Bind tier pricing data for product if any.
            foreach (PublishCategoryProductDetailModel tierPriceSKU in lstProductPriceDetail)
            {
                if (HelperUtility.IsNotNull(tierPriceSKU.TierPrice) && HelperUtility.IsNotNull(tierPriceSKU.TierQuantity))
                {
                    PriceTierModel tierPrice = new PriceTierModel();
                    tierPrice.Price = tierPriceSKU.TierPrice;
                    tierPrice.Quantity = tierPriceSKU.TierQuantity;
                    tierPrice.MinQuantity = tierPriceSKU.TierQuantity;
                    tierPrice.Custom1 = tierPriceSKU.Custom1;
                    tierPrice.Custom2 = tierPriceSKU.Custom2;
                    tierPrice.Custom3 = tierPriceSKU.Custom3;
                    if (HelperUtility.IsNotNull(lstProductPriceDetail.ElementAtOrDefault(Count)))
                        tierPrice.MaxQuantity = lstProductPriceDetail.ElementAt(Count).TierQuantity;
                    else
                        tierPrice.MaxQuantity = decimal.MaxValue;

                    publishProduct.TierPriceList.Add(tierPrice);
                    Count++;
                }
            }
            ModifyTierPriceListDetails(publishProduct.TierPriceList);
        }

        //Get Product SEo Settings.
        protected virtual void GetProductsSEO(PublishProductModel publishProduct, int portalId, int localeId, string whereClause, int? catalogVersionId = null)
        {
            ZnodeCMSPortalSEOSetting portalSeoSetting = new ZnodeRepository<ZnodeCMSPortalSEOSetting>().GetEntity(whereClause);

            string seoCode = string.IsNullOrEmpty(publishProduct.ParentSEOCode) ? publishProduct.Attributes?.FirstOrDefault(x => x.AttributeCode == "SKU")?.AttributeValues
                : publishProduct.ParentSEOCode;

            ZnodePublishSeoEntity seoDetails = GetPublishSEODetails(seoCode, ZnodeConstant.Product, portalId, localeId, catalogVersionId);

            publishProduct.SEODescription = GetSeoDetails(seoDetails?.SEODescription, portalSeoSetting?.ProductDescription, publishProduct);
            publishProduct.SEOKeywords = GetSeoDetails(seoDetails?.SEOKeywords, portalSeoSetting?.ProductKeyword, publishProduct);
            publishProduct.SEOTitle = GetSeoDetails(seoDetails?.SEOTitle, portalSeoSetting?.ProductTitle, publishProduct);
            publishProduct.SEOUrl = seoDetails?.SEOUrl;
            publishProduct.SEOCode = seoDetails?.SEOCode;
            publishProduct.CanonicalURL = seoDetails?.CanonicalURL;
            publishProduct.RobotTagValue = seoDetails?.RobotTag;
        }

        //Get SEO according to portal default setting.
        protected static string GetSeoDetails(string actualSEOSettings, string siteConfigSEOSettings, PublishProductModel entity)
        {
            string seoDetailsText = actualSEOSettings;

            if (string.IsNullOrEmpty(actualSEOSettings) && !string.IsNullOrEmpty(siteConfigSEOSettings))
            {
                string seoDetails = siteConfigSEOSettings;
                seoDetails = seoDetails.Replace(ZnodeConstant.DefaultSEOName, entity.Name);
                seoDetails = seoDetails.Replace(ZnodeConstant.DefaultSEOSku, entity.SKU);
                seoDetails = seoDetails.Replace(ZnodeConstant.DefaultSEOProductNumber, entity.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.ProductCode)?.AttributeValues);
                seoDetails = seoDetails.Replace(ZnodeConstant.DefaultSEOBrand, entity.Attributes?.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.Brand)?.SelectValues?.FirstOrDefault()?.Value);
                seoDetailsText = seoDetails;
            }
            return seoDetailsText;
        }


        //Get promotional price of product if any promotion associated to it.
        protected virtual void GetPromotionalPrice(PublishProductModel publishProduct)
        {
            try
            {
                if (publishProduct?.Promotions?.Count > 0 && (PromotionOnDisplayPrice(publishProduct?.Promotions)))
                {
                    ZnodePricePromotionManager pricePromoManager = new ZnodePricePromotionManager();

                    decimal productPrice = HelperUtility.IsNotNull(publishProduct.SalesPrice) ? publishProduct.SalesPrice.GetValueOrDefault() : publishProduct.RetailPrice.GetValueOrDefault();

                    publishProduct.PromotionalPrice = pricePromoManager.PromotionalPrice(publishProduct.PublishProductId, productPrice);

                }
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetPromotionalPrice method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Get promotional price of product if any promotion associated to it.
        protected virtual void GetPromotionalPrice(SearchProductModel publishProduct)
        {
            if (publishProduct?.Promotions?.Count > 0)
            {
                if (publishProduct.Promotions.Any(x => x.PromotionType == "Amount Off Displayed Product Price" || x.PromotionType == "Percent Off Displayed Product Price"))
                {
                    ZnodePricePromotionManager pricePromoManager = new ZnodePricePromotionManager();

                    decimal productPrice = HelperUtility.IsNotNull(publishProduct.SalesPrice) ? publishProduct.SalesPrice.GetValueOrDefault() : publishProduct.RetailPrice.GetValueOrDefault();

                    publishProduct.PromotionalPrice = pricePromoManager.PromotionalPrice(publishProduct.ZnodeProductId, productPrice);
                }
            }
        }

        protected virtual decimal? GetPricingInfoFromList(string property, List<PriceSKUModel> pricingBySKU, string sku)
        {
            if (HelperUtility.IsNull(pricingBySKU))
                return null;

            switch (property)
            {
                case nameof(AssociatedProductsModel.RetailPrice):
                    return pricingBySKU.FirstOrDefault(x => string.Equals(x.SKU, sku, System.StringComparison.InvariantCultureIgnoreCase))?.RetailPrice;
                case nameof(AssociatedProductsModel.SalesPrice):
                    return pricingBySKU.FirstOrDefault(x => string.Equals(x.SKU, sku, System.StringComparison.InvariantCultureIgnoreCase))?.SalesPrice;
                default:
                    return default(decimal);
            }
        }

        protected virtual string GetCurrencyInfoFromList(string property, List<PriceSKUModel> pricingBySKU, string sku)
        {
            if (HelperUtility.IsNull(pricingBySKU))
                return null;

            switch (property)
            {
                case nameof(AssociatedProductsModel.CurrencyCode):
                    return pricingBySKU.FirstOrDefault(x => string.Equals(x.SKU, sku, StringComparison.InvariantCultureIgnoreCase))?.CurrencyCode;
                case nameof(AssociatedProductsModel.CurrencySuffix):
                    return pricingBySKU.FirstOrDefault(x => string.Equals(x.SKU, sku, StringComparison.InvariantCultureIgnoreCase))?.CurrencySuffix;
                default:
                    return default(string);
            }
       }

        protected virtual List<PriceSKUModel> GetPricingBySKUs(string skus, int portalId, int userId = 0, int profileId = 0)
        {
            IZnodeViewRepository<PriceSKUModel> skuPrice = new ZnodeViewRepository<PriceSKUModel>();
            skuPrice.SetParameter("@SKU", skus, ParameterDirection.Input, DbType.String);
            skuPrice.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);        
            skuPrice.SetParameter("@currentUtcDate", HelperUtility.GetDateTime().Date, ParameterDirection.Input, DbType.DateTime);
            skuPrice.SetParameter("@UserId", userId, ParameterDirection.Input, DbType.Int32);
            skuPrice.SetParameter("@ProfileId", profileId, ParameterDirection.Input, DbType.Int32);
            List<PriceSKUModel> priceSKUList = skuPrice.ExecuteStoredProcedureList("Znode_GetPublishProductPricingBySku @SKU,@PortalId,@currentUtcDate,@UserId,@ProfileId")?.ToList();

            ModifySKUPriceListDetails(priceSKUList);
            return priceSKUList;
        }

        protected virtual void GetAttributeValue(List<PublishedAttributeEntityModel> attributeEntity, ref ConcurrentDictionary<string, string> additionalAttributesDictionary)
        {
            string attributeValue = string.Empty;

            foreach (PublishedAttributeEntityModel attribute in attributeEntity)
            {
                attributeValue = !string.IsNullOrEmpty(attribute.AttributeValues) ? attribute.AttributeValues
                                                                                  : string.Join(",", attribute.SelectValues.Select(s => s.Code));

                if (!string.IsNullOrEmpty(attribute.AttributeCode))
                    additionalAttributesDictionary.TryAdd(attribute.AttributeCode, attributeValue);
            }
        }

        protected virtual ConcurrentDictionary<string, string> GetAdditionalAttributeValues(PublishedProductEntityModel p, string additionalAttributes, ConcurrentDictionary<string, string> additionalAttributesDictionary)
        {
            if (string.IsNullOrEmpty(additionalAttributes))
                return additionalAttributesDictionary;

            string[] additionalAttributeList = additionalAttributes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            List<PublishedAttributeEntityModel> attributeEntities = p.Attributes.Where(w => additionalAttributeList.Any(a => string.Equals(a.Trim(), w.AttributeCode, StringComparison.InvariantCultureIgnoreCase)))
                                                                  .ToList();

            GetAttributeValue(attributeEntities, ref additionalAttributesDictionary);

            return additionalAttributesDictionary;
        }

        //Get promotional price of product if any promotion associated to it.
        public virtual List<PublishedConfigurableProductEntityModel> GetConfigurableProductEntity(int productId, int? catalogVersionId)
        {

            try
            {
                List<PublishedConfigurableProductEntityModel> configEntity = _publishConfigurableProductEntity.Table.Where(x => x.ZnodeProductId == productId)?.ToModel<PublishedConfigurableProductEntityModel>()?.ToList();
                configEntity = catalogVersionId.HasValue && catalogVersionId.Value > 0 ? configEntity.Where(x => x.VersionId == catalogVersionId)?.ToList() : configEntity;


                return configEntity;
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetConfigurableProductEntity method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Get Configurable product entities.
        public virtual List<PublishedConfigurableProductEntityModel> GetConfigurableProductEntity(List<int> productIds, int? catalogVersionId)
        {

            try
            {
                List<PublishedConfigurableProductEntityModel> configEntity = _publishConfigurableProductEntity.Table.Where(x => productIds.Contains(x.ZnodeProductId))?.ToModel<PublishedConfigurableProductEntityModel>()?.ToList();
                configEntity = catalogVersionId.HasValue && catalogVersionId.Value > 0 ? configEntity.Where(x => x.VersionId == catalogVersionId)?.ToList() : configEntity;


                return configEntity;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetConfigurableProductEntity method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        public List<SEODetailsModel> GetSEODetailsForList(int portalId, string seoTypeName)
        {
            try
            {
                List<SEODetailsModel> znodeCMSSEODetails = (from seoDetail in new ZnodeRepository<ZnodeCMSSEODetail>().GetEntityList(string.Empty).ToList()
                                                            join seoType in new ZnodeRepository<ZnodeCMSSEOType>().GetEntityList(string.Empty).ToList() on seoDetail.CMSSEOTypeId equals seoType.CMSSEOTypeId
                                                            join seoDetailLocale in new ZnodeRepository<ZnodeCMSSEODetailLocale>().Table on seoDetail.CMSSEODetailId equals seoDetailLocale.CMSSEODetailId
                                                            where seoType.Name == seoTypeName && (portalId == 0 || seoDetail.PortalId == portalId)
                                                            select new SEODetailsModel
                                                            {
                                                                SEOId = seoDetail.SEOId,
                                                                SEODescription = seoDetailLocale.SEODescription,
                                                                SEOTitle = seoDetailLocale.SEOTitle,
                                                                SEOUrl = seoDetail.SEOUrl,
                                                                SEOKeywords = seoDetailLocale.SEOKeywords,
                                                                LocaleId = seoDetailLocale.LocaleId.GetValueOrDefault(),
                                                                CMSSEODetailId = seoDetail.CMSSEODetailId,
                                                                IsPublish = seoDetail.IsPublish,
                                                                PortalId = seoDetail.PortalId.Value,
                                                                SEOCode = seoDetail.SEOCode
                                                            }).ToList();

                return znodeCMSSEODetails;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetSEODetailsForList method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //checks if the SKU available or not
        private bool CheckIfSKUAvailable(List<InventorySKUModel> model, List<InventorySKUModel> inventoryModel)
        {
            bool isAvailable = false;
            if (HelperUtility.IsNotNull(model) || HelperUtility.IsNotNull(inventoryModel))
                isAvailable = false;

            foreach (InventorySKUModel invModel in inventoryModel)
            {
                InventorySKUModel nwModel = model.Where(x => x.SKU == invModel.SKU).Select(x => x)?.FirstOrDefault();
                if (HelperUtility.IsNotNull(nwModel) && nwModel.SKU.Equals(invModel.SKU))
                {
                    isAvailable = true;
                    break;
                }
                else
                    isAvailable = false;
            }

            return isAvailable;
        }

        //Get the real-time inventory combined to the znode inventory(if passed in the first parameter) using the list of coma separated sku passed in request parameter.
        private List<InventorySKUModel> RealTimeProductInventoryCall(List<InventorySKUModel> znodeInventoryModel, IEnumerable<string> realtimeProductSkus, int portalId)
        {
            if (znodeInventoryModel.Count == 0)
                znodeInventoryModel = new List<InventorySKUModel>();

            //Pass decorated InventoryModel if znodeInventoryModel is empty.
            //Generate list of product inventory.
            List<InventorySKUModel> realtimeProductInventoryModel = znodeInventoryModel.Any() ? znodeInventoryModel
                : realtimeProductSkus.Select(sku => new InventorySKUModel
                {
                    SKU = sku,
                    PortalId = portalId
                }).ToList();

            //Real time inventory call
            ERPInitializer<List<InventorySKUModel>> _erpInc = new ERPInitializer<List<InventorySKUModel>>(realtimeProductInventoryModel, "ProductInventory");
            List<InventorySKUModel> inventoryModel = (List<InventorySKUModel>)_erpInc.Result;//List of child product inventory 

            //Update quantity available in returning object by the inventory received from real-time call.
            foreach (string sku in realtimeProductSkus)
            {

                if (HelperUtility.IsNotNull(inventoryModel))
                {
                    //Real-time inventory model
                    InventorySKUModel internalModel = inventoryModel.Where(x => x.SKU == sku).Select(x => x)?.FirstOrDefault();
                    //Znode inventory model
                    InventorySKUModel updateModel = znodeInventoryModel.Where(x => x.SKU == sku).Select(x => x)?.FirstOrDefault();

                    if (HelperUtility.IsNotNull(updateModel) && HelperUtility.IsNotNull(internalModel) && CheckIfSKUAvailable(znodeInventoryModel, inventoryModel))
                        updateModel.Quantity = Convert.ToDecimal(inventoryModel?.FirstOrDefault()?.Quantity);
                    else if(HelperUtility.IsNotNull(internalModel))
                        znodeInventoryModel.Add(new InventorySKUModel
                        {
                            SKU = sku,
                            Quantity = HelperUtility.IsNotNull(internalModel) ? internalModel.Quantity : 0M,
                            ReOrderLevel = HelperUtility.IsNotNull(internalModel) ? internalModel.ReOrderLevel : 0M,
                            PortalId = portalId
                        });
                }
            }
            return znodeInventoryModel;
        }


        public void GetAssociatedProducts(List<PublishProductModel> publishProductList, int portalId, int userId, int? catalogVersionId = null)
        {
            publishProductList.ForEach(x =>
            {
                GetAssociatedProducts(x, portalId, userId, catalogVersionId);
            });
        }

        public void GetAssociatedProducts(PublishProductModel publishProduct, int portalId, int userId, int? catalogVersionId = null)
        {

            publishProduct.AssociatedProducts = new List<AssociatedProductsModel>();

            //Get Configurable Product

            List<PublishedConfigurableProductEntityModel> configEntity = _publishConfigurableProductEntity.Table.Where(x => x.ZnodeProductId == publishProduct.PublishProductId)?.ToModel<PublishedConfigurableProductEntityModel>()?.ToList();
            configEntity = catalogVersionId.HasValue && catalogVersionId.Value > 0 ? configEntity.Where(x => x.VersionId == catalogVersionId)?.ToList() : configEntity;

            if (!Equals(configEntity, null) && configEntity?.Count > 0)
            {
                //Get filter
                FilterCollection filters = GetConfigurableProductFilter(publishProduct.LocaleId, configEntity);

                if (catalogVersionId.HasValue && catalogVersionId.Value > 0)
                    filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.Equals, catalogVersionId.Value.ToString());

                List<PublishedProductEntityModel> products = GetProductList(filters)?.ToModel<PublishedProductEntityModel>()?.ToList();


                var _products = (from p in products
                                 select new AssociatedProductsModel
                                 {
                                     PublishProductId = p.ZnodeProductId,
                                     PimProductId = p.ZnodeProductId,
                                     SKU = p.SKU,
                                     OMSColorCode = GetAttributeValue(p, nameof(AssociatedProductsModel.OMSColorCode)),
                                 }).ToList();

                publishProduct.AssociatedProducts.AddRange(_products);
            }
        }

        private string GetAttributeValue(PublishedProductEntityModel productEntity, string attributeCode, bool isSelectAttribute = false)
        {
            return isSelectAttribute ? productEntity.Attributes.Where(w => string.Equals(w.AttributeCode, attributeCode, System.StringComparison.InvariantCultureIgnoreCase))
                                                               .Select(y => y.SelectValues.FirstOrDefault()?.SwatchText)
                                                               .FirstOrDefault()
                                     : productEntity.Attributes.Where(w => string.Equals(w.AttributeCode, attributeCode, System.StringComparison.InvariantCultureIgnoreCase))
                                                               .Select(y => y.AttributeValues)
                                                               .FirstOrDefault();
        }

        private string GetOMSSwatchTextAttributeValue(PublishedProductEntityModel productEntity, string attributeCode)
        {
            return productEntity.Attributes.Where(w => string.Equals(w.AttributeCode, attributeCode, System.StringComparison.InvariantCultureIgnoreCase))
                                            .Select(y => y.SelectValues.FirstOrDefault()?.SwatchText)
                                            .FirstOrDefault();
        }

        private string GetOMSCodeAttributeValue(PublishedProductEntityModel productEntity, string attributeCode)
        {
            return productEntity.Attributes.Where(w => string.Equals(w.AttributeCode, attributeCode, System.StringComparison.InvariantCultureIgnoreCase))
                                            .Select(y => y.SelectValues.FirstOrDefault()?.Code)
                                            .FirstOrDefault();
        }

        private string GetOMSValueTextAttributeValue(PublishedProductEntityModel productEntity, string attributeCode)
        {
            return productEntity.Attributes.Where(w => string.Equals(w.AttributeCode, attributeCode, System.StringComparison.InvariantCultureIgnoreCase))
                                            .Select(y => y.SelectValues.FirstOrDefault()?.Value)
                                            .FirstOrDefault();
        }

        private string GetOMSValuePathAttributeValue(PublishedProductEntityModel productEntity, string attributeCode)
        {
            return productEntity.Attributes.Where(w => string.Equals(w.AttributeCode, attributeCode, System.StringComparison.InvariantCultureIgnoreCase))
                                            .Select(y => y.SelectValues.FirstOrDefault()?.Path)
                                            .FirstOrDefault();
        }
        #endregion

        #region Private

        private List<ZnodePublishSeoEntity> GetPublishSEODetailsForList(int portalId, string seoType, int localeId, List<string> SKUs, int? catalogVersionId = null)
        {
            try
            {
                FilterCollection filters = new FilterCollection();
                filters.Add("PortalId", FilterOperators.Equals, portalId.ToString());
                filters.Add("SEOTypeName", FilterOperators.Is, seoType);
                filters.Add("LocaleId", FilterOperators.Equals, localeId.ToString());
                if (catalogVersionId.HasValue && catalogVersionId.Value > 0)
                    filters.Add("VersionId", FilterOperators.Equals, catalogVersionId.ToString());
                if (SKUs?.Count > 0)
                    filters.Add("SEOCode", FilterOperators.In, string.Join(",", SKUs.Select(x => $"\"{x}\"")) );

                List<ZnodePublishSeoEntity> publishSEOList = GetSEOSettings(filters);

                return publishSEOList;

            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetPublishSEODetailsForList method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        private ZnodePublishSeoEntity GetPublishSEODetails(string seoCode, string seoType, int portalId, int localeId, int? catalogVersionId = null)
        {

            FilterCollection filters = new FilterCollection();
            filters.Add("PortalId", FilterOperators.Equals, portalId.ToString());
            filters.Add("SEOTypeName", FilterOperators.Is, seoType);
            if (localeId > 0)
                filters.Add("LocaleId", FilterOperators.Equals, localeId.ToString());
            filters.Add("SEOCode", FilterOperators.Is, seoCode);

            if (catalogVersionId.HasValue && catalogVersionId.Value > 0)
                filters.Add("VersionId", FilterOperators.Equals, catalogVersionId.ToString());


            ZnodePublishSeoEntity publishSEOList = GetSEOSettings(filters).FirstOrDefault();

            return publishSEOList;
        }

        public virtual List<ZnodePublishSeoEntity> GetSEOSettings(FilterCollection filters)
        {
            IZnodeRepository<ZnodePublishSeoEntity> _publishSEOEntity = new ZnodeRepository<ZnodePublishSeoEntity>(HelperMethods.Context);
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollections());
            return _publishSEOEntity.GetEntityListWithoutOrderBy(whereClauseModel.WhereClause, whereClauseModel.FilterValues)?.ToList();
        }
        //to check promotion to be apply product on display price
        protected virtual bool PromotionOnDisplayPrice(List<ProductPromotionModel> promotions)
        {
            bool result = true;
            if (promotions?.Count > 0)
            {
                foreach (ProductPromotionModel promo in promotions)
                {
                    if (promo.PromotionType.Contains(ZnodeConstant.AmountOffProduct)
                        || promo.PromotionType.Contains(ZnodeConstant.PercentOffProduct)
                        || promo.PromotionType.Contains(ZnodeConstant.AmountOffXifYPurchased)
                        || promo.PromotionType.Contains(ZnodeConstant.PercentOffXifYPurchased))
                        return false;
                }
            }
            return result;
        }

        //Get Webstore version Id
        protected virtual int? GetWebstoreVersionId(PublishProductModel publishProduct, int portalId)
        {
            IZnodeRepository<ZnodePublishWebstoreEntity> _versionEntity = new ZnodeRepository<ZnodePublishWebstoreEntity>(HelperMethods.Context);
            ZnodePublishStatesEnum publishState = GetPortalPublishState();

            int? version = Convert.ToInt32(_versionEntity.Table.FirstOrDefault(x => x.PortalId == portalId && x.PublishState == publishState.ToString() && x.LocaleId == publishProduct.LocaleId)?.VersionId);

            return version.HasValue ? version : 0;

        }

        #endregion
        public virtual void GetAdditionalProductData(PublishProductModel publishProduct, List<string> expands, int localeId, int portalId, int userId)
        {
            PublishProductListModel productListModel = new PublishProductListModel { PublishProducts = new List<PublishProductModel>() };
            productListModel.PublishProducts.Add(publishProduct);
            DataTable productDetails = GetProductFiltersForSP(productListModel.PublishProducts);
            GetRequiredProductDetails(productListModel, productDetails, expands, localeId, userId, portalId);
        }

        //To Do need to move on a centralize place
        public virtual DataTable GetProductFiltersForSP(List<PublishProductModel> products)
        {
            DataTable table = new DataTable("ProductTable");
            DataColumn productId = new DataColumn("Id");
            productId.DataType = typeof(int);
            productId.AllowDBNull = false;
            table.Columns.Add(productId);
            table.Columns.Add("ProductType", typeof(string));
            table.Columns.Add("OutOfStockOptions", typeof(string));
            table.Columns.Add("SKU", typeof(string));

            foreach (PublishProductModel item in products)                            
                table.Rows.Add(item.PublishProductId, ValueFromSelectValue(item.Attributes, ZnodeConstant.ProductType), ValueFromSelectValue(item.Attributes, ZnodeConstant.OutOfStockOptions), item.SKU);

            return table;
        }

        public virtual DataTable GetProductFiltersForSP(List<SearchProductModel> products)
        {
            DataTable table = new DataTable("ProductTable");
            DataColumn productId = new DataColumn("Id");
            productId.DataType = typeof(int);
            productId.AllowDBNull = false;
            table.Columns.Add(productId);
            table.Columns.Add("ProductType", typeof(string));
            table.Columns.Add("OutOfStockOptions", typeof(string));
            table.Columns.Add("SKU", typeof(string));

            foreach (SearchProductModel item in products)
                table.Rows.Add(item.ZnodeProductId, ValueFromSelectValue(item.Attributes, ZnodeConstant.ProductType), ValueFromSelectValue(item.Attributes, ZnodeConstant.OutOfStockOptions), item.SKU);

            return table;
        }
        public string ValueFromSelectValue(List<PublishAttributeModel> attributes, string attributeCode)
            => attributes?.FirstOrDefault(x => x.AttributeCode == attributeCode)?.SelectValues?.FirstOrDefault()?.Code;
        //Get details of category products.
        protected virtual void GetRequiredProductDetails(PublishProductListModel publishProductListModel, DataTable tableDetails, List<string> expands, int localeId, int userId = 0, int portalId = 0, int omsOrderId = 0)
        {
            try
            {
                ExecuteSpHelper executeSpHelper = new ExecuteSpHelper();
                executeSpHelper.GetParameter("@PortalId", portalId, ParameterDirection.Input, SqlDbType.Int);
                executeSpHelper.GetParameter("@LocaleId", localeId, ParameterDirection.Input, SqlDbType.Int);
                executeSpHelper.GetParameter("@UserId", userId, ParameterDirection.Input, SqlDbType.Int);
                executeSpHelper.GetParameter("@currentUtcDate", HelperUtility.GetDateTime().Date, ParameterDirection.Input, SqlDbType.Text);
                executeSpHelper.GetParameter("@navigationProperties", string.Join(",", expands), ParameterDirection.Input, SqlDbType.Text);
                executeSpHelper.SetTableValueParameter("@ProductDetailsFromWebStore", tableDetails, ParameterDirection.Input, SqlDbType.Structured, "dbo.ProductDetailsFromWebStore");
                executeSpHelper.GetParameter("@OmsOrderId", omsOrderId, ParameterDirection.Input, SqlDbType.Int);
                executeSpHelper.GetParameter("@IsFetchPriceFromOrder", true, ParameterDirection.Input, SqlDbType.Bit);
                DataSet productDetails = executeSpHelper.GetSPResultInDataSet("Znode_GetInventoryPromotionPricingBySkuWrapper");

                //Bind product details.
                BindProductDetails(publishProductListModel, portalId, productDetails);



            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetRequiredProductDetails method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Get details of category products.
        protected virtual void GetRequiredProductDetails(List<SearchProductModel> searchResult, DataTable tableDetails, List<string> expands, int localeId, int userId = 0, int portalId = 0)
        {
            
            ExecuteSpHelper executeSpHelper = new ExecuteSpHelper();
            executeSpHelper.GetParameter("@PortalId", portalId, ParameterDirection.Input, SqlDbType.Int);
            executeSpHelper.GetParameter("@LocaleId", localeId, ParameterDirection.Input, SqlDbType.Int);
            executeSpHelper.GetParameter("@UserId", userId, ParameterDirection.Input, SqlDbType.Int);
            executeSpHelper.GetParameter("@currentUtcDate", HelperUtility.GetDateTime().Date, ParameterDirection.Input, SqlDbType.Text);
            executeSpHelper.GetParameter("@navigationProperties", string.Join(",", expands), ParameterDirection.Input, SqlDbType.Text);
            executeSpHelper.SetTableValueParameter("@ProductDetailsFromWebStore", tableDetails, ParameterDirection.Input, SqlDbType.Structured, "dbo.ProductDetailsFromWebStore");

            DataSet productDetails = executeSpHelper.GetSPResultInDataSet("Znode_GetInventoryPromotionPricingBySkuWrapper");



            //Bind product details.
            BindProductDetails(searchResult, portalId, productDetails);
        }
        private GlobalSettingDetail GetDefaultGlobalSettingData(string key)
        {
            GlobalSettingDetail data = new GlobalSettingDetail();

            DefaultGlobalConfigListModel globalSettings = Equals(HttpRuntime.Cache[CachedKeys.DefaultGlobalConfigCache], null)
               ? GetDefaultGlobalConfigSettings()
               : (DefaultGlobalConfigListModel)HttpRuntime.Cache.Get(CachedKeys.DefaultGlobalConfigCache);

            if (globalSettings?.DefaultGlobalConfigs.Count > 0)
            {
                int index = globalSettings.DefaultGlobalConfigs.FindIndex(item => Equals(item.FeatureName, key));
                if (index != -1)
                {
                    DefaultGlobalConfigModel model = globalSettings.DefaultGlobalConfigs[index];
                    data.FeatureValues = Convert.ToString(model.FeatureValues);
                    data.FeatureSubValues = GlobalSettingHelper.SetFeatureValue(model.FeatureSubValues);
                }
            }
            return data;
        }
        private DefaultGlobalConfigListModel GetDefaultGlobalConfigSettings()
        {
            try
            {
                IZnodeRepository<ZnodeGlobalSetting> _defaultGlobalConfigRepository = new ZnodeRepository<ZnodeGlobalSetting>();
                List<ZnodeGlobalSetting> defaultGlobalConfigList = new List<ZnodeGlobalSetting>(_defaultGlobalConfigRepository.Table);

                return HelperUtility.IsNotNull(defaultGlobalConfigList) ? new DefaultGlobalConfigListModel() { DefaultGlobalConfigs = defaultGlobalConfigList.ToModel<DefaultGlobalConfigModel>().ToList() } : null;

            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        //Bind product details.
        public virtual void BindProductDetails(List<SearchProductModel> searchResult, int portalId, IList<PublishCategoryProductDetailModel> productDetails)
        {
            searchResult?.ForEach(product =>
            {
                PublishCategoryProductDetailModel productSKU = productDetails?
                            .FirstOrDefault(productdata => productdata.SKU == product.SKU);

                if (HelperUtility.IsNotNull(productSKU))
                {
                    product.SalesPrice = productSKU.SalesPrice;
                    product.RetailPrice = productSKU.RetailPrice;
                    product.CurrencyCode = productSKU.CurrencyCode;
                    product.CultureCode = productSKU.CultureCode;
                    product.CurrencySuffix = productSKU.CurrencySuffix;
                    product.Quantity = productSKU.Quantity;
                    product.ReOrderLevel = productSKU.ReOrderLevel;
                    product.Rating = productSKU.Rating;
                    product.TotalReviews = productSKU.TotalReviews;
                }
                GetPromotionalPrice(product);
            });
        }

        public virtual void BindProductDetails(PublishProductListModel publishProductListModel, int portalId, IList<PublishCategoryProductDetailModel> productDetails)
        {
            ZnodePortal portalDetails = GetPortalDetailsById(portalId);
            string parentSKU = string.Empty;
            publishProductListModel?.PublishProducts?.ForEach(product =>
            {
                parentSKU = (!string.IsNullOrEmpty(product.ConfigurableProductSKU)) ? product.ConfigurableProductSKU : product.SKU;

                PublishCategoryProductDetailModel productSKU = productDetails?
                            .FirstOrDefault(productdata => productdata.SKU == parentSKU);

                if (HelperUtility.IsNotNull(productSKU))
                {
                    product.SalesPrice = productSKU.SalesPrice;
                    product.RetailPrice = productSKU.RetailPrice;
                    product.CurrencyCode = productSKU.CurrencyCode;
                    product.CultureCode = productSKU.CultureCode;
                    product.CurrencySuffix = productSKU.CurrencySuffix;
                    product.Quantity = productSKU.Quantity;
                    product.ReOrderLevel = productSKU.ReOrderLevel;
                    product.Rating = productSKU.Rating;
                    product.TotalReviews = productSKU.TotalReviews;
                    product.InStockMessage = portalDetails?.InStockMsg;
                    product.OutOfStockMessage = portalDetails?.OutOfStockMsg;
                    product.BackOrderMessage = portalDetails?.BackOrderMsg;
                    product.ProductType = product.Attributes.FirstOrDefault(y => y.AttributeCode == ZnodeConstant.ProductType)?.SelectValues.FirstOrDefault()?.Value;
                    product.IsActive = Convert.ToBoolean(product.Attributes.FirstOrDefault(y => y.AttributeCode == ZnodeConstant.IsActive)?.AttributeValues);

                    //Get tier price.
                    GetTierPriceData(product, productDetails);
                    GetPromotionalPrice(product);
                }
            });
        }

        
        protected virtual void BindProductDetails(PublishProductListModel publishProductListModel, int portalId, DataSet productDetails)
        {
            ZnodePortal portalDetails = GetPortalDetailsById(portalId);
            ConvertDataTableToList dt = new ConvertDataTableToList();
            List<ProductPromotionModel> promotionList = dt.ConvertDataTable<ProductPromotionModel>(productDetails?.Tables[0]);            
            List<PriceSKUModel> sKUPriceList = dt.ConvertDataTable<PriceSKUModel>(productDetails?.Tables[1]);
            List<InventorySKUModel> inventoryList = dt.ConvertDataTable<InventorySKUModel>(productDetails?.Tables[2]);

            ModifySKUPriceListDetails(sKUPriceList);

            string parentSKU = string.Empty;
            publishProductListModel?.PublishProducts?.ForEach(product =>
            {
                parentSKU = (!string.IsNullOrEmpty(product.ConfigurableProductSKU)) ? product.ConfigurableProductSKU : product.SKU;
                string associateProductSKu = product?.GroupProductSKUs?.Count > 0 ? product?.GroupProductSKUs.FirstOrDefault().Sku : product.ConfigurableProductSKU;
                List<string> skuList = new List<string> { product.SKU, associateProductSKu };
                List<PriceSKUModel> priceSKU = sKUPriceList?.Where(x => skuList.Contains(x.SKU)).ToList();

                if (priceSKU?.Count == 0)
                {
                    priceSKU = sKUPriceList?.Where(x => x.SKU == product.ParentPublishProductSKU).ToList();
                }
                InventorySKUModel inventory = inventoryList?.FirstOrDefault(x => x.SKU == parentSKU);

                product.Promotions = promotionList?.Where(x => x.PublishProductId == product.PublishProductId).ToList();
                if (HelperUtility.IsNotNull(inventory))
                {
                    product.Quantity = inventory?.Quantity;
                    product.ReOrderLevel = inventory?.ReOrderLevel;
                }              

                if (priceSKU?.Count > 0)
                {
                    PriceSKUModel priceData = null;
                    PriceSKUModel parentProductPrice = priceSKU.FirstOrDefault(x => x.SKU.Equals(parentSKU, StringComparison.InvariantCultureIgnoreCase));
                    if (HelperUtility.IsNotNull(parentProductPrice))
                    {
                        priceData = parentProductPrice;
                        priceSKU.RemoveAll(x => !(x.SKU.Equals(parentSKU, StringComparison.InvariantCultureIgnoreCase)));
                    }
                    else
                        priceData = priceSKU.FirstOrDefault();

                    product.SalesPrice = priceData.SalesPrice;
                    product.RetailPrice = priceData.RetailPrice;
                    product.CurrencyCode = priceData.CurrencyCode;
                    product.CultureCode = priceData.CultureCode;
                    product.CurrencySuffix = priceData.CurrencySuffix;
                                 
                    GetTierPriceData(product, priceSKU);
                    GetPromotionalPrice(product);
                }
            });
        }
              

        //Bind product details.
        public virtual void BindProductDetails(List<SearchProductModel> searchResult, int portalId, DataSet productDetails)
        {
            ConvertDataTableToList dt = new ConvertDataTableToList();
            List<ProductPromotionModel> promotionList = dt.ConvertDataTable<ProductPromotionModel>(productDetails?.Tables[0]);
            List<PriceSKUModel> sKUPriceList = dt.ConvertDataTable<PriceSKUModel>(productDetails?.Tables[1]);
            List<InventorySKUModel> inventoryList = dt.ConvertDataTable<InventorySKUModel>(productDetails?.Tables[2]);

            ModifySKUPriceListDetails(sKUPriceList);

            searchResult?.ForEach(product =>
            {

                PriceSKUModel priceSKU = sKUPriceList?.FirstOrDefault(x => x.SKU == product.SKU);
                InventorySKUModel inventory = inventoryList?.FirstOrDefault(x => x.SKU == product.SKU);
                product.Promotions = promotionList?.Where(x => x.PublishProductId == product.ZnodeProductId).ToList();
               
                if (HelperUtility.IsNotNull(inventory))
                {
                    product.Quantity = inventory?.Quantity;
                    product.ReOrderLevel = inventory?.ReOrderLevel;
                }

                if (HelperUtility.IsNotNull(priceSKU))
                {
                    product.SalesPrice = priceSKU.SalesPrice;
                    product.RetailPrice = priceSKU.RetailPrice;
                    product.CultureCode = priceSKU.CultureCode;
                    product.CurrencyCode = priceSKU.CurrencyCode;
                    product.CurrencySuffix = priceSKU.CurrencySuffix;
                }
                GetPromotionalPrice(product);
            });
        }




        public virtual ZnodePortal GetPortalDetailsById(int portalId)
        {
            string cacheKey = $"PortalDeatails_{portalId}";
            ZnodePortal portalDetails = Equals(HttpRuntime.Cache[cacheKey], null)
               ? GetPortalDetailsByIdFromDB(portalId, cacheKey)
               : ((ZnodePortal)HttpRuntime.Cache.Get(cacheKey));
            return portalDetails;
        }
        protected virtual ZnodePortal GetPortalDetailsByIdFromDB(int portalId, string cacheKey)
        {
            try
            {
                IZnodeRepository<ZnodePortal> _portalRepository = new ZnodeRepository<ZnodePortal>();
                ZnodePortal znodePortal = _portalRepository.GetById(portalId);
                if (HelperUtility.IsNotNull(znodePortal))
                    HttpRuntime.Cache.Insert(cacheKey, znodePortal);
                return znodePortal;

            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetPortalDetailsByIdFromDB method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        public virtual List<PublishProductModel> GetAssociatedProducts(List<PublishProductModel> publishProductList, int portalId, int userId, int localeId, int? catalogVersionId = null)
        {
            int[] ids = publishProductList?.Select(x => x.PublishProductId)?.ToArray();
            List<AssociatedProductsModel> associatedProductList = GetAssociatedProducts(ids, portalId, userId, localeId, catalogVersionId);
            publishProductList.ForEach(x => x.AssociatedProducts = associatedProductList.Where(y => y.ParentPublishProductId == x.PublishProductId)?.ToList());
            return publishProductList;
        }

        public virtual List<SearchProductModel> GetAssociatedProducts(List<SearchProductModel> publishProductList, int portalId, int userId, int localeId, int? catalogVersionId = null)
        {
            int[] ids = publishProductList?.Select(x => x.ZnodeProductId)?.ToArray();
            List<AssociatedProductsModel> associatedProductList = GetAssociatedProducts(ids, portalId, userId, localeId, catalogVersionId);
            publishProductList.ForEach(x => x.AssociatedProducts = associatedProductList.Where(y => y.ParentPublishProductId == x.ZnodeProductId)?.ToList());
            return publishProductList;
        }

        public virtual List<AssociatedProductsModel> GetAssociatedProducts(int[] publishProductIds, int portalId, int userId, int localeId, int? catalogVersionId = 0)
        {

            try
            {
                List<AssociatedProductsModel> model;
                IZnodeViewRepository<AssociatedProductsModel> storeProcedure = new ZnodeViewRepository<AssociatedProductsModel>();
                storeProcedure.SetParameter("@CatalogVersionId", catalogVersionId, ParameterDirection.Input, DbType.Int32);
                storeProcedure.SetParameter("@PublishProductIds", string.Join(",", publishProductIds), ParameterDirection.Input, DbType.String);
                storeProcedure.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
                storeProcedure.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);
                storeProcedure.SetParameter("@UserId", userId, ParameterDirection.Input, DbType.Int32);
                model = storeProcedure.ExecuteStoredProcedureList("Znode_GetPublishedAssociateConfigurableProducts @CatalogVersionId,@PublishProductIds,@PortalId,@LocaleId,@UserId")?.ToList();

                return model?.Count > 0 ? model : new List<AssociatedProductsModel>();
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in GetAssociatedProducts method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        protected virtual void GetAdditionalProductData(PublishProductListModel productListModel, List<string> expands, int localeId, int portalId, int userId, int omsOrderId = 0)
        {
            DataTable productDetails = GetProductFiltersForSP(productListModel.PublishProducts);
            GetRequiredProductDetails(productListModel, productDetails, expands, localeId, userId, portalId, omsOrderId);
        }

        // Published product price details shown on respective modules of Admin and Webstore application can be modified by this method.
        public virtual void ModifySKUPriceListDetails(List<PriceSKUModel> priceSKUList)
        {
            // customization.
        }

        // Published product tier price details shown on respective modules of Admin and Webstore application can be modified by this method.
        public virtual void ModifyTierPriceListDetails(List<PriceTierModel> tierPriceList)
        {
            // customization.
        }
    }
}

