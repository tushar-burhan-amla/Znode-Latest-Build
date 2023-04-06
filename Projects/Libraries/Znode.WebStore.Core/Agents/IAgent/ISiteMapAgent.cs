using System.Collections.Generic;
using System.Text;
using System.Xml;
using Znode.Engine.Api.Models;
using Znode.WebStore.Core.ViewModels;

namespace Znode.WebStore.Core.Agents
{
    public interface ISiteMapAgent
    {
        /// <summary>
        /// This is used to fetch the categories for sitemap.
        /// </summary>
        /// <param name="pageSize">This parameter is use to set the page number.</param>
        /// <param name="pageLength">This parameter is use to set the length of the page.</param>
        /// <returns name="SiteMapCategoryListViewModel">SiteMapCategoryListViewModel return</returns>
        SiteMapCategoryListViewModel GetSiteMapCategoryList(int? pageSize = 0, int? pageLength = 0);

        /// <summary>
        /// This is used to fetch the publish products for sitemap.
        /// </summary>
        /// <param name="pageSize">This parameter is use to set the page number.</param>
        /// <param name="pageLength">This parameter is use to set the length of the page.</param>
        /// <returns name="PublishProductListModel">PublishProductListModel return</returns>
        SiteMapProductListViewModel GetPublishProductList(int? pageIndex, int? pageSize);
        
        /// <summary>
        /// Get XML from API for webstore domain.
        /// </summary>
        /// <param name="requestUrl">requestUrl</param>
        /// <returns>XmlDocument</returns>
        string GetXmlFromAPIForWebstoreDomain(string requestUrl);

        /// <summary>
        /// Get product feed by portal Id.
        /// </summary>
        /// <param name="actionName">actionName</param>
        /// <returns>ProductFeedModel</returns>
        List<ProductFeedModel> GetProductFeedByPortalId(string actionName, string requestUrl, out string xmlDocument);
    }
}
