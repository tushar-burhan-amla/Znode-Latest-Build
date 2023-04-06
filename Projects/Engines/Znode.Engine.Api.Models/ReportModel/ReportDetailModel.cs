namespace Znode.Engine.Api.Models
{
    public class ReportDetailModel : BaseModel
    {
        public int ReportDetailId { get; set; }
        public int ReportCategoryId { get; set; }
        public string ReportCode { get; set; }
        public string ReportName { get; set; }
        public string Description { get; set; }
    }
}
