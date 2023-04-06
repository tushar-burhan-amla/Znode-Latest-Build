using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.ViewModels
{
    public class ProductCompareViewModel : BaseViewModel
    {
        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.RequiredEmail)]
        [RegularExpression(WebStoreConstants.EmailRegularExpression, ErrorMessageResourceName = ZnodeWebStore_Resources.ValidEmailAddress, ErrorMessageResourceType = typeof(WebStore_Resources))]
        public string SenderEmailAddress { get; set; }

        [RegularExpression(WebStoreConstants.EmailRegularExpression, ErrorMessageResourceName = ZnodeAdmin_Resources.ValidEmailAddress, ErrorMessageResourceType = typeof(WebStore_Resources))]
        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.RequiredEmail)]
        public string ReceiverEmailAddress { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public int PortalId { get; set; }
        public string ProductIds { get; set; }
        public string BaseUrl { get; set; }
        public string ProductName { get; set; }
        public bool IsProductDetails { get; set; }
        public int LocaleId { get; set; }
        public int CatalogId { get; set; }
        public List<ProductViewModel> ProductList { get; set; }
        public string WebstoreDomainName { get; set; }
        public string WebstoreDomainScheme { get; set; } 
        public bool IsShowPriceAndInventoryToLoggedInUsersOnly { get; set; }
    }
}