using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class AttributesListViewModel : BaseViewModel
    {
        public List<AttributesViewModel> List { get; set; }
        public GridModel GridModel { get; set; }

    }
}