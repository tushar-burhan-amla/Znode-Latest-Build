using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Agents
{
    public interface IDashboardAgent
    {
        /// <summary>
        /// Gets the top brands list
        /// <param name="portalId">portalId provided to get top brands list</param>
        /// </summary>
        /// <returns>returns the list of top brands</returns>
        Task<DashboardListViewModel> GetDashboardTopBrands(int portalId);

        /// <summary>
        /// Gets the top categories list
        /// </summary>
        /// <returns>returns the list of top categories</returns>
        Task<DashboardListViewModel> GetDashboardTopCategories();

        /// <summary>
        /// Gets the top products list
        /// <param name="portalId">portalId provided to get top products list</param>
        /// </summary>
        /// <returns>returns the list of top products</returns>
        Task<DashboardListViewModel> GetDashboardTopProducts(int portalId);

        /// <summary>
        /// Gets the top searches list
        /// <param name="portalId">portalId provided to get top searches list</param>
        /// </summary>
        /// <returns>returns the list of top searches</returns>
        Task<DashboardListViewModel> GetDashboardTopSearches(int portalId);

        /// <summary>
        /// Gets the total orders, total sales, total new customers, total average orders list
        /// <param name="portalId">portalId provided to get sales details list</param>
        /// </summary>
        /// <returns>returns the list of total orders, total sales, total new customers, total average orders</returns>
        Task<DashboardListViewModel> GetDashboardSalesDetails(int portalId);

        /// <summary>
        /// Gets the count of low inventory products       
        /// </summary>
        /// <param name="portalId">portalId provided to get sales details list</param>
        /// <returns>returns the low inventory product count</returns>
        Task<DashboardListViewModel> GetDashboardLowInventoryProductCount(int portalId = 0);

        /// <summary>
        /// Gets the list of portals        
        /// </summary>
        /// <returns>returns the list of portals</returns>
        List<SelectListItem> GetStoreList();

        /// <summary>
        /// Gets the list of Accounts        
        /// </summary>
        /// <returns>returns the list of portals</returns>
        List<SelectListItem> GetAccountList();

        /// <summary>
        /// Gets the top Quotes list
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<DashboardItemsListViewModel> GetDashboardQuotes(int portalId, int accountId);

        /// <summary>
        /// Gets the top Orders list
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<DashboardItemsListViewModel> GetDashboardOrders(int portalId, int accountId);

        /// <summary>
        /// Gets the top Returns list
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<DashboardItemsListViewModel> GetDashboardReturns(int portalId, int accountId);

        /// <summary>
        /// Gets the top SaleDetails list
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<DashboardItemsListViewModel> GetDashboardDetails(int portalId, int accountId);

        /// <summary>
        /// Gets the top Top Account list
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<DashboardItemsListViewModel> GetDashboardTopAccounts(int portalId, int accountId);


        /// <summary>
        /// Gets the total orders, total sales, total new customers, total average orders list
        /// <param name="portalId">portalId provided to get sales details list</param>
        /// </summary>
        /// <returns>returns the list of total orders, total sales, total new customers, total average orders</returns>
        Task<DashboardListViewModel> GetDashboardSalesCountDetails(int portalId);

        /// <summary>
        /// Gets the sale details
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<DashboardItemsListViewModel> GetDashboardSaleDetails(int portalId = 0, int accountId = 0);

        /// <summary>
        /// Gets account and store list
        /// </summary>
        /// <param name="dashboard"></param>
        /// <returns></returns>
        void GetAccountAndStoreList(DashboardViewModel dashboard);

    }
}
