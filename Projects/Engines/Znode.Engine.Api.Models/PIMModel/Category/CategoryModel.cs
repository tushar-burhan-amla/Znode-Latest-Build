namespace Znode.Engine.Api.Models
{
    public class CategoryModel : BaseModel
    {
        public int PimCategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool Status { get; set; }
        public string CategoryImage { get; set; }
        public int CMSOfferPageCategoryId { get; set; }
        public int CMSContentPagesId { get; set; }
        public int PublishCategoryId { get; set; }
        public string AttributeFamilyName { get; set; }
        public int  CMSWidgetCategoryId { get; set; }
        public int CMSWidgetsId { get; set; }
        public string WidgetsKey { get; set; }
        public int CMSMappingId { get; set; }
        public string TypeOFMapping { get; set; }
        public int ZnodeCategoryId { get; set; }
        public int? DisplayOrder { get; set; }
        public string CategoryCode { get; set; }
    }
}
