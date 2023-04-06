using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class PIMAttributeGroupListViewModel : BaseViewModel
    {
        public int AttributeGroupId { get; set; }
        public List<PIMAttributeGroupViewModel> AttributeGroupList { get; set; }
        public GridModel GridModel { get; set; }
        public int PimAttributeFamilyId { get; set; }
    }
}