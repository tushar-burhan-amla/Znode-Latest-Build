using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IDevExpressReportClient
    {
        /// <summary>
        /// Get all active report categories. 
        /// </summary>
        /// <returns>Report category list model</returns>
        ReportCategoryListModel GetReportCategories();

        /// <summary>
        /// Get all active reports details by report category Id.
        /// </summary>
        /// <param name="reportCategoryId"></param>
        /// <returns>Report detail list model</returns>
        ReportDetailListModel GetReportDetails(int reportCategoryId);

        /// <summary>
        /// Get report setting by report code
        /// </summary>
        /// <param name="ReportCode"></param>
        /// <returns>Report setting model</returns>
        ReportSettingModel GetReportSetting(string ReportCode);

        /// <summary>
        /// Get orders list by report filters
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Order details model.</returns>
        List<ReportOrderDetailsModel> GetOrdersReport(FilterCollection filters = null);

        /// <summary>
        /// Get orders item details by oms order id
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Order Items Details Model.</returns>
        List<ReportOrderItemsDetailsModel> GetOrdersItemsReport(int OmsOrderId);


        /// <summary>
        /// Get coupons usages details for report
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        List<ReportCouponModel> GetCouponReport(FilterCollection filters = null);

        /// <summary>
        /// Get sales tax details for report.
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        List<ReportSalesTaxModel> GetSalesTaxReport(FilterCollection filters = null);

        /// <summary>
        /// Get affiliate orders details for report
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        List<ReportAffiliateOrderModel> GetAffiliateOrderReport(FilterCollection filters = null);

        /// <summary>
        /// Get Orders pick list for report. 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        List<ReportOrderPickModel> GetOrderPickListReport(FilterCollection filters = null);

        /// <summary>
        /// Get best seller products list for report
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        List<ReportBestSellerProductModel> GetBestSellerProductReport(FilterCollection filters = null);

        /// <summary>
        /// Get inventory list for report
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        List<ReportInventoryReorderModel> GetInventoryReorderReport(FilterCollection filters = null);

        /// <summary>
        /// Get popular search products list for report 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        List<ReportPopularSearchModel> GetPopularSearchReport(FilterCollection filters = null);

        /// <summary>
        /// Get service request list for report
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        List<ReportServiceRequestModel> GetServiceRequestReport(FilterCollection filters = null);

        /// <summary>
        /// Get users list for report
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        List<ReportUsersModel> GetUsersReport(FilterCollection filters = null);

        /// <summary>
        /// Get customers list for report
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        List<ReportEmailOptInCustomerModel> GetEmailOptInCustomerReport(FilterCollection filters = null);

        /// <summary>
        /// Get most frequent customers list for report
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        List<ReportMostFrequentCustomerModel> GetMostFrequentCustomerReport(FilterCollection filters = null);

        /// <summary>
        /// Get top spending customers for report
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        List<ReportTopSpendingCustomersModel> GetTopSpendingCustomersReport(FilterCollection filters = null);

        /// <summary>
        /// Get vendors list for report
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        List<ReportVendorsModel> GetVendorsReport(FilterCollection filters = null);

        /// <summary>
        /// Get Abandoned Cart list for report
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        List<ReportAbandonedCartModel> GetAbandonedCartReport(FilterCollection filters = null);
        
        /// <summary>
        /// Get list of stores with currency
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        List<ReportStoresDetailsModel> GetStoresWithCurrency(FilterCollection filters = null);

        /// <summary>
        /// Get list of Order status
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        List<ReportOrderStatusModel> GetOrderStatus(FilterCollection filters = null);

        /// <summary>
        /// Get list of discounts type
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        List<ReportDiscountTypeModel> GetDiscountType(FilterCollection filters = null);

        /// <summary>
        /// To save the report Layout into the database
        /// </summary>
        /// <param name="reportModel"></param>
        ReportViewModel SaveReportIntoDatabase(ReportViewModel reportModel);

        /// <summary>
        /// To save the report Layout into the database
        /// </summary>
        /// <param name="reportModel"></param>
        List<ReportViewModel> LoadSavedReportLayout(ReportViewModel reportModel);

        /// <summary>
        ///Get report layout by report code and name.  
        /// </summary>
        /// <param name="reportCode"></param>
        /// <param name="reportName"></param>
        /// <returns></returns>
        ReportViewModel GetReportSaveLayout(string reportCode, string reportName);

        /// <summary>
        /// This method is used to delete the save report layout view from the database.
        /// </summary>
        /// <param name="reportViewId"></param>
        bool DeleteSavedReportLayout(int reportViewId);
    }
}
