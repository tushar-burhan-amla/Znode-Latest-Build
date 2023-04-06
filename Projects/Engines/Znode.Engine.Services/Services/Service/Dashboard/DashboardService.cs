using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Services
{
    public class DashboardService : BaseService, IDashboardService
    {
        #region Private Variables
        private readonly IZnodeViewRepository<DashboardItemsModel> objDashStoredProc;
        private readonly IZnodeViewRepository<DashboardTopItemsModel> objStoredProc;
        #endregion

        #region Constructor
        public DashboardService()
        {
            objDashStoredProc = new ZnodeViewRepository<DashboardItemsModel>();
            objStoredProc = new ZnodeViewRepository<DashboardTopItemsModel>();
        }

        #endregion

        #region Public Methods
        //Gets total sales, total orders, total new customers and avg orders
        public virtual DashboardTopItemsListModel GetDashboardSalesDetails(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            int portalId = GetPortalIdFromFilters(filters);
            ZnodeLogging.LogMessage("portalId:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info, portalId);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate dashboardTopItemsList list ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            List<DashboardTopItemsModel> dashboardTopItemsList = new List<DashboardTopItemsModel>();

            GetDashBoardSalesList(portalId, ref dashboardTopItemsList);
            GetDashboardBrandList(portalId, ref dashboardTopItemsList);
            GetDashboardProductList(portalId, ref dashboardTopItemsList);
            GetDashboardSearchList(portalId, ref dashboardTopItemsList);

            DashboardTopItemsListModel dashboardTopItemsListModel = new DashboardTopItemsListModel { TopItemsList = dashboardTopItemsList?.ToList() };

            dashboardTopItemsListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return dashboardTopItemsListModel;
        }
        
        //Gets the list of top brands used.
        public virtual DashboardTopItemsListModel GetDashboardTopBrands(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            int portalId = GetPortalIdFromFilters(filters);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate dashboardTopBrandsList list ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            List<DashboardTopItemsModel> dashboardTopBrandsList = new List<DashboardTopItemsModel>();

            GetDashboardBrandList(portalId, ref dashboardTopBrandsList);

            DashboardTopItemsListModel dashboardTopItemsListModel = new DashboardTopItemsListModel { TopItemsList = dashboardTopBrandsList?.ToList() };

            dashboardTopItemsListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return dashboardTopItemsListModel;
        }

        //Gets the list of top products used.
        public virtual DashboardTopItemsListModel GetDashboardTopProducts(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            int portalId = GetPortalIdFromFilters(filters);

            //Bind the filters, sorts and paging details
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate dashboardTopProductsList list ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            List<DashboardTopItemsModel> dashboardTopProductsList = new List<DashboardTopItemsModel>();
            GetDashboardProductList(portalId, ref dashboardTopProductsList);

            DashboardTopItemsListModel dashboardTopItemsListModel = new DashboardTopItemsListModel { TopItemsList = dashboardTopProductsList?.ToList() };
            dashboardTopItemsListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return dashboardTopItemsListModel;
        }

        //Gets the list of top items searched with their number
        public virtual DashboardTopItemsListModel GetDashboardTopSearches(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            int portalId = GetPortalIdFromFilters(filters);
            //Bind the filters, sorts and paging details
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate dashboardTopSearchesList list ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            List<DashboardTopItemsModel> dashboardTopSearchesList = new List<DashboardTopItemsModel>();

            GetDashboardSearchList(portalId, ref dashboardTopSearchesList);

            DashboardTopItemsListModel dashboardTopItemsListModel = new DashboardTopItemsListModel { TopItemsList = dashboardTopSearchesList?.ToList() };

            dashboardTopItemsListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return dashboardTopItemsListModel;
        }

        //Gets the count of low inventory product
        public virtual DashboardTopItemsListModel GetDashboardLowInventoryProductCount(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            int portalId = GetPortalIdFromFilters(filters);
            //fetches the count of low inventory product

            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.String);

            IList<DashboardTopItemsModel> dashboardTopItemsList = objStoredProc.ExecuteStoredProcedureList("ZnodeReport_DashboardLowInventoryProductCount @PortalId");
            ZnodeLogging.LogMessage("dashboardTopItemsList list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, dashboardTopItemsList?.Count());
            DashboardTopItemsListModel dashboardTopItemsListModel = new DashboardTopItemsListModel { TopItemsList = dashboardTopItemsList?.ToList() };
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return dashboardTopItemsListModel;
        }

        //Gets the list of top categories used.
        public virtual DashboardTopItemsListModel GetDashboardTopCategories(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            //Bind the filters, sorts and paging details
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate dashboardTopItemsList list ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            //fetches the top categories list from database
            IList<DashboardTopItemsModel> dashboardTopItemsList = objStoredProc.ExecuteStoredProcedureList("ZnodeReport_DashboardTopCategory");
            ZnodeLogging.LogMessage("dashboardTopItemsList list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, dashboardTopItemsList?.Count());
            DashboardTopItemsListModel dashboardTopItemsListModel = new DashboardTopItemsListModel { TopItemsList = dashboardTopItemsList?.ToList() };

            dashboardTopItemsListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return dashboardTopItemsListModel;
        }

        //Gets the list of Quotes.
        public virtual DashboardItemsListModel GetDashboardQuotes(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            int portalId = GetPortalIdFromFilters(filters);
            int accountId = 0;
            if (HelperUtility.IsNotNull(filters))
                accountId = Convert.ToInt32(filters.Where(filterTuple => filterTuple.Item1 == ZnodeConstant.AccountId.ToString().ToLower())?.FirstOrDefault()?.Item3);
            //Bind the filters, sorts and paging details
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate dashboardTopItemsList list ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            //fetches the top categories list from database

            List<DashboardItemsModel> dashboardQuotesList = new List<DashboardItemsModel>();
            GetDashboardQuotesList(portalId, accountId, ref dashboardQuotesList);

            DashboardItemsListModel dashboardTopItemsListModel = new DashboardItemsListModel { TopItemsList = dashboardQuotesList?.ToList() };

            dashboardTopItemsListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return dashboardTopItemsListModel;
        }
        //Gets the list of top categories used.
        public virtual DashboardItemsListModel GetDashboardOrders(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            int portalId = GetPortalIdFromFilters(filters);
            int accountId = 0;
            if (HelperUtility.IsNotNull(filters))
                accountId = Convert.ToInt32(filters.Where(filterTuple => filterTuple.Item1 == ZnodeConstant.AccountId.ToString().ToLower())?.FirstOrDefault()?.Item3);
            //Bind the filters, sorts and paging details
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate dashboardTopItemsList list ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            //fetches the top categories list from database

            List<DashboardItemsModel> dashboardQuotesList = new List<DashboardItemsModel>();
            GetDashboardOrdersList(portalId, accountId, ref dashboardQuotesList);

            DashboardItemsListModel dashboardTopItemsListModel = new DashboardItemsListModel { TopItemsList = dashboardQuotesList?.ToList() };

            dashboardTopItemsListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return dashboardTopItemsListModel;
        }

        //Gets the list of top categories used.
        public virtual DashboardItemsListModel GetDashboardReturns(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            int portalId = GetPortalIdFromFilters(filters);
            int accountId = 0;
            if (HelperUtility.IsNotNull(filters))
                accountId = Convert.ToInt32(filters.Where(filterTuple => filterTuple.Item1 == ZnodeConstant.AccountId.ToString().ToLower())?.FirstOrDefault()?.Item3);
            //Bind the filters, sorts and paging details
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate dashboardTopItemsList list ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            //fetches the top categories list from database

            List<DashboardItemsModel> dashboardQuotesList = new List<DashboardItemsModel>();
            GetDashboardReturnsList(portalId, accountId, ref dashboardQuotesList);

            DashboardItemsListModel dashboardTopItemsListModel = new DashboardItemsListModel { TopItemsList = dashboardQuotesList?.ToList() };

            dashboardTopItemsListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return dashboardTopItemsListModel;
        }

        //Gets the list of top categories used.
        public virtual DashboardItemsListModel GetDashboardTopAccounts(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            int portalId = GetPortalIdFromFilters(filters);
            int accountId = 0;
            if (HelperUtility.IsNotNull(filters))
                accountId = Convert.ToInt32(filters.Where(filterTuple => filterTuple.Item1 == ZnodeConstant.AccountId.ToString().ToLower())?.FirstOrDefault()?.Item3);
            //Bind the filters, sorts and paging details
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate dashboardTopItemsList list ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            //fetches the top categories list from database

            List<DashboardItemsModel> dashboardQuotesList = new List<DashboardItemsModel>();
            GetDashboardTopAccountsList(portalId, accountId, ref dashboardQuotesList);

            DashboardItemsListModel dashboardTopItemsListModel = new DashboardItemsListModel { TopItemsList = dashboardQuotesList?.ToList() };

            dashboardTopItemsListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return dashboardTopItemsListModel;
        }
        
        //Gets total sales, total orders, total new customers and avg orders
        public virtual DashboardItemsListModel GetDashboardDetails(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            int portalId = GetPortalIdFromFilters(filters);
            int accountId = 0;
            if (HelperUtility.IsNotNull(filters))
                accountId = Convert.ToInt32(filters.Where(filterTuple => filterTuple.Item1 == ZnodeConstant.AccountId.ToString().ToLower())?.FirstOrDefault()?.Item3);
            ZnodeLogging.LogMessage("portalId:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info, portalId);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate dashboardTopItemsList list ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            List<DashboardItemsModel> dashboardTopItemsList = new List<DashboardItemsModel>();

            GetDashboardSalesInfo(portalId, accountId, ref dashboardTopItemsList);
            GetDashboardQuotesList(portalId, accountId, ref dashboardTopItemsList);
            GetDashboardOrdersList(portalId, accountId, ref dashboardTopItemsList);
            GetDashboardReturnsList(portalId, accountId, ref dashboardTopItemsList);
            GetDashboardTopAccountsList(portalId, accountId, ref dashboardTopItemsList);

            DashboardItemsListModel dashboardTopItemsListModel = new DashboardItemsListModel { TopItemsList = dashboardTopItemsList?.ToList() };

            dashboardTopItemsListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return dashboardTopItemsListModel;
        }

        public virtual DashboardTopItemsListModel GetDashboardSalesCountDetails(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            int portalId = GetPortalIdFromFilters(filters);
            ZnodeLogging.LogMessage("portalId:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info, portalId);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate dashboardTopItemsList list ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            List<DashboardTopItemsModel> dashboardTopItemsList = new List<DashboardTopItemsModel>();

            GetDashBoardSalesList(portalId, ref dashboardTopItemsList);

            DashboardTopItemsListModel dashboardTopItemsListModel = new DashboardTopItemsListModel { TopItemsList = dashboardTopItemsList?.ToList() };

            dashboardTopItemsListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return dashboardTopItemsListModel;
        }

        //Gets the sales details.
        public virtual DashboardItemsListModel GetDashboardSalesInfo(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            int portalId = GetPortalIdFromFilters(filters);
            int accountId = 0;
            if (HelperUtility.IsNotNull(filters))
                accountId = Convert.ToInt32(filters.Where(filterTuple => filterTuple.Item1 == ZnodeConstant.AccountId.ToString().ToLower())?.FirstOrDefault()?.Item3);

            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter portalId:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, portalId);
            int salesRepId = Libraries.Data.Helpers.HelperMethods.GetSalesRepUserId();
            objDashStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.String);
            objDashStoredProc.SetParameter("@AccountId", accountId, ParameterDirection.Input, DbType.String);
            objDashStoredProc.SetParameter("@SalesRepUserId", salesRepId, ParameterDirection.Input, DbType.Int32);

            //fetches the top searched items list from database
            IList<DashboardItemsModel> dashboardTopItemsModel = objDashStoredProc.ExecuteStoredProcedureList("ZnodeReport_DashboardSales @PortalId, @AccountId, @SalesRepUserId");
            DashboardItemsListModel dashboardTopItemsListModel = new DashboardItemsListModel { TopItemsList = dashboardTopItemsModel?.ToList() };

            ZnodeLogging.LogMessage("DashboardTopItemsModel list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, dashboardTopItemsModel?.Count());
            return dashboardTopItemsListModel;
        }

        //Gets the account and store list.
        public virtual DashboardDropDownListModel GetAccountAndStoreList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("Where condition in GetPortalList method:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IZnodeViewRepository<DashboardDropDownModel> objStoredProc = new ZnodeViewRepository<DashboardDropDownModel>();
            List<DashboardDropDownModel> dashboardDropDownList = objStoredProc.ExecuteStoredProcedureList("ZnodeReport_DashboardDropdownAccountAndStore")?.ToList();
            //fetches the top searched items list from database
            DashboardDropDownListModel dashboardDropDownListModel = new DashboardDropDownListModel { DashboardDropDownList = dashboardDropDownList?.ToList() };

            return dashboardDropDownListModel;
        }
        #endregion

        #region Private Methods
        //Get Dashboard Search List
        private void GetDashboardSearchList(int portalId, ref List<DashboardTopItemsModel> dashboardTopItemsList)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter portalId:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, portalId);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.String);

            //fetches the top searched items list from database
            IList<DashboardTopItemsModel> list = objStoredProc.ExecuteStoredProcedureList("ZnodeReport_DashboardTopSearches @PortalId");
            ZnodeLogging.LogMessage("DashboardTopItemsModel list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, list?.Count());
            list.Select(x => { x.Type = ZnodeConstant.Search; return x; }).ToList();
            dashboardTopItemsList.AddRange(list);
        }

        //Get Dashboard Product List
        private void GetDashboardProductList(int portalId, ref List<DashboardTopItemsModel> dashboardTopItemsList)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter portalId:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, portalId);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.String);

            //fetches the top products list from database
            IList<DashboardTopItemsModel> list = objStoredProc.ExecuteStoredProcedureList("ZnodeReport_DashboardTopProducts @PortalId");
            ZnodeLogging.LogMessage("DashboardTopItemsModel list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, list?.Count());
            list.Select(x => { x.Type = ZnodeConstant.Product; return x; }).ToList();
            dashboardTopItemsList.AddRange(list);
        }

        //Get Dashboard Brand List
        private void GetDashboardBrandList(int portalId, ref List<DashboardTopItemsModel> dashboardTopItemsList)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter portalId:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, portalId);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.String);

            //fetches the top brands list from database
            IList<DashboardTopItemsModel> list = objStoredProc.ExecuteStoredProcedureList("ZnodeReport_DashboardTopBrands @PortalId");
            ZnodeLogging.LogMessage("DashboardTopItemsModel list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, list?.Count());
            list.Select(x => { x.Type = ZnodeConstant.Brand; return x; }).ToList();
            dashboardTopItemsList.AddRange(list);
        }

        //Get Dashboard Sales List
        private void GetDashBoardSalesList(int portalId, ref List<DashboardTopItemsModel> dashboardTopItemsList)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter portalId:", ZnodeLogging.Components.GlobalSettings.ToString(),TraceLevel.Verbose, portalId);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.String);

            //fetches the total sales, total orders, total new customers and avg orders list from database
            IList<DashboardTopItemsModel> list = objStoredProc.ExecuteStoredProcedureList("ZnodeReport_Dashboard  @PortalId");
            ZnodeLogging.LogMessage("DashboardTopItemsModel list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, list?.Count());
            list.Select(x => { x.Type = ZnodeConstant.Sales; return x; }).ToList();
            dashboardTopItemsList.AddRange(list);
        }

        //gets portalId from filters
        private static int GetPortalIdFromFilters(FilterCollection filters)
        {
            int portalId = 0;
            if (HelperUtility.IsNotNull(filters))
                portalId = Convert.ToInt32(filters.Where(filterTuple => filterTuple.Item1 == ZnodePortalEnum.PortalId.ToString().ToLower())?.FirstOrDefault()?.Item3);
            ZnodeLogging.LogMessage("portalId:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, portalId);
            return portalId;
        }

        //Get Dashboard Quotes List
        protected virtual void GetDashboardQuotesList(int portalId, int accountId, ref List<DashboardItemsModel> dashboardItemsList)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter portalId:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, portalId);
            int salesRepId = Libraries.Data.Helpers.HelperMethods.GetSalesRepUserId();
            objDashStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.String);
            objDashStoredProc.SetParameter("@AccountId", accountId, ParameterDirection.Input, DbType.String);
            objDashStoredProc.SetParameter("@SalesRepUserId", salesRepId, ParameterDirection.Input, DbType.Int32);

            //fetches the Quotes list from database
            IList<DashboardItemsModel> list = objDashStoredProc.ExecuteStoredProcedureList("ZnodeReport_DashboardQuotes @PortalId, @AccountId, @SalesRepUserId");
            ZnodeLogging.LogMessage("DashboardTopItemsModel list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, list?.Count());
            list.Select(x => { x.Type = ZnodeConstant.Quote; return x; }).ToList();
            dashboardItemsList.AddRange(list);
        }

        //Get Dashboard Orders List
        protected virtual void GetDashboardOrdersList(int portalId, int accountId, ref List<DashboardItemsModel> dashboardOrdersList)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter portalId:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, portalId);
            int salesRepId = Libraries.Data.Helpers.HelperMethods.GetSalesRepUserId();
            objDashStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.String);
            objDashStoredProc.SetParameter("@AccountId", accountId, ParameterDirection.Input, DbType.String);
            objDashStoredProc.SetParameter("@SalesRepUserId", salesRepId, ParameterDirection.Input, DbType.Int32);

            //fetches the Orders list from database
            IList<DashboardItemsModel> list = objDashStoredProc.ExecuteStoredProcedureList("ZnodeReport_DashboardOrders @PortalId, @AccountId, @SalesRepUserId");
            ZnodeLogging.LogMessage("DashboardTopItemsModel list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, list?.Count());
            list.Select(x => { x.Type = ZnodeConstant.Order; return x; }).ToList();
            dashboardOrdersList.AddRange(list);
        }

        //Get Dashboard Returns List
        protected virtual void GetDashboardReturnsList(int portalId, int accountId, ref List<DashboardItemsModel> dashboardOrdersList)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter portalId:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, portalId);
            int salesRepId = Libraries.Data.Helpers.HelperMethods.GetSalesRepUserId();
            objDashStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.String);
            objDashStoredProc.SetParameter("@AccountId", accountId, ParameterDirection.Input, DbType.String);
            objDashStoredProc.SetParameter("@SalesRepUserId", salesRepId, ParameterDirection.Input, DbType.Int32);

            //fetches the Returns list from database
            IList<DashboardItemsModel> list = objDashStoredProc.ExecuteStoredProcedureList("ZnodeReport_DashboardReturns @PortalId, @AccountId, @SalesRepUserId");
            ZnodeLogging.LogMessage("DashboardTopItemsModel list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, list?.Count());
            list.Select(x => { x.Type = ZnodeConstant.Return; return x; }).ToList();
            dashboardOrdersList.AddRange(list);
        }

        //Get Dashboard top accounts List
        protected virtual void GetDashboardTopAccountsList(int portalId, int accountId, ref List<DashboardItemsModel> dashboardOrdersList)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter portalId:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, portalId);
            int salesRepId = Libraries.Data.Helpers.HelperMethods.GetSalesRepUserId();
            objDashStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.String);
            objDashStoredProc.SetParameter("@AccountId", accountId, ParameterDirection.Input, DbType.String);
            objDashStoredProc.SetParameter("@SalesRepUserId", salesRepId, ParameterDirection.Input, DbType.Int32);

            //fetches the top accounts list from database
            IList<DashboardItemsModel> list = objDashStoredProc.ExecuteStoredProcedureList("ZnodeReport_DashboardTopAccounts @PortalId, @AccountId, @SalesRepUserId");
            ZnodeLogging.LogMessage("DashboardTopItemsModel list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, list?.Count());
            list.Select(x => { x.Type = ZnodeConstant.TopAccount; return x; }).ToList();
            dashboardOrdersList.AddRange(list);
        }

        protected virtual void GetDashboardSalesInfo(int portalId, int accountId, ref List<DashboardItemsModel> dashboardOrdersList)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter portalId:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, portalId);
            int salesRepId = Libraries.Data.Helpers.HelperMethods.GetSalesRepUserId();
            objDashStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.String);
            objDashStoredProc.SetParameter("@AccountId", accountId, ParameterDirection.Input, DbType.String);
            objDashStoredProc.SetParameter("@SalesRepUserId", salesRepId, ParameterDirection.Input, DbType.Int32);

            //fetches the top searched items list from database
            IList<DashboardItemsModel> list = objDashStoredProc.ExecuteStoredProcedureList("ZnodeReport_DashboardSales @PortalId, @AccountId, @SalesRepUserId");
            ZnodeLogging.LogMessage("DashboardTopItemsModel list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, list?.Count());
            list.Select(x => { x.Type = ZnodeConstant.Sales; return x; }).ToList();
            dashboardOrdersList.AddRange(list);
        }
        #endregion
    }
}
