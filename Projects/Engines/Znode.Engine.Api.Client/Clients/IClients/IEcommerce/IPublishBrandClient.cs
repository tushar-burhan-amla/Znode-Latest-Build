using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IPublishBrandClient : IBaseClient
    {
        /// <summary>
        /// Gets the list of brands.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with brand list.</param>
        /// <param name="filters">Filters to be applied on brand list.</param>
        /// <param name="sorts">Sorting to be applied on brand list.</param>
        /// <param name="pageIndex">Start page index of brand list.</param>
        /// <param name="pageSize">Page size of brand list.</param>
        /// <returns>Brand list model.</returns>
        BrandListModel GetPublishBrandList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets a brand by brand Id.
        /// </summary>
        /// <param name="brandId">brandId of the brand to be retrieved.</param>
        /// <param name="localeId">localeId of the locale.</param>
        /// <returns>Brand model.</returns>
        BrandModel GetPublishBrand(int brandId, int localeId, int portalId);

    }
}
