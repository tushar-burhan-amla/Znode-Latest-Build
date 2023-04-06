using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class FamilyGroupAttributeListViewModel : BaseViewModel
    {
        public List<FamilyGroupAttributeViewModel> FamilyGroupAttributeList { get; set; }

        public FamilyGroupAttributeListViewModel()
        {
            FamilyGroupAttributeList = new List<FamilyGroupAttributeViewModel>();
        }

    }
}