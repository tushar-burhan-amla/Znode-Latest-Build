using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class ProfileCatalogListViewModel : BaseViewModel
    {
        public List<ProfileCatalogViewModel> List { get; set; }
        public GridModel GridModel { get; set; }
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public int PortalId { get; set; }
        public int? ParentProfileId { get; set; }
    }
}