namespace Znode.Engine.Api.Models
{
    public class WebStoreWidgetCategoryModel : BaseModel
    {
        public int WidgetCategoryId { get; set; }
        public int ZnodeCategoryId { get; set; }
        public int MappingId { get; set; }
        public int PortalId { get; set; }
        public string WidgetsKey { get; set; }
        public string TypeOFMapping { get; set; }
        public int? DisplayOrder { get; set; }
        public PublishCategoryModel PublishCategoryModel { get; set; }
        public string CategoryCode { get; set; }
    }
}
