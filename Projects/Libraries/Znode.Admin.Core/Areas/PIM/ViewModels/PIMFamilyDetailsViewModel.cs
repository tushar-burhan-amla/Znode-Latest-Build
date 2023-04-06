using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class PIMFamilyDetailsViewModel : BaseViewModel
    {
        public int ProductId { get; set; }
        public int FamilyId { get; set; }
        public int LocaleId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        public string AssociatedProductIds { get; set; }
        public string ConfigureAttributeIds { get; set; }
        public List<PIMAttributeGroupViewModel> Groups { get; set; }
        public List<PIMProductAttributeValuesViewModel> Attributes { get; set; }
        [Display(Name = ZnodePIM_Resources.LabelAttributeFamily, ResourceType = typeof(PIM_Resources))]
        public List<SelectListItem> Families { get; set; }

        public int? PimCatalogId { get; set; }
        public int? PimParentCategoryId { get; set; }
        public int CopyProductId { get; set; }
        public int? PimCategoryHierarchyId { get; set; }

        public int? ProductPublishId { get; set; }
         
    }
}