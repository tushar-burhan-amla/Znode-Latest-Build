using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IProductFeedService
    {
        /// <summary>
        /// Create Product feed.
        /// </summary>
        /// <param name="model">Product Feed Model.</param>
        /// <returns>returns Product Feed Model.</returns>
        ProductFeedModel CreateGoogleSiteMap(ProductFeedModel model);

        /// <summary>
        /// Update product feed.
        /// </summary>
        /// <param name="model">ProductFeedModel</param>
        /// <returns>Return True/false</returns>
        bool UpdateProductFeed(ProductFeedModel model);

        /// <summary>
        /// Get product feed by id.
        /// </summary>
        /// <param name="productFeedId">productFeedId</param>
        /// <param name="expands">Expands to be retrieved along with product feed list.</param>
        /// <returns>ProductFeedModel</returns>
        ProductFeedModel GetProductFeed(int productFeedId, NameValueCollection expands);

        /// <summary>
        /// Get product feed list.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with product feed list.</param>
        /// <param name="filters">Filters to be applied on product feed list.</param>
        /// <param name="sorts">Sorting to be applied on product feed list.</param>
        /// <param name="page">Start page index of product feed list.</param>
        /// <returns>ProductFeedListModel</returns>
        ProductFeedListModel GetProductFeedList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Delete product feed.
        /// </summary>
        /// <param name="productFeedIds">ParameterModel</param>
        /// <returns>Return True/false</returns>
        bool DeleteProductFeed(ParameterModel productFeedIds);

        /// <summary>
        /// Get Product Feed master details
        /// </summary>
        /// <returns>ProductFeedModel</returns>
        ProductFeedModel GetProductFeedMasterDetails();

        /// <summary>
        /// Get Product Feed details by Portal Id.
        /// </summary>
        /// <param name="portalId">portalId</param>
        /// <returns>ProductFeedModel</returns>
        ProductFeedListModel GetProductFeedByPortalId(int portalId);

        /// <summary>
        /// Check if the file name already exists.
        /// </summary>
        /// <returns>localeId</returns>
        /// <returns>fileName</returns>
        bool FileNameCombinationAlreadyExist(int localeId, string fileName);
    }
}
