namespace Znode.Engine.Admin.ViewModels
{
    public class PortalProfileShippingViewModel : BaseViewModel
    {
        public int ProfileId { get; set; }
        public int ProfileShippingId { get; set; }
        public int PortalShippingId { get; set; }
        public int PortalId { get; set; }
        public string ShippingIds { get; set; }
    }
}