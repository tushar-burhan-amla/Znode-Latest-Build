using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IPortalCountryClient : IBaseClient
    {
        #region Country Association.
        /// <summary>
        /// Get list of unassociated countries.
        /// </summary>
        /// <param name="expands">expand for country.</param>
        /// <param name="filters">filter for country.</param>
        /// <param name="sorts">sorts for country</param>
        /// <param name="pageIndex">pageindex.</param>
        /// <param name="pageSize">pagesize.</param>
        /// <returns>UnAssociated country list.</returns>
        CountryListModel GetUnAssociatedCountryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get associated country list based on portal.
        /// </summary>
        /// <param name="expands">expand for country.</param>
        /// <param name="filters">filter for country.</param>
        /// <param name="sorts">sorts for country.</param>
        /// <param name="pageIndex">pageindex.</param>
        /// <param name="pageSize">pagesize.</param>
        /// <returns>Associated country list.</returns>
        CountryListModel GetAssociatedCountryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Remove associated countries.
        /// </summary>
        /// <param name="portalCountries">portalCountryId contain ids to unassociate country from portal.</param>
        /// <returns>true/or false response.</returns>
        bool UnAssociateCountries(ParameterModelForPortalCountries portalCountries);

        /// <summary>
        /// Associate countries.
        /// </summary>
        /// <param name="parameterModel">model with countryIds and portalId.</param>
        /// <returns>true/false response</returns>
        bool AssociateCountries(ParameterModelForPortalCountries parameterModel);
        #endregion
    }
}
