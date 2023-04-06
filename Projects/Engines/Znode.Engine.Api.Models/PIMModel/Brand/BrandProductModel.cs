namespace Znode.Engine.Api.Models
{
    public class BrandProductModel : BaseModel
    {
        public string ProductIds { get; set; }
        public int BrandId { get; set; }
        public string AttributeCode { get; set; }
        public string AttributeValue { get; set; }
        public bool IsUnAssociated { get; set; }
    }
}
