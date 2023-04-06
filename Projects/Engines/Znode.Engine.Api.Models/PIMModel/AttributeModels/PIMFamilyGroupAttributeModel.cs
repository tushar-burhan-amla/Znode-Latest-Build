namespace Znode.Engine.Api.Models
{
    public class PIMFamilyGroupAttributeModel : BaseModel
    {
        public int AttributeGroupId { get; set; }
        public int? AttributeId { get; set; }
        public int FamilyGroupAttributeId { get; set; }
        public int AttributeFamilyId { get; set; }
        public string AttributeGroupName { get; set; }
        public int? DisplayOrder { get; set; }
    }
}
