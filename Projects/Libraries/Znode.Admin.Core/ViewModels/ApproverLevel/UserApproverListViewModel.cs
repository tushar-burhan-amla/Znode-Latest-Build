using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class UserApproverListViewModel : BaseViewModel
    {
        public List<UserApproverViewModel> UserApprover { get; set; }
        public List<SelectListItem> Levels { get; set; }
        [Display(Name = ZnodeAdmin_Resources.TextApprovalTypes, ResourceType = typeof(Admin_Resources))]
        public string PermissionCode { get; set; }
        public int? PortalId { get; set; }
        public int? AccountId { get; set; }
        public int UserId { get; set; }
        public int? AccountPermissionAccessId { get; set; }
        public int? AccountUserPermissionId { get; set; }
    }
}
