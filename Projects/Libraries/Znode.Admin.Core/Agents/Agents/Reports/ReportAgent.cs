using MvcReportViewer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
namespace Znode.Engine.Admin.Agents
{
    public class ReportAgent : BaseAgent, IReportAgent
    {
        #region Private Variables
        private readonly IReportClient _reportClient;
        private readonly ILocaleAgent _localeAgent;
        private readonly IImportAgent _importAgent;
        private readonly ICatalogClient _catalogClient;
        private readonly ICategoryClient _categoryClient;
        private readonly IPriceClient _priceClient;
        private readonly IWarehouseClient _warehouseClient;
        #endregion

        #region Constructor
        public ReportAgent(IReportClient reportClient, ICatalogClient catalogClient, ICategoryClient categoryClient, IPriceClient priceClient, IWarehouseClient warehouseClient)
        {
            _reportClient = reportClient;
            _localeAgent = new LocaleAgent(GetClient<LocaleClient>());
            _importAgent = new ImportAgent(GetClient<ImportClient>(), GetClient<PriceClient>(), GetClient<CountryClient>(), GetClient<PortalClient>(), GetClient<CatalogClient>());
            _catalogClient = catalogClient;
            _categoryClient = categoryClient;
            _priceClient = priceClient;
            _warehouseClient = warehouseClient;
        }
        #endregion

        #region Public Methods    
        //Get Report list
        public virtual ReportListViewModel GetDynamicReportList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new {  Filters = filters, Sorts = sortCollection });
            ReportListModel ssrsReportList = _reportClient.List(filters, sortCollection, pageIndex, recordPerPage);
            ReportListViewModel reportList = new ReportListViewModel { Reports = ssrsReportList?.ReportList.ToViewModel<ViewModels.ReportViewModel>()?.ToList() };
            SetListPagingData(reportList, ssrsReportList);
            //Set tool menu for custom report list grid view.
            SetCustomReportToolMenu(reportList);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            return reportList?.Reports?.Count > 0 ? reportList : new ReportListViewModel();
        }

        //Set tool menu for custom report list grid view.
        private void SetCustomReportToolMenu(ReportListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('ReportPopup')", ControllerName = "MyReports", ActionName = "DeleteReport" });
            }
        }

        //Set the Default Properties for Report Control Settings.
        public virtual ViewModels.ReportViewModel SetReportControlSetting(string path, bool isDynamicReport = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            ViewModels.ReportViewModel reportSettingModel = new ViewModels.ReportViewModel();
            reportSettingModel.ControlSetting = new ControlSettings { ShowToolBar = true, EnableExternalImages = true, ShowZoomControl = true, SizeToReportContent = true, AsyncRendering = false };
            reportSettingModel.Path = isDynamicReport ? $"/{ZnodeAdminSettings.ReportServerDynamicReportFolderName}/{path}" : $"/{ZnodeAdminSettings.ZnodeReportFolderName}/{path}";
            reportSettingModel.ParameterList = new Dictionary<string, object>();
            if (!isDynamicReport)
                reportSettingModel.ParameterList.Add("LoginUserId", Convert.ToInt32(SessionProxyHelper.GetUserDetails()?.UserId));
            ZnodeLogging.LogMessage("Path:", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, new { Path = reportSettingModel.Path });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            return reportSettingModel;
        }

        //Set the Report Type based on the Report Type. 
        public virtual ViewModels.ReportViewModel SetReportType(string reportType)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            ViewModels.ReportViewModel reportSettingModel = new ViewModels.ReportViewModel();
            reportSettingModel.ReportType = SetReportTypeSetting(reportType);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            return reportSettingModel;
        }

        //Get custom report.
        public virtual DynamicReportViewModel GetCustomReport(int customReportId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            DynamicReportViewModel dynamicReportViewModel = new DynamicReportViewModel();
            DynamicReportModel customReportDataList = _reportClient.GetCustomReport(customReportId);
            dynamicReportViewModel = DynamicReportViewModelMap.ToViewModel(customReportDataList);
            dynamicReportViewModel.Locale = _localeAgent.GetLocalesList(dynamicReportViewModel.LocaleId); dynamicReportViewModel.ReportTypeList = _importAgent.GetImportTypeList(true);
            ZnodeLogging.LogMessage("Locale list count:", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, new { LocaleListCount = dynamicReportViewModel?.Locale });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            return dynamicReportViewModel;
        }

        //Get filter row.
        public virtual DynamicReportViewModel GetFilterRow(string reportType, string filterName, string OperatorName, string filterValue)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            DynamicReportModel dataList = _reportClient.GetExportData(reportType);
            DynamicReportViewModel reportViewModel = new DynamicReportViewModel();
            reportViewModel.FilterValue = filterValue;
            reportViewModel.ReportFilter = GetFilterList(filterName, dataList, reportViewModel);
            reportViewModel.ReportOperator = GetOperatorList(OperatorName, dataList, reportViewModel);
            ZnodeLogging.LogMessage("ReportFilter and ReportOperator list count:", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, new { FilterListCount = reportViewModel?.ReportFilter?.Count, OperatorListCount = reportViewModel?.ReportOperator?.Count });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            return reportViewModel;
        }

        //Generate dynamic report.
        public virtual bool GenerateDynamicReport(DynamicReportViewModel model, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            try
            {
                errorMessage = string.Empty;
                return _reportClient.GenerateDynamicReport(DynamicReportViewModelMap.ToModel(model));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        errorMessage = Admin_Resources.DynamicReportAlreadyExists;
                        return false;
                    case ErrorCodes.DynamicReportSoapException:
                        errorMessage = Admin_Resources.DynamicReportSoapException;
                        return false;
                    case ErrorCodes.DynamicReportGeneralException:
                        errorMessage = Admin_Resources.DynamicReportGeneralException;
                        return false;
                    default:
                        errorMessage = Admin_Resources.DynamicReportGeneralException;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
                errorMessage = Admin_Resources.DynamicReportGeneralException;
                return false;
            }
        }

        //Bind Dynamic report view model.
        public virtual DynamicReportViewModel BindViewModel() => new DynamicReportViewModel { Locale = _localeAgent.GetLocalesList(), ReportTypeList = _importAgent.GetImportTypeList(true), IsImportCompleted = _importAgent.CheckImportProcess() };

        //Delete dynamic report.
        public virtual bool DeleteDynamicReport(string customReportTemplateId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(customReportTemplateId))
            {
                try
                {
                    return _reportClient.DeleteDynamicReport(new ParameterModel { Ids = customReportTemplateId });
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.DynamicReports.ToString(), TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        default:
                            errorMessage = Admin_Resources.ErrorFailedToDelete;
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.DynamicReports.ToString(), TraceLevel.Error);
                    errorMessage = Admin_Resources.ErrorFailedToDelete;
                    return false;
                }
            }
            return false;
        }

        //Get the Attributes and Filters on type of Export
        public virtual List<List<SelectListItem>> GetExportData(string dynamicReportType)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            List<List<SelectListItem>> data = new List<List<SelectListItem>>();
            DynamicReportModel dataList = _reportClient.GetExportData(dynamicReportType);
            List<SelectListItem> columnList = new List<SelectListItem>();
            List<SelectListItem> parametersList = new List<SelectListItem>();

            dataList.Columns.ColumnList.ToList().ForEach(x => { columnList.Add(new SelectListItem() { Text = x.ColumnName }); });
            dataList.Parameters.ParameterList.ToList().ForEach(x => { parametersList.Add(new SelectListItem() { Text = x.Name, Value = Convert.ToString(x.Id) }); });

            data.Add(columnList);
            data.Add(parametersList);

            ZnodeLogging.LogMessage("columnList and parametersList list count:", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, new { ColumnListCount = columnList?.Count, ParameterListCount = parametersList?.Count });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            return data;
        }

        //Get the Attributes and Filters on type of Export
        public virtual List<List<string>> GetColumnList(int reportId, string dynamicReportType)
        => DynamicReportViewModelMap.GetColumnList(_reportClient.GetCustomReport(reportId));

        //Get the Operators based on the DataTypes.
        public virtual string GetOperators(string reportType, string filterName, out string datatype)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            if (!string.IsNullOrEmpty(filterName))
            {
                DynamicReportModel dataList = _reportClient.GetExportData(reportType);
                filterName = dataList?.Parameters?.ParameterList?.FirstOrDefault(x => string.Equals(x.Name?.Trim(), filterName?.Trim(), StringComparison.CurrentCultureIgnoreCase))?.DataType;
                filterName = filterName.ToLower().Equals(AdminConstants.ColumnBoolean) ? "String" : filterName;
                filterName = string.IsNullOrEmpty(filterName) ? "String" : filterName;
            }
            datatype = filterName;
            ZnodeLogging.LogMessage("Filter name:", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, new { FilterName = filterName });
            return FilterHelpers.GetOperators(filterName, 0);
        }

        //Get report view by report type.
        public virtual DynamicReportViewModel GetReportView(string reportType, int selectedValue)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("reportType and selectedValue to get catalog, price and warehouse list:", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, new { ReportType = reportType, SelectedValue = selectedValue });
            switch (reportType)
            {
                case AdminConstants.Product:
                    return GetCatalogList(selectedValue);
                case AdminConstants.Category:
                    return GetCatalogList(selectedValue);
                case AdminConstants.Pricing:
                    return GetPriceList(selectedValue);
                case AdminConstants.Inventory:
                    return GetWarehouseList(selectedValue);
                case AdminConstants.Voucher:
                    return GetWarehouseList(selectedValue);
                case AdminConstants.Account:
                    return GetWarehouseList(selectedValue);
            }
            return new DynamicReportViewModel();
        }

        //Get view by report type.
        public virtual string GetReportViewName(string reportType)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            switch (reportType)
            {
                case AdminConstants.Product:
                    return AdminConstants.ReportCatalogView;
                case AdminConstants.Category:
                    return AdminConstants.ReportCatalogView;
                case AdminConstants.Pricing:
                    return AdminConstants.ReportPriceView;
                case AdminConstants.Inventory:
                    return AdminConstants.ReportWarehouseView;
                case AdminConstants.Voucher:
                    return AdminConstants.ReportWarehouseView;
                case AdminConstants.Account:
                    return AdminConstants.ReportWarehouseView;
            }
            return AdminConstants.ReportCatalogView;
        }

        #endregion

        #region Private Method 

        //Get catalog list dynamic report view model.
        private DynamicReportViewModel GetCatalogList(int selectedValue)
        {
            CatalogListModel catalogList = _catalogClient.GetCatalogList(null, null, null, null, null);
            List<SelectListItem> catalogs = new List<SelectListItem>();
            catalogList.Catalogs.ForEach(catalog =>
            {
                catalogs.Add(new SelectListItem { Text = catalog.CatalogName, Value = Convert.ToString(catalog.PimCatalogId), Selected = catalog.PimCatalogId == selectedValue });
            });
            ZnodeLogging.LogMessage("Catalogs list count:", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, new { CatalogsListCount = catalogs?.Count});
            return new DynamicReportViewModel
            {
                CatalogList = catalogs
            };
        }

        //Get price list dynamic report view model.
        private DynamicReportViewModel GetPriceList(int selectedValue)
        {
            PriceListModel priceList = _priceClient.GetPriceList(null, null, null, null, null);
            List<SelectListItem> list = new List<SelectListItem>();
            priceList.PriceList.ForEach(price =>
            {
                list.Add(new SelectListItem { Text = price.ListName, Value = Convert.ToString(price.PriceListId), Selected = price.PriceListId == selectedValue });
            });
            ZnodeLogging.LogMessage("Price list count:", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, new { PriceListCount = list?.Count });
            return new DynamicReportViewModel
            {
                PriceList = list
            };
        }

        //Get warehouse list dynamic report view model.
        private DynamicReportViewModel GetWarehouseList(int selectedValue)
        {
            WarehouseListModel warehouseList = _warehouseClient.GetWarehouseList(null, null, null, null, null);
            List<SelectListItem> list = new List<SelectListItem>();
            warehouseList.WarehouseList.ForEach(warehouse =>
            {
                list.Add(new SelectListItem { Text = warehouse.WarehouseName, Value = Convert.ToString(warehouse.WarehouseId), Selected = warehouse.WarehouseId == selectedValue });
            });
            ZnodeLogging.LogMessage("Warehouse list count:", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, new { WarehouseListCount = list?.Count });
            return new DynamicReportViewModel { WarehouseList = list };
        }

        //Gets the filters for category list.
        private FilterCollection GetFiltersForCategoryList(int catalogId, string isAssociated, FilterCollection filters)
        {
            //If null set it to new.
            if (IsNull(filters))
                filters = new FilterCollection();

            //Remove IsAssociatedProducts and PimCatalogId filter.
            filters.RemoveAll(x => x.Item1.Equals(FilterKeys.IsAssociated, StringComparison.InvariantCultureIgnoreCase));
            filters.RemoveAll(x => x.Item1.Equals(ZnodePimCatalogEnum.PimCatalogId.ToString(), StringComparison.InvariantCultureIgnoreCase));

            //Add IsAssociatedProducts and PimCatalogId with new values.
            filters.Add(new FilterTuple(FilterKeys.IsAssociated.ToLower(), FilterOperators.Equals, isAssociated));
            filters.Add(new FilterTuple(ZnodePimCatalogEnum.PimCatalogId.ToString(), FilterOperators.Equals, catalogId.ToString()));

            return filters;
        }

        //Get the Report Type Enum Values based on the report type. In case report Type Not found, default first value of Report type gets returned.
        private string SetReportTypeSetting(string reportType)
        {
            ReportTypeEnum reportEnum;
            Enum.TryParse(reportType, out reportEnum);
            return reportEnum.ToString();
        }

        //Get filter list.
        private List<SelectListItem> GetFilterList(string filterName, DynamicReportModel dataList, DynamicReportViewModel reportViewModel)
        {
            List<SelectListItem> reportFilter = new List<SelectListItem>();
            dataList.Parameters.ParameterList.ToList().ForEach(x =>
            {
                reportFilter.Add(new SelectListItem()
                {
                    Text = x.Name,
                    Value = Convert.ToString(x.Id),
                    Selected = Equals(filterName.Trim(), x.Name.Trim())
                });
            });
            return reportFilter;
        }

        //Get Operator list.
        private List<SelectListItem> GetOperatorList(string OperatorName, DynamicReportModel dataList, DynamicReportViewModel reportViewModel)
        {
            List<SelectListItem> reportOperator = new List<SelectListItem>();
            string column = GetColumnDataType(dataList, reportViewModel);
            var list = FilterHelpers.GetCustomReportOperators(column, 0);
            list?.ForEach(x => { reportOperator.Add(new SelectListItem() { Text = Convert.ToString(x.displayname), Value = Convert.ToString(x.id), Selected = Equals(Convert.ToString(x.displayname).Trim().ToLower(), OperatorName.Trim().ToLower()) }); });
            return reportOperator;
        }

        //Get column data type.
        private static string GetColumnDataType(DynamicReportModel dataList, DynamicReportViewModel reportViewModel)
        {
            string column = reportViewModel.ReportFilter.Where(x => x.Selected)?.FirstOrDefault()?.Text;
            if (!string.IsNullOrEmpty(column))
            {
                column = dataList.Parameters.ParameterList.Where(x => x.Name.Trim().Equals(column.Trim())).ToList().FirstOrDefault()?.DataType;
                column = column.Equals(AdminConstants.ColumnDateTime) ? "String" : column;
                column = string.IsNullOrEmpty(column) ? "String" : column;
            }
            ZnodeLogging.LogMessage("Column data type:", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, new { ColumnData = column });
            return column;
        }

        #endregion
    }
}