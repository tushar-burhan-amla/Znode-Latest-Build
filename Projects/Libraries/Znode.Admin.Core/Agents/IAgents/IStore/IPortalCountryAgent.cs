using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Agents
{
    public interface IPortalCountryAgent
    {
        #region Country Association
        /// <summary>
        /// Get list of associated and unassociate countries on the basis of flag.
        /// </summary>
        /// <param name="model">FilterCollectionDataModel parameters related to grid.</param>
        /// <param name="portalId">portalId to get list of associated countries.</param>
        /// <param name="isAssociated">flag on the basis of which we get country associated or unassociated list.</param>
        /// <returns>return true or false</returns>
        CountryListViewModel GetAssociatedOrUnAssociatedCountryList(FilterCollectionDataModel model, int portalId, bool isAssociatedList);

        /// <summary>
        ///Remove associated countries.
        /// </summary>
        /// <param name="portalCountryId">portalCountry ids to unassociate from portal.</param>
        /// <param name="portalId">portalId to which countries are associated.</param>
        /// <param name="message">to set error message.</param>
        /// <returns>return true or false</returns>
        bool UnAssociateCountries(string portalCountryId,int portalId, out string message);

        /// <summary>
        ///Associate countries.
        /// </summary>
        /// <param name="portalId">portalId to which countries are associated.</param>
        /// <param name="countryCode"> multiple country codes to associate.</param>
        /// <param name="isDefault">value of country is default or not.</param>
        /// <param name="portalCountryId">portal country id.</param>
        /// <returns></returns>
        bool AssociateCountries(int portalId, string countryCode, bool isDefault, int portalCountryId);
        #endregion
    }
}
