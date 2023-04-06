using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class AddToCartViewModel : BaseViewModel
    {
        public AddToCartViewModel()
        {
            ExternalId = System.Guid.NewGuid().ToString();
            ShoppingCartItems = new List<ShoppingCartItemModel>();
        }

        public string SKU { get; set; }
        public string BundleProductSKUs { get; set; }
        public string AddOnProductSKUs { get; set; }
        public string GroupProductSKUs { get; set; }
        public string ConfigurableProductSKUs { get; set; }
        public string AutoAddonSKUs { get; set; }
        public string PersonalisedCodes { get; set; }
        public string PersonalisedValues { get; set; }
        public string ProductType { get; set; }

        public decimal Quantity { get; set; }
        public decimal AddOnQuantity { get; set; }
        public string GroupProductsQuantity { get; set; }

        public string ExternalId { get; set; }
        public int PublishedCatalogId { get; set; }
        public int LocaleId { get; set; }
        public int UserId { get; set; }
        public int PortalId { get; set; }
        public int CatalogId { get; set; }

        public List<ShoppingCartItemModel> ShoppingCartItems { get; set; }
        public Dictionary<string, object> PersonaliseValuesList { get; set; }
        public List<AssociatedProductModel> GroupProducts { get; set; }
        public List<AssociatedProductModel> AssociatedAddOnProducts { get; set; }

        public bool IsQuote { get; set; }
    }
}