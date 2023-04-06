using System;

namespace Znode.Engine.Admin.ViewModels
{
    public class WishListItemViewModel : BaseViewModel
    {
        public int AccountId { get; set; }
        public DateTime CreateDate { get; set; }
        public string Custom { get; set; }
        public int ProductId { get; set; }
        public int WishListId { get; set; }
        //public ProductViewModel Product { get; set; }
    }
}