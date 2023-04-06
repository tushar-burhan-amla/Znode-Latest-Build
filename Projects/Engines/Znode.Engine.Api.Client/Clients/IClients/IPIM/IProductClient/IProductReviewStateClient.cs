using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IProductReviewStateClient : IBaseClient
    {
        /// <summary>
        /// Get ProductReviewState List
        /// </summary>
        /// <param name="expands">Collection of expands</param>
        /// <param name="filters">Collection of filters</param>
        /// <param name="sorts">Collection of sorts</param>
        /// <param name="pageIndex">int Page Index</param>
        /// <param name="pageSize">int Page Size</param>
        /// <returns> ProductReviewState List</returns>
        ProductReviewStateListModel GetProductReviewStates(ExpandCollection expands,FilterCollection filters,SortCollection sorts,int? pageIndex,int? pageSize);
    }
}
