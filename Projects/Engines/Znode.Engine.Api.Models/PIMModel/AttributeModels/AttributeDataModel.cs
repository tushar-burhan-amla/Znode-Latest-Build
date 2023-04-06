namespace Znode.Engine.Api.Models
{
    public class AttributeDataModel : BaseModel
    {
        public int PimAttributeId { get; set; }
        public int? AttributeFamilyId { get; set; }
        public int? AttributeGroupId { get; set; }
        public string PimAttributeIds { get; set; }
    }
}
