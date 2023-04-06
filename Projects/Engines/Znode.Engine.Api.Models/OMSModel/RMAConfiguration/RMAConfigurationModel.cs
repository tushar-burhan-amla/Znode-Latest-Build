namespace Znode.Engine.Api.Models
{
    public class RMAConfigurationModel : BaseModel
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string ShippingDirections { get; set; }
        public string GcNotification { get; set; }
        public int RmaConfigurationId { get; set; }
        public int? MaxDays { get; set; } = 7;
        public int? GcExpirationPeriod { get; set; }
        public bool IsEmailNotification { get; set; }
    }
}
