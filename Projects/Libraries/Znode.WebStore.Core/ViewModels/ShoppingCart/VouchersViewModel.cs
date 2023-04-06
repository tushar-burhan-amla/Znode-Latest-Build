namespace Znode.Engine.WebStore.ViewModels
{
    public  class VouchersViewModel :BaseViewModel
    {
        public decimal VoucherBalance { get; set; }
        public string VoucherNumber { get; set; }
        public string VoucherMessage { get; set; }
        public bool IsVoucherValid { get; set; }
        public bool IsVoucherApplied { get; set; }
        public string VoucherAmountUsed { get; set; }
        public string VoucherName { get; set; }
        public string ExpirationDate { get; set; }
        public string CultureCode { get; set; }
        public string OrderVoucherAmount { get; set; }
    }
}
