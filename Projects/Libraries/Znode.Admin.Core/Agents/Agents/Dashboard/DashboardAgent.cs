using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Admin.Agents
{
    public class DashboardAgent : BaseAgent, IDashboardAgent
    {
        #region Private Variables      
        private readonly IDashboardClient _dashboardClient;
        private readonly IAccountClient _accountClient;
        private readonly IPortalClient _portalClient;
        #endregion

        #region Constructor
        public DashboardAgent(IDashboardClient dashboardClient, IPortalClient portalClient, IAccountClient accountClient)
        {
            _dashboardClient = GetClient(dashboardClient);
            _accountClient = GetClient(accountClient);
            _portalClient = GetClient(portalClient);
        }
        #endregion

        #region public Methods
        //Gets the list of dashboard top brands
        public virtual async Task<DashboardListViewModel> GetDashboardTopBrands(int portalId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
            //Binds the portalId filter to get brand list according to selected portal
            FilterCollection filters = BindPortalFilter(ref portalId);

            DashboardTopItemsListModel dashboardTopItemsListModel = await _dashboardClient.GetDashboardTopBrandsList(filters, null, null, null);
            ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
            //Converts Top Brands Model List into Top Brands View Model List with paging data
            return ConvertListModelToListViewModel(dashboardTopItemsListModel);
        }

        //Gets the list of dashboard top categories
        public virtual async Task<DashboardListViewModel> GetDashboardTopCategories()
        {
            DashboardTopItemsListModel dashboardTopItemsListModel = await _dashboardClient.GetDashboardTopCategoriesList(null, null, null, null);

            //Converts Top Categories Model List into Top Categories View Model List with paging data
            return ConvertListModelToListViewModel(dashboardTopItemsListModel);
        }

        //Gets the list of dashboard top products
        public virtual async Task<DashboardListViewModel> GetDashboardTopProducts(int portalId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
            //Binds the portalId filter to get product list according to selected portal
            FilterCollection filters = BindPortalFilter(ref portalId);

            DashboardTopItemsListModel dashboardTopItemsListModel = await _dashboardClient.GetDashboardTopProductsList(filters, null, null, null);
            ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
            //Converts Top Products Model List into Top Products View Model List with paging data
            return ConvertListModelToListViewModel(dashboardTopItemsListModel);
        }

        //Gets the list of dashboard top searches
        public virtual async Task<DashboardListViewModel> GetDashboardTopSearches(int portalId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
            //Binds the portalId filter to get searches list according to selected portal
            FilterCollection filters = BindPortalFilter(ref portalId);

            DashboardTopItemsListModel dashboardTopItemsListModel = await _dashboardClient.GetDashboardTopSearchesList(filters, null, null, null);
            ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
            //Converts Top Searches Model List into Top Searches View Model List with paging data
            return ConvertListModelToListViewModel(dashboardTopItemsListModel);
        }

        //Gets the list of dashboard total sales, total orders, total new customers, total avg orders
        public virtual async Task<DashboardListViewModel> GetDashboardSalesDetails(int portalId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
            //Binds the portalId filter to get sales list according to selected portal
            FilterCollection filters = BindPortalFilter(ref portalId);

            DashboardTopItemsListModel dashboardTopItemsListModel = await _dashboardClient.GetDashboardSalesDetails(filters, null, null, null);
            ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
            //Converts Top Sales Model List into Top Sales View Model List with paging data
            return ConvertListModelToListViewModel(dashboardTopItemsListModel);
        }

        //Gets the count of low inventory products
        public virtual async Task<DashboardListViewModel> GetDashboardLowInventoryProductCount(int portalId=0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
            //Binds the portalId filter to get sales list according to selected portal
            FilterCollection filters = BindPortalFilter(ref portalId);

            DashboardTopItemsListModel dashboardTopItemsListModel = await _dashboardClient.GetDashboardLowInventoryProductCount(filters, null, null, null);
            DashboardListViewModel listViewModel = new DashboardListViewModel { DashboardViewModelList = dashboardTopItemsListModel?.TopItemsList?.ToViewModel<DashboardTopItemsViewModel>().ToList() };
            ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
            return dashboardTopItemsListModel?.TopItemsList?.Count > 0 ? listViewModel : new DashboardListViewModel { DashboardViewModelList = new List<DashboardTopItemsViewModel>() };
        }

        //Gets the list of dashboard total sales, total orders, total new customers, total avg orders
        public virtual async Task<DashboardListViewModel> GetDashboardSalesCountDetails(int portalId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
            //Binds the portalId filter to get sales list according to selected portal
            FilterCollection filters = BindPortalFilter(ref portalId);

            DashboardTopItemsListModel dashboardTopItemsListModel = await _dashboardClient.GetDashboardSalesCountDetails(filters, null, null, null);
            ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
            //Converts Top Sales Model List into Top Sales View Model List with paging data
            return ConvertListModelToListViewModel(dashboardTopItemsListModel);
        }

        #endregion

        #region Private Methods

        //Converts Top Items Model List into Top Items View View Model List with paging data
        private DashboardListViewModel ConvertListModelToListViewModel(DashboardTopItemsListModel dashboardTopItemsListModel)
        {
            DashboardListViewModel listViewModel = new DashboardListViewModel { DashboardViewModelList = dashboardTopItemsListModel?.TopItemsList?.ToViewModel<DashboardTopItemsViewModel>().ToList() };
            SetListPagingData(listViewModel, dashboardTopItemsListModel);

            return dashboardTopItemsListModel?.TopItemsList?.Count > 0 ? listViewModel : new DashboardListViewModel { DashboardViewModelList = new List<DashboardTopItemsViewModel>() };
        }

        //Binds portal id to the filter
        private FilterCollection BindPortalFilter(ref int portalId)
        {
            //Create Portal Filter
            return new FilterCollection() { new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, portalId.ToString()) };
        }

        //Get Store List
        public List<SelectListItem> GetStoreList()
        {
                ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
                PortalListModel portalList = _portalClient.GetPortalList(null, null, null, null, null);
            List<SelectListItem> selectedPortalList = new List<SelectListItem>();
            selectedPortalList.Add(new SelectListItem() { Text = ZnodeConstant.AllPortal, Value = "0" });

            if (portalList?.PortalList?.Count > 0)
                    portalList.PortalList.OrderBy(x => x.StoreName).ToList().ForEach(item => { selectedPortalList.Add(new SelectListItem() { Text = item.StoreName, Value = item.PortalId.ToString() }); });
                ZnodeLogging.LogMessage("selectedPortalList list count:", string.Empty, TraceLevel.Verbose, selectedPortalList?.Count());
                ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
            return selectedPortalList;
        }

        //Get Store List
        public List<SelectListItem> GetAccountList()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
            AccountListModel accountList = _accountClient.GetAccountList(null, null, null, null, null);
            List<SelectListItem> selectedAccountList = new List<SelectListItem>();

            selectedAccountList.Add(new SelectListItem() { Text = ZnodeConstant.AllAccount, Value = "0" });
            if (accountList?.Accounts?.Count > 0)
                accountList.Accounts.OrderBy(x => x.Name).ToList().ForEach(item => { selectedAccountList.Add(new SelectListItem() { Text = item.Name + " | " + item.AccountCode, Value = item.AccountId.ToString() }); });
            ZnodeLogging.LogMessage("selectedAccountList list count:", string.Empty, TraceLevel.Verbose, selectedAccountList?.Count());
            ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
            return selectedAccountList;
        }

        //Gets the list of Quotes
        public virtual async Task<DashboardItemsListViewModel> GetDashboardQuotes(int portalId = 0, int accountId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
            //Binds the portalId filter to get quotes list 
            FilterCollection filters = BindPortalFilter(ref portalId);

            filters.Add(new FilterTuple(FilterKeys.AccountId, FilterOperators.Equals, accountId.ToString()));

            DashboardItemsListModel dashboardTopItemsListModel = await _dashboardClient.GetDashboardQuotes(filters, null, null, null);
            ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
            //Converts list model to list view model.
            return ConvertListModelToListViewModel(dashboardTopItemsListModel);
        }

        //Gets the list of Orders
        public virtual async Task<DashboardItemsListViewModel> GetDashboardOrders(int portalId = 0, int accountId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
            //Binds the portalId filter to get orders list 
            FilterCollection filters = BindPortalFilter(ref portalId);

            filters.Add(new FilterTuple(FilterKeys.AccountId, FilterOperators.Equals, accountId.ToString()));

            DashboardItemsListModel dashboardTopItemsListModel = await _dashboardClient.GetDashboardOrders(filters, null, null, null);
            ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
            //Converts list model to list view model.
            return ConvertListModelToListViewModel(dashboardTopItemsListModel);
        }

        //Gets the list of Returns
        public virtual async Task<DashboardItemsListViewModel> GetDashboardReturns(int portalId = 0, int accountId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
            //Binds the portalId filter to get returns list 
            FilterCollection filters = BindPortalFilter(ref portalId);

            filters.Add(new FilterTuple(FilterKeys.AccountId, FilterOperators.Equals, accountId.ToString()));

            DashboardItemsListModel dashboardTopItemsListModel = await _dashboardClient.GetDashboardReturns(filters, null, null, null);
            ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
            //Converts list model to list view model.
            return ConvertListModelToListViewModel(dashboardTopItemsListModel);
        }
       
        //Gets the list of top accounts
        public virtual async Task<DashboardItemsListViewModel> GetDashboardTopAccounts(int portalId = 0, int accountId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
            //Binds the portalId filter to get top accounts list 
            FilterCollection filters = BindPortalFilter(ref portalId);

            filters.Add(new FilterTuple(FilterKeys.AccountId, FilterOperators.Equals, accountId.ToString()));

            DashboardItemsListModel dashboardTopItemsListModel = await _dashboardClient.GetDashboardTopAccounts(filters, null, null, null);
            ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
            //Converts list model to list view model.
            return ConvertListModelToListViewModel(dashboardTopItemsListModel);
        }

        //Gets the list of dashboard total sales, total orders, total new customers, total avg orders
        public virtual async Task<DashboardItemsListViewModel> GetDashboardDetails(int portalId = 0, int accountId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
            //Binds the portalId filter to get dashboard sales details

            FilterCollection filters = BindPortalFilter(ref portalId);
            filters.Add(new FilterTuple(FilterKeys.AccountId, FilterOperators.Equals, accountId.ToString()));
            DashboardItemsListModel dashboardTopItemsListModel = await _dashboardClient.GetDashboardDetails(filters, null, null, null);
            ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
            //Converts list model to list view model.
            return ConvertListModelToListViewModel(dashboardTopItemsListModel);
        }

        //Converts Top Items Model List into Top Items View View Model List with paging data
        public DashboardItemsListViewModel ConvertListModelToListViewModel(DashboardItemsListModel dashboardTopItemsListModel)
        {
            DashboardItemsListViewModel listViewModel = new DashboardItemsListViewModel { DashboardViewModelList = dashboardTopItemsListModel?.TopItemsList?.ToViewModel<DashboardItemsViewModel>().ToList() };
            SetListPagingData(listViewModel, dashboardTopItemsListModel);
            return dashboardTopItemsListModel?.TopItemsList?.Count > 0 ? listViewModel : new DashboardItemsListViewModel { DashboardViewModelList = new List<DashboardItemsViewModel>() };
        }

        //Gets the sales details
        public virtual async Task<DashboardItemsListViewModel> GetDashboardSaleDetails(int portalId = 0, int accountId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
            //Binds the portalId filter to get returns list 
            FilterCollection filters = BindPortalFilter(ref portalId);

            filters.Add(new FilterTuple(FilterKeys.AccountId, FilterOperators.Equals, accountId.ToString()));

            DashboardItemsListModel dashboardTopItemsListModel = await _dashboardClient.GetDashboardSaleDetails(filters, null, null, null);
            ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
            //Converts list model to list view model.
            return ConvertListModelToListViewModel(dashboardTopItemsListModel);
        }

        //Get store and account List
        public void GetAccountAndStoreList(DashboardViewModel dashboard)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
            DashboardDropDownListModel accountAndStoreList = _dashboardClient.GetAccountAndStoreList(null, null, null, null);
            dashboard.Portal = new List<SelectListItem>();
            dashboard.Account = new List<SelectListItem>();

            dashboard.Portal.Add(new SelectListItem() { Text = ZnodeConstant.AllPortal, Value = "0" });
            dashboard.Account.Add(new SelectListItem() { Text = ZnodeConstant.AllAccount, Value = "0" });

            List<DashboardDropDownModel> portalList = accountAndStoreList.DashboardDropDownList.Where(x => x.EntityType == "Store").ToList();

            if (portalList?.Count > 0)
                portalList.OrderBy(x => x.EntityName).ToList().ForEach(item => { dashboard.Portal.Add(new SelectListItem() { Text = item.EntityName, Value = item.Id.ToString() }); });

            List<DashboardDropDownModel> accountList = accountAndStoreList.DashboardDropDownList.Where(x => x.EntityType == "Account").ToList();

            if (accountList?.Count > 0)
                accountList.OrderBy(x => x.EntityName).ToList().ForEach(item => { dashboard.Account.Add(new SelectListItem() { Text = item.EntityName, Value = item.Id.ToString() }); });

            ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
        }

        #endregion
    }
}