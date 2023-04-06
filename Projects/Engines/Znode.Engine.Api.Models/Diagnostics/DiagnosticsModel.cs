namespace Znode.Engine.Api.Models
{
    public class DiagnosticsModel : BaseModel
    {
        public string Category { get; set; }
        public string Item { get; set; }
        public bool Status { get; set; }
    }
}
