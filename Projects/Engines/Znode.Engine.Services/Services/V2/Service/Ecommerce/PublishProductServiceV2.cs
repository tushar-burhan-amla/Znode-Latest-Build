using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.V2;
using Znode.Libraries.Admin;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;
using Znode.Libraries.Data.DataModel;

namespace Znode.Engine.Services
{
    public class PublishProductServiceV2 : PublishProductService, IPublishProductServiceV2
    {
        #region Private Variables
        private readonly IPublishProductHelper _publishProductHelper;
        private readonly IPublishedProductDataService publishProductData;
        #endregion

        #region Constructor
        public PublishProductServiceV2()
        {
            _publishProductHelper = ZnodeDependencyResolver.GetService<IPublishProductHelper>();
            publishProductData = ZnodeDependencyResolver.GetService<IPublishedProductDataService>();
        }
        #endregion

        #region Public Methods
        public virtual PublishProductListModel GetPublishProductsByAttribute(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            int catalogId, portalId, localeId, userId;
            GetParametersValueForFilters(filters, out catalogId, out portalId, out localeId);
            ZnodeLogging.LogMessage("catalogId, portalId and localeId: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { catalogId, portalId, localeId });

            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.UserId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out userId);

            filters.RemoveAll(x => x.FilterName == FilterKeys.PortalId);
            filters.RemoveAll(x => x.FilterName == FilterKeys.UserId);

            //get catalog current version id by catalog id.
            if (catalogId > 0)
                filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.Equals, Convert.ToString(GetCatalogVersionId(catalogId)));
            else
                filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.In, GetCatalogAllVersionIds());

            if (userId > 0 && EnableProfileBasedSearch(portalId))
            {
                int? userProfileId = GetUserProfileId(userId);
                filters.Add(WebStoreEnum.ProfileIds.ToString(), FilterOperators.In, Convert.ToString(userProfileId));
            }

            //Replace filter keys with published filter keys
            ReplaceFilterKeys(ref filters);

            if (HelperUtility.IsNotNull(sorts))
                ReplaceSortKeys(ref sorts);

            //Check if products are taken for some specific category.
            SetProductIndexFilter(filters);

            ModifyFiltersForAttributes(ref filters);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            //get publish products 
    
            List<PublishProductModel> publishProducts = publishProductData.GetPublishProducts(new PageListModel(filters, null, null))?.ToModel<PublishProductModel>()?.ToList();

           PublishProductListModel publishProductListModel = new PublishProductListModel
            {
                PublishProducts = FilterProductsByAttribute(publishProducts, filters.Where(x => x.FilterName.Contains(ZnodeConstant.PublishedAttribute)).Select(y => y).ToList())
            };

            publishProductListModel.PublishProducts = Equals(pageListModel.PagingLength, int.MaxValue) ? publishProductListModel.PublishProducts.ToList()
                                                                                                       : publishProductListModel.PublishProducts.Take(pageListModel.PagingLength).ToList();

            if (publishProductListModel?.PublishProducts?.Count > 0)
                _publishProductHelper.GetDataFromExpands(portalId, GetExpands(expands), publishProductListModel, localeId, GetLoginUserId(), GetProfileId());

            if (HelperUtility.IsNotNull(expands[ZnodeConstant.Pricing]))
            {
                foreach (PublishProductModel product in publishProductListModel.PublishProducts)
                {
                    string productType = product.Attributes.SelectAttributeList(ZnodeConstant.ProductType)?.FirstOrDefault()?.Code;
                    BindAssociatedProductDetails(product, product.PublishProductId, product.PublishedCatalogId, portalId, localeId, productType);
                }
            }

            ZnodeLogging.LogMessage("PublishProducts and locale list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { publishProductListModel?.PublishProducts, publishProductListModel?.Locale });
            publishProductListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return publishProductListModel;
        }

        //Get price for products through ajax async call. 
        public virtual ProductInventoryPriceListModel GetProductPriceV2(ParameterInventoryPriceModel parameter)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNotNull(parameter))
            {
                switch (parameter.ProductType)
                {
                    //Get price for group products.
                    case ZnodeConstant.GroupedProduct:
                        PageListModel pageListModel = GetPageModel(parameter.Parameter, parameter.CatalogId);
                        List<PublishedProductEntityModel> productList = ZnodeDependencyResolver.GetService<IPublishedProductDataService>().GetPublishProducts(pageListModel)?.ToModel<PublishedProductEntityModel>()?.ToList();

                        ZnodeLogging.LogMessage("Products list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, productList?.Count);
                        if (productList?.Count > 0)
                            return GetAsyncGroupProductPrice(productList, parameter.LocaleId, parameter.PortalId);
                        break;
                    //Get price for configurable products.
                    case ZnodeConstant.ConfigurableProduct:
                        PageListModel pageModel = GetPageModel(parameter.Parameter, parameter.CatalogId);
                        List<ZnodePublishProductEntity> products = ZnodeDependencyResolver.GetService<IPublishedProductDataService>().GetPublishProducts(pageModel);

                        ZnodeLogging.LogMessage("Products list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, products?.Count);

                        if (products?.Count > 0)
                            return GetAsyncConfigurableProductPrice(products, parameter.CatalogId, parameter.LocaleId, parameter.PortalId);
                        break;
                    //Get price for Simple and Bundle products.
                    default:
                        return new ProductInventoryPriceListModel { ProductList = MapProductPrice(_publishProductHelper.GetPricingBySKUs(parameter.Parameter.Split(','), parameter.PortalId > 0 ? parameter.PortalId : PortalId, GetLoginUserId(), GetProfileId())) };
                }
            }

            return new ProductInventoryPriceListModel();
        }

        public virtual PublishProductModelV2 GetPublishProductV2(int publishProductId, FilterCollection filters, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            int catalogId, portalId, localeId;
            GetParametersValueForFilters(filters, out catalogId, out portalId, out localeId);
            filters.RemoveAll(x => x.FilterName == FilterKeys.PortalId);

            //Replace filter keys.
            ReplaceFilterKeys(ref filters);

            filters.Add(WebStoreEnum.ZnodeProductId.ToString(), FilterOperators.Equals, publishProductId.ToString());

            PublishProductModel publishProduct = null;
            //Get publish product 
            ZnodeLogging.LogMessage("publishProductId to get published product list : ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, publishProductId.ToString());
            List<ZnodePublishProductEntity> products = ZnodeDependencyResolver.GetService<IPublishedProductDataService>().GetPublishProducts(new PageListModel(filters, null, null));

            ZnodeLogging.LogMessage("Published products list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, products?.Count);
            foreach (ZnodePublishProductEntity product in products ?? new List<ZnodePublishProductEntity>())
            {
                publishProduct = product?.ToModel<PublishProductModel>();
            }

            if (HelperUtility.IsNotNull(publishProduct))
            {
                ZnodeLogging.LogMessage("catalogId, portalId, localeId to get data from expands: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { catalogId, portalId, localeId });
                _publishProductHelper.GetDataFromExpands(portalId, GetExpands(expands), publishProduct, localeId, WhereClauseForPortalId(portalId), GetLoginUserId(), GetCatalogVersionId(catalogId).GetValueOrDefault(), null, GetProfileId());
                GetProductImagePath(portalId, publishProduct);
                //set stored based In Stock, Out Of Stock, Back Order Message.
                SetPortalBasedDetails(portalId, publishProduct);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return publishProduct.ToEntity<PublishProductModelV2>();
        }

        //Get the list of published products for V2 Apis
        public virtual PublishProductListModel GetPublishProductListV2(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page, string requiredAttrFilter = "")
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            int catalogId, portalId, localeId, userId;
            GetParametersValueForFilters(filters, out catalogId, out portalId, out localeId);
            ZnodeLogging.LogMessage("catalogId, portalId and localeId: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { catalogId, portalId, localeId });

            int.TryParse(filters.Where(x => x.FilterName.Equals(FilterKeys.UserId, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault()?.FilterValue, out userId);

            filters.RemoveAll(x => x.FilterName == FilterKeys.PortalId);
            filters.RemoveAll(x => x.FilterName == FilterKeys.UserId);

            //get catalog current version id by catalog id.
            if (catalogId > 0)
                filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.Equals, Convert.ToString(GetCatalogVersionId(catalogId)));
            else
            {
                string versionIds = GetCatalogAllVersionIds();
                if(!string.IsNullOrEmpty(versionIds))
                    filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.In, versionIds);

            }

            if (userId > 0 && EnableProfileBasedSearch(portalId))
            {
                int? userProfileId = GetUserProfileId(userId);
                filters.Add(WebStoreEnum.ProfileIds.ToString(), FilterOperators.In, Convert.ToString(userProfileId));
            }
            //Get filter value
            string filterValue = filters.FirstOrDefault(x => x.FilterName.ToLower() == FilterKeys.AttributeValuesForPromotion.ToString().ToLower() && x.FilterOperator == FilterOperators.In)?.FilterValue;

            if (!string.IsNullOrEmpty(filterValue))
            {
                //Remove Payment Setting Filters with IN operator from filters list
                filters.RemoveAll(x => x.FilterName.ToLower() == FilterKeys.AttributeValuesForPromotion.ToString().ToLower() && x.FilterOperator == FilterOperators.In);

                //Add Payment Setting Filters
                filters.Add(FilterKeys.AttributeValuesForPromotion, FilterOperators.In, filterValue.Replace('_', ','));
            }

            if (filters.Any(w => w.FilterName.ToLower() == FilterKeys.fromOrder.ToString().ToLower()))
            {
                //Remove ZnodeCategoryId Filters from filters list
                filters.RemoveAll(x => x.FilterName.ToLower() == FilterKeys.fromOrder.ToString().ToLower());
                string activeCategories = GetActiveCategoryIds(catalogId);
                if(!string.IsNullOrEmpty(activeCategories))
                    filters.Add(FilterKeys.ZnodeCategoryId, FilterOperators.In, activeCategories);
            }
            //Replace filter keys with published filter keys
            ReplaceFilterKeys(ref filters);

            if (HelperUtility.IsNotNull(sorts))
                ReplaceSortKeys(ref sorts);

            //Check if products are taken for some specific category.
            SetProductIndexFilter(filters);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            //get publish products 
            ZnodeLogging.LogMessage("pageListModel to get publish products list : ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            List<ZnodePublishProductEntity> publishProducts = ZnodeDependencyResolver.GetService<IPublishedProductDataService>().GetPublishProducts(pageListModel);

            PublishProductListModel publishProductListModel = new PublishProductListModel()
            {
                PublishProducts = Equals(pageListModel.PagingLength, int.MaxValue) ? publishProducts.ToModel<PublishProductModel>().ToList()
                                                                                   : publishProducts.Take(pageListModel.PagingLength).ToModel<PublishProductModel>().ToList()
            };

            if (!string.IsNullOrEmpty(requiredAttrFilter))
            {
                string[] requiredAttributesArr = requiredAttrFilter.ToLower().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (PublishProductModel item in publishProductListModel.PublishProducts)
                {
                    item.Attributes = item.Attributes.Where(x => requiredAttributesArr.Any(y => y == x.AttributeCode.ToLower())).ToList();
                }
            }

            if (publishProductListModel?.PublishProducts?.Count > 0)
                _publishProductHelper.GetDataFromExpands(portalId, GetExpands(expands), publishProductListModel, localeId, GetLoginUserId(), GetProfileId());

            if (!Equals(expands[ZnodeConstant.Pricing], null))
            {
                foreach (PublishProductModel product in publishProductListModel.PublishProducts)
                {
                    string productType = product.Attributes.SelectAttributeList(ZnodeConstant.ProductType)?.FirstOrDefault()?.Code;
                    BindAssociatedProductDetails(product, product.PublishProductId, product.PublishedCatalogId, portalId, localeId, productType);
                }
            }
            publishProductListModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("PublishProducts and locale list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { publishProductListModel?.PublishProducts, publishProductListModel?.Locale });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return publishProductListModel;
        }

        public virtual PublishProductModel GetPublishProductBySkuV2(ParameterProductModel model, NameValueCollection expands, FilterCollection filters)
        {
            int catalogId, portalId, localeId;

            GetParametersValueForFilters(filters, out catalogId, out portalId, out localeId);

            string configurableProductSKU = model.ParentProductSKU;

            //get catalog current version id by catalog id.
            int? catalogVersionId = GetCatalogVersionId(catalogId, localeId);

            PublishProductModel publishProduct = ZnodeDependencyResolver.GetService<IPublishedProductDataService>().GetPublishProductBySKU(model.SKU, catalogId, localeId, catalogVersionId)?.ToModel<PublishProductModel>();

            if (model.ParentProductId > 0 && !string.IsNullOrEmpty(configurableProductSKU) && HelperUtility.IsNotNull(publishProduct))
            {
                publishProduct.ConfigurableProductSKU = model.SKU;
                publishProduct.ConfigurableProductId = model.ParentProductId;
                publishProduct.SKU = configurableProductSKU;
            }
            else if (HelperUtility.IsNull(publishProduct))
            {
                return null;
            }

            //Get expands associated to Product
            _publishProductHelper.GetDataFromExpands(portalId, GetExpands(expands), publishProduct, localeId, "", GetLoginUserId());

            publishProduct.AssociatedProducts = publishProduct.AssociatedProducts.DistinctBy(x => x.SKU).ToList();

            return publishProduct;
        }

        #endregion

        #region Private Methods

        public virtual void ModifyFiltersForAttributes(ref FilterCollection filters)
        {
            foreach (FilterTuple tuple in filters)
                if (tuple.Item1.Contains("Attributes")) ReplaceFilterKeyName(ref filters, tuple.Item1, "Attributes");
        }

        private List<PublishProductModel> FilterProductsByAttribute(List<PublishProductModel> publishedProducts, IList<FilterTuple> attribFilters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if ((attribFilters.Count % 2).Equals(0))
            {
                for (int i = 0, j = 1; i < attribFilters.Count(); i += 2, j += 2)
                {
                    if (publishedProducts?.Count > 0)
                    {
                        publishedProducts = publishedProducts.Where(x => x.Attributes
                                                                          .Any(y => y.AttributeCode.IndexOf(attribFilters[i].FilterValue, StringComparison.InvariantCultureIgnoreCase) >= 0
                                                                                    && ValueFilter(y, attribFilters[j].FilterValue, attribFilters[j].FilterOperator)))
                                                                          .Select(z => z).ToList();
                    }
                }
            }

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return publishedProducts;
        }

        private bool ValueFilter(PublishAttributeModel model, string filterValue, string filterOperator)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            bool result = false;

            if (model.SelectValues.Count > 0)
            {
                switch (model.AttributeTypeName)
                {
                    case "Simple Select":
                        result = model.SelectValues.Any(x => string.Equals(x.Code, filterValue, StringComparison.InvariantCultureIgnoreCase));
                        break;
                    case "Multi Select":
                        string[] filterValues = filterValue.Split(new char[] { ';' });
                        foreach (string filter in filterValues)
                        {
                            if ((filterOperator == FilterOperators.Contains && model.SelectValues.Any(x => string.Equals(x.Code, filter, StringComparison.InvariantCultureIgnoreCase))
                                 || (filterOperator == FilterOperators.NotContains && !model.SelectValues.Any(x => string.Equals(x.Code, filter, StringComparison.InvariantCultureIgnoreCase)))))
                                return true;
                        }

                        result = false;
                        break;
                    default:
                        result = false;
                        break;
                }
            }
            else
                result = model.AttributeValues.IndexOf(filterValue, StringComparison.InvariantCultureIgnoreCase) >= 0;

            return result;
        }
        #endregion
    }
}