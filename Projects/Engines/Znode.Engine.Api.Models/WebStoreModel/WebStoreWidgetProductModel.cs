namespace Znode.Engine.Api.Models
{
    public class WebStoreWidgetProductModel : BaseModel
    {
        public int WidgetsId { get; set; }
        public int MappingId { get; set; }
        public int ZnodeProductId { get; set; }
        public int WidgetProductId { get; set; }

        public string ProductName { get; set; }
        public string WidgetsKey { get; set; }
        public string TypeOfMapping { get; set; }
        public int? DisplayOrder { get; set; }
        public string ProductType { get; set; }
        public WebStoreProductModel WebStoreProductModel { get; set; }
    }
}
