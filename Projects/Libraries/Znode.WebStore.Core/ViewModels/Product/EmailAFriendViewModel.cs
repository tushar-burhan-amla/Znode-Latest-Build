using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.ViewModels
{
    public class EmailAFriendViewModel:BaseViewModel
    {
        [RegularExpression(WebStoreConstants.EmailRegularExpression, ErrorMessageResourceName = ZnodeAdmin_Resources.ValidEmailAddress, ErrorMessageResourceType = typeof(WebStore_Resources))]
        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.RequiredEmail)]
        public string YourMailId { get; set; }
        [RegularExpression(WebStoreConstants.EmailRegularExpression, ErrorMessageResourceName = ZnodeAdmin_Resources.ValidEmailAddress, ErrorMessageResourceType = typeof(WebStore_Resources))]
        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.RequiredEmail)]
        public string FriendMailId { get; set; }

        public string ProductUrl { get; set; }
        public string ProductName { get; set; }
        public int LocaleId { get; set; }
        public int CatalogId { get; set; }
        public int PortalId { get; set; }

    }
}