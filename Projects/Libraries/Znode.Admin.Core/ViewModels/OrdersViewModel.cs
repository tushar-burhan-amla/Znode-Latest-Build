namespace Znode.Engine.Admin.ViewModels
{
    public class OrdersViewModel : BaseViewModel
    {
        public int OrderId { get; set; }
        public string OrderDate { get; set; }
        public decimal OrderTotal { get; set; }
        public string ProductName { get; set; }
        public string PaymentMethod { get; set; }
        public int? AccountId { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Shipping { get; set; }
        public decimal GiftCardAmount { get; set; }
        public string ReceiptHtml { get; set; }
        public bool IsEmailSend { get; set; }
        //public bool HasMultipleItems
        //{
        //    get { return OrderLineItems.Count > 1; }
        //}
        //public int OtherItemCount
        //{
        //    get { return OrderLineItems.Count - 1; }
        //}

        //public OrderLineItemViewModel MaxOrderLineItem
        //{
        //    get
        //    {
        //        if (OrderLineItems.Count == 1)
        //        {
        //            return OrderLineItems.First();
        //        }
        //        else
        //        {
        //            return OrderLineItems.FirstOrDefault(x => x.SubTotal == OrderLineItems.Max(y => y.SubTotal));
        //        }
        //    }
        //}


        //  public Collection<OrderLineItemViewModel> OrderLineItems { get; set; }
        public AddressViewModel BillingAddress { get; set; }
        public AddressViewModel ShippingAddress { get; set; }

    }
}