namespace Znode.Engine.Admin.ViewModels
{
    public class RMAReturnTotalViewModel : BaseViewModel
    {
        public decimal SubTotal { get; set; }
        public decimal? ReturnShippingCost { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ReturnTaxCost { get; set; }
        public string CultureCode { get; set; }
        public decimal CSRDiscount { get; set; }
        public decimal ReturnShippingDiscount { get; set; }
        public decimal ReturnCharges { get; set; }
        public decimal TotalReturnAmount { get; set; }
        public decimal? VoucherAmount { get; set; }
        public decimal ReturnImportDuty { get; set; }
    }
}