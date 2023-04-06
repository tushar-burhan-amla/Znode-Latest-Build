namespace Znode.Engine.Admin.ViewModels
{
    public class AccountQuoteLineItemViewModel : BaseViewModel
    {
        public int OmsQuoteLineItemId { get; set; }
        public int Sequence { get; set; }
        public int OmsQuoteId { get; set; }
        public int ParentOmsQuoteLineItemItemId { get; set; }
        public int OrderLineItemRelationshipTypeId { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public string SKU { get; set; }
        public string CustomText { get; set; }
        public string CartAddOnDetails { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
    }
}