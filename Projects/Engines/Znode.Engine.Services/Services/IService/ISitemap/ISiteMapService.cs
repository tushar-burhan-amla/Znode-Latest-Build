using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ISiteMapService
    {
        /// This used to get the sitemap category list.
        /// </summary>
        /// <param name="includeAssociatedCategories">Expands for Portal/Profile shipping.
        /// This parameter is used to include the child categorys.
        /// if includeAssociatedCategories  is true then all the categories of parent product
        /// is included otherwise only parent categorires will display.
        /// </param>
        /// <param name="expands">Expands for Portal/Profile shipping.</param>
        /// <param name="filters">Filters for Portal/Profile shipping.</param>
        /// <param name="sorts">Sorts for for Portal/Profile shipping.</param>
        /// <param name="page">Page size.</param>
        /// <returns name="SiteMapCategoryListModel">Returns SiteMapCategoryListModel model.</returns>
        SiteMapCategoryListModel GetSiteMapCategoryList(bool includeAssociatedCategories, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// This used to get the sitemap brand list.
        /// </summary>
        /// <param name="expands">Expands for Portal/Profile shipping.</param>
        /// <param name="filters">Filters for Portal/Profile shipping.</param>
        /// <param name="sorts">Sorts for for Portal/Profile shipping.</param>
        /// <param name="page">Page size.</param>
        /// <returns name="SiteMapBrandListModel">Returns SiteMapBrandListModel model.</returns>
        SiteMapBrandListModel GetSiteMapBrandList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// This used to get the sitemap product list.
        /// </summary>
        /// <param name="expands">Expands for Portal/Profile shipping.</param>
        /// <param name="filters">Filters for Portal/Profile shipping.</param>
        /// <param name="sorts">Sorts for for Portal/Profile shipping.</param>
        /// <param name="page">Page size.</param>
        /// <returns name="SiteMapProductListModel">Returns SiteMapProductListModel model.</returns>
        SiteMapProductListModel GetSiteMapProductList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);
    }
}
