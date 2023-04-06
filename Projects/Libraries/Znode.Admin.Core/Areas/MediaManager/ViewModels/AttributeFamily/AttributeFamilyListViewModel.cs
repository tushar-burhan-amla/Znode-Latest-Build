using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class AttributeFamilyListViewModel : BaseViewModel
    {
        public List<AttributeFamilyViewModel> AttributeFamilies { get; set; }
        public GridModel GridModel { get; set; }
    }
}