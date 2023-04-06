using Znode.Libraries.Resources;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        [EmailAddress(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = "ValidEmailAddress", ErrorMessage = "")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = "RequiredUserName")]
        [Display(Name = ZnodeWebStore_Resources.LabelUserNameEmail, ResourceType = typeof(WebStore_Resources))]
        [MaxLength(100, ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = "InvalidUsernameLength")]
        [RegularExpression(WebStoreConstants.MultipleEmailRegEx, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.ValidEmailAddress)]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = "RequiredPassword")]
        [RegularExpression("^(?=.*[0-9])(?=.*[a-zA-Z]).{8,}$", ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = "ValidPassword")]
        [MaxLength(128, ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = "InValidPasswordLength")]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = "RequiredConfirmPassword")]
        [Compare("Password", ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = "ErrorPasswordMatch")]
        public string ReTypePassword { get; set; }

        public bool EmailOptIn { get; set; }
        
        public bool IsWebStoreUser { get; set; }

        public string StoreName { get; set; }
        public string ExternalId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserVerificationTypeEnum UserVerificationTypeCode { get; set; }
        public string BaseUrl { get; set; }
        public int ErrorCode { get; set; }
        public bool IsTradeCentricUser { get; set; }
    }
}