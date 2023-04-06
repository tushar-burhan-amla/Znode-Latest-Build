namespace Znode.Engine.Api.Models
{
    public class GlobalAttributeGroupEntityModel : BaseModel
    {
        public int EntityId { get; set; }
        public int FormBuilderId { get; set; }
        public int LocaleId { get; set; }
        public string GroupIds { get; set; }
        public string AttributeIds { get; set; }
    }
}
