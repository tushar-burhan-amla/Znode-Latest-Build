﻿namespace Znode.Engine.Api.Models
{
    public class CMSWidgetProductCategoryModel : BaseModel
    {
        public int? PublishProductId { get; set; }
        public int PublishCategoryId { get; set; }
        public int? LocaleId { get; set; }
        public int CMSWidgetsId { get; set; }
        public int CMSMappingId { get; set; }
        public int ProductId { get; set; }
        public int CMSWidgetProductId { get; set; }
        public int CMSWidgetCategoryId { get; set; }

        public string ProductName { get; set; }
        public string SKU { get; set; }
        public string WidgetsKey { get; set; }
        public string TypeOfMapping { get; set; }
        public string CategoryName { get; set; }
        public string ProductType { get; set; }
        public string ImagePath { get; set; }
        public int? DisplayOrder { get; set; }
        public string CategoryCode { get; set; }
    }
}
