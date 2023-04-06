using System.ComponentModel.DataAnnotations;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Models
{
    public class CatalogAssociationModel : BaseModel
    {
        [Required]
        public int PimCategoryHierarchyId { get; set; }
        [Required]
        public int CatalogId { get; set; }
        public int? CategoryId { get; set; }
        public int? ProductId { get; set; }
        public int LocaleId { get; set; }
        public string CategoryIds { get; set; }
        public string PimCategoryHierarchyIds { get; set; }
        public string ProductIds { get; set; }
        public bool DisplayProducts { get; set; }
        public bool IsAssociated { get; set; }
        public int ProfileCatalogId { get; set; }
        public string ProfileCatalogCategoryIds { get; set; }
        public int DisplayOrder { get; set; } = ZnodeConstant.DisplayOrder;
        public bool IsActive { get; set; }
    }
}
