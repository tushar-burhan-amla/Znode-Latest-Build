using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Admin;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Services
{
    public class ReportService : BaseService, IReportService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeCustomReportTemplate> _customReportTemplateRepository;
        private readonly IZnodeRepository<ZnodeCustomReportFilter> _customReportFilterRepository;
        private readonly IZnodeRepository<ZnodeCustomReportTemplateColumn> _customReportTemplateColumnRepository;
        #endregion

        #region Constructor
        public ReportService()
        {
            _customReportTemplateRepository = new ZnodeRepository<ZnodeCustomReportTemplate>();
            _customReportFilterRepository = new ZnodeRepository<ZnodeCustomReportFilter>();
            _customReportTemplateColumnRepository = new ZnodeRepository<ZnodeCustomReportTemplateColumn>();
        }

        #endregion

        #region Public Methods
        //Get SSRS Reports list.      
        public ReportListModel GetReportList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters :", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            //SP parameters
            IZnodeViewRepository<ReportModel> objStoredProc = new ZnodeViewRepository<ReportModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            //Execute SP
            List<ReportModel> reportList = objStoredProc.ExecuteStoredProcedureList("Znode_GetCustomReport @WhereClause, @Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount)?.ToList();
            ZnodeLogging.LogMessage("reportList count:", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, new object[] { reportList?.Count});

            reportList?.Select(report => { report.Path = "/" + ZnodeApiSettings.ReportServerDynamicReportFolderName + "/" + report.Name + ".rdl"; return report; }).ToList();
            ReportListModel reportListModel = new ReportListModel();
            reportListModel.ReportList = reportList?.Count > 0 ? reportList : new List<ReportModel>();

            //Set for pagination
            reportListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            return reportListModel;
        }

        //Get Attributes and Filters   
        public DynamicReportModel GetExportData(string dynamicReportType)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters dynamicReportType:", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, new object[] { dynamicReportType });

            DynamicReportModel model = new DynamicReportModel();
            ExecuteSpHelper executeSpHelper = new ExecuteSpHelper();
            executeSpHelper.GetParameter("@ReportType", dynamicReportType, ParameterDirection.Input, SqlDbType.NVarChar);
            DataSet ds = executeSpHelper.GetSPResultInDataSet("Znode_GetDynamicReportData");
            model.Columns = ConvertToColumnList(ds.Tables[0]);
            model.Parameters = ConvertToParamList(ds.Tables[1]);
            ZnodeLogging.LogMessage("DynamicReportModel:", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, new object[] { model });

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);

            return model;
        }

        //Get Attributes and Filters   
        public DynamicReportModel GetCustomReport(int customReportId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters customReportId:", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, new object[] { customReportId });

            DynamicReportModel model = new DynamicReportModel() { Columns = new ReportColumnsListModel { ColumnList = new List<ReportColumnModel>() }, Parameters = new ReportParameterListModel { ParameterList = new List<ReportParameterModel>() } };
            ZnodeCustomReportTemplate customReportTemplate = GetCustomReportTemplate(customReportId);
            if (IsNotNull(customReportTemplate))
            {
                model = GetExportData(customReportTemplate.ZnodeImportHead?.Name);
                model.ReportName = customReportTemplate?.ReportName;
                model.ReportType = customReportTemplate?.ZnodeImportHead?.Name;
                model.CustomReportTemplateId = customReportTemplate.CustomReportTemplateId;
                model.ReportTypeId = customReportTemplate.ImportHeadId;
                model.CatalogId = customReportTemplate.CatalogId;
                model.PriceId = customReportTemplate.PriceId;
                model.WarehouseId = customReportTemplate.WarehouseId;
                GetCustomReportColumn(customReportTemplate, model);
                GetCustomReportFilter(customReportTemplate, model);
                model.LocaleId = customReportTemplate.LocaleId;
            }
            ZnodeLogging.LogMessage("DynamicReportModel:", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, new object[] { model });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);

            return model;
        }

        //Generate the dynamic reports
        public bool GenerateDynamicReport(DynamicReportModel model, out int errorCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);

            try
            {
                if (CreateUpdateDynamicReport(model, out errorCode))
                    return model.CustomReportTemplateId > 0 ? UpdateCustomReport(model) : CreateCustomReport(model);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
                errorCode = 0;
                return false;
            }
        }

        //Delete custom report.
        public bool DeleteCustomReport(ParameterModel customReportIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("customReportIds to be deleted:", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, new object[] { customReportIds?.Ids });

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeCustomReportFilterEnum.CustomReportTemplateId.ToString(), FilterOperators.In, customReportIds.Ids.ToString()));
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            List<ZnodeCustomReportTemplate> customReportList = _customReportTemplateRepository.GetEntityList(whereClauseModel.WhereClause)?.ToList();

            if (DeleteDynamicReport(string.Join(",", customReportList?.Select(x => x.ReportName))))
            {
                _customReportTemplateColumnRepository.Delete(whereClauseModel.WhereClause);
                _customReportFilterRepository.Delete(whereClauseModel.WhereClause);
                _customReportTemplateRepository.Delete(whereClauseModel.WhereClause);
                return true;
            }
            return false;
        }
        #endregion

        #region Private Methods

        //Delete the dynamic reports
        private bool DeleteDynamicReport(string reportName)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("reportName to be deleted:", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, new object[] { reportName });

            ReportHelper helper = new ReportHelper();
            List<string> list = reportName?.Split(',')?.ToList();
            ZnodeLogging.LogMessage("report count to be deleted:", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, new object[] { list?.Count });

            if (list?.Count > 0)
            {
                try
                {
                    list.ForEach(report => { helper.DeleteDynamicReport(report); });
                    return true;
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
                    return false;
                }
            }
            return false;
        }

        //This method will convert the datable to List model.
        private ReportParameterListModel ConvertToParamList(DataTable dataTable)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);

            ReportParameterListModel model = new ReportParameterListModel();
            model.ParameterList = new List<ReportParameterModel>();
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow dr in dataTable.Rows)
                {
                    model.ParameterList.Add(new ReportParameterModel { Name = dr["Name"].ToString(), DataType = dr["DataType"].ToString(), Id = Convert.ToInt32(dr["Id"].ToString()) });
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);

            return model;
        }

        //This method will convert the datable to List model.
        private ReportColumnsListModel ConvertToColumnList(DataTable dataTable)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);

            ReportColumnsListModel model = new ReportColumnsListModel();
            model.ColumnList = new List<ReportColumnModel>();
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow dr in dataTable.Rows)
                {
                    model.ColumnList.Add(new ReportColumnModel { ColumnName = Convert.ToString(dr.ItemArray[0]) });
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Info);

            return model;
        }

        //Generate the whereclause to execute the procedure for dynamic reports.
        private string GenerateWhereClauseFromFilter(string reportType, List<ReportParameterModel> parameters)
        {
            FilterCollection filter = new FilterCollection();
            foreach (ReportParameterModel item in parameters)
                filter.Add(new FilterTuple(item.Name, GetFilterOperator(item.Operator), item.Value.ToLower().Equals("true") ? "True" : string.Equals(item.DataType, ZnodeConstant.DateTime, StringComparison.CurrentCultureIgnoreCase) ? "'" + item.Value + "'" : item.Value));

            if (reportType.ToLower().Contains("product") || reportType.ToLower().Contains("category"))
                return ProductService.GenerateXMLWhereClauseForSP(filter.ToFilterDataCollection());
            else
                return GetWhereClauseFromFilters(filter);
        }

        //Generates the where clause for reports.
        private string GetWhereClauseFromFilters(FilterCollection filter)
        {
            string whereClause = string.Empty;
            var filters = filter.ToFilterDataCollection();
            foreach (var tuple in filters)
            {
                if (!string.IsNullOrEmpty(whereClause))
                    whereClause += " AND ";

                whereClause += DynamicClauseHelper.GenerateReportsWhereClauseForSP(tuple);
            }
            return whereClause;
        }

        //Get the filter operators.
        private string GetFilterOperator(string filterOperator)
        {
            if (string.IsNullOrEmpty(filterOperator))
                return string.Empty;

            string returnValue = string.Empty;
            switch (filterOperator.ToLower())
            {
                case "contains":
                    returnValue = "cn";
                    break;
                case "equals":
                    returnValue = "eq";
                    break;
                case "ends with":
                    returnValue = "ew";
                    break;
                case "greater than":
                    returnValue = "gt";
                    break;
                case "greater or equal":
                    returnValue = "ge";
                    break;
                case "less than":
                    returnValue = "lt";
                    break;
                case "less or equal":
                    returnValue = "le";
                    break;
                case "note quals":
                    returnValue = "ne";
                    break;
                case "begins with":
                    returnValue = "sw";
                    break;
                case "like":
                    returnValue = "lk";
                    break;
                case "is":
                    returnValue = "is";
                    break;
                case "in":
                    returnValue = "in";
                    break;
                case "notin":
                    returnValue = "not in";
                    break;
                case "not contains":
                    returnValue = "ncn";
                    break;
                case "between":
                    returnValue = "bw";
                    break;
                case "before":
                    returnValue = "lt";
                    break;
                case "after":
                    returnValue = "gt";
                    break;
                case "on or before":
                    returnValue = "le";
                    break;
                case "on or after":
                    returnValue = "ge";
                    break;
                case "not on":
                    returnValue = "ne";
                    break;
                default:
                    returnValue = "eq";
                    break;
            }

            return returnValue;
        }

        //Create custom report.
        private bool CreateCustomReport(DynamicReportModel model)
        {
            ZnodeLogging.LogMessage("DynamicReportModel :", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, new object[] { model });

            ZnodeCustomReportTemplate customReportTemplate = CreateCustomReportTemplate(model);
            if (IsNotNull(customReportTemplate))
            {
                CreateCustomReportTemplateColumn(model, customReportTemplate.CustomReportTemplateId);
                CreateCustomReportFilter(model, customReportTemplate.CustomReportTemplateId);
                return true;
            }
            return false;
        }

        //Update custom report.
        private bool UpdateCustomReport(DynamicReportModel model)
        {
            UpdateCustomReportTemplate(model);
            UpdateCustomReportTemplateColumn(model, model.CustomReportTemplateId);
            UpdateCustomReportFilter(model, model.CustomReportTemplateId);
            return true;
        }

        //Create update dynamic report.
        private bool CreateUpdateDynamicReport(DynamicReportModel model, out int errorCode)
        {
            ReportHelper helper = new ReportHelper();
            errorCode = 0;
            string whereClause = GenerateWhereClauseFromFilter(model.ReportType, model.Parameters.ParameterList);
            ZnodeLogging.LogMessage("whereClause returned from GenerateWhereClauseFromFilter :", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, new object[] { whereClause });

            if (model.CustomReportTemplateId > 0)
                DeleteDynamicReport(model.ReportName);
            return helper.GenerateDynamicReport(model, whereClause, out errorCode);
        }

        //Create custom report.
        private ZnodeCustomReportTemplate CreateCustomReportTemplate(DynamicReportModel model)
        => _customReportTemplateRepository.Insert(new ZnodeCustomReportTemplate
        {
            ReportName = model.ReportName,
            LocaleId = model.LocaleId,
            ImportHeadId = model.ReportTypeId,
            CatalogId = model.CatalogId,
            PriceId = model.PriceId,
            WarehouseId = model.WarehouseId
        });

        //Update custom report.
        private bool UpdateCustomReportTemplate(DynamicReportModel model)
         => _customReportTemplateRepository.Update(new ZnodeCustomReportTemplate
         {
             CustomReportTemplateId = model.CustomReportTemplateId,
             ReportName = model.ReportName,
             LocaleId = model.LocaleId,
             ImportHeadId = model.ReportTypeId,
             CatalogId = model.CatalogId,
             PriceId = model.PriceId,
             WarehouseId = model.WarehouseId
         });

        //Create or insert custom report column.
        private List<ZnodeCustomReportTemplateColumn> CreateCustomReportTemplateColumn(DynamicReportModel model, int customReportTemplateId)
        {
            List<ZnodeCustomReportTemplateColumn> columnList = new List<ZnodeCustomReportTemplateColumn>();
            model?.Columns?.ColumnList.ForEach(column =>
            {
                columnList.Add(new ZnodeCustomReportTemplateColumn { ColumnName = column.ColumnName, CustomReportTemplateId = customReportTemplateId });
            });
            return _customReportTemplateColumnRepository.Insert(columnList)?.ToList();
        }

        //Update custom report column.
        private List<ZnodeCustomReportTemplateColumn> UpdateCustomReportTemplateColumn(DynamicReportModel model, int customReportTemplateId)
        {
            ZnodeLogging.LogMessage("Update custom report Template column with customReportTemplateId:", ZnodeLogging.Components.Reports.ToString(), TraceLevel.Verbose, new object[] { customReportTemplateId });

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeCustomReportTemplateColumnEnum.CustomReportTemplateId.ToString(), ProcedureFilterOperators.Equals, model.CustomReportTemplateId.ToString()));
            bool status = _customReportTemplateColumnRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
            if (status)
            {
                List<ZnodeCustomReportTemplateColumn> columnList = new List<ZnodeCustomReportTemplateColumn>();
                model?.Columns?.ColumnList.ForEach(column =>
                {
                    columnList.Add(new ZnodeCustomReportTemplateColumn { ColumnName = column.ColumnName, CustomReportTemplateId = customReportTemplateId });
                });
                return _customReportTemplateColumnRepository.Insert(columnList)?.ToList();
            }
            return new List<ZnodeCustomReportTemplateColumn>();
        }

        //Create custom report filter.
        private List<ZnodeCustomReportFilter> CreateCustomReportFilter(DynamicReportModel model, int customReportTemplateId)
        {
            List<ZnodeCustomReportFilter> filterList = new List<ZnodeCustomReportFilter>();
            model?.Parameters?.ParameterList.ForEach(filter =>
            {
                filterList.Add(new ZnodeCustomReportFilter { Action = filter.Operator, FilterName = filter.Name, FilterValue = filter.Value, CustomReportTemplateId = customReportTemplateId });
            });
            return _customReportFilterRepository.Insert(filterList)?.ToList();
        }

        //Update custom report filter.
        private List<ZnodeCustomReportFilter> UpdateCustomReportFilter(DynamicReportModel model, int customReportTemplateId)
        {

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeCustomReportFilterEnum.CustomReportTemplateId.ToString(), ProcedureFilterOperators.Equals, model.CustomReportTemplateId.ToString()));
            _customReportFilterRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);

            if (model?.Parameters?.ParameterList?.Count > 0)
            {
                List<ZnodeCustomReportFilter> filterList = new List<ZnodeCustomReportFilter>();
                model?.Parameters?.ParameterList.ForEach(filter =>
                {
                    filterList.Add(new ZnodeCustomReportFilter { Action = filter.Operator, FilterName = filter.Name, FilterValue = filter.Value, CustomReportTemplateId = customReportTemplateId });
                });
                return _customReportFilterRepository.Insert(filterList)?.ToList();
            }
            return new List<ZnodeCustomReportFilter>();
        }

        //Get custom report. 
        private ZnodeCustomReportTemplate GetCustomReportTemplate(int customReportId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeCustomReportFilterEnum.CustomReportTemplateId.ToString(), FilterOperators.Equals, customReportId.ToString()));
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            _customReportTemplateRepository.EnableDisableLazyLoading = true;
            return _customReportTemplateRepository.GetEntity(whereClauseModel.WhereClause);
        }

        //Get custom report columns.
        private void GetCustomReportColumn(ZnodeCustomReportTemplate customReportTemplate, DynamicReportModel model)
        {
            customReportTemplate?.ZnodeCustomReportTemplateColumns?.ToList().ForEach(column =>
            {
                model?.Columns.CustomReportColumnList.Add(new ReportColumnModel { ColumnName = column.ColumnName });
            });
        }

        //Get custom report filters. 
        private void GetCustomReportFilter(ZnodeCustomReportTemplate customReportTemplate, DynamicReportModel model)
        {
            customReportTemplate?.ZnodeCustomReportFilters?.ToList().ForEach(filter =>
            {
                model?.Parameters.CustomReportParameterList.Add(new ReportParameterModel { Name = filter.FilterName, Value = filter.FilterValue, Operator = filter.Action });
            });
        }

        #endregion
    }
}
