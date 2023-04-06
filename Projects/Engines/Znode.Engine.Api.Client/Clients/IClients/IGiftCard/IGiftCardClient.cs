using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IGiftCardClient : IBaseClient
    {
        /// <summary>
        /// Gets the list of GiftCard.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with GiftCard list.</param>
        /// <param name="filters">Filters to be applied on GiftCard list.</param>
        /// <param name="sorts">Sorting to be applied on GiftCard list.</param>
        /// <returns>Returns GiftCard list.</returns>
        GiftCardListModel GetGiftCardList(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Get Random Gift Card Number.
        /// </summary>
        /// <returns>Returns gift card random number.</returns>
        string GetRandomGiftCardNumber();

        /// <summary>
        /// Gets the list of GiftCard.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with GiftCard list.</param>
        /// <param name="filters">Filters to be applied on GiftCard list.</param>
        /// <param name="sorts">Sorting to be applied on GiftCard list.</param>
        /// <param name="pageIndex">Start page index of GiftCard list.</param>
        /// <param name="pageSize">Page size of GiftCard list.</param>
        /// <returns>Returns GiftCard list.</returns>
        GiftCardListModel GetGiftCardList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create GiftCard.
        /// </summary>
        /// <param name="giftCardModel">GiftCard Model.</param>
        /// <returns>Returns created GiftCard Model.</returns>
        GiftCardModel CreateGiftCard(GiftCardModel giftCardModel);

        /// <summary>
        /// Get GiftCard on the basis of GiftCard id.
        /// </summary>
        /// <param name="giftCardId">GiftCardId to get GiftCard details.</param>
        /// <returns>Returns GiftCardModel.</returns>
        GiftCardModel GetGiftCard(int giftCardId);

        /// <summary>
        /// Update GiftCard data.
        /// </summary>
        /// <param name="giftCardModel">GiftCard model to update.</param>
        /// <returns>Returns updated GiftCard model.</returns>
        GiftCardModel UpdateGiftCard(GiftCardModel giftCardModel);

        /// <summary>
        /// Delete GiftCard.
        /// </summary>
        /// <param name="giftCardId">GiftCard Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteGiftCard(ParameterModel giftCardId);

        /// <summary>
        /// Get gift card history for a user.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with GiftCard list.</param>
        /// <param name="filters">Filters to be applied on GiftCardHistory list.</param>
        /// <param name="sorts">Sorting to be applied on GiftCardHistory list.</param>
        /// <param name="pageIndex">Start page index of GiftCardHistory list.</param>
        /// <param name="pageSize">Page size of GiftCardHistory list.</param>
        /// <returns>Returns gift card history list.</returns>
        GiftCardHistoryListModel GetGiftCardHistoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Activate deactivate vouchers.
        /// </summary>
        /// <param name="voucherIds"> voucherIds</param>
        /// <param name="isActive">isActive</param>
        /// <returns>true/false</returns>
        bool ActivateDeactivateVouchers(ParameterModel voucherIds, bool isActive);

        /// <summary>
        /// Get Vouchers List
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with GiftCard list.</param>
        /// <param name="filters">Filters to be applied on GiftCardHistory list.</param>
        /// <param name="sorts">Sorting to be applied on GiftCardHistory list.</param>
        /// <param name="pageIndex">Start page index of GiftCardHistory list.</param>
        /// <param name="pageSize">Page size of GiftCardHistory list.</param>
        /// <returns>Returns vouchers list.</returns>
        GiftCardListModel GetVouchers(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

    }
}
