namespace Znode.Engine.Admin.ViewModels
{
    public class CurrencyViewModel : BaseViewModel
    {
        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencySuffix { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public string Symbol { get; set; }
        public int CultureId { get; set; }
        public string CultureCode { get; set; }
        public string CultureName { get; set; }
    }
}