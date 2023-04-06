namespace Znode.Engine.Api.Models
{
    public class WebStoreWidgetBrandModel : BaseModel
    {
        public int WidgetBrandId { get; set; }
        public int BrandId { get; set; }
        public int MappingId { get; set; }
        public int PortalId { get; set; }
        public string WidgetsKey { get; set; }
        public string TypeOFMapping { get; set; }
        public BrandModel BrandModel { get; set; }
    }
}