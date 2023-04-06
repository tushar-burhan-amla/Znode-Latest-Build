using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class AssignedEntityGroupListViewModel : BaseViewModel
    {
        public AssignedEntityGroupListViewModel()
        {
            AssignedEntityGroupList = new List<AssignedEntityGroupViewModel>();
        }

        public int EntityId { get; set; }
        public List<AssignedEntityGroupViewModel> AssignedEntityGroupList { get; set; }

    }
}
