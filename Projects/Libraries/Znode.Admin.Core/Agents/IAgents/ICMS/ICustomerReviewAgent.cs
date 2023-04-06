using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface ICustomerReviewAgent
    {
        /// <summary>
        /// Get Customer Review list.
        /// </summary>
        /// <param name="filters">Filters for customer reviews.</param>
        /// <param name="sorts">Sorts  for customer reviews.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns>Returns list of customer reviews.</returns>
        CustomerReviewListViewModel GetCustomerReviewList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get Customer Review by customer review id.
        /// </summary>
        /// <param name="customerReviewId">Customer Review Id.</param>
        /// <returns>Returns CustomerReviewModel.</returns>
        CustomerReviewViewModel GetCustomerReview(int customerReviewId);

        /// <summary>
        /// Update customer review.
        /// </summary>
        /// <param name="customerReviewModel">CustomerReviewModel.</param>
        /// <returns>Returns updated customer review model.</returns>
        CustomerReviewViewModel UpdateCustomerReview(CustomerReviewViewModel customerReviewViewModel);

        /// <summary>
        /// Delete customer review.
        /// </summary>
        /// <param name="customerReviewId">Customer review Ids to be deleted.</param>
        /// <returns>Returns true if customer review deleted successfully else resturn false.</returns>
        bool DeleteCustomerReview(string customerReviewId, out string errorMessage);

        /// <summary>
        /// Change customer review status.
        /// </summary>
        /// <param name="customerReviewId">Customer Review Id whose status has to be changed.</param>
        /// <param name="statusId">Id of status.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>Returns true if status changed successfully else return false.</returns>
        bool BulkStatusChange(string cmsCustomerReviewId, string statusId, out string errorMessage);
    }
}
