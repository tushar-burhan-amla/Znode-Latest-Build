using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;

namespace Znode.Engine.WebStore.Agents
{
    public class CategoryAgent : BaseAgent, ICategoryAgent
    {
        #region Private Variables
        private readonly IWebStoreCategoryClient _categoryClient;
        private readonly IPublishCategoryClient _publishCategoryClient;
        private readonly IPublishBrandClient _publishBrandClient;
        #endregion

        #region Public Constructor
        public CategoryAgent(IWebStoreCategoryClient categoryClient, IPublishCategoryClient publishCategoryClient, IPublishBrandClient publishBrandClient)
        {
            _categoryClient = GetClient<IWebStoreCategoryClient>(categoryClient);
            _publishCategoryClient = GetClient<IPublishCategoryClient>(publishCategoryClient);
            _publishBrandClient = GetClient<IPublishBrandClient>(publishBrandClient);
        }
        #endregion

        #region Public Methods
        //Get categories for header.
        public virtual CategoryHeaderListViewModel GetCategories(int? pageSize = 0, int? pageLength = 0)
        {
            //From here we will get the cache Key
            string cachekey = GenerateCacheKeyForCategories();

            //If data is not cached make a call else get cached data for portal.
            if (HelperUtility.IsNull(HttpRuntime.Cache[cachekey]))
            {
                //Gets category list according to the mentioned page size and length
                CategoryHeaderListViewModel listViewModel = GetWebStoreCategories(pageSize, pageLength);
                
                if (listViewModel?.Categories?.Count > 0 || listViewModel?.BrandList?.Count > 0)
                {
                    Helper.AddIntoCache(listViewModel, cachekey, "CurrentPortalCacheDuration");
                    return listViewModel;
                }
                return new CategoryHeaderListViewModel { Categories = new List<CategoryHeaderViewModel>(), BrandList = new List<BrandViewModel>() };

            }

            return Helper.GetFromCache<CategoryHeaderListViewModel>(cachekey);
        }

        [Obsolete("This method is deprecated and will be discontinued in upcoming versions." +
        " This method is marked as obsolute because sitemap logic is move to sitemap agent." +
        " Please use sitemap controller for sitemap releated functions.")]
        //Get categories for header.
        public virtual CategoryHeaderListViewModel GetSiteMapCategories(int? pageSize = 0, int? pageLength = 0)
        {
            //Gets category list according to the mentioned page size and length
            CategoryHeaderListViewModel listViewModel = GetWebStoreCategories(pageSize, pageLength);

            return listViewModel?.Categories?.Count > 0 ? listViewModel
                : new CategoryHeaderListViewModel { Categories = new List<CategoryHeaderViewModel>(), BrandList = new List<BrandViewModel>() };
        }

        //Get category by category id.
        public virtual CategoryViewModel GetCategoryById(int? categoryId)
        {
            ExpandCollection expands = new ExpandCollection
            {
                ExpandKeys.Product
            };

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return _categoryClient.GetWebStoreCategoryList(expands, CreateCategoriesFilter(false, categoryId), null, null, null)?.Categories?.FirstOrDefault()?.ToViewModel<CategoryViewModel>();
        }

        //Get Category Seo Details.
        public virtual CategoryViewModel GetCategorySeoDetails(int categoryId, bool isGetParentCategory = true)
        {
            FilterCollection filters = GetRequiredFilters();
            //filter of parents categories
            filters.Add(WebStoreEnum.IsGetParentCategory.ToString(), FilterOperators.Equals, isGetParentCategory.ToString());
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            PublishCategoryModel category = _publishCategoryClient.GetPublishCategory(categoryId, filters, new ExpandCollection { ZnodeConstant.SEO });

            if (HelperUtility.IsNull(category))
            {
                throw new ZnodeException(ErrorCodes.NotFound, WebStore_Resources.ErrorCategoryNotFound);
            }

            return category?.ToViewModel<CategoryViewModel>();
        }

        //Removes keys from session.
        public virtual void RemoveFromSession(string sessionKey)
        {
            RemoveInSession(sessionKey);
        }

        //Get BreadCrumb.
        public virtual void GetBreadCrumb(CategoryViewModel category)
        {
            category.BreadCrumbHtml = $"<a href='/'>{WebStore_Resources.LinkTextHome}</a> / {GetBreadCrumbHtml(category)}";
        }

        //Get BreadCrumb.
        public virtual string GetBreadCrumb(int categoryId)
        {
            string cachekey = GenerateCacheKeyForBreadCrumb(categoryId);
            if (HelperUtility.IsNull(HttpRuntime.Cache[cachekey]))
            {
                FilterCollection filters = GetRequiredFilters();
                filters.Add(WebStoreEnum.IsGetParentCategory.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue);
                filters.Add(WebStoreEnum.IsBindImage.ToString(), FilterOperators.Equals, ZnodeConstant.FalseValue);
                PublishCategoryModel category = _publishCategoryClient.GetPublishCategory(categoryId, filters, new ExpandCollection { ZnodeConstant.SEO });

                if (HelperUtility.IsNull(category))
                {
                    throw new ZnodeException(ErrorCodes.NotFound, WebStore_Resources.ErrorCategoryNotFound);
                }

                CategoryViewModel categoryViewModel = category?.ToViewModel<CategoryViewModel>();
                string response = $"<a href='/' {WebStore_Resources.LinkHomeIcon}></a> {GetBreadCrumbHtml(categoryViewModel)}";
                Helper.AddIntoCache(response, cachekey, "CurrentPortalCacheDuration");
                return response;
            }
            return Helper.GetFromCache<string>(cachekey);
        }
        #endregion

        #region Private Methods
        //Create filters to get category list.
        private FilterCollection CreateCategoriesFilter(bool isTopLevelCategories, int? categoryId)
        {
            FilterCollection filters = GetRequiredFilters();
            if (isTopLevelCategories)
            {
                filters.Add(WebStoreEnum.ZnodeParentCategoryIds.ToString(), FilterOperators.Equals, null);
            }
            else
            {
                filters.Add(WebStoreEnum.ZnodeCategoryId.ToString(), FilterOperators.Equals, categoryId.ToString());
            }

            return filters;
        }

        //Gets bread html for category.
        private string GetBreadCrumbHtml(CategoryViewModel category, bool isParentCategory = false)
        {
            if (HelperUtility.IsNotNull(category))
            {
                string breadCrumb = string.Empty;

                breadCrumb = (isParentCategory)
                        ? $"<a href='/{(string.IsNullOrEmpty(category.SEODetails?.SEOUrl) ? "category/" + category.CategoryId : category.SEODetails.SEOUrl)}'>{category.CategoryName}</a>"
                        : category.CategoryName;

                if (category?.ParentCategory?.Count > 0)
                {
                    breadCrumb = GetBreadCrumbHtml(category.ParentCategory[0], true) + " / " + breadCrumb;
                }

                return breadCrumb;
            }
            return string.Empty;
        }

        //Gets category list according to the mentioned page size and length
        private CategoryHeaderListViewModel GetWebStoreCategories(int? pageSize, int? pageLength)
        {
            //Sorting For Brand List.
            SortCollection sorts = new SortCollection
                {
                    { SortKeys.DisplayOrder, SortDirections.Ascending }
                };

            CategoryHeaderListViewModel listViewModel = new CategoryHeaderListViewModel { Categories = _categoryClient.GetWebStoreCategoryList(null, CreateCategoriesFilter(true, 0), sorts, pageSize, pageLength)?.Categories?.ToViewModel<CategoryHeaderViewModel>().ToList() };

            int TotalBrandCount = string.IsNullOrEmpty(ZnodeWebstoreSettings.TotalBrandCount) ? Convert.ToInt32(ZnodeWebstoreSettings.TotalBrandCount) : 25;

            //Get First 25 Brands for header.
            BrandListModel brandList = _publishBrandClient.GetPublishBrandList(null, GetBrandFilter(), sorts, 1, TotalBrandCount);

            listViewModel.BrandList = brandList?.Brands?.ToViewModel<BrandViewModel>().ToList();
            return listViewModel;
        }


        //Generate the cache key to get the categories
        private string GenerateCacheKeyForCategories()
        {
            UserViewModel userViewModel = Helper.GetUserDetails();

            int profileId = Convert.ToInt32(Helper.GetProfileId());
         
            int cacheVaryById = Convert.ToInt32(GetCatalogId());

            return $"Toplevelnavigation{PortalAgent.LocaleId.ToString()}{PortalAgent.CurrentPortal.PortalId}{profileId}{cacheVaryById}{PortalAgent.CurrentPortal.PublishState}{profileId}{Helper.GetUserAccountId()}{Helper.GetUserProfileCatalogId()}";
        }


        //Generate the cache key to get the categories breadcrumb
        private string GenerateCacheKeyForBreadCrumb(int categoryId)
        {
            return $"CategoryBreadCrumb{GetCatalogId()}{PortalAgent.LocaleId.ToString()}{PortalAgent.CurrentPortal.PortalId.ToString()}{categoryId}";
        }
        #endregion
    }
}