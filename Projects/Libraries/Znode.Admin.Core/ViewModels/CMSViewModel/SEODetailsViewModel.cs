using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class SEODetailsViewModel : BaseViewModel
    {
        public string ItemName { get; set; }
        public int CMSSEODetailId { get; set; }
        public int CMSSEOTypeId { get; set; }
        public string SEOTypeName { get; set; }
        public int? SEOId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelSEOTitle, ResourceType = typeof(Admin_Resources))]
        public string SEOTitle { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelSEODescription, ResourceType = typeof(Admin_Resources))]
        public string SEODescription { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelSEOKeywords, ResourceType = typeof(Admin_Resources))]
        public string SEOKeywords { get; set; }

        [RegularExpression(AdminConstants.SEOUrlValidation, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorValidSEOUrl)]
        [MaxLength(1000, ErrorMessageResourceName = ZnodeAdmin_Resources.Errorlength, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.LabelSEOUrl, ResourceType = typeof(Admin_Resources))]
        public string SEOUrl { get; set; }

        public string OldSEOURL { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelIsRedirect, ResourceType = typeof(Admin_Resources))]
        public bool? IsRedirect { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelMetaInformation, ResourceType = typeof(Admin_Resources))]
        public string MetaInformation { get; set; }

        public List<SelectListItem> Locales { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelLocale, ResourceType = typeof(Admin_Resources))]
        public int LocaleId { get; set; }
        public int PortalId { get; set; }
        public List<SelectListItem> Portals { get; set; }
        public bool IsAllStore { get; set; }
        public int? PimProductId { get; set; }
        public string SEOCode { get; set; }
        public string TargetPublishState { get; set; }
        public bool TakeFromDraftFirst { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelCanonicalURL, ResourceType = typeof(Admin_Resources))]
        public string CanonicalURL { get; set; }
        public string RobotTagValue { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelRobotTag, ResourceType = typeof(Admin_Resources))]
        public RobotTag RobotTag { get; set; }

    }
}