namespace Znode.Engine.Api.Models
{
    public class CaseStatusModel : BaseModel
    {
        public int CaseStatusId { get; set; }
        public int DisplayOrder { get; set; }
        public string CaseStatusName { get; set; }
    }
}
