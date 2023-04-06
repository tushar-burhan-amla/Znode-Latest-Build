using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface ICustomerReviewClient : IBaseClient
    {
        /// <summary>
        /// Get customer review list.
        /// </summary>
        /// <param name="localeId">Current Locale Id.</param>
        /// <param name="expands">Expands for customer reviews.</param>
        /// <param name="filters">Filters for customer reviews.</param>
        /// <param name="sorts">Sorts  for customer reviews.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns>Returns list of customer reviews.</returns>
        CustomerReviewListModel GetCustomerReviewList(string localeId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get Customer Review by customer review id.
        /// </summary>
        /// <param name="customerReviewId">Customer Review Id.</param>
        /// <param name="localeId">Current Locale Id.</param>
        /// <returns>Returns CustomerReviewModel.</returns>
        CustomerReviewModel GetCustomerReview(int customerReviewId, string localeId);

        /// <summary>
        /// Update customer review.
        /// </summary>
        /// <param name="customerReviewModel">CustomerReviewModel.</param>
        /// <returns>Returns updated customer review model.</returns>
        CustomerReviewModel UpdateCustomerReview(CustomerReviewModel customerReviewModel);

        /// <summary>
        /// Delete customer review.
        /// </summary>
        /// <param name="customerReviewId">Customer review Ids to be deleted.</param>
        /// <returns>Returns true if customer review deleted successfully else resturn false.</returns>
        bool DeleteCustomerReview(ParameterModel customerReviewId);

        /// <summary>
        /// Change customer review status.
        /// </summary>
        /// <param name="customerReviewId">Customer Review Id whose status has to be changed.</param>
        /// <param name="statusId">Id of status.</param>
        /// <returns>Returns true if status changed successfully else return false.</returns>
        bool BulkStatusChange(ParameterModel customerReviewId, string statusId);

        /// <summary>
        /// create customer review.
        /// </summary>
        /// <param name="customerReviewModel">CustomerReviewModel.</param>
        /// <returns>Returns customer review model.</returns>
        CustomerReviewModel CreateCustomerReview(CustomerReviewModel customerReviewModel);
    }
}
