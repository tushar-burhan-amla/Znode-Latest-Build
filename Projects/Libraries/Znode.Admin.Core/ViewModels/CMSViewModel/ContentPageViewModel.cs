using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class ContentPageViewModel : BaseViewModel
    {

        public ContentPageViewModel()
        {
            ProfileList = new List<SelectListItem>();
        }

        public int CMSContentPagesId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelStore, ResourceType = typeof(Admin_Resources))]
        public int PortalId { get; set; }
        public string StoreName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.TitleProfile, ResourceType = typeof(Admin_Resources))]
        public string[] ProfileIds { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelLocale, ResourceType = typeof(Admin_Resources))]
        public int LocaleId { get; set; }

        public string Tree { get; set; }

        public int CMSContentPageGroupId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.TitlePageTitle, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredMessagePageTitle)]
        public string PageTitle { get; set; }

        [MaxLength(50)]
        [Display(Name = ZnodeAdmin_Resources.TitlePageName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredMessagePageName)]
        public string PageName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelSEOTitle, ResourceType = typeof(Admin_Resources))]
        public string SEOTitle { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelSEODescription, ResourceType = typeof(Admin_Resources))]
        public string SEODescription { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelSEOKeywords, ResourceType = typeof(Admin_Resources))]
        public string SEOKeywords { get; set; }

        [RegularExpression(AdminConstants.SEOUrlValidation, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorValidSEOUrl)]
        [Display(Name = ZnodeAdmin_Resources.LabelSEOFriendlyUrl, ResourceType = typeof(Admin_Resources))]
        public string SEOUrl { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelMetaInformation, ResourceType = typeof(Admin_Resources))]
        public string MetaInformation { get; set; }
        public string PortalName { get; set; }
        public string PageTemplateName { get; set; }
        public string PageTemplateFileName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelStaticPageHtml, ResourceType = typeof(Admin_Resources))]
        [AllowHtml]
        [UIHint("RichTextBox")]
        public string ContentPageHtml { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelIsRedirect, ResourceType = typeof(Admin_Resources))]
        public bool IsRedirect { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelIsActive, ResourceType = typeof(Admin_Resources))]
        public bool IsActive { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorSelectTemplate)]
        [Display(Name = ZnodeAdmin_Resources.LabelTemplateName, ResourceType = typeof(Admin_Resources))]
        public int CMSTemplateId { get; set; }

        public List<SelectListItem> TemplateList { get; set; }
        public List<TemplateViewModel> TemplateImageList { get; set; }        
        public List<SelectListItem> PortalList { get; set; }
        public List<SelectListItem> ProfileList { get; set; }
        public List<SelectListItem> Locales { get; set; }

        public bool IsConfigurable { get; set; }
        public string ContentPageTemplateCode { get; set; }
        public string TemplatePath { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelActivationDate, ResourceType = typeof(Admin_Resources))]
        public DateTime? ActivationDate { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelExpirationDate, ResourceType = typeof(Admin_Resources))]
        public DateTime? ExpirationDate { get; set; }

        public string ItemName { get; set; }
        public int ItemId { get; set; }
        public string FileName { get; set; }
        public string CMSContentPageGroupName { get; set; }
        public CMSWidgetsListViewModel Widgets { get; set; }
        public int FolderId { get; set; }
        public string OldSEOURL { get; set; }
        public bool IsSelectAllProfile { get; set; }
        public string PublishStatus { get; set; }
        public string SEOPublishStatus { get; set; }
        public bool IsPublished { get; set; }

        public string SEOCode { get; set; }
        public string TargetPublishState { get; set; }
        public bool TakeFromDraftFirst { get; set; }
        public string StorePublishStatus { get; set; }
        public string PreviewUrl { get; set; }
        public bool IsPreviewGloballyEnabled { get; set; }
        public bool StorePublished { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelCanonicalURL, ResourceType = typeof(Admin_Resources))]
        public string CanonicalURL { get; set; }
        public string RobotTagValue { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelRobotTag, ResourceType = typeof(Admin_Resources))]
        public RobotTag RobotTag { get; set; }
        public string MediaPath { get; set; }
    }
}