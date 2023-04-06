namespace Znode.Engine.Api.Models
{
    public class ReportModel : BaseModel
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string FolderName { get; set; }
        public int CustomReportTemplateId { get; set; }
        public int ReportTypeId { get; set; }
        public string ReportType { get; set; }
    }
}
