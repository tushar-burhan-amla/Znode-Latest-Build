namespace Znode.Engine.Api.Models
{
    public class TaxAmountOverrideModel : BaseModel
    {
        public int OmsOrderLineItemsId { get; set; }
        public string SKU { get; set; }
        public decimal TaxOverrideAmount { get; set; }
    }
}
