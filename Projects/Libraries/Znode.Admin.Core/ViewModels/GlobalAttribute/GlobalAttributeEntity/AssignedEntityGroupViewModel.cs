using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class AssignedEntityGroupViewModel : BaseViewModel
    {
        public int GlobalAttributeGroupId { get; set; }

        public string GroupCode { get; set; }
        public string AttributeGroupName { get; set; }
        public int? DisplayOrder { get; set; }
        public List<GlobalAttributeViewModel> Attributes { get; set; }

    }
}
