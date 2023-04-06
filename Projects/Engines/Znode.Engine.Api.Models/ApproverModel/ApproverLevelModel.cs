namespace Znode.Engine.Api.Models
{
    public class ApproverLevelModel : BaseModel
    {
        public int ApproverLevelId { get; set; }
        public string LevelCode { get; set; }
        public string LevelName { get; set; }
        public string Description { get; set; }
    }
}
