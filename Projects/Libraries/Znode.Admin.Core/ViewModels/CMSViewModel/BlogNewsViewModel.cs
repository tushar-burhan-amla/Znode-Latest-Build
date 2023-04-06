using System;
using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class BlogNewsViewModel : BaseViewModel
    {
        public int BlogNewsId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelStore, ResourceType = typeof(Admin_Resources))]
        public int PortalId { get; set; }
        public string StoreName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelLocale, ResourceType = typeof(Admin_Resources))]
        public int LocaleId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelContentPage, ResourceType = typeof(Admin_Resources))]
        public int? CMSContentPagesId { get; set; }
        public string PageName { get; set; }

        public int? MediaId { get; set; }
        public int BlogNewsLocaleId { get; set; }
        public int CMSSEODetailId { get; set; }
        public int CMSSEODetailLocaleId { get; set; }
        public int? SEOId { get; set; }
        public int? CMSSEOTypeId { get; set; }
        public int? BlogNewsContentId { get; set; }
        public string CountComments { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelIsActive, ResourceType = typeof(Admin_Resources))]
        public bool IsBlogNewsActive { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelIsAllowGuestComment, ResourceType = typeof(Admin_Resources))]
        public bool IsAllowGuestComment { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelType, ResourceType = typeof(Admin_Resources))]
        public string BlogNewsType { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelType, ResourceType = typeof(Admin_Resources))]
        public BlogNewsType BlogNewsTypeValue { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelTitle, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.BannerTitleRequiredMessage)]
        [MaxLength(300, ErrorMessageResourceName = ZnodeAdmin_Resources.BlogNewsTitleLengthErrorMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string BlogNewsTitle { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelBodyOverview, ResourceType = typeof(Admin_Resources))]
        public string BodyOverview { get; set; }

        [MaxLength(300, ErrorMessageResourceName = ZnodeAdmin_Resources.BlogNewsTagsLengthErrorMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.LabelTags, ResourceType = typeof(Admin_Resources))]
        public string Tags { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelContentHtml, ResourceType = typeof(Admin_Resources))]
        public string BlogNewsContent { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelSEOTitle, ResourceType = typeof(Admin_Resources))]
        public string SEOTitle { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelSEODescription, ResourceType = typeof(Admin_Resources))]
        public string SEODescription { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelSEOKeywords, ResourceType = typeof(Admin_Resources))]
        public string SEOKeywords { get; set; }

        [RegularExpression(AdminConstants.SEOUrlValidation, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorValidSEOUrl)]
        [Display(Name = ZnodeAdmin_Resources.LabelSEOFriendlyUrl, ResourceType = typeof(Admin_Resources))]
        public string SEOUrl { get; set; }

        public string MediaPath { get; set; } = "";

        [Display(Name = ZnodeAdmin_Resources.LabelActivationDate, ResourceType = typeof(Admin_Resources))]
        public DateTime? ActivationDate { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelExpirationDate, ResourceType = typeof(Admin_Resources))]
        public DateTime? ExpirationDate { get; set; }

        public string UseExistingContent { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelCode, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.BlogNewsCodeRequired)]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeAdmin_Resources.AlphanumericOnlyWithNoSpaces, ErrorMessageResourceType = typeof(Admin_Resources))]
        [MaxLength(300, ErrorMessageResourceName = ZnodeAdmin_Resources.BlogNewsTitleLengthErrorMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string BlogNewsCode { get; set; }
        public string SelectedTab { get; set; }
        public string TargetPublishState { get; set; }
        public bool TakeFromDraftFirst { get; set; }
        public string PublishStatus { get; set; }
        public int PublishStateId { get; set; }
    }
}
