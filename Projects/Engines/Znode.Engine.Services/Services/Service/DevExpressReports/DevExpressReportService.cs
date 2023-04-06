using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Services
{
    public class DevExpressReportService : BaseService, IDevExpressReportService
    {
        private const string PartialRefund = "PARTIALREFUND";
        private const string GiftCard = "GIFTCARD";
        #region Private Variables
        private readonly IZnodeRepository<ZnodeReportCategory> _reportCategory;
        private readonly IZnodeRepository<ZnodeReportDetail> _reportDetail;
        private readonly IZnodeRepository<ZnodeReportSetting> _reportSetting;
        private readonly IZnodeRepository<ZnodeReportStyleSheet> _reportStyleSheet;
        private readonly IZnodeRepository<ZnodeSavedReportView> _reportView;
        private readonly IZnodeRepository<ZnodeOmsOrderState> _omsOrderStateRepository;
        private readonly IZnodeRepository<ZnodeOmsDiscountType> _omsDiscountTypeRepository;
        #endregion Private Variables

        #region Constructor
        public DevExpressReportService()
        {
            _reportCategory = new ZnodeRepository<ZnodeReportCategory>();
            _reportDetail = new ZnodeRepository<ZnodeReportDetail>();
            _reportSetting = new ZnodeRepository<ZnodeReportSetting>();
            _reportView = new ZnodeRepository<ZnodeSavedReportView>();
            _reportStyleSheet = new ZnodeRepository<ZnodeReportStyleSheet>();
            _omsOrderStateRepository = new ZnodeRepository<ZnodeOmsOrderState>();
            _omsDiscountTypeRepository = new ZnodeRepository<ZnodeOmsDiscountType>();
        }
        #endregion Constructor

        #region Public Methods
        // Get all active report categories        
        public virtual List<ReportCategoryModel> GetReportCategories() => _reportCategory.Table.Where(x => x.IsActive)?.Select(x => new ReportCategoryModel()
        {
            CategoryName = x.CategoryName,
            ReportCategoryId = x.ReportCategoryId,
            IsActive = x.IsActive
        }).ToList();


        // Get reports details report categoryId        
        public virtual List<ReportDetailModel> GetReportDetails(int reportCategoryId) => _reportDetail.Table.Where(x => x.ReportCategoryId == reportCategoryId && x.IsActive == true)?.Select(x => new ReportDetailModel()
        {
            ReportDetailId = x.ReportDetailId,
            ReportCategoryId = x.ReportCategoryId,
            ReportCode = x.ReportCode,
            ReportName = x.ReportName,
            Description = x.Description
        }).ToList();


        // Get report setting by reportcode.        
        public virtual ReportSettingModel GetReportSetting(string ReportCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter ReportCode: ", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, ReportCode);

            var reportsetting = _reportSetting.Table.FirstOrDefault(x => x.ReportCode == ReportCode);

            if(!Equals(reportsetting, null))
            {
                var styleSheetId = HelperUtility.IsNull(reportsetting?.StyleSheetId) ? _reportStyleSheet.Table.FirstOrDefault(x => x.IsDefault == true)?.StyleSheetId : reportsetting?.StyleSheetId;
                ZnodeLogging.LogMessage("styleSheetId: ", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, styleSheetId);

                return new ReportSettingModel()
                {
                    ReportCode = reportsetting.ReportCode,
                    SettingXML = reportsetting.SettingXML,
                    DefaultLayoutXML = reportsetting.DefaultLayoutXML,
                    DisplayMode = reportsetting.DisplayMode.Value,
                    StyleSheetXml = _reportStyleSheet.Table.FirstOrDefault(x => x.StyleSheetId == styleSheetId)?.StyleSheetXml
                };
            }
            else
                return new ReportSettingModel();
        }

        //To save the report layout
        public virtual ReportViewModel SaveReportLayout(ReportViewModel reportViewModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            ZnodeSavedReportView address = _reportView.Insert(new ZnodeSavedReportView
            {
                IsActive = true,
                ModifiedBy = reportViewModel.UserId,
                ModifiedDate = DateTime.Now,
                UserId = reportViewModel.UserId,
                LayoutXml = reportViewModel.LayoutXml,
                ReportCode = reportViewModel.ReportCode,
                ReportName = reportViewModel.ReportName,
                CreatedBy = reportViewModel.UserId,
                CreatedDate = DateTime.Now,

            });
            reportViewModel.ReportViewId = address.ReportViewId;
            ZnodeLogging.LogMessage("ReportViewId, UserId and ReportCode properties of reportViewModel to be returned: ", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, new { ReportViewId = reportViewModel?.ReportViewId, UserId = reportViewModel?.UserId, ReportCode = reportViewModel?.ReportCode });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            return reportViewModel;
        }

        //Get saved report layouts from database by user id and report code.
        public virtual List<ReportViewModel> GetSavedReportLayouts(ReportViewModel reportViewModel) =>
        _reportView.Table.Where(d => d.UserId == reportViewModel.UserId && d.ReportCode == reportViewModel.ReportCode && (string.IsNullOrEmpty(reportViewModel.ReportName) || d.ReportName == reportViewModel.ReportName))?.Select(d => new ReportViewModel()
        {
            ReportName = d.ReportName,
            ReportViewId = d.ReportViewId
        }).ToList();

        //Get saved report layout from database.
        public virtual ReportViewModel GetSavedReportLayout(string reportCode, string reportName)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters reportCode and reportName: ", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, new object[] { reportCode, reportName });

            var reportView = _reportView.Table.FirstOrDefault(d => d.ReportCode == reportCode && d.ReportName == reportName);
            ZnodeLogging.LogMessage("reportView: ", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, reportView);

            return new ReportViewModel()
            {
                ReportName = reportView?.ReportName,
                LayoutXml = reportView?.LayoutXml
            };
        }

        // This method is use for deleting saved report layout by reportviewId from database.         
        public virtual bool DeleteSavedReportLayout(int reportViewId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            var reportSavedView = _reportView.Table.FirstOrDefault(d => d.ReportViewId == reportViewId);
            ZnodeLogging.LogMessage("reportView: ", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, reportSavedView);
            return HelperUtility.IsNotNull(reportSavedView) ? _reportView.Delete(reportSavedView) : false;
        }

        //Get orders list by report filters.
        public virtual List<ReportOrderDetailsModel> GetOrdersReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);

            object beginDate, endDate, storesNames, orderStatus;
            RemoveParametersGetValue(filters, FilterKeys.BeginDate, out beginDate);
            RemoveParametersGetValue(filters, FilterKeys.EndDate, out endDate);
            RemoveParametersGetValue(filters, FilterKeys.StoresName, out storesNames);
            RemoveParametersGetValue(filters, FilterKeys.OrderStatus, out orderStatus);
            ZnodeLogging.LogMessage("beginDate, endDate, storesNames, orderStatus values to set SP parameters to get stores list with currency: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { beginDate, endDate, storesNames, orderStatus });
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            string cmdString = "ZnodeDevExpressReport_GetOrderDetails @BeginDate, @EndDate, @StoreName, @OrderStatus";

            IZnodeViewRepository<ReportOrderDetailsModel> objStoredProc = new ZnodeViewRepository<ReportOrderDetailsModel>();
            objStoredProc.SetParameter("@BeginDate", beginDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter("@EndDate", endDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter("@StoreName", GetWithoutCurrencyStore(Convert.ToString(storesNames)), ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@OrderStatus", orderStatus, ParameterDirection.Input, DbType.String);
            return objStoredProc.ExecuteStoredProcedureList(cmdString).ToList();

        }

        //Get orders list by report filters.
        public virtual List<ReportCouponModel> GetCouponReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);

            object beginDate, endDate, storesNames, discountType, promotionCode;
            RemoveParametersGetValue(filters, FilterKeys.BeginDate, out beginDate);
            RemoveParametersGetValue(filters, FilterKeys.EndDate, out endDate);
            RemoveParametersGetValue(filters, FilterKeys.StoresName, out storesNames);
            RemoveParametersGetValue(filters, FilterKeys.DiscountType, out discountType);
            ZnodeLogging.LogMessage("beginDate, endDate, storesNames, discountType values to set SP parameters to get coupon list with currency: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { beginDate, endDate, storesNames, discountType });
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            string cmdString = "ZnodeDevExpressReport_GetCouponFiltered @BeginDate, @EndDate, @StoreName, @DiscountType";

            IZnodeViewRepository<ReportCouponModel> objStoredProc = new ZnodeViewRepository<ReportCouponModel>();
            objStoredProc.SetParameter("@BeginDate", beginDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter("@EndDate", endDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter("@StoreName", GetWithoutCurrencyStore(Convert.ToString(storesNames)), ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@DiscountType", discountType, ParameterDirection.Input, DbType.String);            
            return objStoredProc.ExecuteStoredProcedureList(cmdString).ToList();

        }

        //Get sale tax list by report filters.
        public virtual List<ReportSalesTaxModel> GetSalesTaxReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);

            object beginDate, endDate;
            RemoveParametersGetValue(filters, FilterKeys.BeginDate, out beginDate);
            RemoveParametersGetValue(filters, FilterKeys.EndDate, out endDate);
            ConfigureParameter(filters, FilterKeys.StoresName, FilterKeys.StoreName);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel generated to set SP parameters to get sale tax list: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            string cmdString = "ZnodeDevExpressReport_GetTaxFiltered @BeginDate, @EndDate, @WhereClause";

            IZnodeViewRepository<ReportSalesTaxModel> objStoredProc = new ZnodeViewRepository<ReportSalesTaxModel>();
            objStoredProc.SetParameter("@BeginDate", beginDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter("@EndDate", endDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            return objStoredProc.ExecuteStoredProcedureList(cmdString).ToList();

        }

        //Get Affiliate orders list by report filters.
        public virtual List<ReportAffiliateOrderModel> GetAffiliateOrderReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            object beginDate, endDate;

            RemoveParametersGetValue(filters, FilterKeys.BeginDate, out beginDate);
            RemoveParametersGetValue(filters, FilterKeys.EndDate, out endDate);
            ConfigureParameter(filters, FilterKeys.StoresName, FilterKeys.StoreName);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel generated to set SP parameters to get affiliate orders list: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            string cmdString = "ZnodeDevExpressReport_GetAffiliateFiltered  @BeginDate, @EndDate, @WhereClause";

            IZnodeViewRepository<ReportAffiliateOrderModel> objStoredProc = new ZnodeViewRepository<ReportAffiliateOrderModel>();
            objStoredProc.SetParameter("@BeginDate", beginDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter("@EndDate", endDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            return objStoredProc.ExecuteStoredProcedureList(cmdString).ToList();
        }

        //Get orders pick list by report filters.
        public virtual List<ReportOrderPickModel> GetOrderPickListReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            object beginDate, endDate;

            RemoveParametersGetValue(filters, FilterKeys.BeginDate, out beginDate);
            RemoveParametersGetValue(filters, FilterKeys.EndDate, out endDate);
            ConfigureParameter(filters, FilterKeys.StoresName, FilterKeys.StoreName);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel generated to set SP parameters to get orders pick list: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            string cmdString = "ZnodeDevExpressReport_GetOrderPicklist @BeginDate, @EndDate, @WhereClause";

            IZnodeViewRepository<ReportOrderPickModel> objStoredProc = new ZnodeViewRepository<ReportOrderPickModel>();
            objStoredProc.SetParameter("@BeginDate", beginDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter("@EndDate", endDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            return objStoredProc.ExecuteStoredProcedureList(cmdString).ToList();
        }

        //Get order list by sp.
        public virtual List<ReportOrderItemsDetailsModel> GetOrdersItemsReport(int omsOrderId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters omsOrderId: ", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, omsOrderId);
            IZnodeViewRepository<ReportOrderItemsDetailsModel> objStoredProc = new ZnodeViewRepository<ReportOrderItemsDetailsModel>();
            objStoredProc.SetParameter("@OmsOrderId", omsOrderId, ParameterDirection.Input, DbType.Int32);
            return objStoredProc.ExecuteStoredProcedureList("ZnodeReport_GetOrderLineItemByOrderId @OmsOrderId").ToList();
        }

        #region report
        //Get orders pick list by report filters.
        public virtual List<ReportBestSellerProductModel> GetBestSellerProductReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            object beginDate, endDate, ShowAllProducts, TopProducts, storesNames;
            RemoveParametersGetValue(filters, FilterKeys.BeginDate, out beginDate);
            RemoveParametersGetValue(filters, FilterKeys.EndDate, out endDate);
            RemoveParametersGetValue(filters, FilterKeys.ShowAllProducts, out ShowAllProducts);
            RemoveParametersGetValue(filters, FilterKeys.TopProducts, out TopProducts);
            RemoveParametersGetValue(filters, FilterKeys.StoresName, out storesNames);

            ZnodeLogging.LogMessage("beginDate, endDate, ShowAllProducts, TopProducts, storesNames values to set SP parameters to get stores list with currency: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { beginDate, endDate, ShowAllProducts, TopProducts, storesNames });
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            string cmdString = "ZnodeDevExpressReport_GetPopularProductFiltered @BeginDate, @EndDate, @StoreName, @ShowAllProducts, @TopProducts";

            IZnodeViewRepository<ReportBestSellerProductModel> objStoredProc = new ZnodeViewRepository<ReportBestSellerProductModel>();
            objStoredProc.SetParameter("@BeginDate", beginDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter("@EndDate", endDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter("@StoreName", storesNames, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@ShowAllProducts", ShowAllProducts, ParameterDirection.Input, DbType.Boolean);
            objStoredProc.SetParameter("@TopProducts", TopProducts, ParameterDirection.Input, DbType.Int32);
            return objStoredProc.ExecuteStoredProcedureList(cmdString).ToList();
        }

        //Get orders pick list by report filters.
        public virtual List<ReportInventoryReorderModel> GetInventoryReorderReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);

            //Remove VisibleColumns from filters collection               
            object visibleColumns, WarehouseName, showOnlyActiveProducts;
            RemoveParametersGetValue(filters, FilterKeys.VisibleColumns, out visibleColumns);
            RemoveParametersGetValue(filters, FilterKeys.WareHousesName, out WarehouseName);
            RemoveParametersGetValue(filters, FilterKeys.ShowOnlyActiveProducts, out showOnlyActiveProducts);
            ZnodeLogging.LogMessage("visibleColumns, WarehouseName, showOnlyActiveProducts values to set SP parameters to get stores list with currency: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { visibleColumns, WarehouseName, showOnlyActiveProducts });
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            string cmdString = "ZnodeDevExpressReport_LowInventoryProduct @WarehouseName, @ShowOnlyActiveProducts";

            IZnodeViewRepository<ReportInventoryReorderModel> objStoredProc = new ZnodeViewRepository<ReportInventoryReorderModel>();
            objStoredProc.SetParameter("@WarehouseName", WarehouseName, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@ShowOnlyActiveProducts", showOnlyActiveProducts, ParameterDirection.Input, DbType.Boolean);
            return objStoredProc.ExecuteStoredProcedureList(cmdString).ToList();

        }

        //Get orders pick list by report filters.
        public virtual List<ReportPopularSearchModel> GetPopularSearchReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);

            object beginDate, endDate;
            RemoveParametersGetValue(filters, FilterKeys.BeginDate, out beginDate);
            RemoveParametersGetValue(filters, FilterKeys.EndDate, out endDate);
            ConfigureParameter(filters, FilterKeys.StoresName, FilterKeys.StoreName);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel generated to set SP parameters to get orders pick list by report filters: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            string cmdString = "ZnodeDevExpressReport_KeywordFiltered @BeginDate, @EndDate, @WhereClause";

            IZnodeViewRepository<ReportPopularSearchModel> objStoredProc = new ZnodeViewRepository<ReportPopularSearchModel>();
            objStoredProc.SetParameter("@BeginDate", beginDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter("@EndDate", endDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            return objStoredProc.ExecuteStoredProcedureList(cmdString).ToList();

        }
        #endregion report
        #region Report
        // Get list of user.
        public virtual List<ReportUsersModel> GetUsersReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            object beginDate, endDate, typeParam,storesNames;
            RemoveParametersGetValue(filters, FilterKeys.BeginDate, out beginDate);
            RemoveParametersGetValue(filters, FilterKeys.EndDate, out endDate);
            RemoveParametersGetValue(filters, FilterKeys.StoresName, out storesNames);
            RemoveParametersGetValue(filters, FilterKeys.TypeParam, out typeParam);
            typeParam = typeParam.ToString().Replace("inactive", "0").Replace("active", "1").Replace("1|0", "2").Replace("0|1", "2");
            ZnodeLogging.LogMessage("beginDate, endDate, typeParam,storesNames values to set SP parameters to get stores list with currency: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { beginDate, endDate, typeParam, storesNames });
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            string cmdString = " ZnodeDevExpressReport_GetUserList @BeginDate, @EndDate, @StoreName, @CustomerStatus";

            IZnodeViewRepository<ReportUsersModel> objStoredProc = new ZnodeViewRepository<ReportUsersModel>();
            objStoredProc.SetParameter("@BeginDate", beginDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter("@EndDate", endDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter("@StoreName", storesNames, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@CustomerStatus", typeParam, ParameterDirection.Input, DbType.String);
            return objStoredProc.ExecuteStoredProcedureList(cmdString).ToList();

        }

        // Get list of email opt customer.
        public virtual List<ReportEmailOptInCustomerModel> GetEmailOptInCustomerReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            object firstName, lastName, email, storesNames, showonlyregisteredUsers;
            RemoveParametersGetValue(filters, FilterKeys.FirstName, out firstName);
            RemoveParametersGetValue(filters, FilterKeys.LastName, out lastName);
            RemoveParametersGetValue(filters, FilterKeys.Email, out email);
            RemoveParametersGetValue(filters, FilterKeys.StoresName, out storesNames);
            RemoveParametersGetValue(filters, FilterKeys.ShowOnlyRegisteredUsers, out showonlyregisteredUsers);
            filters?.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.VisibleColumns, StringComparison.CurrentCultureIgnoreCase));

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel generated to set SP parameters to get list of email opt customer: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            string cmdString = "ZnodeDevExpressReport_GetEmailOptinCustomer @StoreName, @FirstName, @LastName, @Email, @ShowOnlyRegisteredUsers";
            IZnodeViewRepository<ReportEmailOptInCustomerModel> objStoredProc = new ZnodeViewRepository<ReportEmailOptInCustomerModel>();
            objStoredProc.SetParameter("@StoreName", storesNames, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@FirstName", firstName, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@LastName", lastName, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Email", email, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@ShowOnlyRegisteredUsers", showonlyregisteredUsers, ParameterDirection.Input, DbType.Boolean);
            return objStoredProc.ExecuteStoredProcedureList(cmdString).ToList();

        }

        // Get list of most frequent customer.
        public virtual List<ReportMostFrequentCustomerModel> GetMostFrequentCustomerReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            object beginDate, endDate, IsAllCustomer, TopCustomers,storename;

            RemoveParametersGetValue(filters, FilterKeys.BeginDate, out beginDate);
            RemoveParametersGetValue(filters, FilterKeys.EndDate, out endDate);
            RemoveParametersGetValue(filters, FilterKeys.ShowAllCustomers, out IsAllCustomer);
            RemoveParametersGetValue(filters, FilterKeys.TopCustomers, out TopCustomers);
            RemoveParametersGetValue(filters, FilterKeys.StoresName, out storename);

            ZnodeLogging.LogMessage("beginDate, endDate, IsAllCustomer, TopCustomers,storename values to set SP parameters to get stores list with currency: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { beginDate, endDate, IsAllCustomer, TopCustomers, storename });
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            string cmdString = "ZnodeDevExpressReport_GetTopOrderingcustomers @BeginDate, @EndDate, @StoreName, @ShowAllCustomers, @TopCustomers";

            IZnodeViewRepository<ReportMostFrequentCustomerModel> objStoredProc = new ZnodeViewRepository<ReportMostFrequentCustomerModel>();
            objStoredProc.SetParameter("@BeginDate", beginDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter("@EndDate", endDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter("@StoreName", Convert.ToString(storename), ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@ShowAllCustomers", IsAllCustomer, ParameterDirection.Input, DbType.Boolean);
            objStoredProc.SetParameter("@TopCustomers", TopCustomers, ParameterDirection.Input, DbType.Int32);
            return objStoredProc.ExecuteStoredProcedureList(cmdString).ToList();

        }

        // Get list of top spending customer.
        public virtual List<ReportTopSpendingCustomersModel> GetTopSpendingCustomersReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);

            object beginDate, endDate, ShowAllCustomers, TopCustomers, AmountGreaterThan, storename;

            RemoveParametersGetValue(filters, FilterKeys.BeginDate, out beginDate);
            RemoveParametersGetValue(filters, FilterKeys.EndDate, out endDate);
            RemoveParametersGetValue(filters, FilterKeys.ShowAllCustomers, out ShowAllCustomers);
            RemoveParametersGetValue(filters, FilterKeys.TopCustomers, out TopCustomers);
            RemoveParametersGetValue(filters, FilterKeys.AmountGreaterThan, out AmountGreaterThan);
            RemoveParametersGetValue(filters, FilterKeys.StoresName, out storename);
            ZnodeLogging.LogMessage("beginDate, endDate, ShowAllCustomers, TopCustomers, AmountGreaterThan, storename values to set SP parameters to get stores list with currency: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { beginDate, endDate, ShowAllCustomers, TopCustomers, AmountGreaterThan, storename });

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            string cmdString = "ZnodeDevExpressReport_GetTopSpendingCustomers @BeginDate, @EndDate, @StoreName, @ShowAllCustomers, @TopCustomers, @AmountGreaterThan";

            IZnodeViewRepository<ReportTopSpendingCustomersModel> objStoredProc = new ZnodeViewRepository<ReportTopSpendingCustomersModel>();
            objStoredProc.SetParameter("@BeginDate", beginDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter("@EndDate", endDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter("@StoreName", GetWithoutCurrencyStore(Convert.ToString(storename)), ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@ShowAllCustomers", ShowAllCustomers, ParameterDirection.Input, DbType.Boolean);
            objStoredProc.SetParameter("@TopCustomers", TopCustomers, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@AmountGreaterThan", AmountGreaterThan, ParameterDirection.Input, DbType.String);
            return objStoredProc.ExecuteStoredProcedureList(cmdString).ToList();

        }
        #endregion Report

        // Get list of Abandoned Cart.
        public virtual List<ReportAbandonedCartModel> GetAbandonedCartReport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);

            object beginDate, endDate, showonlyregisteredUsers, storename;

            RemoveParametersGetValue(filters, FilterKeys.BeginDate, out beginDate);
            RemoveParametersGetValue(filters, FilterKeys.EndDate, out endDate);
            RemoveParametersGetValue(filters, FilterKeys.ShowOnlyRegisteredUsers, out showonlyregisteredUsers);
            RemoveParametersGetValue(filters, FilterKeys.StoresName, out storename);

            ZnodeLogging.LogMessage("beginDate, endDate, showonlyregisteredUsers and storename values to set SP parameters to get stores list with currency: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { beginDate, endDate, showonlyregisteredUsers, storename });
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            string cmdString = "ZnodeDevExpressReport_GetAbandonedCart @BeginDate, @EndDate, @StoreName,  @ShowOnlyRegisteredUsers";

            IZnodeViewRepository<ReportAbandonedCartModel> objStoredProc = new ZnodeViewRepository<ReportAbandonedCartModel>();
            objStoredProc.SetParameter("@BeginDate", beginDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter("@EndDate", endDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter("@StoreName", GetWithoutCurrencyStore(Convert.ToString(storename)), ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@ShowOnlyRegisteredUsers", showonlyregisteredUsers, ParameterDirection.Input, DbType.Boolean);            
            return objStoredProc.ExecuteStoredProcedureList(cmdString).ToList();

        }

        //Get stores list by filters.
        public virtual List<ReportStoresDetailsModel> GetStoresWithCurrency(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);

            object @UserId;
            RemoveParametersGetValue(filters, FilterKeys.UserId, out UserId);
            ZnodeLogging.LogMessage("@UserId value to set SP parameters to get stores list with currency: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, @UserId);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            string cmdString = "Znode_GetStoresWithCurrency @UserId";

            IZnodeViewRepository<ReportStoresDetailsModel> objStoredProc = new ZnodeViewRepository<ReportStoresDetailsModel>();            
            objStoredProc.SetParameter("@UserId", @UserId, ParameterDirection.Input, DbType.Int32);
            return objStoredProc.ExecuteStoredProcedureList(cmdString).ToList();

        }

        //Get order states.
        public virtual List<ReportOrderStatusModel> GetOrderStatus(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            return _omsOrderStateRepository.Table.Where(x => x.IsOrderState == true).Select(x=> new ReportOrderStatusModel() {
                OmsOrderStateId = x.OmsOrderStateId,
                OrderStateName = x.OrderStateName
            }).ToList();
        }

        //Get order discount type
        public virtual List<ReportDiscountTypeModel> GetDiscountType(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            return _omsDiscountTypeRepository.Table.Where(x => !x.Name.Equals(PartialRefund) && !x.Name.Equals(GiftCard)).Select(x => new ReportDiscountTypeModel()
            {
                OmsDiscountTypeId = x.OmsDiscountTypeId,
                Name = x.Name
            }).ToList();
        }

        #endregion Public Methods

        #region private methods
        private void ConfigureParameter(FilterCollection filters, string filterName, string filterNewName)
        {
            var filter = filters?.Find(x => string.Equals(x.FilterName, filterName, StringComparison.CurrentCultureIgnoreCase));
            if (HelperUtility.IsNotNull(filter) && HelperUtility.IsNotNull(filters))
            {              
                filters.Remove(filter);
                filters.Add(new FilterTuple(filterNewName, filter.FilterOperator, filter.Item3));
            }
        }

        private object GetWithoutCurrencyStore(string storeNames)
        {
            string[] stores = storeNames?.Split('|');
            if (stores?.FirstOrDefault()?.Contains(" (") ?? false)
            {
                List<string> updatedStores = new List<string>();
                foreach (string store in stores)
                {
                    if (store.LastIndexOf(" (") >= 0)
                        updatedStores.Add(store.Substring(0, store.LastIndexOf(" (")));
                    else
                        updatedStores.Add(store);
                }
                return string.Join("|", updatedStores);
            }
            return storeNames;
        }

        private void RemoveParametersGetValue(FilterCollection filters, string paramName, out object paramValue)
        {
            //Remove begin date from filters collection
            paramValue = filters?.Find(x => string.Equals(x.FilterName, paramName, StringComparison.CurrentCultureIgnoreCase))?.Item3;
            filters?.RemoveAll(x => string.Equals(x.FilterName, paramName, StringComparison.CurrentCultureIgnoreCase));
        }
        #endregion

    }
}
