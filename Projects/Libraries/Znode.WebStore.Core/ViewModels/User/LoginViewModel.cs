using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;
namespace Znode.Engine.WebStore.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = "RequiredUserName")]
        public string Username { get; set; }

        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = "RequiredPassword")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }      

        public bool IsResetPassword { get; set; }

        public string PasswordResetToken { get; set; }

        public int? PortalId { get; set; }

        public bool IsWebStoreUser { get; set; }

        public string ReturnUrl { get; set; }

        public bool IsSinglePageCheckout { get; set; }

        public bool IsFromSocialMedia { get; set; }

        public int? PublishCatalogId { get; set; }

        public int? ErrorCode { get; set; }
    }
}