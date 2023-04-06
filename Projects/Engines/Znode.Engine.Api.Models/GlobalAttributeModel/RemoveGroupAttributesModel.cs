namespace Znode.Engine.Api.Models
{
    public class RemoveGroupAttributesModel : BaseModel
    {
        public int GlobalAttributeGroupId { get; set; }
        public string GlobalAttributeIds { get; set; }
    }
}
