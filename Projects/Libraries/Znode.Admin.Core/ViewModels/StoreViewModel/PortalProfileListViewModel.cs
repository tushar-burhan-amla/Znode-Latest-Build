using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class PortalProfileListViewModel : BaseViewModel
    {
        public List<PortalProfileViewModel> PortalProfiles { get; set; }
        public List<SelectListItem> Profiles { get; set; }
        public GridModel GridModel { get; set; }
        public int PortalId { get; set; }
        public int? ProfileCount { get; set; }
        public int? ActiveProfileCount { get; set; }
        public ProfileViewModel ProfileName { get; set; }
        public string PortalName { get; set; }
        public PortalProfileListViewModel()
        {
            PortalProfiles = new List<PortalProfileViewModel>();
            GridModel = new GridModel();
        }
    }
}