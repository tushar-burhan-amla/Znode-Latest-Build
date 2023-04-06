namespace Znode.Engine.Api.Cache
{
    public interface ICustomerReviewCache
    {
        /// <summary>
        /// Get Customer Review list.
        /// </summary>
        /// <param name="localeId">Current Locale Id.</param>
        /// <param name="routeUri">Route uri.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>Returns string data for customer review list.</returns>
        string GetCustomerReviewList(string localeId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Customer Review by customer review id.
        /// </summary>
        /// <param name="customerReviewId">Customer Review Id.</param>
        /// <param name="localeId">Current Locale Id.</param>
        /// <param name="routeUri">Route uri.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>Returns string data for customer review model.</returns>
        string GetCustomerReview(int customerReviewId, string localeId, string routeUri, string routeTemplate);
    }
}
