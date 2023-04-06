using System.Web;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses.V2;
using Znode.Engine.Api.Models.V2;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class WebStoreCategoryCacheV2 : WebStoreCategoryCache, IWebStoreCategoryCacheV2
    {
        #region Private Variables
        private readonly ICategoryServiceV2 _service;
        #endregion

        #region Constructor
        public WebStoreCategoryCacheV2(ICategoryServiceV2 categoryservice): base(categoryservice)
        {
            _service = categoryservice;
        }
        #endregion

        public string GetCategoryProducts(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);

            if (HelperUtility.IsNull(data))
            {
                CategoryProductListModelV2 list = _service.GetCategoryProducts(Expands, Filters, Sorts, Page, HttpContext.Current.Request.QueryString[ZnodeConstant.RequiredAttributes]);
                if (list.CategoryProducts.Count > 0)
                {
                    CategoryProductListResponseV2 response = new CategoryProductListResponseV2 { CategoryProducts = list.CategoryProducts };

                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
    }
}
