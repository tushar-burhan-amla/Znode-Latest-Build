using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IPortalCountryService
    {
        #region Country Association
        /// <summary>
        /// Get list of unassociate countries.
        /// </summary>
        /// <param name="expands">expand for country.</param>
        /// <param name="filters">filter for country.</param>
        /// <param name="sorts">sorts for country.</param>
        /// <param name="pageIndex">pageindex</param>
        /// <param name="pageSize">pagesize</param>
        /// <returns>CategoryListModel</returns>
        CountryListModel GetUnAssociatedCountryList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// UnAssociate associated countries.
        /// </summary>
        /// <param name="portalCountries">portalCountries contains Ids to delete associated countries.</param>
        /// <returns>true or false response</returns>
        bool UnAssociateCountries(ParameterModelForPortalCountries portalCountries);

        /// <summary>
        /// Get associated country list.
        /// </summary>
        /// <param name="expands">expand</param>
        /// <param name="filters">filter</param>
        /// <param name="sorts">sorts</param>
        /// <param name="pageIndex">pageindex</param>
        /// <param name="pageSize">pagesize</param>
        /// <returns></returns>
        CountryListModel GetAssociatedCountryList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Associate countries.
        /// </summary>
        /// <param name="model">model with portalId and multiple country ids.</param>
        /// <returns>true or false response</returns>
        bool AssociateCountries(ParameterModelForPortalCountries model);
        #endregion
    }
}
