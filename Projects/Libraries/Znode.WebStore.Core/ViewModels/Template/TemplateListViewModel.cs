using System.Collections.Generic;
using Znode.Engine.WebStore.Models;

namespace Znode.Engine.WebStore.ViewModels
{
    public class TemplateListViewModel : BaseViewModel
    {
        public List<TemplateViewModel> List { get; set; }
        public GridModel GridModel { get; set; }
        public string RoleName { get; set; }
    }
}