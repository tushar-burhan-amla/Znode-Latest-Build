using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IUrlRedirectAgent
    {
        /// <summary>
        /// Get Url Redirect list.
        /// </summary>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sorts.</param>
        /// <param name="pageIndex">Page Index.</param>
        /// <param name="pageSize">Page Size.</param>
        /// <returns>Returns Url Redirect list.</returns>
        UrlRedirectListViewModel GetUrlRedirectList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize,int portalId);

        /// <summary>
        /// Get Url Redirect.
        /// </summary>
        /// <param name="urlRedirectId">Id of the Url Redirect.</param>
        /// <returns>Returns Url Redirect.</returns>
        UrlRedirectViewModel GetUrlRedirect(int urlRedirectId);

        /// <summary>
        /// Create Url Redirect.
        /// </summary>
        /// <param name="UrlRedirectViewModel">model to create.</param>
        /// <returns>Returns created model.</returns>
        UrlRedirectViewModel CreateUrlRedirect(UrlRedirectViewModel UrlRedirectViewModel);

        /// <summary>
        /// Update Url Redirect.
        /// </summary>
        /// <param name="UrlRedirectViewModel">Model to update.</param>
        /// <returns>Updated model.</returns>
        UrlRedirectViewModel UpdateUrlRedirect(UrlRedirectViewModel UrlRedirectViewModel);

        /// <summary>
        /// Delete Url Redirect.
        /// </summary>
        /// <param name="urlRedirectId">Ids to delete.</param>
        /// <returns>Returns true if deleted successfully else false.</returns>
        bool DeleteUrlRedirect(string urlRedirectId);       
    }
}
