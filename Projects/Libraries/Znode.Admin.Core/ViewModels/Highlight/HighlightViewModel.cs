using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class HighlightViewModel : BaseViewModel
    {
        public int HighlightId { get; set; }
        public int HighlightLocaleId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelImage, ResourceType = typeof(Admin_Resources))]
        public int? MediaId { get; set; }
        public string MediaPath { get; set; }
        public bool DisplayPopup { get; set; } = true;
        public string HighlightName { get; set; }   

        [Display(Name = ZnodeAdmin_Resources.LabelDescription, ResourceType = typeof(Admin_Resources))]
        [AllowHtml]
        [UIHint("RichTextBox")]
        public string Description { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelHyperlink, ResourceType = typeof(Admin_Resources))]
        [RegularExpression(AdminConstants.HyperlinkValidation, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidationUrl)]
        public string Hyperlink { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelHighlightType, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.HighlightTypeRequiredMessage)]
        public int HighlightTypeId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelIsActive, ResourceType = typeof(Admin_Resources))]
        public bool IsActive { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelDisplayOrder, ResourceType = typeof(Admin_Resources))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeAdmin_Resources.PleaseEnterDisplayOrder, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Range(1, 999, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorRangeBetween)]
        public int DisplayOrder { get; set; } = 99;

        [Display(Name = ZnodeAdmin_Resources.LabelImageAltText, ResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ImageAltTextLengthErrorMessage)]
        public string ImageAltTag { get; set; }        

        [Display(Name = ZnodeAdmin_Resources.LabelShortDescription, ResourceType = typeof(Admin_Resources))]
        [AllowHtml]
        [UIHint("RichTextBox")]
        public string ShortDescription { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelLocale, ResourceType = typeof(Admin_Resources))]
        public int LocaleId { get; set; } = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
        public List<SelectListItem> Locale { get; set; }
        public string HighlightType { get; set; }
        public List<SelectListItem> HighlightTypeList { get; set; }
        public bool IsHyperlink { get; set; }
        public string IsDescription { get; set; }
        public List<SelectListItem> HighlightCodeList { get; set; }
        public HighlightViewModel()
        {
            HighlightCodeList = new List<SelectListItem>();
        }

        [Display(Name = ZnodeAdmin_Resources.LabelHighlightName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField)]
        [MaxLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.HighlightNameLengthErrorMessage)]       
        public string HighlightCode { get; set; }
        public string TemplateTokens { get; set; }
        public string TemplateTokensPartOne { get; set; }
        public string TemplateTokensPartTwo { get; set; }
    }
}