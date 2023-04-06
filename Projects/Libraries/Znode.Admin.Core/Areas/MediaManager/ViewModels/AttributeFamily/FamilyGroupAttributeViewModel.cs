

namespace Znode.Engine.Admin.ViewModels
{
    public class FamilyGroupAttributeViewModel : BaseViewModel
    {
        public int AttributeGroupId { get; set; }
        public int FamilyGroupAttributeId { get; set; }
        public int AttributeFamilyId { get; set; }
        public string AttributeGroupName { get; set; }
        public string AttributeGroupIds { get; set; }
    }
}