using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class WishListListModel : BaseListModel
    {
        public List<WishListModel> WishLists { get; set; }
        public PublishProductListModel Products { get; set; }
    }
}
