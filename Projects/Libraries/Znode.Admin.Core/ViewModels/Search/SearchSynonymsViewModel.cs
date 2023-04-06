using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class SearchSynonymsViewModel : BaseViewModel
    {
        public int SearchSynonymsId { get; set; }
        public int PublishCatalogId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField)]
        [Display(Name = ZnodeAdmin_Resources.LabelOriginalTerm, ResourceType = typeof(Admin_Resources))]
        public string OriginalTerm { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField)]
        [Display(Name = ZnodeAdmin_Resources.LabelReplacedBy, ResourceType = typeof(Admin_Resources))]
        public string ReplacedBy { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelIsBidirectional, ResourceType = typeof(Admin_Resources))]
        public bool IsBidirectional { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelSynonymCode, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField)]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeAdmin_Resources.AlphanumericOnlyWithNoSpaces, ErrorMessageResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.SynonymCodeMaxLength, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string SynonymCode { get; set; }
    }
}
