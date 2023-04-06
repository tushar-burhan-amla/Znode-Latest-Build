namespace Znode.Engine.Admin.ViewModels
{
    public class ReviewOrderViewModel : BaseViewModel
    {
        public AddressViewModel ShippingAddress { get; set; }

        public CartViewModel ShoppingCart { get; set; }

        public ShippingViewModel ShippingOption { get; set; }

        public ShippingListViewModel ShippingOptionList { set; get; }

        public AddressViewModel BillingAddress { get; set; }

        public string PaymentType { get; set; }
    }
}