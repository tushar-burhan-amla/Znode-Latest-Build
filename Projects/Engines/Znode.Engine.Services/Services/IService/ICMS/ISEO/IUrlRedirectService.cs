using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IUrlRedirectService
    {
        /// <summary>
        /// Get Url Redirect list.
        /// </summary>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sorts.</param>
        /// <param name="page">Page.</param>
        /// <returns>Returns Url Redirect list.</returns>
        UrlRedirectListModel GetUrlRedirectList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get Url Redirect.
        /// </summary>
        /// <param name="filters">Filters.</param>
        /// <returns>Returns Url Redirect.</returns>
        UrlRedirectModel GetUrlRedirect(FilterCollection filters);

        /// <summary>
        /// Create Url Redirect.
        /// </summary>
        /// <param name="urlRedirectModel">model to create.</param>
        /// <returns>Returns created model.</returns>
        UrlRedirectModel Create(UrlRedirectModel urlRedirectModel);

        /// <summary>
        /// Update Url Redirect.
        /// </summary>
        /// <param name="model">Model to update.</param>
        /// <returns>Returns true if updated successfully.</returns>
        bool Update(UrlRedirectModel urlRedirectModel);

        /// <summary>
        /// Delete Url Redirect.
        /// </summary>
        /// <param name="urlRedirectIds">Ids to delete.</param>
        /// <returns>Returns true if deleted successfully else false.</returns>
        bool Delete(ParameterModel urlRedirectIds);
    }
}
