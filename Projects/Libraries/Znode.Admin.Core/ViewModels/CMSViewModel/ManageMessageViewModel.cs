using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class ManageMessageViewModel : BaseViewModel
    {
        public int CMSMessageId { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelLocale, ResourceType = typeof(Admin_Resources))]
        public int LocaleId { get; set; }
        public int CMSMessageKeyId { get; set; }
        public int CMSPortalMessageId { get; set; }
       
        [Display(Name = ZnodeAdmin_Resources.LabelSelectStore, ResourceType = typeof(Admin_Resources))]
        public List<SelectListItem> Portals { get; set; }
        
        public string[] PortalIds { get; set; }
        public int PortalId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelMessageDescription, ResourceType = typeof(Admin_Resources))]
        [AllowHtml]
        [UIHint("RichTextBox")]
        public string Message { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelMessageKey, ResourceType = typeof(Admin_Resources))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField, ErrorMessageResourceType = typeof(Admin_Resources))]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeAdmin_Resources.AlphaNumericOnly, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string MessageKey { get; set; }

        public string StoreName { get; set; }
        public string Location { get; set; }

        [MaxLength(1000)]
        [Display(Name = ZnodeAdmin_Resources.LabelTags, ResourceType = typeof(Admin_Resources))]
        public string MessageTag { get; set; }

        public string PublishStatus { get; set; }
        public bool IsPublished { get; set; }

        public List<SelectListItem> Locales { get; set; }
        public int? CMSPortalMessageKeyTagId { get; set; }
        public string TargetPublishState { get; set; }
        public bool TakeFromDraftFirst { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelIsGlobal, ResourceType = typeof(Admin_Resources))]
        public bool IsGlobal { get; set; }
        public string IsGlobalContentBlock { get; set; }

    }
}