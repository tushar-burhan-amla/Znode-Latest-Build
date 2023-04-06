using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Libraries.Admin;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Libraries.Admin
{
    public class ZnodeProductFeedHelper : IZnodeProductFeedHelper
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodePortalCatalog> _portalCatalogRepository;
        private readonly IZnodeRepository<ZnodePublishProductEntity> _publishProductEntity;
        private readonly IZnodeRepository<ZnodePublishCategoryEntity> _publishCategoryEntity;



        private readonly IZnodeRssWriter rssWriter;
        private const string SEOURLProductType = "Product";
        private const string SEOURLContentPageType = "ContentPage";
        private const string SEOURLCategoryType = "Category";
        private const string LastModifier = "Use the database update date";
        #endregion

        #region Public Constructor
        public ZnodeProductFeedHelper()
        {
            _portalCatalogRepository = new ZnodeRepository<ZnodePortalCatalog>();
            _publishCategoryEntity = new ZnodeRepository<ZnodePublishCategoryEntity>(HelperMethods.Context);
            _publishProductEntity = new ZnodeRepository<ZnodePublishProductEntity>(HelperMethods.Context);

            rssWriter = GetService<IZnodeRssWriter>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the Product list
        /// For more adding tags for the product, please refere to  http://www.google.com/support/merchants/bin/answer.py?answer=188494
        /// </summary>
        /// <param name="portalId">Portal Id to get the product</param>
        /// <param name="changeFreq">Change Frequency</param>
        /// <param name="rootTag">Root tag for the XML</param>
        /// <param name="rootTagValue">Root tag value</param>
        /// <param name="xmlFileName"> XML File Name</param>
        /// <param name="lastModified">Last Modified date</param>
        /// <param name="localeId">locale id</param>
        /// <returns>Returns true if the XML file is created</returns>
        public virtual List<string> GetProductList(int portalId, string rootTagValue, string xmlFileName, string googleFeedTitle, string googleFeedLink, string googleFeedDesc, string feedType, int localeId)    
            => rssWriter.CreateGoogleSiteMap(GetProductFeedDataFromSQL(portalId, localeId, feedType), rootTagValue, xmlFileName, googleFeedTitle, googleFeedLink, googleFeedDesc);           
        
        /// <summary>
        /// Gets the Category List
        /// </summary>
        /// <param name="portalId">Portal Id to get category</param>
        /// <param name="model">Product Feed Model</param>
        /// <param name="rootTag">Root tag for the XML</param>
        /// <param name="rootTagValue">Root tag value</param>
        /// <returns>Returns the count of XML files created</returns>
        public virtual List<string> GetCategoryList(int portalId, ProductFeedModel productFeedModel, string rootTag, string rootTagValue)
            => rssWriter.CreateXMLSiteMap(GetCategoryFeedDataFromSQL(portalId, productFeedModel), rootTag, rootTagValue, productFeedModel.FileName);

        /// <summary>
        /// Gets the Content Page List
        /// </summary>
        /// <param name="portalId">Portal Id to get the contents</param>
        /// <param name="rootTag">Root tag for the XML</param>
        /// <param name="rootTagValue">Root tag value</param>
        /// <param name="model">Product Feed Model</param>
        /// <returns>Returns the count of XML files created</returns>
        public virtual List<string> GetContentPagesList(int portalId, string rootTag, string rootTagValue, ProductFeedModel model)
            => rssWriter.CreateXMLSiteMap(GetContentPageFeedData(portalId, model), rootTag, rootTagValue, model.FileName);

        /// <summary>
        /// Gets the list of all files generated
        /// </summary>
        /// <param name="model">Product Feed Model</param>
        /// <param name="portalId">Portal Id to get category</param>
        /// <param name="rootTag">Root tag for the XML</param>
        /// <param name="rootTagValue">Root tag value</param>
        /// <param name="feedType">Type of Feed</param>
        /// <returns>Returns the count of XML files created</returns>   
        public virtual List<string> GetAllList(ProductFeedModel model, int portalId, string rootTag, string rootTagValue, string feedType)
            => rssWriter.CreateXMLSiteMapForAll(GetAllFeedDataFromSQL(portalId, feedType, model), rootTag, rootTagValue, model.FileName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileNameCount"></param>
        /// <param name="txtXMLFileName"></param>
        /// <returns></returns>
        public virtual string GenerateGoogleSiteMapIndexFiles(int fileNameCount, string txtXMLFileName)
         => rssWriter.GenerateGoogleSiteMapIndexFiles(fileNameCount, txtXMLFileName);

        /// <summary>
        /// Gets the count of XML file generated for product
        /// </summary>
        /// <param name="portalId">Portal Id to get category</param>
        /// <param name="rootTagValue">Root tag value</param>
        /// <param name="feedType">Type of Feed</param>
        /// <param name="rootTag">Root tag for the XML</param>
        /// <param name="productFeedModel">Product Feed Model</param>
        /// <returns></returns>
        public virtual List<string> GetProductXMLList(int portalId, string rootTagValue, string feedType, string rootTag, ProductFeedModel productFeedModel)
            => rssWriter.CreateProductSiteMap(GetProductFeedDataFromSQL(portalId, productFeedModel.LocaleId, feedType), rootTag, rootTagValue, productFeedModel.FileName);

        #endregion

        #region Private Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="portalIds"></param>
        /// <param name="feedType"></param>
        /// <param name="localeId"></param>
        /// <returns></returns>
        protected virtual DataSet GetResultsFromSP(string spName, int portalId, string feedType, int localeId, bool forProductData = false, string commaSeperatedIds = "")
        {
            DataTable skuData = GetSeoCodeInDataTable(commaSeperatedIds);
            ExecuteSpHelper executeSpHelper = new ExecuteSpHelper();
            if (!forProductData)
            {
                executeSpHelper.GetParameter("@PortalID", portalId, ParameterDirection.Input, SqlDbType.NVarChar);
                executeSpHelper.GetParameter("@LocaleId", localeId, ParameterDirection.Input, SqlDbType.Int);
                if (!string.IsNullOrEmpty(feedType))
                    executeSpHelper.GetParameter("@FeedType", feedType, ParameterDirection.Input, SqlDbType.NVarChar);
                if (!string.IsNullOrEmpty(commaSeperatedIds))
                    executeSpHelper.GetParameter("@CommaSeparatedId", commaSeperatedIds, ParameterDirection.Input, SqlDbType.NVarChar);
            }
            else
            {
                executeSpHelper.GetParameter("@PortalId", portalId, ParameterDirection.Input, SqlDbType.NVarChar);
                executeSpHelper.SetTableValueParameter("@SKU", skuData, ParameterDirection.Input, SqlDbType.Structured, "dbo.SelectColumnList");
                executeSpHelper.GetParameter("@LocaleId", localeId, ParameterDirection.Input, SqlDbType.Int);
                executeSpHelper.GetParameter("@FeedType", feedType, ParameterDirection.Input, SqlDbType.NVarChar);
            }
            DataSet resultDataSet = executeSpHelper.GetSPResultInDataSet(spName);
            return resultDataSet.Tables.Count > 0 ? resultDataSet : null;
        }

        /// <summary>
        /// Get SEO Code in data table.
        /// </summary>
        /// <param name="commaSeperatedCodes"></param>
        /// <returns>Datatable with SKUs</returns>
        protected DataTable GetSeoCodeInDataTable(string commaSeperatedCodes)
        {
            DataTable table = new DataTable("SKU");
            table.Columns.Add("SKU", typeof(string));

            foreach (string model in commaSeperatedCodes?.Split(','))
                table.Rows.Add(model);

            return table;
        }

        /// <summary>
        /// This method will get the data from both Mongo and SQL.  Also it will convert the data in one DataSet. 
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <param name="localeId">Locale Id</param>
        /// <param name="feedType">Feed Type For Ex Google, Bing etc.</param>
        /// <returns>DataSet of combination of SQL and Published Data</returns>
        protected virtual DataSet GetProductFeedDataFromSQL(int portalId, int localeId, string feedType)
        {
            DataSet productDataFromSQL = null;
            DataSet finalMergedDataSet = null;
            List<ZnodePublishSeoEntity> SEOSettings = null;
            try
            {
                List<PublishedProductEntityModel> productData = null;
                string commaSeparatedProductIds = string.Empty;

                List<PublishedCategoryEntityModel> categoryData = null;
                string commaSeparatedCategoryIds = string.Empty;

                //Get the Published Catalog Ids from SQL on the basis of Portal and Locale
                string commaSeparatedCatalogIds = GetPublishedCatalogIDsFromSQL(portalId, localeId);

                //Get the category details 
                if (!string.IsNullOrEmpty(commaSeparatedCatalogIds))
                    categoryData = GetCategoryData(commaSeparatedCatalogIds, localeId);

                //Get the Category Ids 
                if (HelperUtility.IsNotNull(categoryData) && categoryData?.Count > 0)
                    commaSeparatedCategoryIds = string.Join(",", categoryData.Select(x => x.ZnodeCategoryId).ToList());

                string versionIds = GetVersionIds(commaSeparatedCatalogIds, localeId);
                //Get the product details 
                if (!string.IsNullOrEmpty(commaSeparatedCatalogIds))
                    productData = GetPublishedProductData(commaSeparatedCatalogIds, localeId, commaSeparatedCategoryIds);

                List<string> productCodes = productData.Select(h => h.SKU).Distinct().ToList();

                //Get the Product Ids 
                if (HelperUtility.IsNotNull(productData) && productData.Count > 0)
                    commaSeparatedProductIds = string.Join(",", productCodes);

                //Get the other details for ex. Price, Inventory etc from SQL for the selected product ids
                if (!string.IsNullOrEmpty(commaSeparatedProductIds))
                    productDataFromSQL = GetProductDataFromSQL(commaSeparatedProductIds, localeId, portalId, feedType);

                if (portalId > 0)
                    SEOSettings = GetSeoSettingList(localeId, portalId, ZnodeConstant.Product, productCodes, versionIds);

                //Merge the published data and return the merged data set for further processing
                if (HelperUtility.IsNotNull(productData) && productData.Count > 0 && HelperUtility.IsNotNull(productDataFromSQL) && HelperUtility.IsNotNull(productDataFromSQL.Tables)
                    && productDataFromSQL.Tables[0].Rows.Count > 0)
                    finalMergedDataSet = ConvertDataInDataSet(productDataFromSQL, productData, SEOSettings);
                return finalMergedDataSet;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return null;
            }
            finally
            {
                productDataFromSQL = null;
                finalMergedDataSet = null;
            }
        }

        private List<ZnodePublishSeoEntity> GetSeoSettingList(int localeId, int portalId, string seoType, List<string> commaSeparatedProductIds = null, string versionIds = null)
        {
            IZnodeRepository<ZnodePublishSeoEntity> _publishSEOEntity = new ZnodeRepository<ZnodePublishSeoEntity>(HelperMethods.Context);
           

            FilterCollection filters = new FilterCollection();
            if (portalId > 0)
                filters.Add("PortalId", FilterOperators.In, portalId.ToString());
            if (!string.IsNullOrEmpty(versionIds))
                filters.Add("VersionId", FilterOperators.In, versionIds);
            filters.Add("SEOTypeName", FilterOperators.Is, seoType);
            filters.Add("LocaleId", FilterOperators.Equals, localeId.ToString());

            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollections());
            List<ZnodePublishSeoEntity> publishSEOList =  _publishSEOEntity.GetEntityListWithoutOrderBy(whereClauseModel.WhereClause, whereClauseModel.FilterValues)?.ToList();

            if (commaSeparatedProductIds?.Count > 0)
                publishSEOList = publishSEOList.Where(x => commaSeparatedProductIds.Contains(x.SEOCode))?.ToList();

            if (publishSEOList?.Count > 0)
                return publishSEOList;

            return new List<ZnodePublishSeoEntity>();
        }

        /// <summary>
        /// This method will get the data from both Mongo and SQL.  Also it will convert the data in one DataSet. 
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <param name="localeId">Locale Id</param>
        /// <param name="changeFrequency">Change Frequency</param>
        /// <returns>Database of combination of SQL</returns>
        protected virtual DataSet GetCategoryFeedDataFromSQL(int portalId, ProductFeedModel productFeedModel)
        {
            //string portalId, int localeId, string changeFrequency
            DataSet categoryDataFromSQL = null;
            DataSet finalMergedDataSet = null;
            List<ZnodePublishSeoEntity> SEOSettings = null;
            try
            {
                List<PublishedCategoryEntityModel> categoryData = null;
                string commaSeparatedCategoryIds = string.Empty;

                //Get the Published Catalog Ids from SQL on the basis of Portal and Locale
                string commaSeparatedCatalogIds = GetPublishedCatalogIDsFromSQL(portalId, productFeedModel.LocaleId);

                //Get the category details
                if (!string.IsNullOrEmpty(commaSeparatedCatalogIds))
                    categoryData = GetCategoryData(commaSeparatedCatalogIds, productFeedModel.LocaleId);

                //Get te Category Ids 
                if (HelperUtility.IsNotNull(categoryData) && categoryData?.Count > 0)
                    commaSeparatedCategoryIds = string.Join(",", categoryData.Select(x => x.ZnodeCategoryId).ToList());

                string versionIds = GetVersionIds(commaSeparatedCatalogIds, productFeedModel.LocaleId);

                List<string> categoryCodes = categoryData.SelectMany(h => h.Attributes)
                                                            .Where(rt => rt.AttributeCode == "CategoryCode").Select(x => x.AttributeValues)
                                                            .Distinct().ToList();


                commaSeparatedCategoryIds = string.Join(",", categoryCodes);

                //Get the other details for ex. Category Name, id etc from SQL for the selected category ids
                if (!string.IsNullOrEmpty(commaSeparatedCategoryIds))
                    categoryDataFromSQL = GetCategoryDataFromSQL(commaSeparatedCategoryIds, portalId, productFeedModel.LocaleId);

                if (portalId > 0)
                    SEOSettings = GetSeoSettingList(productFeedModel.LocaleId, portalId, ZnodeConstant.Category, categoryCodes, versionIds);

                //Merge the data from SQL and return the merged data set for further processing
                if (HelperUtility.IsNotNull(categoryData) && categoryData?.Count > 0 && HelperUtility.IsNotNull(categoryDataFromSQL) && HelperUtility.IsNotNull(categoryDataFromSQL?.Tables)
                    && categoryDataFromSQL?.Tables[0]?.Rows.Count > 0)
                    finalMergedDataSet = ConvertDataInCategoryDataSet(categoryDataFromSQL, categoryData, productFeedModel.ChangeFreq, SEOSettings);

                finalMergedDataSet.Tables[0].TableName = "url";
                return finalMergedDataSet;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return null;
            }
            finally
            {
                categoryDataFromSQL = null;
                finalMergedDataSet = null;
            }
        }

        /// <summary>
        /// This method will gets the data for Content Pages
        /// </summary>
        /// <param name="portalIds">Portal Id to get the contents</param>
        /// <param name="model">Product Feed Model</param>
        /// <returns>Returns the dataset containing the Content Page Data</returns>
        protected virtual DataSet GetContentPageFeedData(int portalId, ProductFeedModel model)
        {
            DataSet datasetContentPagesList = null;
            List<ZnodePublishSeoEntity> seoSettings = null;
            try
            {
                datasetContentPagesList = GetResultsFromSP("Znode_GetContentFeedList", portalId, string.Empty, model.LocaleId);
                seoSettings = GetSeoSettingList(model.LocaleId, portalId, ZnodeConstant.ContentPage);
                if (HelperUtility.IsNotNull(datasetContentPagesList))
                {
                    AddColumns(ref datasetContentPagesList, model.ChangeFreq);

                    foreach (DataRow dr in datasetContentPagesList.Tables[0].Rows)
                    {
                        int contentPagePortalId = Convert.ToInt16(dr["PortalId"].ToString());
                        if (contentPagePortalId > 0 && string.IsNullOrEmpty(dr["DomainName"].ToString()))
                            break;
                        int id = Convert.ToInt32(dr["CMSContentPagesId"].ToString());

                        string seoUrl = seoSettings.FirstOrDefault(x => x.SEOId == id)?.SEOUrl;
                        string domainname = $"{HttpContext.Current.Request.Url.Scheme}://{dr["DomainName"].ToString()}";

                        dr["loc"] = string.IsNullOrEmpty(Convert.ToString(dr["loc"])) ? $"{domainname}{"/contentpage/"}{Convert.ToString(dr["CMSContentPagesId"])}"
      : $"{domainname}/{Convert.ToString(dr["loc"])}";
                    }
                    RemoveUnwantedColumnsFromDS(ref datasetContentPagesList);
                }
                return datasetContentPagesList;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return null;
            }
            finally
            {
                datasetContentPagesList = null;
            }
        }

        /// <summary>
        /// Get all the feed data from SQL for Product, Category and Content Pages
        /// </summary>
        /// <param name="portalId">Portal Id to get the data</param>
        /// <param name="feedType">Feed type</param>
        /// <param name="productFeedModel">Product Feed Model</param>
        /// <returns>Returns the dataset containing all the data</returns>
        protected virtual DataSet GetAllFeedDataFromSQL(int portalId, string feedType, ProductFeedModel productFeedModel)
        {
            DataSet dataSet = new DataSet();
            int count = 0;
            DataTable categoryData = GetCategoryFeedDataFromSQL(portalId, productFeedModel)?.Tables[0];
            if (categoryData != null)
            {
                dataSet.Tables.Add("Category");
                dataSet.Tables[count++].Merge(categoryData);
            }
            DataTable productData = GetProductFeedDataFromSQL(portalId, productFeedModel.LocaleId, feedType)?.Tables[0];
            if (productData != null)
            {
                dataSet.Tables.Add("Product");
                dataSet.Tables[count++].Merge(productData);
            }
            DataTable contentData = GetContentPageFeedData(portalId, productFeedModel)?.Tables[0];
            if (contentData != null)
            {
                dataSet.Tables.Add("Content");
                dataSet.Tables[count++].Merge(contentData);
            }
            return dataSet;
        }

        /// <summary>
        /// Get the published Products data 
        /// </summary>
        /// <param name="commaSeparatedProductIDs">Comma Separated Product Ids</param>
        /// <param name="localeId">Locale Id</param>
        /// <returns>List<ProductEntity></returns>
        protected virtual List<PublishedProductEntityModel> GetProductData(string commaSepCatalogIds, int localeId)
        {
            //Get version id 
            string versionIds = GetVersionIds(commaSepCatalogIds, localeId);

            FilterCollection productFilter = new FilterCollection();
            if(!string.IsNullOrEmpty(commaSepCatalogIds))
                productFilter.Add(FilterKeys.ZnodeCatalogId, FilterOperators.In, commaSepCatalogIds);
            productFilter.Add(FilterKeys.PublishedLocaleId, FilterOperators.Equals, Convert.ToString(localeId));
            productFilter.Add(FilterKeys.VersionId, FilterOperators.In, versionIds);
            productFilter.Add(FilterKeys.ProductIndex, FilterOperators.Equals, ZnodeConstant.DefaultPublishProductIndex.ToString());
            productFilter.Add(FilterKeys.PublishedIsActive, FilterOperators.Equals, ZnodeConstant.True);
            NameValueCollection sortCollection = new NameValueCollection();

            return GetProductList(productFilter, sortCollection);
        }

        //Get product list.
        protected virtual List<PublishedProductEntityModel> GetProductList(FilterCollection productFilter, NameValueCollection sortCollection)
        {
            List<PublishedProductEntityModel> productEntityList = new List<PublishedProductEntityModel>();
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(productFilter.ToFilterDataCollections());
            try
            {
                int totalRowCount = 0;
                int totalRecordCount = 0;

                _publishProductEntity.GetPagedList(whereClauseModel.WhereClause, DynamicClauseHelper.GenerateDynamicOrderByClause(sortCollection), 1, 10, out totalRecordCount);

                int chunkSize = 1000;
                int startIndex = 0;
                int totalRows = totalRecordCount;
                int totalRowsCount = totalRows / chunkSize;

                if (totalRows % chunkSize > 0)
                    totalRowsCount++;

                for (int iCount = 0; iCount < totalRowsCount; iCount++)
                {
                    List<PublishedProductEntityModel> publishProductEntityList = _publishProductEntity.GetPagedList(whereClauseModel.WhereClause, DynamicClauseHelper.GenerateDynamicOrderByClause(sortCollection), iCount + 1, chunkSize, out totalRowCount)?.ToModel<PublishedProductEntityModel>()?.ToList();

                    startIndex = startIndex + chunkSize;
                    productEntityList.AddRange(publishProductEntityList);
                }
            }
            catch (Exception ex)
            {
                productEntityList = new List<PublishedProductEntityModel>();
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return productEntityList;
        }

        /// <summary>
        /// Get the list of version ids from catalog ids
        /// </summary>
        /// <param name="commaSepCatalogIds">Catalog Ids</param>
        /// <returns>version ids</returns>
        protected virtual string GetVersionIds(string commaSepCatalogIds, int localeId = 0)
        {
            IPublishProductHelper publishProductHelper = GetService<IPublishProductHelper>();
            IZnodeRepository<ZnodePublishVersionEntity> _versionEntity = new ZnodeRepository<ZnodePublishVersionEntity>(HelperMethods.Context);

            FilterCollection filters = new FilterCollection();
            filters.Add(FilterKeys.ZnodeCatalogId, FilterOperators.In, commaSepCatalogIds);
            filters.Add(FilterKeys.PublishedLocaleId, FilterOperators.Equals, localeId.ToString());
            string revisionState = publishProductHelper.GetPortalPublishState().ToString();
            List<int> versionIds = _versionEntity.Table.Where(x => x.LocaleId == localeId && commaSepCatalogIds.Contains(x.ZnodeCatalogId.ToString()) && x.RevisionType == revisionState).Select(x => x.VersionId).ToList();
            return string.Join(",", versionIds);
        }

        /// <summary>
        /// Get the Category data 
        /// </summary>
        /// <param name="commaSepCatalogIds">Comma Separated Catalog Id</param>
        /// <param name="localeId">Locale Id</param>
        /// <returns>List<CategoryEntity></returns>
        protected virtual List<PublishedCategoryEntityModel> GetCategoryData(string commaSepCatalogIds, int localeId)
        {
            //Get version id 
            string versionIds = GetVersionIds(commaSepCatalogIds, localeId);

            FilterCollection categoryFilter = new FilterCollection();
            if(!string.IsNullOrEmpty(commaSepCatalogIds))
                categoryFilter.Add(FilterKeys.ZnodeCatalogId, FilterOperators.In, commaSepCatalogIds);
            categoryFilter.Add(FilterKeys.PublishedLocaleId, FilterOperators.Equals, Convert.ToString(localeId));
            categoryFilter.Add(FilterKeys.VersionId, FilterOperators.In, versionIds);
            NameValueCollection sortCollection = new NameValueCollection();
            sortCollection.Add(FilterKeys.ZnodeCategoryId, "asc");


            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(categoryFilter.ToFilterDataCollections());
            return _publishCategoryEntity.GetEntityList(whereClauseModel.WhereClause, DynamicClauseHelper.GenerateDynamicOrderByClause(sortCollection))?.ToModel<PublishedCategoryEntityModel>()?.ToList();
        }

        /// <summary>
        /// Get the Product Data from SQL
        /// </summary>
        /// <param name="commaSeparatedProductIds">Comma Separated Product Ids</param>
        /// <param name="localeId">Locale Id</param>
        /// <param name="portalIds">Portal Ids</param>
        /// <param name="feedType">Feed Type</param>
        /// <returns>DataSet</returns>
        protected virtual DataSet GetProductDataFromSQL(string commaSeparatedProductIds, int localeId, int portalId, string feedType)
            => GetResultsFromSP("Znode_GetProductFeedList", portalId, feedType, localeId, true, commaSeparatedProductIds);

        /// <summary>
        /// Get the Category Data from SQL
        /// </summary>
        /// <param name="commaSeparatedCategoryIds">Comma Separated Category Ids</param>
        /// <param name="localeId">Locale Id</param>
        /// <returns>DataSet</returns>
        protected virtual DataSet GetCategoryDataFromSQL(string commaSeparatedCategoryIds, int portalId, int localeId)
            => GetResultsFromSP("Znode_GetCategoryFeedList", portalId, string.Empty, localeId, false, commaSeparatedCategoryIds);

        /// <summary>
        /// Merge Published entities with DataSet
        /// </summary>
        /// <param name="productDataFromSQL">SQL Product Data</param>
        /// <param name="publishedProductData">Published Product Data</param>
        /// <returns>DataSet</returns>
        protected virtual DataSet ConvertDataInDataSet(DataSet productDataFromSQL, List<PublishedProductEntityModel> publishedProductData, List<ZnodePublishSeoEntity> seoSettings)
        {
            DataSet dsProduct = new DataSet();
            CreateDataSetColumns(ref dsProduct, SEOURLProductType);
            AddDataToDataSet(ref dsProduct, productDataFromSQL, publishedProductData, seoSettings);

            return dsProduct;
        }

        /// <summary>
        /// Merge Published entities with DataSet of Category
        /// </summary>
        /// <param name="categoryDataFromSQL">SQL Category Data</param>
        /// <param name="publishedCategoryData">Published Category Data</param>
        /// <param name="changeFrequency">Change Frequency</param>
        /// <returns>DataSet</returns>
        protected virtual DataSet ConvertDataInCategoryDataSet(DataSet categoryDataFromSQL, List<PublishedCategoryEntityModel> publishedCategoryData, string changeFrequency, List<ZnodePublishSeoEntity> seoEntities)
        {
            DataSet dsCategory = new DataSet();
            CreateDataSetColumns(ref dsCategory, SEOURLCategoryType);
            AddDataToCategoryDataSet(ref dsCategory, categoryDataFromSQL, publishedCategoryData, changeFrequency, seoEntities);

            return dsCategory;
        }

        /// <summary>
        /// This method will add the data in the Dataset for Product
        /// </summary>
        /// <param name="dsProduct">Product DataSet</param>
        /// <param name="productDataFromSQL">SQL Product DataSet</param>
        /// <param name="publishedProductData">Published Product Entity</param>
        protected virtual void AddDataToDataSet(ref DataSet dsProduct, DataSet productDataFromSQL, List<PublishedProductEntityModel> productData, List<ZnodePublishSeoEntity> seoSettings)
        {
            try
            {
                int loopCount = productData.Count;
            if (productData.Count > productDataFromSQL.Tables[0].Rows.Count)
                loopCount = productDataFromSQL.Tables[0].Rows.Count;

            //Merge the Product Data from SQL in one DataSet
            for (int iCount = 0; iCount < loopCount; iCount++)
            {
                int pid = Convert.ToInt16(productDataFromSQL.Tables[0].Rows[iCount]["PortalId"].ToString());
                if (pid > 0 && string.IsNullOrEmpty(productDataFromSQL.Tables[0].Rows[iCount]["DomainName"].ToString()))
                    break;
                int id = Convert.ToInt32(Convert.ToString(productDataFromSQL.Tables[0].Rows[iCount]["g:id"]));
                PublishedProductEntityModel publishedProduct = productData?.FirstOrDefault(x => x.ZnodeProductId == id);
                if (!Equals(publishedProduct, null))
                {
                    
                    string domainName = $"{HttpContext.Current.Request.Url.Scheme}://{Convert.ToString(productDataFromSQL.Tables[0].Rows[iCount]["DomainName"])}";

                    string seoUrl = seoSettings?.Where(x => x.SEOCode == publishedProduct.SKU && x.VersionId == publishedProduct.VersionId)?.Select(x => x.SEOUrl)?.FirstOrDefault();

                    DataRow dr = dsProduct.Tables[0].NewRow();

                    dr["loc"] = string.IsNullOrEmpty(seoUrl) ? $"{domainName}{"/product/"}{Convert.ToString(productDataFromSQL.Tables[0].Rows[iCount]["g:id"])}"
                        : $"{domainName}/{seoUrl}";
                    dr["title"] = publishedProduct.Name;
                    dr["g:condition"] = Convert.ToString(productDataFromSQL.Tables[0].Rows[iCount]["g:condition"]);
                    dr["description"] = publishedProduct.Attributes.Where(x => x.AttributeCode == ZnodeConstant.ShortDescription)?.FirstOrDefault()?.AttributeValues;
                    dr["g:id"] = Convert.ToString(productDataFromSQL.Tables[0].Rows[iCount]["g:id"]);
                    dr["g:image_link"] = $"{Convert.ToString(productDataFromSQL.Tables[0].Rows[iCount]["MediaConfiguration"])}{publishedProduct.Attributes.Where(x => x.AttributeCode == ZnodeConstant.ProductImage)?.FirstOrDefault()?.AttributeValues}";
                    dr["link"] = string.IsNullOrEmpty(Convert.ToString(productDataFromSQL.Tables[0].Rows[iCount]["loc"])) ? $"{domainName}{"/product/"}{Convert.ToString(productDataFromSQL.Tables[0].Rows[iCount]["g:id"])}"
                        : $"{domainName}/{Convert.ToString(productDataFromSQL.Tables[0].Rows[iCount]["loc"])}";
                    dr["g:price"] = Convert.ToString(productDataFromSQL.Tables[0].Rows[iCount]["g:price"]);
                    dr["g:identifier_exists"] = Convert.ToString(productDataFromSQL.Tables[0].Rows[iCount]["g:identifier_exists"]);
                    dr["g:availability"] = Convert.ToString(productDataFromSQL.Tables[0].Rows[iCount]["g:availability"]);
                    dsProduct.Tables[0].Rows.Add(dr);
                }
            }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
        }

        /// <summary>
        /// This method will add the data in the Dataset for Category
        /// </summary>
        /// <param name="dsCategory">Category DataSet</param>
        /// <param name="categoryDataFromSQL">SQL Category Data Set</param>
        /// <param name="publishedCategoryData">Published Category Entity</param>
        /// <param name="changeFrequency">Change Frequency</param>
        
        protected virtual void AddDataToCategoryDataSet(ref DataSet dsCategory, DataSet categoryDataFromSQL, List<PublishedCategoryEntityModel> publishedCategoryData, string changeFrequency, List<ZnodePublishSeoEntity> seoEntities)
        {
            //Merge the Product Data from SQL in one DataSet
            for (int iCount = 0; iCount < publishedCategoryData.Count; iCount++)
            {
                int catID = publishedCategoryData[iCount].ZnodeCategoryId;
                string seoCode = publishedCategoryData[iCount].Attributes.FirstOrDefault(x => x.AttributeCode == "CategoryCode")?.AttributeValues;
                string seoUrl = seoEntities.FirstOrDefault(x => x.SEOCode == seoCode)?.SEOUrl;

                var result =
        categoryDataFromSQL.Tables[0].AsEnumerable().Where(dr => dr.Field<int>("id") == catID);

                foreach (DataRow row in result)
                {
                    string domainName = $"{HttpContext.Current.Request.Url.Scheme}://{Convert.ToString(row["DomainName"])}";
                    DataRow dr = dsCategory.Tables[0].NewRow();
                    dr["loc"] = string.IsNullOrEmpty(seoUrl) ? $"{domainName}{"/category/"}{Convert.ToString(row["id"])}" : $"{domainName}/{seoUrl}";
                    dr["changefreq"] = changeFrequency;
                    dsCategory.Tables[0].Rows.Add(dr);
                }
            }
        }

        /// <summary>
        /// This method will create data set depending upon provided type
        /// </summary>
        /// <param name="dsDataSet">DataSet</param>
        /// <param name="type">Type</param>
        protected virtual void CreateDataSetColumns(ref DataSet dsDataSet, string type)
        {
            dsDataSet.Tables.Add(new DataTable());
            switch (type)
            {
                case SEOURLProductType:
                    dsDataSet.Tables[0].Columns.Add("loc", typeof(string));
                    dsDataSet.Tables[0].Columns.Add("title", typeof(string));
                    dsDataSet.Tables[0].Columns.Add("g:condition", typeof(string));
                    dsDataSet.Tables[0].Columns.Add("description", typeof(string));
                    dsDataSet.Tables[0].Columns.Add("g:id", typeof(string));
                    dsDataSet.Tables[0].Columns.Add("g:image_link", typeof(string));
                    dsDataSet.Tables[0].Columns.Add("link", typeof(string));
                    dsDataSet.Tables[0].Columns.Add("g:price", typeof(string));
                    dsDataSet.Tables[0].Columns.Add("g:identifier_exists", typeof(string));
                    dsDataSet.Tables[0].Columns.Add("g:availability", typeof(string));
                    break;
                case SEOURLCategoryType:                   
                case SEOURLContentPageType:
                    dsDataSet.Tables[0].Columns.Add("loc", typeof(string));
                    dsDataSet.Tables[0].Columns.Add("changefreq", typeof(string));
                    break;
            }
        }

        /// <summary>
        /// Get the Publish Catalog IDs on the basis of Portal and Locale
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <param name="localeId">Locale Id</param>
        /// <returns>Comma Separated Catalog Ids</returns>
        protected virtual string GetPublishedCatalogIDsFromSQL(int portalId, int localeId)
        {
            string whereClause = string.Empty;

            if (!portalId.Equals("0"))
            {
                FilterDataCollection filters = new FilterDataCollection();
                filters.Add(new FilterDataTuple(ZnodePortalEnum.PortalId.ToString(), FilterOperators.In, portalId.ToString()));
                whereClause = DynamicClauseHelper.GenerateDynamicWhereClause(filters);
            }

            List<ZnodePortalCatalog> portalCatalogs = _portalCatalogRepository.GetEntityList(whereClause)?.ToList();
            return string.Join(",", portalCatalogs?.Select(x => x.PublishCatalogId).Distinct());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="seoURL"></param>
        /// <returns></returns>
        protected virtual string MakeUrlForMvc(string id, string type, string seoURL)
        {
            var mapUrl = new StringBuilder();

            if (!string.IsNullOrEmpty(seoURL))
            {
                switch (type)
                {
                    case SEOURLProductType:
                        mapUrl.Append($"~/product/{seoURL}/{id}");
                        break;
                    case SEOURLCategoryType:
                        mapUrl.Append($"~/category/{seoURL}/{id}");
                        break;
                    case SEOURLContentPageType:
                        mapUrl.Append($"~/{seoURL}");
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case SEOURLProductType:
                        mapUrl.Append("~/product/" + id);
                        break;
                    case SEOURLCategoryType:
                        mapUrl.Append("~/category/" + id);
                        break;
                    case SEOURLContentPageType:
                        mapUrl.Append("~/content/" + id);
                        break;
                }
            }

            return mapUrl.ToString();
        }

        /// <summary>
        /// This method will remove the unwanted columns from ContentPage dataset
        /// </summary>
        /// <param name="datasetContentPagesList">DataSet</param>
        protected virtual void RemoveUnwantedColumnsFromDS(ref DataSet datasetContentPagesList)
        {
            datasetContentPagesList.Tables[0].Columns.Remove("Name");
            if (datasetContentPagesList.Tables[0].Columns.Contains("DomainName"))
                datasetContentPagesList.Tables[0].Columns.Remove("DomainName");
            if (datasetContentPagesList.Tables[0].Columns.Contains("PortalId"))
                datasetContentPagesList.Tables[0].Columns.Remove("PortalId");
            if (datasetContentPagesList.Tables[0].Columns.Contains("CMSContentPagesId"))
                datasetContentPagesList.Tables[0].Columns.Remove("CMSContentPagesId");
        }

        /// <summary>
        /// This method will add required columns in ContentPage dataset
        /// </summary>
        /// <param name="datasetContentPagesList">DataSet</param>
        /// <param name="changeFreq">Change Frequency</param>
        protected virtual void AddColumns(ref DataSet datasetContentPagesList, string changeFreq)
        {
            datasetContentPagesList.Tables[0].TableName = "url";
            DataColumn dataColumnChangeFreq = new DataColumn("changefreq");
            dataColumnChangeFreq.DefaultValue = changeFreq;

            datasetContentPagesList.Tables[0].Columns.Add(dataColumnChangeFreq);
        }

        /// <summary>
        /// Get the Products data 
        /// </summary>
        /// <param name="commaSeparatedCategoryIds">Comma Separated Category Ids</param>
        /// <param name="commaSeparatedProductIDs">Comma Separated Product Ids</param>
        /// <param name="localeId">Locale Id</param>
        /// <returns>List<ProductEntity></returns>
        protected virtual List<PublishedProductEntityModel> GetPublishedProductData(string commaSepCatalogIds, int localeId, string commaSeparatedCategoryIds)
        {
            //Get version id 
            string versionIds = GetVersionIds(commaSepCatalogIds, localeId);

            FilterCollection productFilter = new FilterCollection();
            productFilter.Add(FilterKeys.ZnodeCatalogId, FilterOperators.In, commaSepCatalogIds);
            productFilter.Add(FilterKeys.ZnodeCategoryIds, FilterOperators.In, commaSeparatedCategoryIds);
            productFilter.Add(FilterKeys.PublishedLocaleId, FilterOperators.Equals, Convert.ToString(localeId));
            productFilter.Add(FilterKeys.VersionId, FilterOperators.In, versionIds);
            productFilter.Add(FilterKeys.ProductIndex, FilterOperators.Equals, ZnodeConstant.DefaultPublishProductIndex.ToString());
            productFilter.Add(FilterKeys.PublishedIsActive, FilterOperators.Equals, ZnodeConstant.True);
            NameValueCollection sortCollection = new NameValueCollection();

            return GetProductList(productFilter, sortCollection);
        }

        #endregion
    }
}
