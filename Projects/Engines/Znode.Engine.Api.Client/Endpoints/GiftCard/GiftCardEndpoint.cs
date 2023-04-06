namespace Znode.Engine.Api.Client.Endpoints
{
    public class GiftCardEndpoint : BaseEndpoint
    {
        //Get GiftCard list Endpoint.
        public static string GetGiftCardList() => $"{ApiRoot}/giftcard/list";

        //Get RandomGiftCardNumber Endpoint.
        public static string GetRandomGiftCardNumber() => $"{ApiRoot}/giftcard/getrandomgiftcardnumber";

        //Create GiftCard Endpoint.
        public static string CreateGiftCard() => $"{ApiRoot}/giftcard/create";

        //Get GiftCard on the basis of GiftCard id Endpoint.
        public static string GetGiftCard(int giftCardId) => $"{ApiRoot}/giftcard/{giftCardId}";

        //Get GiftCard on the basis of GiftCard id Endpoint.
        public static string GetVoucher(string voucherCode) => $"{ApiRoot}/giftcard/getvoucher/{voucherCode}";

        //Update GiftCard Endpoint.
        public static string UpdateGiftCard() => $"{ApiRoot}/giftcard/update";

        //Delete GiftCard Endpoint.
        public static string DeleteGiftCard() => $"{ApiRoot}/giftcard/delete";

        //Delete Voucher Endpoint.
        public static string DeleteVoucher() => $"{ApiRoot}/giftcard/deletevoucher";

        //Get gift card history for a user.
        public static string GetGiftCardHistoryList() => $"{ApiRoot}/giftcard/getgiftcardhistorylist";

        //Activate Deactivate user vouchers.
        public static string ActivateDeactivateVouchers(bool isActive) => $"{ApiRoot}/giftcard/activatedeactivatevouchers/{isActive}";

        //Activate Deactivate user vouchers by voucher code.
        public static string ActivateDeactivateVouchersByVoucherCode(bool isActive) => $"{ApiRoot}/giftcard/activatedeactivatevouchersbyvouchercode/{isActive}";

        

    }
}
