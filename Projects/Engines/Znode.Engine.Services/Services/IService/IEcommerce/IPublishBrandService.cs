using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IPublishBrandService
    {
        /// <summary>
        /// Get list of publish brands.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Brand list.</param>
        /// <param name="filters">Filters to be applied on Brand list.</param>
        /// <param name="sorts">Sorting to be applied on Brand list.</param>
        /// <param name="page">Page index.</param>
        /// <returns>Returns list of brand.</returns>
        BrandListModel GetPublishBrandList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get brand on the basis of Brand Id, Locale Id, Portal Id.
        /// </summary>
        /// <param name="brandId">Brand Id.</param>
        /// <param name="localeId">localeId Id.</param>
        /// <param name="portalId">portal Id.</param>
        /// <returns>Returns Brand Model.</returns>
        BrandModel GetPublishBrand(int brandId, int localeId, int portalId);
    }
}
