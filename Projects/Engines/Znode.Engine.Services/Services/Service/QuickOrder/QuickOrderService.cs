using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Newtonsoft.Json;
using System.Collections.Specialized;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.ShoppingCart;


namespace Znode.Engine.Services
{
    public class QuickOrderService : BaseService, IQuickOrderService
    {
        //This method return products based on parameter values
        public virtual QuickOrderProductListModel GetQuickOrderProductList(QuickOrderParameterModel parameters, FilterCollection filters)
        {
            QuickOrderProductListModel model = new QuickOrderProductListModel();
            int catalogId, portalId, localeId;
            GetParametersValueForFilters(filters, out catalogId,out portalId, out localeId);
            int catalogVersionId = GetCatalogVersionId(catalogId, localeId).GetValueOrDefault();
            ZnodeLogging.LogMessage("Parameter:", string.Empty, TraceLevel.Verbose, new object[] { catalogVersionId = catalogVersionId, catalogId = catalogId });

            model.Products = GetQuickOrderProductDetails(portalId, catalogId, localeId, parameters.SKUs, catalogVersionId);

            IZnodeShoppingCart znodeShoppingCarts = GetService<IZnodeShoppingCart>();
            List<AssociatedPublishedBundleProductModel> bundleProductList = znodeShoppingCarts.BindBundleProductChildByParentSku(parameters.SKUs, catalogId, localeId);
            ZnodeLogging.LogMessage("QuickOrderProducts list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, model.Products?.Count);

            model.Products?.ForEach(quickProduct =>
            {
                BindQuickOrderProductModel(quickProduct);
                quickProduct.PublishBundleProducts = bundleProductList?.Where(d => d.ParentBundleSKU == quickProduct.SKU).ToList();
            });

            return model;
        }

        //This method return random quick order product basic details
        public virtual QuickOrderProductListModel GetDummyQuickOrderProductList(FilterCollection filters, NameValueCollection page)
        {
            IZnodeRepository<ZnodePublishProductEntity>  publishProductEntity = new ZnodeRepository<ZnodePublishProductEntity>(HelperMethods.Context);
            QuickOrderProductListModel model = new QuickOrderProductListModel();
            int catalogId, localeId, portalId;
            GetParametersValueForFilters(filters, out catalogId,out portalId, out localeId);
            int catalogVersionId = GetCatalogVersionId(catalogId, localeId).GetValueOrDefault();
            int pagingStart = 1;
            int pagingLength = 10;
            SetPaging(page, out pagingStart, out pagingLength);
            ZnodeLogging.LogMessage("Parameter:", string.Empty, TraceLevel.Verbose, new object[] { catalogVersionId = catalogVersionId, catalogId = catalogId });
            model.Products = publishProductEntity.Table.Where(m => m.ZnodeCatalogId == catalogId && m.LocaleId == localeId && m.VersionId == catalogVersionId && m.IsActive && m.ZnodeCategoryIds != 0 && m.ProductIndex == ZnodeConstant.DefaultPublishProductIndex).OrderBy(p => p.Name)?.Skip(pagingStart)?.Take(pagingLength)?.Select(m => new QuickOrderProductModel() { Id = m.ZnodeProductId, SKU = m.SKU, Name = m.Name })?.ToList();
            return model;
        }

        //Get details of quick order products.
        protected virtual List<QuickOrderProductModel> GetQuickOrderProductDetails(int portalId, int catalogId, int localeId, string skus, int versionId)
        {
            string activeCategoryList = GetActiveCategoryIds(catalogId);
            int productIndex = ZnodeConstant.DefaultPublishProductIndex;
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter localeId:", string.Empty, TraceLevel.Info, new object[] { localeId });
            IZnodeViewRepository<QuickOrderProductModel> objStoredProc = new ZnodeViewRepository<QuickOrderProductModel>();
            objStoredProc.SetParameter("@PublishCatalogId", catalogId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PublishCategoryIds", activeCategoryList, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@SKUs", skus, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@VersionId", versionId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@ProductIndex", productIndex, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            IList<QuickOrderProductModel> productDetails = objStoredProc.ExecuteStoredProcedureList("Znode_GetQuickOrderPublishProduct @PublishCatalogId,@LocaleId,@PublishCategoryIds,@SKUs,@VersionId,@ProductIndex,@PortalId");
            return productDetails?.ToList();
        }

        //bind quick order model from product attribute
        protected virtual void BindQuickOrderProductModel(QuickOrderProductModel product)
        {
            List<PublishAttributeModel> attributeModels = string.IsNullOrEmpty(product.Attributes) ? null : JsonConvert.DeserializeObject<List<PublishAttributeModel>>(product.Attributes);
            if (attributeModels?.Count > 0)
            {
                decimal quantity = 0;                
                bool? isCallForPricing = false;
                if (!Convert.ToBoolean(attributeModels.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.CallForPricing)?.AttributeValues))
                    isCallForPricing = product.HasPromotion;
                else
                    isCallForPricing = Convert.ToBoolean(attributeModels.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.CallForPricing)?.AttributeValues);
                product.ProductType = attributeModels.SelectAttributeList(ZnodeConstant.ProductType)?.FirstOrDefault()?.Code;
                product.IsCallForPricing = isCallForPricing;//product.Attributes.Where(x => x.AttributeCode == ZnodeConstant.CallForPricing
                product.TrackInventory = attributeModels.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.OutOfStockOptions)?.AttributeValues;
                product.OutOfStockMessage = string.IsNullOrEmpty(product.OutOfStockMessage) ? WebStore_Resources.TextOutofStock : product.OutOfStockMessage;
                if (!decimal.TryParse(attributeModels.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.MaximumQuantity)?.AttributeValues, out quantity))
                {
                    quantity = 0;
                }
                product.MaxQuantity = quantity;
                if (!decimal.TryParse(attributeModels.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.MinimumQuantity)?.AttributeValues, out quantity))
                {
                    quantity = 0;
                }
                product.MinQuantity = quantity;
                product.IsPersonalizable = attributeModels.FirstOrDefault(x => x.IsPersonalizable)?.IsPersonalizable ?? false;
                if (product.ConfigurableProductSKUs?.Length > 0)
                {
                    string configurableSKU = product.ConfigurableProductSKUs;
                    product.ConfigurableProductSKUs = product.SKU;
                    product.SKU = configurableSKU;
                }
                product.InventoryCode = attributeModels.SelectAttributeList(ZnodeConstant.OutOfStockOptions)?.FirstOrDefault().Code;
                product.IsObsolete = attributeModels.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.IsObsolete)?.AttributeValues;
                product.Attributes = string.Empty;
            }
        }

        //Get parameter values from filters.
        protected virtual void GetParametersValueForFilters(FilterCollection filters, out int catalogId, out int portalId, out int localeId)
        {
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(WebStoreEnum.ZnodeCatalogId.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out catalogId);
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.PortalId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out portalId);
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.LocaleId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out localeId);
        }

        //Get active category ids.
        protected virtual string GetActiveCategoryIds(int catalogId)
        {
            ZnodeLogging.LogMessage("Input Parameter catalogId:", string.Empty, TraceLevel.Info, new object[] { catalogId });
            int versionId = GetCatalogVersionId(catalogId).GetValueOrDefault();
            List<PublishedCategoryEntityModel> publishedCategory = GetService<IPublishedCategoryDataService>().GetActiveCategoryListByCatalogId(catalogId, versionId)?.ToModel<PublishedCategoryEntityModel>()?.ToList();
            return string.Join(",", publishedCategory?.Select(s => s.ZnodeCategoryId)?.Distinct()?.ToArray());
        }        
    }
}
