using System.Collections.Specialized;
using Znode.Engine.Api.Models.V2;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ICategoryServiceV2 : ICategoryService
    {
        /// <summary>
        /// Gets a list of published Products based on Categories, SubCategories.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with product list.</param>
        /// <param name="filters">Filters to be applied on product list.</param>
        /// <param name="sorts">Sorting to be applied on product list.</param>
        /// <param name="page">Paging size and index</param>
        /// <param name="requiredAttributes">Comma-separated list of all the attribute required in the response.</param>        
        /// <returns>Category Product list model.</returns>
        CategoryProductListModelV2 GetCategoryProducts(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page, string requiredAttributes);
    }
}