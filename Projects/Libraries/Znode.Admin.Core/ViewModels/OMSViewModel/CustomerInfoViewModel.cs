namespace Znode.Engine.Admin.ViewModels
{
    public class CustomerInfoViewModel
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public string UserName { get; set; }
        public string CustomerName { get; set; }
        public string AccountName { get; set; }
        public string OrderStatus { get; set; }
        public string PhoneNumber { get; set; }
        public AddressViewModel ShippingAddress { get; set; }
        public AddressViewModel BillingAddress { get; set; }
        public OrderTotalViewModel orderTotal { get; set; }
        public int OmsOrderId { get; set; }
        public int OmsQuoteId { get; set; }
        public string Email { get; set; }
        public string CustomerGUID { get; set; }
 
    }
}
