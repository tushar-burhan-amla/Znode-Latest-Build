namespace Znode.Engine.Api.Models
{
    public class GlobalAttributeGroupLocaleModel : BaseModel
    {
        public int GlobalAttributeGroupLocaleId { get; set; }
        public int GlobalAttributeGroupId { get; set; }
        public int? LocaleId { get; set; }
        public string AttributeGroupName { get; set; }
        public string Description { get; set; }
    }
}
