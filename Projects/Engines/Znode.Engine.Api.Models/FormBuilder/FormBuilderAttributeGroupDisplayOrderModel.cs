namespace Znode.Engine.Api.Models
{
    public class FormBuilderAttributeGroupDisplayOrderModel : BaseModel
    {
        public int FormBuilderId { get; set; }
        public int? GroupId { get; set; }
        public int? AttributeId { get; set; }
        public bool IsNavigateUpward { get; set; }
    }
}
