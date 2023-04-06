namespace Znode.Engine.Api.Models
{
    public class PIMProductAttributeValuesLocalModel : BaseModel  
    {
        public int ZnodePimAttributeValueLocaleId { get; set; }
        public int? PimAttributeValueId { get; set; }
        public int? LocaleId { get; set; }
        public string AttributeValue { get; set; }
    }
}
