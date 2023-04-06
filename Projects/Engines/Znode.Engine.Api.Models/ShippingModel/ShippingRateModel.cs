namespace Znode.Engine.Api.Models
{
    public class ShippingRateModel : BaseModel
    {
        public decimal ShippingRate { get; set; }
        public string ApproximateArrival { get; set; }
    }
}
