using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IProductReviewStateService 
    {
        /// <summary>
        /// Get List of ProductReviewState
        /// </summary>
        /// <param name="expands">Collection of expands</param>
        /// <param name="filters">Collection of sorts</param>
        /// <param name="sorts">Collection of sorts</param>
        /// <param name="page">int page</param>
        /// <returns>List of ProductReviewState</returns>
        ProductReviewStateListModel GetProductReviewStates(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);
    }
}
