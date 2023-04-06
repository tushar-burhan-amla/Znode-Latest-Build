using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class PIMFamilyGroupAttributeListViewModel : BaseViewModel
    {
        public List<PIMFamilyGroupAttributeViewModel> FamilyGroupAttributeList { get; set; }

        public PIMFamilyGroupAttributeListViewModel()
        {
            FamilyGroupAttributeList = new List<PIMFamilyGroupAttributeViewModel>();
        }

    }
}