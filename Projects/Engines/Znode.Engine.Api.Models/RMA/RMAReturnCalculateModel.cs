using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class RMAReturnCalculateModel : BaseModel
    {
        [Required]
        public List<RMAReturnCalculateLineItemModel> ReturnCalculateLineItemList { get; set; }
        [Required]
        public string OrderNumber { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int PortalId { get; set; }
        public decimal ReturnSubTotal { get; set; }
        public decimal ReturnTaxCost { get; set; }
        public decimal? ReturnShippingCost { get; set; }
        public string CultureCode { get; set; }
        public bool IsAdminRequest { get; set; }
        public decimal Discount { get; set; }
        public decimal CSRDiscount { get; set; }
        public decimal ReturnShippingDiscount { get; set; }
        public decimal ReturnCharges { get; set; }
        public decimal VoucherAmount { get; set; }
        public virtual decimal ReturnTotal => (ReturnSubTotal + ReturnShippingCost.GetValueOrDefault() + ReturnTaxCost + ReturnImportDuty - Discount - CSRDiscount - ReturnShippingDiscount - ReturnCharges);
        public int PaymentStatusId { get; set; }
        public decimal ReturnImportDuty { get; set; }
    }
}