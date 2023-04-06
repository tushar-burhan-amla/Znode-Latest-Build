using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class ProviderEngineListViewModel : BaseViewModel
    {
        public List<ProviderEngineViewModel> ProvideEngineTypes { get; set; }
        public GridModel GridModel { get; set; }

        public ProviderEngineListViewModel()
        {
            ProvideEngineTypes = new List<ProviderEngineViewModel>();
        }
    }
}