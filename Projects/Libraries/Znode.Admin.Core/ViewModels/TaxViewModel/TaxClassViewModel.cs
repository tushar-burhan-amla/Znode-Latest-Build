using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class TaxClassViewModel : BaseViewModel
    {
        public string PortalName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelTaxClassName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField)]
        [MaxLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.TaxClassNameLengthErrorMessage)]
        public string Name { get; set; }
        public string ExternalId { get; set; }

        public int TaxClassId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelDisplayOrder, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField)]
        [RegularExpression(AdminConstants.NumberValidation, ErrorMessageResourceName = ZnodeAdmin_Resources.DisplayOrderMustBeNumber, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Range(1, 999999999, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.InvalidDisplayOrder)]
        public int DisplayOrder { get; set; } = 99;
        public int PortalId { get; set; }

        public bool IsActive { get; set; }

        public List<SelectListItem> TaxClasses { get; set; }

        public TaxClassSKUViewModel TaxClassSKU { get; set; }
        public TaxRuleViewModel TaxRule { get; set; }

        public HttpPostedFileBase ImportFile { get; set; }

        public string ImportedSKUs { get; set; }
        public bool IsDefault { get; set; }
    }
}