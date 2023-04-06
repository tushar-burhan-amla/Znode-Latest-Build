using System;
namespace Znode.Engine.Api.Models
{
    public class VoucherModel : BaseModel
    {
        public decimal VoucherBalance { get; set; }
        public string VoucherNumber { get; set; }
        public string VoucherMessage { get; set; }
        public bool IsVoucherValid { get; set; }
        public bool IsVoucherApplied { get; set; }

        //The voucher amount used in calculation and this would update in each complete return
        public decimal VoucherAmountUsed { get; set; }
        public string VoucherName { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string CultureCode { get; set; }
        public int? PortalId { get; set; }
        public bool IsExistInOrder { get; set; }
        public int? UserId { get; set; }
        public bool IsActive { get; set; }

        //The voucher amount while placing an order and this would not update in each complete return
        public decimal OrderVoucherAmount { get; set; }
    }
}