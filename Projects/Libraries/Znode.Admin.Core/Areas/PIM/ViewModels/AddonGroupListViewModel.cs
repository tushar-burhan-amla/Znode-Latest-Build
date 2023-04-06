using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class AddonGroupListViewModel : BaseViewModel
    {
        public List<AddonGroupViewModel> AddonGroups { get; set; }

        public int ParentProductId { get; set; }

        public GridModel GridModel { get; set; }

        public AddonGroupListViewModel()
        {
            AddonGroups = new List<AddonGroupViewModel>();
        }
    }
}