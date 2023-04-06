using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Services.Maps
{
    public static class ProductReviewStateMap
    {
        public static ProductReviewStateModel ToModel(ZnodeProductReviewState entity)
        {
            if (IsNull(entity))
                return null;

            ProductReviewStateModel model = new ProductReviewStateModel
            {
                ReviewStateID = entity.ProductReviewStateId,
                ReviewStateName = entity.ReviewStateName,
                Description = entity.Description
            };
            return model;
        }
    }
}
