namespace Znode.Engine.Api.Cache
{
    public interface IDevExpressReportCache
    {
        /// <summary>
        /// Get the list of all Report Categories.
        /// </summary>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>list of Report Categories.</returns>
        string GetReportCategories(string routeUri, string routeTemplate);

        /// <summary>
        /// Get report detail for give report category id.
        /// </summary>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <param name="reportCategoryId">report Category Id</param>
        /// <returns>Attributes and Filters</returns>
        string GetReportDetails(string routeUri, string routeTemplate, int reportCategoryId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <param name="ReportCode"></param>
        /// <returns></returns>
        string GetReportSetting(string routeUri, string routeTemplate, string ReportCode);

        /// <summary>
        /// This method for get saved report layout from the database.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <param name="ReportCode"></param>
        /// <param name="ReportName"></param>
        /// <returns></returns>
        string GetSavedReportLayout(string routeUri, string routeTemplate, string ReportCode, string ReportName);

        /// <summary>
        /// Get orders list for report.
        /// </summary>
        ///  <param name="routeUri">URI to route.</param>
        ///  <param name="routeTemplate">Template of route.</param>
        /// <returns></returns>
        string GetOrdersReport(string routeUri, string routeTemplate);

        /// <summary>
        /// Get coupons list for report.
        /// </summary>
        ///  <param name="routeUri">URI to route.</param>
        ///  <param name="routeTemplate">Template of route.</param>
        /// <returns></returns>
        string GetCouponReport(string routeUri, string routeTemplate);

        /// <summary>
        /// Get sale tax details for report.
        /// </summary>
        ///  <param name="routeUri">URI to route.</param>
        ///  <param name="routeTemplate">Template of route.</param>
        /// <returns></returns>
        string GetSalesTaxReport(string routeUri, string routeTemplate);

        /// <summary>
        /// Get affiliate orders for report.
        /// </summary>
        ///  <param name="routeUri">URI to route.</param>
        ///  <param name="routeTemplate">Template of route.</param>
        /// <returns></returns>
        string GetAffiliateOrderReport(string routeUri, string routeTemplate);

        /// <summary>
        /// Get order pick list for report.
        /// </summary>
        ///  <param name="routeUri">URI to route.</param>
        ///  <param name="routeTemplate">Template of route.</param>
        /// <returns></returns>
        string GetOrderPickListReport(string routeUri, string routeTemplate);

        #region reports
        /// <summary>
        /// Get list of best seller product according to parameters
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns></returns>
        string GetBestSellerProductReport(string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of inventory according to parameters
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns></returns>
        string GetInventoryReorderReport(string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of popular search product according to parameters
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns></returns>
        string GetPopularSearchReport(string routeUri, string routeTemplate);

        #endregion report

        #region report
        /// <summary>
        /// Get list of user according to parameters
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns></returns>
        string GetUsersReport(string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of email opt customer according to parameters
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns></returns>
        string GetEmailOptInCustomerReport(string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of most frequent customer according to parameters
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns></returns>
        string GetMostFrequentCustomerReport(string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of top spending customer according to parameters
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns></returns>
        string GetTopSpendingCustomersReport(string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of Abandoned Cart according to parameters
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns></returns>
        string GetAbandonedCartReport(string routeUri, string routeTemplate);

        /// <summary>
        /// Get stores list by according to parameters.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetStoresWithCurrency(string routeUri, string routeTemplate);

        /// <summary>
        /// Get order status list.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetOrderStatus(string routeUri, string routeTemplate);
        /// <summary>
        /// Get order status list.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetDiscountType(string routeUri, string routeTemplate);
        #endregion report
    }
}
