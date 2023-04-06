using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IGiftCardService
    {
        /// <summary>
        /// Create GiftCard.
        /// </summary>
        /// <param name="giftCardModel">GiftCard Model</param>
        /// <returns>returns GiftCardModel</returns>
        GiftCardModel CreateGiftCard(GiftCardModel giftCardModel);

        /// <summary>
        /// Update GiftCard.
        /// </summary>
        /// <param name="giftCardModel">GiftCard Model</param>
        /// <returns>return status</returns>
        bool UpdateGiftCard(GiftCardModel giftCardModel);

        /// <summary>
        /// Get GiftCard by GiftCardId
        /// </summary>
        /// <param name="giftCardId">Id of GiftCard</param>
        /// <returns>returns GiftCardModel </returns>
        GiftCardModel GetGiftCard(int giftCardId);

        /// <summary>
        /// Get voucher by voucher Code
        /// </summary>
        /// <param name="voucherCode">voucher Code</param>
        /// <returns>returns GiftCardModel</returns>
        GiftCardModel GetVoucher(string voucherCode);

        /// <summary>
        /// Get paged GiftCard list.
        /// </summary>
        /// <param name="expands">expand collection list </param>
        /// <param name="filters">filter list</param>
        /// <param name="sorts">sort list</param>
        /// <param name="page">paging parameters </param>
        /// <returns>return GiftCardListModel </returns>
        GiftCardListModel GetGiftCardList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Delete GiftCard by GiftCardId.
        /// </summary>
        /// <param name="giftCardId">Id of GiftCard</param>
        /// <returns>return status</returns>
        bool DeleteGiftCard(ParameterModel giftCardId);

        /// <summary>
        /// Delete voucher by voucher Code.
        /// </summary>
        /// <param name="voucherCodes">voucher Codes</param>
        /// <returns>return status</returns>
        bool DeleteVoucher(ParameterModel voucherCodes);

        /// <summary>
        /// Get Random GiftCard Number.
        /// </summary>
        /// <returns>return gift card random number.</returns>
        string GetRandomCardNumber();

        /// <summary>
        /// Get gift card history for a user.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter Collection.</param>
        /// <param name="sorts">Sorts.</param>
        /// <param name="page">Page Size.</param>
        /// <returns>Returns gift card history list for a user.</returns>
        GiftCardHistoryListModel GetGiftCardHistoryList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Activate Deactivate Vouchers
        /// </summary>
        /// <param name="voucherId">Voucher Id</param>
        /// <param name="isActive">IsActive</param>
        /// <returns>true/false</returns>
        bool ActivateDeactivateVouchers(ParameterModel voucherId, bool isActive);
        /// <summary>
        /// Send voucher expiration reminder email.
        /// </summary>
        /// <returns></returns>
        bool SendVoucherExpirationReminderEmail();

        /// <summary>
        /// Activate Deactivate Vouchers
        /// </summary>
        /// <param name="voucherCodes">voucher Codes</param>
        /// <param name="isActive">IsActive</param>
        /// <returns>true/false</returns>
        bool ActivateDeactivateVouchersByVoucherCode(ParameterModel voucherCodes, bool isActive);
    }
}
