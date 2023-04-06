namespace Znode.Engine.Api.Models
{
    public class WebStoreWidgetFormParameters : BaseModel
    {
        public int CMSMappingId { get; set; }
        public int LocaleId { get; set; }
        public string FormCode { get; set; }
        public int FormBuilderId { get; set; }
        public string TypeOfMapping { get; set; }
        public string DisplayName { get; set; }
    }
}
