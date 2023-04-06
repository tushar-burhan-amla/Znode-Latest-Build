using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class PortalProfileViewModel : BaseViewModel
    {
        public int PortalProfileID { get; set; }
        public int PortalId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.HeaderProfile, ResourceType = typeof(Admin_Resources))]
        public int ProfileId { get; set; }
        public bool IsDefaultAnonymousProfile { get; set; } = true;
        public bool IsDefaultRegistedProfile { get; set; } = true;
        public string ProfileName { get; set; }
        public string PortalName { get; set; }
        public int? ParentProfileId { get; set; }
        public string ProfileNumber { get; set; }
        public List<SelectListItem> Profiles { get; set; }
        public List<SelectListItem> IsDefaultAnonymousProfileList { get; set; }
        public List<SelectListItem> IsDefaultRegistedProfileList { get; set; }
        public bool IsDefaultAnonymousProfileEnable { get; set; } = false;
        public bool IsDefaultRegistedProfileEnable { get; set; } = false;
    }
}