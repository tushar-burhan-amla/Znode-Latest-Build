using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class ContainerVariantListViewModel : BaseViewModel
    {
        public List<ContainerVariantViewModel> ContainerVariants { get; set; }
        public GridModel GridModel { get; set; }
        public string ContainerKey { get; set; }
    }
}
