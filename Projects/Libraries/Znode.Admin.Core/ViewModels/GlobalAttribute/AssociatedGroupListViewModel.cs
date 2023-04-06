using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Engine.Admin.ViewModels
{
    public class AssociatedGroupListViewModel : BaseViewModel
    {
        public List<AttributeGroupViewModel> AssignedAttributeGroups { get; set; }
        public int AttributeFamilyId { get; set; }
    }
}
