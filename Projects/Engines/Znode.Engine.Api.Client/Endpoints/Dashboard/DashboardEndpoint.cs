namespace Znode.Engine.Api.Client.Endpoints
{
    public class DashboardEndpoint : BaseEndpoint
    {
        //Gets the list of top products
        public static string GetDashboardTopProductList() => $"{ApiRoot}/dashboard/getdashboardtopproductlist";

        //Gets the list of top categories
        public static string GetDashboardTopCategoriesList() => $"{ApiRoot}/dashboard/getdashboardtopcategorieslist";

        //Gets the list of top brands
        public static string GetDashboardTopBrandsList() => $"{ApiRoot}/dashboard/getdashboardtopbrandslist";

        //Gets the list of top searches
        public static string GetDashboardTopSearchesList() => $"{ApiRoot}/dashboard/getdashboardtopsearcheslist";

        //Gets the list of total orders, total sales, total new customers, total average orders
        public static string GetDashboardSalesDetails() => $"{ApiRoot}/dashboard/getdashboardsalesdetails";

        //Gets the count of low inventory products
        public static string GetDashboardLowInventoryProductsCount() => $"{ApiRoot}/dashboard/getdashboardlowinventoryproductcount";

        //Gets the list of Quotes
        public static string GetDashboardQuotes() => $"{ApiRoot}/dashboard/GetDashboardQuotes";

        //Gets the list of Orders
        public static string GetDashboardOrders() => $"{ApiRoot}/dashboard/GetDashboardOrders";

        //Gets the list of Returns
        public static string GetDashboardReturns() => $"{ApiRoot}/dashboard/GetDashboardReturns";

        //Gets the list of Top Accounts
        public static string GetDashboardTopAccounts() => $"{ApiRoot}/dashboard/GetDashboardTopAccounts";

        //Gets the total sales, total orders, count of quotes and count of returns
        public static string GetDashboardDetails() => $"{ApiRoot}/dashboard/getdashboarddetails";


        //Gets the list of total orders, total sales, total new customers, total average orders
        public static string GetDashboardSalesCountDetails() => $"{ApiRoot}/dashboard/getdashboardsalescountdetails";
        //Gets the sales details
        public static string GetDashboardSaleDetails() => $"{ApiRoot}/dashboard/getdashboardsaledetails";

        //Gets the account and store list.
        public static string GetAccountAndStoreList() => $"{ApiRoot}/dashboard/getaccountandstorelist";

    }
}
