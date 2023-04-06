using System;
using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class CatalogAssociateCategoryViewModel : BaseViewModel
    {
        public int PimCatalogId { get; set; }
        public int ParentPimCategoryHierarchyId { get; set; }
        public int PimCategoryHierarchyId { get; set; }
        public int PimCategoryId { get; set; }
        public int LocaleId { get; set; }

        //These properties are used to change the display order from Tree of categories associated to catalog 
        public bool IsMoveUp { get; set; }
        public bool IsMoveDown { get; set; }
        public int CategoryId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelDisplayOrder, ResourceType = typeof(Admin_Resources))]
        [RegularExpression(AdminConstants.WholeNoRegularExpression, ErrorMessageResourceName = ZnodePIM_Resources.InvalidDisplayOrder, ErrorMessageResourceType = typeof(PIM_Resources))]
        public int DisplayOrder { get; set; } = 999;
        public string CategoryValue { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelIsActive, ResourceType = typeof(Admin_Resources))]
        public bool IsActive { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelActivationDate, ResourceType = typeof(Admin_Resources))]
        public DateTime? ActivationDate { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelExpirationDate, ResourceType = typeof(Admin_Resources))]
        public DateTime? ExpirationDate { get; set; }

        public string CategoryName { get; set; }
        public bool Status { get; set; }
        public string CategoryImage { get; set; }
        public string AttributeFamilyName { get; set; }
    }
}