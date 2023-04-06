namespace Znode.Engine.Api.Models
{
    public class ReportViewModel : BaseModel
    {
        public int ReportViewId { get; set; }
        public int UserId { get; set; }
        public string ReportCode { get; set; }
        public string ReportName { get; set; }
        public string LayoutXml { get; set; }
        public bool? IsActive { get; set; }
    }
}
