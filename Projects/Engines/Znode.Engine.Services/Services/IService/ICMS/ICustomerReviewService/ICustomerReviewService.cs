using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ICustomerReviewService
    {
        /// <summary>
        /// Get Customer Review list.
        /// </summary>
        /// <param name="localeId">Current Locale Id.</param>
        /// <param name="filters">Filters for customer reviews.</param>
        /// <param name="sorts">Sorts for customer reviews.</param>
        /// <param name="page">Name Value Collection.</param>
        /// <returns>Returns list of customer reviews.</returns>
        CustomerReviewListModel GetCustomerReviewList(string localeId, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

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
        /// <returns>Returns true if customer review updated successfully else returns false.</returns>
        bool UpdateCustomerReview(CustomerReviewModel customerReviewModel);

        /// <summary>
        /// Delete customer review.
        /// </summary>
        /// <param name="customerReviewId">Customer review Ids to be deleted.</param>
        /// <returns>Returns true if customer review deleted successfully else resturn false.</returns>
        bool DeleteCustomerReview(ParameterModel customerReviewIds);

        /// <summary>
        /// Change customer review status.
        /// </summary>
        /// <param name="customerReviewId">Customer Review Id whose status has to be changed.</param>
        /// <param name="statusId">Id of status.</param>
        /// <returns>Returns true if status changed successfully else return false.</returns>
        bool BulkStatusChange(ParameterModel customerReviewId, string statusId);

        /// <summary>
        /// create customer review
        /// </summary>
        /// <param name="customerReviewModel">model with product review details</param>
        /// <returns>review model</returns>
        CustomerReviewModel Create(CustomerReviewModel customerReviewModel);
    }
}
