namespace Znode.Engine.Api.Models
{
    public class FormSubmitAttributeModel : BaseModel
    {
        public int GlobalAttributeId { get; set; }
        public int? GlobalAttributeValueId { get; set; }
        public int? GlobalAttributeDefaultValueId { get; set; }
        public string AttributeCode { get; set; }
        public string AttributeValue { get; set; }
        public int LocaleId { get; set; }
        public string AttributeName { get; set; }
    }
}
