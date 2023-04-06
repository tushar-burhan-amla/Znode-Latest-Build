using System;

namespace Znode.Engine.WebStore.ViewModels
{
    public class WishListViewModel : BaseViewModel
    {
        public int UserWishListId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string SKU { get; set; }
        public DateTime WishListAddedDate { get; set; }
        public ProductViewModel Product { get; set; }
        public string AddOnProductSKUs { get; set; }
        public int PortalId { get; set; }
    }
}