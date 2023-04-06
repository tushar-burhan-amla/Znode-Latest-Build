using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IProductFeedAgent
    {

        /// <summary>
        /// Creates Google Site Map for generating XML file
        /// </summary>
        /// <param name="model">Model of type GoogleSiteMapViewModel</param>
        /// <param name="domainName">Domain Name</param>
        /// <returns>Returns GoogleSiteMapViewModel</returns>
        ProductFeedViewModel CreateProductFeed(ProductFeedViewModel model, string domainName);

        /// <summary>
        /// Check Whether the File Name already exists.
        /// </summary>
        /// <param name="fileName">fileName</param>
        /// <returns>return the status in true or false</returns>
        bool CheckFileNameExist(string fileName);

        /// <summary>
        /// Get product feed list.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with product feed list.</param>
        /// <param name="filters">Filters to be applied on product feed list.</param>
        /// <param name="sorts">Sorting to be applied on product feed list.</param>
        /// <param name="pageIndex">Start page index of product feed list.</param>
        /// <param name="pageSize">Page size of product feed list.</param>
        /// <returns>ProductFeedListViewModel</returns>
        ProductFeedListViewModel GetProductFeedList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get product feed details by id.
        /// </summary>
        /// <param name="productFeedId">product feed id</param>
        /// <returns>ProductFeedViewModel</returns>
        ProductFeedViewModel GetProductFeedById(int productFeedId);

        /// <summary>
        /// Get product feed master details.
        /// </summary>
        /// <returns>ProductFeedViewModel</returns>
        ProductFeedViewModel GetProductFeedMasterDetails();

        /// <summary>
        /// Delete product feed and Sitemap url from Robots.txt.
        /// </summary>
        /// <param name="productFeedId">product feed id</param>
        /// <returns>Return true/false</returns>
        bool DeleteProductFeed(string productFeedId);

        /// <summary>
        /// Update product feed.
        /// </summary>
        /// <param name="productFeedViewModel">ProductFeedViewModel</param>
        /// <param name="domainName">Domain Name</param>
        /// <returns>ProductFeedViewModel</returns>
        ProductFeedViewModel UpdateProductFeed(ProductFeedViewModel productFeedViewModel, string domainName);

        /// <summary>
        /// Set product feed details.
        /// </summary>
        /// <param name="productFeedViewModel">ProductFeedViewModel</param>
        /// <returns>ProductFeedViewModel</returns>
        ProductFeedViewModel SetProductFeedDetails(ProductFeedViewModel productFeedViewModel);

        /// <summary>
        /// Generate product feed link.
        /// </summary>
        /// <param name="productFeedId">Product Feed Id.</param>
        /// <param name="domainName">Domain name.</param>
        /// <returns>Returns ProductFeedViewModel.</returns>
        ProductFeedViewModel GenerateProductFeedLink(int productFeedId, string domainName);

        /// <summary>
        /// Check if the file name already exists.
        /// </summary>
        /// <returns>localeId</returns>
        /// <returns>fileName</returns>
        bool FileNameCombinationAlreadyExist(int localeId, string fileName);
    }
}
