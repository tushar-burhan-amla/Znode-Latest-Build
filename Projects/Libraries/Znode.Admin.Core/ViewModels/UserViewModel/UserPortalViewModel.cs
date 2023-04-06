using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    public class UserPortalViewModel : BaseViewModel
    {
        public int UserId { get; set; }

        public bool IsSelectAllPortal { get; set; }
        public bool IsCustomer { get; set; }

        [Required]
        public string AspNetUserId { get; set; }
        public string[] PortalIds { get; set; }

        public List<SelectListItem> Portals { get; set; }
        public List<SelectListItem> SelectedPortals { get; set; }
    }
}   