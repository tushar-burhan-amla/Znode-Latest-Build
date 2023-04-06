using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore;

namespace Znode.Engine.Core.ViewModels
{
    public class AddToCartViewModel : BaseViewModel
    {
        public string ProductId { get; set; }
        public int ParentProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public decimal Quantity { get; set; }
        public decimal AddOnQuantity { get; set; }
        public string GroupProductsQuantity { get; set; }
        public string SKU { get; set; }
        public string BundleProductSKUs { get; set; }
        public string AddOnProductSKUs { get; set; }
        public string ExternalId { get; set; }
        public string GroupProductSKUs { get; set; }
        public string ConfigurableProductSKUs { get; set; }
        public string PersonalisedCodes { get; set; }
        public string PersonalisedValues { get; set; }
        public string AutoAddonSKUs { get; set; }

        public int PublishedCatalogId { get; set; }

        public int LocaleId { get; set; }
        public int? UserId { get; set; }
        public int PortalId { get; set; }
        public string CookieMappingId { get; set; }
        public int? OmsSavedCartLineItemId { get; set; }
        public int? ParentOmsSavedcartLineItemId { get; set; }
        public string GroupId { get; set; }

        public int ShippingId { get; set; }
        public bool IsCallFromPDP { get; set; }
        public bool IsProductEdit { get; set; }

        public UserModel UserDetails { get; set; }

        public Dictionary<string, object> PersonaliseValuesList { get; set; }
        public List<ShoppingCartItemModel> ShoppingCartItems { get; set; }

        public List<AssociatedProductModel> GroupProducts { get; set; }

        public List<CouponModel> Coupons { get; set; }
        public decimal CartCount { get; set; }
        public bool IsRedirectToCart { get; set; }

        public List<AssociatedProductModel> AssociatedAddOnProducts { get; set; }
        public string ConfigurableProductQuantity { get; set; }
        public AddToCartViewModel()
        {
            ShoppingCartItems = new List<ShoppingCartItemModel>();
        }
    }
}
