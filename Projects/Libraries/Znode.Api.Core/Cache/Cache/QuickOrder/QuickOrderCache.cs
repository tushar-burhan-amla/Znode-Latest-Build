using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class QuickOrderCache : BaseCache, IQuickOrderCache
    {
        #region Private Variable
        private readonly IQuickOrderService _service;
        #endregion

        #region Constructor
        public QuickOrderCache(IQuickOrderService service)
        {
            _service = service;
        }
        #endregion

        #region Public Methods
        
        //Get Publish Product by Publish Product sku.
        public virtual string GetQuickOrderProductList(QuickOrderParameterModel parameters, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                QuickOrderProductListModel productList = _service.GetQuickOrderProductList(parameters, Filters);
                if (IsNotNull(productList))
                {
                    QuickOrderProductListResponse response = new QuickOrderProductListResponse { Products = productList.Products };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //This method return random quick order product basic details
        public virtual string GetDummyQuickOrderProductList(string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                QuickOrderProductListModel productList = _service.GetDummyQuickOrderProductList(Filters, Page);
                if (IsNotNull(productList))
                {
                    QuickOrderProductListResponse response = new QuickOrderProductListResponse { Products = productList.Products };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}
