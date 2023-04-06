namespace Znode.Engine.Api.Models
{
    public class NavigationModel : BaseModel
    {
        public int? CurrentIndex { get; set; }
        public int? TotalCount { get; set; }
        public string PreviousRecordId { get; set; }
        public string NextRecordId { get; set; }
    }
}
