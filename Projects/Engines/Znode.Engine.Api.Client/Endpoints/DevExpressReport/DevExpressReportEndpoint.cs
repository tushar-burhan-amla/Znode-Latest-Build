namespace Znode.Engine.Api.Client.Endpoints
{
    public class DevExpressReportEndpoint : BaseEndpoint
    {
        public static string GetReportCategories() => $"{ApiRoot}/report/categorieslist";
        public static string GetReportDetails(int reportCategoryId) => $"{ApiRoot}/report/getreportdetails/{reportCategoryId}";
        public static string GetReportSetting(string ReportCode) => $"{ApiRoot}/report/GetReportSetting/{ReportCode}";
        public static string GetOrders() => $"{ApiRoot}/report/Orders";
        public static string GetCoupon() => $"{ApiRoot}/report/Coupons";

        public static string GetSalesTax() => $"{ApiRoot}/report/salestax";
        public static string GetAffiliateOrder() => $"{ApiRoot}/report/affiliateorder";
        public static string GetOrderPickList() => $"{ApiRoot}/report/orderpicklist";
        public static string GetBestSellerProduct() => $"{ApiRoot}/report/bestsellerproduct";
        public static string GetInventoryReorder() => $"{ApiRoot}/report/inventoryreorder";
        public static string GetPopularSearch() => $"{ApiRoot}/report/popularsearch";
        public static string GetServiceRequest() => $"{ApiRoot}/report/servicerequest";
        public static string GetVendors() => $"{ApiRoot}/report/vendors";
        public static string GetUsers() => $"{ApiRoot}/report/users";
        public static string GetEmailOptInCustomer() => $"{ApiRoot}/report/emailoptincustomer";
        public static string GetMostFrequentCustomer() => $"{ApiRoot}/report/mostfrequentcustomer";
        public static string GetTopSpendingCustomers() => $"{ApiRoot}/report/topspendingcustomers";
        public static string GetAbandonedCartReport() => $"{ApiRoot}/report/abandonedcart";
        public static string GetStoresWithCurrency() => $"{ApiRoot}/report/storesdetails";
        public static string GetOrderStatus() => $"{ApiRoot}/report/reportorderstatuses";
        public static string GetDiscountType() => $"{ApiRoot}/report/reportdiscounttype";
        public static string GetOrdersItems(int OmsOrderId) => $"{ApiRoot}/report/getorderitemdetails/{OmsOrderId}";
        public static string SaveReportIntoDatabase() => $"{ApiRoot}/report/save";
        public static string LoadSavedReportLayout() => $"{ApiRoot}/report/load";
        public static string GetReportSaveLayout(string reportCode, string reportName) => $"{ApiRoot}/report/load/{reportCode}/{reportName}";
        public static string DeleteSavedReportLayout() => $"{ApiRoot}/report/deletesavedview";
    }
}
