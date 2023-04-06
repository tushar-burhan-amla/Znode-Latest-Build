using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class PIMAttributeFamilyViewModel : BaseViewModel
    {
        public int PimAttributeFamilyId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodePIM_Resources.RequiredFamilyCode, ErrorMessageResourceType = typeof(PIM_Resources))]
        [RegularExpression(@"^[A-Za-z][a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeAdmin_Resources.AlphanumericStartWithAlphabet, ErrorMessageResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.AttributeFamilyCodeLength, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string FamilyCode { get; set; }
        public string AttributeFamilyName { get; set; }
        public int ExistingAttributeFamilyId { get; set; }
        public string FamilyLocale { get; set; }
        public bool IsDefaultFamily { get; set; }
        public bool IsSystemDefined { get; set; }
        public bool IsCategory { get; set; }
    }
}