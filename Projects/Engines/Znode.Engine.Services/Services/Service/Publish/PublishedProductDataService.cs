using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Exceptions;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Services
{
    public class PublishedProductDataService : BaseService, IPublishedProductDataService
    {

        #region Private Variables

        private readonly IZnodeRepository<ZnodePublishProductEntity> _publishProductEntity;
        private readonly IZnodeRepository<ZnodePublishAddonEntity> _publishAddonEntity;
        private readonly IZnodeRepository<ZnodePublishGroupProductEntity> _publishGroupProductEntity;
        private readonly IZnodeRepository<ZnodePublishBundleProductEntity> _publishBundleProductEntity;
        private readonly IZnodeRepository<ZnodePublishConfigurableProductEntity> _publishConfigurableProductEntity;

        #endregion

        #region Constructor
        public PublishedProductDataService()
        {

            _publishProductEntity = new ZnodeRepository<ZnodePublishProductEntity>(HelperMethods.Context);
            _publishAddonEntity = new ZnodeRepository<ZnodePublishAddonEntity>(HelperMethods.Context);
            _publishGroupProductEntity = new ZnodeRepository<ZnodePublishGroupProductEntity>(HelperMethods.Context);
            _publishBundleProductEntity = new ZnodeRepository<ZnodePublishBundleProductEntity>(HelperMethods.Context);
            _publishConfigurableProductEntity = new ZnodeRepository<ZnodePublishConfigurableProductEntity>(HelperMethods.Context);

        }
        #endregion

        //Get Published Products based on pageListModel
        public virtual List<ZnodePublishProductEntity> GetPublishProducts(PageListModel pageListModel)
        {
            if(string.IsNullOrEmpty(pageListModel?.OrderBy))
                return  _publishProductEntity.GetEntityListWithoutOrderBy(pageListModel.EntityWhereClause.WhereClause, null, pageListModel.EntityWhereClause.FilterValues)?.ToList();

            else
                return _publishProductEntity.GetEntityList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues)?.ToList();

        }

        //Gets Published products by filters
        public virtual ZnodePublishProductEntity GetPublishProductByFilters(FilterCollection filters)
        {
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollections());
            return _publishProductEntity.GetEntity(whereClauseModel.WhereClause, whereClauseModel.FilterValues);
        }

        public virtual List<ZnodePublishProductEntity> GetPublishProductsPageList(PageListModel pageListModel, out int rowCount)
            => _publishProductEntity.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out rowCount)?.ToList();


        //Gets published product list by Ids
        public virtual List<ZnodePublishProductEntity> GetPublishProductListByIds(List<int> productIds)
            => _publishProductEntity.Table.Where(x => productIds.Contains(x.ZnodeProductId))?.ToList();


        //Get Published Product Based on Parameters
        public virtual ZnodePublishProductEntity GetPublishProduct(int publishProductId, int portalId, int localeId, int? catalogVersionId = 0)
          => _publishProductEntity.Table.FirstOrDefault(x => x.ZnodeProductId == publishProductId && x.LocaleId == localeId && x.VersionId == catalogVersionId);


        //Get Products Bt Category Id
        public virtual List<ZnodePublishProductEntity> GetProductsByCategoryId(int categoryId, int localeId)
            => _publishProductEntity.Table.Where(x => x.LocaleId == localeId && x.ZnodeCategoryIds == categoryId)?.ToList();

        //Get Product Name by SKU
        public virtual string GetProductNameBySKU(string sku, int localeId, int catalogVersionId)
        {
            if (!string.IsNullOrEmpty(sku) && localeId > 0 && catalogVersionId > 0)
                return _publishProductEntity.Table.FirstOrDefault(x => x.LocaleId == localeId && x.VersionId == catalogVersionId && x.SKU == sku)?.Name;

            return string.Empty;
        }

        //Get Published Product based on SKU
        public virtual ZnodePublishProductEntity GetPublishProductBySKU(string sku, int publishedCatalogId, int localeId, int? catalogVersionId = 0, int omsOrderId = 0)
        {
            if (!string.IsNullOrEmpty(sku))
            {
                FilterCollection filters = new FilterCollection();
                filters.Add("SKULower", FilterOperators.Is, sku.ToLower());
                filters.Add("ZnodeCatalogId", FilterOperators.Equals, publishedCatalogId.ToString());
                filters.Add("LocaleId", FilterOperators.Equals, localeId.ToString());
                filters.Add("VersionId", FilterOperators.Equals, catalogVersionId.ToString());
                if (omsOrderId < 1)
                    filters.Add("IsActive", FilterOperators.Equals, ZnodeConstant.TrueValue);

                PageListModel pageListModel = new PageListModel(filters, null, null);

                return GetPublishProducts(pageListModel).FirstOrDefault();
            }
            return null;
        }

        //Get Published Product based on comma separated SKU
        public virtual List<ZnodePublishProductEntity> GetPublishProductBySKUs(List<string> sku, int publishedCatalogId, int localeId, int? catalogVersionId = 0)
        {
            FilterCollection filters = new FilterCollection();
            if (sku?.Count > 0)
                filters.Add(FilterKeys.SKU, FilterOperators.In, string.Join(",", sku.Select(x => $"\"{x}\"")));
            filters.Add(FilterKeys.LocaleId, FilterOperators.Equals, localeId.ToString());
            filters.Add(FilterKeys.ZnodeCatalogId, FilterOperators.Equals, publishedCatalogId.ToString());
            if (catalogVersionId.HasValue && catalogVersionId.Value > 0)
                filters.Add(FilterKeys.VersionId, FilterOperators.Equals, catalogVersionId.Value.ToString());

            PageListModel pageListModel = new PageListModel(filters, null, null);

            return sku?.Count > 0 ? GetPublishProducts(pageListModel) : new List<ZnodePublishProductEntity>();
        }

        //Get Published Add on Products
        public virtual List<ZnodePublishAddonEntity> GetPublishAddonProducts(PageListModel pageListModel)
            => _publishAddonEntity.GetEntityList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues).ToList();

        //Get published bundle products
        public virtual List<ZnodePublishBundleProductEntity> GetPublishedBundleProduct(PageListModel pageListModel)
        {
            if (string.IsNullOrEmpty(pageListModel?.OrderBy))
                return _publishBundleProductEntity.GetEntityListWithoutOrderBy(pageListModel.EntityWhereClause.WhereClause, null, pageListModel.EntityWhereClause.FilterValues).ToList();
            
            else
              return _publishBundleProductEntity.GetEntityList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues).ToList();

        }

        //Get published Configurable products
        public virtual List<ZnodePublishConfigurableProductEntity> GetPublishedConfigurableProducts(PageListModel pageListModel)
        {
            if (string.IsNullOrEmpty(pageListModel?.OrderBy))
                return _publishConfigurableProductEntity.GetEntityListWithoutOrderBy(pageListModel.EntityWhereClause.WhereClause, null, pageListModel.EntityWhereClause.FilterValues).ToList();

            else
               return  _publishConfigurableProductEntity.GetEntityList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues).ToList();

        }

        //Get published group products
        public virtual List<ZnodePublishGroupProductEntity> GetPublishedGroupProducts(PageListModel pageListModel)
        {
            if (string.IsNullOrEmpty(pageListModel?.OrderBy))
                return _publishGroupProductEntity.GetEntityListWithoutOrderBy(pageListModel.EntityWhereClause.WhereClause, null, pageListModel.EntityWhereClause.FilterValues).ToList();

            else
                return _publishGroupProductEntity.GetEntityList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues).ToList();
        }

        //Get Associated Products
        public virtual List<ZnodePublishProductEntity> GetAssociatedConfigurableProducts(int publishProductId, int localeId, int? catalogVersionId)
          => (from configEntity in _publishConfigurableProductEntity.Table
              join productEntity in _publishProductEntity.Table
              on configEntity.AssociatedZnodeProductId equals productEntity.ZnodeProductId
              where configEntity.ZnodeProductId == publishProductId &&
              configEntity.VersionId == catalogVersionId &&
              productEntity.VersionId == catalogVersionId
              select productEntity)?.ToList();

        //Get Associated Bundle Products
        public virtual List<ZnodePublishProductEntity> GetAssociatedBundleProducts(int publishProductId, int localeId, int? catalogVersionId)
          => (from bundleProductEntity in _publishBundleProductEntity.Table
              join productEntity in _publishProductEntity.Table
              on bundleProductEntity.AssociatedZnodeProductId equals productEntity.ZnodeProductId
              where bundleProductEntity.ZnodeProductId == publishProductId &&
              bundleProductEntity.VersionId == catalogVersionId &&
              productEntity.VersionId == catalogVersionId
              select productEntity).ToList();

        //Get Product Count
        public virtual int GetPublishProductCount(int localeId, int versionId, int catalogId)
            => (from productEntity in _publishProductEntity.Table
                where productEntity.VersionId == versionId && 
                productEntity.LocaleId == localeId &&
                productEntity.ZnodeCatalogId == catalogId && 
                productEntity.IsActive select productEntity).Count();

        //Call update store procedure to update associated & linked products data
        public virtual void UpdatePublishedProductAssociatedData()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            try
            {
                IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
                IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_PublishAssociatedProduct");
                if (!deleteResult.FirstOrDefault().Status.Value)
                {
                    ZnodeLogging.LogMessage("Failed to update associated products of publish products", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                }
                ZnodeLogging.LogMessage("Successfully update associated products of publish products", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.GenericExceptionDuringPublish, ex.Message);
            }
        }
    }
}
