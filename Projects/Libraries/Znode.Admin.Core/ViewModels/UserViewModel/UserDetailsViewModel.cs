using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class UserDetailsViewModel : BaseViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = "RequiredUserName")]
        [RegularExpression("^((\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*)\\s*[,]{0,1}\\s*)+$", ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorEmailAddress)]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = "RequiredUserId")]
        public int UserId { get; set; }

        public int? PortalId { get; set; }
    }
}
