namespace Znode.Engine.Api.Models
{
    public class CasePriorityModel : BaseModel
    {
        public int CasePriorityId { get; set; }
        public int DisplayOrder { get; set; }
        public string CasePriorityName { get; set; }
    }
}
