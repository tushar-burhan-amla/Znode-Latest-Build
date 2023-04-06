namespace Znode.Engine.Api.Models
{
    public class OrderAttributeModel : BaseModel
    {
        public int OmsOrderLineItemsId { get; set; }
        public string AttributeCode { get; set; }
        public string AttributeValue { get; set; }
        public string AttributeValueCode { get; set; }
    }
}
