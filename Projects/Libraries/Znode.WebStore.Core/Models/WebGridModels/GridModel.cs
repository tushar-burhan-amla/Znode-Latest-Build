using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Helpers;
using System.Web.Mvc;
using Znode.Engine.Api.Models;

namespace Znode.Engine.WebStore.Models
{
    public class GridModel
    {
        public List<WebGridColumn> WebGridColumn { get; set; }
        public List<dynamic> DataTableToList { get; set; }
        public string XmlOperatorString { get; set; }
        public int RowCount { get; set; }
        public string FilterCondition { get; set; }
        public int TotalRowCount { get; set; }
        public int RowPerPageCount { get; set; }
        public List<dynamic> TotalOperatorList { get; set; }
        public int TableWidth { get; set; }
        public int TotalRecordCount { get; set; }
        public GridSettingModel ReportSettingModel { get; set; }
        public FilterColumnListModel FilterColumn { get; set; }
        public SortDirection GridSortDirection { get; set; }
        public string GridSortColumn { get; set; }

        public int FolderId { get; set; }

        [UIHint("TwoListView")]
        public ListViewModel FilterColumnList { get; set; }
        public DropdownListModel FieldList { get; set; }

        /// <summary>
        /// Field name for fetch details records.
        /// </summary>
        public string SubRecordFilterKeyName { get; set; }

        /// <summary>
        /// Class name with namespace ex. : Znode.Engine.Admin.Agents.ReportsAgent
        /// </summary>
        public string DelegateTypeName { get; set; }

        /// <summary>
        /// Method name for fetching detail records.
        /// </summary>
        public string DelegateTypeMethodName { get; set; }

        /// <summary>
        /// Fields collection for graph X Axis 
        /// </summary>
        public List<SelectListItem> XAxisFields { get; set; }

        /// <summary>
        /// Fields collection for graph y Axis 
        /// </summary>
        public List<SelectListItem> YAxisFields { get; set; }
        //public List<SelectListItem> ExportFileType { get; set; }

        public int ExportFileTypeId { get; set; }
        public bool IsAdvanceSearchHide { get; set; }

        public List<WebGridColumn> AddNewRowWebGridColumn { get; set; }
        public List<dynamic> BulkAddNewRowDataList { get; set; }
        public bool IsPagingRequired { get; set; }
        public string FrontObjectName { get; set; }
        public ApplicationSettingDataModel GridSettingModel { get; set; }
        public List<dynamic> GridColumnList { get; set; }
        public string ViewMode { get; set; } = "List";

        public GridModel()
        {
            FilterColumnList = new ListViewModel();
            WebGridColumn = new List<System.Web.Helpers.WebGridColumn>();
            DataTableToList = new List<dynamic>();
        }
        public List<SelectListItem> ViewModeType { get; set; }
    }
}