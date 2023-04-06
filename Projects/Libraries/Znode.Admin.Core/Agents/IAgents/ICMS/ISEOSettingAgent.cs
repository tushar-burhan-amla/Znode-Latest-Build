using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface ISEOSettingAgent
    {
        /// <summary>
        /// Get portal's SEO settings.
        /// </summary>
        /// <param name="portalId">Portal ID for SEO setting.</param>
        /// <returns>Portal SEO Setting.</returns>
        PortalSEOSettingViewModel GetPortalSEOSetting(int portalId);

        /// <summary>
        /// Creates a new Portal SEO Setting.
        /// </summary>
        /// <param name="model">Portal SEO Setting model.</param>
        /// <returns>New Portal SEO setting.</returns>
        PortalSEOSettingViewModel CreatePortalSEOSetting(PortalSEOSettingViewModel model);

        /// <summary>
        /// Updates Portal SEO Setting.
        /// </summary>
        /// <param name="model">Portal SEO setting to be updated.</param>
        /// <returns>Updated portal SEO setting.</returns>
        PortalSEOSettingViewModel UpdatePortalSEOSetting(PortalSEOSettingViewModel model);

        /// <summary>
        /// Gets list of published products.
        /// </summary>
        /// <param name="filters">Filters for list of published products.</param>
        /// <param name="sorts">Sorts for list of published products.</param>
        /// <param name="pageIndex">Page Index for list of published products.</param>
        /// <param name="pageSize">Page size for list of published products.</param>
        /// <returns>List of published products.</returns>
        ProductDetailsListViewModel GetPublishedProducts(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int portalId);

        /// <summary>
        /// Get Products list for SEO
        /// </summary>
        /// <param name="filters">Filters for list of published products.</param>
        /// <param name="sorts">Sorts for list of published products.</param>
        /// <param name="pageIndex">Page Index for list of published products.</param>
        /// <param name="pageSize">Page size for list of published products.</param>
        /// <returns>List of published products.</returns>
        ProductDetailsListViewModel GetProductsForSEO(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int portalId);

        /// <summary>
        /// Gets list of published categories.
        /// </summary>
        /// <param name="filters">Filters for list of published categories.</param>
        /// <param name="sorts">Sorts for list of published categories.</param>
        /// <param name="pageIndex">Page index for list of published categories.</param>
        /// <param name="pageSize">Page size for list of published categories.</param>
        /// <returns>List of published categories.</returns>
        CategoryListViewModel GetPublishedCategories(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int portalId);

        /// <summary>
        /// Creates SEO details.
        /// </summary>
        /// <param name="model">Model to be created.</param>
        /// <returns>Newly created SEO details model.</returns>
        SEODetailsViewModel CreateSEODetails(SEODetailsViewModel model);

        /// <summary>
        /// Updates SEO details model.
        /// </summary>
        /// <param name="model">SEO details model to be updated.</param>
        /// <returns>Updated SEO details model.</returns>
        SEODetailsViewModel UpdateSEODetails(SEODetailsViewModel model);

        /// <summary>
        /// Gets SEO details for an item.
        /// </summary>
        /// <param name="itemId">ID of the Content page/category/product.</param>
        /// <param name="seoTypeId">SEO Type Id</param>
        /// <returns>SEO details view model.</returns>
        SEODetailsViewModel GetSEODetails(string seoCode, int seoTypeId, int localeId, int portalId);

        /// <summary>
        /// Gets SEO details for product view.
        /// </summary>
        /// <param name="itemId">ID of the Content page/category/product.</param>
        /// <param name="seoTypeId">SEO Type Id</param>
        /// <param name="localeId">Locale Id.</param>
        /// <param name="portalId">Portal Id</param>      
        /// <returns>SEO details view model.</returns>
        SEODetailsViewModel GetProductSEODetails(int itemId, int seoTypeId, int localeId, int portalId);

        /// <summary>
        /// Gets SEO details for product view.
        /// </summary>
        /// <param name="itemId">ID of the Content page/category/product.</param>
        /// <param name="seoTypeId">SEO Type Id</param>
        /// <param name="localeId">Locale Id.</param>
        /// <param name="portalId">Portal Id</param>
        /// <param name="seoCode">seoCode</param>
        /// <returns>SEO details view model.</returns>
        SEODetailsViewModel GetSEODetailsBySEOCode(int seoTypeId, int localeId, int portalId,string seoCode);


        /// <summary>
        /// Gets list of portals.
        /// </summary>        
        /// <returns>List of portals.</returns>
        List<SelectListItem> GetPortalList();


        /// <summary>
        /// Set filters for portalID.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="portalId"></param>
        /// <param name="portalList"></param>
        /// <returns></returns>
        FilterCollection SetFilter(FilterCollectionDataModel filter, int portalId, List<SelectListItem> portalList);

        /// <summary>
        /// Get Content Page List Details.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="PortalId"></param>
        /// <param name="PortalList"></param>
        /// <returns></returns>
        ContentPageListViewModel GetContentPageDetails(FilterCollectionDataModel model, int PortalId, List<SelectListItem> PortalList);

        /// <summary>
        /// Publish SEO setting.
        /// </summary>
        /// <param name="seoCode"></param>
        /// <param name="portalId"></param>
        /// <param name="localeId"></param>
        /// <param name="seoTypeId"></param>
        /// <param name="errorMessage"></param>
        /// <returns>Returns true or false on publish true else false</returns>
        bool Publish(string seoCode, int portalId, int localeId, int seoTypeId,out string errorMessage);

        /// <summary>
        /// Publish SEO setting.
        /// </summary>
        /// <param name="seoCode"></param>
        /// <param name="portalId"></param>
        /// <param name="localeId"></param>
        /// <param name="seoTypeId"></param>
        /// <param name="errorMessage"></param>
        /// <param name="targetPublishState"></param>
        /// <param name="takeFromDraftFirst"></param>
        /// <returns></returns>
        bool Publish(string seoCode, int portalId, int localeId, int seoTypeId, out string errorMessage, string targetPublishState = null, bool takeFromDraftFirst = false);

        /// <summary>
        /// Gets list of categories.
        /// </summary>
        /// <param name="filters">Filters for list of published categories.</param>
        /// <param name="sorts">Sorts for list of published categories.</param>
        /// <param name="pageIndex">Page index for list of published categories.</param>
        /// <param name="pageSize">Page size for list of published categories.</param>
        /// <returns>List of published categories.</returns>
        CategoryListViewModel GetCategoriesForSEO(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int portalId);

        /// <summary>
        /// Gets SEO details for product, category, content page view.
        /// </summary>
        /// <param name="itemId">ID of the Content page/category/product.</param>
        /// <param name="seoTypeId">SEO Type Id</param>
        /// <param name="localeId">Locale Id.</param>
        /// <param name="portalId">Portal Id</param>      
        /// <returns>SEO details view model.</returns>
        SEODetailsViewModel GetDefaultSEODetails(int seoTypeId, int localeId, int portalId, string seoCode, int itemId);
        /// <summary>
        /// Gets CMS SEO details for product, category, content page view.
        /// </summary>
        /// <param name="itemId">ID of the Content page/category/product.</param>
        /// <param name="seoTypeId">SEO Type Id</param>
        /// <param name="localeId">Locale Id.</param>
        /// <param name="portalId">Portal Id</param>      
        /// <returns>SEO details view model.</returns>
        ContentPageViewModel GetDefaultCMSSEODetails(int seoTypeId, int localeId, int portalId, string seoCode, int itemId);

        /// <summary>
        /// Delete Seo Detail by using seoTypeId, portalId and seoCode.
        /// </summary>
        /// <param name="seoTypeId">seoType Id</param>
        /// <param name="portalId">portal Id</param>
        /// /// <param name="seoCode">seoCode</param>
        /// <returns>Returns true if record deleted successfully, else false.</returns>.
        bool DeleteSeo(int seoTypeId, int portalId, string seoCode = "");
    }
}
