using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class ProductReviewStateCache : BaseCache, IProductReviewStateCache
    {
        #region Private Variable
        private readonly IProductReviewStateService _service;
        #endregion

        #region Constructor
        public ProductReviewStateCache(IProductReviewStateService service)
        {
            _service = service;
        }
        #endregion

        #region Public Method
        public virtual string GetProductReviewStates(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ProductReviewStateListModel list = _service.GetProductReviewStates(Expands, Filters, Sorts, Page);
                if (list?.ProductReviewStates?.Count > 0)
                {
                    ProductReviewStateListResponse response = new ProductReviewStateListResponse { ProductReviewStates = list.ProductReviewStates };
                    response.MapPagingDataFromModel(list);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}