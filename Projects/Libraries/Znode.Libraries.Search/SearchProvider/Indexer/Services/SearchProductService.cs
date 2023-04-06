using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.Search
{
    public class SearchProductService : ISearchProductService
    {
        #region Protected Variables

        protected readonly IZnodeRepository<ZnodePublishProductEntity> _publishProductEntity;
        protected readonly IZnodeRepository<ZnodePublishVersionEntity> _publishVersionEntity;
        protected readonly IZnodeRepository<ZnodePublishSearchProfileEntity> _publishSearchProfileEntity;
        protected readonly IZnodeRepository<ZnodeCatalogIndex> _catalogIndexEntity;

        #endregion Protected Variables

        #region Constructor

        public SearchProductService()
        {
            _publishSearchProfileEntity = new ZnodeRepository<ZnodePublishSearchProfileEntity>(GetPublishEntityContext());
            _publishProductEntity = new ZnodeRepository<ZnodePublishProductEntity>(GetPublishEntityContext());
            _publishVersionEntity = new ZnodeRepository<ZnodePublishVersionEntity>(HelperMethods.Context);
            _catalogIndexEntity = new ZnodeRepository<ZnodeCatalogIndex>();
        }
        #endregion Constructor

        #region Public Methods.

        public virtual List<SearchProduct> GetAllProducts(int catalogId, int versionId, IEnumerable<int> publishCategoryIds, int start, int pageLength, long indexStartTime, out decimal totalPages)
        {
            int totalCount = 0;

            FilterCollection filters = new FilterCollection();

            if (catalogId > 0)
                filters.Add(WebStoreEnum.ZnodeCatalogId.ToString(), FilterOperators.Equals, catalogId.ToString());

            filters.Add(WebStoreEnum.IsActive.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue);
            filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.Equals, versionId.ToString());
            filters.Add("ZnodeCategoryIds", FilterOperators.NotEquals, "0");

            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollections());

            IList<ZnodePublishProductEntity> publishProductEntityList = _publishProductEntity.GetPagedList(whereClauseModel.WhereClause, string.Empty, start + 1, pageLength, out totalCount);

            if (totalCount < pageLength)
                totalPages = 1;
            else
                totalPages = Math.Ceiling((decimal)totalCount / pageLength);

            //Perform actions on data to convert in into elastic model & bind required product data.
            List<SearchProduct> elasticProductList = PerformDataTransform(catalogId, publishCategoryIds, indexStartTime, publishProductEntityList);

            return elasticProductList;
        }

        //Get all products require for the elastic index process. 
        public virtual List<SearchProduct> GetAllProducts(int catalogId, int[] versionIds, IEnumerable<int> publishCategoryIds, long indexStartTime, int pageIndex, int pageSize,bool isPublishDraftProductsOnly)
        {
            try
            {
                ZnodeLogging.LogMessage("Get all product execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                ZnodeLogging.LogMessage("catalogId, versionIds, indexStartTime, pageIndex, pageSize : ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { catalogId, versionIds, indexStartTime, pageIndex, pageSize });

                int totalCount = 0;

                //Get filter for getting products data. #Step 1
                FilterCollection filters = GetProductFilters(catalogId, versionIds, isPublishDraftProductsOnly);

                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollections());
             
                //Get all published products data in chunks for elastic index having categoryid greater than 0 & isactive. #Step 2
                IList<ZnodePublishProductEntity> publishProductEntityList = _publishProductEntity.GetPagedList(whereClauseModel.WhereClause, string.Empty, pageIndex, pageSize, out totalCount);

                ZnodeLogging.LogMessage($"Publish product count of page index : {pageIndex} for elastic indexing : {publishProductEntityList?.Count}", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

                //Perform actions on data to convert in into elastic model & bind required product data.  #Step 3
                List<SearchProduct> elasticProductList = PerformDataTransform(catalogId, publishCategoryIds, indexStartTime, publishProductEntityList);

                ZnodeLogging.LogMessage("Get all product execution done", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

                return elasticProductList;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage($"In case Out of Memory exception occurs during create index process, check the PublishProductFetchChunkSize key value in web.config file appropriately as per publish data volume and respective server configuration. The server may not be able to fetch {pageSize} products data at single request", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error, ex);
                throw;
            }
        }

        // To get the published search profile data against the publishCatalogId, if a custom profile is not available in the database then the default search profile will be returned.
        public virtual PublishSearchProfileModel GetPublishSearchProfile(int publishCatalogId)
        {
            List<ZnodePublishSearchProfileEntity> publishSearchProfile = _publishSearchProfileEntity.Table.Where(x => x.ZnodeCatalogId == publishCatalogId || x.ZnodeCatalogId == null).ToList();
            return publishSearchProfile?.FirstOrDefault(x => x.ZnodeCatalogId == publishCatalogId)?.ToModel<PublishSearchProfileModel>() ?? publishSearchProfile?.FirstOrDefault(x => x.ZnodeCatalogId == null)?.ToModel<PublishSearchProfileModel>();
        }

        public virtual void SetElasticProduct(IEnumerable<int> publishCategoyIds, long indexStartTime, List<SearchProduct> elasticProductList, List<PublishProductModel> productsWithReviewCount)
        {

            elasticProductList.ForEach(product =>
            {
                PublishProductModel productReviews = productsWithReviewCount?.FirstOrDefault(x => x.PublishProductId == product.znodeproductid);
                if (HelperUtility.IsNotNull(productReviews))
                {
                    product.rating = productReviews.Rating.GetValueOrDefault();
                    product.totalreviewcount = productReviews.TotalReviews.GetValueOrDefault();
                }


                //Sort by price to be work on sales price or retail price if sales price is null.
                decimal productPrice;
                if (!string.IsNullOrEmpty(product.salesprice))
                    decimal.TryParse(product.salesprice, out productPrice);
                else
                    decimal.TryParse(product.retailprice, out productPrice);

                product.productprice = productPrice;

                product.timestamp = indexStartTime;
            });

            if (publishCategoyIds?.Count() > 0)
                elasticProductList.RemoveAll(product => !publishCategoyIds.Contains(product.categoryid));
        }
       
        protected virtual List<SearchProduct> RemoveAttributes(List<ZnodePublishProductEntity> publishProductEntityList)
        {
            Dictionary<string, string> removableProductAttribute = JsonConvert.DeserializeObject<Dictionary<string, string>>(ZnodeApiSettings.RemovableProductAttributes);

            List<SearchProduct> productList = publishProductEntityList.ToModel<SearchProduct>().ToList();

            productList.RemoveAll(product => product.attributes.Any(attr => removableProductAttribute.ContainsKey(attr.attributecode)
                                                                                  && (attr.attributevalues == removableProductAttribute.Where(x => x.Key == attr.attributecode).Select(x => x.Value).FirstOrDefault())));

            //Attribute code to be exclude from removing.
            List<string> attributeCode = new List<string>();

            attributeCode.Add(ZnodeConstant.CallForPricing);
            attributeCode.Add(ZnodeConstant.OutOfStockOptions);
            attributeCode.Add(ZnodeConstant.ProductType);

            productList.ForEach(x => x.searchableattributes.AddRange(x.attributes.Where(y => attributeCode.Contains(y.attributecode) || y.isuseinsearch || y.isfacets).ToList()));

            //product which hideFromSearch is true need to be remove and not be add in search index.
            productList.RemoveAll(x => x.attributes.Any(y => y.attributecode.ToLower() == ZnodeConstant.HideFromSearch.ToLower() && y.attributevalues.ToLower() == ZnodeConstant.TrueValue.ToLower()));

            return productList;
        }
       
        public virtual List<SearchProduct> GetElasticProducts(List<ZnodePublishProductEntity> publishedProducts, long indexStartTime)
        {
            List<SearchProduct> elasticProductList = null;
            if (publishedProducts?.Count > 0)
            {
                elasticProductList = RemoveAttributes(publishedProducts);

                if (elasticProductList.Any())
                    SetElasticProduct(null, indexStartTime, elasticProductList, null);

                return elasticProductList;
            }
            else
                return elasticProductList;
        }


        public virtual int? GetCatalogVersionId(int catalogId)
           => _publishVersionEntity.Table.FirstOrDefault(x => x.ZnodeCatalogId == catalogId)?.VersionId;

        public virtual int GetVersionId(int publishCatalogId, string revisionType, int localeId = 0)
        {
            ZnodePublishVersionEntity publishVersionEntity = _publishVersionEntity.Table.Where(x =>
                                                   x.ZnodeCatalogId == publishCatalogId &&
                                                   x.RevisionType == revisionType &&
                                                   x.IsPublishSuccess == true &&
                                                   x.LocaleId == localeId)
                                                   .OrderByDescending(x => x.VersionId)
                                                   .FirstOrDefault();

            return publishVersionEntity != null ? publishVersionEntity.VersionId : 0;
       }


        //Get version id base on passed parameter for elastic search
        public virtual int GetLatestVersionId(int publishCatalogId, string revisionType, int localeId = 0)
        {
            return GetLatestVersionId(publishCatalogId, revisionType, new int[] { localeId }).FirstOrDefault();
        }

        //Get version id(s) base on passed parameter for elastic search
        public virtual int[] GetLatestVersionId(int publishCatalogId, string revisionType, int[] localeIds)
        {
            FilterCollection filters = new FilterCollection();

            filters.Add(WebStoreEnum.ZnodeCatalogId.ToString(), FilterOperators.Equals, publishCatalogId.ToString());
            filters.Add(WebStoreEnum.RevisionType.ToString(), FilterOperators.Is, revisionType);
            filters.Add(WebStoreEnum.LocaleId.ToString(), FilterOperators.In, string.Join(",", localeIds));

            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollections());

            IEnumerable<ZnodePublishVersionEntity> publishVersionEntity = _publishVersionEntity.GetEntityList(whereClauseModel.WhereClause, whereClauseModel.FilterValues)?.OrderByDescending(x => x.VersionId);

            return publishVersionEntity?.ToList()?.Count >  0 ? publishVersionEntity.GroupBy(x => x.LocaleId).Select(y => y.FirstOrDefault().VersionId).ToArray() : new int[] {0};
        }


        //Get all products count who's categoryid greater than 0 & isactive
        public virtual int GetAllProductCount(int publishCatalogId, int[] versionIds)
        {
            return (from ppe in _publishProductEntity.Table
                    where versionIds.Contains(ppe.VersionId) &&
                    ppe.ZnodeCatalogId == publishCatalogId &&
                    ppe.IsActive == true && ppe.ZnodeCategoryIds != 0
                    select ppe).Count();
        }

        // To get a record from ZnodeCatalogIndexTable.
        public virtual ZnodeCatalogIndex GetCatalogIndexByPublishCatalogId(int publishCatalogId)
            => _catalogIndexEntity.Table.FirstOrDefault(x => x.PublishCatalogId == publishCatalogId);

        // To update an entry in ZnodeCatalogIndex table.
        public virtual bool UpdateCatalogIndex(ZnodeCatalogIndex catalogIndex)
             => _catalogIndexEntity.Update(catalogIndex);

        //Get filter for getting product data
        protected virtual FilterCollection GetProductFilters(int catalogId, int[] versionIds, bool isPublishDraftProductsOnly)
        {
            FilterCollection filters = new FilterCollection();

            filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.In, string.Join(",", versionIds));
            filters.Add(WebStoreEnum.ZnodeCatalogId.ToString(), FilterOperators.Equals, catalogId.ToString());
            filters.Add(WebStoreEnum.IsActive.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue);
            filters.Add(WebStoreEnum.ZnodeCategoryIds.ToString(), FilterOperators.NotEquals, "0");
            if(isPublishDraftProductsOnly)
                filters.Add(ElasticEvent.ElasticSearchEvent.ToString(), FilterOperators.GreaterThan, "0");
            return filters;
        }

        //Perform actions on data to convert in into elastic model & bind required product data.
        protected virtual List<SearchProduct> PerformDataTransform(int catalogId, IEnumerable<int> publishCategoryIds, long indexStartTime, IList<ZnodePublishProductEntity> publishProductEntityList)
        {
            ZnodeLogging.LogMessage("Get all product execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            //Create flat structure model for the searchable attribute.
            List<SearchProduct> elasticProductList = RemoveAttributes(publishProductEntityList.AsEnumerable().ToList());

            ZnodeLogging.LogMessage($"elastic product count for elastic indexing : {elasticProductList?.Count}", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            if (elasticProductList?.Count > 0)
            {
                //Get the list of sku in the table format.
                DataTable dtSkuList = ConvertProductListToDataTable(elasticProductList);

                //Get products review to bind while adding product to elastic search.
                List<PublishProductModel> productsWithReviewCount = GetProductReviewCount(0, catalogId, 0, dtSkuList);
                SetElasticProduct(publishCategoryIds, indexStartTime, elasticProductList, productsWithReviewCount);
            }

            ZnodeLogging.LogMessage("Get all product execution done", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            return elasticProductList;
        }

        protected virtual List<PublishProductModel> GetProductReviewCount(int portalId, int catalogId, int localeId)
        {
            IZnodeViewRepository<PublishProductModel> objStoredProc = new ZnodeViewRepository<PublishProductModel>();
            objStoredProc.SetParameter("@SKU", "", ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@PublishCatalogId", catalogId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PublishProductId", "", ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);

            //Data on the basis of product skus and product ids
            return objStoredProc.ExecuteStoredProcedureList("Znode_GetProductDataForWebStore @SKU,@PublishCatalogId,@PublishProductId,@PortalId,@LocaleId").ToList();
        }

        //Get synonyms data for catalog.
        public virtual List<ZnodeSearchSynonym> GetSynonymsData(int catalogId)
        {
            IZnodeRepository<ZnodeSearchSynonym> _searchSynonymRepository = new ZnodeRepository<ZnodeSearchSynonym>();
            string whereClause = $"PublishCatalogId={catalogId}";
            List<ZnodeSearchSynonym> searchSynonymList = _searchSynonymRepository.GetEntityList(whereClause)?.ToList();
            return searchSynonymList;
        }

        public virtual List<ZnodeSearchKeywordsRedirect> GetKeywordsData(int catalogId)
        {
            IZnodeRepository<ZnodeSearchKeywordsRedirect> _searchKeywordsRepository = new ZnodeRepository<ZnodeSearchKeywordsRedirect>();
            string whereClause = $"PublishCatalogId={catalogId}";
            List<ZnodeSearchKeywordsRedirect> searchKeywordsList = _searchKeywordsRepository.GetEntityList(whereClause)?.ToList();
            return searchKeywordsList;
        }

        // This method is used to get the product list as per mentioned SKU
        protected virtual List<PublishProductModel> GetProductReviewCount(int portalId, int catalogId, int localeId, DataTable dtSkuList)
        {
            //The custom time is used for the Stored Procedure rather than default time out.
            int storedProcedureTimeout = ZnodeApiSettings.ConnectionTimeout;

            IZnodeViewRepository<PublishProductModel> objStoredProc = new ZnodeViewRepository<PublishProductModel>();
            objStoredProc.SetTableValueParameter("@SKU", dtSkuList, ParameterDirection.Input, SqlDbType.Structured, "dbo.SelectColumnList");
            objStoredProc.SetParameter("@PublishCatalogId", catalogId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PublishProductId", "", ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);

            //Data on the basis of product skus and product ids
            List<PublishProductModel> result = objStoredProc.ExecuteStoredProcedureList("Znode_GetProductDataForWebStore @SKU,@PublishCatalogId,@PublishProductId,@PortalId,@LocaleId", storedProcedureTimeout).ToList();
            return result;
        }

        //Convert comma separated column name to data table
        protected virtual DataTable ConvertProductListToDataTable(List<SearchProduct> productList)
        {
            DataTable columnNameTable = new DataTable();
            columnNameTable.Columns.Add("SKU");
            List<string> skuList = productList.Select(c => c.sku).ToList();
            foreach (string sku in skuList)
                columnNameTable.Rows.Add(sku);
            return columnNameTable;
        }

        //Get the database context with custom time out 
        protected virtual ZnodePublish_Entities GetPublishEntityContext()
        {
            //The custom time is used for the entity framework rather than default time out.
            int entityFrameworkTimeout = ZnodeApiSettings.ConnectionTimeout;

            ZnodePublish_Entities znodePublish_Entities = HelperMethods.Context;
            znodePublish_Entities.Database.CommandTimeout = entityFrameworkTimeout;

            return znodePublish_Entities;
        }
        #endregion
    }
}
