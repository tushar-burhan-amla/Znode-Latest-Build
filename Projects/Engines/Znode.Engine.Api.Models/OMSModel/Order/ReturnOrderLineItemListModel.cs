using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ReturnOrderLineItemListModel : BaseListModel
    {
        public List<ReturnOrderLineItemModel> ReturnItemList { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxCost { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal CSRDiscount { get; set; }
        public decimal ShippingDiscount { get; set; }
        public decimal ReturnCharges { get; set; }
        public virtual decimal Total => (SubTotal + ShippingCost + TaxCost + ImportDuty.GetValueOrDefault() - DiscountAmount - CSRDiscount - ShippingDiscount - ReturnCharges);
        public decimal VoucherAmount { get; set; }
        public decimal? ImportDuty { get; set; }
    }
}
