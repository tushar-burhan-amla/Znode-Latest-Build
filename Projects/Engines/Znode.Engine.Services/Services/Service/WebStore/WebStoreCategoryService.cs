using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public partial class CategoryService
    {
        #region Public Methods
        //Get list of published Categories.
        public virtual WebStoreCategoryListModel GetCategoryDetails(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {

            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            int catalogId, portalId, localeId;
            int? versionId;
            bool ZnodeParentCategoryIdsExist;
            GetValuesFromFilters(filters, out catalogId, out portalId, out localeId, out versionId, out ZnodeParentCategoryIdsExist);

            IPublishedCategoryDataService publishedDataService = GetService<IPublishedCategoryDataService>();

            if (HelperUtility.IsNotNull(sorts))
                ReplaceSortKeys(ref sorts);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel generated to get categoryEntityList: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            List<ZnodePublishCategoryEntity> categoryEntityList = publishedDataService.GetPublishedCategoryList(pageListModel);

            categoryEntityList = !ZnodeParentCategoryIdsExist ? categoryEntityList.Where(x => x.ZnodeParentCategoryIds == null).ToList() : categoryEntityList;
            ZnodeLogging.LogMessage("categoryEntityList count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, categoryEntityList?.Count);

            WebStoreCategoryListModel webStoreCategoryListModel = new WebStoreCategoryListModel { Categories = GetFilterDateResult(categoryEntityList)?.ToModel<WebStoreCategoryModel>().ToList() };

            //Get Category list of catalog.
            List<ZnodePublishCategoryEntity> categoryList = publishedDataService.GetCatalogCategoryList(localeId, catalogId, sorts, versionId);

            ZnodeLogging.LogMessage("Count of category list of catalog returned from method GetCatalogCategoryList: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, categoryList?.Count);

            //Get List of SubCategory associated to category ID.
            if (webStoreCategoryListModel?.Categories.Count > 0)
                GetPublishedSubCategories(webStoreCategoryListModel.Categories, localeId, Convert.ToString(portalId), GetFilterDateResult(categoryList));

            //Get Seo data for category         
            GetCategorySeoDetails(webStoreCategoryListModel, portalId, localeId);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return webStoreCategoryListModel;
        }
   
        #endregion

        protected virtual void GetValuesFromFilters(FilterCollection filters, out int catalogId,out int portalId, out int localeId, out int? versionId, out bool ZnodeParentCategoryIdsExist)
        {
              
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(WebStoreEnum.ZnodeCatalogId.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out catalogId);
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.PortalId.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out portalId);
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.LocaleId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out localeId);
            versionId = GetCatalogVersionId(catalogId, localeId);
            filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.Equals, Convert.ToString(versionId));
            ZnodeParentCategoryIdsExist = true;
            if (filters.Exists(x => x.FilterName.Equals(FilterKeys.ZnodeParentCategoryIds, StringComparison.InvariantCultureIgnoreCase)))
            {
                filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.ZnodeParentCategoryIds, StringComparison.CurrentCultureIgnoreCase));
                ZnodeParentCategoryIdsExist = false;
            }
            filters.RemoveAll(x => x.FilterName == FilterKeys.PortalId);
        }

        #region Private Method
        //Get list of published SubCategories.


        protected virtual void GetPublishedSubCategories(List<WebStoreCategoryModel> webStoreCategoryListModel, int localeId, string portalId, List<ZnodePublishCategoryEntity> categoryList)
        {
            if (webStoreCategoryListModel.Count > 0)
            {
                Parallel.ForEach(webStoreCategoryListModel, x =>
                {
                    List<ZnodePublishCategoryEntity> subCategories = categoryList?.Where(y => (HelperUtility.IsNotNull(y.ZnodeParentCategoryIds) && y.IsActive && y.ZnodeParentCategoryIds.Contains(x.PublishCategoryId.ToString()))).ToList();

                    List<ZnodePublishCategoryEntity> parentCategory = categoryList?.Where(y => (x.ZnodeParentCategoryIds?.Count() > 0 && y.IsActive && x.ZnodeParentCategoryIds.Contains(y.ZnodeCategoryId))).ToList();

                    x.SubCategories = subCategories?.Count > 0 ? subCategories.ToModel<WebStoreCategoryModel>().ToList() : new List<WebStoreCategoryModel>();
                    x.ParentCategory = parentCategory?.Count > 0 ? parentCategory?.ToModel<PublishCategoryModel>().ToList() : new List<PublishCategoryModel>();
                    if (HelperUtility.IsNotNull(x.ParentCategory) && x.ParentCategory.Any())
                        GetParentCategoryList(x.ParentCategory, categoryList);

                    GetPublishedSubCategories(x.SubCategories, localeId, portalId, categoryList);
                });
            }
        }

       

        protected virtual void GetParentCategoryList(List<PublishCategoryModel> parentCategory, List<ZnodePublishCategoryEntity> webStoreCategoryListModel)
        {
            if (parentCategory.FirstOrDefault()?.ZnodeParentCategoryIds.Length > 0)
            {
                parentCategory.FirstOrDefault().ParentCategory = webStoreCategoryListModel.Where(y => y.ZnodeCategoryId == parentCategory.FirstOrDefault().ZnodeParentCategoryIds[0])?.ToModel<PublishCategoryModel>().ToList();
                GetParentCategoryList(parentCategory.FirstOrDefault().ParentCategory, webStoreCategoryListModel);
            }
        }
        //Get seo details for category
        protected virtual void GetCategorySeoDetails(WebStoreCategoryListModel categoryList, int portalId, int localeId, int? versionId = 0)
        {

            ZnodeCMSPortalSEOSetting portalSeoSetting = _portalSEOSettingRepository.Table.FirstOrDefault(x => x.PortalId == portalId);
            AssignSEODetails(categoryList.Categories, GetService<IPublishedPortalDataService>().GetSEOSettings(GetFiltersForSEO(portalId, localeId, ZnodeConstant.Category)), portalSeoSetting);

        }

        //Assign SEO data to categories and subcategories.
        protected virtual void AssignSEODetails(List<WebStoreCategoryModel> categories, List<ZnodePublishSeoEntity> seoSettings, ZnodeCMSPortalSEOSetting seoDefaultSetting)
        {
            if (categories?.Count > 0)
            {
                Parallel.ForEach(categories, category =>
                {
                    string categoryCode = category.Attributes?.FirstOrDefault(x => x.AttributeCode == "CategoryCode")?.AttributeValues;
                    ZnodeLogging.LogMessage("categoryCode generated: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, categoryCode);

                    ZnodePublishSeoEntity seoDetails = seoSettings?
                                 .FirstOrDefault(seoDetail => seoDetail.SEOCode == categoryCode);
                    category.SEODetails = new WebStoreSEOModel();
                    if (HelperUtility.IsNotNull(seoDetails))
                    {
                        category.SEODetails.SEOPageName = seoDetails.SEOUrl;
                        category.SEODetails.SEOKeywords = seoDetails.SEOKeywords;
                        category.SEODetails.SEODescription = seoDetails.SEODescription;
                        category.SEODetails.SEOTitle = seoDetails.SEOTitle;
                    }
                    if (category?.ParentCategory?.Count > 0)
                        GetParentCategorySEO(category.ParentCategory?.FirstOrDefault(), seoSettings);
                    AssignSEODetails(category.SubCategories, seoSettings, seoDefaultSetting);
                });
            }
        }

        //Get Parent category seo details.
        protected virtual void GetParentCategorySEO(PublishCategoryModel category, List<ZnodePublishSeoEntity> seoSettings)
        {
            string categoryCode = category.Attributes?.FirstOrDefault(x => x.AttributeCode == "CategoryCode")?.AttributeValues;
            ZnodePublishSeoEntity seoDetails = seoSettings?
                              .FirstOrDefault(seoDetail => seoDetail.SEOCode == categoryCode);
            if (HelperUtility.IsNotNull(seoDetails))
            {
                category.SEOUrl = seoDetails.SEOUrl;
                category.SEOKeywords = seoDetails.SEOKeywords;
                category.SEODescription = seoDetails.SEODescription;
                category.SEOTitle = seoDetails.SEOTitle;
            }
            if (category?.ParentCategory?.Count > 0)
                GetParentCategorySEO(category.ParentCategory?.FirstOrDefault(), seoSettings);

        }

        //Replace filter keys.
        private void ReplaceFilterKeys(ref FilterCollection filters)
        {
            foreach (FilterTuple tuple in filters)
            {
                if (tuple.Item1 == WebStoreEnum.ZnodeCatalogId.ToString().ToLower()) ReplaceFilterKeyName(ref filters, WebStoreEnum.ZnodeCatalogId.ToString().ToLower(), WebStoreEnum.ZnodeCatalogId.ToString());
                if (tuple.Item1 == WebStoreEnum.ZnodeParentCategoryIds.ToString().ToLower()) ReplaceFilterKeyName(ref filters, WebStoreEnum.ZnodeParentCategoryIds.ToString().ToLower(), WebStoreEnum.ZnodeParentCategoryIds.ToString());
                if (tuple.Item1 == WebStoreEnum.ProfileIds.ToString().ToLower()) ReplaceFilterKeyName(ref filters, WebStoreEnum.ProfileIds.ToString().ToLower(), WebStoreEnum.ProfileIds.ToString());
                if (tuple.Item1 == ZnodeLocaleEnum.LocaleId.ToString().ToLower()) ReplaceFilterKeyName(ref filters, ZnodeLocaleEnum.LocaleId.ToString().ToLower(), ZnodeLocaleEnum.LocaleId.ToString());
                if (tuple.Item1 == WebStoreEnum.ZnodeCategoryId.ToString().ToLower()) ReplaceFilterKeyName(ref filters, WebStoreEnum.ZnodeCategoryId.ToString().ToLower(), WebStoreEnum.ZnodeCategoryId.ToString());
                if (tuple.Item1 == WebStoreEnum.IsActive.ToString().ToLower()) ReplaceFilterKeyName(ref filters, WebStoreEnum.IsActive.ToString().ToLower(), WebStoreEnum.IsActive.ToString());
            }
        }
        //Replace Sort Keys
        private void ReplaceSortKeys(ref NameValueCollection sorts)
        {
            foreach (string key in sorts.Keys)
            {
                if (string.Equals(key, FilterKeys.DisplayOrder, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, FilterKeys.DisplayOrder, ZnodePortalAddressEnum.DisplayOrder.ToString()); }
                if (string.Equals(key, WebStoreEnum.ZnodeCategoryId.ToString(), StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, WebStoreEnum.ZnodeCategoryId.ToString(), WebStoreEnum.ZnodeCategoryId.ToString()); }
            }
        }

        protected virtual FilterCollection GetFiltersForSEO(int portalId, int localeId, string seoTypeName)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple("SEOTypeName", FilterOperators.Is, seoTypeName));
            filters.Add(new FilterTuple("LocaleId", FilterOperators.Equals, localeId.ToString()));
            filters.Add(new FilterTuple("VersionId", FilterOperators.Equals, GetCatalogVersionId().ToString()));
            if (HelperUtility.IsNotNull(portalId))
                filters.Add(new FilterTuple("PortalId", FilterOperators.Equals, portalId.ToString()));
            return filters;
        }

        //Filter list by expiration date and activation date.
        private List<ZnodePublishCategoryEntity> GetFilterDateResult(List<ZnodePublishCategoryEntity> list) =>
         list.Where(x => (x.ActivationDate == null || x.ActivationDate.GetValueOrDefault().Date <= HelperUtility.GetDate()) && (x.ExpirationDate == null || x.ExpirationDate.GetValueOrDefault().Date >= HelperUtility.GetDate())).ToList();

        #endregion
    }
}
