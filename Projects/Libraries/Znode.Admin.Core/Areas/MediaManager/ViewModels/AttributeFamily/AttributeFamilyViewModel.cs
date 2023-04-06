using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class AttributeFamilyViewModel : BaseViewModel
    {
        public int MediaAttributeFamilyId { get; set; }
        public int ExistingAttributeFamilyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = "RequiredFamilyCode")]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.AttributeFamilyCodeLength, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string FamilyCode { get; set; }
        public string FamilyLocale { get; set; }
        public string AttributeFamilyName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelIsDefaultFamily, ResourceType = typeof(Admin_Resources))]
        public bool IsDefaultFamily { get; set; }
        public bool IsSystemDefined { get; set; }
        public NavigationViewModel NavigationViewModel { get; set; }

    }
}