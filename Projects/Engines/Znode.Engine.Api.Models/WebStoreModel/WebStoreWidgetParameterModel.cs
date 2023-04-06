namespace Znode.Engine.Api.Models
{
    public class WebStoreWidgetParameterModel : BaseModel
    {
        public int CMSMappingId { get; set; }
        public int LocaleId { get; set; }
        public int PortalId { get; set; }
        public string WidgetCode { get; set; }
        public string WidgetKey { get; set; }
        public string TypeOfMapping { get; set; }
        public string DisplayName { get; set; }
        public int? ProfileId { get; set; }
        public int PublishCatalogId { get; set; }
        public int? ProductProfileId { get; set; }
        public string CategoryIds { get; set; }
    }
}
