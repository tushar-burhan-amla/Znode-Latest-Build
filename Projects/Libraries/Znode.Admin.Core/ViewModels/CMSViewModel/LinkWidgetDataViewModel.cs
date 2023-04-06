using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class LinkWidgetDataViewModel : BaseViewModel
    {
        public List<LinkWidgetConfigurationViewModel> LinkWidgetConfigurationList { get; set; }
        public GridModel GridModel { get; set; }
        public LinkWidgetConfigurationViewModel LinkWidgetConfiguration { get; set; }
        public int? CMSWidgetTitleConfigurationId { get; set; }
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

        [Display(Name = ZnodeAdmin_Resources.LabelLocale, ResourceType = typeof(Admin_Resources))]
        public int LocaleId { get; set; }
        [Required]
        [MaxLength(200, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorTitleLength)]
        public new string Title { get; set; }

        [RegularExpression(AdminConstants.URLValidation, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidationUrl)]
        public string Url { get; set; }

        public List<LinkWidgetConfigurationLocaleViewModel> LinkWidgetConfigurationLocaleList { get; set; }

        public LinkWidgetConfigurationLocaleViewModel LinkWidgetConfigurationLocale { get; set; }

        public string TitleCode { get; set; }

        public int? CMSWidgetTitleConfigurationLocaleId { get; set; }

        public bool IsNewTab { get; set; }

        public int DisplayOrder { get; set; }

        public bool EnableCMSPreview { get; set; }
    }
}