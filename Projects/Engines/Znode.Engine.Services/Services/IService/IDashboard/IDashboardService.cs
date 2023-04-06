using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IDashboardService
    {
        /// <summary>
        /// Gets the list of top brands
        /// </summary>
        /// <param name="filters">filters for top brands list</param>
        /// <param name="sorts">sorts for top brands list</param>
        /// <param name="page">page for top brands list</param>
        /// <returns>returns the list of top brands list</returns>
        DashboardTopItemsListModel GetDashboardTopBrands(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets the list of top categories
        /// </summary>
        /// <param name="filters">filters for top categories list</param>
        /// <param name="sorts">sorts for top categories list</param>
        /// <param name="page">page for top categories list</param>
        /// <returns>returns the list of top categories list</returns>
        DashboardTopItemsListModel GetDashboardTopCategories(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets the list of top products
        /// </summary>
        /// <param name="filters">filters for top products list</param>
        /// <param name="sorts">sorts for top products list</param>
        /// <param name="page">page for top products list</param>
        /// <returns>returns the list of top products list</returns>
        DashboardTopItemsListModel GetDashboardTopProducts(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets the list of top searches
        /// </summary>
        /// <param name="filters">filters for top searches list</param>
        /// <param name="sorts">sorts for top searches list</param>
        /// <param name="page">page for top searches list</param>
        /// <returns>returns the list of top searches list</returns>
        DashboardTopItemsListModel GetDashboardTopSearches(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets total sales, total orders, total customers and average orders
        /// </summary>
        /// <param name="filters">filters for total parameters</param>
        /// <param name="sorts">sorts for total parameters</param>
        /// <param name="page">page for top searches list</param>
        /// <returns>returns total sales, total orders, total customers and average orders</returns>
        DashboardTopItemsListModel GetDashboardSalesDetails(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);
       
        /// <summary>
        /// Gets dashboard low inventory product count
        /// </summary>
        /// <param name="filters">filters for total parameters</param>
        /// <param name="sorts">sorts for total parameters</param>
        /// <param name="page">page for top searches list</param>
        /// <returns>returns dashboard low inventory product count</returns>
        DashboardTopItemsListModel GetDashboardLowInventoryProductCount(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets dashboard quotes
        /// </summary>
        /// <param name="filters">filters for total parameters</param>
        /// <param name="sorts">sorts for total parameters</param>
        /// <param name="page">page for top quotes list</param>
        /// <returns>returns dashboard quotes</returns>
        DashboardItemsListModel GetDashboardQuotes(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets dashboard Orders
        /// </summary>
        /// <param name="filters">filters for total parameters</param>
        /// <param name="sorts">sorts for total parameters</param>
        /// <param name="page">page for top Orders list</param>
        /// <returns>returns dashboard Orders</returns>
        DashboardItemsListModel GetDashboardOrders(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets dashboard Returns
        /// </summary>
        /// <param name="filters">filters for total parameters</param>
        /// <param name="sorts">sorts for total parameters</param>
        /// <param name="page">page for top Returns list</param>
        /// <returns>returns dashboard Returns</returns>
        DashboardItemsListModel GetDashboardReturns(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets dashboard top accounts
        /// </summary>
        /// <param name="filters">filters for total parameters</param>
        /// <param name="sorts">sorts for total parameters</param>
        /// <param name="page">page for top accounts list</param>
        /// <returns>returns dashboard top accounts</returns>
        DashboardItemsListModel GetDashboardTopAccounts(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets dashboard details
        /// </summary>
        /// <param name="filters">filters for total sales, total orders, count of quotes and count of returns</param>
        /// <param name="sorts">sorts for total sales, total orders, count of quotes and count of returns</param>
        /// <param name="page">page for total sales, total orders, count of quotes and count of returns</param>
        /// <returns>returns dashboard total sales, total orders, count of quotes and count of returns</returns>
        DashboardItemsListModel GetDashboardDetails(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets total sales, total orders, total customers and average orders
        /// </summary>
        /// <param name="filters">filters for total parameters</param>
        /// <param name="sorts">sorts for total parameters</param>
        /// <param name="page">page for top searches list</param>
        /// <returns>returns total sales, total orders, total customers and average orders</returns>
        DashboardTopItemsListModel GetDashboardSalesCountDetails(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);


        /// <summary>
        /// Gets dashboard sales details
        /// </summary>
        /// <param name="filters">filters for sales details</param>
        /// <param name="sorts">sorts for sales details</param>
        /// <param name="page">page for sales details</param>
        /// <returns>returns dashboard sales details</returns>
        DashboardItemsListModel GetDashboardSalesInfo(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets dashboard account and store list.
        /// </summary>
        /// <param name="filters">filters for account and store list.</param>
        /// <param name="sorts">sorts for account and store list.</param>
        /// <param name="page">page for account and store list.</param>
        /// <returns>returns dashboard account and store list.</returns>
        DashboardDropDownListModel GetAccountAndStoreList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

    }
}
