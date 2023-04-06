using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IDevExpressReportService
    {
        /// <summary>
        /// Get active report categories
        /// </summary>
        /// <returns></returns>
        List<ReportCategoryModel> GetReportCategories();

        /// <summary>
        /// Get Reports details by report category Id.
        /// </summary>
        /// <param name="reportCategoryId"></param>
        /// <returns></returns>
        List<ReportDetailModel> GetReportDetails(int reportCategoryId);

        /// <summary>
        /// Get report setting by report code
        /// </summary>
        /// <param name="ReportCode"></param>
        /// <returns></returns>
        ReportSettingModel GetReportSetting(string ReportCode);

        /// <summary>
        /// Save the report view into the database.
        /// </summary>
        /// <param name="reportViewModel"></param>
        ReportViewModel SaveReportLayout(ReportViewModel reportViewModel);

        /// <summary>
        /// Get report layout for given report model
        /// </summary>
        /// <param name="savedReportViewModel"></param>
        /// <returns></returns>
        List<ReportViewModel> GetSavedReportLayouts(ReportViewModel reportViewModel);

        /// <summary>
        /// Get saved report layout from database.
        /// </summary>
        /// <param name="reportCode"></param>
        /// <param name="reportName"></param>
        /// <returns></returns>
        ReportViewModel GetSavedReportLayout(string reportCode, string reportName);

        /// <summary>
        /// This method is use for deleting saved report layout by reportviewId from database.
        /// </summary>
        /// <param name="reportViewId"></param>
        /// <returns></returns>
        bool DeleteSavedReportLayout(int reportViewId);

        /// <summary>
        /// This method is used to get order details for report
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters.</param>
        /// <returns>list of ReportOrderDetailsModel.</returns>
        List<ReportOrderDetailsModel> GetOrdersReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// This method is used to get used coupons details for report
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters.</param>
        /// <returns>list of ReportCouponModel.</returns>
        List<ReportCouponModel> GetCouponReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get sale tax report
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters.</param>
        /// <returns>list of ReportSalesTaxModel.</returns>
        List<ReportSalesTaxModel> GetSalesTaxReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get affiliate orders for report.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters.</param>
        /// <returns>list of ReportAffiliateOrderModel.</returns>
        List<ReportAffiliateOrderModel> GetAffiliateOrderReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get order pick list for report.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">list of ReportOrderPickModel.</param>
        /// <returns></returns>
        List<ReportOrderPickModel> GetOrderPickListReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get order items detail by oms order id.
        /// </summary>
        /// <param name="omsOrderId"></param>
        /// <returns></returns>
        List<ReportOrderItemsDetailsModel> GetOrdersItemsReport(int omsOrderId);

        #region report
        /// <summary>
        /// Get list of best seller product according to parameters
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters.</param>
        /// <returns>list of ReportBestSellerProductModel.</returns>
        List<ReportBestSellerProductModel> GetBestSellerProductReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get list of inventory according to parameters.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters.</param>
        /// <returns>list of ReportInventoryReorderModel.</returns>
        List<ReportInventoryReorderModel> GetInventoryReorderReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);


        /// <summary>
        /// Get list of popular search product according to parameters.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters.</param>
        /// <returns>list of ReportPopularSearchModel.</returns>
        List<ReportPopularSearchModel> GetPopularSearchReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);
        #endregion report

        #region Report
        /// <summary>
        /// Get list of user according to parameters
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters.</param>
        /// <returns>list of ReportUsersModel.</returns>
        List<ReportUsersModel> GetUsersReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get list of email opt customer according to parameters
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters.</param>
        /// <returns>list of ReportEmailOptInCustomerModel.</returns>
        List<ReportEmailOptInCustomerModel> GetEmailOptInCustomerReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get list of most frequent customer according to parameters
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters.</param>
        /// <returns>list of ReportMostFrequentCustomerModel.</returns>
        List<ReportMostFrequentCustomerModel> GetMostFrequentCustomerReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get list of top spending customer according to parameters
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters.</param>
        /// <returns>list of ReportTopSpendingCustomersModel.</returns>
        List<ReportTopSpendingCustomersModel> GetTopSpendingCustomersReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get list of Abandoned Cart according to parameters
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters.</param>
        /// <returns>list of ReportAbandonedCartModel.</returns>
        List<ReportAbandonedCartModel> GetAbandonedCartReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get stores list by filters.
        /// </summary>
        /// <param name="expands"></param>
        /// <param name="filters"></param>
        /// <param name="sorts"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        List<ReportStoresDetailsModel> GetStoresWithCurrency(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get order states.
        /// </summary>
        /// <returns></returns>
        List<ReportOrderStatusModel> GetOrderStatus(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get order discount type.
        /// </summary>
        /// <returns></returns>
        List<ReportDiscountTypeModel> GetDiscountType(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);
        #endregion report
    }
}
