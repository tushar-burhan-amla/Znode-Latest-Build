using System.Collections.Generic;


namespace Znode.Engine.Api.Models
{
    public class AddToCartModel : BaseModel
    {
        public int PublishedCatalogId { get; set; }
        public int LocaleId { get; set; }
        public int? UserId { get; set; }
        public int PortalId { get; set; }
        public decimal Quantity { get; set; }
        public decimal AddOnQuantity { get; set; }
        public string SKU { get; set; }
        public string CookieMappingId { get; set; }
        public string ZipCode { get; set; }
        public int ShippingId { get; set; }
        public bool HasError { get; set; }
        public UserModel UserDetails { get; set; }

        public List<ShoppingCartItemModel> ShoppingCartItems { get; set; }
        public List<CouponModel> Coupons { get; set; }
        public decimal CartCount { get; set; }
        public AddToCartModel()
        {
            ShoppingCartItems = new List<ShoppingCartItemModel>();
        }
    }
}
