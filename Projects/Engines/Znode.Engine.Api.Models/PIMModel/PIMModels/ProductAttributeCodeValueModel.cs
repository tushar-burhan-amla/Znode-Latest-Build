namespace Znode.Engine.Api.Models
{
    public class ProductAttributeCodeValueModel : BaseModel
    {
        public string SKU { get; set; }
        public string AttributeCode { get; set; }
        public string AttributeValue { get; set; }
    }
}
