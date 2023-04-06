namespace Znode.Engine.Api.Cache
{
    public interface IDashboardCache
    {
        /// <summary>
        /// Gets the list of top products
        /// </summary>
        /// <param name="routeUri">routeUri for top product list</param>
        /// <param name="routeTemplate">routeTemplate for top product list</param>
        /// <returns>returns the list of top products as string</returns>
        string GetDashboardTopProducts(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets the list of top categories
        /// </summary>
        /// <param name="routeUri">routeUri for top categories list</param>
        /// <param name="routeTemplate">routeTemplate for top categories list</param>
        /// <returns>returns the list of top categories as string</returns>
        string GetDashboardTopCategories(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets the list of top brands
        /// </summary>
        /// <param name="routeUri">routeUri for top brands list</param>
        /// <param name="routeTemplate">routeTemplate for top brands list</param>
        /// <returns>returns the list of top brands as string</returns>
        string GetDashboardTopBrands(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets the list of top searches
        /// </summary>
        /// <param name="routeUri">routeUri for top brands list</param>
        /// <param name="routeTemplate">routeTemplate for top brands list</param>
        /// <returns>returns the list of top searches as string</returns>
        string GetDashboardTopSearches(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets the total orders, total new customers, total sales and average total sales
        /// </summary>
        /// <param name="routeUri">routeUri for total orders, total new customers, total sales and average total sales</param>
        /// <param name="routeTemplate">routeTemplate for total orders, total new customers, total sales and average total sales</param>
        /// <returns>returns total orders, total new customers, total sales and average total saless</returns>
        string GetDashboardSalesDetails(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets dashboard low inventory product count
        /// </summary>
        /// <param name="routeUri">routeUri for low inventory product count</param>
        /// <param name="routeTemplate">routeTemplate for low inventory product count</param>
        /// <returns>returns the dashboard low inventory product count</returns>
        string GetDashboardLowInventoryProductCount(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets the list of Quotes
        /// </summary>
        /// <param name="routeUri">routeUri for Quotes list</param>
        /// <param name="routeTemplate">routeTemplate for Quotes list</param>
        /// <returns>returns the list of Quotes as string</returns>
        string GetDashboardQuotes(string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of orders
        /// </summary>
        /// <param name="routeUri">routeUri for orders list</param>
        /// <param name="routeTemplate">routeTemplate for orders list</param>
        /// <returns>returns the list of orders as string</returns>
        string GetDashboardOrders(string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of returns
        /// </summary>
        /// <param name="routeUri">routeUri for top brands list</param>
        /// <param name="routeTemplate">routeTemplate for top brands list</param>
        /// <returns>returns the list of returns as string</returns>
        string GetDashboardReturns(string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of top accounts
        /// </summary>
        /// <param name="routeUri">routeUri for top brands list</param>
        /// <param name="routeTemplate">routeTemplate for top brands list</param>
        /// <returns>returns the list of top accounts as string</returns>
        string GetDashboardTopAccounts(string routeUri, string routeTemplate);

        /// <summary>
        /// Get dashboard details
        /// </summary>
        /// <param name="routeUri">routeUri for dashboard details</param>
        /// <param name="routeTemplate">routeTemplate for dashboard details</param>
        /// <returns>returns the dashboard details as string</returns>
        string GetDashboardDetails(string routeUri, string routeTemplate);


        /// <summary>
        /// Gets the total orders, total new customers, total sales and average total sales
        /// </summary>
        /// <param name="routeUri">routeUri for total orders, total new customers, total sales and average total sales</param>
        /// <param name="routeTemplate">routeTemplate for total orders, total new customers, total sales and average total sales</param>
        /// <returns>returns total orders, total new customers, total sales and average total saless</returns>
        string GetDashboardSalesCountDetails(string routeUri, string routeTemplate);
        /// <summary>
        /// Get list of sale details
        /// </summary>
        /// <param name="routeUri">routeUri for sale details</param>
        /// <param name="routeTemplate">routeTemplate for sale details</param>
        /// <returns>returns the list of sale details</returns>
        string GetDashboardSaleDetails(string routeUri, string routeTemplate);

        /// <summary>
        /// Get account and store list
        /// </summary>
        /// <param name="routeUri">routeUri for account and store list</param>
        /// <param name="routeTemplate">routeTemplate for account and store list</param>
        /// <returns>returns the list of account and store list</returns>
        string GetAccountAndStoreList(string routeUri, string routeTemplate);

    }
}
