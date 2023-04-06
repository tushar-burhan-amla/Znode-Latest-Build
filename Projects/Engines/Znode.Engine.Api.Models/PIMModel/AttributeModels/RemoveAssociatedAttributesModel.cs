namespace Znode.Engine.Api.Models
{
    public class RemoveAssociatedAttributesModel : BaseModel
    {
        public int PimAttributeGroupId { get; set; }
        public string PimAttributeIds { get; set; }
        public bool isCategory { get; set; }
    }
}
