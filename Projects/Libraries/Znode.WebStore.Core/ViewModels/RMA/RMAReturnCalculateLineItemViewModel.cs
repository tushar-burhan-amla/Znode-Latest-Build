namespace Znode.Engine.WebStore.ViewModels
{
    public class RMAReturnCalculateLineItemViewModel
    {
        public int OrderId { get; set; }
        public string Guid { get; set; }
        public decimal ShippedQuantity { get; set; }
        public decimal ExpectedReturnQuantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalLineItemPrice { get; set; }
        public string TotalLineItemPriceWithCurrency { get; set; }
        public int ReasonForReturnId { get; set; }
        public string ReasonForReturn { get; set; }
        public int ProductId { get; set; }
        public int OmsOrderLineItemsId { get; set; }
        public string OrderNumber { get; set; }
        public string ErrorMessage { get; set; }
        public string CultureCode { get; set; }
    }
}
