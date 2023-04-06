using System;
namespace Znode.Engine.Api.Models
{
    public class WishListModel : BaseModel
    {
        public int UserWishListId { get; set; }
        public int UserId { get; set; }
        public string SKU { get; set; }
        public DateTime WishListAddedDate { get; set; }
        public string AddOnProductSKUs { get; set; }
        public int PortalId { get; set; }
    }
}
