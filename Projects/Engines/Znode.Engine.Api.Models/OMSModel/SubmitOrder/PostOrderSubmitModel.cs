using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PostOrderSubmitModel
    {
        public decimal GiftCardAmount { get; set; }
        public List<VoucherModel> Vouchers { get; set; }
        public List<CouponModel> Coupons { get; set; }
        public int CookieMappingId { get; set; }
        public bool IsCalculateVoucher { get; set; }
        public int OrderID { get; set; }
        public bool IsGuest { get; set; }
        public bool IsReferralCommission { get; set; }
        public decimal CommissionAmount { get; set; }
        public int BillingAddressId { get; set; }
        public bool SetBillingShippingFlags { get; set; }
        public List<TaxSummaryModel> TaxSummaryList { get; set; }

    }
}
