using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class AddonGroupProductListViewModel : BaseViewModel
    {
        public List<AddonGroupProductViewModel> AddonGroupProductViewModel { get; set; }

        public AddonGroupProductListViewModel()
        {
            AddonGroupProductViewModel = new List<AddonGroupProductViewModel>();
        }
    }
}
