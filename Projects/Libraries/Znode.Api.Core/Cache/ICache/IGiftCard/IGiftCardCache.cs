namespace Znode.Engine.Api.Cache
{
    public interface IGiftCardCache
    {
        /// <summary>
        /// Get GiftCard list.
        /// </summary>
        /// <param name="routeUri">route Uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns GiftCard list.</returns>
        string GetGiftCardList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get GiftCard on the basis of giftCardId.
        /// </summary>
        /// <param name="giftCardId">GiftCard id.</param>
        /// <param name="routeUri">route Uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns GiftCard.</returns>
        string GetGiftCard(int giftCardId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get voucher on the basis of voucher code.
        /// </summary>
        /// <param name="voucherCode">voucher code</param>
        /// <param name="routeUri">route Uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns voucher</returns>
        string GetVoucher(string voucherCode, string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of Gift Card history for a user.
        /// </summary>
        /// <param name="routeUri">Route Uri.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>Returns gift card history list for a user.</returns>
        string GetGiftCardHistoryList(string routeUri, string routeTemplate);
    }
}