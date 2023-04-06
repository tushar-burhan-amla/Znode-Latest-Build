
namespace Znode.Engine.Api.Models
{
    public class DynamicReportModel : BaseModel
    {
        public int CustomReportTemplateId { get; set; }
        public int LocaleId { get; set; }
        public string ReportName { get; set; }
        public string ReportType { get; set; }
        public string StoredProcedureName { get; set; }
        public ReportParameterListModel Parameters { get; set; }
        public ReportColumnsListModel Columns { get; set; }
        public int ReportTypeId { get; set; }
        public int? CatalogId { get; set; }
        public int? PriceId { get; set; }
        public int? WarehouseId { get; set; }
    }
}
