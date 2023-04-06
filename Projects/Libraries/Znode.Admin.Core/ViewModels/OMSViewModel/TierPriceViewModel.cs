namespace Znode.Engine.Admin.ViewModels
{
    public class TierPriceViewModel : BaseViewModel
    {
        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? MinQuantity { get; set; }
        public decimal? MaxQuantity { get; set; }
    }
}