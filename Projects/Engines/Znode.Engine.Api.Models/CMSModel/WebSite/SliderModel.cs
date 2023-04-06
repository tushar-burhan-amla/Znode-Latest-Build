namespace Znode.Engine.Api.Models
{
    public class SliderModel : BaseModel
    {
        public int CMSSliderId { get; set; }
        public string Name { get; set; }
        public bool IsWidgetAssociated { get; set; }
        public string PublishStatus { get; set; }
        public bool? IsPublished { get; set; }
        public byte PublishStateId { get; set; }
    }
}
