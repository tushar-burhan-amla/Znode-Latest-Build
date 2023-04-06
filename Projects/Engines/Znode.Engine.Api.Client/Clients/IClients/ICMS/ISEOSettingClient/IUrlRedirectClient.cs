using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IUrlRedirectClient : IBaseClient
    {
        /// <summary>
        /// Create Url Redirect.
        /// </summary>
        /// <param name="model">model to create.</param>
        /// <returns>Returns created model.</returns>
        UrlRedirectModel CreateUrlRedirect(UrlRedirectModel model);

        /// <summary>
        /// Get Url Redirect list.
        /// </summary>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sorts.</param>
        /// <param name="pageIndex">Page Index.</param>
        /// <param name="pageSize">Page Size.</param>
        /// <returns>Returns Url Redirect list.</returns>
        UrlRedirectListModel GetUrlRedirectList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get Url Redirect.
        /// </summary>
        /// <param name="filters">Filters.</param>
        /// <returns>Returns Url Redirect.</returns>
        UrlRedirectModel GetUrlRedirect(FilterCollection filters);

        /// <summary>
        /// Update Url Redirect.
        /// </summary>
        /// <param name="urlRedirectModel">Model to update.</param>
        /// <returns>Updated model.</returns>
        UrlRedirectModel UpdateUrlRedirect(UrlRedirectModel urlRedirectModel);

        /// <summary>
        /// Delete Url Redirect.
        /// </summary>
        /// <param name="urlRedirectId">Ids to delete.</param>
        /// <returns>Returns true if deleted successfully else false.</returns>
        bool DeleteUrlRedirect(ParameterModel urlRedirectId);
    }
}
