using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class ReturnOrderLineItemListViewModel : BaseViewModel
    {
        public List<ReturnOrderLineItemViewModel> ReturnItemList { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxCost { get; set; }
        public decimal ShippingCost { get; set; }
        public string CurrencyCode { get; set; }
        public string CultureCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal CSRDiscount { get; set; }
        public decimal ShippingDiscount { get; set; }
        public decimal ReturnCharges { get; set; }
        public decimal Total { get; set; }
        public decimal ReturnVoucherAmount { get; set; }

        public decimal? ImportDuty { get; set; }
    }
}
