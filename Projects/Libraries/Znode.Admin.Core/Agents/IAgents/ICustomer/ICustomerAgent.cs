using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;
using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.Agents
{
    public interface ICustomerAgent
    {
        #region Profile Association
        /// <summary>
        /// Get associated profiles based on customers.
        /// </summary>
        /// <param name="model">FilterCollectionDataModel</param>
        /// <param name="userId">userId to get associated profiles associated to this customer.</param>
        /// <returns>list model of profile.</returns>
        ProfileListViewModel GetAssociatedProfileList(FilterCollectionDataModel model, int userId);

        /// <summary>
        /// Get unassociated profiles.
        /// </summary>
        /// <param name="model">FilterCollectionDataModel</param>
        /// <param name="userId">userId to get unassociated profiles to customer.</param>
        /// <returns>list model of profile</returns>
        ProfileListViewModel GetUnAssociatedProfileList(FilterCollectionDataModel model, int userId);

        /// <summary>
        ///Remove associated profiles by profile Id.
        /// </summary>
        /// <param name="profileIds">profileIds.</param>
        /// <returns>return true or false</returns>
        bool UnAssociateProfiles(string profileIds, int userId, out string errorMessage);

        /// <summary>
        ///Associate profiles.
        /// </summary>
        /// <param name="profileIds">Multiple profile ids</param>
        /// <param name="userId">user id.</param>
        /// <returns>return true or false.</returns>
        bool AssociateProfiles(string profileIds, int userId);
        #endregion

        #region Customer Note
        /// <summary>
        /// Add customer note.
        /// </summary>
        /// <param name="noteViewModel">NoteViewModel to create customer note.</param>
        /// <returns>Added customer note.</returns>
        NoteViewModel CreateCustomerNote(NoteViewModel noteViewModel);

        /// <summary>
        /// Get customer note list from database.
        /// </summary>
        /// <param name="userId">Get customer note on the basis of userId.</param>
        /// <param name="filters">Filters collection.</param>
        /// <param name="sorts">Sort collection.</param>
        /// <param name="page">Page Number.</param>
        /// <returns>Returns NoteListViewModel</returns>
        NoteListViewModel GetCustomerNotes(int userId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? recordPerPage);

        /// <summary>
        /// Get customer note by noteId.
        /// </summary>
        /// <param name="noteId">Get customer note on the basis of noteId.</param>
        /// <returns>Returns NoteViewModel.</returns>
        NoteViewModel GetCustomerNote(int noteId);

        /// <summary>
        /// Update customer note.
        /// </summary>
        /// <param name="noteViewModel">NoteViewModel.</param>
        /// <returns>Returns NoteViewModel.</returns>
        NoteViewModel UpdateCustomerNote(NoteViewModel noteViewModel);

        /// <summary>
        /// Delete Customer note on the basis of noteIds.
        /// </summary>
        /// <param name="noteIds">note ids to delete customer note.</param>
        /// <returns>Returns True/False.</returns>
        bool DeleteCustomerNote(string noteIds);
        #endregion

        #region Address
        /// <summary>
        /// Get the address list.
        /// </summary>
        /// <param name="userId">Customer id to get the list.</param>
        /// <param name="sortCollection">Sort.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="recordPerPage">Record per page.</param>
        /// <returns>Returns address list.</returns>
        AddressListViewModel GetAddressList(int userId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get the address list.
        /// </summary>
        /// <param name="model">Filter collection data model.</param>
        /// <param name="userId">User id to get the address.</param>
        /// <returns>Returns address list.</returns>
        AddressListViewModel GetAddressList(FilterCollectionDataModel model, int userId);

        /// <summary>
        /// Create customer address.
        /// </summary>
        /// <param name="addressViewModel">Model to create.</param>
        /// <returns>Returns created model.</returns>
        AddressViewModel CreateCustomerAddress(AddressViewModel addressViewModel);

        /// <summary>
        /// Get customer address. 
        /// </summary>
        /// <param name="userAddressId">Customer address id to get.</param>
        /// <param name="accountAddressId">to get address of administrator user.</param>
        /// <param name="userId">userId</param>
        /// <returns>Returns address model.</returns>
        AddressViewModel GetCustomerAddress(int userAddressId, int accountAddressId, int userId);

        /// <summary>
        /// Update customer address.
        /// </summary>
        /// <param name="addressViewModel">Model to update in db.</param>
        /// <returns>Updated model.</returns>
        AddressViewModel UpdateCustomerAddress(AddressViewModel addressViewModel);

        /// <summary>
        /// Delete customer address.
        /// </summary>
        /// <param name="userAddressId">Customer Address Id to delete</param>
        /// <param name="accountAddressId">Account Address Id to delete record of administrator user.</param>
        /// <param name="errorMessage">Error message to display.</param>
        /// <returns>Returns true if deleted successfully.</returns>
        bool DeleteCustomerAddress(string userAddressId, string accountAddressId, out string errorMessage);
        #endregion

        #region Associate Price
        /// <summary>
        /// Get list for Price for Customer.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Price list.</param>
        /// <param name="filters">Filters to be applied on Price list.</param>
        /// <param name="sorts">Sorting to be applied on Price list.</param>
        /// <param name="pageIndex">Start page index of Price list.</param>
        /// <param name="pageSize">Page size of Price list.</param>
        /// <param name="accountId">accountId to get price list associated to this account.</param>
        /// <returns>Returns Price list.</returns>
        PriceListViewModel GetPriceList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int accountId);

        /// <summary>
        /// Get unassociated price list for Customer.
        /// </summary>
        /// <param name="expands">Expands for Price List.</param>
        /// <param name="filters">Filters for Price List.</param>
        /// <param name="sorts">Sorts for Price List.</param>
        /// <param name="pageIndex">Page Index.</param>
        /// <param name="recordPerPage">Size of page.</param>
        /// <returns>Returns unassociated price list.</returns>
        PriceListViewModel GetUnAssociatedPriceList(int accountId, ExpandCollection expands = null, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Associate Price List to Customer.
        /// </summary>
        /// <param name="userId">User Id to which pricelist to be associated.</param>
        /// <param name="priceListIds">priceListIds to be associated.</param>
        /// <returns>Returns true if pricelist associated successfully else return false.</returns>
        bool AssociatePriceList(int userId, string priceListId);

        /// <summary>
        /// Remove associated price list from Customer.
        /// </summary>
        /// <param name="priceListIds">price list ids associate to customer.</param>
        /// <param name="userId">User Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool UnAssociatePriceList(string priceListIds, int userId, out string message);

        /// <summary>
        /// Get associated price list precedence value for Customer.
        /// </summary>
        /// <param name="priceListId">priceListId.</param>
        /// <param name="userId">userId to get precedence for Customer.</param>
        /// <param name="listName">price list name.</param>
        /// <returns>Returns PriceUserViewModel.</returns>
        PriceUserViewModel GetAssociatedPriceListPrecedence(int priceListId, int userId, string listName);

        /// <summary>
        /// Update associated price list precedence value for Customer.
        /// </summary>
        /// <param name="priceUserViewModel">PriceUserViewModel model.</param>
        /// <returns>Returns true/false status.</returns>
        bool UpdateAssociatedPriceListPrecedence(PriceUserViewModel priceUserViewModel);
        #endregion

        /// <summary>
        /// Get the customer affiliate data.
        /// </summary>
        /// <param name="userId">User Id to get the customer data.</param>
        /// <returns>Returns customer affiliate data.</returns>
        CustomerAffiliateViewModel GetCustomerAffiliateData(int userId);

        /// <summary>
        /// Gets the list of webstore url on portal id.
        /// </summary>
        /// <param name="portalId">Portal id.</param>
        /// <param name="userId">User id.</param>
        /// <returns>Returns the list of webstore url on portal id.</returns>
        List<SelectListItem> GetDomains(int portalId, int userId);

        /// <summary>
        /// Save the affiliate data.
        /// </summary>
        /// <param name="viewModel">Customer affiliate view model to save in database.</param>
        /// <returns>Returns customer affiliate view model.</returns>
        CustomerAffiliateViewModel SaveAffiliateData(CustomerAffiliateViewModel viewModel);

        /// <summary>
        /// Create tab structure.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        TabViewListModel CreateTabStructure(int userId);

        /// <summary>
        /// Get Referral Commission List.
        /// </summary>
        /// <param name="model">FilterCollectionDataModel</param>
        /// <param name="userId">userId to get referral commission.</param>
        /// <returns>list model of Referral Commission.</returns>
        ReferralCommissionListViewModel GetReferralCommissionList(FilterCollectionDataModel model, int userId);

        /// <summary>
        /// Get referral commission payment list.
        /// </summary>
        /// <param name="model">FilterCollectionDataModel</param>
        /// <param name="userId">To get referral commission payment list of particular user.</param>
        /// <returns>GiftCardListViewModel</returns>
        GiftCardListViewModel GetPaymentList(FilterCollectionDataModel model, int userId);

        /// <summary>
        /// Get the data required to create referral commission payment.
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>GiftCardViewModel</returns>
        GiftCardViewModel GetDataToCreatePayment(int userId);

        /// <summary>
        /// Set default profile for customer. 
        /// </summary>
        /// <param name="userId">Id of user.</param>
        /// <param name="profileId">Id of profile.</param>
        /// <param name="errorMessage">error/success message.</param>
        /// <returns>true/false response.</returns>
        bool SetDefaultProfile(int userId, string profileId, out string errorMessage);

        /// <summary>
        /// Get customer associated order list.
        /// </summary>
        /// <param name="filters">FilterCollection</param>
        /// <param name="sortCollection">SortCollection</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="recordPerPage">record per page.</param>
        /// <param name="userId">user id</param>
        /// <returns>OrdersListViewModel</returns>
        OrdersListViewModel GetCustomerOrderList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null, int userId = 0);

        #region Impersonation 
        /// <summary>
        /// Get details for customer like list of portals
        /// </summary>
        /// <param name="userId"> Shopper Id</param>
        /// <returns name="ImpersonationViewModel"> Return model with list of portal assign to user</returns>
        ImpersonationViewModel GetImpersonationByUserId(int userId);
        #endregion
    }
}
