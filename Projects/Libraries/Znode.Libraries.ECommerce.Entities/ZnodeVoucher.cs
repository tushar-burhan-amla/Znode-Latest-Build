using System;

namespace Znode.Libraries.ECommerce.Entities
{
    /// <summary>
    /// Represents Voucher Information applied on shopping cart.
    /// </summary>
    public class ZnodeVoucher
    {
        public decimal VoucherBalance { get; set; }
        public string VoucherNumber { get; set; }
        public string VoucherMessage { get; set; }
        public bool IsVoucherValid { get; set; }
        public bool IsVoucherApplied { get; set; }
        public decimal VoucherAmountUsed { get; set; }
        public string VoucherName { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string CultureCode { get; set; }
        public int? PortalId { get; set; }
        public bool IsExistInOrder { get; set; }
        public int? UserId { get; set; }
        public decimal OrderVoucherAmount { get; set; }
    }
}
