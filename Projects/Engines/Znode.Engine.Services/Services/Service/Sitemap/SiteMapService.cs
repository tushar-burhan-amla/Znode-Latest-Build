using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class SiteMapService : BaseService, ISiteMapService
    {
        #region Private Variables
        protected readonly ICategoryService _categoryService;
        #endregion Private Variables

        #region Constructor

        public SiteMapService()
        {
            _categoryService = GetService<ICategoryService>();
        }

        #endregion Constructor

        // This used to get the sitemap category list.
        public virtual SiteMapCategoryListModel GetSiteMapCategoryList(bool includeAssociatedCategories, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started to get Publish Brands.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);

            SiteMapCategoryListModel siteMapCategoryListModel = null;
            try
            {
                int catalogId, portalId, localeId;
                GetParametersValueForFilters(filters, out catalogId, out portalId, out localeId);

                PageListModel pageListModel = new PageListModel(filters, sorts, page);
                IZnodeViewRepository<SiteMapCategoryModel> objStoredProc = new ZnodeViewRepository<SiteMapCategoryModel>();
                objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@CatalogId", catalogId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@VersionId", GetCatalogVersionId(catalogId), ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@IncludeAssociatedCategories", includeAssociatedCategories, ParameterDirection.Input, DbType.Boolean);
                objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
                IList<SiteMapCategoryModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetPublishCategoryForSiteMap  @Rows,@PageNo,@CatalogId,@VersionId,@LocaleId,@IncludeAssociatedCategories,@RowsCount OUT", 6, out pageListModel.TotalRowCount);
                siteMapCategoryListModel = new SiteMapCategoryListModel { };
                siteMapCategoryListModel.TotalResults = pageListModel.TotalRowCount;
                if (list.Count > 0)
                    GetParentCategories(siteMapCategoryListModel, list);

            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                throw ex;
            }
            return siteMapCategoryListModel;
        }

        // This method is used to parent categories.
        protected virtual void GetParentCategories(SiteMapCategoryListModel webStoreCategoryListModel, IList<SiteMapCategoryModel> list)
        {
            try
            {
                webStoreCategoryListModel.CategoryList = new List<SiteMapCategoryModel>();
                if (list.Count > 0)
                {
                    webStoreCategoryListModel.CategoryList = list?.Where(y => (HelperUtility.IsNull(y.ZnodeParentCategoryIds))).ToList();
                    GetSubCategories(webStoreCategoryListModel.CategoryList, list);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                throw ex;
            }
        }

        // This method is used to sub categories.
        protected virtual void GetSubCategories(List<SiteMapCategoryModel> siteMapCategoryModelList, IList<SiteMapCategoryModel> list)
        {
            try
            {
                foreach (SiteMapCategoryModel item in siteMapCategoryModelList)
                {
                    List<SiteMapCategoryModel> subCategories = list?.Where(y => (HelperUtility.IsNotNull(y.ZnodeParentCategoryIds) && y.ZnodeParentCategoryIds.Contains(item.ZnodeCategoryId.ToString()))).ToList();
                    item.SubCategoryItems = subCategories?.Count > 0 ? subCategories.ToList() : new List<SiteMapCategoryModel>();
                    GetSubCategories(item.SubCategoryItems, list);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                throw ex;
            }
        }

        // This used to get the sitemap brand list.
        public SiteMapBrandListModel GetSiteMapBrandList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started to get Publish Brands.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);

            SiteMapBrandListModel siteMapBrandListModel = null;
            try
            {
                int catalogId, portalId, localeId;
                GetParametersValueForFilters(filters, out catalogId, out portalId, out localeId);

                PageListModel pageListModel = new PageListModel(filters, sorts, page);
                IZnodeViewRepository<SiteMapBrandModel> objStoredProc = new ZnodeViewRepository<SiteMapBrandModel>();
                objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@VersionId", WebstoreVersionId.ToString(), ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
                IList<SiteMapBrandModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetPublishBrandForSiteMap  @Rows,@PageNo,@PortalId,@VersionId,@LocaleId,@RowsCount OUT", 5, out pageListModel.TotalRowCount);
                siteMapBrandListModel = new SiteMapBrandListModel { BrandList = list?.ToList() };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                throw ex;
            }

            return siteMapBrandListModel;
        }

        // This used to get the sitemap product list.
        public virtual SiteMapProductListModel GetSiteMapProductList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started to get Publish Brands.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);

            SiteMapProductListModel siteMapProductListModel = null;
            try
            {
                int catalogId, portalId, localeId;
                GetParametersValueForFilters(filters, out catalogId, out portalId, out localeId);

                PageListModel pageListModel = new PageListModel(filters, sorts, page);
                IZnodeViewRepository<SiteMapProductModel> objStoredProc = new ZnodeViewRepository<SiteMapProductModel>();
                objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@CatalogId", catalogId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@VersionId", GetCatalogVersionId(catalogId), ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
                IList<SiteMapProductModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetPublishProductForSiteMap  @Rows,@PageNo,@CatalogId,@VersionId,@LocaleId,@RowsCount OUT", 5, out pageListModel.TotalRowCount);

                siteMapProductListModel = new SiteMapProductListModel { ProductList = list?.ToList() };
                siteMapProductListModel.TotalResults = pageListModel.TotalRowCount;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                throw ex;
            }

            return siteMapProductListModel;
        }

        //Get parameter values from filters.
        protected virtual void GetParametersValueForFilters(FilterCollection filters, out int catalogId, out int portalId, out int localeId)
        {
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(WebStoreEnum.ZnodeCatalogId.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out catalogId);
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.PortalId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out portalId);
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.LocaleId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out localeId);
        }
    }
}
