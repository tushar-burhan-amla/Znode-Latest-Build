using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class DynamicReportViewModel : BaseViewModel
    {
        public int CustomReportTemplateId { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelDynamicReportName, ResourceType = typeof(Admin_Resources))]
        public string ReportName { get; set; }
        public int ReportTypeId { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelDynamicReportType, ResourceType = typeof(Admin_Resources))]
        public string ReportType { get; set; }
        public string StoredProcedureName { get; set; }
        public List<SelectListItem> ParameterList { get; set; }
        public ReportParameterListViewModel Parameters { get; set; }
        public ReportColumnsListViewModel Columns { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelLocale, ResourceType = typeof(Admin_Resources))]
        public int LocaleId { get; set; }
        public List<SelectListItem> ReportTypeList { get; set; }
        public List<SelectListItem> ReportFilter { get; set; }
        public List<SelectListItem> ReportOperator { get; set; }
        public string FilterValue { get; set; }        
        public int? CatalogId { get; set; }
        public List<SelectListItem> CatalogList { get; set; }
        public int? PriceId { get; set; }
        public List<SelectListItem> PriceList { get; set; }
        public int? WarehouseId { get; set; }
        public List<SelectListItem> WarehouseList { get; set; }
        public bool IsImportCompleted { get; set; }
    }
}