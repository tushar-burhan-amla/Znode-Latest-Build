using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class GlobalAttributeGroupListViewModel : BaseViewModel
    {
        public List<GlobalAttributeGroupViewModel> AttributeGroupList { get; set; }

        public GridModel GridModel { get; set; }
        public string GlobalEntity { get; set; }
        public int GlobalEntityId { get; set; }
        public string FamilyCode { get; set; }
    }
}