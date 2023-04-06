using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class BannerViewModel : BaseViewModel
    {
        public int CMSSliderBannerId { get; set; }
        public int CMSSliderId { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelLocale, ResourceType = typeof(Admin_Resources))]
        public int LocaleId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.TextRequiredImage)]
        public int? MediaId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelBannerTitle, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField)]
        [MaxLength(500, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorBannerTitle)]
        public new string Title { get; set; }

        public string ImageAlternateText { get; set; }

        [MaxLength(500, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorButtonLabelName)]
        [Display(Name = ZnodeAdmin_Resources.LabelButtonLabelName, ResourceType = typeof(Admin_Resources))]
        public string ButtonLabelName { get; set; }

        public string ButtonLink { get; set; }

        public string TextAlignment { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelBannerSequence, ResourceType = typeof(Admin_Resources))]
        [Range(1, 99999, ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorBannerSequence, ErrorMessageResourceType = typeof(Admin_Resources))]
        [RegularExpression(AdminConstants.WholeNoRegularExpression, ErrorMessageResourceName = ZnodeAdmin_Resources.InvalidBannerSequence, ErrorMessageResourceType = typeof(Admin_Resources))]
        public int? BannerSequence { get; set; }

        [AllowHtml]
        [UIHint("RichTextBox")]
        public string Description { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelActivationDate, ResourceType = typeof(Admin_Resources))]
        public DateTime? ActivationDate { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelExpirationDate, ResourceType = typeof(Admin_Resources))]
        public DateTime? ExpirationDate { get; set; }

        public string MediaPath { get; set; } = "";
        public string Name { get; set; }
        public string FileName { get; set; }

        public List<SelectListItem> Locales { get; set; }
    }
}