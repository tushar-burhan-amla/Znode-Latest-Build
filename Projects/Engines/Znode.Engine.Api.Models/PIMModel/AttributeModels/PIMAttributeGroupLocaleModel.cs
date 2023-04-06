namespace Znode.Engine.Api.Models
{
    public class PIMAttributeGroupLocaleModel : BaseModel
    {
        public int PIMAttributeGroupLocaleId { get; set; }
        public int PIMAttributeGroupId { get; set; }
        public int? LocaleId { get; set; }
        public string AttributeGroupName { get; set; }
        public string Description { get; set; }
    }
}
