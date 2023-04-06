using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class PIMAttributeFamilyListViewModel : BaseViewModel
    {
        public List<PIMAttributeFamilyViewModel> List { get; set; }
        public GridModel GridModel { get; set; }
    }
}