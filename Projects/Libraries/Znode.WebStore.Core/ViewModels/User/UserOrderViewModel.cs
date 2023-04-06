using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.ViewModels
{
    public class UserOrderViewModel : BaseViewModel
    {
        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.ErrorOrderNORequired)]
        public string OrderNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [EmailAddress(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.ValidEmailAddress)]
        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.RequiredEmail)]
        public string EmailAddress { get; set; }
    }
}
