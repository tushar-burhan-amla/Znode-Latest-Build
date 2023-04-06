namespace Znode.Engine.Api.Models
{
    public class TreeModel : BaseModel
    {
        public int PimCategoryHierarchyId { get; set; }
        public int? PimCatalogId { get; set; }
        public int? PimCategoryId { get; set; }
        public string CategoryValue { get; set; }
        public int? ParentPimCategoryHierarchyId { get; set; }

        public int? PimParentCategoryId { get; set; }
        public int DisplayOrder { get; set; }
        public string CatalogName { get; set; }
    }
}
