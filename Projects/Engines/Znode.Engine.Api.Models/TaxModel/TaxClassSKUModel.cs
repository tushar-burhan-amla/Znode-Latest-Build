namespace Znode.Engine.Api.Models
{
    public class TaxClassSKUModel : BaseModel
    {
        public int? TaxClassSKUId { get; set; }
        public int? TaxClassId { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public string SKUs { get; set; }
        public string ProductName { get; set; }
        public int? PimProductId { get; set; }
    }
}
