using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class LinkWidgetConfigurationLocaleViewModel : BaseViewModel
    {
        public int CMSWidgetTitleConfigurationLocaleId { get; set; }
        public int CMSWidgetTitleConfigurationId { get; set; }      
        public int? MediaId { get; set; }
        public string MediaPath { get; set; }

        [Required]
        [MaxLength(200, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorTitleLength)]
        public new string Title { get; set; }

        [RegularExpression(AdminConstants.URLValidation, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidationUrl)]
        public string URL { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelLocale, ResourceType = typeof(Admin_Resources))]
        public int? LocaleId { get; set; }

        public new string TitleCode { get; set; }
    }
}
