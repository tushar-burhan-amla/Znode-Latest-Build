namespace Znode.Engine.Api.Models
{
    public class AttributeGroupLocaleModel : BaseModel
    {
        public int MediaAttributeGroupLocaleId { get; set; }
        public int MediaAttributeGroupId { get; set; }
        public int? LocaleId { get; set; }
        public string AttributeGroupName { get; set; }
        public string Description { get; set; }
        public int? MediaCategoryId { get; set; }
    }
}
