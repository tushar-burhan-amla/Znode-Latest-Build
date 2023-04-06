
namespace Znode.Engine.Admin.ViewModels
{
    public class ManageAddressModel : BaseViewModel
    {
        public int OmsQuoteId { get; set; }
        public int SelectedAddressId { get; set; }
        public int BillingAddressId { get; set; }
        public int ShippingAddressId { get; set; }
        public int UserId { get; set; }      
        public int PortalId { get; set; }
        public string FromBillingShipping { get; set; }
    }
}
