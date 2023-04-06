using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Models
{
    public class CMSWidgetProductModel : BaseModel
    {
        public int? PublishProductId { get; set; }
        public int? LocaleId { get; set; }
        public int CMSWidgetsId { get; set; }
        public int CMSMappingId { get; set; }
        public int ProductId { get; set; }
        public int CMSWidgetProductId { get; set; }
        public int? DisplayOrder { get; set; }= ZnodeConstant.WidgetItemDisplayOrder;
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string SKU { get; set; }
        public string WidgetsKey { get; set; }
        public string TypeOfMapping { get; set; }
    }
}
