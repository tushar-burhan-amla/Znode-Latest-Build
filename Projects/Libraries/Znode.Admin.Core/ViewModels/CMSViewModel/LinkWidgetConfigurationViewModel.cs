using System;
using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class LinkWidgetConfigurationViewModel : BaseViewModel
    {
        public int CMSWidgetTitleConfigurationId { get; set; }
        public int? CMSWidgetsId { get; set; }
        public int CMSMappingId { get; set; }
        public string WidgetsKey { get; set; }
        public string TypeOfMapping { get; set; }
        public string DisplayName { get; set; }
        public string WidgetName { get; set; }
        public string FileName { get; set; }
        public int PortalId { get; set; }
        public int? MediaId { get; set; }
        public string MediaPath { get; set; }

        [Required]
        [MaxLength(200, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorTitleLength)]
        public new string Title { get; set; }

        [RegularExpression(AdminConstants.URLValidation, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidationUrl)]
        public string URL { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelLocale, ResourceType = typeof(Admin_Resources))]
        public int? LocaleId { get; set; }

        public string TitleCode { get; set; }

        public int CMSWidgetTitleConfigurationLocaleId { get; set; }

        public bool IsNewTab { get; set; }

        [RegularExpression(AdminConstants.NumberExpression, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.DisplayOrderMustBeNumber)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeAdmin_Resources.PleaseEnterDisplayOrder, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Range(1, 999, ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.ErrorDisplayOrder)]
        public int DisplayOrder { get; set; }
    }
}
