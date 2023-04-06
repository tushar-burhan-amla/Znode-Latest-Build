using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class PIMAttributeListViewModel:BaseViewModel
    {
        public List<PIMAttributeViewModel> List { get; set; }
        public GridModel GridModel { get; set; }

        public int AttributeGroupId { get; set; }
        public int PimAttributeFamilyId { get; set; }
    }
}