namespace Znode.Engine.WebStore.ViewModels
{
    public class ShippingOptionViewModel : BaseViewModel
    {
        public int ShippingId { get; set; }
        public int? ProfileId { get; set; }
        public string ShippingCode { get; set; }
        public string DestinationCountryCode { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsSelected { get; set; }
        public string StateCode { get; set; }
        public decimal? ShippingRate { get; set; }
        public decimal? ShippingRateWithoutDiscount { get; set; }
        public string FormattedShippingRate { get; set; }
        public string FormattedShippingRateWithoutDiscount { get; set; }
        public string ApproximateArrival { get; set; }
        public string ShippingTypeName { get; set; }
        public string ClassName { get; set; }
        public string DeliveryTimeframe { get; set; }
        public bool IsExpedited { get; set; }
        public string ShippingName { get; set; }
        public string EstimateDate { get; set; }
        public decimal HandlingCharge { get; set; }
    }
}