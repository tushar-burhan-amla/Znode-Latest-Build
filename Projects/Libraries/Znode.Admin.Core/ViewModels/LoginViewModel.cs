using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        [Display(Name = ZnodeAdmin_Resources.Username, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredUserName)]
        public string Username { get; set; }

        [Display(Name = ZnodeAdmin_Resources.Password, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredPassword)]
        public string Password { get; set; }

        [Display(Name = ZnodeAdmin_Resources.RememberMe, ResourceType = typeof(Admin_Resources))]
        public bool RememberMe { get; set; }          

        public bool IsResetPassword { get; set; }

        public bool IsResetAdmin { get; set; }

        public string PasswordResetToken { get; set; }

        public int? ProfileId { get; set; }

        public bool IsResetAdminPassword { get; set; }

        public bool IsAdminUser { get; set; }

        public CMSWidgetsListViewModel Widgets { get; set; }

    }
}