
namespace Znode.Engine.Admin.ViewModels
{
    public class QuoteLineItemHistoryModel : BaseViewModel
    {
        public bool IsUpdateQuantity { get; set; }
        public bool IsUpdateUnitPrice { get; set; }
        public bool IsDeleteLineItem { get; set; }
        public string ProductName { get; set; }
        public string Quantity { get; set; }
        public string UnitPrice { get; set; }
        public string SKU { get; set; }
        public int OmsQuoteLineItemsId { get; set; }
        public decimal SubTotal { get; set; }
        public string ShippingCost { get; set; }
        public bool IsUpdateLineItemShippingPrice { get; set; }
    }
}
