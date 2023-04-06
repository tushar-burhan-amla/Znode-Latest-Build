using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class CatalogAssociationViewModel : BaseViewModel
    {
        public int PimCategoryHierarchyId { get; set; }
        public int CatalogId { get; set; }
        public int CategoryId { get; set; }
        public int LocaleId { get; set; }
        public string PimCategoryHierarchyIds { get; set; }
        public string CategoryIds { get; set; }
        public string CatalogName { get; set; }
        public string ProductIds { get; set; }
        public string ProfileCatalogCategoryIds { get; set; }
        public int ProductId { get; set; }
        public int? ProfileCatalogId { get; set; }
        public int? ProfileId { get; set; }
        public int PortalId { get; set; }

        public string Tree { get; set; }

        public List<SelectListItem> UnAssociatedProductList { get; set; }
        public List<SelectListItem> UnAssociatedCategoryList { get; set; }
        public List<SelectListItem> PortalList { get; set; }

        public List<ProductDetailsViewModel> AssociatedProducts { get; set; }

        public GridModel GridModel { get; set; }

        public int DisplayOrder { get; set; } = 999;
        public bool IsActive { get; set; } = true;
    }
}