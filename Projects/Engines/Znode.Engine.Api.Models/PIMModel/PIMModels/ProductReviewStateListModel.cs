using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ProductReviewStateListModel : BaseListModel
    {
        public List<ProductReviewStateModel> ProductReviewStates { get; set; }

        public ProductReviewStateListModel()
        {
            ProductReviewStates = new List<ProductReviewStateModel>();
        }
    }
}
