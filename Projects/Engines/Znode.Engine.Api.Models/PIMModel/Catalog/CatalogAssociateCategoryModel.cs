using System;

namespace Znode.Engine.Api.Models
{
    public class CatalogAssociateCategoryModel : BaseModel
    {
        public int PimCategoryHierarchyId { get; set; }
        public int? PimCatalogId { get; set; }
        public int? ParentPimCategoryHierarchyId { get; set; }
        public int? PimParentCategoryId { get; set; }
        public int? PimCategoryId { get; set; }
        public int LocaleId { get; set; }
        public int ProfileCatalogId { get; set; }
        public int ProfileId { get; set; }

        //These properties are used to change the display order from Tree of categories associated to catalog 
        public bool IsMoveUp { get; set; }
        public bool IsMoveDown { get; set; }
        public int CategoryId { get; set; }

        public int? DisplayOrder { get; set; }
        public string CategoryValue { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string CategoryName { get; set; }
        public bool Status { get; set; }
        public string CategoryImage { get; set; }
        public string AttributeFamilyName { get; set; }

        public bool UpdateDisplayOrder { get; set; }
    }
}
