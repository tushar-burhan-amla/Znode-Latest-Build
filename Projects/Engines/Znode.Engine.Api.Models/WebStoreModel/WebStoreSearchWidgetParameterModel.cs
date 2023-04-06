namespace Znode.Engine.Api.Models
{
    public class WebStoreSearchWidgetParameterModel : SearchRequestModel
    {
        public int CMSMappingId { get; set; }
        public string WidgetCode { get; set; }
        public string WidgetKey { get; set; }
        public string TypeOfMapping { get; set; }
        public string DisplayName { get; set; }
        public int? ProductProfileId { get; set; }
    }
}
