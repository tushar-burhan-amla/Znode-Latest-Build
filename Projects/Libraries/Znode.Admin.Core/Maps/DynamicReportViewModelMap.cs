using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;
namespace Znode.Engine.Admin.Maps
{
    public static class DynamicReportViewModelMap
    {
        public static DynamicReportModel ToModel(DynamicReportViewModel model)
        {
            if (!Equals(model, null))
            {
                return new DynamicReportModel()
                {
                    ReportName = model.ReportName,
                    ReportType = model.ReportType,
                    ReportTypeId = model.ReportTypeId,
                    StoredProcedureName = model.StoredProcedureName,
                    Parameters = Equals(model.Parameters, null) ? new ReportParameterListModel() : AddParameters(model.Parameters.ParamList),
                    Columns = AddColumns(model.Columns.ColumnList),
                    LocaleId = model.LocaleId,
                    CustomReportTemplateId = model.CustomReportTemplateId,
                    CatalogId = model.CatalogId,
                    PriceId = model.PriceId,
                    WarehouseId = model.WarehouseId
                };
            }
            return null;
        }

        private static ReportColumnsListModel AddColumns(List<ReportColumnViewModel> columnList)
        {
            ReportColumnsListModel listModel = new ReportColumnsListModel();
            foreach (ReportColumnViewModel item in columnList)
            {
                ReportColumnModel model = new ReportColumnModel
                {
                    ColumnName = item.ColumnName
                };
                listModel.ColumnList.Add(model);
            }
            return listModel;
        }

        private static ReportColumnsListViewModel AddCustomReportColumns(DynamicReportModel model)
        {
            ReportColumnsListViewModel listModel = new ReportColumnsListViewModel();
            foreach (ReportColumnModel item in model.Columns.CustomReportColumnList)
            {
                ReportColumnViewModel reportModel = new ReportColumnViewModel
                {
                    ColumnName = item.ColumnName,
                };
                listModel.CustomReportColumnList.Add(reportModel);
            }
            foreach (ReportColumnModel item in model.Columns.ColumnList)
            {
                ReportColumnViewModel reportModel = new ReportColumnViewModel
                {
                    ColumnName = item.ColumnName
                };
                listModel.ColumnList.Add(reportModel);
            }
            return listModel;
        }
        private static ReportParameterListModel AddParameters(List<ReportParameterViewModel> paramList)
        {
            ReportParameterListModel listModel = new ReportParameterListModel();
            foreach (ReportParameterViewModel item in paramList)
            {
                ReportParameterModel model = new ReportParameterModel
                {
                    Name = item.Name,
                    Operator = item.Operator,
                    Value = item.Value,
                    DataType = item.DataType
                };
                listModel.ParameterList.Add(model);
            }
            return listModel;
        }

        private static ReportParameterListViewModel AddParameters(List<ReportParameterModel> customReportParamList, List<ReportParameterModel> paramList)
        {
            ReportParameterListViewModel listModel = new ReportParameterListViewModel();
            foreach (ReportParameterModel item in customReportParamList)
            {
                ReportParameterViewModel model = new ReportParameterViewModel
                {
                    Name = item.Name,
                    Operator = item.Operator,
                    Value = item.Value,
                    DataType = item.DataType
                };
                listModel.CustomReportParamList.Add(model);
            }
            foreach (ReportParameterModel item in paramList)
            {
                ReportParameterViewModel model = new ReportParameterViewModel
                {
                    Name = item.Name,
                    Operator = item.Operator,
                    Value = item.Value,
                    DataType = item.DataType
                };
                listModel.ParamList.Add(model);
            }

            return listModel;
        }

        public static DynamicReportViewModel ToViewModel(DynamicReportModel model)
        {
            if (!Equals(model, null))
            {
                return new DynamicReportViewModel()
                {
                    ReportName = model.ReportName,
                    ReportType = model.ReportType,
                    ReportTypeId = model.ReportTypeId,
                    CustomReportTemplateId = model.CustomReportTemplateId,
                    Parameters = Equals(model.Parameters, null) ? new ReportParameterListViewModel() : AddParameters(model.Parameters.CustomReportParameterList, model.Parameters.ParameterList),
                    Columns = AddCustomReportColumns(model),
                    LocaleId = model.LocaleId,
                    CatalogId = model.CatalogId,
                    PriceId = model.PriceId,
                    WarehouseId = model.WarehouseId
                };
            }
            return null;
        }

        //Get available column and selected columns.
        public static List<List<string>> GetColumnList(DynamicReportModel model)
        {
            List<List<string>> columnList = new List<List<string>>();
            List<string> availableColumnList = new List<string>();
            List<string> selectedColumnList = new List<string>();

            //Add column available column.
            availableColumnList.AddRange((from col in model?.Columns?.ColumnList
                                          where !((from col2 in model?.Columns?.CustomReportColumnList
                                                   select col2.ColumnName)?.ToList()).Contains(col.ColumnName)
                                          select col.ColumnName)?.ToList());

            //Add selected columns.
            selectedColumnList.AddRange((from col in model?.Columns?.CustomReportColumnList select col.ColumnName)?.ToList());

            //Add both list to columns list.
            columnList.Add(availableColumnList);
            columnList.Add(selectedColumnList);

            return columnList;
        }
    }
}