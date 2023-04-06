namespace Znode.Engine.Api.Models
{
    public class GlobalEntityModel : BaseModel
    {
        public int GlobalEntityId { get; set; }
        public string EntityName { get; set; }
        public bool IsActive { get; set; }
        public bool IsFamilyUnique { get; set; }
    }
}
