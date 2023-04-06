namespace Znode.Engine.Api.Models
{
    public class SKUPriceDeleteModel : BaseModel
    {
        public string PriceId { get; set; }
        public int PriceListId { get; set; }
        public int PimProductId { get; set; }    
    }
}
