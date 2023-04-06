using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class DevExpressReportCache : BaseCache, IDevExpressReportCache
    {
        #region Private Variables
        private readonly IDevExpressReportService _devExpressReportService;
        #endregion

        #region Constructor
        public DevExpressReportCache(IDevExpressReportService service)
        {
            _devExpressReportService = service;
        }
        #endregion

        #region Public Methods
        //Get report categories.
        public string GetReportCategories(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                ReportCategoryListResponse response = new ReportCategoryListResponse { ReportCategoryList = _devExpressReportService.GetReportCategories() };
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        //Get report details.
        public string GetReportDetails(string routeUri, string routeTemplate, int reportCategoryId)
        {
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                ReportDetailListResponse response = new ReportDetailListResponse { ReportDetailList = _devExpressReportService.GetReportDetails(reportCategoryId) };
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        //Get report setting.
        public string GetReportSetting(string routeUri, string routeTemplate, string ReportCode)
        {
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                ReportSettingResponse response = new ReportSettingResponse { ReportSetting = _devExpressReportService.GetReportSetting(ReportCode) };
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        //Get report layout.
        public string GetSavedReportLayout(string routeUri, string routeTemplate, string ReportCode, string ReportName)
        {
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                ReportViewResponse response = new ReportViewResponse { };
                response.ReportView = _devExpressReportService.GetSavedReportLayout(ReportCode, ReportName);
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        // Get order list for report.
        public virtual string GetOrdersReport(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                OrderDetailsListResponse response = new OrderDetailsListResponse() { OrderDetailsList = _devExpressReportService.GetOrdersReport(Expands, Filters, Sorts, Page) };
                data = InsertIntoCache(routeUri, routeTemplate, response);
                return data;
            }

            return data;
        }

        // Get order list for report.
        public virtual string GetCouponReport(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ReportCouponListResponse response = new ReportCouponListResponse() { CouponList = _devExpressReportService.GetCouponReport(Expands, Filters, Sorts, Page) };
                data = InsertIntoCache(routeUri, routeTemplate, response);
                return data;
            }

            return data;
        }

        //Get sale tax details for report.
        public virtual string GetSalesTaxReport(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ReportSalesTaxListResponse response = new ReportSalesTaxListResponse() { ReportSalesTaxList = _devExpressReportService.GetSalesTaxReport(Expands, Filters, Sorts, Page) };
                data = InsertIntoCache(routeUri, routeTemplate, response);
                return data;
            }

            return data;
        }

        // Get affiliate orders for report.
        public virtual string GetAffiliateOrderReport(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ReportAffiliateOrderListResponse response = new ReportAffiliateOrderListResponse() { ReportAffiliateOrderList = _devExpressReportService.GetAffiliateOrderReport(Expands, Filters, Sorts, Page) };
                data = InsertIntoCache(routeUri, routeTemplate, response);
                return data;
            }

            return data;
        }

        // Get order pick list for report.
        public virtual string GetOrderPickListReport(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ReportOrderPickListResponse response = new ReportOrderPickListResponse() { ReportOrderPickList = _devExpressReportService.GetOrderPickListReport(Expands, Filters, Sorts, Page) };
                data = InsertIntoCache(routeUri, routeTemplate, response);
                return data;
            }

            return data;
        }
        #endregion

        #region report
        public virtual string GetBestSellerProductReport(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ReportBestSellerProductResponse response = new ReportBestSellerProductResponse() { ReportBestSellerProductList = _devExpressReportService.GetBestSellerProductReport(Expands, Filters, Sorts, Page) };
                data = InsertIntoCache(routeUri, routeTemplate, response);
                return data;
            }

            return data;
        }
        public virtual string GetInventoryReorderReport(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ReportInventoryReorderListResponse response = new ReportInventoryReorderListResponse() { ReportInventoryReorderList = _devExpressReportService.GetInventoryReorderReport(Expands, Filters, Sorts, Page) };
                data = InsertIntoCache(routeUri, routeTemplate, response);
                return data;
            }

            return data;
        }
        public virtual string GetPopularSearchReport(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ReportPopularSearchListResponse response = new ReportPopularSearchListResponse() { ReportPopularSearchList = _devExpressReportService.GetPopularSearchReport(Expands, Filters, Sorts, Page) };
                data = InsertIntoCache(routeUri, routeTemplate, response);
                return data;
            }

            return data;
        }
        #endregion report

        #region report
        // Get list of user.
        public virtual string GetUsersReport(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ReportUsersListResponse response = new ReportUsersListResponse() { UsersList = _devExpressReportService.GetUsersReport(Expands, Filters, Sorts, Page) };
                data = InsertIntoCache(routeUri, routeTemplate, response);
                return data;
            }

            return data;
        }

        // Get list of email opt customer.
        public virtual string GetEmailOptInCustomerReport(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ReportEmailOptInCustomerListResponse response = new ReportEmailOptInCustomerListResponse() { EmailOptInCustomerList = _devExpressReportService.GetEmailOptInCustomerReport(Expands, Filters, Sorts, Page) };
                data = InsertIntoCache(routeUri, routeTemplate, response);
                return data;
            }

            return data;
        }

        // Get list of most frequent customer.
        public virtual string GetMostFrequentCustomerReport(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ReportMostFrequentCustomerListResponse response = new ReportMostFrequentCustomerListResponse() { FrequentCustomerList = _devExpressReportService.GetMostFrequentCustomerReport(Expands, Filters, Sorts, Page) };
                data = InsertIntoCache(routeUri, routeTemplate, response);
                return data;
            }

            return data;
        }

        // Get list of top spending customer.
        public virtual string GetTopSpendingCustomersReport(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ReportTopSpendingCustomersListResponse response = new ReportTopSpendingCustomersListResponse() { SpendingCustomersList = _devExpressReportService.GetTopSpendingCustomersReport(Expands, Filters, Sorts, Page) };
                data = InsertIntoCache(routeUri, routeTemplate, response);
                return data;
            }

            return data;
        }

        // Get list of Abandoned Cart.
        public virtual string GetAbandonedCartReport(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ReportAbandonedCartListResponse response = new ReportAbandonedCartListResponse() { AbandonedCartList = _devExpressReportService.GetAbandonedCartReport(Expands, Filters, Sorts, Page) };
                data = InsertIntoCache(routeUri, routeTemplate, response);
                return data;
            }

            return data;
        }

        // Get stores list by filters.
        public virtual string GetStoresWithCurrency(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ReportStoresDetailsListResponse response = new ReportStoresDetailsListResponse() { StoresDetails = _devExpressReportService.GetStoresWithCurrency(Expands, Filters, Sorts, Page) };
                data = InsertIntoCache(routeUri, routeTemplate, response);
                return data;
            }

            return data;
        }

        // Get Order statuses list.
        public virtual string GetOrderStatus(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ReportOrderStatusListResponse response = new ReportOrderStatusListResponse() { OrderStatusList = _devExpressReportService.GetOrderStatus(Expands, Filters, Sorts, Page) };
                data = InsertIntoCache(routeUri, routeTemplate, response);
                return data;
            }

            return data;
        }

        // Get Order discount type list.
        public virtual string GetDiscountType(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ReportDiscountTypeListResponse response = new ReportDiscountTypeListResponse() { DiscountTypeList = _devExpressReportService.GetDiscountType(Expands, Filters, Sorts, Page) };
                data = InsertIntoCache(routeUri, routeTemplate, response);
                return data;
            }

            return data;
        }
        #endregion report
    }
}
