using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class PortalSEOSettingViewModel : BaseViewModel
    {
        public int CMSPortalSEOSettingId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorSelectStore)]
        [Display(Name = ZnodeAdmin_Resources.LabelStoreName, ResourceType = typeof(Admin_Resources))]
        public int PortalId { get; set; }

        public string StoreName { get; set; }
        [AllowHtml]
        [Display(Name = ZnodeAdmin_Resources.LabelSEOCategoryTitle, ResourceType = typeof(Admin_Resources))]
        public string CategoryTitle { get; set; }

        [AllowHtml]
        [Display(Name = ZnodeAdmin_Resources.LabelSEOCategoryDescription, ResourceType = typeof(Admin_Resources))]
        public string CategoryDescription { get; set; }

        [AllowHtml]
        [Display(Name = ZnodeAdmin_Resources.LabelSEOCategoryKeyword, ResourceType = typeof(Admin_Resources))]
        public string CategoryKeyword { get; set; }

        [AllowHtml]
        [Display(Name = ZnodeAdmin_Resources.LabelProductTitle, ResourceType = typeof(Admin_Resources))]
        public string ProductTitle { get; set; }

        public string ProductName { get; set; }

        [AllowHtml]
        [Display(Name = ZnodeAdmin_Resources.LabelSEOProductDescription, ResourceType = typeof(Admin_Resources))]
        public string ProductDescription { get; set; }

        [AllowHtml]
        [Display(Name = ZnodeAdmin_Resources.LabelSEOProductKeyword, ResourceType = typeof(Admin_Resources))]
        public string ProductKeyword { get; set; }

        [AllowHtml]
        [Display(Name = ZnodeAdmin_Resources.LabelSEOContentTitle, ResourceType = typeof(Admin_Resources))]
        public string ContentTitle { get; set; }

        [AllowHtml]
        [Display(Name = ZnodeAdmin_Resources.LabelSEOContentDescription, ResourceType = typeof(Admin_Resources))]
        public string ContentDescription { get; set; }

        [AllowHtml]
        [Display(Name = ZnodeAdmin_Resources.LabelSEOContentKeyword, ResourceType = typeof(Admin_Resources))]
        public string ContentKeyword { get; set; }

        public List<SelectListItem> PortalList { get; set; }

        public PortalSEOSettingViewModel()
        {
            PortalList = new List<SelectListItem>();
        }
    }
}