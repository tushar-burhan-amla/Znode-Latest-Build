using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.UI.WebControls;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Libraries.DevExpress.Report;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Admin.Agents
{
    public class DevExpressReportAgent : BaseAgent, IDevExpressReportAgent
    {
        #region Protected Variables
        protected readonly IDevExpressReportClient _devExpressReportClient;
        protected readonly IPortalClient _portalClient;
        protected readonly IWarehouseClient _warehouseClient;
        protected readonly IReportHelper _reportHelper;
        #endregion Private Variables

        #region Constructor
        public DevExpressReportAgent(IDevExpressReportClient devExpressReportClient, IPortalClient portalClient, IWarehouseClient warehouseClient, IReportHelper deportHelper)
        {
            _devExpressReportClient = devExpressReportClient;
            _portalClient = GetClient<IPortalClient>(portalClient);
            _warehouseClient = GetClient<IWarehouseClient>(warehouseClient);
            _reportHelper = deportHelper;
        }
        #endregion Constructor

        #region Public Methods
        // Get report setting by report code.
        public virtual ReportSettingModel GetReportSetting(string reportCode)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            var reportSetting = _devExpressReportClient.GetReportSetting(reportCode);

            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            return new ReportSettingModel() { OperatorXML = reportSetting?.ReportCode, DefaultLayoutXML = reportSetting.DefaultLayoutXML, ReportSettingXML = reportSetting?.SettingXML, DisplayMode = Convert.ToBoolean(reportSetting?.DisplayMode), StyleSheetXml = reportSetting?.StyleSheetXml, PortalList = GetPortalList(), WareHouseList=GetWarehouseList(), OrderStatusList = GetOrderStatusList(), DiscountTypeList= GetDiscountTypeList(), DefaultPriceRoundOff = DefaultSettingHelper.DefaultPriceRoundOff };
        }

        // Get Discount type list
        public virtual List<Api.Models.DevExpressReportParameterModel> GetDiscountTypeList()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            List<Api.Models.ReportDiscountTypeModel> orderStatuses = _devExpressReportClient.GetDiscountType(null);
            ZnodeLogging.LogMessage("orderStatuses list count:", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, orderStatuses?.Count());
            List<Api.Models.DevExpressReportParameterModel> discountTypeSelectList = new List<Api.Models.DevExpressReportParameterModel>();

            if (orderStatuses?.Count > 0)
                orderStatuses.ForEach(item => { discountTypeSelectList.Add(new Api.Models.DevExpressReportParameterModel() { Value = item.Name, Id = item.OmsDiscountTypeId }); });

            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            return discountTypeSelectList.Count > 0 ? discountTypeSelectList : new List<Api.Models.DevExpressReportParameterModel>();
        }

        // Get report setting by report code.
        public Api.Models.ReportViewModel GetReportSaveLayout(string reportCode, string reportName)
        {
            return _devExpressReportClient.GetReportSaveLayout(reportCode, reportName);
        }

        // Get report data source by report code.
        public virtual dynamic GetReportDataSource(string reportCode, dynamic reportFilterModel) => GetReportDataFromAPI(reportCode, reportFilterModel);


        //get all Portal(Store) names for import.
        public virtual List<Api.Models.DevExpressReportParameterModel> GetPortalList()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            FilterCollection filterCollection = new FilterCollection();
            filterCollection.Add("UserId", ReportConstants.ProcedureContainOperator,Convert.ToString(SessionProxyHelper.GetUserDetails()?.UserId));
            ZnodeLogging.LogMessage("Input parameters filters:", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, new { expandFilterCollection = filterCollection });
            List<Api.Models.ReportStoresDetailsModel> portalList = _devExpressReportClient.GetStoresWithCurrency(filterCollection);
            ZnodeLogging.LogMessage("portalList list count:", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, portalList?.Count());
            List<Api.Models.DevExpressReportParameterModel> portalSelectList = new List<Api.Models.DevExpressReportParameterModel>();

            if (portalList?.Count > 0)
                portalList.ForEach(item => { portalSelectList.Add(new Api.Models.DevExpressReportParameterModel() { Value = item.StoreName, Id = item.PortalId,DisplayMember = item.StoreNameWithCurrencySymbol }); });

            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            return portalSelectList.Count > 0 ? portalSelectList : new List<Api.Models.DevExpressReportParameterModel>();
        }

        //Get List of warehouse.
        public virtual List<Api.Models.DevExpressReportParameterModel> GetWarehouseList()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            Api.Models.WarehouseListModel warehouses = _warehouseClient.GetWarehouseList(null, null, null, null, null);

            List<Api.Models.DevExpressReportParameterModel> warehousesSelectList = new List<Api.Models.DevExpressReportParameterModel>();

            if (warehouses?.WarehouseList?.Count > 0)
                warehouses?.WarehouseList.ToList().ForEach(item => { warehousesSelectList.Add(new Api.Models.DevExpressReportParameterModel() { Value = item.WarehouseName, Id = item.WarehouseId }); });

            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            return warehousesSelectList.Count > 0 ? warehousesSelectList : new List<Api.Models.DevExpressReportParameterModel>();
        }

        //Get List of order status.
        public virtual List<Api.Models.DevExpressReportParameterModel> GetOrderStatusList()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            List<Api.Models.ReportOrderStatusModel> orderStatuses = _devExpressReportClient.GetOrderStatus(null);
            ZnodeLogging.LogMessage("orderStatuses list count:", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, orderStatuses?.Count());
            List<Api.Models.DevExpressReportParameterModel> orderStatusesSelectList = new List<Api.Models.DevExpressReportParameterModel>();

            if (orderStatuses?.Count > 0)
                orderStatuses.ForEach(item => { orderStatusesSelectList.Add(new Api.Models.DevExpressReportParameterModel() { Value = item.OrderStateName, Id = item.OmsOrderStateId }); });

            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            return orderStatusesSelectList.Count > 0 ? orderStatusesSelectList : new List<Api.Models.DevExpressReportParameterModel>();
        }

        //Generate report by report code.
        public virtual ReportModel GenerateReport(string reportCode, string reportName)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            //Creating a delegate and assigning GetReportSetting method into in it for later execution.  
            Func<ReportSettingModel> reportSetting = () => GetReportSetting(reportCode);

            //Creating a delegate and assigning GetReportDataSource method into in it for later execution. 
            Func<dynamic, dynamic> dataSourcefun = (x) => GetReportDataSource(reportCode, x);

            //Creating a delegate and assigning GetReportSaveLayout method into in it for later execution.  
            Func<string, Api.Models.ReportViewModel> reportViewModel = (x) => GetReportSaveLayout(reportCode, x);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            return _reportHelper.GenerateReport(reportCode, reportName, System.Web.HttpContext.Current, reportSetting, dataSourcefun, reportViewModel);
        }

        //Get report categories by reportCategoryId
        public virtual ReportCategoryListViewModel GetReportCategories(int? reportCategoryId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            //all active report categories 
            var reportCategoriesList = _devExpressReportClient.GetReportCategories()?.ReportCategoryList.Select(x => DevExpressReportViewModelMap.ToViewModel(x)).ToList();

            //If report category id is null than it will display first category reports default.
            reportCategoryId = HelperUtility.IsNull(reportCategoryId) ? reportCategoriesList.FirstOrDefault().ReportCategoryId : reportCategoryId;
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            return new ReportCategoryListViewModel
            {
                ReportCategoryList = reportCategoriesList,
                ReportDetailList = _devExpressReportClient.GetReportDetails(reportCategoryId.Value)?.ReportDetailList?.Select(x => DevExpressReportViewModelMap.ToViewModel(x)).ToList()
            };
        }

        public virtual Api.Models.ReportViewModel SaveReportIntoDatabase(Api.Models.ReportViewModel reportModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            reportModel.LayoutXml = _reportHelper.SaveReportIntoDatabase();
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            return _devExpressReportClient.SaveReportIntoDatabase(reportModel);

        }

        public virtual bool DeleteSavedReportLayout(int reportViewId)
        {
            return _devExpressReportClient.DeleteSavedReportLayout(reportViewId);
        }

        public virtual List<Api.Models.ReportViewModel> LoadSavedReportLayout(Api.Models.ReportViewModel reportModel)
        {
            return _devExpressReportClient.LoadSavedReportLayout(reportModel);

        }
        #endregion Public Methods

        #region Protected Methods
        //Get report data from API by report code.
        protected virtual dynamic GetReportDataFromAPI(string reportCode, dynamic reportFilterModel)
        {
            var filterCollection = AssignValuesToFilters(reportFilterModel);
            switch (reportCode.ToLower())
            {
                //Report type "Sales"
                case ReportConstants.Orders:
                    return (dynamic)_devExpressReportClient.GetOrdersReport(filterCollection);
                case ReportConstants.Coupons:
                    return (dynamic)_devExpressReportClient.GetCouponReport(filterCollection);
                case ReportConstants.SalesTax:
                    return (dynamic)_devExpressReportClient.GetSalesTaxReport(filterCollection);
                case ReportConstants.AffiliateOrders:
                    return (dynamic)_devExpressReportClient.GetAffiliateOrderReport(filterCollection);
                case ReportConstants.OrderPickList:
                    return (dynamic)_devExpressReportClient.GetOrderPickListReport(filterCollection);

                //Report type "Product"
                case ReportConstants.BestSellerProduct:
                    return (dynamic)_devExpressReportClient.GetBestSellerProductReport(filterCollection);
                case ReportConstants.InventoryReorder:
                    return (dynamic)_devExpressReportClient.GetInventoryReorderReport(filterCollection);
                case ReportConstants.TopEarningProduct:
                    return (dynamic)_devExpressReportClient.GetBestSellerProductReport(filterCollection);
                case ReportConstants.PopularSearch:
                    return (dynamic)_devExpressReportClient.GetPopularSearchReport(filterCollection);

                //Report type "Review"
                case ReportConstants.ServiceRequest:
                    return (dynamic)_devExpressReportClient.GetServiceRequestReport(filterCollection);

                //Report type "Customer"
                case ReportConstants.Users:
                    return (dynamic)_devExpressReportClient.GetUsersReport(filterCollection);
                case ReportConstants.EmailOptInCustomer:
                    return (dynamic)_devExpressReportClient.GetEmailOptInCustomerReport(filterCollection);
                case ReportConstants.MostFrequentCustomer:
                    return (dynamic)_devExpressReportClient.GetMostFrequentCustomerReport(filterCollection);
                case ReportConstants.TopSpendingCustomers:
                    return (dynamic)_devExpressReportClient.GetTopSpendingCustomersReport(filterCollection);

                //Report type "General"
                case ReportConstants.Vendors:
                    return (dynamic)_devExpressReportClient.GetVendorsReport(filterCollection);
                //Report type "General"
                case ReportConstants.AbandonedCart:
                    return (dynamic)_devExpressReportClient.GetAbandonedCartReport(filterCollection);

                default:
                    return (dynamic)_devExpressReportClient.GetOrdersReport(filterCollection);
            }

        }

        //Get data source for sub report.
        protected virtual dynamic GetSubReportDataFromAPI(string reportCode, dynamic parameters)
        {
            switch (reportCode.ToLower())
            {
                //Report type "Orders"
                case ReportConstants.Orders:
                    var omsOrderId = Convert.ToInt32(parameters[0].Value);
                    return (dynamic)_devExpressReportClient.GetOrdersItemsReport(omsOrderId);
                default:
                    return null;
            }

        }

        //Creating model from parameter values.
        protected virtual FilterCollection AssignValuesToFilters(dynamic parameters)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            FilterCollection filterCollection = new FilterCollection();
            foreach (var parameter in parameters)
            {
                string paraName = parameter.Name.ToString();
                string paraValue = Convert.ToString((parameter.Type.Name != ReportConstants.DateTime ? ((parameter.ValueInfo.Contains("|") || parameter.Value.GetType().IsArray == true) ? parameter.ValueInfo : parameter.Value) : parameter.ValueInfo));

                string paraOperator = paraValue.Contains('|') ? ReportConstants.ProcedureInOperator : ReportConstants.ProcedureContainOperator;
                filterCollection.Add(parameter.Name, paraOperator, paraValue);
            }
            ZnodeLogging.LogMessage("filterCollection:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { filterCollection = filterCollection });
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            return filterCollection;
        }
        #endregion Protected Methods

    }
}
