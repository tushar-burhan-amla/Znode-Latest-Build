using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class TemplateListViewModel : BaseViewModel
    {
        public List<TemplateViewModel> Templates { get; set; }
        public GridModel GridModel { get; set; }
    }
}