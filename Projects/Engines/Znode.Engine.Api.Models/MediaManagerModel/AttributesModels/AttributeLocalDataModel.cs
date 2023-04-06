namespace Znode.Engine.Api.Models
{
    public class AttributeLocalDataModel : BaseModel
    {
        public int MediaAttributeLocaleId { get; set; }
        public int? MediaAttributeId { get; set; }
        public int? LocaleId { get; set; }
        public string AttributeName { get; set; }
        public string Description { get; set; }
    }
}
