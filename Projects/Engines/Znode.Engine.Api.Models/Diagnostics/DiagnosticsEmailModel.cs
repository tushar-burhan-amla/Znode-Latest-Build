namespace Znode.Engine.Api.Models
{
    public class DiagnosticsEmailModel : BaseModel
    {
        public string CaseNumber { get; set; }
        public string MergedText { get; set; }
    }
}
