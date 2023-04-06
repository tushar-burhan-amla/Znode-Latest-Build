namespace Znode.Engine.Api.Models
{
    public class ProductAdditionalInfoModel : BaseModel
    {
        public decimal QuantityOnHand { get; set; }
        public decimal ReOrderLevel { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal SalesPrice { get; set; }
    }
}
