using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class ProductAllReviewListViewModel : BaseViewModel
    {
        public List<ProductReviewViewModel> AllReviewsList { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }
}
