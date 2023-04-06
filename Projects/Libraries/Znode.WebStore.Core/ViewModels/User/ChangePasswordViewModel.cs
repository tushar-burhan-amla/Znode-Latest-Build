using Znode.Libraries.Resources;
using System.ComponentModel.DataAnnotations;
namespace Znode.Engine.WebStore.ViewModels
{
    public class ChangePasswordViewModel : BaseViewModel
    {
        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = "RequiredPassword")]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = "RequiredNewPassword")]
        [RegularExpression("^(?=.*[0-9])(?=.*[a-zA-Z]).{8,}$", ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = "ValidPassword")]
        [MaxLength(128, ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = "InValidPasswordLength")]
        public string NewPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = "RequiredNewConfirmPassword")]
        [Compare("NewPassword", ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = "ErrorPasswordMatch")]
        public string ReTypeNewPassword { get; set; }

        public string UserName { get; set; }
        public bool IsResetPassword { get; set; }
        public string PasswordToken { get; set; }
    }
}