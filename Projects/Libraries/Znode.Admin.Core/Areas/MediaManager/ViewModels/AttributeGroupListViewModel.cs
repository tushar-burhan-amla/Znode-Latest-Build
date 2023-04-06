using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class AttributeGroupListViewModel :BaseViewModel
    {
        public List<AttributeGroupViewModel> AttributeGroups { get; set; }
        public GridModel GridModel { get; set; }

        public List<BaseDropDownList> UnAssignedAttributeGroups { get; set; }
        public int AttributeFamilyId { get; set; }
        public AttributeGroupListViewModel()
        {
            AttributeGroups = new List<AttributeGroupViewModel>();
            GridModel = new GridModel();
        }
    }
}