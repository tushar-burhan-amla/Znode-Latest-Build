using Znode.Libraries.Resources;
using System.ComponentModel.DataAnnotations;
namespace Znode.Engine.Admin.ViewModels
{
    public class ChangePasswordViewModel : BaseViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = "RequiredPassword")]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = "RequiredNewPassword")]
        [RegularExpression("^(?=.*[0-9])(?=.*[a-zA-Z]).{8,}$", ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = "ValidPassword")]
        [MaxLength(128, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = "InValidPasswordLength")]
        public string NewPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = "RequiredNewConfirmPassword")]
        [Compare("NewPassword", ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = "ErrorPasswordMatch")]
        public string ReTypeNewPassword { get; set; }

        public string UserName { get; set; }
        public bool IsResetPassword { get; set; }
        public string PasswordToken { get; set; }
    }
}