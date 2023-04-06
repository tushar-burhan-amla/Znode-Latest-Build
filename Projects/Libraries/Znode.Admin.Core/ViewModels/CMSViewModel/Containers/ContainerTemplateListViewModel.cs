using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class ContainerTemplateListViewModel : BaseViewModel
    {
        public List<ContainerTemplateViewModel> ContainerTemplates { get; set; }
        public GridModel GridModel { get; set; }
    }
}
