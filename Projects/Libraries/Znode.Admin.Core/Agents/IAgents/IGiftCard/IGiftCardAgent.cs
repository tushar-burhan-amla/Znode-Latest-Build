using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IGiftCardAgent
    {
        /// <summary>
        /// Gets the list of GiftCard.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with GiftCard list.</param>
        /// <param name="filters">Filters to be applied on GiftCard list.</param>
        /// <param name="sorts">Sorting to be applied on GiftCard list.</param>
        /// <param name="pageIndex">Start page index of GiftCard list.</param>
        /// <param name="pageSize">Page size of GiftCard list.</param>
        /// <param name="excludeExpired">excludeExpired</param>
        /// <param name="userId">User Id</param>
        /// <returns>Returns GiftCard list.</returns>
        GiftCardListViewModel GetGiftCardList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, bool isExcludeExpired, int userId = 0);

        /// <summary>
        /// Create GiftCard.
        /// </summary>
        /// <param name="giftCardViewModel">GiftCard View Model.</param>
        /// <returns>Returns created model.</returns>
        GiftCardViewModel Create(GiftCardViewModel giftCardViewModel);

        /// <summary>
        /// Get GiftCard list by GiftCard id.
        /// </summary>
        /// <param name="giftCardId">GiftCard list Id</param>
        /// <returns>Returns GiftCardViewModel.</returns>
        GiftCardViewModel GetGiftCard(int giftCardId);

        /// <summary>
        /// Update GiftCard.
        /// </summary>
        /// <param name="giftCardViewModel">GiftCard view model to update.</param>
        /// <returns>Returns updated GiftCard model.</returns>
        GiftCardViewModel Update(GiftCardViewModel giftCardViewModel);

        /// <summary>
        /// Delete GiftCard.
        /// </summary>
        /// <param name="giftCardId">GiftCard Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteGiftCard(string giftCardId);

        /// <summary>
        /// Get GiftCard random number.
        /// </summary>
        /// <returns>Returns gift card random number.</returns>
        string GetRandomGiftCardNumber();

        /// <summary>
        /// Get active currency name.
        /// </summary>
        /// <returns>currency name</returns>
        CurrencyViewModel GetActiveCurrency(int portalId);

        /// <summary>
        /// Check whether entered Customer id already exists or not.
        /// </summary>
        /// <param name="userId">userId to check.</param>
        /// <param name="portalId">portalId.</param>
        /// <returns>return the status in true or false</returns>
        bool CheckIsUserIdExist(int userId, int portalId = 0);

        /// <summary>
        /// Update RMA request for gift card
        /// </summary>
        /// <param name="giftCardViewModel"></param>
        /// <param name="message"></param>
        /// <returns>true / false</returns>
        bool UpdateRMA(GiftCardViewModel giftCardViewModel, out string message);

        /// <summary>
        /// Set RMA data.
        /// </summary>
        /// <param name="rmaRequestViewModel">RMARequestViewModel</param>
        /// <returns>Return GiftCardViewModel</returns>
        GiftCardViewModel SetRMAData(RMARequestViewModel rmaRequestViewModel);

        /// <summary>
        /// Get currency details by code.
        /// </summary>
        /// <param name="currencyCode">Currency code</param>
        /// <returns>CurrencyViewModel</returns>
        CurrencyViewModel GetCurrencyDetailsByCode(string currencyCode);

        /// <summary>
        /// Activate Deactivate vouchers.
        /// </summary>
        /// <param name="voucherIds">voucherIds</param>
        /// <param name="isActive">Is Active flag</param>
        /// <returns>true/ False</returns>
        bool ActivateDeactivateVouchers(string voucherIds, bool isActive);

        /// <summary>
        /// Get Voucher history list.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Voucher list.</param>
        /// <param name="filters">Filters to be applied on Voucher list.</param>
        /// <param name="sorts">Sorting to be applied on Voucher list.</param>
        /// <param name="pageIndex">Start page index of Voucher list.</param>
        /// <param name="pageSize">Page size of Voucher list.</param>
        /// <param name="voucherId">Voucher Id</param>
        /// <param name="portalId">Portal Id</param>
        /// <returns>VoucherHistoryListViewModel model</returns>
        VoucherHistoryListViewModel GetVoucherHistoryList(ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int pageIndex, int pageSize, int voucherId, int portalId);
    }
}
