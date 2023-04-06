using System.Threading.Tasks;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IDashboardClient : IBaseClient
    {
        /// <summary>
        /// Gets top products list
        /// </summary>
        /// <param name="filters">filters for top products list</param>
        /// <param name="sorts">sorts for top products list</param>
        /// <param name="pageIndex">pageIndex for top products list</param>
        /// <param name="pageSize">pageSize for top products list</param>
        /// <returns>returns the list of top products</returns>
        Task<DashboardTopItemsListModel> GetDashboardTopProductsList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets top categories list
        /// </summary>
        /// <param name="filters">filters for top categories list</param>
        /// <param name="sorts">sorts for top categories list</param>
        /// <param name="pageIndex">pageIndex for top categories list</param>
        /// <param name="pageSize">pageSize for top categories list</param>
        /// <returns>returns the list of top categories</returns>
        Task<DashboardTopItemsListModel> GetDashboardTopCategoriesList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets the top brands list
        /// </summary>
        /// <param name="filters">filters for top brands list</param>
        /// <param name="sorts">sorts for top brands list</param>
        /// <param name="pageIndex">pageIndex for top brands list</param>
        /// <param name="pageSize">pageSize for top brands list</param>
        /// <returns>returns the list of top brands</returns>
        Task<DashboardTopItemsListModel> GetDashboardTopBrandsList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets the top searches list
        /// </summary>
        /// <param name="filters">filters for top searches list</param>
        /// <param name="sorts">sorts for top searches list</param>
        /// <param name="pageIndex">pageIndex for top searches list</param>
        /// <param name="pageSize">pageSize for top searches list</param> 
        /// <returns>returns the list of top searches</returns>
        Task<DashboardTopItemsListModel> GetDashboardTopSearchesList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets total sales, total orders, total customers and average orders
        /// </summary>
        /// <param name="filters">filters for revenue details list</param>
        /// <param name="sorts">sorts for revenue details list</param>
        /// <param name="pageIndex">pageIndex for revenue details list</param>
        /// <param name="pageSize">pageSize for revenue details list</param> 
        /// <returns>returns total sales, total orders, total customers and average orders</returns>
        Task<DashboardTopItemsListModel> GetDashboardSalesDetails(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// //Gets the count of low inventory products
        /// </summary>
        /// <param name="filters">filters for revenue details list</param>
        /// <param name="sorts">sorts for revenue details list</param>
        /// <param name="pageIndex">pageIndex for revenue details list</param>
        /// <param name="pageSize">pageSize for revenue details list</param>
        /// <returns>returns the count of low inventory products</returns>
        Task<DashboardTopItemsListModel> GetDashboardLowInventoryProductCount(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// //Gets the Quotes list
        /// </summary>
        /// <param name="filters">filters for Quotes list</param>
        /// <param name="sorts">sorts for Quotes list</param>
        /// <param name="pageIndex">pageIndex for Quotes list</param>
        /// <param name="pageSize">pageSize for Quotes list</param>
        /// <returns>returns the list of Quotes</returns>
        Task<DashboardItemsListModel> GetDashboardQuotes(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// //Gets the orders list
        /// </summary>
        /// <param name="filters">filters for orders list</param>
        /// <param name="sorts">sorts for orders list</param>
        /// <param name="pageIndex">pageIndex for orders list</param>
        /// <param name="pageSize">pageSize for orders list</param>
        /// <returns>returns the list of orders</returns>
        Task<DashboardItemsListModel> GetDashboardOrders(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// //Gets the returns list 
        /// </summary>
        /// <param name="filters">filters for returns list</param>
        /// <param name="sorts">sorts for returns list</param>
        /// <param name="pageIndex">pageIndex for returns list</param>
        /// <param name="pageSize">pageSize for returns list</param>
        /// <returns>returns the list of returns</returns>
        Task<DashboardItemsListModel> GetDashboardReturns(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// //Gets the top accounts list 
        /// </summary>
        /// <param name="filters">filters for top accounts list</param>
        /// <param name="sorts">sorts for top accounts list</param>
        /// <param name="pageIndex">pageIndex for top accounts list</param>
        /// <param name="pageSize">pageSize for top accounts list</param>
        /// <returns>returns the top accounts list</returns>
        Task<DashboardItemsListModel> GetDashboardTopAccounts(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets total sales, total orders, count of quotes and count of returns
        /// </summary>
        /// <param name="filters">filters for dashboard details list</param>
        /// <param name="sorts">sorts for dashboard details list</param>
        /// <param name="pageIndex">pageIndex for dashboard details list</param>
        /// <param name="pageSize">pageSize for dashboard details list</param> 
        /// <returns>returns total sales, total orders, count of quotes and count of returns</returns>
        Task<DashboardItemsListModel> GetDashboardDetails(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets total sales, total orders, total customers and average orders
        /// </summary>
        /// <param name="filters">filters for revenue details list</param>
        /// <param name="sorts">sorts for revenue details list</param>
        /// <param name="pageIndex">pageIndex for revenue details list</param>
        /// <param name="pageSize">pageSize for revenue details list</param> 
        /// <returns>returns total sales, total orders, total customers and average orders</returns>
        Task<DashboardTopItemsListModel> GetDashboardSalesCountDetails(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);


        /// <summary>
        /// //Gets the sale details 
        /// </summary>
        /// <param name="filters">filters for sales details</param>
        /// <param name="sorts">sorts for sales details</param>
        /// <param name="pageIndex">pageIndex for sales details</param>
        /// <param name="pageSize">pageSize for sales details</param>
        /// <returns>returns the sales details</returns>
        Task<DashboardItemsListModel> GetDashboardSaleDetails(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// //Gets the account and store list. 
        /// </summary>
        /// <param name="filters">filters for account and store list.</param>
        /// <param name="sorts">sorts for account and store list.</param>
        /// <param name="pageIndex">pageIndex for account and store list.</param>
        /// <param name="pageSize">pageSize for account and store list.</param>
        /// <returns>returns the account and store list.</returns>
        DashboardDropDownListModel GetAccountAndStoreList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

    }
}
