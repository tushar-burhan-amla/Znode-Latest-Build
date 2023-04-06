using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Services.Constants;
using Znode.Libraries.Admin;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Utilities = Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class PublishCategoryService : BaseService, IPublishCategoryService
    {
        #region Private Variables
        private readonly ISEOService _seoService;
        private readonly IZnodeRepository<ZnodePortalCatalog> _portalCatalogRepository;
        private readonly IPublishedCategoryDataService publishedCategoryDataService;
        #endregion

        #region Constructor
        public PublishCategoryService()
        {
            _seoService = GetService<ISEOService>();
            _portalCatalogRepository = new ZnodeRepository<ZnodePortalCatalog>();
            publishedCategoryDataService = GetService<IPublishedCategoryDataService>();
        }
        #endregion

        #region Public Methods

        public virtual PublishCategoryModel GetPublishCategory(int publishCategoryId, FilterCollection filters, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            //Get parameter values from filters.
            int catalogId, portalId, localeId;
            bool isBindImage, isGetParentCategory;
            GetParametersValueForFilters(filters, out catalogId, out portalId, out localeId,out isBindImage,out isGetParentCategory);
            int publishCatalogId = 0;

            if (catalogId <= 0)
            {
                publishCatalogId = _portalCatalogRepository.Table.FirstOrDefault(x => x.PortalId == portalId)?.PublishCatalogId ?? 0;
            }

            //Remove portal id, isBindImage, isGetParentCategory filter.
            RemovePortalIdImageParentCategoryFromFilters(filters);

            ZnodeLogging.LogMessage("publishCatalogId : ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { publishCatalogId });

            //Replace filter keys.
            ReplaceFilterKeys(ref filters);

            filters.Add(WebStoreEnum.ZnodeCategoryId.ToString(), FilterOperators.Equals, Convert.ToString(publishCategoryId));

            filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.Equals, Convert.ToString(GetCatalogVersionId(catalogId, localeId)));

            //get publish category  
            PublishCategoryModel publishCategory = publishedCategoryDataService.GetPublishedCategory(filters)?.ToModel<PublishCategoryModel>();

            //get products associated to category from expands
            GetDataFromExpands(expands, publishCategory, localeId, portalId);

            //get Image Path For Category.
            if(isBindImage)
                GetCategoryImagePath(portalId, publishCategory);

            if (isGetParentCategory && HelperUtility.IsNotNull(publishCategory))
                publishCategory.ParentCategory = (publishCategory.ZnodeParentCategoryIds?.Count() > 0) ?  GetParentCategories(publishCategory.ZnodeParentCategoryIds,  GetService<IPublishedPortalDataService>().GetSEOSettings(GetFiltersForSEO(portalId, localeId, ZnodeConstant.Category)), localeId) : null;

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return publishCategory;
        }

        //Get published category list
        public virtual PublishCategoryListModel GetPublishCategoryList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            int catalogId, portalId, localeId;
            bool isBindImage, isGetParentCategory;
            GetParametersValueForFilters(filters, out catalogId, out portalId, out localeId,out isBindImage,out isGetParentCategory);

            filters.RemoveAll(x => x.FilterName == Utilities.FilterKeys.PortalId);

            //Replace filter keys with published data filter keys
            ReplaceFilterKeys(ref filters);
            ReplaceSortKeys(ref sorts);

            if (catalogId > 0)
                filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.Equals, Convert.ToString(GetCatalogVersionId(catalogId, localeId)));
            else
            {
                string versionIds = filters.Exists(x => x.Item1 == Utilities.FilterKeys.RevisionType) ? GetCatalogAllVersionIds(localeId) : GetCatalogAllVersionIds();
                if(!string.IsNullOrEmpty(versionIds))
                    filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.In, versionIds);
            }
             if (filters.Exists(x => x.Item1 == Utilities.FilterKeys.RevisionType))
                filters.RemoveAll(x => x.Item1 == Utilities.FilterKeys.RevisionType);

            SetCategoryIndexFilter(filters);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetPagedList: ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            //Get publish categories
            List<PublishCategoryModel> categories = publishedCategoryDataService.GetPublishCategoryPageList(pageListModel, out pageListModel.TotalRowCount).ToModel<PublishCategoryModel>().ToList();
            ZnodeLogging.LogMessage("publish categories count   : ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { categories?.Count });

            //Filter list by expiration date and activation date.
            categories = GetFilterDateResult(categories);
            ZnodeLogging.LogMessage("categories returned from GetFilterDateResult : ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { categories });


            PublishCategoryListModel publishCategoryListModel = new PublishCategoryListModel() { PublishCategories = categories };

            //get products associated to categories from expands
            GetDataFromExpands(expands, publishCategoryListModel, localeId, portalId);

            GetCategoryImagePathForList(portalId, publishCategoryListModel);

            //Map pagination parameters
            publishCategoryListModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return publishCategoryListModel;
        }

        //Get publish Category excluding assigned Ids.
        public virtual PublishCategoryListModel GetUnAssignedPublishCategoryList(string assignedIds, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            filters.Add(Utilities.FilterKeys.ZnodeCategoryId, FilterOperators.NotIn, assignedIds);
            return GetPublishCategoryList(expands, filters, sorts, page);
        }

        #endregion

        #region Private Methods
        //get products associated to category from expands
        private void GetDataFromExpands(NameValueCollection expands, PublishCategoryModel publishCategory, int localeId, int PortalId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters localeId, PortalId.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose,new object[] { localeId, PortalId });

            if (publishCategory?.PublishCategoryId > 0 && (expands?.HasKeys() ?? false))
            {
                foreach (string key in expands.Keys)
                {
                    if (string.Equals(key, ExpandKeys.Product, StringComparison.OrdinalIgnoreCase))
                    {
                        //get publish products associated with category and map products to category
                        publishCategory.products = GetPublishProducts(new List<int> { publishCategory.PublishCategoryId }, publishCategory.LocaleId);
                        ZnodeLogging.LogMessage("published products associated with category.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { publishCategory?.products });
                    }
                    else if (string.Equals(key, ZnodeConstant.SEO, StringComparison.OrdinalIgnoreCase))
                        //get category seo details.
                        GetCategorySEODetails(publishCategory, localeId, PortalId);
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

        }

        //get products associated to categories from expands
        private void GetDataFromExpands(NameValueCollection expands, PublishCategoryListModel publishCategories, int localeId, int portalId)
        {
            ZnodeLogging.LogMessage("Input parameters localeId, portalId .", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { localeId, portalId });

            if (publishCategories?.PublishCategories?.Count > 0 && (expands?.HasKeys() ?? false))
            {
                foreach (string key in expands.Keys)
                {
                    if (string.Equals(key, ExpandKeys.Product, StringComparison.OrdinalIgnoreCase))
                    {
                        //get publish products associated with category
                        List<PublishProductModel> products = GetPublishProducts(publishCategories.PublishCategories.Select(x => x.PublishCategoryId), localeId);

                        //map products to associated categories
                        publishCategories.PublishCategories.ForEach(
                            x => x.products = products.Where(s => s.ZnodeCategoryIds.Equals(x.PublishCategoryId))?.ToList());
                    }
                    else if (string.Equals(key, ZnodeConstant.SEO, StringComparison.OrdinalIgnoreCase))
                        //get category list seo details.
                        GetSEOForCategoryList(publishCategories, localeId, portalId);

                }
            }
        }

        //Get SEO For category list.
        private void GetSEOForCategoryList(PublishCategoryListModel publishCategories, int localeId, int portalId)
        {
            List<ZnodePublishSeoEntity> seoSettings = GetService<IPublishedPortalDataService>().GetSEOSettings(GetFiltersForSEO(portalId, localeId, ZnodeConstant.Category));

            ZnodeLogging.LogMessage("seoSettings count returned from GetPublishSEOSettingList .", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { seoSettings?.Count });

            List<SEODetailsModel> znodeCMSSEODetails = ZnodeDependencyResolver.GetService<IPublishProductHelper>().GetSEODetailsForList(portalId, ZnodeConstant.Category);
            ZnodeLogging.LogMessage("znodeCMSSEODetails count .", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { znodeCMSSEODetails?.Count });

            if (publishCategories?.PublishCategories.Count > 0)
            {
                publishCategories.PublishCategories.ForEach(category =>
                {
                    ZnodePublishSeoEntity categorySeoDetails = seoSettings?
                                .FirstOrDefault(seoDetail => seoDetail.SEOCode == category.Attributes?.FirstOrDefault(x => x.AttributeCode == "CategoryCode")?.AttributeValues);

                    SEODetailsModel seoDetails = znodeCMSSEODetails
                             .FirstOrDefault(productdata => productdata.SEOCode == category.Attributes?.FirstOrDefault(x => x.AttributeCode == "CategoryCode")?.AttributeValues);

                    if (HelperUtility.IsNull(category.SEODetails))
                        category.SEODetails = new SEODetailsModel();

                    category.PublishStatus = seoDetails?.IsPublish ?? false ? ZnodeConstant.Published : ZnodeConstant.Draft;
                    ZnodeLogging.LogMessage("PublishStatus :", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { category.PublishStatus });

                    if (category.PublishStatus == ZnodeConstant.Published && HelperUtility.IsNotNull(categorySeoDetails))
                    {
                        category.SEODetails.SEOUrl = categorySeoDetails.SEOUrl;
                        category.SEOUrl = categorySeoDetails.SEOUrl;
                        category.SEOTitle = categorySeoDetails.SEOTitle;
                        category.SEOKeywords = categorySeoDetails.SEOKeywords;
                        category.SEODescription = categorySeoDetails.SEODescription;
                    }
                    else if (HelperUtility.IsNotNull(seoDetails))
                    {
                        category.SEODescription = seoDetails.SEODescription;
                        category.SEOKeywords = seoDetails.SEOKeywords;
                        category.SEOTitle = seoDetails.SEOTitle;
                        category.SEODetails.SEOUrl = seoDetails.SEOUrl;
                    }

                });
            }

        }

        //Get filters for SEO
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

        //get publish products associated with category
        private List<PublishProductModel> GetPublishProducts(IEnumerable<int> categoryIds, int? localeId)
        {
            ZnodeLogging.LogMessage("Input parameters categoryIds, localeId.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { categoryIds, localeId });

            FilterCollection filters = new FilterCollection();
            filters.Add("ZnodeCatalogId", FilterOperators.In, string.Join(",", categoryIds));
            if (localeId > 0)
                filters.Add("LocaleId", FilterOperators.Equals, localeId.ToString());


            return GetService<IPublishedProductDataService>().GetPublishProducts(new PageListModel(filters, null, null))?.ToModel<PublishProductModel>()?.ToList();
        }

        //Replace Filter Keys
        private void ReplaceFilterKeys(ref FilterCollection filters)
        {
            foreach (FilterTuple tuple in filters)
            {
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.LocaleId, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.LocaleId, Utilities.FilterKeys.PublishedLocaleId); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.CatalogId, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.CatalogId, Utilities.FilterKeys.ZnodeCatalogId); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.CategoryId, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.CategoryId, Utilities.FilterKeys.ZnodeCategoryId); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.CategoryName, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.CategoryName, Utilities.FilterKeys.Name); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.ItemName, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.ItemName, Utilities.FilterKeys.Name); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.ParentCategoryIds, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.ParentCategoryIds, Utilities.FilterKeys.ZnodeParentCategoryIds); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.ZnodecatalogId, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.ZnodecatalogId, Utilities.FilterKeys.ZnodeCatalogId); }
                if (string.Equals(tuple.Item1, WebStoreEnum.ProfileIds.ToString(), StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, WebStoreEnum.ProfileIds.ToString().ToLower(), WebStoreEnum.ProfileIds.ToString()); }
                if (string.Equals(tuple.Item1, WebStoreEnum.IsActive.ToString(), StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, WebStoreEnum.IsActive.ToString().ToLower(), WebStoreEnum.IsActive.ToString()); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.PublishedCatalogName, StringComparison.OrdinalIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.PublishedCatalogName.ToLower(), Utilities.FilterKeys.PublishedCatalogName); }
            }
            ReplaceFilterKeysForOr(ref filters);
        }

        //Replace Filter Keys
        private void ReplaceFilterKeysForOr(ref FilterCollection filters)
        {
            foreach (FilterTuple tuple in filters)
            { 
                if (tuple.Item1.Contains("|"))
                {
                    List<string> newValues = new List<string>();
                    foreach (var item in tuple.Item1.Split('|'))
                    {
                        if (string.Equals(item, Utilities.FilterKeys.CategoryName, StringComparison.OrdinalIgnoreCase)) { newValues.Add(Utilities.FilterKeys.Name);}
                        else if (string.Equals(item, Utilities.FilterKeys.PublishedCatalogName, StringComparison.OrdinalIgnoreCase)) {newValues.Add(Utilities.FilterKeys.PublishedCatalogName); }
                        else if (string.Equals(item, Utilities.FilterKeys.ItemName, StringComparison.OrdinalIgnoreCase)) { newValues.Add(Utilities.FilterKeys.Name); }
                        else newValues.Add(item);
                    }
                    ReplaceFilterKeyName(ref filters, tuple.Item1, string.Join("|", newValues));
                }
            }
        }

        //Replace sort Keys
        private void ReplaceSortKeys(ref NameValueCollection sorts)
        {
            foreach (string key in sorts.Keys)
            {
                if (string.Equals(key, Utilities.FilterKeys.ItemName, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.ItemName, Utilities.FilterKeys.Name); }
                if (string.Equals(key, Utilities.FilterKeys.PublishCategoryId, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.PublishCategoryId.ToLower(), Utilities.FilterKeys.ZnodeCategoryId); }
                if (string.Equals(key, Utilities.FilterKeys.ItemId, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.ItemId, Utilities.FilterKeys.ZnodeCategoryId); }
                if (string.Equals(key, Utilities.FilterKeys.DisplayOrder, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.DisplayOrder, ZnodePortalAddressEnum.DisplayOrder.ToString()); }
                if (string.Equals(key, Utilities.FilterKeys.CategoryName, StringComparison.OrdinalIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.CategoryName, Utilities.FilterKeys.Name); }
            }
        }

        //Get category SEO Settings.
        private void GetCategorySEODetails(PublishCategoryModel publishCategory, int localeId, int portalId)
        {
            ZnodeLogging.LogMessage("Input parameters PublishCategoryModel with PublishCategoryId, localeId, portalId  ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { publishCategory?.PublishCategoryId, localeId, portalId });

            FilterCollection filter = new FilterCollection();
            filter.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString());

            PortalSEOSettingModel portalSeoSetting = _seoService.GetPortalSEOSetting(portalId);
            string CategoryCode = publishCategory.Attributes?.FirstOrDefault(x => x.AttributeCode == "CategoryCode")?.AttributeValues;
            ZnodeLogging.LogMessage("CategoryCode ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { CategoryCode });

            ZnodePublishSeoEntity seoSettings = GetService<IPublishedPortalDataService>().GetSEOEntityByCode(CategoryCode, ZnodeConstant.Category, portalId, localeId);
            ZnodeLogging.LogMessage("seoSettings returned from GetPublishSeoSetting ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { seoSettings });

            if (HelperUtility.IsNull(publishCategory.SEODetails))
                publishCategory.SEODetails = new SEODetailsModel();

            publishCategory.SEODetails.SEODescription = GetSeoDetails(seoSettings?.SEODescription, portalSeoSetting?.CategoryDescription, publishCategory);
            publishCategory.SEODetails.SEOKeywords = GetSeoDetails(seoSettings?.SEOKeywords, portalSeoSetting?.CategoryKeyword, publishCategory);
            publishCategory.SEODetails.SEOTitle = GetSeoDetails(seoSettings?.SEOTitle, portalSeoSetting?.CategoryTitle, publishCategory);
            publishCategory.SEODetails.SEOUrl = seoSettings?.SEOUrl;
            publishCategory.SEODetails.CanonicalURL = seoSettings?.CanonicalURL;
            publishCategory.SEODetails.RobotTag = seoSettings?.RobotTag;
        }

        //Get SEO according to portal default setting.
        private string GetSeoDetails(string actualSEOSettings, string siteConfigSEOSettings, PublishCategoryModel entity)
        {
            string seoDetailsText = actualSEOSettings;
            if (string.IsNullOrEmpty(actualSEOSettings) && !string.IsNullOrEmpty(siteConfigSEOSettings))
            {
                string seoDetails = siteConfigSEOSettings;
                seoDetails = seoDetails.Replace(ZnodeConstant.DefaultSEOName, entity.Name);
                seoDetailsText = seoDetails;
            }
            ZnodeLogging.LogMessage("seoDetailsText ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { seoDetailsText });
            return seoDetailsText;
        }

        //get Image Paths For Category.
        private void GetCategoryImagePath(int portalId, PublishCategoryModel publishCategory)
        {
            ZnodeLogging.LogMessage("Input parameters portalId, PublishCategoryModel with PublishCategoryId ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { portalId, publishCategory?.PublishCategoryId });

            //Get Product Image Path
            if (portalId > 0 && HelperUtility.IsNotNull(publishCategory))
            {
                IImageHelper image = GetService<IImageHelper>();
                string categoryImageName = publishCategory.Attributes.Where(x => x.AttributeCode == ZnodeConstant.CategoryImage)?.FirstOrDefault()?.AttributeValues;
                publishCategory.ImageLargePath = image.GetImageHttpPathLarge(categoryImageName);
                publishCategory.ImageMediumPath = image.GetImageHttpPathMedium(categoryImageName);
                publishCategory.ImageThumbNailPath = image.GetImageHttpPathThumbnail(categoryImageName);
                publishCategory.ImageSmallPath = image.GetImageHttpPathSmall(categoryImageName);
            }
        }

        //Get Image Path For Category List.
        protected virtual void GetCategoryImagePathForList(int portalId, PublishCategoryListModel publishCategoryListModel)
        {
            ZnodeLogging.LogMessage("Input parameters portalId ", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { portalId });

            if (portalId > 0)
            {
                string ImageName = string.Empty;
                IImageHelper image = GetService<IImageHelper>();
                publishCategoryListModel?.PublishCategories.ForEach(
                     x =>
                     {
                         ImageName = x.Attributes.Where(y => y.AttributeCode == ZnodeConstant.CategoryImage)?.FirstOrDefault()?.AttributeValues;
                         x.ImageSmallPath = image.GetImageHttpPathSmall(ImageName);
                         x.ImageMediumPath = image.GetImageHttpPathMedium(ImageName);
                         x.ImageThumbNailPath = image.GetImageHttpPathThumbnail(ImageName);
                     });
            }
        }

        //Gets parent category hierarchy.
        private List<PublishCategoryModel> GetParentCategories(int[] parentCategoryIds, List<ZnodePublishSeoEntity> categorySeoDetails, int localeId)
        {
            List<PublishCategoryModel> parentCategories = new List<PublishCategoryModel>();

            List<PublishedCategoryEntityModel> parentCategoryList = publishedCategoryDataService.GetCategoryByIds(localeId, parentCategoryIds)?.ToModel<PublishedCategoryEntityModel>().ToList();

            foreach (int categoryId in parentCategoryIds)
            {
                PublishedCategoryEntityModel parentCategory = parentCategoryList.FirstOrDefault(x => x.ZnodeCategoryId == categoryId);

                if (HelperUtility.IsNotNull(parentCategory))
                {
                    PublishCategoryModel elasticParentCategory = GetElasticCategory(categorySeoDetails, parentCategory, localeId);

                    parentCategories.Add(elasticParentCategory);
                }
            }
            return parentCategories;
        }

        //Maps category entity to Elastic category.
        protected virtual PublishCategoryModel GetElasticCategory(List<ZnodePublishSeoEntity> categorySeoDetails, PublishedCategoryEntityModel parentCategory, int localeId)
        {
            PublishCategoryModel category = new PublishCategoryModel();
            category.SEODetails = new SEODetailsModel();
            category.Name = parentCategory.Name;
            category.PublishCategoryId = parentCategory.ZnodeCategoryId;
            category.SEODetails.SEOUrl = categorySeoDetails.Where(seoDetail => seoDetail.SEOCode == parentCategory?.Attributes?.Where(x => x.AttributeCode == "CategoryCode")?.Select(x => x.AttributeValues)?.FirstOrDefault()?.ToString() && seoDetail.LocaleId == parentCategory.LocaleId)?.FirstOrDefault()?.SEOUrl;
            category.ParentCategory = (parentCategory.ZnodeParentCategoryIds?.Count() > 0) ? GetParentCategories(parentCategory.ZnodeParentCategoryIds, categorySeoDetails, localeId) : null;

            return category;
        }

        //Get parameter values from filters.
        private static void GetParametersValueForFilters(FilterCollection filters, out int catalogId, out int portalId, out int localeId,out bool isBindImage,out bool isGetParentCategory)
        {

            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(WebStoreEnum.ZnodeCatalogId.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out catalogId);
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(Utilities.FilterKeys.PortalId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out portalId);
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(Utilities.FilterKeys.LocaleId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out localeId);
            if (filters.Exists(x => x.FilterName.Equals(Utilities.FilterKeys.IsBindImage.ToString(), StringComparison.InvariantCultureIgnoreCase)))
            {
                bool.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(Utilities.FilterKeys.IsBindImage, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out isBindImage);
            }
            else
            {
                isBindImage = true;
            }
            bool.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(Utilities.FilterKeys.IsGetParentCategory, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out isGetParentCategory);
        }

        //Filter list by expiration date and activation date.
        private List<PublishCategoryModel> GetFilterDateResult(List<PublishCategoryModel> list) =>
         list.Where(x => (x.ActivationDate == null || x.ActivationDate.GetValueOrDefault().Date <= HelperUtility.GetDate()) && (x.ExpirationDate == null || x.ExpirationDate.GetValueOrDefault().Date >= HelperUtility.GetDate())).ToList();

        //Method to remove PortalId Image ParentCategory filter.
        private FilterCollection RemovePortalIdImageParentCategoryFromFilters(FilterCollection filters)
        {
            filters.Remove(filters.Where(x => x.FilterName == Utilities.FilterKeys.PortalId.ToString().ToLower())?.FirstOrDefault());
            filters.Remove(filters.Where(x => x.FilterName == Utilities.FilterKeys.IsBindImage.ToString().ToLower())?.FirstOrDefault());
            filters.Remove(filters.Where(x => x.FilterName == Utilities.FilterKeys.IsGetParentCategory.ToString().ToLower())?.FirstOrDefault());
            return filters;
        }

        #endregion
    }
}
